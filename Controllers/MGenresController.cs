using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Controllers
{
    [Authorize]
    public class MGenresController : Controller
    {
        private readonly WarikakeWebContext _context;

        public MGenresController(WarikakeWebContext context)
        {
            _context = context;
        }

        // GET: MGenres
        public ActionResult Index()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // DB検索
            GenreModel model = new GenreModel(_context);
            List<MGenre> genres = model.GetGenres((int)GroupId);
            // 画面表示向けに編集
            List<MGenreDisp> genreDisps = model.GetDisps(genres);

            return View(genreDisps);
        }

        // GET: MGenres/Create
        public ActionResult Create()
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,GenreName")] MGenreDisp mGenreDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(mGenreDisp);
            }

            try
            {
                // 登録処理
                GenreModel model = new GenreModel(_context);
                model.CreateLogic(mGenreDisp, (int)UserId, (int)GroupId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(mGenreDisp);
            }
        }

        // GET: MGenres/Edit/5
        public ActionResult Edit(int? id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id == null)
            {
                return NotFound();
            }

            // 検索処理
            GenreModel model = new GenreModel(_context);
            MGenre mGenre = model.GetGenreById((int)id);
            if (mGenre == null)
            {
                return NotFound();
            }

            // 画面表示処理
            MGenreDisp mGenreDisp = model.GetGenreDisp(mGenre);

            return View(mGenreDisp);
        }

        // POST: MGenres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, [Bind("Id,GenreName")] MGenreDisp mGenreDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id == null)
            {
                return NotFound();
            }

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(mGenreDisp);
            }

            try
            {
                // 更新処理
                GenreModel model = new GenreModel(_context);
                model.UpdateLogic(mGenreDisp, (int)UserId, (int)id);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                View(mGenreDisp);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: MGenres/Delete/5
        public ActionResult Delete(int? Id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (Id == null)
            {
                return NotFound();
            }

            // 検索処理
            GenreModel model = new GenreModel(_context);
            var mGenre = model.GetGenreById((int)Id);
            if (mGenre == null)
            {
                return NotFound();
            }

            // 画面表示処理
            MGenreDisp genreDisp = model.GetGenreDisp(mGenre);

            return View(genreDisp);
        }

        // POST: MGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? Id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (Id == null)
            {
                return NotFound();
            }

            // 検索処理
            GenreModel model = new GenreModel(_context);
            var mGenre = model.GetGenreById((int)Id);

            WarikakeModel wModel = new WarikakeModel(_context);
            int gCnt = wModel.GetGenreUsedCount((int)GroupId, mGenre.GenreId);
            if(gCnt > 0)
            {
                ModelState.AddModelError(nameof(MGenreDisp.Id), "この種別は既に使用されています");
                MGenreDisp mGenreDisp = model.GetGenreDisp(mGenre);
                return View(mGenreDisp);
            }

            try
            {
                // 論理削除
                model.StatusChangeLogic(mGenre, (int)UserId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                MGenreDisp mGenreDisp = model.GetGenreDisp(mGenre);
                return View(mGenreDisp);
            }
        }

    }
}
