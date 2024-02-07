using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Controllers
{
    [Authorize]
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

            // 画面表示
            CsvModel model = new CsvModel(_context);
            csvImport.MigrationDisps = model.GetMigrationDispList((int)GroupId);

            // 移行後の画面遷移の場合、移行結果を画面表示
            try
            {
                List<string> resultMessage = ((string[])TempData["resultMessage"]).ToList();
                ViewBag.resultMessage = resultMessage;
            }
            catch
            {

            }
            return View(csvImport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(CsvImport csvImport)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // nullチェック
            IFormFile file = csvImport.formFile;
            if (file != null && file.Length > 0)
            {
                View(csvImport);
            }

            string fileName = Path.GetFileName(file.FileName);
            string fileExtension = Path.GetExtension(fileName);
            string filePath;

            // CSVファイル以外の場合、保存して処理終了
            if (fileExtension != ".csv")
            { 
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                View(csvImport);
            }

            // CSVファイルの場合
            string parentPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            DirectoryInfo parentDir = new DirectoryInfo(parentPath);
            if (!parentDir.Exists)
            {
                parentDir.Create();
            }
            DateTime currDate = DateTime.Now;
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName + currDate.ToString("_yyyyMMddHHmmss"));
            // ファイルを保存
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            CsvModel model = new CsvModel(_context);

            int impCnt = 0;
            try
            {
                // 登録処理
                impCnt = model.CreateCsvImport((int)GroupId, (int)UserId, filePath, currDate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(csvImport);
            }

            // 結果メッセージをセット
            List<string> result = new List<string>();
            if (impCnt > 0)
            {
                result.Add($"データインポート件数は{impCnt}件です");
            }
            else
            {
                result.Add("データはインポートされませんでした");
            }
            ViewBag.resultMessage = result;

            // 画面表示
            csvImport.formFile = null;
            csvImport.MigrationDisps = model.GetMigrationDispList((int)GroupId);

            return View(csvImport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Migrate(int? id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if(id == null)
            {
                return NotFound();
            }


            // 結果メッセージをセット
            CsvModel model = new CsvModel(_context);
            try
            {
                // 対象データ取得
                List<CsvMigration> csvList = model.GetMigrationList((int)GroupId, (int)UserId, (int)id);

                // 登録処理
                List<string> resultMessage = model.CreateCostPayRepay((int)GroupId, (int)UserId, csvList);

                // 結果を遷移先でメッセージ表示する
                TempData["resultMessage"] = resultMessage;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
            }

            return RedirectToAction(nameof(Import));
        }

    }
}
