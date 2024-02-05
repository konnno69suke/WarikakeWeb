using System.ComponentModel.DataAnnotations;
using System.Text;
using WarikakeWeb.Entities;

namespace WarikakeWeb.ViewModel
{
    public class SubscribeQuery
    {
        public int SubscribeId { get; set; }
        [Display(Name = "Cost Title")]
        public string? CostTitle { get; set; }
        public int GroupId { get; set; }
        [Display(Name = "Group Name")]
        public string? GroupName { get; set; }
        public int GenreId { get; set; }
        [Display(Name = "Genre Name")]
        public string? GenreName { get; set; }
        [Display(Name = "status")]
        public int status { get; set; }
        public int CostStatus { get; set; }
        [Display(Name = "Cost Amount")]
        public int CostAmount { get; set; }
        public int PayId { get; set; }
        public int PayUserId { get; set; }
        [Display(Name = "Pay User Name")]
        public string PayUserName { get; set; }
        [Display(Name = "Pay Amount")]
        public int PayAmount { get; set; }
        public int RepayId { get; set; }
        public int RepayUserId { get; set; }
        [Display(Name = "Repay User Name")]
        public string RepayUserName { get; set; }
        [Display(Name = "Repay Amount")]
        public int RepayAmount { get; set; }
        public DateTime? LastSubscribedDate { get; set; }
        public int? LastCostId { get; set; }

    }


    public class SubscribeDisp
    {
        public bool isChecked { get; set; }
        public int SubscribeId { get; set; }
        [Display(Name = "備考")]
        public string? CostTitle { get; set; }
        public int GroupId { get; set; }
        [Display(Name = "グループ")]
        public string? GroupName { get; set; }
        [Display(Name = "種別")]
        public int GenreId { get; set; }
        [Display(Name = "種別")]
        public string? GenreName { get; set; }
        [Display(Name = "登録状態")]
        public int status { get; set; }
        [Display(Name = "清算状況")]
        public string? statusName { get; set; }
        public int CostStatus { get; set; }
        [Display(Name = "支払額")]
        [Range(1, int.MaxValue, ErrorMessage = "1以上の整数を入力してください")]
        public int CostAmount { get; set; }
        [Display(Name = "支払額")]
        public string? CostDisp { get; set; }
        public List<SubscribePayDisp> Pays { get; set; }
        public List<SubscribeRepayDisp> Repays { get; set; }
        public bool isAllMonth { get; set; }
        public string weekOrDay { get; set; }
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


        //ログ出力用にオーバーライド
        public string ToString()
        {
            FormattableString fs = $"Wari :{isChecked}, {SubscribeId}, {CostTitle}, {GroupId}, {GroupName}, {GenreId}, {GenreName}, {status}, {statusName}, {CostStatus}, {CostAmount}, {CostDisp}, {isAllMonth}, {weekOrDay}, {LastSubscribedDate}, {LastCostId}";
            StringBuilder sb = new StringBuilder();
            sb.Append(fs);
            for (int i = 0; i < Pays.Count; i++)
            {
                sb.Append($" Pays({i}) :{Pays[i].PayId}, {Pays[i].PayUserId}, {Pays[i].PayUserOn}, {Pays[i].PayUserName}, {Pays[i].PayAmount}, {Pays[i].PayDisp}");
            }
            for (int j = 0; j < Repays.Count; j++)
            {
                sb.Append($" Reps({j}) :{Repays[j].RepayId}, {Repays[j].RepayUserId}, {Repays[j].RepayUserOn}, {Repays[j].RepayUserName}, {Repays[j].RepayAmount}, {Repays[j].RepayDisp}");
            }
            sb.Append($" TDateSubscribe :{dateSubscribe.Id}, {dateSubscribe.status}, {dateSubscribe.SubscribeId}, {dateSubscribe.UserId}, " +
                $"{dateSubscribe.m1}, {dateSubscribe.m2}, {dateSubscribe.m3}, {dateSubscribe.m4}, {dateSubscribe.m5}, {dateSubscribe.m6}, {dateSubscribe.m7}, {dateSubscribe.m8}, {dateSubscribe.m9}, {dateSubscribe.m10}, {dateSubscribe.m11}, {dateSubscribe.m12}, " +
                $"{dateSubscribe.r1}, {dateSubscribe.r2}, {dateSubscribe.r3}, {dateSubscribe.r4}, {dateSubscribe.r5}, " +
                $"{dateSubscribe.w1}, {dateSubscribe.w2}, {dateSubscribe.w3}, {dateSubscribe.w4}, {dateSubscribe.w5}, {dateSubscribe.w6}, {dateSubscribe.w7}, " +
                $"{dateSubscribe.d1}, {dateSubscribe.d2}, {dateSubscribe.d3}, {dateSubscribe.d4}, {dateSubscribe.d5}, {dateSubscribe.d6}, {dateSubscribe.d7}, {dateSubscribe.d8}, {dateSubscribe.d9}, {dateSubscribe.d10}, " +
                $"{dateSubscribe.d11}, {dateSubscribe.d12}, {dateSubscribe.d13}, {dateSubscribe.d14}, {dateSubscribe.d15}, {dateSubscribe.d16}, {dateSubscribe.d17}, {dateSubscribe.d18}, {dateSubscribe.d19}, {dateSubscribe.d20}, " +
                $"{dateSubscribe.d21}, {dateSubscribe.d22}, {dateSubscribe.d23}, {dateSubscribe.d24}, {dateSubscribe.d25}, {dateSubscribe.d26}, {dateSubscribe.d27}, {dateSubscribe.d28}, {dateSubscribe.d29}, {dateSubscribe.d30}, " +
                $"{dateSubscribe.d31}");

            return sb.ToString();
        }
    }

    public class SubscribePayDisp
    {
        public int PayId { get; set; }
        public int PayUserId { get; set; }
        public bool PayUserOn { get; set; }
        [Display(Name = "メンバー")]
        public string? PayUserName { get; set; }
        [Display(Name = "立替額")]
        [Range(0, int.MaxValue, ErrorMessage = "0以上の整数を入力してください")]
        public int PayAmount { get; set; }
        [Display(Name = "立替額")]
        public string? PayDisp { get; set; }
    }
    public class SubscribeRepayDisp
    {
        public int RepayId { get; set; }
        public int RepayUserId { get; set; }
        [Display(Name = "メンバー")]
        public bool RepayUserOn { get; set; }
        public string? RepayUserName { get; set; }
        [Display(Name = "割勘額")]
        [Range(0, int.MaxValue, ErrorMessage = "0以上の整数を入力してください")]
        public int RepayAmount { get; set; }
        [Display(Name = "割勘額")]
        public string? RepayDisp { get; set; }
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
        public string UserName { get; set; }
        [Display(Name = "精算額")]
        public int processAmount { get; set; }
    }
}
