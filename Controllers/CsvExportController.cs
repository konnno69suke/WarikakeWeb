using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Text;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    public class CsvExportController : Controller
    {

        private readonly WarikakeWebContext _context;

        public CsvExportController(WarikakeWebContext context)
        {
            _context = context;
        }

        public ActionResult Export()
        {
            return View();
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

            CsvMigration csvMigration = new CsvMigration(_context);
            List<CsvMigration> csvMigrations = csvMigration.GetExportData((int)GroupId);

            Boolean hasQuote = export.hasDoubleQuote.Equals("has") ? true : false;
            string csvString = csvMigration.GetExportString(csvMigrations, hasQuote);


            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_家計簿出力.csv";
    
            var csvData = Encoding.UTF8.GetBytes(csvString);
            return File(csvData, "text/csv", fileName);
        }


    }
}
