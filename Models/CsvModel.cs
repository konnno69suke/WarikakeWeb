using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.CompilerServices;
using System.Text;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Models
{
    public class CsvModel
    {
        WarikakeWebContext _context;

        public CsvModel(WarikakeWebContext context)
        {
            _context = context;
        }

        // インポート済CSV情報を取得
        public List<MigrationDisp> GetMigrationDispList(int GroupId)
        {
            {
                List<MigrationDisp> migrationDispList = _context.Database.SqlQuery<MigrationDisp>(@$"
                    select cm.importid, cm.status, ' ' statusname, cm.userid, mu.username, count(cm.status) impcount, min(convert(datetime, cm.buydate)) fromdate, max(convert(datetime, cm.buydate)) untildate
                    from csvmigration cm
                    inner join muser mu on cm.userid = mu.userid
                    where cm.groupid = {GroupId} and mu.status = 1
                    group by cm.importid, cm.status, cm.userid, mu.username
                    ").ToList();

                foreach (MigrationDisp migrationDisp in migrationDispList)
                {
                    switch (migrationDisp.Status)
                    {
                        case 0:
                            migrationDisp.StatusName = "移行前";
                            break;
                        case 1:
                            migrationDisp.StatusName = "移行済";
                            break;
                        case 9:
                            migrationDisp.StatusName = "移行不可";
                            break;
                        default:
                            migrationDisp.StatusName = "移行不可";
                            break;
                    }
                }
                return migrationDispList;
            }
        }

        // 移行対象情報を取得
        public List<CsvMigration> GetMigrationList(int GroupId, int UserId, int ImportId)
        {
            List<CsvMigration> csvList = _context.CsvMigration.Where(c => c.importId == ImportId && c.status == 0 && c.GroupId == GroupId && c.UserId == UserId).ToList();
            return csvList;
        }

        // CSVエクスポート用情報を取得
        public List<CsvMigration> GetExportData(int GroupId, CsvExport export)
        {
            DateTime? FromDate = export.FromDate;
            DateTime? UntilDate = export.UntilDate;

            Serilog.Log.Information($"SQL param:{GroupId}, {FromDate}, {UntilDate}");
            List<SqlParameter> paramList = new List<SqlParameter>();
            StringBuilder query1 = new StringBuilder(@"
                with paydata as (select costid,
                                max(case when payid = 0 then payamount end) as pay1,
                                max(case when payid = 1 then payamount end) as pay2,
                                max(case when payid = 2 then payamount end) as pay3
                                from tpay
                                group by costid),
                    repaydata as (select costid,
                                max(case when repayid = 0 then repayamount end) as repay1,
                                max(case when repayid = 1 then repayamount end) as repay2,
                                max(case when repayid = 2 then repayamount end) as repay3
                                from trepay
                                group by costid)
                select tc.id, -1 importid, tc.groupid, -1 userid, tc.status, 
                       format(tc.createddate, N'yyyy年MM月dd日HH時mm分ss秒') inputDate, format(tc.costdate, 'yyyyMMdd') buyDate, tc.genrename kindName, convert(varchar, tc.costamount) buyAmount, 
                       convert(varchar, isnull(tp.pay1,0)) pf1, convert(varchar, isnull(tp.pay2,0)) pf2, convert(varchar, isnull(tp.pay3,0)) pf3,  
                       convert(varchar, isnull(tr.repay1, 0)) pa1, convert(varchar, isnull(tr.repay2, 0)) pa2, convert(varchar, isnull(tr.repay3, 0)) pa3, 
                       '0' pr1, '0' pr2, '0' pr3, convert(varchar, tc.status) buyStatus, 
                       tc.createddate, tc.createuser, tc.createpg, tc.updateddate, tc.updateuser, tc.updatepg
                from tcost tc
                inner join paydata tp on tc.costid = tp.costid
                inner join repaydata tr on tc.costid = tr.costid
                where tc.groupid = @GroupId ");
            paramList.Add(new SqlParameter("@GroupId", GroupId));
            if (FromDate != null)
            {
                StringBuilder query2 = new StringBuilder($@"
                and tc.costdate >= @FromDate ");
                query1.Append(query2);
                paramList.Add(new SqlParameter("@FromDate", FromDate));
            }
            if (UntilDate != null)
            {
                StringBuilder query3 = new StringBuilder($@"
                and tc.costdate <= @UntilDate ");
                query1.Append(query3);
                paramList.Add(new SqlParameter("@UntilDate", UntilDate));
            }

            FormattableString query4 = $@"
                order by tc.costdate, tc.costid ";
            query1.Append(query4);

            List<CsvMigration> csvMigrations = _context.Database.SqlQuery<CsvMigration>(FormattableStringFactory.Create(query1.ToString(), paramList.ToArray())).ToList();

            return csvMigrations;
        }

        public int CreateCsvImport(int GroupId, int UserId, string filePath, DateTime currDate)
        {
            int impCnt = 0;

            // CSVファイルの中を読込
            using (TextFieldParser csvParser = new TextFieldParser(filePath))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // 1行ずつ読込
                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();

                    // 14列以外はスキップ
                    if (14 != fields.Count())
                    {
                        continue;
                    }

                    // テーブル登録
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
                    csv.importId = GetNextImportId();
                    csv.GroupId = (int)GroupId;
                    csv.UserId = (int)UserId;
                    _context.CsvMigration.Add(csv);
                    impCnt++;
                }
                _context.SaveChanges();
            }
            return impCnt;
        }

        // 支払データへ移行登録
        public List<string> CreateCostPayRepay(int GroupId, int UserId, List<CsvMigration> csvList)
        {
            List<string> result = new List<string>();

            // データ移行
            string format1 = "yyyy年MM月dd日HH時mm分ss秒";
            string format2 = "yyyyMMdd";
            DateTime currTime = DateTime.Now;
            string currPg = "ImportMigrate";
            int status = (int)statusEnum.移行;
            int migCnt = 0;
            int skpCnt = 0;
            WarikakeModel model = new WarikakeModel(_context);
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
                TCost currCost = model.GetCurrentCost((int)UserId, currTime, currPg);

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
            // 結果をメッセージ出力
            if (migCnt > 0)
            {
                result.Add($"データ移行件数は{migCnt}です。");
            }
            else
            {
                result.Add("データは移行されませんでした。");
            }
            if (skpCnt > 0)
            {
                result.Add("件はデータ重複のため移行されませんでした。");
            }
            else
            {
                result.Add("重複データはありませんでした。");
            }
            return result;
        }

        // DBから取得したデータをcsvフォーマットの文字列に変換
        public string GetExportString(List<CsvMigration> csvMigrations, bool hasQuote)
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendLine("#入力日時,購入日,品目,金額,DB立替,RM立替,FN立替,DB分担,RM分担,FN分担,DB端数,RM端数,FN端数,精算状況");
            if (hasQuote)
            {
                csvMigrations.ForEach(a => sb.AppendLine(CreateCsvBodyWithQuote(a)));
            }
            else
            {
                csvMigrations.ForEach(a => sb.AppendLine(CreateCsvBodyNoQuote(a)));
            }
            return sb.ToString();

        }
        private static string CreateCsvBodyWithQuote(CsvMigration a)
        {
            WarikakeDisp wari = new WarikakeDisp();
            var sb = new StringBuilder();
            sb.Append(string.Format($@"""{a.inputDate}"","));
            sb.Append(string.Format($@"""{a.buyDate}"","));
            sb.Append(string.Format($@"""{a.kindName}"","));
            sb.Append(string.Format($@"""{a.buyAmount}"","));
            sb.Append(string.Format($@"""{a.pf1}"","));
            sb.Append(string.Format($@"""{a.pf2}"","));
            sb.Append(string.Format($@"""{a.pf3}"","));
            sb.Append(string.Format($@"""{a.pa1}"","));
            sb.Append(string.Format($@"""{a.pa2}"","));
            sb.Append(string.Format($@"""{a.pa3}"","));
            sb.Append(string.Format($@"""{a.pr1}"","));
            sb.Append(string.Format($@"""{a.pr2}"","));
            sb.Append(string.Format($@"""{a.pr3}"","));
            sb.Append(string.Format($@"""{(statusEnum)int.Parse(a.buyStatus)}"""));
            return sb.ToString();
        }

        private static string CreateCsvBodyNoQuote(CsvMigration a)
        {
            WarikakeDisp wari = new WarikakeDisp();
            var sb = new StringBuilder();
            sb.Append(string.Format($@"{a.inputDate},"));
            sb.Append(string.Format($@"{a.buyDate},"));
            sb.Append(string.Format($@"{a.kindName},"));
            sb.Append(string.Format($@"{a.buyAmount},"));
            sb.Append(string.Format($@"{a.pf1},"));
            sb.Append(string.Format($@"{a.pf2},"));
            sb.Append(string.Format($@"{a.pf3},"));
            sb.Append(string.Format($@"{a.pa1},"));
            sb.Append(string.Format($@"{a.pa2},"));
            sb.Append(string.Format($@"{a.pa3},"));
            sb.Append(string.Format($@"{a.pr1},"));
            sb.Append(string.Format($@"{a.pr2},"));
            sb.Append(string.Format($@"{a.pr3},"));
            sb.Append(string.Format($@"{(statusEnum)int.Parse(a.buyStatus)}"));
            return sb.ToString();
        }

        // 新規インポートIDの発番
        public int GetNextImportId()
        {
            int ImportId = _context.CsvMigration.Any() ? _context.CsvMigration.Max(c => c.importId) : -0;

            ImportId++;

            return ImportId;
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
                case "雑貨(家具･家電)":
                    retGenre = 105;
                    break;
                case "交通費":
                    retGenre = 106;
                    break;
                case "遊行費":
                    retGenre = 107;
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

