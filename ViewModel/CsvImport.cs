using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class CsvImport
    {
        [Display(Name = "移行ファイル")]
        public IFormFile? formFile { get; set; }

        public List<MigrationDisp> MigrationDisps { get; set; } = new List<MigrationDisp>(); 
    }

    public class MigrationDisp
    {
        public MigrationDisp() 
        { 
        
        }

        public int ImportId { get; set; }

        public int Status { get; set; }

        [Display(Name = "移行状態")]
        public string StatusName { get; set; }

        public int UserId { get; set; }

        [Display(Name = "ユーザー")]
        public string UserName { get; set; }

        [Display(Name = "件数")]
        public int ImpCount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "開始日")]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "終了日")]
        public DateTime UntilDate { get; set; }
    }
}


