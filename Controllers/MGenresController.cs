using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
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

            List<MGenre> genres = _context.MGenre.Where(g => g.status == 1 && g.GroupId == GroupId).ToList();
            List<MGenreDisp> genreDisps = new List<MGenreDisp>();
            foreach (MGenre genre in genres)
            {
                MGenreDisp genreDisp = new MGenreDisp();
                genreDisp.Id = genre.Id;
                genreDisp.GenreName = genre.GenreName;

                genreDisps.Add(genreDisp);
            }
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

            if (ModelState.IsValid)
            {
                // 業務入力チェック

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
                _context.Add(mGenre);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(mGenreDisp);
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

            var mGenre = _context.MGenre.Where(g => g.Id == id && g.status == 1).FirstOrDefault();
            if (mGenre == null)
            {
                return NotFound();
            }
            MGenreDisp mGenreDisp = new MGenreDisp();
            mGenreDisp.Id = mGenre.Id;
            mGenreDisp.GenreName = mGenre.GenreName;

            return View(mGenreDisp);
        }

        // POST: MGenres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,GenreName")] MGenreDisp mGenreDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id != mGenreDisp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // 業務入力チェック

                try
                {
                    string currPg = "MGenreUpdate";
                    DateTime currDate = DateTime.Now;

                    MGenre existingGenre = _context.MGenre.FirstOrDefault(g => g.Id == id && g.status == 1);
                    existingGenre.GenreName = mGenreDisp.GenreName;
                    existingGenre.UpdatedDate = currDate;
                    existingGenre.UpdateUser = UserId.ToString();
                    existingGenre.UpdatePg = currPg;
                    _context.Update(existingGenre);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    View(mGenreDisp);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mGenreDisp);
        }

        // GET: MGenres/Delete/5
        public ActionResult Delete(int? id)
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

            var mGenre = _context.MGenre.FirstOrDefault(m => m.Id == id && m.status == 1);
            if (mGenre == null)
            {
                return NotFound();
            }

            MGenreDisp genreDisp = new MGenreDisp();
            genreDisp.Id = mGenre.Id;
            genreDisp.GenreName = mGenre.GenreName;

            return View(genreDisp);
        }

        // POST: MGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            var mGenre = _context.MGenre.FirstOrDefault(m => m.Id == id && m.status == 1);
            if (mGenre != null)
            {
                string currPg = "MGenreDelete";
                DateTime currDate = DateTime.Now;

                mGenre.status = (int)statusEnum.削除;
                mGenre.UpdatedDate = currDate;
                mGenre.UpdateUser = UserId.ToString();
                mGenre.UpdatePg = currPg;
                _context.Update(mGenre);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        private int getNextGenreId()
        {
            int GenreId = _context.MGenre.Any() ? _context.MGenre.Max(a => a.GenreId) : -0;

            GenreId++;

            return GenreId;
        }
    }
}
