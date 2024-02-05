using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarikakeWeb.ViewModel
{
    public class MGroupQuery
    {
        public int Id { get; set; }
        public string groupName { get; set; }
        public string masterName { get; set; }
        public DateTime startDate { get; set; }
        public int memId { get; set; }
        public string memName { get; set; }
        public DateTime memStartDate { get; set; }
    }


    public class MGroupDisp
    {
        public int Id { get; set; }
        [Display(Name = "グループ名")]
        public string groupName { get; set; }
        [Display(Name = "代表者")]
        public string? masterName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "開始日")]
        public DateTime startDate { get; set; }
        [Display(Name = "メンバー一覧")]
        [NotMapped]
        public List<MUserDisp>? userList { get; set; } = new List<MUserDisp>();

    }
}
