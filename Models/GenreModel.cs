using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Models
{
    public class GenreModel
    {
        WarikakeWebContext _context;

        public GenreModel(WarikakeWebContext context)
        {
            _context = context;
        }

        public List<MGenre> GetGenres(int GroupId)
        {
            List<MGenre> genres = _context.MGenre.Where(g => g.status == 1 && g.GroupId == GroupId).ToList();
            return genres;
        }
        public List<MGenreDisp> GetDisps(List<MGenre> genres)
        {
            List<MGenreDisp> genreDisps = new List<MGenreDisp>();
            foreach (MGenre genre in genres)
            {
                MGenreDisp genreDisp = new MGenreDisp();
                genreDisp.Id = genre.Id;
                genreDisp.GenreName = genre.GenreName;

                genreDisps.Add(genreDisp);
            }
            return genreDisps;
        }

        public MGenre GetGenreById(int id)
        {
            MGenre mGenre = _context.MGenre.Where(g => g.Id == id && g.status == 1).FirstOrDefault();
            return mGenre;
        }

        // 登録処理
        public void CreateLogic(MGenreDisp mGenreDisp, int UserId, int GroupId)
        {
            string currPg = "MGenreInsert";

            DateTime currDate = DateTime.Now;
            MGenre mGenre = new MGenre();
            mGenre.status = 1;
            mGenre.GenreName = mGenreDisp.GenreName;
            mGenre.GenreId = getNextGenreId();
            mGenre.GroupId = (int)GroupId;
            mGenre.CreatedDate = currDate;
            mGenre.CreateUser = UserId.ToString();
            mGenre.CreatePg = currPg;
            mGenre.UpdatedDate = currDate;
            mGenre.UpdateUser = UserId.ToString();
            mGenre.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MGenre:{mGenre.ToString()}");
            _context.Add(mGenre);
            _context.SaveChanges();
        }

        // 更新処理
        public void UpdateLogic(MGenreDisp mGenreDisp, int UserId, int id)
        {
            string currPg = "MGenreUpdate";
            DateTime currDate = DateTime.Now;

            MGenre existingGenre = _context.MGenre.FirstOrDefault(g => g.Id == id && g.status == 1);
            existingGenre.GenreName = mGenreDisp.GenreName;
            existingGenre.UpdatedDate = currDate;
            existingGenre.UpdateUser = UserId.ToString();
            existingGenre.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MGenre:{existingGenre.ToString()}");
            _context.Update(existingGenre);
            _context.SaveChanges();
        }

        // ステータス更新処理
        public void StatusChangeLogic(MGenre mGenre, int UserId)
        {
            // ステータス更新処理
            string currPg = "MGenreDelete";
            DateTime currDate = DateTime.Now;

            mGenre.status = (int)statusEnum.削除;
            mGenre.UpdatedDate = currDate;
            mGenre.UpdateUser = UserId.ToString();
            mGenre.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MGenre:{mGenre.ToString()}");
            _context.Update(mGenre);
            _context.SaveChanges();
        }

        // 画面表示向けの種別情報を取得
        public MGenreDisp GetGenreDisp(MGenre mGenre)
        {
            MGenreDisp mGenreDisp = new MGenreDisp();
            mGenreDisp.Id = mGenre.Id;
            mGenreDisp.GenreName = mGenre.GenreName;
            return mGenreDisp;
        }

        // 種別IDを発番
        private int getNextGenreId()
        {
            int GenreId = _context.MGenre.Any() ? _context.MGenre.Max(a => a.GenreId) : -0;

            GenreId++;

            return GenreId;
        }
    }
}
