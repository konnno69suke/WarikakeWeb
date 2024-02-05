using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class CsvImport
    {
        [Display(Name = "移行ファイル")]
        public IFormFile? formFile { get; set; }
    }
}


