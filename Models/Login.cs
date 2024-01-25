﻿using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class Login
    {
        [Display(Name = "ログインＩＤ")]
        public string EMail { get; set; }
        [Display(Name = "パスワード")]
        public string Password { get; set; }
    }
}
