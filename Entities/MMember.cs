namespace WarikakeWeb.Entities
{
    public class MMember
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }

        public string ToString()
        {
            FormattableString fs = $"MMember :{Id}, {status}, {GroupId}, {UserId}";
            return fs.ToString();
        }
    }
}
