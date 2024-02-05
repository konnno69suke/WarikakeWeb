using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Entities
{
    public class MGroup
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int GroupId { get; set; }
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }

        public string ToString()
        {
            FormattableString fs = $"MGroup :{Id}, {status}, {GroupId}, {GroupName}, {UserId}, {StartDate}";
            return fs.ToString();
        }
    }
}
