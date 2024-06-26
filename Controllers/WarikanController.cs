﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Controllers
{
    [Authorize]
    public class WarikanController : Controller
    {
        private readonly WarikakeWebContext _context;

        public WarikanController(WarikakeWebContext context)
        {
            _context = context;
        }

        // GET: WarikanController
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

            // DB検索
            WarikakeModel model = new WarikakeModel(_context);
            List<WarikakeQuery> unsettledWarikakeQueries = model.GetUnSettledWarikakeQueries((int)GroupId);
            List<WarikakeQuery> unsettledSumWarikakeQueries = model.GetUnSettledSumWarikakeQueries((int)GroupId);
            // 画面表示向けに編集
            WarikakeIndex warikakeIndex = new WarikakeIndex();
            warikakeIndex.warikakeDisps = model.GetWarikakeDisps(unsettledWarikakeQueries);
            warikakeIndex.warikakeSum = model.GetWarikakeDisp(unsettledSumWarikakeQueries);

            // 未精算メッセージのセット
            ViewBag.WarikakeProcResult = model.repayMessage(unsettledSumWarikakeQueries);

            return View(warikakeIndex);
        }

        // GET: WarikanController/Create
        public ActionResult Create()
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId);

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // 画面表示情報を取得
            MUser mUser = new MUser(_context);
            List<MUser> users = mUser.GetUsers((int)GroupId);

            WarikakeModel model = new WarikakeModel(_context);
            WarikakeDisp input = model.GetWarikakeDispInit(users, (int)GroupId, (int)UserId, true);


            // 直前の登録情報を提示
            ViewBag.LastData = model.LastMessage((int)GroupId);

            // 初期値の一部を前回入力値で上書き
            DateTime? prevCostDate = (DateTime?)TempData["prevCostDate"];
            int? prevGenreId = (int?)TempData["prevGenreId"];
            if (prevCostDate != null && prevGenreId != null)
            {
                input.CostDate = (DateTime)prevCostDate;
                input.GenreId = (int)prevGenreId;
            }

            return View(input);
        }

        // POST: WarikanController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WarikakeDisp input)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {input.CostId}");

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, input.GenreId);

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
                WarikakeModel model = new WarikakeModel(_context);
                model.CreateLogic(input, (int)GroupId,(int)UserId);

                // 入力値を自画面遷移後の初期値にセットする準備
                TempData["prevCostDate"] = input.CostDate;
                TempData["prevGenreId"] = input.GenreId;

                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(input);
            }
        }

        // GET: WarikanController/Edit/5
        public ActionResult Edit(int? id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            if (id == null)
            {
                return NotFound();
            }

            // DB検索
            WarikakeModel model = new WarikakeModel(_context);
            List<WarikakeQuery> warikakeQueries = model.GetWarikakeQueries((int)GroupId, (int)id);
            // 画面表示向けに編集
            WarikakeDisp wariDisp = model.GetWarikakeDisp(warikakeQueries);

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, wariDisp.GenreId);

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            return View(wariDisp);
        }

        // POST: WarikanController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WarikakeDisp input)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {input.CostId}");

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, input.GenreId);

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
                WarikakeModel model = new WarikakeModel(_context);
                model.UpdateLogic(input, (int)GroupId,(int)UserId);

                // インデックス画面へ遷移
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(input);
            }
        }

        // GET: WarikanController/ManualSettlement/5
        public ActionResult ManualSettlement()
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // 画面表示情報を取得
            MUser mUser = new MUser(_context);
            List<MUser> users = mUser.GetUsers((int)GroupId);

           WarikakeModel model = new WarikakeModel(_context);
            WarikakeDisp input = model.GetWarikakeDispInit(users,(int)GroupId, (int)UserId,false);

            // 精算処理欄表示
            List<WarikakeQuery> unsettledSumWarikakeQueries = model.GetUnSettledSumWarikakeQueries((int)GroupId);

            // 未精算メッセージのセット
            ViewBag.WarikakeProcResult = model.repayMessage(unsettledSumWarikakeQueries);

            return View(input);
        }

        // POST: WarikanController/ManualSettlement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManualSettlement(WarikakeDisp input)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {input.CostId}");

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, input.GenreId);

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
                WarikakeModel model = new WarikakeModel(_context);
                model.CreateLogic(input, (int)GroupId, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(input);
            }
        }


        // GET: WarikanController/Delete/5
        public ActionResult Delete(int? id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            if (id == null)
            {
                return NotFound();
            }

            // 画面表示情報取得
            // DB検索
            WarikakeModel model = new WarikakeModel(_context);
            List<WarikakeQuery> warikakeQueries = model.GetWarikakeQueries((int)GroupId, (int)id);
            // 画面表示向けに編集
            WarikakeDisp wariDisp = model.GetWarikakeDisp(warikakeQueries);

            // 未精算メッセージのセット
            ViewBag.WarikakeProcResult = model.repayMessage(warikakeQueries);

            return View(wariDisp);
        }

        // POST: WarikanController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, IFormCollection collection)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // 更新処理
                WarikakeModel model = new WarikakeModel(_context);
                model.StatusChangeLogic((int)id, (int)statusEnum.削除, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);

                // 画面表示情報取得
                // DB検索
                WarikakeModel model = new WarikakeModel(_context);
                List<WarikakeQuery> warikakeQueries = model.GetWarikakeQueries((int)GroupId, (int)id);
                // 画面表示向けに編集
                WarikakeDisp wariDisp = model.GetWarikakeDisp(warikakeQueries);

                // 未精算メッセージのセット
                ViewBag.WarikakeProcResult = model.repayMessage(warikakeQueries);
                return View(wariDisp);
            }
        }

        // GET: WarikanController/Registrate/5
        public ActionResult Registrate(int? id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            if (id == null)
            {
                return NotFound();
            }

            // DB検索
            WarikakeModel model = new WarikakeModel(_context);
            List<WarikakeQuery> warikakeQueries = model.GetWarikakeQueries((int)GroupId, (int)id);
            // 画面表示向けに編集
            WarikakeDisp wariDisp = model.GetWarikakeDisp(warikakeQueries);

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, wariDisp.GenreId);

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            return View(wariDisp);
        }

        // POST: WarikanController/Registrate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrate(WarikakeDisp input)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {input.CostId}");

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, input.GenreId);

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
                WarikakeModel model = new WarikakeModel(_context);
                model.UpdateLogic(input, (int)GroupId, (int)UserId, (int)statusEnum.未精算);

                // インデックスに戻る
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(input);
            }
        }

        // GET: WarikanController/Settlement/5
        public ActionResult Settlement(int? id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            if (id == null)
            {
                return NotFound();
            }

            // 画面表示情報取得
            // DB検索
            WarikakeModel model = new WarikakeModel(_context);
            List<WarikakeQuery> warikakeQueries = model.GetWarikakeQueries((int)GroupId, (int)id);
            // 画面表示向けに編集
            WarikakeDisp wariDisp = model.GetWarikakeDisp(warikakeQueries);

            // 未精算メッセージのセット
            ViewBag.WarikakeProcResult = model.repayMessage(warikakeQueries);

            return View(wariDisp);
        }

        // POST: WarikanController/Settlement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settlement(int? id, IFormCollection collection)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: {id}");

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // 更新処理
                WarikakeModel model = new WarikakeModel(_context);
                model.StatusChangeLogic((int)id, (int)statusEnum.精算済, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);

                // 画面表示情報取得
                // DB検索
                WarikakeModel model = new WarikakeModel(_context);
                List<WarikakeQuery> warikakeQueries = model.GetWarikakeQueries((int)GroupId, (int)id);
                // 画面表示向けに編集
                WarikakeDisp wariDisp = model.GetWarikakeDisp(warikakeQueries);

                // 未精算メッセージのセット
                ViewBag.WarikakeProcResult = model.repayMessage(warikakeQueries);

                return View(wariDisp);
            }
        }

        // GET: WarikanController/SettlementAll/5
        public ActionResult SettlementAll(int? id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            if (id == null)
            {
                return NotFound();
            }

            // 画面情報を取得
            WarikakeModel model = new WarikakeModel(_context);
            List<WarikakeQuery> unsettledSumWarikakeQueries = model.GetUnSettledSumWarikakeQueries((int)GroupId);
            // 画面表示向けに編集
            WarikakeDisp warikakeSum = model.GetWarikakeDisp(unsettledSumWarikakeQueries);

            // 未精算メッセージのセット
            ViewBag.WarikakeProcResult = model.repayMessage(unsettledSumWarikakeQueries);

            return View(warikakeSum);
        }

        // POST: WarikanController/SettlementAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SettlementAll(int? id, IFormCollection collection)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}, CostId: none");

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // 一括更新対象取得
                WarikakeModel model = new WarikakeModel(_context);
                List<WarikakeQuery> warikakeQueryList = model.GetUnSettledOrManualWarikakeQueries((int)GroupId);

                // 一括更新
                foreach (WarikakeQuery item in warikakeQueryList)
                {

                    model.StatusChangeLogic(item.CostId, (int)statusEnum.一括精算済, (int)UserId);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);

                // 画面表示情報取得
                // DB検索
                WarikakeModel model = new WarikakeModel(_context);
                List<WarikakeQuery> warikakeQueries = model.GetWarikakeQueries((int)GroupId, (int)id);
                // 画面表示向けに編集
                WarikakeDisp wariDisp = model.GetWarikakeDisp(warikakeQueries);

                // 未精算メッセージのセット
                ViewBag.WarikakeProcResult = model.repayMessage(warikakeQueries);

                return View(wariDisp);
            }
        }

        private int ValidateLogic(WarikakeDisp input)
        {
            // 業務入力チェック
            int errCnt = 0;
            string costTile = input.CostTitle;
            int genreId = input.GenreId;
            int costAmount = input.CostAmount;
            DateTime costDate = input.CostDate;
            DateTime nowDate = DateTime.Now.Date;
            if (nowDate < costDate)
            {
                ModelState.AddModelError(nameof(WarikakeDisp.CostDate), "未来日が入力されています");
                errCnt++;
            }
            int memberCnt = 0;
            int sumPay = 0;
            foreach (WarikakePayDisp pay in input.Pays)
            {
                int payUserId = pay.PayUserId;
                int payAmount = pay.PayAmount;
                sumPay += payAmount;
                memberCnt++;
            }
            int sumRepay = 0;
            foreach (WarikakeRepayDisp repay in input.Repays)
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
                    ModelState.AddModelError(nameof(WarikakeDisp.GenreId), "種別が特定できません");
                    errCnt++;
                }
            }
            int sumR = costAmount % memberCnt;
            if (costAmount != sumPay)
            {
                if (costAmount - sumPay <= sumR && sumPay - costAmount <= sumR)
                {
                    ModelState.AddModelError(nameof(WarikakeDisp.CostAmount), "支払額と立替額が一致しません");
                    errCnt++;
                }
                else
                {
                    ModelState.AddModelError(nameof(WarikakeDisp.CostAmount), "支払額と立替額が一致しません");
                    errCnt++;
                }

            }
            if (costAmount != sumRepay)
            {
                if (costAmount - sumRepay <= sumR && sumRepay - costAmount <= sumR)
                {
                    ModelState.AddModelError(nameof(WarikakeDisp.CostAmount), "支払額と割勘額が一致しません");
                    errCnt++;
                }
                else
                {
                    ModelState.AddModelError(nameof(WarikakeDisp.CostAmount), "支払額と割勘額が一致しません");
                    errCnt++;
                }
            }
            return errCnt;
        }
    }
}
