using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class MMemberDisp
    {
        public int Id { get; set; }
        public int gid { get; set; }
        [Display(Name = "グループ名")]
        public String? groupName { get; set; }
        public int mid { get; set; }
        [Display(Name = "メンバー名")]
        public String? userName { get; set; }
    }
}
