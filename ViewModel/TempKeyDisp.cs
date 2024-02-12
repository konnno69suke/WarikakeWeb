using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class TempKeyDisp
    {
        [Display(Name = "一時キー")]
        public string TempKey { get; set; }
    }
}
