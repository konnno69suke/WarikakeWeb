using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Xml.Linq;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    [Authorize]
    public class MUsersController : Controller
    {
        private readonly WarikakeWebContext _context;

        public MUsersController(WarikakeWebContext context)
        {
            _context = context;
        }

        // GET: MUsers
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

            MGroup mGroup = _context.MGroup.Where(g => g.GroupId == GroupId && g.status == 1).FirstOrDefault();
            List<MUser> users = null;
            if (mGroup.UserId == UserId)
            {
                Serilog.Log.Information($"SQL param: {GroupId}");
                users = _context.Database.SqlQuery<MUser>($@"
                        select mu.* 
                        from muser mu 
                        inner join mmember mm on mu.userid = mm.userid 
                        inner join mgroup mg on mm.groupid = mg.groupid
                        where mg.groupid = {GroupId}
                        and mu.status = 1 and mm.status = 1 and mg.status = 1
                        order by mu.userid").ToList();
            }
            else
            {
                users = _context.MUser.Where(u =>u.status == 1 && u.UserId == UserId).ToList();
            }

            List<MUserDisp> disps = new List<MUserDisp>();
            
            foreach(MUser user in users)
            {
                MUserDisp disp = new MUserDisp();
                disp.Id = user.Id;
                disp.UserName = user.UserName;
                disp.Email = user.Email;
                disp.StartDate = user.StartDate;
                disps.Add(disp);
            }
            return View(disps);
        }

        // GET: MUsers/Details/5
        public ActionResult Details(int? id)
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

            var mUser = _context.MUser.FirstOrDefault(u => u.Id == id && u.status == 1);
            if (mUser == null)
            {
                return NotFound();
            }
            MUserDisp mUserDisp = new MUserDisp();
            mUserDisp.Id = mUser.Id;
            mUserDisp.UserName = mUser.UserName;
            mUserDisp.Email = mUser.Email;
            mUserDisp.StartDate = mUser.StartDate;

            return View(mUserDisp);
        }

        // GET: MUsers/Create
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

            MUserDisp mUserDisp = new MUserDisp();
            mUserDisp.StartDate = DateTime.Now;
            return View(mUserDisp);
        }

        // POST: MUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("UserName,Password,PasswordAssert,Email")] MUserDisp mUserDisp)
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
                if (0 < ValidateLogic(mUserDisp))
                {
                    return View(mUserDisp);
                }

                // ユーザー登録
                DateTime dateTime = DateTime.Now;
                string currPg = "MUsersInsert";
                int newUserId = getNextUserId();

                // パスワードハッシュ対応有無で処理分岐
                IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
                string HashAndSalt = configuration["HashAndSalt"];
                if (HashAndSalt.Equals("use")) {
                    // パスワードをハッシュ化
                    byte[] salt = new byte[128 / 8];
                    using var rng = RandomNumberGenerator.Create();
                    rng.GetBytes(salt);
                    string hash = getHasshedPassword(mUserDisp.Password, salt);

                    MUser mUser = new MUser();
                    mUser.UserId = newUserId;
                    mUser.status = 1;
                    mUser.UserName = mUserDisp.UserName;
                    mUser.Email = mUserDisp.Email;
                    mUser.Password = hash;
                    mUser.StartDate = dateTime.Date;
                    mUser.CreatedDate = dateTime;
                    mUser.CreateUser = UserId.ToString();
                    mUser.CreatePg = currPg;
                    mUser.UpdatedDate = dateTime;
                    mUser.UpdateUser = UserId.ToString();
                    mUser.UpdatePg = currPg;
                    Serilog.Log.Information($"SQL param: MUser: {mUser.ToString()}");
                    _context.Add(mUser);

                    MSalt mSalt = new MSalt();
                    mSalt.status = 1;
                    mSalt.UserId = newUserId;
                    mSalt.salt = salt;
                    mSalt.CreatedDate = dateTime;
                    mSalt.CreateUser = UserId.ToString();
                    mSalt.CreatePg = currPg;
                    mSalt.UpdatedDate = dateTime;
                    mSalt.UpdateUser = UserId.ToString();
                    mSalt.UpdatePg = currPg;
                    Serilog.Log.Information($"SQL param: MSalt: {mSalt.ToString()}");
                    _context.Add(mSalt);
                }
                else
                {
                    // パスワードをハッシュ化しない
                    MUser mUser = new MUser();
                    mUser.UserId = newUserId;
                    mUser.status = 1;
                    mUser.UserName = mUserDisp.UserName;
                    mUser.Email = mUserDisp.Email;
                    mUser.Password = mUserDisp.Password;
                    mUser.StartDate = dateTime.Date;
                    mUser.CreatedDate = dateTime;
                    mUser.CreateUser = UserId.ToString();
                    mUser.CreatePg = currPg;
                    mUser.UpdatedDate = dateTime;
                    mUser.UpdateUser = UserId.ToString();
                    mUser.UpdatePg = currPg;
                    Serilog.Log.Information($"SQL param: MUser: {mUser.ToString()}");
                    _context.Add(mUser);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(mUserDisp);
        }

        // GET: MUsers/Edit/5
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

            // 指定ユーザーの表示情報を取得
            MUserDisp mUserDisp = DispLogic((int)id);
            if (mUserDisp == null)
            {
                return NotFound();
            }

            return View(mUserDisp);
        }

        // POST: MUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,UserName,Password,PasswordAssert,NewPassword,Email")] MUserDisp mUserDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id != mUserDisp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // 本人チェック
                if (0 < PersonCheck(mUserDisp))
                {
                    ViewBag.Message = "本人確認が取れません";
                    return View(mUserDisp);
                }

                // 業務入力チェック
                if (0 < ValidateLogic(mUserDisp))
                {
                    return View(mUserDisp);
                }
                // パスワードチェック
                if (0 < PassWordCheck(mUserDisp))
                {
                    return View(mUserDisp);
                }

                try
                {
                    DateTime currDate = DateTime.Now;
                    string currPg = "MUsersUpdate";

                    // パスワードハッシュ対応有無で処理分岐
                    IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
                    string HashAndSalt = configuration["HashAndSalt"];
                    string password = null;
                    if (HashAndSalt.Equals("use"))
                    {
                        // パスワードをハッシュ化
                        if (!mUserDisp.NewPassword.IsNullOrEmpty())
                        {
                            password = getHasshedPassword(mUserDisp.NewPassword, getSalt(mUserDisp.Id));
                        }
                    }
                    else
                    {
                        password = mUserDisp.Password;
                    }
                    MUser existingUser = _context.MUser.FirstOrDefault(u => u.Id == id && u.status == 1);
                    existingUser.UserName = mUserDisp.UserName;
                    existingUser.Password = password;
                    existingUser.Email = mUserDisp.Email;
                    existingUser.UpdatedDate = currDate;
                    existingUser.UpdateUser = UserId.ToString();
                    existingUser.UpdatePg = currPg;
                    Serilog.Log.Information($"SQL param: MUser: {existingUser.ToString()}");
                    _context.Update(existingUser);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View(mUserDisp);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mUserDisp);
        }

        // GET: MUsers/Delete/5
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

            // 指定ユーザーの表示情報を取得
            MUserDisp mUserDisp = DispLogic((int)id);
            if(mUserDisp == null)
            {
                return NotFound();
            }

            return View(mUserDisp);
        }

        // POST: MUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, [Bind("Id,Password,PasswordAssert")] MUserDisp mUserDisp)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            if (id != mUserDisp.Id)
            {
                return NotFound();
            }
            // 本人チェック
            if (0 < PersonCheck(mUserDisp))
            {
                ViewBag.Message = "本人確認が取れません";
                return View(mUserDisp);
            }
            
            // 業務入力チェック
            if (0 < ValidateLogic(mUserDisp))
            {
                return View(mUserDisp);
            }
            var mUser = _context.MUser.FirstOrDefault(u => u.Id == id && u.status == 1);
            if (mUser != null)
            {
                try
                {
                    string currPg = "MUsersDelete";
                    DateTime currTime = DateTime.Now;

                    mUser.status = (int)statusEnum.削除;
                    mUser.UpdatedDate = currTime;
                    mUser.UpdateUser = UserId.ToString();
                    mUser.UpdatePg = currPg;
                    Serilog.Log.Information($"SQL param: MUser: {mUser.ToString()}");
                    _context.MUser.Update(mUser);

                    // パスワードハッシュ対応有の場合、ソルトテーブルもステータス更新
                    IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
                    string HashAndSalt = configuration["HashAndSalt"];
                    if (HashAndSalt.Equals("use"))
                    {
                        MSalt mSalt = _context.MSalt.Where(s => s.UserId == mUser.UserId && s.status == 1).FirstOrDefault();
                        mSalt.status = (int)statusEnum.削除;
                        mSalt.UpdatedDate = currTime;
                        mSalt.UpdateUser = UserId.ToString();
                        mSalt.UpdatePg = currPg;
                        Serilog.Log.Information($"SQL param: MSalt: {mSalt.ToString()}");
                        _context.MSalt.Update(mSalt);
                    }

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    // todo パラメータなしのdelete画面では表示エラーになるのでは
                    return View();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private int getNextUserId()
        {
            int UserId = _context.MUser.Any() ? _context.MUser.Max(a => a.UserId) : -0;

            UserId++;

            return UserId;
        }

        private int ValidateLogic(MUserDisp mUserDisp)
        {
            int retInt = 0;
            if (!mUserDisp.Password.Equals(mUserDisp.PasswordAssert))
            {
                ModelState.AddModelError(nameof(MUserDisp.Password), "パスワードと確認用は一致させてください");
                retInt++;
            }
            string hasshedPassword = getHasshedPassword(mUserDisp.Password, getSalt(mUserDisp.Id));
            Boolean useChk = _context.MUser.Any(u => u.Email.Equals(mUserDisp.Email) && u.Password.Equals(hasshedPassword) && u.status == 1 && u.Id != mUserDisp.Id);
            if (useChk)
            {
                ModelState.AddModelError(nameof(MUserDisp.Email), "既に使用されているIDです");
                retInt++;
            }
            return retInt;
        }

        private int PassWordCheck(MUserDisp mUserDisp)
        {
            int retInt = 0;
            if(!mUserDisp.NewPassword.IsNullOrEmpty()) 
            {
                if (mUserDisp.NewPassword.Equals(mUserDisp.Password))
                {
                    ModelState.AddModelError(nameof(MUserDisp.NewPassword), "パスワードを変更する際は以前のものから変えてください");
                    retInt++;
                }
            }
            return retInt;
        }

        private MUserDisp DispLogic(int id)
        {
            MUser mUser = _context.MUser.FirstOrDefault(u => u.Id == id && u.status == 1);
            if (mUser == null)
            {
                return null;
            }
            MUserDisp mUserDisp = new MUserDisp();
            mUserDisp.Id = mUser.Id;
            mUserDisp.UserName = mUser.UserName;
            mUserDisp.Email = mUser.Email;
            mUserDisp.StartDate = mUser.StartDate;

            return mUserDisp;
        }

        private int PersonCheck(MUserDisp mUserDisp)
        {
            int retInt = 0;
            MUser user = _context.MUser.Where(u => u.Id == mUserDisp.Id && u.status == 1).FirstOrDefault();
            string password = getHasshedPassword(mUserDisp.Password, getSalt(user.Id));
            string passwordAssert = getHasshedPassword(mUserDisp.PasswordAssert, getSalt(user.Id));
            if (!user.Password.Equals(password) || !user.Password.Equals(passwordAssert))
            {
                retInt++;
            }
            return retInt;
        }

        private byte[] getSalt(int id)
        {
            byte[] salt = null;
            // パスワードハッシュ対応有無で処理分岐
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string HashAndSalt = configuration["HashAndSalt"];
            if (!HashAndSalt.Equals("use"))
            {
                return salt;
            }
            try
            {
                MUser mUser = _context.MUser.Where(u => u.Id == id).FirstOrDefault();
                MSalt mSalt = _context.MSalt.Where(s => s.UserId == mUser.UserId).FirstOrDefault();
                salt = mSalt.salt;
            }
            catch (Exception)
            {
                return salt;
            }

            return salt;
        }

        private string getHasshedPassword(string passwordStr, byte[] salt)
        {
            // パスワードハッシュ対応有無で処理分岐
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string HashAndSalt = configuration["HashAndSalt"];
            if (!HashAndSalt.Equals("use"))
            {
                return passwordStr;
            }
            if (salt == null)
            {
                return passwordStr;
            }
            byte[] hash = KeyDerivation.Pbkdf2(
              passwordStr,
              salt,
              prf: KeyDerivationPrf.HMACSHA256,
              iterationCount: 10000,  // 反復回数
              numBytesRequested: 256 / 8);
            return System.Text.Encoding.UTF8.GetString(hash);
        }

    }
}
