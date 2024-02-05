using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Controllers
{
    [Authorize]
    public class MMembersController : Controller
    {
        private readonly WarikakeWebContext _context;

        public MMembersController(WarikakeWebContext context)
        {
            _context = context;
        }
        /*
        // GET: MMembers
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

            // 検索処理
            MemberModel model = new MemberModel(_context);
            List<MMemberDisp> memberList = model.GetGroupMemberList((int)UserId);

            return View(memberList);
        }

        // GET: MMembers/Create
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

            List<MGroup> groups = _context.MGroup.Where(g => g.UserId == (int)UserId && g.status == 1).ToList();
            ViewBag.Groups = new SelectList(groups.Select(g => new { Id = g.GroupId, Name = g.GroupName}), "Id", "Name");
            List<MUser> users = _context.MUser.Where(u => u.status == 1).ToList();
            ViewBag.Users = new SelectList(users.Select(u => new { Id = u.UserId, Name = u.UserName }), "Id", "Name");

            return View();
        }

        // POST: MMembers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("gid,mid")] MMemberDisp mMemberDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            List<MGroup> groups = _context.MGroup.Where(g => g.UserId == (int)UserId && g.status == 1).ToList();
            ViewBag.Groups = new SelectList(groups.Select(g => new { Id = g.GroupId, Name = g.GroupName }), "Id", "Name");
            List<MUser> users = _context.MUser.Where(u => u.status == 1).ToList();
            ViewBag.Users = new SelectList(users.Select(u => new { Id = u.UserId, Name = u.UserName }), "Id", "Name");

            // 本人チェック
            if (0 < InsPersonCheck(mMemberDisp, (int)UserId))
            {
                ModelState.AddModelError(nameof(MMemberDisp.gid), "本人確認が取れません");

                return View(mMemberDisp);
            }

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(mMemberDisp);
            }
            // 業務入力チェック
            if (0 < ValidateLogic(mMemberDisp))
            {
                return View(mMemberDisp);
            }
            try
            {
                // 登録処理
                MemberModel model = new MemberModel(_context);
                model.CreateLogic(mMemberDisp, (int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(mMemberDisp);
            }
        }

        // GET: MMembers/Delete/5
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

            // 指定メンバーの表示情報を取得
            MemberModel model = new MemberModel(_context);
            MMemberDisp mMemberDisp = model.GetMemberDispById((int)id);

            return View(mMemberDisp);
        }

        // POST: MMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, [Bind("Id,groupName,userName")] MMemberDisp mMemberDisp)
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

            // 業務入力チェック
            if (0 < DelPersonCheck(mMemberDisp, (int)UserId))
            {
                return View(mMemberDisp);
            }

            // 対象を検索
            MemberModel model = new MemberModel(_context);
            MMember mMember = model.GetMemberById((int)id);
            if (mMember == null)
            {
                return NotFound();
            }
            try
            {
                // 更新処理
                model.StatusChangeLogic(mMember,(int)UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                // 指定メンバーの表示情報を取得
                return View(mMemberDisp);
            }
        }

        private int ValidateLogic(MMemberDisp mMemberDisp)
        {
            int retInt = 0;

            // 重複する組み合わせでは登録できない
            if (_context.MMember.Any(m => m.GroupId == mMemberDisp.gid && m.UserId == mMemberDisp.mid && m.status == 1))
            {
                ModelState.AddModelError(nameof(MMemberDisp.mid), "既にグループに入っているユーザーです");
                retInt++;
            }
            return retInt;
        }

        private int InsPersonCheck(MMemberDisp mMemberDisp, int UserId)
        {
            int retInt = 0;
            MGroup group = _context.MGroup.FirstOrDefault(g => g.GroupId == mMemberDisp.gid && g.status == 1);
            if (group.UserId != UserId)
            {
                retInt++;
            }
            return retInt;
        }

        private int DelPersonCheck(MMemberDisp mMemberDisp, int UserId)
        {
            // 自分のグループは除名できない
            MemberModel model = new MemberModel(_context);
            int retInt = 0;
            bool res = _context.Database.SqlQuery<int>($@"
                select mg.userid
                from mgroup mg inner join mmember mm on mg.groupid = mm.groupid
                where mm.id = {mMemberDisp.Id}
                and mg.userid = {UserId}
                and mg.status = 1 and mm.status = 1").Any();
            if (!res)
            {
                ModelState.AddModelError(nameof(MMemberDisp.gid), "自身をグループからは除名できません");
                retInt++;
            }
            // 自分自身は除名できない
            MMember mMember = model.GetMemberById(mMemberDisp.Id);
            if(mMember.UserId == UserId)
            {
                ModelState.AddModelError(nameof(MMemberDisp.mid), "自身をグループからは除名できません");
                retInt++;
            }
            return retInt;
        }
        */
    }
}
