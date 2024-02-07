using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Controllers
{
    [Authorize]
    public class CsvExportController : Controller
    {

        private readonly WarikakeWebContext _context;

        public CsvExportController(WarikakeWebContext context)
        {
            _context = context;
        }

        public ActionResult Export()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 画面表示情報をセット
            CsvExport export = new CsvExport();
            export.hasDoubleQuote = "no";

            return View(export);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(CsvExport export)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View();
            }

            CsvModel model = new CsvModel(_context);
            List<CsvMigration> csvMigrations = model.GetExportData((int)GroupId, export);

            bool hasQuote = export.hasDoubleQuote.Equals("has") ? true : false;
            string csvString = model.GetExportString(csvMigrations, hasQuote);


            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_家計簿出力.csv";
    
            var csvData = Encoding.UTF8.GetBytes(csvString);
            return File(csvData, "text/csv", fileName);
        }
    }
}
