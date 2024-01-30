using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WarikakeWeb.Data;
using WarikakeWeb.Logic;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly WarikakeWebContext _context;

        public SubscribeController(WarikakeWebContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            List<SubscribeDisp> subscribeDisps = IndexLogic((int)GroupId);
            return View(subscribeDisps);
        }

        // GET: SubscribeController/Create
        public ActionResult Create()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            // グループ情報に応じた登録画面を表示
            List<MGenre> genres = _context.MGenre.Where(g => g.status == 1 && g.GroupId == (int)GroupId).ToList();
            ViewBag.Genres = new SelectList(genres.Select(u => new { Id = u.GenreId, Name = u.GenreName }), "Id", "Name");
            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            List<MUser> users = _context.Database.SqlQuery<MUser>(
                $@"select mu.*
                    from muser mu 
                    inner join mmember mm on mu.UserId = mm.UserId
                    where mm.GroupId = {GroupId}
                    and mu.status = 1 and mm.status = 1
                    order by mu.UserId").ToList();
            SubscribeDisp input = new SubscribeDisp();
            input.GroupId = (int)GroupId;
            foreach (MUser user in users)
            {
                SubscribePayDisp inputPay = new SubscribePayDisp();
                inputPay.PayUserId = user.UserId;
                inputPay.PayUserOn = (user.UserId == UserId ? true : false);
                inputPay.PayUserName = user.UserName;
                input.Pays.Add(inputPay);
                SubscribeRepayDisp inputRepay = new SubscribeRepayDisp();
                inputRepay.RepayUserId = user.UserId;
                inputRepay.RepayUserOn = true;
                inputRepay.RepayUserName = user.UserName;
                input.Repays.Add(inputRepay);
            }
            input.weekOrDay = "day";
            input.isAllMonth = true;



            return View(input);
        }

        // POST: SubscribeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubscribeDisp input)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId:none");

            List<MGenre> genres = _context.MGenre.Where(g => g.status == 1 && g.GroupId == (int)GroupId).ToList();
            ViewBag.Genres = new SelectList(genres.Select(u => new { Id = u.GenreId, Name = u.GenreName }), "Id", "Name");
            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            if (ModelState.IsValid)
            {
                // 業務入力チェック
                int errCnt = ValidateLogic(input);
                if (0 < errCnt)
                {
                    return View(input);
                }
                // 登録処理
                CreateLogic(input, (int)GroupId);

                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        // GET: SubscribeController/Edit/5
        public ActionResult Edit(int id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            List<MGenre> genres = _context.MGenre.Where(g => g.status == 1 && g.GroupId == (int)GroupId).ToList();
            ViewBag.Genres = new SelectList(genres.Select(u => new { Id = u.GenreId, Name = u.GenreName }), "Id", "Name");
            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            var query = _context.Database.SqlQuery<SubscribeQuery>(
                    $@"select tc.SubscribeId, tc.CostTitle, tc.GroupId, mgr.GroupName, tc.GenreId, mge.GenreName, tc.status, ' ' statusName, tc.CostStatus, tc.CostAmount,
                          tp.payid, tp.userid PayUserId, mup.username PayUserName, tp.PayAmount, 
                          tr.repayid, tr.userid RepayUserId, mur.username RepayUserName, tr.RepayAmount, getdate() LastSubscribedDate, 0 lastcostid
                    from  tcostSubscribe tc 
                    inner join tpaySubscribe tp on tc.subscribeid = tp.subscribeid 
                    inner join trepaySubscribe tr on tc.subscribeid = tr.subscribeid and tp.userid = tr.userid
                    inner join muser mup on tp.userid = mup.userid
                    inner join muser mur on tr.userid = mur.userid
                    inner join mgroup mgr on tc.groupid = mgr.groupid
                    inner join mgenre mge on tc.genreid = mge.genreid
                    where tc.status = 1 and tp.status = 1 and tr.status = 1
                    and   tc.GroupId = {GroupId}
                    and   tc.subscribeid = {id}
                    order by tc.Subscribeid, tp.payid, tr.repayid");
            int prevSubscribeId = 0;
            SubscribeDisp wariDisp = new SubscribeDisp();
            foreach (SubscribeQuery item in query)
            {
                if (prevSubscribeId != item.SubscribeId)
                {
                    wariDisp = new SubscribeDisp();
                    wariDisp.SubscribeId = item.SubscribeId;
                    wariDisp.CostTitle = item.CostTitle;
                    wariDisp.GroupName = item.GroupName;
                    wariDisp.GenreId = item.GenreId;
                    wariDisp.GenreName = item.GenreName;
                    wariDisp.status = item.status;
                    wariDisp.statusName = ((statusEnum)item.status).ToString();
                    wariDisp.CostAmount = item.CostAmount;
                    wariDisp.CostDisp = item.CostAmount.ToString("C0", new CultureInfo("ja-JP"));

                    prevSubscribeId = item.SubscribeId;
                }

                SubscribePayDisp payDisp = new SubscribePayDisp();
                payDisp.PayId = item.PayId;
                payDisp.PayUserId = item.PayUserId;
                payDisp.PayUserOn = (item.PayAmount != 0 ? true : false);
                payDisp.PayUserName = item.PayUserName;
                payDisp.PayAmount = item.PayAmount;
                payDisp.PayDisp = item.PayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Pays.Add(payDisp);

                SubscribeRepayDisp repayDisp = new SubscribeRepayDisp();
                repayDisp.RepayId = item.RepayId;
                repayDisp.RepayUserId = item.RepayUserId;
                repayDisp.RepayUserOn = (item.RepayAmount != 0 ? true : false);
                repayDisp.RepayUserName = item.RepayUserName;
                repayDisp.RepayAmount = item.RepayAmount;
                repayDisp.RepayDisp = item.RepayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Repays.Add(repayDisp);
            }
            // 日付設定を検索
            TDateSubscribe mwd = _context.TDateSubscribe.Where(d => d.SubscribeId == wariDisp.SubscribeId).FirstOrDefault();
            wariDisp.dateSubscribe = mwd;
            if(mwd.m1 && mwd.m2 && mwd.m3 && mwd.m4 && mwd.m5 && mwd.m6 && mwd.m7 && mwd.m8 && mwd.m9 && mwd.m10 && mwd.m11 && mwd.m12)
            {
                wariDisp.isAllMonth = true;
            }
            else
            {
                wariDisp.isAllMonth = false;
            }
            if (mwd.r1 || mwd.r2 || mwd.r3 || mwd.r4 || mwd.r5)
            {
                if(mwd.w1 || mwd.w2 || mwd.w3 || mwd.w4 || mwd.w5 || mwd.w6 | mwd.w7)
                {
                    wariDisp.weekOrDay = "week";
                }
            }
            else if (mwd.d1 || mwd.d2 || mwd.d3 || mwd.d4 || mwd.d5 || mwd.d6 || mwd.d7 || mwd.d8 || mwd.d9 || mwd.d10
                || mwd.d11 || mwd.d12 || mwd.d13 || mwd.d14 || mwd.d15 || mwd.d16 || mwd.d17 || mwd.d18 || mwd.d19 || mwd.d20
                || mwd.d21 || mwd.d22 || mwd.d23 || mwd.d24 || mwd.d25 || mwd.d26 || mwd.d27 || mwd.d28 || mwd.d29 || mwd.d30 || mwd.d31)
            {
                wariDisp.weekOrDay = "day";
            }
            else
            {
                wariDisp.weekOrDay = "";
            }

            return View(wariDisp);
        }

        // POST: SubscribeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SubscribeDisp input)
        {
            try
            {
                int? GroupId = HttpContext.Session.GetInt32("GroupId");
                int? UserId = HttpContext.Session.GetInt32("UserId");
                if (GroupId == null)
                {
                    // セッション切れ
                    return RedirectToAction("Login", "Home");
                }
                Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

                if (ModelState.IsValid)
                {
                    // 業務入力チェック
                    int errCnt = ValidateLogic(input);
                    if (0 < errCnt)
                    {
                        return View(input);
                    }

                    DateTime dateTime = DateTime.Now;

                    TCostSubscribe existingCost = _context.TCostSubscribe.FirstOrDefault(c => c.SubscribeId == id);
                    existingCost.SubscribeId = id;

                    existingCost.CostTitle = input.CostTitle;
                    existingCost.GroupId = (int)GroupId;
                    existingCost.GenreId = input.GenreId;
                    MGenre genre = _context.MGenre.Where(g => g.GenreId == input.GenreId).FirstOrDefault();
                    existingCost.GenreName = genre.GenreName;
                    existingCost.ProvisionFlg = 0;
                    existingCost.CostAmount = input.CostAmount;

                    existingCost.UpdatedDate = dateTime;
                    existingCost.UpdateUser = UserId.ToString();
                    existingCost.UpdatePg = "SubscribeEdit";
                    _context.Update(existingCost);

                    foreach (SubscribePayDisp inputPay in input.Pays)
                    {
                        TPaySubscribe existingPay = _context.TPaySubscribe.FirstOrDefault(p => p.UserId == inputPay.PayUserId && p.SubscribeId == id);

                        existingPay.SubscribeId = id;
                        existingPay.UserId = inputPay.PayUserId;
                        existingPay.PayAmount = inputPay.PayAmount;

                        existingPay.UpdatedDate = dateTime;
                        existingPay.UpdateUser = UserId.ToString();
                        existingPay.UpdatePg = "SubscribeEdit";
                        _context.Update(existingPay);
                    }
                    foreach (SubscribeRepayDisp inputRepay in input.Repays)
                    {
                        TRepaySubscribe existingRepay = _context.TRepaySubscribe.FirstOrDefault(r => r.UserId == inputRepay.RepayUserId && r.SubscribeId == id);

                        existingRepay.SubscribeId = id;
                        existingRepay.UserId = inputRepay.RepayUserId;
                        existingRepay.RepayAmount = inputRepay.RepayAmount;

                        existingRepay.UpdatedDate = dateTime;
                        existingRepay.UpdateUser = UserId.ToString();
                        existingRepay.UpdatePg = "SubscribeEdit";
                        _context.Update(existingRepay);
                    }
                    TDateSubscribe existingDate = _context.TDateSubscribe.FirstOrDefault(d => d.SubscribeId == id);
                    existingDate.UserId = (int)UserId;
                    existingDate.m1 = input.dateSubscribe.m1;
                    existingDate.m2 = input.dateSubscribe.m2;
                    existingDate.m3 = input.dateSubscribe.m3;
                    existingDate.m4 = input.dateSubscribe.m4;
                    existingDate.m5 = input.dateSubscribe.m5;
                    existingDate.m6 = input.dateSubscribe.m6;
                    existingDate.m7 = input.dateSubscribe.m7;
                    existingDate.m8 = input.dateSubscribe.m8;
                    existingDate.m9 = input.dateSubscribe.m9;
                    existingDate.m10 = input.dateSubscribe.m10;
                    existingDate.m11 = input.dateSubscribe.m11;
                    existingDate.m12 = input.dateSubscribe.m12;
                    existingDate.r1 = input.dateSubscribe.r1;
                    existingDate.r2 = input.dateSubscribe.r2;
                    existingDate.r3 = input.dateSubscribe.r3;
                    existingDate.r4 = input.dateSubscribe.r4;
                    existingDate.r5 = input.dateSubscribe.r5;
                    existingDate.w1 = input.dateSubscribe.w1;
                    existingDate.w2 = input.dateSubscribe.w2;
                    existingDate.w3 = input.dateSubscribe.w3;
                    existingDate.w4 = input.dateSubscribe.w4;
                    existingDate.w5 = input.dateSubscribe.w5;
                    existingDate.w6 = input.dateSubscribe.w6;
                    existingDate.w7 = input.dateSubscribe.w7;
                    existingDate.d1 = input.dateSubscribe.d1;
                    existingDate.d2 = input.dateSubscribe.d2;
                    existingDate.d3 = input.dateSubscribe.d3;
                    existingDate.d4 = input.dateSubscribe.d4;
                    existingDate.d5 = input.dateSubscribe.d5;
                    existingDate.d6 = input.dateSubscribe.d6;
                    existingDate.d7 = input.dateSubscribe.d7;
                    existingDate.d8 = input.dateSubscribe.d8;
                    existingDate.d9 = input.dateSubscribe.d9;
                    existingDate.d10 = input.dateSubscribe.d10;
                    existingDate.d11 = input.dateSubscribe.d11;
                    existingDate.d12 = input.dateSubscribe.d12;
                    existingDate.d13 = input.dateSubscribe.d13;
                    existingDate.d14 = input.dateSubscribe.d14;
                    existingDate.d15 = input.dateSubscribe.d15;
                    existingDate.d16 = input.dateSubscribe.d16;
                    existingDate.d17 = input.dateSubscribe.d17;
                    existingDate.d18 = input.dateSubscribe.d18;
                    existingDate.d19 = input.dateSubscribe.d19;
                    existingDate.d20 = input.dateSubscribe.d20;
                    existingDate.d21 = input.dateSubscribe.d21;
                    existingDate.d22 = input.dateSubscribe.d22;
                    existingDate.d23 = input.dateSubscribe.d23;
                    existingDate.d24 = input.dateSubscribe.d24;
                    existingDate.d25 = input.dateSubscribe.d25;
                    existingDate.d26 = input.dateSubscribe.d26;
                    existingDate.d27 = input.dateSubscribe.d27;
                    existingDate.d28 = input.dateSubscribe.d28;
                    existingDate.d29 = input.dateSubscribe.d29;
                    existingDate.d30 = input.dateSubscribe.d30;
                    existingDate.d31 = input.dateSubscribe.d31;

                    existingDate.UpdatedDate = dateTime;
                    existingDate.UpdateUser = UserId.ToString();
                    existingDate.UpdatePg = "SubscribeEdit";
                    _context.Update(existingDate);

                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    List<MGenre> genres = _context.MGenre.Where(g => g.status == 1 && g.GroupId == (int)GroupId).ToList();
                    ViewBag.Genres = new SelectList(genres.Select(u => new { Id = u.GenreId, Name = u.GenreName }), "Id", "Name");
                    // 端数を優先するユーザー情報をセット
                    ViewBag.qid = UserId;

                    return View(input);
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: SubscribeController/Delete/5
        public ActionResult Delete(int id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            SubscribeDisp wariDisp = DispLogic(id, (int)GroupId);
            return View(wariDisp);
        }

        // POST: SubscribeController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            try
            {
                UpdateLogic(id, 9);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SubscribeController/Publish
        public ActionResult Publish()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            List<SubscribeDisp> subscribeIndex = IndexLogic((int)GroupId);
            ViewBag.ResultMessage = null;
            return View(subscribeIndex);
        }

        // POST: SubscribeController/Publish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Publish(int id, List<SubscribeDisp> subscribeDisps)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            try
            {
                List<string> messageList = new List<string>();
                foreach (SubscribeDisp subDis in subscribeDisps)
                {
                    Boolean updFlg = false;
                    if (!subDis.isChecked)
                    {
                        continue;
                    }
                    int subid = subDis.SubscribeId;
                    TSubscribe sub = _context.TSubscribe.Where(s => s.SubscribeId == subid && s.status == 1).OrderByDescending(s => s.SubscribedDate).FirstOrDefault();
                    TDateSubscribe dateSub = _context.TDateSubscribe.Where(d => d.SubscribeId == subid && d.status == 1).FirstOrDefault();
                    List<int> monthList = dateSub.getMonthList();
                    List<int> weekOfMonthList = dateSub.getWeekOfMonthList();
                    List<int> dayOfWeekList = dateSub.getDayOfWeekList();
                    List<int> dayList = dateSub.getdayList();

                    String createResult = "";
                    DateTime lastDate = sub.SubscribedDate.Date;
                    DateTime currDate = DateTime.Now.Date;
                    DateTime loopDate = lastDate;
                    loopDate = loopDate.AddDays(1);
                    if (dateSub.isWeekly())
                    {
                        while (currDate >= loopDate)
                        {
                            int weekOfMonth = (loopDate.Day - 1) / 7 + 1;
                            int dayOfWeek = (int)loopDate.DayOfWeek;
                            if (weekOfMonthList.Contains(weekOfMonth) && dayOfWeekList.Contains(dayOfWeek))
                            {
                                createResult = createCostData(subid, loopDate);
                                messageList.Add(createResult);
                            }
                            loopDate = loopDate.AddDays(1);
                        }
                    }
                    else if (dateSub.isDayly())
                    {
                        while (currDate >= loopDate)
                        {
                            int day = loopDate.Day;
                            if (dayList.Contains(day))
                            {
                                createResult = createCostData(subid, loopDate);
                                messageList.Add(createResult);
                            }

                            int lastDay = DateTime.DaysInMonth(loopDate.Year, loopDate.Month);
                            if (day == lastDay)
                            {
                                if ((dayList.Contains(29) && lastDay < 29) || (dayList.Contains(30) && lastDay < 30) || (dayList.Contains(31) && lastDay < 31))
                                {
                                    createResult = createCostData(subid, loopDate);
                                    messageList.Add(createResult);
                                }
                            }
                            loopDate = loopDate.AddDays(1);
                        }
                    }
                    else
                    {
                        // 不正入力
                        messageList.Add("入力内容が処理できませんでした。");
                        ViewBag.ResultMessage = messageList;
                        return View(subscribeDisps);
                    }
                }

                // 
                if(messageList.Count == 0)
                {
                    messageList.Add("仮登録するべきデータは無かったようです。");
                }
                ViewBag.ResultMessage = messageList;

                List<SubscribeDisp> subscribeIndex = IndexLogic((int)GroupId);
                return View(subscribeIndex);
            }
            catch
            {
                return View(subscribeDisps);
            }
        }

        private List<SubscribeDisp> IndexLogic(int GroupId)
        {
            List<SubscribeDisp> subscribeList = new List<SubscribeDisp>();

            // 一覧表示
            var query = _context.Database.SqlQuery<SubscribeQuery>(
                $@"with published as (select subscribeid, max(subscribeddate) lastsubscribeddate, max(costid) lastcostid from tsubscribe where status = 1 group by subscribeid) 
                    select tc.SubscribeId, tc.CostTitle, tc.GroupId, mgr.GroupName, tc.GenreId, mge.GenreName, tc.status, ' ' statusName, tc.CostStatus, tc.CostAmount, 
                          tp.payid, tp.userid PayUserId, mup.username PayUserName, tp.PayAmount, 
                          tr.repayid, tr.userid RepayUserId, mur.username RepayUserName, tr.RepayAmount, 
                          ts.lastsubscribeddate, ts.lastcostid
                    from  tcostSubscribe tc 
                    inner join tpaySubscribe tp on tc.subscribeid = tp.subscribeid 
                    inner join trepaySubscribe tr on tc.subscribeid = tr.subscribeid and tp.userid = tr.userid
                    left join published ts on tc.subscribeid = ts.subscribeid
                    inner join muser mup on tp.userid = mup.userid
                    inner join muser mur on tr.userid = mur.userid
                    inner join mgroup mgr on tc.groupid = mgr.groupid
                    inner join mgenre mge on tc.genreid = mge.genreid
                    where tc.status = 1 and tp.status = 1 and tr.status = 1
                    and   tc.GroupId = {GroupId}
                    order by tc.Subscribeid desc, tp.payid, tr.repayid");
            int prevSubscribeId = 0;

            SubscribeDisp wariDisp = new SubscribeDisp();
            TDateSubscribe dateSub = new TDateSubscribe();
            foreach (SubscribeQuery item in query)
            {
                if (prevSubscribeId != item.SubscribeId)
                {
                    wariDisp = new SubscribeDisp();
                    wariDisp.SubscribeId = item.SubscribeId;
                    wariDisp.CostTitle = item.CostTitle;
                    wariDisp.GroupName = item.GroupName;
                    wariDisp.GenreId = item.GenreId;
                    wariDisp.GenreName = item.GenreName;
                    wariDisp.status = item.status;
                    wariDisp.statusName = ((statusEnum)item.status).ToString();
                    wariDisp.CostAmount = item.CostAmount;
                    wariDisp.CostDisp = item.CostAmount.ToString("C0", new CultureInfo("ja-JP"));
                    wariDisp.LastSubscribedDate = item.LastSubscribedDate;
                    wariDisp.LastCostId = item.LastCostId;

                    subscribeList.Add(wariDisp);
                    prevSubscribeId = item.SubscribeId;
                }

                SubscribePayDisp payDisp = new SubscribePayDisp();
                payDisp.PayId = item.PayId;
                payDisp.PayUserId = item.PayUserId;
                payDisp.PayUserName = item.PayUserName;
                payDisp.PayAmount = item.PayAmount;
                payDisp.PayDisp = item.PayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Pays.Add(payDisp);

                SubscribeRepayDisp repayDisp = new SubscribeRepayDisp();
                repayDisp.RepayId = item.RepayId;
                repayDisp.RepayUserId = item.RepayUserId;
                repayDisp.RepayUserName = item.RepayUserName;
                repayDisp.RepayAmount = item.RepayAmount;
                repayDisp.RepayDisp = item.RepayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Repays.Add(repayDisp);
            }

            return subscribeList;
        }

        private SubscribeDisp DispLogic(int id, int GroupId)
        {
            // 対象データの表示欄
            var query = _context.Database.SqlQuery<SubscribeQuery>(
                    $@"select tc.Subscribeid, tc.CostTitle, tc.GroupId, mgr.GroupName, tc.GenreId, mge.GenreName, tc.status, ' ' statusName, tc.CostStatus, tc.CostAmount, 
                          tp.payid, tp.userid PayUserId, mup.username PayUserName, tp.PayAmount, 
                          tr.repayid, tr.userid RepayUserId, mur.username RepayUserName, tr.RepayAmount, getdate() LastSubscribedDate, 0 lastcostid
                    from  tcostSubscribe tc 
                    inner join tpaySubscribe tp on tc.subscribeid = tp.subscribeid 
                    inner join trepaySubscribe tr on tc.subscribeid = tr.subscribeid and tp.userid = tr.userid
                    inner join muser mup on tp.userid = mup.userid
                    inner join muser mur on tr.userid = mur.userid
                    inner join mgroup mgr on tc.groupid = mgr.groupid
                    inner join mgenre mge on tc.genreid = mge.genreid
                    where tc.status = 1 and tp.status = 1 and tr.status = 1
                    and   tc.GroupId = {GroupId}
                    and   tc.subscribeid = {id}
                    order by tc.Subscribeid, tp.payid, tr.repayid");
            int prevSubscribeId = 0;
            SubscribeDisp wariDisp = new SubscribeDisp();
            foreach (SubscribeQuery item in query)
            {
                if (prevSubscribeId != item.SubscribeId)
                {
                    wariDisp = new SubscribeDisp();
                    wariDisp.SubscribeId = item.SubscribeId;
                    wariDisp.CostTitle = item.CostTitle;
                    wariDisp.GroupName = item.GroupName;
                    wariDisp.GenreId = item.GenreId;
                    wariDisp.GenreName = item.GenreName;
                    wariDisp.status = item.status;
                    wariDisp.statusName = ((statusEnum)item.status).ToString();
                    wariDisp.CostAmount = item.CostAmount;
                    wariDisp.CostDisp = item.CostAmount.ToString("C0", new CultureInfo("ja-JP"));

                    prevSubscribeId = item.SubscribeId;
                }

                SubscribePayDisp payDisp = new SubscribePayDisp();
                payDisp.PayId = item.PayId;
                payDisp.PayUserId = item.PayUserId;
                payDisp.PayUserName = item.PayUserName;
                payDisp.PayAmount = item.PayAmount;
                payDisp.PayDisp = item.PayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Pays.Add(payDisp);

                SubscribeRepayDisp repayDisp = new SubscribeRepayDisp();
                repayDisp.RepayId = item.RepayId;
                repayDisp.RepayUserId = item.RepayUserId;
                repayDisp.RepayUserName = item.RepayUserName;
                repayDisp.RepayAmount = item.RepayAmount;
                repayDisp.RepayDisp = item.RepayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Repays.Add(repayDisp);
            }

            // 日付設定を検索
            TDateSubscribe mwd = _context.TDateSubscribe.Where(d => d.SubscribeId == wariDisp.SubscribeId).FirstOrDefault();
            wariDisp.dateSubscribe = mwd;

            return wariDisp;
        }

        private int ValidateLogic(SubscribeDisp input)
        {
            // 業務入力チェック
            int errCnt = 0;
            string costTile = input.CostTitle;
            int genreId = input.GenreId;
            int costAmount = input.CostAmount;
            int memberCnt = 0;
            int sumPay = 0;
            foreach (SubscribePayDisp pay in input.Pays)
            {
                int payUserId = pay.PayUserId;
                int payAmount = pay.PayAmount;
                sumPay += payAmount;
                memberCnt++;
            }
            int sumRepay = 0;
            foreach (SubscribeRepayDisp repay in input.Repays)
            {
                int repayUserId = repay.RepayUserId;
                int repayAmount = repay.RepayAmount;
                sumRepay += repayAmount;
            }
            if (genreId != 0)
            {
                MGenre genres = _context.MGenre.Where(g => g.GenreId == genreId).FirstOrDefault();
                if (genres == null)
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.GenreId), "種別が特定できません");
                    errCnt++;
                }
            }
            int sumR = costAmount % memberCnt;
            if (costAmount != sumPay)
            {
                if (costAmount - sumPay <= sumR && sumPay - costAmount <= sumR)
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.CostAmount), "支払額と立替額が一致しません");
                    errCnt++;
                }
                else
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.CostAmount), "支払額と立替額が一致しません");
                    errCnt++;
                }

            }
            if (costAmount != sumRepay)
            {
                if (costAmount - sumRepay <= sumR && sumRepay - costAmount <= sumR)
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.CostAmount), "支払額と割勘額が一致しません");
                    errCnt++;
                }
                else
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.CostAmount), "支払額と割勘額が一致しません");
                    errCnt++;
                }
            }
            // 日付登録情報を確認
            String weekOrDay = input.weekOrDay;
            TDateSubscribe mwd = input.dateSubscribe;

            // 1～12月全て未設定はNG
            if (!mwd.m1 && !mwd.m2 && !mwd.m3 && !mwd.m4 && !mwd.m5 && !mwd.m6 && !mwd.m7 && !mwd.m8 && !mwd.m9 && !mwd.m10 && !mwd.m11 && !mwd.m12)
            {
                ModelState.AddModelError(nameof(SubscribeDisp.isAllMonth), "月の指定がありません");
                errCnt++;
            }
            if (weekOrDay.Equals("week"))
            {
                // 第1～第5全て未設定はNG
                if(!mwd.r1 && !mwd.r2 && !mwd.r3 && !mwd.r4 && !mwd.r5)
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.weekOrDay), "曜日指定時は何度目の曜日かを指定してください");
                    errCnt++;
                }
                // 日～土全て未設定はNG
                if (!mwd.w1 && !mwd.w2 && !mwd.w3 && !mwd.w4 && !mwd.w5 && !mwd.w6 && !mwd.w7)
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.weekOrDay), "日付や曜日の指定がありません");
                    errCnt++;
                }
            }else if (weekOrDay.Equals("day"))
            {
                // 1～31日全て未設定はNG
                if (!mwd.d1 && !mwd.d2 && !mwd.d3 && !mwd.d4 && !mwd.d5 && !mwd.d6 && !mwd.d7 && !mwd.d8 && !mwd.d9 && !mwd.d10
                    && !mwd.d11 && !mwd.d12 && !mwd.d13 && !mwd.d14 && !mwd.d15 && !mwd.d16 && !mwd.d17 && !mwd.d18 && !mwd.d19 && !mwd.d20
                    && !mwd.d21 && !mwd.d22 && !mwd.d23 && !mwd.d24 && !mwd.d25 && !mwd.d26 && !mwd.d27 && !mwd.d28 && !mwd.d29 && !mwd.d30 && !mwd.d31)
                {
                    ModelState.AddModelError(nameof(SubscribeDisp.weekOrDay), "日付や曜日の指定がありません");
                    errCnt++;
                }
            }
            return errCnt;
        }

        private void UpdateLogic(int id, int status)
        {
            DateTime dateTime = DateTime.Now;
            int? UserId = HttpContext.Session.GetInt32("UserId");

            TCostSubscribe cost = _context.TCostSubscribe.Where(c => c.SubscribeId == id).FirstOrDefault();
            cost.status = status; 
            cost.UpdatedDate = dateTime;
            cost.UpdateUser = UserId.ToString();
            cost.UpdatePg = "WarikakeUpdate";
            _context.TCostSubscribe.Update(cost);

            List<TPaySubscribe> payList = _context.TPaySubscribe.Where(p => p.SubscribeId == id).ToList();
            foreach (TPaySubscribe pay in payList)
            {
                pay.status = status;
                pay.UpdatedDate = dateTime;
                pay.UpdateUser = UserId.ToString();
                pay.UpdatePg = "WarikakeUpdate";
                _context.TPaySubscribe.Update(pay);
            }
            List<TRepaySubscribe> repayList = _context.TRepaySubscribe.Where(r => r.SubscribeId == id).ToList();
            foreach (TRepaySubscribe repay in repayList)
            {
                repay.status = status;
                repay.UpdatedDate = dateTime;
                repay.UpdateUser = UserId.ToString();
                repay.UpdatePg = "WarikakeUpdate";
                _context.TRepaySubscribe.Update(repay);
            }
            TDateSubscribe dateSub = _context.TDateSubscribe.Where(d => d.SubscribeId == id).FirstOrDefault();
            dateSub.status = status;
            dateSub.UpdatedDate = dateTime;
            dateSub.UpdateUser = UserId.ToString();
            dateSub.UpdatePg = "SubscribeUpdate";
            _context.TDateSubscribe.Update(dateSub);

            _context.SaveChanges();
        }

        private void CreateLogic(SubscribeDisp input, int GroupId)
        {
            DateTime currDate = DateTime.Now;
            int? UserId = HttpContext.Session.GetInt32("UserId");
            String currPg = "SubscribeInsert";
            ModelLogic modelLogic = new ModelLogic(_context);
            int payId = 0;
            int repayId = 0;

            int updStatus = 0;
            string updGenreName = "";
            if (input.GenreId == 0)
            {
                updStatus = (int)statusEnum.手動精算;
                updGenreName = (statusEnum.手動精算).ToString();
            }
            else
            {
                updStatus = (int)statusEnum.未精算;
                MGenre genre = _context.MGenre.Where(g =>g.GenreId == input.GenreId).FirstOrDefault();
                updGenreName = genre.GenreName;
            }

            TCostSubscribe cost = new TCostSubscribe();
            cost.status = updStatus;
            cost.CostTitle = input.CostTitle;
            cost.GroupId = GroupId;
            cost.GenreId = input.GenreId;
            cost.GenreName = updGenreName;
            cost.ProvisionFlg = 0;
            cost.CostAmount = input.CostAmount;
            cost.CreatedDate = currDate;
            cost.CreateUser = UserId.ToString();
            cost.CreatePg = currPg;
            cost.UpdatedDate = currDate;
            cost.UpdateUser = UserId.ToString();
            cost.UpdatePg = currPg;
            _context.TCostSubscribe.Add(cost);


            _context.SaveChanges();
            TCostSubscribe currSubscribe = modelLogic.GetCurrentSubscribe((int)UserId, currDate, currPg);
            try
            {
                foreach (SubscribePayDisp inputPay in input.Pays)
                {

                    TPaySubscribe pay = new TPaySubscribe();
                    pay.status = updStatus;
                    pay.SubscribeId = currSubscribe.SubscribeId;
                    pay.PayId = payId;
                    pay.UserId = inputPay.PayUserId;
                    pay.PayAmount = inputPay.PayAmount;

                    pay.CreatedDate = currDate;
                    pay.CreateUser = UserId.ToString();
                    pay.CreatePg = currPg;
                    pay.UpdatedDate = currDate;
                    pay.UpdateUser = UserId.ToString();
                    pay.UpdatePg = currPg;
                    _context.TPaySubscribe.Add(pay);
                    payId++;
                }
                foreach (SubscribeRepayDisp inputRepay in input.Repays)
                {

                    TRepaySubscribe repay = new TRepaySubscribe();
                    repay.status = updStatus;
                    repay.SubscribeId = currSubscribe.SubscribeId;
                    repay.RepayId = repayId;
                    repay.UserId = inputRepay.RepayUserId;
                    repay.RepayAmount = inputRepay.RepayAmount;
                    repay.CreatedDate = currDate;
                    repay.CreateUser = UserId.ToString();
                    repay.CreatePg = currPg;
                    repay.UpdatedDate = currDate;
                    repay.UpdateUser = UserId.ToString();
                    repay.UpdatePg = currPg;
                    _context.TRepaySubscribe.Add(repay);
                    repayId++;
                }
                TDateSubscribe datesub = new TDateSubscribe();
                datesub.status = updStatus;
                datesub.UserId = (int)UserId;
                datesub.SubscribeId = currSubscribe.SubscribeId;
                datesub.m1 = input.dateSubscribe.m1;
                datesub.m2 = input.dateSubscribe.m2;
                datesub.m3 = input.dateSubscribe.m3;
                datesub.m4 = input.dateSubscribe.m4;
                datesub.m5 = input.dateSubscribe.m5;
                datesub.m6 = input.dateSubscribe.m6;
                datesub.m7 = input.dateSubscribe.m7;
                datesub.m8 = input.dateSubscribe.m8;
                datesub.m9 = input.dateSubscribe.m9;
                datesub.m10 = input.dateSubscribe.m10;
                datesub.m11 = input.dateSubscribe.m11;
                datesub.m12 = input.dateSubscribe.m12;
                datesub.r1 = input.dateSubscribe.r1;
                datesub.r2 = input.dateSubscribe.r2;
                datesub.r3 = input.dateSubscribe.r3;
                datesub.r4 = input.dateSubscribe.r4;
                datesub.r5 = input.dateSubscribe.r5;
                datesub.w1 = input.dateSubscribe.w1;
                datesub.w2 = input.dateSubscribe.w2;
                datesub.w3 = input.dateSubscribe.w3;
                datesub.w4 = input.dateSubscribe.w4;
                datesub.w5 = input.dateSubscribe.w5;
                datesub.w6 = input.dateSubscribe.w6;
                datesub.w7 = input.dateSubscribe.w7;
                datesub.d1 = input.dateSubscribe.d1;
                datesub.d2 = input.dateSubscribe.d2;
                datesub.d3 = input.dateSubscribe.d3;
                datesub.d4 = input.dateSubscribe.d4;
                datesub.d5 = input.dateSubscribe.d5;
                datesub.d6 = input.dateSubscribe.d6;
                datesub.d7 = input.dateSubscribe.d7;
                datesub.d8 = input.dateSubscribe.d8;
                datesub.d9 = input.dateSubscribe.d9;
                datesub.d10 = input.dateSubscribe.d10;
                datesub.d11 = input.dateSubscribe.d11;
                datesub.d12 = input.dateSubscribe.d12;
                datesub.d13 = input.dateSubscribe.d13;
                datesub.d14 = input.dateSubscribe.d14;
                datesub.d15 = input.dateSubscribe.d15;
                datesub.d16 = input.dateSubscribe.d16;
                datesub.d17 = input.dateSubscribe.d17;
                datesub.d18 = input.dateSubscribe.d18;
                datesub.d19 = input.dateSubscribe.d19;
                datesub.d20 = input.dateSubscribe.d20;
                datesub.d21 = input.dateSubscribe.d21;
                datesub.d22 = input.dateSubscribe.d22;
                datesub.d23 = input.dateSubscribe.d23;
                datesub.d24 = input.dateSubscribe.d24;
                datesub.d25 = input.dateSubscribe.d25;
                datesub.d26 = input.dateSubscribe.d26;
                datesub.d27 = input.dateSubscribe.d27;
                datesub.d28 = input.dateSubscribe.d28;
                datesub.d29 = input.dateSubscribe.d29;
                datesub.d30 = input.dateSubscribe.d30;
                datesub.d31 = input.dateSubscribe.d31;
                datesub.CreatedDate = currDate;
                datesub.CreateUser = UserId.ToString();
                datesub.CreatePg = "SubscribeInsert";
                datesub.UpdatedDate = currDate;
                datesub.UpdateUser = UserId.ToString();
                datesub.UpdatePg = "SubscribeInsert";
                _context.TDateSubscribe.Add(datesub);

                TSubscribe subscribe = new TSubscribe();
                subscribe.status = updStatus;
                subscribe.SubscribeId = currSubscribe.SubscribeId;
                subscribe.CostId = 0;
                subscribe.SubscribedDate = currDate;
                subscribe.CreatedDate = currDate;
                subscribe.CreateUser = UserId.ToString();
                subscribe.CreatePg = "SubscribeInsert";
                subscribe.UpdatedDate = currDate;
                subscribe.UpdateUser = UserId.ToString();
                subscribe.UpdatePg = "SubscribeInsert";
                _context.TSubscribe.Add(subscribe);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // TPaySubscribe、TRepaySubscribe、TDateSubscribe、TSubscribeのインサートに失敗した場合、
                // TCostSubscribeがごみデータになるので論理削除する
                currSubscribe.status = (int)statusEnum.削除;
                currSubscribe.UpdatedDate = currDate;
                currSubscribe.UpdateUser = UserId.ToString();
                currSubscribe.UpdatePg = currPg;
                _context.SaveChanges();
            }
        }

        private String createCostData(int SubscribeId, DateTime date)
        {
            int status = (int)statusEnum.仮登録;
            DateTime currDate = DateTime.Now;
            String currUser = HttpContext.Session.GetInt32("UserId").ToString();
            String currPg = "SubscribeCreate";
            ModelLogic modelLogic = new ModelLogic(_context);

            TCostSubscribe costBase = _context.TCostSubscribe.Where(c => c.SubscribeId == SubscribeId && c.status == 1).FirstOrDefault();
            TCost cost = new TCost();
            cost.status = status;
            cost.CostTitle = costBase.CostTitle;
            cost.GroupId = costBase.GroupId;
            cost.GenreId = costBase.GenreId;
            cost.GenreName = costBase.GenreName;
            cost.ProvisionFlg = costBase.ProvisionFlg;
            cost.CostAmount = costBase.CostAmount;
            cost.CostDate = date;
            cost.CostStatus = costBase.CostStatus;
            cost.CreatedDate = currDate;
            cost.CreateUser = currUser;
            cost.CreatePg = currPg;
            cost.UpdatedDate = currDate;
            cost.UpdateUser = currUser;
            cost.UpdatePg = currPg;
            _context.TCost.Add(cost);

            _context.SaveChanges();
            TCost currCost = modelLogic.GetCurrentCost(int.Parse(currUser), currDate, currPg);
            try
            {
                List<TPaySubscribe> payBaseList = _context.TPaySubscribe.Where(p => p.SubscribeId == SubscribeId && p.status == 1).ToList();
                foreach (TPaySubscribe basePay in payBaseList)
                {
                    TPay pay = new TPay();
                    pay.status = status;
                    pay.CostId = currCost.CostId;
                    pay.PayId = basePay.PayId;
                    pay.UserId = basePay.UserId;
                    pay.PayAmount = basePay.PayAmount;
                    pay.CreatedDate = currDate;
                    pay.CreateUser = currUser;
                    pay.CreatePg = currPg;
                    pay.UpdatedDate = currDate;
                    pay.UpdateUser = currUser;
                    pay.UpdatePg = currPg;
                    _context.TPay.Add(pay);
                }

                List<TRepaySubscribe> repayBaseList = _context.TRepaySubscribe.Where(p => p.SubscribeId == SubscribeId && p.status == 1).ToList();
                foreach (TRepaySubscribe baseRepay in repayBaseList)
                {
                    TRepay repay = new TRepay();
                    repay.status = status;
                    repay.CostId = currCost.CostId;
                    repay.RepayId = baseRepay.RepayId;
                    repay.UserId = baseRepay.UserId;
                    repay.RepayAmount = baseRepay.RepayAmount;
                    repay.CreatedDate = currDate;
                    repay.CreateUser = currUser;
                    repay.CreatePg = currPg;
                    repay.UpdatedDate = currDate;
                    repay.UpdateUser = currUser;
                    repay.UpdatePg = currPg;
                    _context.TRepay.Add(repay);
                }

                TSubscribe subscribe = new TSubscribe();
                subscribe.status = (int)statusEnum.未精算;
                subscribe.SubscribeId = SubscribeId;
                subscribe.CostId = currCost.CostId;
                subscribe.SubscribedDate = date;
                subscribe.CreatedDate = currDate;
                subscribe.CreateUser = currUser;
                subscribe.CreatePg = currPg;
                subscribe.UpdatedDate = currDate;
                subscribe.UpdateUser = currUser;
                subscribe.UpdatePg = currPg;
                _context.TSubscribe.Add(subscribe);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // TPay、TRepayのインサートに失敗した場合、TCostがごみデータになるので論理削除する
                currCost.status = (int)statusEnum.削除;
                currCost.UpdatedDate = currDate;
                currCost.UpdateUser = currUser;
                currCost.UpdatePg = currPg;
                _context.SaveChanges();
            }

            return cost.GenreName + "の" + cost.CostAmount.ToString("C0", new CultureInfo("ja-JP")) + "を" + date.ToString("yyyy/MM/dd") + "日付で仮登録しました。";
        }
    }
}
