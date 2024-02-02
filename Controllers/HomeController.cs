using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly WarikakeWebContext _context;

        public HomeController(WarikakeWebContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            string GroupName = HttpContext.Session.GetString("GroupName");
            if (UserId == null || GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            HomeDisp homeDisp = new HomeDisp();
            MUser user = _context.MUser.FirstOrDefault(u => u.UserId == UserId && u.status == 1);
            homeDisp.UserId = UserId;
            homeDisp.UserName = user.UserName;
            homeDisp.GroupId = GroupId;
            homeDisp.GroupName = GroupName;

            MGroup group = _context.MGroup.FirstOrDefault(g => g.GroupId == GroupId && g.status == 1);
            homeDisp.GroupUserId = group.UserId;

            return View(homeDisp);
        }

        public ActionResult Login()
        {
            Serilog.Log.Information($"GroupId: notyet, UserId: notyet");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login login)
        {
            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                // エラーメッセージを表示する
                ModelState.AddModelError("", "ユーザー名またはパスワードが間違っています。");
                return View();
            }


            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string HashAndSalt = configuration["HashAndSalt"];
            string password = null;
            if (HashAndSalt.Equals("use"))
            {
                // 存在チェック
                byte[] salt = null;
                try
                {
                    MUser mUser = _context.MUser.Where(u => u.Email == login.EMail).FirstOrDefault();
                    MSalt mSalt = _context.MSalt.Where(s => s.UserId == mUser.UserId).FirstOrDefault();
                    salt = mSalt.salt;
                }
                catch (Exception ex)
                {
                    // エラーメッセージを表示する
                    ModelState.AddModelError("", "ユーザー名またはパスワードが間違っています。");
                    return View();
                }
                password = getHasshedPassword(login.Password, salt);
            }
            else
            {
                password = login.Password;
            }
            MUser user = _context.MUser.Where(u => u.Email == login.EMail && u.Password == password && u.status == 1).FirstOrDefault();

            if (user == null)
            {
                // エラーメッセージを表示する
                ModelState.AddModelError("", "ユーザー名またはパスワードが間違っています。");
                return View();
            }

            // 認証処理を行う
            Claim[] claims ={new Claim(ClaimTypes.NameIdentifier, login.EMail), new Claim(ClaimTypes.Name, login.EMail)};
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    // Cookie をブラウザー セッション間で永続化するか？（ブラウザを閉じてもログアウトしないかどうか）
                    IsPersistent = true
                }); ;

            // セッションにユーザーIDを格納
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // 単一グループに属している場合はグループ選択画面での処理を先回りして行い、グループ選択画面を飛ばす
            List<MMember> members = _context.MMember.Where(m => m.UserId == user.UserId && m.status == 1).ToList();
            if (members.Count == 1)
            {
                foreach (MMember member in members)
                {
                    HttpContext.Session.SetInt32("GroupId", member.GroupId);
                    MGroup group = _context.MGroup.FirstOrDefault(m => m.GroupId == member.GroupId);
                    HttpContext.Session.SetString("GroupName", group.GroupName);

                    Serilog.Log.Information($"GroupId:{member.GroupId}, UserId:{user.UserId}");
                    return RedirectToAction("Index");
                }
            }
            // 複数グループに属している場合はグループ選択画面へ遷移する
            Serilog.Log.Information($"GroupId: notyet, UserId:{user.UserId}");
            return RedirectToAction("GroupSet");
        }

        public ActionResult GroupSet()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId: notyet, UserId:{UserId}");

            HomeDisp homeDisp = new HomeDisp();
            homeDisp.UserId = (int)UserId;
            MUser user = _context.MUser.FirstOrDefault(u => u.UserId == UserId);
            homeDisp.UserName = user.UserName;

            List<MGroup> groups = _context.Database.SqlQuery<MGroup>($@"
                    select mg.* 
                    from MGroup mg inner join MMember mm on mg.GroupId = mm.GroupId
                    where mm.UserId = {UserId}
                    and mg.status = 1 and mm.status = 1
                    order by mg.GroupId").ToList();
            ViewBag.Groups = new SelectList(groups.Select(g => new { Id = g.GroupId, Name = g.GroupName }), "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupSet(HomeDisp homeDisp)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId: notyet, UserId:{UserId}");

            if (ModelState.IsValid)
            {
                MGroup group = _context.MGroup.FirstOrDefault(g => g.GroupId == homeDisp.GroupId);
                if (group != null)
                {
                    HttpContext.Session.SetInt32("GroupId", group.GroupId);
                    HttpContext.Session.SetString("GroupName", group.GroupName);

                    Serilog.Log.Information($"GroupId:{group.GroupId}, UserId:{UserId}");
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("", "グループが取得できません");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            Serilog.Log.Information($"GroupId: logout, UserId: logout");

            // ログアウト処理
            HttpContext.SignOutAsync();

            return RedirectToAction("Login");
        }

        public ActionResult Denied()
        {

            return View();
        }

        private byte[] getSalt(int id)
        {
            byte[] salt = null;
            try
            {
                salt = _context.Database.SqlQuery<byte[]>(@$"
                        select ms.salt from msalt ms 
                        inner join muser mu on ms.userid = mu.userid
                        where ms.status = 1 and mu.status = 1 and mu.id = {id}").FirstOrDefault();
            }
            catch (SqlException)
            {
                return salt;
            }

            return salt;
        }
        private string getHasshedPassword(string passwordStr, byte[] salt)
        {
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
