using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WarikakeWeb.Data;

namespace WarikakeWeb.Entities
{
    public class MUser
    {
        private WarikakeWebContext _context;

        public MUser()
        {

        }

        public MUser(WarikakeWebContext context)
        {
            _context = context;
        }

        public int Id { get; set; }
        public int? status { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }

        // 指定したグループのユーザー一覧を取得
        public List<MUser> GetUsers(int GroupId)
        {
            Serilog.Log.Information($"SQL param: {GroupId}");
            List<MUser> users = _context.Database.SqlQuery<MUser>(
               $@"select mu.*
                    from muser mu 
                    inner join mmember mm on mu.UserId = mm.UserId
                    where mm.GroupId = {GroupId}
                    and mu.status = 1 and mm.status = 1
                    order by mu.UserId").ToList();
            return users;
        }


        public string ToString()
        {
            FormattableString fs = $"MSalt :{Id}, {status}, {UserId}, {UserName}, ******, {Email}";
            return fs.ToString();
        }
    }

}
