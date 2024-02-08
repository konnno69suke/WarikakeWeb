using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

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
            if (UserId == null || GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 表示情報をDB検索
            UserModel model = new UserModel(_context);
            MUser user = model.GetUserByUserId((int)UserId);
            GroupModel gModel = new GroupModel(_context);
            MGroup group = gModel.GetGroupByGroupId((int)GroupId);

            // 画面表示向けに編集
            HomeDisp homeDisp = gModel.GetHomeDisp(user, group);

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


            // パスワードのハッシュ化とソルト使用
            UserModel model = new UserModel(_context);
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
                password = model.getHasshedPassword(login.Password, salt);
            }
            else
            {
                // パスワードハッシュを行わない場合
                password = login.Password;
            }
            MUser user = model.GetUserByEmailPassWord(login.EMail, password);

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
            GroupModel gModel = new GroupModel(_context);
            List<MMember> members = gModel.GetMemberListByUserId(user.UserId);
            if (members.Count == 1)
            {
                foreach (MMember member in members)
                {
                    HttpContext.Session.SetInt32("GroupId", member.GroupId);
                    
                    MGroup group = gModel.GetGroupByGroupId(member.GroupId);
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

            // DB検索
            UserModel model = new UserModel(_context);
            MUser mUser = model.GetUserByUserId((int)UserId);
            // 画面表示向けに編集
            HomeDisp homeDisp = model.GetHomeDisp(mUser, (int)UserId);

            // グループのドロップダウンを取得
            GroupModel gModel = new GroupModel(_context);
            SelectList groupSelect = gModel.GetGroupSelect((int) UserId);

            ViewBag.Groups = groupSelect;

            return View(homeDisp);
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

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "グループが取得できません");
                return View();
            }

            GroupModel model = new GroupModel(_context);
            MGroup group = model.GetGroupByGroupId((int)homeDisp.GroupId);
            
            if (group == null)
            {
                return NotFound();
            }

            // グループをセット
            HttpContext.Session.SetInt32("GroupId", group.GroupId);
            HttpContext.Session.SetString("GroupName", group.GroupName);

            Serilog.Log.Information($"GroupId:{group.GroupId}, UserId:{UserId}");
            return RedirectToAction("Index");
        }

        public ActionResult CreateUser()
        {
            Serilog.Log.Information($"GroupId: notyet, UserId: notyet");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(MUserDisp mUserDisp) {

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(mUserDisp);
            }
            // 業務入力チェック
            if (0 < ValidateLogic(mUserDisp))
            {
                return View(mUserDisp);
            }
            try
            {
                // 登録処理
                UserModel model = new UserModel(_context);
                model.CreateLogic(mUserDisp);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(mUserDisp);
            }

            return RedirectToAction("Login");
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


        public int ValidateLogic(MUserDisp mUserDisp)
        {
            int retInt = 0;
            if (!mUserDisp.Password.Equals(mUserDisp.PasswordAssert))
            {
                ModelState.AddModelError(nameof(MUserDisp.Password), "パスワードと確認用は一致させてください");
                retInt++;
            }
            UserModel model = new UserModel(_context);
            Boolean useChk = _context.MUser.Any(u => u.Email.Equals(mUserDisp.Email) && u.status == 1);
            if (useChk)
            {
                ModelState.AddModelError(nameof(MUserDisp.Email), "既に使用されているIDです");
                retInt++;
            }
            return retInt;
        }
    }
}
