namespace WarikakeWeb.Entities
{
    public class TSubscribe
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int SubscribeId { get; set; }
        public int CostId { get; set; }
        public DateTime SubscribedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }
    }
}
