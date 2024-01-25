using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class MGenreDisp
    {
        public int Id { get; set; }
        [Display(Name = "名称")]
        public string GenreName { get; set; }

    }
}
