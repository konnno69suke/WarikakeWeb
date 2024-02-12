using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Models
{
    public class GroupModel
    {
        WarikakeWebContext _context;

        public GroupModel(WarikakeWebContext context)
        {
            _context = context;
        }

        // ユーザーに紐づくグループのリストを取得
        public List<MGroupDisp> GetGroupDispList(int UserId)
        {
            Serilog.Log.Information($"SQL param:{UserId}");
            List<MGroupDisp> groupDisps = _context.Database.SqlQuery<MGroupDisp>($@"
                select mg.id, mg.groupname, mv.username mastername, mg.startdate
                from mgroup mg inner join mmember mm on mg.groupid = mm.groupid
                inner join muser mu on mm.userid = mu.userid
                inner join muser mv on mg.userid = mv.userid
                where mu.userid = {UserId}
                and mg.status = 1 and mm.status = 1 and mu.status = 1 and mv.status = 1
                order by mg.startdate desc").ToList();
            return groupDisps;
        }

        // グループIDに紐づくグループ情報を取得
        public List<MGroupQuery> GetGroupLeaderQueryList(int GroupId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}");
            List<MGroupQuery> queryList = _context.Database.SqlQuery<MGroupQuery>($@"
                select mg.id, mg.groupname, mv.username mastername, mg.startdate,
                mu.Id memid, mu.username memname, mu.startdate memstartdate 
                from mgroup mg 
                inner join mmember mm on mg.groupid = mm.groupid
                inner join muser mu on mm.userid = mu.userid
                inner join muser mv on mg.userid = mv.userid
                where mg.groupid = {GroupId}
                and mg.status = 1 and mm.status = 1 and mu.status = 1 and mv.status = 1
                order by mg.startdate desc").ToList();

            return queryList;
        }

        // ユーザーとグループIDに紐づくグループ情報を取得
        public List<MGroupQuery> GetGroupMemberQueryList(int GroupId, int UserId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {UserId}");
            List<MGroupQuery> queryList = _context.Database.SqlQuery<MGroupQuery>($@"
                select mg.id, mg.groupname, mv.username mastername, mg.startdate,
                mu.Id memid, mu.username memname, mu.startdate memstartdate 
                from mgroup mg 
                inner join mmember mm on mg.groupid = mm.groupid
                inner join muser mu on mm.userid = mu.userid
                inner join muser mv on mg.userid = mv.userid
                where mu.userid = {UserId}
                and mg.groupid = {GroupId}
                and mg.status = 1 and mm.status = 1 and mu.status = 1 and mv.status = 1
                order by mg.startdate desc").ToList();

            return queryList;
        }

        // 対象者がメンバーか確認する
        public bool chkGroupMember(int GroupId, int UserId)
        {
            bool isMember = _context.MMember.Any(m => m.GroupId == GroupId && m.UserId == UserId && m.status == 1);
            return isMember;
        }

        // idでグループ情報を取得
        public MGroup GetGroupById(int id)
        {
            MGroup mGroup = _context.MGroup.Where(m => m.Id == id && m.status == 1).FirstOrDefault();
            return mGroup;
        }
        public MGroup GetGroupByGroupId(int GroupId)
        {
            MGroup group = _context.MGroup.Where(g => g.GroupId == GroupId && g.status == 1).FirstOrDefault();
            return group;
        }


        public MGroup GetGroupByGroupName(string GroupName)
        {
            MGroup group = _context.MGroup.Where(g => g.GroupName == GroupName && g.status == 1).FirstOrDefault();
            return group;
        }

        public MGroup GetGroupByGroupName(string GroupName, int UserId)
        {
            MGroup group = _context.MGroup.Where(g => g.GroupName == GroupName && g.UserId == UserId && g.status == 1).FirstOrDefault();
            return group;
        }

        public MGroup GetDupliGroupByGroupName (string GroupName, int UserId)
        {
            MGroup group = _context.MGroup.Where(g=> g.GroupName == GroupName && g.UserId != UserId && g.status == 1).FirstOrDefault();
            return group;
        }

        // グループIDで指定されたメンバーリストを取得
        public List<MMember> GetMemberByGroupId(int GroupId)
        {
            List<MMember> memberList = _context.MMember.Where(m => m.GroupId == GroupId && m.status == 1).ToList();
            return memberList;
        }

        // ユーザーIDで指定されたメンバーリストを取得
        public List<MMember> GetMemberListByUserId(int UserId)
        {
            List<MMember> members = _context.MMember.Where(m => m.UserId == UserId && m.status == 1).ToList();
            return members;
        }

        // ユーザー登録処理
        public void CreateLogic(string GroupName, int UserId)
        {

            int newGroupId = getNextGroupId();
            DateTime currTime = DateTime.Now;
            string currPg = "MGroupsInsert";

            MGroup mGroup = new MGroup();
            mGroup.status = 1;
            mGroup.GroupId = newGroupId;
            mGroup.GroupName = GroupName;
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
        }
        // 更新処理
        public void UpdateLogic(MGroupDisp mGroupDisp, int UserId, int id)
        {
            DateTime currDate = DateTime.Now;
            string currPg = "MGroupsUpdate";

            MGroup existingGroup = GetGroupById(id);
            existingGroup.GroupName = mGroupDisp.groupName;
            existingGroup.UpdatedDate = currDate;
            existingGroup.UpdateUser = UserId.ToString();
            existingGroup.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MGroup:{existingGroup.ToString()}");
            _context.Update(existingGroup);
            _context.SaveChanges();
        }

        // ステータス変更処理
        public void StatusChangeLogic(MGroup mGroup, int UserId)
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

            List<MMember> memberList = GetMemberByGroupId(mGroup.GroupId);

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

        // Idに基づく画面表示
        public MGroupDisp DispLogic(int id)
        {
            MGroup mGroup = GetGroupById(id);
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

        // 画面表示向けに編集
        public MGroupDisp GetGroupDisp(List<MGroupQuery> queryList)
        {
            MGroupDisp mGroupDisp = new MGroupDisp();
            int currId = -1;
            foreach (MGroupQuery query in queryList)
            {
                if (currId != query.Id)
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
            return mGroupDisp;
        }

        // ホーム画面表示向けに編集
        public HomeDisp GetHomeDisp(MUser user, MGroup group)
        {
            HomeDisp homeDisp = new HomeDisp();
            homeDisp.UserId = user.UserId;
            homeDisp.UserName = user.UserName;
            homeDisp.GroupId = group.GroupId;
            homeDisp.GroupName = group.GroupName;
            homeDisp.GroupUserId = group.UserId;
            return homeDisp;
        }

        // グループのプルダウンリストを取得
        public SelectList GetGroupSelect(int UserId)
        {
            List<MGroup> groups = _context.Database.SqlQuery<MGroup>($@"
                    select mg.* 
                    from MGroup mg inner join MMember mm on mg.GroupId = mm.GroupId
                    where mm.UserId = {UserId}
                    and mg.status = 1 and mm.status = 1
                    order by mg.GroupId").ToList();
            SelectList groupList = new SelectList(groups.Select(g => new { Id = g.GroupId, Name = g.GroupName }), "Id", "Name");
            return groupList;
        }

        // グループIDを発番
        public int getNextGroupId()
        {
            int GroupId = _context.MGroup.Any() ? _context.MGroup.Max(a => a.GroupId) : -0;

            GroupId++;

            return GroupId;
        }
    }
}
