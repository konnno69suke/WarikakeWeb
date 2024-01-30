using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    public class MMembersController : Controller
    {
        private readonly WarikakeWebContext _context;

        public MMembersController(WarikakeWebContext context)
        {
            _context = context;
        }

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

            // 所有グループのみ表示可
            List<MMemberDisp> memberList = _context.Database.SqlQuery<MMemberDisp>($@"
                select mm.Id, 0 gid, mg.groupname, 0 mid, mu.username
                from MMember mm inner join mgroup mg on mm.groupid = mg.groupid
                inner join muser mu on mm.userid = mu.userid
                where mg.UserId = {UserId}
                and mm.status = 1 and mg.status = 1 and mu.status = 1
                order by mm.GroupId, mm.UserId").ToList();

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                ViewBag.Message = "本人確認が取れません";
                return View(mMemberDisp);
            }

            if (ModelState.IsValid)
            {
                // 業務入力チェック
                if(0 < ValidateLogic(mMemberDisp))
                {
                    return View(mMemberDisp);
                }

                DateTime currTime = DateTime.Now;
                string currPg = "MMemberInsert";

                MMember mMember = new MMember();
                mMember.status = 1;
                mMember.GroupId = mMemberDisp.gid;
                mMember.UserId = mMemberDisp.mid;
                mMember.CreatedDate = currTime;
                mMember.CreateUser = UserId.ToString();
                mMember.CreatePg = currPg;
                mMember.UpdatedDate = currTime;
                mMember.UpdateUser = UserId.ToString();
                mMember.UpdatePg = currPg;
                _context.Add(mMember);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(mMemberDisp);
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

            MMember mMember = _context.MMember.FirstOrDefault<MMember>(m => m.Id == id && m.status == 1);
            if (mMember == null)
            {
                return NotFound();
            }

            // 指定メンバーの表示情報を取得
            MMemberDisp mMemberDisp = _context.Database.SqlQuery<MMemberDisp>($@"
                select mm.id, mm.groupid gid, mg.groupname, mm.userid mid, mu.username
                from mmember mm inner join mgroup mg on mm.groupid = mg.groupid
                inner join muser mu on mm.userid = mu.userid
                where mm.id = {id}
                and mm.status = 1 and mg.status = 1 and mu.status = 1").FirstOrDefault();

            return View(mMemberDisp);
        }

        // POST: MMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, [Bind("Id,groupName,userName")] MMemberDisp mMemberDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 業務入力チェック
            if (0 < DelPersonCheck(mMemberDisp, (int)UserId))
            {
                return View(mMemberDisp);
            }
            var mMember = _context.MMember.Find(id);
            if (mMember != null)
            {
                try
                {
                    DateTime currTime = DateTime.Now;
                    string currPg = "MMembersDelete";

                    mMember.status = (int)statusEnum.削除;
                    mMember.UpdatedDate = currTime;
                    mMember.UpdateUser = UserId.ToString();
                    mMember.UpdatePg = currPg;
                    _context.Update(mMember);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    // todo パラメータなしのdelete画面では表示エラーになるのでは
                    return View();
                }

            }

            return RedirectToAction(nameof(Index));
        }

        private int ValidateLogic(MMemberDisp mMemberDisp)
        {
            int retInt = 0;

            // 重複する組み合わせでは登録できない
            if (_context.MMember.Any(m => m.GroupId == mMemberDisp.gid && m.UserId == mMemberDisp.mid && m.status == 1))
            {
                ViewBag.Message = "the user already joining in this group.";
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
            int retInt = 0;
            Boolean res = _context.Database.SqlQuery<int>($@"
                select mg.userid
                from mgroup mg inner join mmember mm on mg.groupid = mm.groupid
                where mm.id = {mMemberDisp.Id}
                and mg.userid = {UserId}
                and mg.status = 1 and mm.status = 1").Any();
            if (!res)
            {
                ViewBag.Message = "you cannot delete any user in this group.";
                retInt++;
            }
            // 自分自身は除名できない
            MMember mMember = _context.MMember.FirstOrDefault(m => m.Id == mMemberDisp.Id && m.status == 1);
            if(mMember.UserId == UserId)
            {
                ViewBag.Message = "you cannot delete yourself.";
                retInt++;
            }
            return retInt;
        }
    }
}
