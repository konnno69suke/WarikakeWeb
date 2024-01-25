using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
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
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }
    }
}
