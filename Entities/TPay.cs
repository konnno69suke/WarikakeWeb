using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Entities
{
    public class TPay
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int CostId { get; set; }
        public int PayId { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Pay Amount")]
        public int PayAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }
    }
}
