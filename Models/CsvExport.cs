using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class CsvExport
    {
        [Display(Name = "ダブルクォート有無")]
        public string hasDoubleQuote { get; set; }
    }
}
