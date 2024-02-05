using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Entities
{
    public class TRepaySubscribe
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int SubscribeId { get; set; }
        public int RepayId { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Repay Amount")]
        public int RepayAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }
    }
}
