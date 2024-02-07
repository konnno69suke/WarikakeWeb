using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.Text;
using System.Text.RegularExpressions;
using WarikakeWeb.Data;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Entities
{
    public class CsvMigration
    {
        public int Id { get; set; }
        public int importId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public int status { get; set; }
        public string inputDate { get; set; }
        public string buyDate { get; set; }
        public string kindName { get; set; }
        public string buyAmount { get; set; }
        public string pf1 { get; set; }
        public string pf2 { get; set; }
        public string pf3 { get; set; }
        public string pa1 { get; set; }
        public string pa2 { get; set; }
        public string pa3 { get; set; }
        public string pr1 { get; set; }
        public string pr2 { get; set; }
        public string pr3 { get; set; }
        public string buyStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }
    }
}
