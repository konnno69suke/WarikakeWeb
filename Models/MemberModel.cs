using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Models
{
    public class MemberModel
    {
        WarikakeWebContext _context;

        public MemberModel(WarikakeWebContext context)
        {
            _context = context;
        }

        // 所有グループのメンバーを一覧取得
        public List<MMemberDisp> GetGroupMemberList(int UserId)
        {
            List<MMemberDisp> memberList = _context.Database.SqlQuery<MMemberDisp>($@"
                select mm.Id, 0 gid, mg.groupname, 0 mid, mu.username
                from MMember mm inner join mgroup mg on mm.groupid = mg.groupid
                inner join muser mu on mm.userid = mu.userid
                where mg.UserId = {UserId}
                and mm.status = 1 and mg.status = 1 and mu.status = 1
                order by mm.GroupId, mm.UserId").ToList();

            return memberList;
        }

        // Idで指定されたメンバーを取得
        public MMember GetMemberById(int id)
        {
            MMember mMember = _context.MMember.FirstOrDefault<MMember>(m => m.Id == id && m.status == 1);
            return mMember;
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

        // 登録処理
        public void CreateLogic(MMemberDisp mMemberDisp, int UserId)
        {
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
            Serilog.Log.Information($"SQL param: MMember:{mMember.ToString()}");
            _context.Add(mMember);
            _context.SaveChanges();
        }

        // 更新処理
        public void StatusChangeLogic(MMember mMember, int UserId)
        {
            DateTime currTime = DateTime.Now;
            string currPg = "MMembersDelete";

            mMember.status = (int)statusEnum.削除;
            mMember.UpdatedDate = currTime;
            mMember.UpdateUser = UserId.ToString();
            mMember.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MMember: {mMember.ToString()}");
            _context.Update(mMember);
            _context.SaveChanges();
        }

        // Idで指定されたメンバーを表示状態で取得
        public MMemberDisp GetMemberDispById(int id)
        {
            Serilog.Log.Information($"SQL param: {id}");
            MMemberDisp mMemberDisp = _context.Database.SqlQuery<MMemberDisp>($@"
                select mm.id, mm.groupid gid, mg.groupname, mm.userid mid, mu.username
                from mmember mm inner join mgroup mg on mm.groupid = mg.groupid
                inner join muser mu on mm.userid = mu.userid
                where mm.id = {id}
                and mm.status = 1 and mg.status = 1 and mu.status = 1").FirstOrDefault();
            return mMemberDisp;
        }
    }
}
