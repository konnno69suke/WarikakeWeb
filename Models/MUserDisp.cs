﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarikakeWeb.Models
{
    public class MUserDisp
    {
        public int Id { get; set; }
        [Display(Name = "ユーザー表示名")]
        public String UserName { get; set; }
        [Display(Name = "ログインID")]
        public String Email { get; set; }
        [Display(Name = "パスワード")]
        public String Password { get; set; }
        [Display(Name = "パスワード(確認用)")]
        public String PasswordAssert { get; set; }
        [Display(Name = "新規パスワード")]
        public String? NewPassword { get; set; }
        [Display(Name = "登録日")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "グループ一覧")]
        [NotMapped]
        public List<MGroupDisp>? GroupDispList { get; set; }

        public MUserDisp() 
        {
            GroupDispList = new List<MGroupDisp>();
        }
    }
}
