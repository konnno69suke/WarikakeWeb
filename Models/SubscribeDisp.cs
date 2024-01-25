using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class SubscribeQuery
    {
        public int SubscribeId { get; set; }
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
        public DateTime? LastSubscribedDate { get; set; }
        public int? LastCostId { get; set; }

    }

    public class SubscribeDisp
    {
        public Boolean isChecked { get; set; }
        public int SubscribeId { get; set; }
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
        public List<SubscribePayDisp> Pays { get; set; }
        public List<SubscribeRepayDisp> Repays { get; set; }
        public Boolean isAllMonth { get; set; }
        public String weekOrDay { get; set; }
        [Display(Name = "最終仮登録日")]
        public DateTime? LastSubscribedDate { get; set; }
        public int? LastCostId { get; set; }
        public TDateSubscribe? dateSubscribe { get; set; }

        public SubscribeDisp()
        {
            isChecked = true;
            Pays = new List<SubscribePayDisp>();
            Repays = new List<SubscribeRepayDisp>();
            dateSubscribe = new TDateSubscribe();
        }
    }

    public class SubscribePayDisp
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
        public String? PayDisp { get; set; }
    }
    public class SubscribeRepayDisp
    {
        public int RepayId { get; set; }
        public int RepayUserId { get; set; }
        [Display(Name = "メンバー")]
        public Boolean RepayUserOn { get; set; }
        public String? RepayUserName { get; set; }
        [Display(Name = "割勘額")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer")]
        public int RepayAmount { get; set; }
        [Display(Name = "割勘額")]
        public String? RepayDisp { get; set; }
    }
    public class SubscribeProcess
    {
        public int memberNumber { get; set; }
        [Display(Name = "精算時受け取る側")]
        public List<SubscribeUserProcess> plusUsers { get; set; }
        [Display(Name = "精算時支払う側")]
        public List<SubscribeUserProcess> minusUsers { get; set; }

        public SubscribeProcess()
        {
            memberNumber = 0;
            plusUsers = new List<SubscribeUserProcess>();
            minusUsers = new List<SubscribeUserProcess>();
        }
    }
    public class SubscribeUserProcess
    {
        public int UserId { get; set; }
        [Display(Name = "精算者")]
        public String UserName { get; set; }
        [Display(Name = "精算額")]
        public int processAmount { get; set; }
    }
}
