using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class TCostSubscribe
    {
        public int Id { get; set; }
        public int SubscribeId { get; set; }
        public int? status { get; set; }
        [Display(Name = "Cost Title")]
        public String? CostTitle { get; set; }
        public int GroupId { get; set; }
        public int GenreId { get; set; }
        [Display(Name = "Genre Name")]
        public String? GenreName { get; set; }
        [Display(Name = "Provision Flag")]
        public int ProvisionFlg { get; set; }
        [Display(Name = "Cost Amount")]
        public int CostAmount { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Cost Date")]
        public int CostStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }
    }
}
