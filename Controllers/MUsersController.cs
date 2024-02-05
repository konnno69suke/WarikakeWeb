using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;

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

            // 検索処理
            UserModel model = new UserModel(_context);
            MGroup mGroup = _context.MGroup.Where(g => g.GroupId == GroupId && g.status == 1).FirstOrDefault();
            List<MUser> users = new List<MUser>();
            if (mGroup.UserId == UserId)
            {
                // リーダーの場合
                users = model.GetGroupUsers((int)GroupId);
            }
            else
            {
                // メンバーの場合
                users.Add(model.GetUserByUserId((int)UserId));
            }

            // 画面表示処理
            List<MUserDisp> disps = model.GetMUserDispList(users);

            return View(disps);
        }

        // GET: MUsers/Details/5
        public ActionResult Details(int id)
        {
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login", "Home");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 検索処理
            UserModel model = new UserModel(_context);
            MUser mUser = model.GetUserById(id);
            if (mUser == null)
            {
                return NotFound();
            }

            // 画面表示処理
            MUserDisp mUserDisp = model.GetMUserDisp(mUser);

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

            // 画面表示処理
            MUserDisp mUserDisp = new MUserDisp();
            mUserDisp.StartDate = DateTime.Now;
            return View(mUserDisp);
        }

        // POST: MUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MUserDisp mUserDisp)
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
                model.CreateLogic((int)GroupId, (int)UserId, mUserDisp);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(mUserDisp);
            }
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

            // DB検索
            UserModel model = new UserModel(_context);
            MUser mUser = model.GetUserById((int)id);
            if (mUser == null)
            {
                return NotFound();
            }

            // 画面表示処理
            MUserDisp mUserDisp = model.GetMUserDisp(mUser);
            return View(mUserDisp);
        }

        // POST: MUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, MUserDisp mUserDisp)
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
                return View(mUserDisp);
            }
            // 本人チェック
            if (0 < PersonCheck(mUserDisp))
            {
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
                // 更新処理
                UserModel model = new UserModel(_context);
                model.UpdateLogic(mUserDisp, (int)UserId, (int)id);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return View(mUserDisp);
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: MUsers/Delete/5
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

            // DB検索
            UserModel model = new UserModel(_context);
            MUser mUser = model.GetUserById((int)Id);
            if (mUser == null)
            {
                return null;
            }
            // 画面表示処理
            MUserDisp mUserDisp = model.GetMUserDisp(mUser);

            return View(mUserDisp);
        }

        // POST: MUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? Id, MUserDisp mUserDisp)
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

            // 本人チェック
            if (0 < PersonCheck(mUserDisp))
            {
                return View(mUserDisp);
            }
            // 業務入力チェック
            if (0 < ValidateLogic(mUserDisp))
            {
                return View(mUserDisp);
            }
            // 未精算チェック
            if (0 < UnSettledCheck((int)GroupId, mUserDisp))
            {
                return View(mUserDisp);
            }


            try
            {
                // 更新処理
                UserModel model = new UserModel(_context);
                model.StatusChangeLogic((int)GroupId, (int)UserId, (int)Id);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // todo パラメータなしのdelete画面では表示エラーになるのでは
                return View();
            }

            return RedirectToAction(nameof(Index));
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
            string hasshedPassword = model.getHasshedPassword(mUserDisp.Password, model.getSalt(mUserDisp.Id));
            Boolean useChk = _context.MUser.Any(u => u.Email.Equals(mUserDisp.Email) && u.Password.Equals(hasshedPassword) && u.status == 1 && u.Id != mUserDisp.Id);
            if (useChk)
            {
                ModelState.AddModelError(nameof(MUserDisp.Email), "既に使用されているIDです");
                retInt++;
            }
            return retInt;
        }

        public int PassWordCheck(MUserDisp mUserDisp)
        {
            int retInt = 0;
            if(!mUserDisp.NewPassword.IsNullOrEmpty()) 
            {
                if (mUserDisp.NewPassword.Equals(mUserDisp.Password))
                {
                    ModelState.AddModelError(nameof(MUserDisp.NewPassword), "パスワード変更は以前のパスワードから変えてください");
                    retInt++;
                }
            }
            return retInt;
        }

        public int PersonCheck(MUserDisp mUserDisp)
        {
            int retInt = 0;

            UserModel model = new UserModel(_context);
            MUser currentUser = model.GetUserById(mUserDisp.Id);
            string password = model.getHasshedPassword(mUserDisp.Password, model.getSalt(currentUser.Id));
            string passwordAssert = model.getHasshedPassword(mUserDisp.PasswordAssert, model.getSalt(currentUser.Id));
            if (!currentUser.Password.Equals(password) || !currentUser.Password.Equals(passwordAssert))
            {
                ModelState.AddModelError(nameof(MUserDisp.Password), "本人確認が取れません");
                retInt++;
            }
            return retInt;
        }

        public int UnSettledCheck(int GroupId, MUserDisp mUserDisp)
        {
            int retInt = 0;

            UserModel model = new UserModel(_context);
            MUser mUser = model.GetUserById(mUserDisp.Id);
            WarikakeModel wModel = new WarikakeModel(_context);
            int cCnt = wModel.GetUserUnSettledCostCount(GroupId, mUser.UserId);
            if(cCnt > 0)
            {
                ModelState.AddModelError(nameof(MUserDisp.Password), "未精算の支払が残っています");
                retInt++;
            }
            return retInt;
        }
    }
}
