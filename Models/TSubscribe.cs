namespace WarikakeWeb.Models
{
    public class TSubscribe
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int SubscribeId { get; set; }
        public int CostId { get; set; }
        public DateTime SubscribedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }
    }
}
