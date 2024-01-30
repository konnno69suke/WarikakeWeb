
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Runtime.Intrinsics.X86;
using WarikakeWeb.Data;
using WarikakeWeb.Logic;
//using WarikakeWeb.Migrations;
using WarikakeWeb.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WarikakeWeb.Controllers
{
    public class CsvImportController : Controller
    {
        private readonly WarikakeWebContext _context;

        public CsvImportController(WarikakeWebContext context)
        {
            _context = context;
        }

        public IActionResult Import()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            CsvImport csvImport = new CsvImport();
            return View(csvImport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(CsvImport csvImport)
        {
            IFormFile file = csvImport.formFile;
            if (file != null && file.Length > 0)
            {
                int? GroupId = HttpContext.Session.GetInt32("GroupId");
                int? UserId = HttpContext.Session.GetInt32("UserId");
                if (GroupId == null)
                {
                    // セッション切れ
                    return RedirectToAction("Login", "Home");
                }
                Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

                string fileName = Path.GetFileName(file.FileName);
                string fileExtension = Path.GetExtension(fileName);

                if (fileExtension == ".csv")
                {
                    DateTime currDate = DateTime.Now;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName + currDate.ToString("_yyyyMMddHHmmss"));
                    
                    int impCnt = 0;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    using (TextFieldParser csvParser = new TextFieldParser(filePath))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = false;


                        while (!csvParser.EndOfData)
                        {
                            string[] fields = csvParser.ReadFields();

                            // TODO: Implement your logic here
                            if (14 != fields.Count())
                            {
                                continue;
                            }

                            CsvMigration csv = new CsvMigration();
                            csv.status = 0;
                            csv.inputDate = fields[0];
                            csv.buyDate = fields[1];
                            csv.kindName = fields[2];
                            csv.buyAmount = fields[3];
                            csv.pf1 = fields[4];
                            csv.pf2 = fields[5];
                            csv.pf3 = fields[6];
                            csv.pa1 = fields[7];
                            csv.pa2 = fields[8];
                            csv.pa3 = fields[9];
                            csv.pr1 = fields[10];
                            csv.pr2 = fields[11];
                            csv.pr3 = fields[12];
                            csv.buyStatus = fields[13];
                            csv.CreatedDate = currDate;
                            csv.CreateUser = UserId.ToString();
                            csv.CreatePg = "ImportInsert";
                            csv.UpdatedDate = currDate;
                            csv.UpdateUser = UserId.ToString();
                            csv.UpdatePg = "ImportInsert";
                            _context.CsvMigration.Add(csv);
                            impCnt++;
                        }
                    }
                    if(impCnt > 0)
                    {
                        ViewBag.impCnt = "データインポート件数は" + impCnt + "件です";
                    }
                    else
                    {
                        ViewBag.impCnt = "データはインポートされませんでした";
                    }

                    _context.SaveChanges();

                    return View(csvImport);
                }
                else
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }

            return View();
        }


        public ActionResult Migrate()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            int migCnt = _context.CsvMigration.Where(c => c.status == 0).Count();
            if(migCnt > 0)
            {
                ViewBag.MigCnt = "データ移行予定件数は" + migCnt + "件です。";
            }
            else
            {
                ViewBag.MigCnt = "データは移行されないようです。";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Migrate(CsvImport import)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            string format1 = "yyyy年MM月dd日HH時mm分ss秒";
            string format2 = "yyyyMMdd";
            DateTime currTime = DateTime.Now;

            string currPg = "ImportMigrate";
            int status = (int)statusEnum.移行;
            int migCnt = 0;
            int skpCnt = 0;
            ModelLogic modelLogic = new ModelLogic(_context);



            List<CsvMigration> csvList = _context.CsvMigration.Where(c => c.status == 0).ToList();
            foreach (var csv in csvList)
            {
                // 同一データ重複チェック
                TCost costChk = _context.TCost.Where(c => c.CostDate == DateTime.ParseExact(csv.buyDate, format2, null) && c.CreatedDate == DateTime.ParseExact(csv.inputDate, format1, null) && c.CostAmount == int.Parse(csv.buyAmount)).FirstOrDefault();
                if (costChk != null && costChk.CostStatus != (int)statusEnum.削除)
                {
                    csv.status = 9;
                    _context.CsvMigration.Update(csv);
                    _context.SaveChanges();

                    skpCnt++;
                    continue;
                }


                int PayId = 0;
                int RepayId = 0;

                TCost cost = new TCost();
                cost.status = status;
                cost.CostTitle = csv.kindName;
                cost.GroupId = (int)GroupId;
                cost.GenreId = getGenreId(csv.kindName);
                cost.GenreName = csv.kindName;
                cost.ProvisionFlg = 0;
                cost.CostAmount = int.Parse(csv.buyAmount);
                cost.CostDate = DateTime.ParseExact(csv.buyDate, format2, null);
                cost.CostStatus = getCostStatus(csv.buyStatus);
                cost.CreatedDate = DateTime.ParseExact(csv.inputDate, format1, null);
                cost.CreateUser = UserId.ToString();
                cost.CreatePg = currPg;
                cost.UpdatedDate = currTime;
                cost.UpdateUser = UserId.ToString();
                cost.UpdatePg = currPg;
                _context.TCost.Add(cost);

                _context.SaveChanges();
                TCost currCost = modelLogic.GetCurrentCost((int)UserId, currTime, currPg);

                try
                {
                    TPay pay1 = new TPay();
                    pay1.status = status;
                    pay1.PayId = PayId;
                    pay1.CostId = currCost.CostId;
                    pay1.UserId = 101;
                    int pf1cs = csv.pf1.IsNullOrEmpty() ? 0 : int.Parse(csv.pf1);
                    pay1.PayAmount = pf1cs;
                    pay1.CreatedDate = DateTime.ParseExact(csv.inputDate, format1, null);
                    pay1.CreateUser = UserId.ToString();
                    pay1.CreatePg = currPg;
                    pay1.UpdatedDate = currTime;
                    pay1.UpdateUser = UserId.ToString();
                    pay1.UpdatePg = currPg;
                    _context.TPay.Add(pay1);
                    PayId++;

                    TPay pay2 = new TPay();
                    pay2.status = status;
                    pay2.PayId = PayId;
                    pay2.CostId = currCost.CostId;
                    pay2.UserId = 102;
                    int pf2cs = csv.pf2.IsNullOrEmpty() ? 0 : int.Parse(csv.pf2);
                    pay2.PayAmount = pf2cs;
                    pay2.CreatedDate = DateTime.ParseExact(csv.inputDate, format1, null);
                    pay2.CreateUser = UserId.ToString();
                    pay2.CreatePg = currPg;
                    pay2.UpdatedDate = currTime;
                    pay2.UpdateUser = UserId.ToString();
                    pay2.UpdatePg = currPg;
                    _context.TPay.Add(pay2);
                    PayId++;

                    TPay pay3 = new TPay();
                    pay3.status = status;
                    pay3.PayId = PayId;
                    pay3.CostId = currCost.CostId;
                    pay3.UserId = 103;
                    int pf3cs = csv.pf3.IsNullOrEmpty() ? 0 : int.Parse(csv.pf3);
                    pay3.PayAmount = pf3cs;
                    pay3.CreatedDate = DateTime.ParseExact(csv.inputDate, format1, null);
                    pay3.CreateUser = UserId.ToString();
                    pay3.CreatePg = currPg;
                    pay3.UpdatedDate = currTime;
                    pay3.UpdateUser = UserId.ToString();
                    pay3.UpdatePg = currPg;
                    _context.TPay.Add(pay3);
                    PayId++;

                    TRepay rep = new TRepay();
                    rep.status = status;
                    rep.RepayId = RepayId;
                    rep.CostId = currCost.CostId;
                    rep.UserId = 101;
                    int pa1cv = csv.pa1.IsNullOrEmpty() ? 0 : int.Parse(csv.pa1);
                    int pr1cv = (csv.pr1.IsNullOrEmpty() || csv.pr1.Equals("null")) ? 0 : int.Parse(csv.pr1);
                    rep.RepayAmount = pa1cv + pr1cv;
                    rep.CreatedDate = DateTime.ParseExact(csv.inputDate, format1, null);
                    rep.CreateUser = UserId.ToString();
                    rep.CreatePg = currPg;
                    rep.UpdatedDate = currTime;
                    rep.UpdateUser = UserId.ToString();
                    rep.UpdatePg = currPg;
                    _context.TRepay.Add(rep);
                    RepayId++;

                    TRepay rep2 = new TRepay();
                    rep2.status = status;
                    rep2.RepayId = RepayId;
                    rep2.CostId = currCost.CostId;
                    rep2.UserId = 102;
                    int pa2cv = csv.pa2.IsNullOrEmpty() ? 0 : int.Parse(csv.pa2);
                    int pr2cv = (csv.pr2.IsNullOrEmpty() || csv.pr2.Equals("null")) ? 0 : int.Parse(csv.pr2);
                    rep2.RepayAmount = pa2cv + pr2cv;
                    rep2.CreatedDate = DateTime.ParseExact(csv.inputDate, format1, null);
                    rep2.CreateUser = UserId.ToString();
                    rep2.CreatePg = currPg;
                    rep2.UpdatedDate = currTime;
                    rep2.UpdateUser = UserId.ToString();
                    rep2.UpdatePg = currPg;
                    _context.TRepay.Add(rep2);
                    RepayId++;

                    TRepay rep3 = new TRepay();
                    rep3.status = status;
                    rep3.RepayId = RepayId;
                    rep3.CostId = currCost.CostId;
                    rep3.UserId = 103;
                    int pa3cv = csv.pa1.IsNullOrEmpty() ? 0 : int.Parse(csv.pa3);
                    int pr3cv = (csv.pr1.IsNullOrEmpty() || csv.pr3.Equals("null")) ? 0 : int.Parse(csv.pr3);
                    rep3.RepayAmount = pa3cv + pr3cv;
                    rep3.CreatedDate = DateTime.ParseExact(csv.inputDate, format1, null);
                    rep3.CreateUser = UserId.ToString();
                    rep3.CreatePg = currPg;
                    rep3.UpdatedDate = currTime;
                    rep3.UpdateUser = UserId.ToString();
                    rep3.UpdatePg = currPg;
                    _context.TRepay.Add(rep3);
                    RepayId++;

                    csv.status = 1;
                    csv.UpdatedDate = currTime;
                    csv.UpdateUser = UserId.ToString();
                    csv.UpdatePg = currPg;
                    _context.CsvMigration.Update(csv);

                    _context.SaveChanges();
                    migCnt++;
                }
                catch (Exception ex) 
                {
                    currCost.status = (int)statusEnum.削除;
                    currCost.UpdatedDate = currTime;
                    currCost.UpdateUser = UserId.ToString();
                    currCost.UpdatePg = currPg;

                    _context.SaveChanges();
                }
            }
            string migResult = "";
            if(migCnt > 0)
            {
                migResult = "データ移行件数は" + migCnt + "です。";
            }
            else
            {
                migResult = "データは移行されませんでした。";
            }
            if (skpCnt > 0)
            {
                migResult += skpCnt + "件はデータ重複のため移行されませんでした。";
            }
            else 
            {
                migResult += "重複データはありませんでした。";
            }

            ViewBag.migCnt = migResult;

            return View();
        }

        private int getCostStatus(string input)
        {
            int retStatus = 0;
            switch (input)
            {
                case "未精算":
                    retStatus = 1;
                    break;
                case "精算(端数込)":
                    retStatus = 3;
                    break;
            }
            return retStatus;
        }

        private int getGenreId(string input)
        {
            int retGenre = 0;
            switch (input)
            {
                case "食費": 
                    retGenre = 101;
                    break;
                case "外食費": 
                    retGenre = 102; 
                    break;
                case "雑貨(消耗品)": 
                    retGenre = 103; 
                    break;
                case "雑貨(本やCD)": 
                    retGenre = 104; 
                    break;
                case "雑貨(家具･家電)": retGenre = 105; 
                    break;
                case "交通費": 
                    retGenre = 106; 
                    break;
                case "遊行費": retGenre = 107; 
                    break;
                case "NTT料金": 
                    retGenre = 108; 
                    break;
                case "ガス代": 
                    retGenre = 109; 
                    break;
                case "水道代": 
                    retGenre = 110; 
                    break;
                case "電気代": 
                    retGenre = 111; 
                    break;
                case "プロバイダ": 
                    retGenre = 112; 
                    break;
                case "NHK料金": 
                    retGenre = 113; 
                    break;
                case "ケーブル代": 
                    retGenre = 114; 
                    break;
                case "家賃振込料": 
                    retGenre = 115; 
                    break;
                case "クリーニング": 
                    retGenre = 116; 
                    break;
                case "その他": 
                    retGenre = 117; 
                    break;
                default: 
                    retGenre = 118; 
                    break;
            }
            return retGenre;
        }

    }
}
