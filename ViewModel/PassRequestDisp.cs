using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class PassRequestDisp
    {
        [Display(Name = "グループ名")]
        public string GroupName { get; set; }
        [Display(Name = "ログインID")]
        public string EMail { get; set; }
    }
}
