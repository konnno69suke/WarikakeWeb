using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Models
{
    public class WarikakeModel
    {
        WarikakeWebContext _context;

        public WarikakeModel(WarikakeWebContext context)
        {
            _context = context;
        }

        // 仮登録、未精算の一覧取得
        public List<WarikakeQuery> GetUnSettledWarikakeQueries(int GroupId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}");
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(
                $@"select tc.CostId, tc.CostTitle, tc.GroupId, mgr.GroupName, tc.GenreId, mge.GenreName, tc.status, ' ' statusName, tc.CostStatus, tc.CostAmount, tc.CostDate, 
                          tp.payid, tp.userid PayUserId, mup.username PayUserName, tp.PayAmount, 
                          tr.repayid, tr.userid RepayUserId, mur.username RepayUserName, tr.RepayAmount 
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    inner join muser mup on tp.userid = mup.Userid
                    inner join muser mur on tr.userid = mur.Userid
                    inner join mgroup mgr on tc.groupid = mgr.Groupid
                    inner join mgenre mge on tc.genreid = mge.Genreid
                    where tc.status <= 1 and tp.status <= 1 and tr.status <= 1
                    and   tc.GroupId = {GroupId}
                    order by tc.costid desc, tp.payid, tr.repayid").ToList();
            return warikakeQueries;
        }

        // 仮登録の一覧取得
        public List<WarikakeQuery> GetProvisionWarikakeQueries(int GroupId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}");
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(
                $@"select tc.CostId, tc.CostTitle, tc.GroupId, mgr.GroupName, tc.GenreId, mge.GenreName, tc.status, ' ' statusName, tc.CostStatus, tc.CostAmount, tc.CostDate, 
                          tp.payid, tp.userid PayUserId, mup.username PayUserName, tp.PayAmount, 
                          tr.repayid, tr.userid RepayUserId, mur.username RepayUserName, tr.RepayAmount 
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    inner join muser mup on tp.userid = mup.Userid
                    inner join muser mur on tr.userid = mur.Userid
                    inner join mgroup mgr on tc.groupid = mgr.Groupid
                    inner join mgenre mge on tc.genreid = mge.Genreid
                    where tc.status = 0 and tp.status = 0 and tr.status = 0
                    and   tc.GroupId = {GroupId}
                    order by tc.costid desc, tp.payid, tr.repayid").ToList();
            return warikakeQueries;
        }


        // 未精算、手動精算の合計一覧取得
        public List<WarikakeQuery> GetUnSettledSumWarikakeQueries(int GroupId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}");
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(
                $@"select -1 CostId, ' ' CostTitle, 0 GroupId, ' ' GroupName, 0 GenreId, N'合計' GenreName, 7 status, ' ' statusName, 0 CostStatus, sum(tc.CostAmount) CostAmount, max(tc.CostDate) CostDate, 
                          0 payid, tp.userid PayUserId, mup.username PayUserName, sum(tp.PayAmount) PayAmount, 
                          0 repayid, tr.userid RepayUserId, mur.username RepayUserName, sum(tr.RepayAmount) RepayAmount
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    inner join muser mup on tp.userid = mup.Userid
                    inner join muser mur on tr.userid = mur.Userid
                    inner join mgroup mgr on tc.groupid = mgr.Groupid
                    where tc.status in (1, 4) and tp.status in (1, 4) and tr.status in (1, 4)
                    and   tc.GroupId = {GroupId}
                    group by tp.userid, mup.username, tr.userid, mur.username").ToList();
            return warikakeQueries;
        }

        // 未精算、手動精算の一覧取得（一括精算の更新処理直前用）
        public List<WarikakeQuery> GetUnSettledOrManualWarikakeQueries(int GroupId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}");
            List<WarikakeQuery> warikakeQueryList = _context.Database.SqlQuery<WarikakeQuery>(
                $@"select tc.costid, ' ' CostTitle, 0 GroupId, ' ' GroupName, 0 GenreId, ' ' GenreName, 0 status, ' ' statusName, 0 CostStatus, 0 CostAmount, sysdatetime() CostDate, 
                          0 payid, 0 PayUserId, ' ' PayUserName, 0 PayAmount, 
                          0 repayid, 0 RepayUserId, ' ' RepayUserName, 0 RepayAmount
                    from  tcost tc 
                    where tc.status in (1, 4)
                    and   tc.GroupId = {GroupId}").ToList();
            return warikakeQueryList;
        }

        // 指定された支払情報の取得
        public List<WarikakeQuery> GetWarikakeQueries(int GroupId, int CostId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {CostId}");
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(
                    $@"select tc.CostId, tc.CostTitle, tc.GroupId, mgr.GroupName, tc.GenreId, mge.GenreName, tc.status, ' ' statusName, tc.CostStatus, tc.CostAmount, tc.CostDate, 
                          tp.payid, tp.userid PayUserId, mup.username PayUserName, tp.PayAmount, 
                          tr.repayid, tr.userid RepayUserId, mur.username RepayUserName, tr.RepayAmount 
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    inner join muser mup on tp.userid = mup.Userid
                    inner join muser mur on tr.userid = mur.Userid
                    inner join mgroup mgr on tc.groupid = mgr.Groupid
                    inner join mgenre mge on tc.genreid = mge.Genreid
                    where tc.status <= 1 and tp.status <= 1 and tr.status <= 1
                    and   tc.GroupId = {GroupId}
                    and   tc.CostId = {CostId}
                    order by tc.costid, tp.payid, tr.repayid").ToList();
            return warikakeQueries;
        }

        // 指定された仮登録の支払情報の取得
        public List<WarikakeQuery> GetProvisionQueries(int GroupId, int CostId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {CostId}");
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(
                    $@"select tc.CostId, tc.CostTitle, tc.GroupId, mgr.GroupName, tc.GenreId, mge.GenreName, tc.status, ' ' statusName, tc.CostStatus, tc.CostAmount, tc.CostDate, 
                          tp.payid, tp.userid PayUserId, mup.username PayUserName, tp.PayAmount, 
                          tr.repayid, tr.userid RepayUserId, mur.username RepayUserName, tr.RepayAmount 
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    inner join muser mup on tp.userid = mup.Userid
                    inner join muser mur on tr.userid = mur.Userid
                    inner join mgroup mgr on tc.groupid = mgr.Groupid
                    inner join mgenre mge on tc.genreid = mge.Genreid
                    where tc.status = 0 and tp.status = 0 and tr.status = 0
                    and   tc.GroupId = {GroupId}
                    and   tc.CostId = {CostId}
                    order by tc.costid, tp.payid, tr.repayid").ToList();
            return warikakeQueries;
        }

        // 月別集計表
        public List<WarikakeQuery> GetAggregatedWarikakeQueries(int GroupId, int year)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {year}, {GroupId}");
            FormattableString queryString = $@"with main as
                    (select year(tc.costdate) cyear, month(tc.costdate) cmonth, tc.costamount costamount, 
                          tp.payid, tp.userid PayUserId, tp.PayAmount payamount, 
                          tr.repayid, tr.userid RepayUserId, tr.RepayAmount repayamount
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    where tc.GroupId = {GroupId} 
                    and tc.status < 9 and tp.status < 9 and tr.status < 9
                    ), agg as
                    (select mt.cyear, mt.cmonth, sum(mt.costamount) costamount, 
                          max(mt.payid) payid, mt.payuserid, sum(mt.payamount) payamount,
                          max(mt.repayid) repayid, mt.repayuserid, sum(mt.repayamount) repayamount
                    from main mt
                    where mt.cyear = {year}
                    group by mt.cyear, mt.cmonth, mt.payuserid, mt.repayuserid)
                    select ag.cmonth costid, convert(varchar, ag.cyear) + '_' + convert(varchar, ag.cmonth) costtitle, {GroupId} groupid, ' ' groupname, 0 GenreId, convert(varchar, ag.cmonth) + N'月合計' GenreName, 7 status, ' ' statusname, 0 coststatus, ag.costamount, sysdatetime() costdate,
                         ag.payid, ag.payuserid, pu.username payusername, ag.payamount,
                         ag.repayid, ag.repayuserid, ru.username repayusername, ag.repayamount
                    from agg ag
                    inner join muser pu on ag.payuserid = pu.userid
                    inner join muser ru on ag.repayuserid = ru.userid
                    order by ag.cmonth, ag.payuserid";
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(queryString).ToList();
            return warikakeQueries;
        }


        // 年間集計
        public List<WarikakeQuery> GetAggregatedSumWarikakeQueries(int GroupId, int year)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {year}, {GroupId}");
            FormattableString queryString = $@"with main as
                    (select year(tc.costdate) cyear, month(tc.costdate) cmonth, tc.costamount costamount, 
                          tp.payid, tp.userid PayUserId, tp.PayAmount payamount, 
                          tr.repayid, tr.userid RepayUserId, tr.RepayAmount repayamount
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    where tc.GroupId = {GroupId}
                    and tc.status < 9 and tp.status < 9 and tr.status < 9
                    ), agg as
                    (select max(mt.cyear) cyear, max(mt.cmonth) cmonth, sum(mt.costamount) costamount, 
                          max(mt.payid) payid, mt.payuserid, sum(mt.payamount) payamount,
                          max(mt.repayid) repayid, mt.repayuserid, sum(mt.repayamount) repayamount
                    from main mt
                    where mt.cyear = {year}
                    group by mt.payuserid, mt.repayuserid)
                    select ag.cmonth costid, ' ' costtitle, {GroupId} groupid, ' ' groupname, -1 genreid, convert(varchar, ag.cyear) + N'年計' genrename, 7 status, ' ' statusname, 0 coststatus, ag.costamount, sysdatetime() costdate,
                         ag.payid, ag.payuserid, pu.username payusername, ag.payamount,
                         ag.repayid, ag.repayuserid, ru.username repayusername, ag.repayamount
                    from agg ag
                    inner join muser pu on ag.payuserid = pu.userid
                    inner join muser ru on ag.repayuserid = ru.userid
                    order by ag.cmonth, ag.payuserid";
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(queryString).ToList();
            return warikakeQueries;
        }
        // グラフ用の種別年間推移
        public List<WarikakeQuery> GetAggreateGraphWarikakeQueries(int GroupId, int year)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {year}");
            FormattableString queryString = $@"with main as 
                    (select tc.genreid, tc.genrename, tc.costamount, year(tc.costdate) cyear, month(tc.costdate) cmonth 
                    from tcost tc
                    where tc.groupid = {GroupId})
                    select 
                    ma.cmonth costid, ' ' costtitle, 0 groupid, ' ' groupname, ma.genreid, ma.genrename, 7 status, 0 coststatus, sum(ma.costamount) costamount, sysdatetime() costdate, 
                    0 payid, 0 payuserid, ' ' payusername, 0 payamount, 0 repayid, 0 repayuserid, ' ' repayusername, 0 repayamount
                    from main ma
                    where ma.cyear = {year}
                    group by ma.genreid, ma.genrename, ma.cmonth
                    order by ma.genreid, ma.cmonth";
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(queryString).ToList();
            return warikakeQueries;
        }

        // 日別集計表
        public List<WarikakeQuery> GetAggregatedWarikakeQueries(int GroupId, int year, int month)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {year}, {month}, {GroupId}");
            FormattableString queryString = $@"with main as
                    (select year(tc.costdate) cyear, month(tc.costdate) cmonth, day(tc.costdate) cday, tc.costamount costamount, 
                          tp.payid, tp.userid PayUserId, tp.PayAmount payamount, 
                          tr.repayid, tr.userid RepayUserId, tr.RepayAmount repayamount
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    where tc.GroupId = {GroupId}
                    and tc.status < 9 and tp.status < 9 and tr.status < 9
                    ), agg as
                    (select mt.cyear, mt.cmonth, mt.cday, sum(mt.costamount) costamount, 
                          max(mt.payid) payid, mt.payuserid, sum(mt.payamount) payamount,
                          max(mt.repayid) repayid, mt.repayuserid, sum(mt.repayamount) repayamount
                    from main mt
                    where mt.cyear = {year} and mt.cmonth = {month}
                    group by mt.cyear, mt.cmonth, mt.cday, mt.payuserid, mt.repayuserid)
                    select ag.cday costid, convert(varchar, ag.cyear) + '_' + convert(varchar, ag.cmonth) + '_' + convert(varchar, ag.cday) costtitle, {GroupId} groupid, ' ' groupname, 0 GenreId, convert(varchar, ag.cmonth) + N'月' + convert(varchar, ag.cday) + N'日' GenreName, 7 status, ' ' statusname, 0 coststatus, ag.costamount, sysdatetime() costdate,
                         ag.payid, ag.payuserid, pu.username payusername, ag.payamount,
                         ag.repayid, ag.repayuserid, ru.username repayusername, ag.repayamount
                    from agg ag
                    inner join muser pu on ag.payuserid = pu.userid
                    inner join muser ru on ag.repayuserid = ru.userid
                    order by ag.cday, ag.payuserid";
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(queryString).ToList();
            return warikakeQueries;
        }

        // グラフ用の種別年間推移
        public List<WarikakeQuery> GetAggreateGraphWarikakeQueries(int GroupId, int year, int month)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {year}, {month}");
            FormattableString queryString = $@"with main as 
                    (select tc.genreid, tc.genrename, tc.costamount, year(tc.costdate) cyear, month(tc.costdate) cmonth, day(tc.costdate) cday 
                    from tcost tc
                    where tc.groupid = {GroupId})
                    select 
                    ma.cday costid, ' ' costtitle, 0 groupid, ' ' groupname, ma.genreid, ma.genrename, 7 status, 0 coststatus, sum(ma.costamount) costamount, sysdatetime() costdate, 
                    0 payid, 0 payuserid, ' ' payusername, 0 payamount, 0 repayid, 0 repayuserid, ' ' repayusername, 0 repayamount
                    from main ma
                    where ma.cyear = {year} and ma.cmonth = {month}
                    group by ma.genreid, ma.genrename, ma.cmonth, cday
                    order by ma.genreid, ma.cday";
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(queryString).ToList();
            return warikakeQueries;
        }

        // 指定日の支払情報
        public List<WarikakeQuery> GetAggregatedWarikakeQueries(int GroupId, int year, int month, int day)
        {
            DateTime dateTime = new DateTime(year, month, day);
            Serilog.Log.Information($"SQL param:{GroupId}, {dateTime}");
            FormattableString queryString = $@"select tc.costid, format(tc.costdate, 'yyyy年MM月dd日') costtitle, tc.groupid, ' ' groupname, tc.genreid, tc.genrename, 7 status, ' ' statusname, coststatus, tc.costamount, tc.costdate,
                    tp.payid, tp.userid PayUserId, pu.username payusername, tp.PayAmount payamount, 
                    tr.repayid, tr.userid RepayUserId, ru.username repayusername, tr.RepayAmount repayamount
                    from  tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid and tp.userid = tr.userid
                    inner join muser pu on tp.userid = pu.userid
                    inner join muser ru on tr.userid = ru.userid
                    where tc.GroupId = {GroupId} and tc.costdate = {dateTime}
                    and tc.status < 9 and tp.status < 9 and tr.status < 9
                    order by tc.costid";
            List<WarikakeQuery> warikakeQueries = _context.Database.SqlQuery<WarikakeQuery>(queryString).ToList();
            return warikakeQueries;
        }

        // 一括処理等のループ内でたった今登録したTCostレコードを取得する
        public TCost GetCurrentCost(int UserId, DateTime currTime, String pg)
        {
            TCost currCost = _context.TCost.Where(c => c.UpdateUser.Equals(UserId.ToString()) && c.UpdatedDate.Equals(currTime) && c.UpdatePg.Equals(pg)).OrderByDescending(a => a.CostId).FirstOrDefault();

            return currCost;
        }

        // グループでのその種別の使用実績を取得する
        public int GetGenreUsedCount(int GroupId, int GenreId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {GroupId}, {GenreId}");
            int count = _context.TCost.Where(c => c.GroupId == GroupId && c.GenreId == GenreId && c.status != 9).Count();
            return count;
        }

        // グループでそのユーザーの未精算件数を取得する
        public int GetUserUnSettledCostCount(int GroupId, int UserId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}, {UserId}, {UserId}");
            int count = _context.Database.SqlQuery<UnSettledCount>($@"select count(*) count from tcost tc 
                    inner join tpay tp on tc.costid = tp.costid 
                    inner join trepay tr on tc.costid = tr.costid
                    where tc.groupid = {GroupId} and (tp.UserId = {UserId} or tr.UserId = {UserId})
                    and tc.status <= 1 and tp.status <= 1 and tr.status <= 1").FirstOrDefault().count;
            return count;
        }


        // 新規登録
        public void CreateLogic(WarikakeDisp input, int GroupId, int UserId)
        {
            DateTime currDate = DateTime.Now;
            string currPg = "WarikakeInsert";

            Serilog.Log.Information($"SQL param: WarikakeDisp:{input.ToString()}, GroupId:{GroupId}, UserId:{UserId}");

            int payId = 0;
            int repayId = 0;

            int updStatus = 0;
            string updGenreName = "";
            if (input.GenreId == 0)
            {
                updStatus = (int)statusEnum.手動精算;
                updGenreName = statusEnum.手動精算.ToString();
            }
            else
            {
                updStatus = (int)statusEnum.未精算;
                MGenre genre = _context.MGenre.Where(g => g.GenreId == input.GenreId).FirstOrDefault();
                updGenreName = genre.GenreName;
            }

            TCost cost = new TCost();
            cost.status = updStatus;
            cost.CostTitle = input.CostTitle;
            cost.GroupId = GroupId;
            cost.GenreId = input.GenreId;
            cost.GenreName = updGenreName;
            cost.ProvisionFlg = 0;
            cost.CostAmount = input.CostAmount;
            cost.CostDate = input.CostDate;
            cost.CreatedDate = currDate;
            cost.CreateUser = UserId.ToString();
            cost.CreatePg = currPg;
            cost.UpdatedDate = currDate;
            cost.UpdateUser = UserId.ToString();
            cost.UpdatePg = currPg;
            _context.Add(cost);

            _context.SaveChanges();
            TCost currCost = GetCurrentCost(UserId, currDate, currPg);
            try
            {
                foreach (WarikakePayDisp inputPay in input.Pays)
                {
                    TPay pay = new TPay();
                    pay.status = updStatus;
                    pay.CostId = currCost.CostId;
                    pay.PayId = payId;
                    pay.UserId = inputPay.PayUserId;
                    pay.PayAmount = inputPay.PayAmount;

                    pay.CreatedDate = currDate;
                    pay.CreateUser = UserId.ToString();
                    pay.CreatePg = currPg;
                    pay.UpdatedDate = currDate;
                    pay.UpdateUser = UserId.ToString();
                    pay.UpdatePg = currPg;
                    _context.Add(pay);
                    payId++;
                }
                foreach (WarikakeRepayDisp inputRepay in input.Repays)
                {

                    TRepay repay = new TRepay();
                    repay.status = updStatus;
                    repay.CostId = currCost.CostId;
                    repay.RepayId = repayId;
                    repay.UserId = inputRepay.RepayUserId;
                    repay.RepayAmount = inputRepay.RepayAmount;
                    repay.CreatedDate = currDate;
                    repay.CreateUser = UserId.ToString();
                    repay.CreatePg = currPg;
                    repay.UpdatedDate = currDate;
                    repay.UpdateUser = UserId.ToString();
                    repay.UpdatePg = currPg;
                    _context.Add(repay);
                    repayId++;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // TPay、TRepayのインサートに失敗した場合、TCostがごみデータになるので論理削除する
                currCost.status = (int)statusEnum.削除;
                currCost.UpdatedDate = currDate;
                currCost.UpdateUser = UserId.ToString();
                currCost.UpdatePg = currPg;
                _context.SaveChanges();
            }
        }

        public void StatusChangeLogic(int CostId, int status, int UserId)
        {
            DateTime dateTime = DateTime.Now;

            Serilog.Log.Information($"SQL param: CostId:{CostId}, UserId:{UserId}, status:{status}");

            List<TCost> costList = _context.TCost.Where(c => c.CostId == CostId).ToList();
            foreach (TCost cost in costList)
            {
                cost.status = status;
                cost.UpdatedDate = dateTime;
                cost.UpdateUser = UserId.ToString();
                cost.UpdatePg = "WarikakeUpdate";
                _context.TCost.Update(cost);
            }
            List<TPay> payList = _context.TPay.Where(p => p.CostId == CostId).ToList();
            foreach (TPay pay in payList)
            {
                pay.status = status;
                pay.UpdatedDate = dateTime;
                pay.UpdateUser = UserId.ToString();
                pay.UpdatePg = "WarikakeUpdate";
                _context.Update(pay);
            }
            List<TRepay> repayList = _context.TRepay.Where(r => r.CostId == CostId).ToList();
            foreach (TRepay repay in repayList)
            {
                repay.status = status;
                repay.UpdatedDate = dateTime;
                repay.UpdateUser = UserId.ToString();
                repay.UpdatePg = "WarikakeUpdate";
                _context.Update(repay);
            }
            _context.SaveChanges();
        }

        // 更新処理（ステータス変更無）
        public void UpdateLogic(WarikakeDisp input, int GroupId, int UserId)
        {
            UpdateLogic(input, GroupId, UserId, -1, false);
        }
        // 更新処理（ステータス変更有）
        public void UpdateLogic(WarikakeDisp input, int GroupId, int UserId, int status)
        {
            UpdateLogic(input, GroupId, UserId, status, true);
        }

        public void UpdateLogic(WarikakeDisp input, int GroupId, int UserId, int status, bool isStatus)
        {
            DateTime dateTime = DateTime.Now;
            int CostId = input.CostId;

            Serilog.Log.Information($"SQL param: WarikakeDisp:{input.ToString()}, GroupId:{GroupId}, UserId:{UserId}, status:{status}");

            TCost existingCost = _context.TCost.FirstOrDefault(c => c.CostId == CostId);
            existingCost.CostId = CostId;
            if (isStatus)
            {
                existingCost.status = status;
            }
            existingCost.CostTitle = input.CostTitle;
            existingCost.GroupId = GroupId;
            existingCost.GenreId = input.GenreId;
            MGenre genre = _context.MGenre.Where(g => g.GenreId == input.GenreId).FirstOrDefault();
            existingCost.GenreName = genre.GenreName;
            existingCost.ProvisionFlg = 0;
            existingCost.CostAmount = input.CostAmount;
            existingCost.CostDate = input.CostDate;

            existingCost.UpdatedDate = dateTime;
            existingCost.UpdateUser = UserId.ToString();
            existingCost.UpdatePg = "WarikakeEdit";
            _context.Update(existingCost);

            foreach (WarikakePayDisp inputPay in input.Pays)
            {
                TPay existingPay = _context.TPay.FirstOrDefault(p => p.UserId == inputPay.PayUserId && p.CostId == CostId);

                existingPay.CostId = CostId;
                if (isStatus)
                {
                    existingPay.status = status;
                }
                existingPay.UserId = inputPay.PayUserId;
                existingPay.PayAmount = inputPay.PayAmount;

                existingPay.UpdatedDate = dateTime;
                existingPay.UpdateUser = UserId.ToString();
                existingPay.UpdatePg = "WarikakeEdit";
                _context.Update(existingPay);
            }
            foreach (WarikakeRepayDisp inputRepay in input.Repays)
            {
                TRepay existingRepay = _context.TRepay.FirstOrDefault(r => r.UserId == inputRepay.RepayUserId && r.CostId == CostId);

                existingRepay.CostId = CostId;
                if (isStatus)
                {
                    existingRepay.status = status;
                }
                existingRepay.UserId = inputRepay.RepayUserId;
                existingRepay.RepayAmount = inputRepay.RepayAmount;

                existingRepay.UpdatedDate = dateTime;
                existingRepay.UpdateUser = UserId.ToString();
                existingRepay.UpdatePg = "WarikakeEdit";
                _context.Update(existingRepay);
            }
            _context.SaveChanges();
        }


        // 一覧画面表示用に編集
        public List<WarikakeDisp> GetWarikakeDisps(List<WarikakeQuery> warikakeQueries)
        {
            List<WarikakeDisp> warikakeDisps = new List<WarikakeDisp>();

            int prevCostId = 0;
            WarikakeDisp wariDisp = new WarikakeDisp();
            foreach (WarikakeQuery item in warikakeQueries)
            {
                if (prevCostId != item.CostId)
                {
                    wariDisp = new WarikakeDisp();
                    wariDisp.CostId = item.CostId;
                    wariDisp.CostTitle = item.CostTitle;
                    wariDisp.GroupName = item.GroupName;
                    wariDisp.GenreId = item.GenreId;
                    wariDisp.GenreName = item.GenreName;
                    wariDisp.status = item.status;
                    wariDisp.statusName = ((statusEnum)item.status).ToString();
                    wariDisp.CostAmount = item.CostAmount;
                    wariDisp.CostDisp = item.CostAmount.ToString("C0", new CultureInfo("ja-JP"));
                    wariDisp.CostDate = item.CostDate;

                    warikakeDisps.Add(wariDisp);
                    prevCostId = item.CostId;
                }

                WarikakePayDisp payDisp = new WarikakePayDisp();
                payDisp.PayId = item.PayId;
                payDisp.PayUserId = item.PayUserId;
                payDisp.PayUserName = item.PayUserName;
                payDisp.PayAmount = item.PayAmount;
                payDisp.PayDisp = item.PayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Pays.Add(payDisp);

                WarikakeRepayDisp repayDisp = new WarikakeRepayDisp();
                repayDisp.RepayId = item.RepayId;
                repayDisp.RepayUserId = item.RepayUserId;
                repayDisp.RepayUserName = item.RepayUserName;
                repayDisp.RepayAmount = item.RepayAmount;
                repayDisp.RepayDisp = item.RepayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Repays.Add(repayDisp);
            }

            return warikakeDisps;
        }

        // 詳細画面表示用に編集
        public WarikakeDisp GetWarikakeDisp(List<WarikakeQuery> warikakeQueries)
        {
            int prevCostId = 0;
            WarikakeDisp wariDisp = new WarikakeDisp();
            foreach (WarikakeQuery item in warikakeQueries)
            {
                if (prevCostId != item.CostId)
                {
                    wariDisp = new WarikakeDisp();
                    wariDisp.CostId = item.CostId;
                    wariDisp.CostTitle = item.CostTitle;
                    wariDisp.GroupName = item.GroupName;
                    wariDisp.GenreId = item.GenreId;
                    wariDisp.GenreName = item.GenreName;
                    wariDisp.status = item.status;
                    wariDisp.statusName = ((statusEnum)item.status).ToString();
                    wariDisp.CostAmount = item.CostAmount;
                    wariDisp.CostDisp = item.CostAmount.ToString("C0", new CultureInfo("ja-JP"));
                    wariDisp.CostDate = item.CostDate;

                    prevCostId = item.CostId;
                }

                WarikakePayDisp payDisp = new WarikakePayDisp();
                payDisp.PayId = item.PayId;
                payDisp.PayUserId = item.PayUserId;
                payDisp.PayUserOn = item.PayAmount != 0 ? true : false;
                payDisp.PayUserName = item.PayUserName;
                payDisp.PayAmount = item.PayAmount;
                payDisp.PayDisp = item.PayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Pays.Add(payDisp);

                WarikakeRepayDisp repayDisp = new WarikakeRepayDisp();
                repayDisp.RepayId = item.RepayId;
                repayDisp.RepayUserId = item.RepayUserId;
                repayDisp.RepayUserOn = item.RepayAmount != 0 ? true : false;
                repayDisp.RepayUserName = item.RepayUserName;
                repayDisp.RepayAmount = item.RepayAmount;
                repayDisp.RepayDisp = item.RepayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Repays.Add(repayDisp);
            }
            return wariDisp;
        }

        // 入力画面表示用に編集
        public WarikakeDisp GetWarikakeDispInit(List<MUser> users, int GroupId, int UserId, bool repayOn)
        {
            WarikakeDisp input = new WarikakeDisp();
            input.GroupId = GroupId;
            DateTime dateTime = DateTime.Now;
            input.CostDate = dateTime;
            foreach (MUser user in users)
            {
                WarikakePayDisp inputPay = new WarikakePayDisp();
                inputPay.PayUserId = user.UserId;
                inputPay.PayUserOn = user.UserId == UserId ? true : false;
                inputPay.PayUserName = user.UserName;
                input.Pays.Add(inputPay);
                WarikakeRepayDisp inputRepay = new WarikakeRepayDisp();
                inputRepay.RepayUserId = user.UserId;
                inputRepay.RepayUserOn = repayOn;
                inputRepay.RepayUserName = user.UserName;
                input.Repays.Add(inputRepay);
            }
            return input;
        }

        // 折れ線グラフ用に編集
        public List<ChartDataset> GetChartDataset(List<WarikakeQuery> warikakeQueries)
        {
            List<ChartDataset> chartDatasets = new List<ChartDataset>();

            int prevGenreId = -1;
            int day = -1;
            ChartDataset chartDataset = new ChartDataset();
            foreach (WarikakeQuery warikakeQuery in warikakeQueries)
            {
                if (prevGenreId != warikakeQuery.GenreId)
                {
                    if (prevGenreId != -1)
                    {
                        chartDatasets.Add(chartDataset);
                        chartDataset = new ChartDataset();
                    }
                    prevGenreId = warikakeQuery.GenreId;
                    day = 1;

                    chartDataset.label = "'" + warikakeQuery.GenreName + "'";
                    chartDataset.borderWidth = 1;
                }
                while (day <= warikakeQuery.CostId)
                {
                    if (day == warikakeQuery.CostId)
                    {
                        chartDataset.data.Add(warikakeQuery.CostAmount);
                    }
                    else
                    {
                        chartDataset.data.Add(0);
                    }
                    day++;
                }
            }
            chartDatasets.Add(chartDataset);

            return chartDatasets;
        }


        public List<WarikakeGraph> GetWarikakeGraph(List<WarikakeQuery> warikakeQueries)
        {
            List<WarikakeGraph> warikakeGraphs = new List<WarikakeGraph>();

            int prevGenreId = -1;
            WarikakeGraph warikakeGraph = new WarikakeGraph();
            foreach (WarikakeQuery warikakeQuery in warikakeQueries)
            {
                if (prevGenreId != warikakeQuery.GenreId)
                {
                    if (prevGenreId != -1)
                    {
                        warikakeGraphs.Add(warikakeGraph);
                        warikakeGraph = new WarikakeGraph();
                    }
                    prevGenreId = warikakeQuery.GenreId;

                    warikakeGraph.GenreId = warikakeQuery.GenreId;
                    warikakeGraph.GenreName = warikakeQuery.GenreName;
                }
                switch (warikakeQuery.CostId)
                {
                    case 1:
                        warikakeGraph.Amount1 = warikakeQuery.CostAmount;
                        break;
                    case 2:
                        warikakeGraph.Amount2 = warikakeQuery.CostAmount;
                        break;
                    case 3:
                        warikakeGraph.Amount3 = warikakeQuery.CostAmount;
                        break;
                    case 4:
                        warikakeGraph.Amount4 = warikakeQuery.CostAmount;
                        break;
                    case 5:
                        warikakeGraph.Amount5 = warikakeQuery.CostAmount;
                        break;
                    case 6:
                        warikakeGraph.Amount6 = warikakeQuery.CostAmount;
                        break;
                    case 7:
                        warikakeGraph.Amount7 = warikakeQuery.CostAmount;
                        break;
                    case 8:
                        warikakeGraph.Amount8 = warikakeQuery.CostAmount;
                        break;
                    case 9:
                        warikakeGraph.Amount9 = warikakeQuery.CostAmount;
                        break;
                    case 10:
                        warikakeGraph.Amount10 = warikakeQuery.CostAmount;
                        break;
                    case 11:
                        warikakeGraph.Amount11 = warikakeQuery.CostAmount;
                        break;
                    case 12:
                        warikakeGraph.Amount12 = warikakeQuery.CostAmount;
                        break;
                    default:
                        break;
                }
            }
            return warikakeGraphs;
        }


        // 直近の登録情報メッセージを取得
        public String LastMessage(int GroupId)
        {
            string fs = "";
            TCost cost = _context.TCost.Where(c => c.GroupId == (int)GroupId).OrderByDescending(c => c.UpdatedDate).FirstOrDefault();
            if (cost != null && cost.CostId != null)
            {
                MUser user = _context.MUser.Where(u => u.UserId == int.Parse(cost.UpdateUser)).FirstOrDefault();
                String formatDate = cost.CostDate.ToString("yyyy/MM/dd");

                fs = $"※直前に登録されたデータは{cost.UpdatedDate}に{user.UserName}が登録した{formatDate}の{cost.CostAmount}円の{cost.GenreName}です。";
            }
            return fs;
        }

        // 精算情報メッセージ一覧の取得
        public List<string> repayMessage(List<WarikakeQuery> warikakeQueries)
        {
            // 精算指示の文字列作成
            List<string> messageList = new List<string>();

            WarikakeProcess warikakeProcess = GetWarikakeProcess(warikakeQueries);
            foreach (WarikakeUserProcess plsUsr in warikakeProcess.plusUsers)
            {
                messageList.Add($"{plsUsr.UserName}は合計{plsUsr.processAmount}円を支払うこと。");
            }
            foreach (WarikakeUserProcess mnsUsr in warikakeProcess.minusUsers)
            {
                messageList.Add($"{mnsUsr.UserName}は合計{mnsUsr.processAmount * -1}円を受け取ること。");
            }
            return messageList;
        }
        public WarikakeProcess GetWarikakeProcess(List<WarikakeQuery> warikakeQueries)
        {
            WarikakeProcess warikakeProcess = new WarikakeProcess();
            foreach (WarikakeQuery item in warikakeQueries)
            {
                WarikakeUserProcess userProc = new WarikakeUserProcess();
                userProc.UserId = item.PayUserId;
                userProc.UserName = item.PayUserName;
                userProc.processAmount = item.RepayAmount - item.PayAmount;
                if (userProc.processAmount > 0)
                {
                    // 精算時支払い側
                    warikakeProcess.plusUsers.Add(userProc);
                }
                else
                {
                    // 精算時受け取り側
                    warikakeProcess.minusUsers.Add(userProc);
                }
            }
            return warikakeProcess;
        }
    }
    // 件数だけをカウントするSQLから値を取得するためだけのクラス
    public class UnSettledCount
    {
        public int count
        {
            get; set;
        }

    }
}
