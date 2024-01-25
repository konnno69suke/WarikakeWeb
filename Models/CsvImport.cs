using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class CsvImport
    {
        [Display(Name = "移行ファイル")]
        public IFormFile? formFile { get; set; }
    }
}


