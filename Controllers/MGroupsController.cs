using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

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

            List<MGroup> groups = _context.MGroup.Where(g => g.status == 1).ToList();

            Serilog.Log.Information($"SQL param:{UserId}");
            List<MGroupDisp> groupDisps = _context.Database.SqlQuery<MGroupDisp>($@"
                select mg.id, mg.groupname, mv.username mastername, mg.startdate
                from mgroup mg inner join mmember mm on mg.groupid = mm.groupid
                inner join muser mu on mm.userid = mu.userid
                inner join muser mv on mg.userid = mv.userid
                where mu.userid = {UserId}
                and mg.status = 1 and mm.status = 1 and mu.status = 1 and mv.status = 1
                order by mg.startdate desc").ToList();
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

            var mGroup = _context.MGroup.FirstOrDefault(m => m.Id == id && m.status == 1);
            if (mGroup == null)
            {
                return NotFound();
            }

            Serilog.Log.Information($"SQL param:{UserId}, {mGroup.GroupId}");
            List<MGroupQuery> queryList = _context.Database.SqlQuery<MGroupQuery>($@"
                select mg.id, mg.groupname, mv.username mastername, mg.startdate,
                mu.Id memid, mu.username memname, mu.startdate memstartdate 
                from mgroup mg inner join mmember mm on mg.groupid = mm.groupid
                inner join muser mu on mm.userid = mu.userid
                inner join muser mv on mg.userid = mv.userid
                where mu.userid = {UserId}
                and mg.groupid = {mGroup.GroupId}
                and mg.status = 1 and mm.status = 1 and mu.status = 1 and mv.status = 1
                order by mg.startdate desc").ToList();
            int currId = -1;
            MGroupDisp mGroupDisp = new MGroupDisp();
            foreach (MGroupQuery query in queryList)
            {
                if(currId != query.Id)
                {
                    mGroupDisp = new MGroupDisp();
                    mGroupDisp.Id = query.Id;
                    mGroupDisp.groupName = query.groupName;
                    mGroupDisp.masterName = query.masterName;
                    mGroupDisp.startDate = query.startDate;
                    currId = query.Id;
                }
                MUserDisp mUserDisp = new MUserDisp();
                mUserDisp.Id = query.memId;
                mUserDisp.UserName = query.memName;
                mUserDisp.StartDate = query.memStartDate;
                mGroupDisp.userList.Add(mUserDisp);
            }
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            if (ModelState.IsValid)
            {

                int newGroupId = getNextGroupId();
                DateTime currTime = DateTime.Now;
                string currPg = "MGroupsInsert";

                MGroup mGroup = new MGroup();
                mGroup.status = 1;
                mGroup.GroupId = newGroupId;
                mGroup.GroupName = mGroupDisp.groupName;
                mGroup.UserId = (int)UserId;
                mGroup.StartDate = currTime.Date;
                mGroup.CreatedDate = currTime;
                mGroup.CreateUser = UserId.ToString();
                mGroup.CreatePg = currPg;
                mGroup.UpdatedDate = currTime;
                mGroup.UpdateUser = UserId.ToString();
                mGroup.UpdatePg = currPg;
                Serilog.Log.Information($"SQL param: MGroup:{mGroup.ToString()}");
                _context.Add(mGroup);

                MMember mMember = new MMember();
                mMember.status = 1;
                mMember.GroupId = newGroupId;
                mMember.UserId = (int)UserId;
                mMember.CreatedDate = currTime;
                mMember.CreateUser = UserId.ToString();
                mMember.CreatePg = currPg;
                mMember.UpdatedDate = currTime;
                mMember.UpdateUser = UserId.ToString();
                mMember.UpdatePg = currPg;
                Serilog.Log.Information($"SQL param: MMember:{mMember.ToString()}");
                _context.Add(mMember);

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(mGroupDisp);
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
            MGroupDisp mGroupDisp = DispLogic((int)id);
            if (mGroupDisp == null)
            {
                return NotFound();
            }
            return View(mGroupDisp);
        }

        // POST: MGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            if (ModelState.IsValid)
            {
                // 本人チェック
                if (0 < PersonCheck(mGroupDisp, (int)UserId))
                {
                    ViewBag.Message = "本人確認が取れません";
                    return View(mGroupDisp);
                }

                try
                {
                    DateTime currDate = DateTime.Now;
                    string currPg = "MGroupsUpdate";

                    MGroup existingGroup = _context.MGroup.FirstOrDefault(g => g.Id == id && g.status == 1);
                    existingGroup.GroupName = mGroupDisp.groupName;
                    existingGroup.UpdatedDate = currDate;
                    existingGroup.UpdateUser = UserId.ToString();
                    existingGroup.UpdatePg = currPg;
                    Serilog.Log.Information($"SQL param: MGroup:{existingGroup.ToString()}");
                    _context.Update(existingGroup);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View(mGroupDisp);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mGroupDisp);
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

            var mGroup = _context.MGroup.FirstOrDefault(g => g.Id == id && g.status == 1);
            if (mGroup == null)
            {
                return NotFound();
            }

            // 指定グループの表示情報を取得
            MGroupDisp mGroupDisp = DispLogic((int)id);
            if (mGroupDisp == null)
            {
                return NotFound();
            }
            return View(mGroupDisp);
        }

        // POST: MGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, [Bind("Id")] MGroupDisp mGroupDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 本人チェック
            if (0 < PersonCheck(mGroupDisp, (int)UserId))
            {
                ViewBag.Message = "本人確認が取れません";
                return View(mGroupDisp);
            }

            var mGroup = _context.MGroup.FirstOrDefault(g => g.Id == id && g.status == 1);
            if (mGroup != null)
            {
                try
                {
                    string currPg = "MGroupsDelete";
                    DateTime currTime = DateTime.Now;

                    mGroup.status = (int)statusEnum.削除;
                    mGroup.UpdatedDate = currTime;
                    mGroup.UpdateUser = UserId.ToString();
                    mGroup.UpdatePg = currPg;
                    Serilog.Log.Information($"SQL param: MGroup:{mGroup.ToString()}");
                    _context.MGroup.Update(mGroup);
                    _context.SaveChanges();

                    List<MMember> memberList = _context.MMember.Where(m => m.GroupId == mGroup.GroupId && m.status == 1).ToList();
                    foreach (MMember member in memberList)
                    {
                        member.status = (int)statusEnum.削除;
                        member.UpdatedDate = currTime;
                        member.UpdateUser = UserId.ToString();
                        member.UpdatePg = currPg;
                        Serilog.Log.Information($"SQL param: MMember:{member.ToString()}");
                        _context.MMember.Update(member);
                    }
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View(mGroupDisp);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private int getNextGroupId()
        {
            int GroupId = _context.MGroup.Any() ? _context.MGroup.Max(a => a.GroupId) : -0;

            GroupId++;

            return GroupId;
        }

        private MGroupDisp DispLogic(int id)
        {
            MGroup mGroup = _context.MGroup.FirstOrDefault(m => m.Id == id && m.status == 1);
            if (mGroup == null)
            {
                return null;
            }
            MGroupDisp mGroupDisp = new MGroupDisp();
            mGroupDisp.Id = mGroup.Id;
            mGroupDisp.groupName = mGroup.GroupName;
            mGroupDisp.startDate = mGroup.StartDate;

            return mGroupDisp;
        }

        private int PersonCheck(MGroupDisp mGroupDisp, int UserId)
        {
            int retInt = 0;
            MGroup group = _context.MGroup.FirstOrDefault(g => g.Id == mGroupDisp.Id && g.status == 1);
            if(group.UserId != UserId)
            {
                retInt++;
            }
            return retInt;
        }
    }
}
