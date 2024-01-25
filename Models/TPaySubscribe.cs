using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
{
    public class TPaySubscribe
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int SubscribeId { get; set; }
        public int PayId { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Pay Amount")]
        public int PayAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }
    }
}
