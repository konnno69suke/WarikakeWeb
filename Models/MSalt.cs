namespace WarikakeWeb.Models
{
    public class MSalt
    {
        public int Id { get; set; }
        public int status { get; set; }
        public int UserId { get; set; }
        public byte[] salt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }

        public String ToString()
        {
            FormattableString fs = $"MSalt :{Id}, {status}, {UserId}, ******";
            return fs.ToString();
        }
    }
}
