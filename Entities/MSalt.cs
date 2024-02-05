namespace WarikakeWeb.Entities
{
    public class MSalt
    {
        public int Id { get; set; }
        public int status { get; set; }
        public int UserId { get; set; }
        public byte[] salt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }

        public string ToString()
        {
            FormattableString fs = $"MSalt :{Id}, {status}, {UserId}, ******";
            return fs.ToString();
        }
    }
}
