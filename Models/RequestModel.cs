using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.ViewModel;
using static WarikakeWeb.Entities.TRequest;

namespace WarikakeWeb.Models
{
    public class RequestModel
    {
        WarikakeWebContext _context;

        public RequestModel(WarikakeWebContext context)
        {
            _context = context;
        }

        // グループとユーザーIDから表示すべき申請を取得
        public List<RequestDisp> searchGroupRequest(int GroupId, int UserId)
        {
            List<RequestDisp> requestList = _context.Database.SqlQuery<RequestDisp>(@$"
                select distinct tr.requestid, tr.status, ' ' statusname, tr.reqtype, ' ' typename, tr.groupid, tr.requserid, mu.username requsername, tr.agreeuserid, tr.tempkey, tr.limitdate
                from trequest tr
                inner join mgroup mg on tr.groupid = {GroupId} and tr.agreeuserid = {UserId}
                inner join muser mu on tr.requserid = mu.userid
                and tr.limitdate >= sysdatetime()
                where tr.status = 1 and mg.status = 1 and mu.status = 1").ToList();
            foreach(RequestDisp disp in requestList)
            {
                disp.TypeName = ((ReqTypeEnum)disp.ReqType).ToString();
                disp.StatusName = ((ReqStatusEnum)disp.Status).ToString();
            }
            return requestList;
        }

        // 
        public RequestDisp GetRequestDisp(int RequestId, int GroupId, int UserId)
        {
            TRequest request = _context.TRequest.Where(r => r.RequestId == RequestId && r.GroupId == GroupId && r.AgreeUserId == UserId && r.Status == 1).FirstOrDefault();
            UserModel model = new UserModel(_context);
            MUser user = model.GetUserByUserId(request.RequestId);

            RequestDisp disp = new RequestDisp();
            disp.RequestId = request.RequestId;
            disp.Status = request.Status;
            disp.StatusName = ((ReqStatusEnum)disp.Status).ToString();
            disp.ReqType = request.ReqType;
            disp.TypeName = ((ReqTypeEnum)disp.ReqType).ToString();
            disp.GroupId = request.GroupId;
            disp.ReqUserId = request.ReqUserId;
            disp.ReqUserName = user.UserName;
            disp.AgreeUserId = request.AgreeUserId;
            disp.LimitDate = request.LimitDate;

            return disp;
        }

        public TRequest GetGroupRequest(int GroupId, int ReqType, string TempKey)
        {
            try
            {
                TRequest request = _context.TRequest.Where(r => r.GroupId == GroupId && r.ReqType == ReqType && r.TempKey.Equals(TempKey) && r.Status == 1).FirstOrDefault();
                return request;
            }
            catch (Exception ex)
            {
                return null;
            }         
        }

        public void ChangeRequestStatus(int RequestId, int UserId, int ReqStatus)
        {
            DateTime currDate = DateTime.Now;
            TRequest request = _context.TRequest.Where(r => r.RequestId == RequestId).FirstOrDefault();
            request.Status = ReqStatus;
            request.UpdatedDate = currDate;
            request.UpdateUser = UserId.ToString();
            request.UpdatePg = "RequestStatusChange";

            _context.TRequest.Update(request);

            _context.SaveChanges();
        }

        public string CreateRequest(int GroupId, int UserId, int TargetId, int ReqType)
        {
            DateTime currDate = DateTime.Now;
            string pg = "CreateRequest";
            string tempKey = GetTempKey(12);

            TRequest request = new TRequest();
            request.RequestId = GetNextRequestId();
            request.Status = (int)ReqStatusEnum.申請;
            request.ReqType = ReqType;
            request.GroupId = GroupId;
            request.ReqUserId = UserId;
            request.AgreeUserId = TargetId;
            request.TempKey = tempKey;
            request.LimitDate = GetLimitDate();
            request.CreatedDate = currDate;
            request.CreateUser = UserId.ToString();
            request.CreatePg = pg;
            request.UpdatedDate= currDate;
            request.UpdateUser = UserId.ToString();
            request.UpdatePg= pg;
            _context.TRequest.Add(request);

            _context.SaveChanges();

            return tempKey;
        }

        // 申請と承認に基づきパスワードを生成
        public MUserDisp SetNewPassword(string TempKey)
        {
            TRequest request = _context.TRequest.Where(r => r.TempKey == TempKey && r.Status == (int)ReqStatusEnum.承認).FirstOrDefault();
            if (request == null) 
            {
                return null;
            }

            UserModel model = new UserModel(_context);
            MUser user = model.GetUserByUserId(request.ReqUserId);
            if (user == null)
            {
                return null;
            }

            MUserDisp mUserDisp = model.GetMUserDisp(user);
            // 新規パスワードを生成
            mUserDisp.NewPassword = GetTempKey(24);

            // パスワードを更新
            model.UpdateLogic(mUserDisp, user.UserId, user.Id);
            // リクエストを使用済に更新
            ChangeRequestStatus(request.RequestId, user.UserId, (int)ReqStatusEnum.使用済);

            return mUserDisp;
        }

        // メンバー追加承認
        public void AddGroupMember(RequestDisp requestDisp, int GroupId, int UserId)
        {
            string pg = "Request";
            DateTime currDate = DateTime.Now;
            MMember member = new MMember();
            member.status = 1;
            member.GroupId = GroupId;
            member.UserId = requestDisp.ReqUserId;
            member.CreatedDate = currDate;
            member.CreateUser = UserId.ToString();
            member.CreatePg = pg;
            member.UpdatedDate = currDate;
            member.UpdateUser = UserId.ToString();
            member.UpdatePg = pg;

            _context.MMember.Add(member);

            _context.SaveChanges();
        }

        // ユーザーIDの発番
        private int GetNextRequestId()
        {
            int RequestId = _context.TRequest.Any() ? _context.TRequest.Max(a => a.RequestId) : -0;

            RequestId++;

            return RequestId;
        }

        private string GetTempKey(int length)
        {
            Random rd = new Random();   
            string UseChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            char[] chars = new char[length];
            for(int i = 0; i< length; i++)
            {
                chars[i] = UseChars[rd.Next(0,UseChars.Length)];
            }
            return new string(chars);
        }

        private DateTime GetLimitDate()
        {
            DateTime LimitDate = DateTime.Now.Date;
            // いったん翌日とする
            LimitDate = LimitDate.AddDays(1);

            return LimitDate;
        }
    }
}
