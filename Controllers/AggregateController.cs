using Microsoft.AspNetCore.Mvc;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    public class AggregateController : Controller
    {
        private readonly WarikakeWebContext _context;

        public AggregateController(WarikakeWebContext context)
        {
            _context = context; 
        }
            
        

        public ActionResult Index(int year)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId);

            // 戻り値の準備
            WarikakeSearch warikakeSearch = new WarikakeSearch();
            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);


            int currYear = DateTime.Now.Year;
            if (year <= 0)
            {
                year = DateTime.Now.Year;
            }
            warikakeSearch.prevYear = (2000>year) ? 0 : year-1;
            warikakeSearch.nextYear = (DateTime.Now.Year<=year) ? 0 : year+1;
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetAggregatedWarikakeQueries((int)GroupId, year);
            List<WarikakeQuery> warikakeSumQueries = warikakeQuery.GetAggregatedSumWarikakeQueries((int)GroupId, year);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            warikakeSearch.warikakeDisps = warikakeDisp.GetWarikakeDisps(warikakeQueries);
            warikakeSearch.warikakeSum = warikakeDisp.GetWarikakeDisp(warikakeSumQueries);


            return View(warikakeSearch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(WarikakeSearch input)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, input.GenreId);

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            // 戻り値の準備
            WarikakeSearch warikakeSearch = new WarikakeSearch();
            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetAggregatedWarikakeQueries((int)GroupId, -1);
            List<WarikakeQuery> warikakeSumQueries = warikakeQuery.GetAggregatedSumWarikakeQueries((int)GroupId, -1);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            warikakeSearch.warikakeDisps = warikakeDisp.GetWarikakeDisps(warikakeQueries);
            warikakeSearch.warikakeSum = warikakeDisp.GetWarikakeDisp(warikakeSumQueries);


            return View(warikakeSearch);
        }
    }
}
