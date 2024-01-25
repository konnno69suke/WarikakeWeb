using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WarikakeWeb.Data;

namespace WarikakeWeb.Models
{
    public class MUser
    {
        public int Id { get; set; }
        public int? status { get; set; }
        public int UserId { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }

        private readonly WarikakeWebContext _context;

        public MUser()
        {

        }

        public MUser(WarikakeWebContext context)
        {
            _context = context;

        }
        // グループのユーザー一覧を取得
        public List<MUser> GetUsers(int GroupId)
        {
            List<MUser> users = _context.Database.SqlQuery<MUser>(
               $@"select mu.*
                    from muser mu 
                    inner join mmember mm on mu.UserId = mm.UserId
                    where mm.GroupId = {GroupId}
                    and mu.status = 1 and mm.status = 1
                    order by mu.UserId").ToList();
            return users;
        }
    }

}
