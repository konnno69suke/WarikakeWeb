using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;
using static WarikakeWeb.Entities.TRequest;

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

            // 要承認情報をDB検索
            RequestModel rModel = new RequestModel(_context);
            List<RequestDisp> requestList = rModel.searchGroupRequest((int)GroupId, (int)UserId);
            if (requestList != null && requestList.Count > 0)
            {
                homeDisp.RequestFlg = true;
            }

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

            
            GroupModel gModel = new GroupModel(_context);
            List<MMember> members = gModel.GetMemberListByUserId(user.UserId);
            if (members == null || members.Count == 0)
            {
                // グループに属していない場合はグループ申請・グループ作成画面へ遷移する
                Serilog.Log.Information($"GroupId:notyet, UserId:{user.UserId}");
                return RedirectToAction("GroupRequest");
            }
            else if (members.Count == 1)
            {
                // 単一グループに属している場合はグループ選択画面での処理を先回りして行い、グループ選択画面を飛ばす
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

        public ActionResult GroupRequest()
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
            GroupRequestDisp groupRequestDisp = new GroupRequestDisp();

            return View(groupRequestDisp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupRequest(GroupRequestDisp groupRequestDisp)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId: notyet, UserId:{UserId}");

            // 一般入力チェック
            if (!ModelState.IsValid)
            {
                return View(groupRequestDisp);
            }

            // 新規作成か参加申請かで処理分岐
            if (groupRequestDisp.NewOrJoin.Equals("New")) 
            {
                // 入力チェック
                if (groupRequestDisp.NewGroupName.IsNullOrEmpty())
                {
                    ModelState.AddModelError(nameof(GroupRequestDisp.NewGroupName), "新規グループ名を入力してください");
                    return View(groupRequestDisp);
                }
                // 重複チェック
                if(0 < DupliCheck(groupRequestDisp, (int)UserId))
                {
                    return View(groupRequestDisp);
                }
                try
                {
                    // グループ作成
                    GroupModel gModel = new GroupModel(_context);
                    gModel.CreateLogic(groupRequestDisp.NewGroupName, (int)UserId);

                    //作成したグループのIDを取得しセッションにセット
                    MGroup group = gModel.GetGroupByGroupName(groupRequestDisp.NewGroupName, (int)UserId);
                    HttpContext.Session.SetInt32("GroupId", group.GroupId);
                    HttpContext.Session.SetString("GroupName", group.GroupName);

                    Serilog.Log.Information($"GroupId:{group.GroupId}, UserId:{UserId}");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex.Message, ex.StackTrace);
                    return View(groupRequestDisp);
                }
            }
            else
            {
                // 入力チェック
                int retInt = 0;
                if (groupRequestDisp.GroupName.IsNullOrEmpty())
                {
                    ModelState.AddModelError(nameof(GroupRequestDisp.GroupName), "参加グループ名を入力してください");
                    retInt++;
                }
                if (groupRequestDisp.TempKey.IsNullOrEmpty())
                {
                    ModelState.AddModelError(nameof(GroupRequestDisp.TempKey), "一時キーを入力してください");
                    retInt++;
                }
                if(retInt > 0)
                {
                    return View(groupRequestDisp);
                }

                // 申請チェック
                if (0 < RequestCheck(groupRequestDisp, (int)UserId))
                {
                    return View(groupRequestDisp);
                }
                // 登録処理
                try
                {
                    UserModel model = new UserModel(_context);
                    MUser mUser = model.GetUserByUserId((int)UserId);
                    MUserDisp mUserDisp = model.GetMUserDisp(mUser);
                    model.CreateLogic((int)groupRequestDisp.GroupId, (int)UserId, mUserDisp);

                    // グループをセット
                    HttpContext.Session.SetInt32("GroupId", (int)groupRequestDisp.GroupId);
                    HttpContext.Session.SetString("GroupName", groupRequestDisp.GroupName);

                    Serilog.Log.Information($"GroupId:{groupRequestDisp.GroupId}, UserId:{UserId}");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex.Message, ex.StackTrace);
                    return View(groupRequestDisp);
                }
            }
        }

        public ActionResult CreateUser()
        {
            Serilog.Log.Information($"GroupId: notyet, UserId: notyet");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(MUserDisp mUserDisp) 
        {
            Serilog.Log.Information($"GroupId: notyet, UserId:notyet");

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
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View(mUserDisp);
            }
        }

        public ActionResult PassRequest()
        {
            Serilog.Log.Information($"GroupId: notyet, UserId:notyet");

            string TempKey = (string)TempData["PassTempKey"];
            if (!TempKey.IsNullOrEmpty())
            {
                ViewBag.PassTempKey = TempKey;
            }

            PassRequestDisp passRequestDisp = new PassRequestDisp();
            return View(passRequestDisp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PassRequest(PassRequestDisp input)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            // 存在チェック兼マスタ取得
            int retInt = 0;
            GroupModel gModel = new GroupModel(_context);
            MGroup group = gModel.GetGroupByGroupName(input.GroupName);
            if (group == null)
            {
                retInt++;
            }
            UserModel uModel = new UserModel(_context);
            MUser user = uModel.GetUserByEmail(input.EMail);
            if (user == null)
            {
                retInt += 1;
            }
            if (retInt > 0)
            {
                ModelState.AddModelError(nameof(PassRequestDisp.GroupName), "グループかログインIDが見つかりません");
                return View();
            }
            bool isMember = gModel.chkGroupMember(group.GroupId, user.UserId);
            if (!isMember)
            {
                retInt += 1;
            }
            if (retInt > 0)
            {
                ModelState.AddModelError(nameof(PassRequestDisp.GroupName), "グループかログインIDが見つかりません");
                return View();
            }


            try
            {
                // 申請処理
                RequestModel rModel = new RequestModel(_context);
                string tempKey = rModel.CreateRequest(group.GroupId, user.UserId, group.UserId, (int)ReqTypeEnum.パスワード申請);
                TempData["PassTempKey"] = tempKey;
                
                return RedirectToAction("PassRequest");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View();
            }
        }

        public ActionResult TempKeyLogin()
        {
            Serilog.Log.Information($"GroupId: notyet, UserId:notyet");

            string NewPassword = (string)TempData["NewPassword"];
            if (!NewPassword.IsNullOrEmpty())
            {
                ViewBag.NewPassword = NewPassword;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TempKeyLogin(TempKeyDisp input)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                RequestModel model = new RequestModel(_context);
                MUserDisp mUserDisp = model.SetNewPassword(input.TempKey);
                if (mUserDisp == null) 
                {
                    return NotFound();
                }

                // DB更新し新パスワードを表示
                TempData["NewPassword"] = mUserDisp.NewPassword;

                return RedirectToAction("TempKeyLogin");

            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex.StackTrace);
                return View();
            }
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

        public int RequestCheck(GroupRequestDisp groupRequestDisp, int UserId)
        {
            int retInt = 0;

            GroupModel model = new GroupModel(_context);
            MGroup group = model.GetDupliGroupByGroupName(groupRequestDisp.GroupName, UserId);
            if (group == null)
            {
                ModelState.AddModelError(nameof(HomeDisp.GroupName), "グループ名が誤っています");
                retInt++;
            }
            else
            {
                groupRequestDisp.GroupId = group.GroupId;

                RequestModel rModel = new RequestModel(_context);
                TRequest request = rModel.GetGroupRequest(group.GroupId, (int)ReqTypeEnum.グループ申請, groupRequestDisp.TempKey);
                if (request == null)
                {
                    ModelState.AddModelError(nameof(HomeDisp.GroupName), "グループからの招待が確認できません");
                    retInt++;
                }
                else if (request.LimitDate < DateTime.Now)
                {
                    ModelState.AddModelError(nameof(HomeDisp.GroupName), "招待期限が切れています");
                    retInt++;
                }
            }

            return retInt;
        }

        public int DupliCheck(GroupRequestDisp groupRequestDisp, int UserId)
        {
            int retInt = 0;
            GroupModel model = new GroupModel(_context);
            MGroup group = model.GetDupliGroupByGroupName(groupRequestDisp.NewGroupName, UserId);
            if (group != null)
            {
                ModelState.AddModelError(nameof(GroupRequestDisp.NewGroupName), "既に使用されているグループ名です");
                retInt++;
            }
            return retInt;
        }

    }
}
