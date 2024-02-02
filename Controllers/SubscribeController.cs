using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    [Authorize]
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

            // 画面表示処理
            SubscribeQuery subscribeQuery = new SubscribeQuery(_context);
            List<SubscribeQuery> subscribeQueries = subscribeQuery.GetSubscribeQueries((int)GroupId);
            SubscribeDisp subscribeDisp = new SubscribeDisp();
            List<SubscribeDisp> subscribeList = subscribeDisp.GetSubscribeDisps(subscribeQueries);

            return View(subscribeList);
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
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId);
           // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // グループのユーザー取得
            MUser mUser = new MUser(_context);
            List<MUser> users = mUser.GetUsers((int)GroupId);

            // 画面表示情報として編集
            SubscribeDisp subscribeDisp = new SubscribeDisp();
            SubscribeDisp input = subscribeDisp.GetBlankSubscribeDisp(users, (int)UserId);

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

            // グループ情報に応じた登録画面を表示
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId);
            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            // 業務入力チェック
            int errCnt = ValidateLogic(input);
            if (0 < errCnt)
            {
                return View(input);
            }

            try
            {
                // 登録処理
                SubscribeQuery subscribeQuery = new SubscribeQuery(_context);
                subscribeQuery.CreateLogic(input, (int)GroupId, (int)UserId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(input);
            }
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

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // 対象情報取得
            SubscribeQuery subscribeQuery = new SubscribeQuery(_context);
            List<SubscribeQuery> subscribeQueries = subscribeQuery.GetOneSubscribeQueries((int)GroupId, id);

            // 画面表示用に編集
            SubscribeDisp subscribeDisp = new SubscribeDisp();
            SubscribeDisp subscDisp = subscribeDisp.GetSubscribeDisp(subscribeQueries);

            // 日付設定を検索
            TDateSubscribe dateSubscribe = new TDateSubscribe(_context);
            TDateSubscribe mwd = dateSubscribe.GetDateSubscribe(subscDisp.SubscribeId);
            subscDisp.dateSubscribe = mwd;
            if(mwd.m1 && mwd.m2 && mwd.m3 && mwd.m4 && mwd.m5 && mwd.m6 && mwd.m7 && mwd.m8 && mwd.m9 && mwd.m10 && mwd.m11 && mwd.m12)
            {
                subscDisp.isAllMonth = true;
            }
            else
            {
                subscDisp.isAllMonth = false;
            }
            if (mwd.r1 || mwd.r2 || mwd.r3 || mwd.r4 || mwd.r5)
            {
                if(mwd.w1 || mwd.w2 || mwd.w3 || mwd.w4 || mwd.w5 || mwd.w6 | mwd.w7)
                {
                    subscDisp.weekOrDay = "week";
                }
            }
            else if (mwd.d1 || mwd.d2 || mwd.d3 || mwd.d4 || mwd.d5 || mwd.d6 || mwd.d7 || mwd.d8 || mwd.d9 || mwd.d10
                || mwd.d11 || mwd.d12 || mwd.d13 || mwd.d14 || mwd.d15 || mwd.d16 || mwd.d17 || mwd.d18 || mwd.d19 || mwd.d20
                || mwd.d21 || mwd.d22 || mwd.d23 || mwd.d24 || mwd.d25 || mwd.d26 || mwd.d27 || mwd.d28 || mwd.d29 || mwd.d30 || mwd.d31)
            {
                subscDisp.weekOrDay = "day";
            }
            else
            {
                subscDisp.weekOrDay = "";
            }

            // グループ情報に応じた登録画面を表示
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, subscDisp.GenreId);

            return View(subscDisp);
        }

        // POST: SubscribeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SubscribeDisp input)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            // グループ情報に応じた登録画面を表示
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId);            
            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(input);
            }
            // 業務入力チェック
            int errCnt = ValidateLogic(input);
            if (0 < errCnt)
            {
                return View(input);
            }
            try
            {
                // 更新処理
                SubscribeQuery subscribeQuery = new SubscribeQuery(_context);
                subscribeQuery.UpdateLogic(input, (int)UserId, id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(input);
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

            // 対象データの表示欄
            SubscribeQuery subscribeQuery = new SubscribeQuery(_context);
            List<SubscribeQuery> subscribeQueries = subscribeQuery.GetOneSubscribeQueries((int)GroupId, id);

            SubscribeDisp subscribeDisp = new SubscribeDisp();
            SubscribeDisp subscDisp = subscribeDisp.GetSubscribeDisp(subscribeQueries);

            // 日付設定を検索
            TDateSubscribe dateSubscribe = new TDateSubscribe(_context);
            subscDisp.dateSubscribe = dateSubscribe.GetDateSubscribe(subscDisp.SubscribeId);

            return View(subscDisp);
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
                // 論理削除処理
                SubscribeQuery subscribeQuery = new SubscribeQuery(_context);
                subscribeQuery.StatusChangeLogic(id, 9, (int)UserId);

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

            // 一覧表示処理
            SubscribeQuery subscribeQuery = new SubscribeQuery(_context);
            List<SubscribeQuery> subscribeQueries = subscribeQuery.GetSubscribeQueries((int)GroupId);

            SubscribeDisp subscribeDisp = new SubscribeDisp();
            List<SubscribeDisp> subscribeList = subscribeDisp.GetSubscribeDisps(subscribeQueries);

            ViewBag.ResultMessage = null;
            return View(subscribeList);
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


            // 一括仮登録処理
            try
            {
                SubscribeQuery subscribeQuery = new SubscribeQuery(_context);

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
                                createResult = subscribeQuery.createCostData(subid, loopDate, (int)UserId);
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
                                createResult = subscribeQuery.createCostData(subid, loopDate, (int)UserId);
                                messageList.Add(createResult);
                            }

                            int lastDay = DateTime.DaysInMonth(loopDate.Year, loopDate.Month);
                            if (day == lastDay)
                            {
                                if ((dayList.Contains(29) && lastDay < 29) || (dayList.Contains(30) && lastDay < 30) || (dayList.Contains(31) && lastDay < 31))
                                {
                                    createResult = subscribeQuery.createCostData(subid, loopDate, (int)UserId);
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

                // 仮登録対象データなし
                if(messageList.Count == 0)
                {
                    messageList.Add("仮登録するべきデータは無かったようです。");
                }
                ViewBag.ResultMessage = messageList;

                // 一覧表示処理
                List<SubscribeQuery> subscribeQueries = subscribeQuery.GetSubscribeQueries((int)GroupId);
                SubscribeDisp subscribeDisp = new SubscribeDisp();
                List<SubscribeDisp> subscribeList = subscribeDisp.GetSubscribeDisps(subscribeQueries);

                return View(subscribeList);
            }
            catch
            {
                return View(subscribeDisps);
            }
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

    }
}
