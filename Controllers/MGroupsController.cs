using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Controllers
{
    [Authorize]
    public class MGroupsController : Controller
    {
        private readonly WarikakeWebContext _context;

        public MGroupsController(WarikakeWebContext context)
        {
            _context = context;
        }

        // GET: MGroups
        public ActionResult Index()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // DB検索し画面表示
            GroupModel model = new GroupModel(_context);
            List<MGroupDisp> groupDisps = model.GetGroupDispList((int)UserId);

            return View(groupDisps);
        }

        // GET: MGroups/Details/5
        public ActionResult Details(int? id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id == null)
            {
                return NotFound();
            }

            // 対象データを取得
            GroupModel model = new GroupModel(_context);
            MGroup mGroup = model.GetGroupById((int)id);
            if (mGroup == null)
            {
                return NotFound();
            }
            List<MGroupQuery> queryList = model.GetGroupQueryList((int)UserId, mGroup.GroupId);
            // 画面表示向けに編集
            MGroupDisp mGroupDisp = model.GetGroupDisp(queryList);

            return View(mGroupDisp);
        }

        // GET: MGroups/Create
        public ActionResult Create()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            MGroupDisp groupDisp = new MGroupDisp();
            return View(groupDisp);
        }

        // POST: MGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("groupName")] MGroupDisp mGroupDisp)
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
                return View(mGroupDisp);
            }

            try 
            {
                // 登録処理
                GroupModel model = new GroupModel(_context);
                model.CreateLogic(mGroupDisp, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(mGroupDisp);
            }
        }

        // GET: MGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id == null)
            {
                return NotFound();
            }
            // 指定グループの表示情報を取得
            GroupModel model = new GroupModel(_context);
            MGroupDisp mGroupDisp = model.DispLogic((int)id);
            if (mGroupDisp == null)
            {
                return NotFound();
            }
            return View(mGroupDisp);
        }

        // POST: MGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,groupName")] MGroupDisp mGroupDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id != mGroupDisp.Id)
            {
                return NotFound();
            }

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(mGroupDisp);
            }
            // 本人チェック
            if (0 < PersonCheck(mGroupDisp, (int)UserId))
            {
                ModelState.AddModelError(nameof(MGroupDisp.groupName), "本人確認が取れません");
                return View(mGroupDisp);
            }

            try
            {
                // 更新処理
                GroupModel model = new GroupModel(_context);
                model.UpdateLogic(mGroupDisp, (int)UserId, (int)id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(mGroupDisp);
            }        
        }

        // GET: MGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id == null)
            {
                return NotFound();
            }

            // データチェック
            GroupModel model = new GroupModel(_context);
            var mGroup = model.GetGroupById((int)id);
            if (mGroup == null)
            {
                return NotFound();
            }

            // 指定グループの表示情報を取得
            MGroupDisp mGroupDisp = model.DispLogic((int)id);
            if (mGroupDisp == null)
            {
                return NotFound();
            }
            return View(mGroupDisp);
        }

        // POST: MGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, [Bind("Id")] MGroupDisp mGroupDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id == null)
            {
                return NotFound();
            }

            // 本人チェック
            if (0 < PersonCheck(mGroupDisp, (int)UserId))
            {
                ModelState.AddModelError(nameof(MGroupDisp.Id), "本人確認が取れません");
                return View(mGroupDisp);
            }

            // 対象データを取得
            GroupModel model = new GroupModel(_context);
            var mGroup = model.GetGroupById((int)id);
            if (mGroup == null)
            {
                return NotFound();
            }

            try
            {
                // 更新処理
                model.StatusChangeLogic(mGroup, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(mGroupDisp);
            }
        }

        private int PersonCheck(MGroupDisp mGroupDisp, int UserId)
        {
            GroupModel model = new GroupModel(_context);    
            int retInt = 0;
            MGroup group = model.GetGroupById(mGroupDisp.Id);
            if (group.UserId != UserId)
            {
                retInt++;
            }
            return retInt;
        }
    }
}
