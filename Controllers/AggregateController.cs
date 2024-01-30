using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
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
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 戻り値の準備
            WarikakeSearch warikakeSearch = new WarikakeSearch();
            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);

            if (year <= 0)
            {
                year = DateTime.Now.Year;
            }
            warikakeSearch.prevYear = (2010>year) ? 0 : year-1;
            warikakeSearch.currYear = year;
            warikakeSearch.currDisp = year + "年";
            warikakeSearch.nextYear = (DateTime.Now.Year<=year) ? 0 : year+1;
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetAggregatedWarikakeQueries((int)GroupId, year);
            List<WarikakeQuery> warikakeSumQueries = warikakeQuery.GetAggregatedSumWarikakeQueries((int)GroupId, year);

            List<WarikakeQuery> warikakeGraphQueries = warikakeQuery.GetAggreateGraphWarikakeQueries((int)GroupId, year);

            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            warikakeSearch.warikakeDisps = warikakeDisp.GetWarikakeDisps(warikakeQueries);
            warikakeSearch.warikakeSum = warikakeDisp.GetWarikakeDisp(warikakeSumQueries);

            List<WarikakeGraph> warikakeGraphs = warikakeDisp.GetWarikakeGraph(warikakeGraphQueries);
            WarikakeChart chart = new WarikakeChart();
            chart.data.labels = new List<String>();
            for (int i = 1; i <= 12; i++)
            {
                chart.data.labels.Add("'" + i + "月'");
            }
            chart.data.datasets = warikakeDisp.GetChartDataset(warikakeGraphQueries);

            String tmpString = JsonConvert.SerializeObject(chart);
            warikakeSearch.warikakeChart = tmpString.Replace("\"", "");
            return View(warikakeSearch);
        }

        public ActionResult Detail(string yearMonth)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 戻り値の準備
            WarikakeSearch warikakeSearch = new WarikakeSearch();
            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);

            int year = 0;
            int month = 0;
            try
            {
                year = int.Parse(yearMonth.Split('_')[0]);
                month = int.Parse(yearMonth.Split('_')[1]);
            }catch(Exception) 
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }
            DateTime currYearMonth = new DateTime(year, month, 1);

            warikakeSearch.currYear = year;
            DateTime prevMonth = currYearMonth.AddMonths(-1);
            if(prevMonth < new DateTime(2010, 1, 1))
            {
                warikakeSearch.prevMonth = "0";
            }
            else
            {
                warikakeSearch.prevMonth = currYearMonth.AddMonths(-1).ToString("yyyy_MM");
            }
            
            warikakeSearch.currMonth = currYearMonth.ToString("yyyy_MM");
            warikakeSearch.currDisp = currYearMonth.ToString("yyyy年MM月");
            DateTime nextMonth = currYearMonth.AddMonths(1);
            if(nextMonth > DateTime.Now)
            {
                warikakeSearch.nextMonth = "0";
            }
            else
            {
                warikakeSearch.nextMonth = currYearMonth.AddMonths(1).ToString("yyyy_MM");
            }

            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetAggregatedWarikakeQueries((int)GroupId, year, month);

            List<WarikakeQuery> warikakeGraphQueries = warikakeQuery.GetAggreateGraphWarikakeQueries((int)GroupId, year, month);

            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            warikakeSearch.warikakeDisps = warikakeDisp.GetWarikakeDisps(warikakeQueries);

            WarikakeChart chart = new WarikakeChart();
            chart.data.labels = new List<String>();
            int lastDay = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Day;
            for (int i=1; i<= lastDay; i++)
            {
                chart.data.labels.Add("'" + i +  "日'");
            }
            chart.data.datasets = warikakeDisp.GetChartDataset(warikakeGraphQueries);

            String tmpString = JsonConvert.SerializeObject(chart);
            warikakeSearch.warikakeChart = tmpString.Replace("\"", "");
            return View(warikakeSearch);
        }

        public ActionResult Date(string date)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 戻り値の準備
            WarikakeSearch warikakeSearch = new WarikakeSearch();
            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);

            int year = 0;
            int month = 0;
            int day = 0;
            try
            {
                year = int.Parse(date.Split('_')[0]);
                month = int.Parse(date.Split('_')[1]);
                day = int.Parse(date.Split('_')[2]);
            }
            catch (Exception)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                day = DateTime.Now.Day;
            }
            DateTime currDate = new DateTime(year, month, day);

            warikakeSearch.currMonth = currDate.ToString("yyyy_MM");
            DateTime prevDate = currDate.AddDays(-1);
            if(prevDate < new DateTime(2010, 1, 1))
            {
                warikakeSearch.prevDate = "0";
            }
            else
            {
                warikakeSearch.prevDate = currDate.AddDays(-1).ToString("yyyy_MM_dd");
            }
            warikakeSearch.currDate = currDate.ToString("yyyy_MM_dd");
            warikakeSearch.currDisp = currDate.ToString("yyyy年MM月dd日");
            DateTime nextDate = currDate.AddDays(1);
            if(nextDate > DateTime.Now)
            {
                warikakeSearch.nextDate = "0";
            }
            else 
            {
                warikakeSearch.nextDate = currDate.AddDays(1).ToString("yyyy_MM_dd");
            }
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetAggregatedWarikakeQueries((int)GroupId, year, month, day);

            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            warikakeSearch.warikakeDisps = warikakeDisp.GetWarikakeDisps(warikakeQueries);

            return View(warikakeSearch);
        }
    }
}
