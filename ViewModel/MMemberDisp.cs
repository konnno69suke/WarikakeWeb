using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class MMemberDisp
    {
        public int Id { get; set; }
        public int gid { get; set; }
        [Display(Name = "グループ名")]
        public string? groupName { get; set; }
        public int mid { get; set; }
        [Display(Name = "メンバー名")]
        public string? userName { get; set; }
    }
}
