using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class MGenreDisp
    {
        public int Id { get; set; }
        [Display(Name = "名称")]
        public string GenreName { get; set; }

    }
}
