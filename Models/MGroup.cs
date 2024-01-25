using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.Models
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
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }
    }
}
