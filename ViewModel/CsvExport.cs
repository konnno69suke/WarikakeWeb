using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class CsvExport
    {

        [DataType(DataType.Date)]
        [Display(Name = "日付で絞る(開始日)")]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "日付で絞る(終了日)")]
        public DateTime? UntilDate { get; set; }

        [Display(Name = "ダブルクォート有無")]
        public string hasDoubleQuote { get; set; }
    }
}
