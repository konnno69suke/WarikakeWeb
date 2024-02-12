using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class GroupRequestDisp
    {
        public string NewOrJoin { get; set; } = "Join";
        [Display(Name = "新規グループ名")]
        public string? NewGroupName { get; set; }
        public int? GroupId { get; set; }
        [Display(Name = "参加先グループ名")]
        public string? GroupName { get; set; }
        [Display(Name = "一時キー")]
        public string? TempKey { get; set; }
    }
}
