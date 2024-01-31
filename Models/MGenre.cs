using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WarikakeWeb.Data;

namespace WarikakeWeb.Models
{
    public class MGenre
    {
        private WarikakeWebContext _context;

        public MGenre()
        {

        }

        public MGenre(WarikakeWebContext context)
        {
            _context = context;
        }

        public int Id { get; set; }
        public int? status { get; set; }
        [Display(Name = "Genre Name")]
        public String GenreName { get; set; }
        public int GenreId { get; set; }
        public int GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }

        // グループの種別一覧を取得（初期値あり）
        public SelectList GetSelectList(int groupId, object genreId)
        {
            List<MGenre> genres = _context.MGenre.Where(g => g.GroupId == groupId && g.status == 1).ToList();
            return new SelectList(genres.Select(u => new { Id = u.GenreId, Name = u.GenreName }), "Id", "Name", genreId);
        }

        // グループの種別一覧を取得（初期値なし）
        public SelectList GetSelectList(int groupId)
        {
            List<MGenre> genres = _context.MGenre.Where(g => g.GroupId == groupId && g.status == 1).ToList();
            return new SelectList(genres.Select(u => new { Id = u.GenreId, Name = u.GenreName }), "Id", "Name");
        }

        public String ToString() {
            FormattableString fs = $"MGenre :{Id}, {status}, {GenreName}, {GenreId}, {GroupId}";
            return fs.ToString();
        }
    }
}
