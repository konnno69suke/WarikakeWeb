using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class HomeDisp
    {
        public int? UserId { get; set; }
        [Display(Name = "ユーザー")]
        public string? UserName { get; set; }
        public int? GroupId { get; set; }
        [Display(Name = "グループ")]
        public string? GroupName { get; set; }
        public int? GroupUserId { get; set; }

        public bool RequestFlg { get; set; } = false;
    }
}
