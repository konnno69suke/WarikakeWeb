using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using WarikakeWeb.Data;
using WarikakeWeb.Logic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WarikakeWeb.Models
{

    //SQLで取得する際のモデル
    public class WarikakeQuery
    { 
        public int CostId { get; set; }
        [Display(Name = "Cost Title")]
        public String? CostTitle { get; set; }
        public int GroupId { get; set; }
        [Display(Name = "Group Name")]
        public string? GroupName { get; set; }
        public int GenreId { get; set; }
        [Display(Name = "Genre Name")]
        public String? GenreName { get; set; }
        [Display(Name = "status")]
        public int status { get; set; }
        public int CostStatus { get; set; }
        [Display(Name = "Cost Amount")]
        public int CostAmount { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Cost Date")]
        public DateTime CostDate { get; set; }
        public int PayId { get; set; }
        public int PayUserId { get; set; }
        [Display(Name = "Pay User Name")]
        public String PayUserName { get; set; }
        [Display(Name = "Pay Amount")]
        public int PayAmount { get; set; }
        public int RepayId { get; set; }
        public int RepayUserId { get; set; }
        [Display(Name = "Repay User Name")]
        public String RepayUserName { get; set; }
        [Display(Name = "Repay Amount")]
        public int RepayAmount { get; set; }


        private readonly WarikakeWebContext _context;

        public WarikakeQuery(WarikakeWebContext context)
        {
            _context = context;
        }

        // 仮登録、未精算の一覧取得
        public List<WarikakeQuery> GetUnSettledWarikakeQueries(int GroupId)
        {
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


        public void CreateLogic(WarikakeDisp input, int GroupId, int UserId)
        {
            DateTime currDate = DateTime.Now;
            String currPg = "WarikakeInsert";
            ModelLogic modelLogic = new ModelLogic(_context);

            int payId = 0;
            int repayId = 0;

            int updStatus = 0;
            string updGenreName = "";
            if (input.GenreId == 0)
            {
                updStatus = (int)statusEnum.手動精算;
                updGenreName = (statusEnum.手動精算).ToString();
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
            TCost currCost = modelLogic.GetCurrentCost((int)UserId, currDate, currPg);
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

        public void UpdateLogic(WarikakeDisp input, int GroupId, int UserId, int status, Boolean isStatus)
        {
            DateTime dateTime = DateTime.Now;
            int CostId = input.CostId;


            TCost existingCost = _context.TCost.FirstOrDefault(c => c.CostId == CostId);
            existingCost.CostId = CostId;
            if (isStatus)
            {
                existingCost.status = status;
            }
            existingCost.CostTitle = input.CostTitle;
            existingCost.GroupId = (int)GroupId;
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
    }
    // 画面表示用モデル　検索条件を持つ
    public class WarikakeSearch
    {
        // ステータス
        // 年月
        // 種別
        // 人


        public List<WarikakeDisp> warikakeDisps { get; set; }

        public WarikakeDisp warikakeSum { get; set; }

        public WarikakeSearch()
        {
            warikakeDisps = new List<WarikakeDisp>();
            warikakeSum = new WarikakeDisp();
        }
    }


    // 画面表示用モデル　リストで一覧表示用と、同型の合計表示用を持っている
    public class WarikakeIndex
    {
        public List<WarikakeDisp> warikakeDisps { get; set; }

        public WarikakeDisp warikakeSum { get; set;}

        public WarikakeIndex()
        {
            warikakeDisps = new List<WarikakeDisp>();
            warikakeSum = new WarikakeDisp();
        }
    }

    //　画面表示用モデル（親）
    public class WarikakeDisp
    {
        public int CostId { get; set; }
        [Display(Name = "備考")]
        public String? CostTitle { get; set; }
        public int GroupId { get; set; }
        [Display(Name = "グループ")]
        public string? GroupName { get; set; }
        [Display(Name = "種別")]
        public int GenreId { get; set; }
        [Display(Name = "種別")]
        public String? GenreName { get; set; }
        [Display(Name = "登録状態")]
        public int status { get; set; }
        [Display(Name = "清算状況")]
        public String? statusName { get; set; }
        public int CostStatus { get; set; }
        [Display(Name = "支払額")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        public int CostAmount { get; set; }
        [Display(Name = "支払額")]
        public String? CostDisp { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "支払日")]
        public DateTime CostDate { get; set; }
        public List<WarikakePayDisp> Pays { get; set; }
        public List<WarikakeRepayDisp> Repays { get; set; }

        public WarikakeDisp()
        {
            Pays = new List<WarikakePayDisp>();
            Repays = new List<WarikakeRepayDisp>();
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
                payDisp.PayUserOn = (item.PayAmount != 0 ? true : false);
                payDisp.PayUserName = item.PayUserName;
                payDisp.PayAmount = item.PayAmount;
                payDisp.PayDisp = item.PayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Pays.Add(payDisp);

                WarikakeRepayDisp repayDisp = new WarikakeRepayDisp();
                repayDisp.RepayId = item.RepayId;
                repayDisp.RepayUserId = item.RepayUserId;
                repayDisp.RepayUserOn = (item.RepayAmount != 0 ? true : false);
                repayDisp.RepayUserName = item.RepayUserName;
                repayDisp.RepayAmount = item.RepayAmount;
                repayDisp.RepayDisp = item.RepayAmount.ToString("C0", new CultureInfo("ja-JP"));
                wariDisp.Repays.Add(repayDisp);
            }
            return wariDisp;
        }

        // 入力画面表示用に編集
        public WarikakeDisp GetWarikakeDispInit(List<MUser> users, int UserId, Boolean repayOn)
        {
            WarikakeDisp input = new WarikakeDisp();
            input.GroupId = (int)GroupId;
            DateTime dateTime = DateTime.Now;
            input.CostDate = dateTime;
            foreach (MUser user in users)
            {
                WarikakePayDisp inputPay = new WarikakePayDisp();
                inputPay.PayUserId = user.UserId;
                inputPay.PayUserOn = (user.UserId == UserId ? true : false);
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



    }

    // statusのenum
    public enum statusEnum
    {
        仮登録 = 0,
        未精算 = 1,
        精算済 = 2,
        一括精算済 = 3,
        手動精算 = 4,
        定期登録 = 6,
        合計 = 7,
        移行 = 8,
        削除 = 9
    }
    //　画面表示用モデル（子１）
    public class WarikakePayDisp
    {
        public int PayId { get; set; }
        public int PayUserId { get; set; }
        public Boolean PayUserOn { get; set; }
        [Display(Name = "メンバー")]
        public String? PayUserName { get; set; }
        [Display(Name = "立替額")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        public int PayAmount { get; set; }
        [Display(Name = "立替額")]
        public String? PayDisp {  get; set; }
    }
    //　画面表示用モデル（子２）
    public class WarikakeRepayDisp
    {
        public int RepayId { get; set; }
        public int RepayUserId { get; set; }
        [Display(Name = "メンバー")]
        public Boolean RepayUserOn { get; set; }
        public String? RepayUserName { get; set; }
        [Display(Name = "割勘額")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        public int RepayAmount { get; set;}
        [Display(Name = "割勘額")]
        public String ? RepayDisp { get; set; }
    }




    // メッセージ表示用モデル（親）
    public class WarikakeProcess
    {
        public int memberNumber { get; set; }
        [Display(Name = "精算時受け取る側")]
        public List<WarikakeUserProcess> plusUsers { get; set; }
        [Display(Name = "精算時支払う側")]
        public List<WarikakeUserProcess> minusUsers { get; set; }

        public WarikakeProcess()
        {
            memberNumber = 0;
            plusUsers = new List<WarikakeUserProcess>();
            minusUsers = new List<WarikakeUserProcess>();
        }

        // 表示用メッセージ一覧の取得


        public String repayMessage(List<WarikakeQuery> warikakeQueries)
        {
            WarikakeProcess warikakeProcess = new WarikakeProcess();
            warikakeProcess = warikakeProcess.GetWarikakeProcess(warikakeQueries);
            // 精算指示の文字列作成
            StringBuilder sb = new StringBuilder("");
            foreach (WarikakeUserProcess plsUsr in warikakeProcess.plusUsers)
            {
                sb.Append(plsUsr.UserName).Append("は合計").Append(plsUsr.processAmount).Append("円を支払うこと。\r\n");
            }
            foreach (WarikakeUserProcess mnsUsr in warikakeProcess.minusUsers)
            {
                sb.Append(mnsUsr.UserName).Append("は合計").Append(mnsUsr.processAmount * -1).Append("円受け取ること。\r\n");
            }
            return sb.ToString();
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
    // メッセージ表示用モデル（子）
    public class WarikakeUserProcess
    {
        public int UserId { get; set; }
        [Display(Name = "精算者")]
        public String UserName { get; set; }
        [Display(Name = "精算額")]
        public int processAmount { get; set; }
    }

}
