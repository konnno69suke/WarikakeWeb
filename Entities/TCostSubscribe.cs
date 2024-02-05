using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Entities
{
    public class TCostSubscribe
    {
        public int Id { get; set; }
        public int SubscribeId { get; set; }
        public int? status { get; set; }
        [Display(Name = "Cost Title")]
        public string? CostTitle { get; set; }
        public int GroupId { get; set; }
        public int GenreId { get; set; }
        [Display(Name = "Genre Name")]
        public string? GenreName { get; set; }
        [Display(Name = "Provision Flag")]
        public int ProvisionFlg { get; set; }
        [Display(Name = "Cost Amount")]
        public int CostAmount { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Cost Date")]
        public int CostStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }
    }
}
