namespace WarikakeWeb.Models
{
    public class CsvMigration
    {
        public int Id { get; set; }
        public int status { get; set; }
        public String inputDate { get; set; }
        public String buyDate { get; set; }
        public String kindName { get; set; }
        public String buyAmount { get; set; }
        public String pf1 { get; set; }
        public String pf2 { get; set; }
        public String pf3 { get; set; }
        public String pa1 { get; set; }
        public String pa2 { get; set; }
        public String pa3 { get; set; }
        public String pr1 { get; set; }
        public String pr2 { get; set; }
        public String pr3 { get; set; }
        public String buyStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }
    }
}
