using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using WarikakeWeb.Entities;

namespace WarikakeWeb.ViewModel
{

    //SQLで取得する際のモデル
    public class WarikakeQuery
    {
        public int CostId { get; set; }
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
        [DataType(DataType.Date)]
        [Display(Name = "Cost Date")]
        public DateTime CostDate { get; set; }
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
    }
    // 画面表示用モデル　検索条件を持つ
    public class WarikakeSearch : WarikakeIndex
    {
        // ステータス
        // 年月
        // 種別
        public int GenreId { get; set; }
        // 人

        public string currDisp { get; set; }
        public int prevYear { get; set; }
        public int currYear { get; set; }
        public int nextYear { get; set; }

        public string prevMonth { get; set; }
        public string currMonth { get; set; }
        public string nextMonth { get; set; }

        public string prevDate { get; set; }
        public string currDate { get; set; }
        public string nextDate { get; set; }


        public string warikakeChart { get; set; }
    }


    // 画面表示用モデル　リストで一覧表示用と、同型の合計表示用を持っている
    public class WarikakeIndex
    {
        public List<WarikakeDisp> warikakeDisps { get; set; }

        public WarikakeDisp warikakeSum { get; set; }

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

        //ログ出力用にオーバーライド
        public string ToString()
        {
            FormattableString fs = $"Wari :{CostId}, {CostTitle}, {GroupId}, {GroupName}, {GenreId}, {GenreName}, {status}, {statusName}, {CostStatus}, {CostAmount}, {CostDisp}, {CostDate}";
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
            return sb.ToString();
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
        public bool PayUserOn { get; set; }
        [Display(Name = "メンバー")]
        public string? PayUserName { get; set; }
        [Display(Name = "立替額")]
        [Range(0, int.MaxValue, ErrorMessage = "0以上の整数を入力してください")]
        public int PayAmount { get; set; }
        [Display(Name = "立替額")]
        public string? PayDisp { get; set; }
    }
    //　画面表示用モデル（子２）
    public class WarikakeRepayDisp
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
    }
    // メッセージ表示用モデル（子）
    public class WarikakeUserProcess
    {
        public int UserId { get; set; }
        [Display(Name = "精算者")]
        public string UserName { get; set; }
        [Display(Name = "精算額")]
        public int processAmount { get; set; }
    }


    public class WarikakeGraph
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public int Amount1 { get; set; }
        public int Amount2 { get; set; }
        public int Amount3 { get; set; }
        public int Amount4 { get; set; }
        public int Amount5 { get; set; }
        public int Amount6 { get; set; }
        public int Amount7 { get; set; }
        public int Amount8 { get; set; }
        public int Amount9 { get; set; }
        public int Amount10 { get; set; }
        public int Amount11 { get; set; }
        public int Amount12 { get; set; }
    }

    // グラフ表示用にCharts.jsに合わせたモデル
    public class WarikakeChart
    {
        public string type { get; set; }
        public ChartData data { get; set; }

        public ChartOption options { get; set; }

        public WarikakeChart()
        {
            type = "'line'";
            data = new ChartData();
            data.labels = new List<string>();
            data.datasets = new List<ChartDataset>();
            options = new ChartOption();
            options.scales = new ChartScale();
            options.scales.y = new ChartY();
            options.scales.y.beginAtZero = true;
        }
    }

    public class ChartData
    {
        public List<string> labels { get; set; }
        public List<ChartDataset> datasets { get; set; }
    }
    public class ChartOption
    {
        public ChartScale scales { get; set; }
    }
    public class ChartDataset
    {
        public string label { get; set; }
        public List<int> data { get; set; } = new List<int>();
        public int borderWidth { get; set; }
    }
    public class ChartScale
    {
        public ChartY y { get; set; }
    }
    public class ChartY
    {
        public bool beginAtZero { get; set; }
    }
}
