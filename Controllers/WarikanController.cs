using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using WarikakeWeb.Data;
using WarikakeWeb.Logic;
//using WarikakeWeb.Migrations;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
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
            ViewBag.GroupName = HttpContext.Session.GetString("GroupName");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            // 戻り値の準備
            WarikakeIndex warikakeIndex = new WarikakeIndex();

            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> unsettledWarikakeQueries = warikakeQuery.GetUnSettledWarikakeQueries((int)GroupId);
            List<WarikakeQuery> unsettledSumWarikakeQueries = warikakeQuery.GetUnSettledSumWarikakeQueries((int)GroupId);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            warikakeIndex.warikakeDisps = warikakeDisp.GetWarikakeDisps(unsettledWarikakeQueries);
            warikakeIndex.warikakeSum = warikakeDisp.GetWarikakeDisp(unsettledSumWarikakeQueries);

            // 未精算メッセージのセット
            WarikakeProcess warikakeProcess = new WarikakeProcess();
            ViewBag.WarikakeProcResult = warikakeProcess.repayMessage(unsettledSumWarikakeQueries);

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

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId);

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // 画面表示情報を取得
            MUser mUser = new MUser(_context);
            List<MUser> users = mUser.GetUsers((int)GroupId);

            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp input = warikakeDisp.GetWarikakeDispInit(users, (int)UserId, true);


            // 直前の登録情報を提示
            ViewBag.LastData = LastMessage((int)GroupId);

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
                WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
                warikakeQuery.CreateLogic(input, (int)GroupId,(int)UserId);

                // 入力値を自画面遷移後の初期値にセットする準備
                TempData["prevCostDate"] = input.CostDate;
                TempData["prevGenreId"] = input.GenreId;

                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                return View(input);
            }
        }

        // GET: WarikanController/Edit/5
        public ActionResult Edit(int id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetWarikakeQueries((int)GroupId, id);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp wariDisp = warikakeDisp.GetWarikakeDisp(warikakeQueries);

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
                WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
                warikakeQuery.UpdateLogic(input, (int)GroupId,(int)UserId);

                // インデックス画面へ遷移
                return RedirectToAction(nameof(Index));
            }
            catch
            {
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

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            // 画面表示情報を取得
            MUser mUser = new MUser(_context);
            List<MUser> users = mUser.GetUsers((int)GroupId);

            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp input = warikakeDisp.GetWarikakeDispInit(users,(int)UserId,false);

            // 精算処理欄表示
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> unsettledSumWarikakeQueries = warikakeQuery.GetUnSettledSumWarikakeQueries((int)GroupId);

            WarikakeProcess warikakeProcess = new WarikakeProcess();
            ViewBag.WariProcResult = warikakeProcess.repayMessage(unsettledSumWarikakeQueries);

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
                WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
                warikakeQuery.CreateLogic(input, (int)GroupId, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(input);
            }
        }


        // GET: WarikanController/Delete/5
        public ActionResult Delete(int id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // 画面表示情報取得
            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetWarikakeQueries((int)GroupId, id);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp wariDisp = warikakeDisp.GetWarikakeDisp(warikakeQueries);

            // 未精算メッセージのセット
            WarikakeProcess warikakeProcess = new WarikakeProcess();
            ViewBag.WariProcResult = warikakeProcess.repayMessage(warikakeQueries);

            return View(wariDisp);
        }

        // POST: WarikanController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            try
            {
                // 更新処理
                WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
                warikakeQuery.StatusChangeLogic(id, (int)statusEnum.削除, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(id);
            }
        }
        // GET: WarikanController/Registrate/5
        public ActionResult Registrate(int id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetWarikakeQueries((int)GroupId, id);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp wariDisp = warikakeDisp.GetWarikakeDisp(warikakeQueries);

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
                WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
                warikakeQuery.UpdateLogic(input, (int)GroupId, (int)UserId, (int)statusEnum.未精算);

                // インデックスに戻る
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return View(input);
            }
        }

        // GET: WarikanController/Settlement/5
        public ActionResult Settlement(int id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // 画面表示情報取得
            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueries = warikakeQuery.GetWarikakeQueries((int)GroupId, id);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp wariDisp = warikakeDisp.GetWarikakeDisp(warikakeQueries);

            WarikakeProcess warikakeProcess = new WarikakeProcess();
            ViewBag.WariProcResult = warikakeProcess.repayMessage(warikakeQueries);

            return View(wariDisp);
        }

        // POST: WarikanController/Settlement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settlement(int id, IFormCollection collection)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            try
            {
                // 更新処理
                WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
                warikakeQuery.StatusChangeLogic(id, (int)statusEnum.精算済, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WarikanController/SettlementAll/5
        public ActionResult SettlementAll(int id)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // 画面情報を取得
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> unsettledSumWarikakeQueries = warikakeQuery.GetUnSettledSumWarikakeQueries((int)GroupId);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp warikakeSum = warikakeDisp.GetWarikakeDisp(unsettledSumWarikakeQueries);

            // 未精算メッセージのセット
            WarikakeProcess warikakeProcess = new WarikakeProcess();
            ViewBag.WariProcResult = warikakeProcess.repayMessage(unsettledSumWarikakeQueries);

            return View(warikakeSum);
        }

        // POST: WarikanController/SettlementAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SettlementAll(int id, IFormCollection collection)
        {
            // セッション取得
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            try
            {
                // 一括更新対象取得
                WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
                List<WarikakeQuery> warikakeQueryList = warikakeQuery.GetUnSettledOrManualWarikakeQueries((int)GroupId);

                // 一括更新
                foreach (WarikakeQuery item in warikakeQueryList)
                {

                    warikakeQuery.StatusChangeLogic(item.CostId, (int)statusEnum.一括精算済, (int)UserId);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
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

        private String LastMessage(int GroupId)
        {
            StringBuilder sb = new StringBuilder("");
            TCost cost = _context.TCost.Where(c => c.GroupId == (int)GroupId).OrderByDescending(c => c.UpdatedDate).FirstOrDefault();
            if (cost != null && cost.CostId != null)
            {
                MUser user = _context.MUser.Where(u => u.UserId == int.Parse(cost.UpdateUser)).FirstOrDefault();
                String formatDate = cost.CostDate.ToString("yyyy/MM/dd");
                
                sb.Append("※直前に登録されたデータは").Append(cost.UpdatedDate).Append("に").Append(user.UserName).Append("が登録した").Append(formatDate).Append("分の").Append(cost.CostAmount).Append("円の").Append(cost.GenreName).Append("です。");
            }
            return sb.ToString();
        }
    }
}
