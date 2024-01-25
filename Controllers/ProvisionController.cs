using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    public class ProvisionController : Controller
    {
        private readonly WarikakeWebContext _context;

        public ProvisionController(WarikakeWebContext context)
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

            WarikakeIndex warikakeIndex = new WarikakeIndex();

            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueryList = warikakeQuery.GetProvisionWarikakeQueries((int)GroupId);
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            warikakeIndex.warikakeDisps = warikakeDisp.GetWarikakeDisps(warikakeQueryList);

            return View(warikakeIndex);
        }

        // GET: ProvisionController/Edit/5
        public ActionResult Edit(int id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueryList = warikakeQuery.GetProvisionQueries((int)GroupId, id);

            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp wariDisp = warikakeDisp.GetWarikakeDisp(warikakeQueryList);

            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, wariDisp.GenreId);

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            return View(wariDisp);
        }

        // POST: ProvisionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WarikakeDisp input)
        {
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
                warikakeQuery.UpdateLogic(input, (int)GroupId, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(input);
            }

        }

        // GET: ProvisionController/Delete/5
        public ActionResult Delete(int id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // 対象データの表示欄
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueryList = warikakeQuery.GetProvisionQueries((int)GroupId, id);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp wariDisp = warikakeDisp.GetWarikakeDisp(warikakeQueryList);

            // 未精算メッセージのセット
            WarikakeProcess warikakeProcess = new WarikakeProcess();
            ViewBag.WariProcResult = warikakeProcess.repayMessage(warikakeQueryList);

            return View(wariDisp);
        }

        // POST: ProvisionController/Delete
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

        // GET: ProvisionController/Registrate/5
        public ActionResult Registrate(int id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }

            // DB検索
            WarikakeQuery warikakeQuery = new WarikakeQuery(_context);
            List<WarikakeQuery> warikakeQueryList = warikakeQuery.GetProvisionQueries((int)GroupId, id);
            // 画面表示向けに編集
            WarikakeDisp warikakeDisp = new WarikakeDisp();
            WarikakeDisp wariDisp = warikakeDisp.GetWarikakeDisp(warikakeQueryList);


            // 種別プルダウンのセット
            MGenre mGenre = new MGenre(_context);
            ViewBag.Genres = mGenre.GetSelectList((int)GroupId, wariDisp.GenreId);

            // 端数を優先するユーザー情報をセット
            ViewBag.qid = UserId;

            return View(wariDisp);
        }

        // POST: ProvisionController/Registrate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrate(WarikakeDisp input)
        {

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

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(input);
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
