using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class Login
    {
        [Display(Name = "ログインＩＤ")]
        public string EMail { get; set; }
        [Display(Name = "パスワード")]
        public string Password { get; set; }
    }
}
