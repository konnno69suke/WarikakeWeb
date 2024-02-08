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
                // �Z�b�V�����؂�
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // �\������DB����
            UserModel model = new UserModel(_context);
            MUser user = model.GetUserByUserId((int)UserId);
            GroupModel gModel = new GroupModel(_context);
            MGroup group = gModel.GetGroupByGroupId((int)GroupId);

            // ��ʕ\�������ɕҏW
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
            // ��ʓ��̓`�F�b�N
            if (!ModelState.IsValid)
            {
                // �G���[���b�Z�[�W��\������
                ModelState.AddModelError("", "���[�U�[���܂��̓p�X���[�h���Ԉ���Ă��܂��B");
                return View();
            }


            // �p�X���[�h�̃n�b�V�����ƃ\���g�g�p
            UserModel model = new UserModel(_context);
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string HashAndSalt = configuration["HashAndSalt"];
            string password = null;
            if (HashAndSalt.Equals("use"))
            {
                // ���݃`�F�b�N
                byte[] salt = null;
                try
                {
                    MUser mUser = _context.MUser.Where(u => u.Email == login.EMail).FirstOrDefault();
                    MSalt mSalt = _context.MSalt.Where(s => s.UserId == mUser.UserId).FirstOrDefault();
                    salt = mSalt.salt;
                }
                catch (Exception ex)
                {
                    // �G���[���b�Z�[�W��\������
                    ModelState.AddModelError("", "���[�U�[���܂��̓p�X���[�h���Ԉ���Ă��܂��B");
                    return View();
                }
                password = model.getHasshedPassword(login.Password, salt);
            }
            else
            {
                // �p�X���[�h�n�b�V�����s��Ȃ��ꍇ
                password = login.Password;
            }
            MUser user = model.GetUserByEmailPassWord(login.EMail, password);

            if (user == null)
            {
                // �G���[���b�Z�[�W��\������
                ModelState.AddModelError("", "���[�U�[���܂��̓p�X���[�h���Ԉ���Ă��܂��B");
                return View();
            }

            // �F�؏������s��
            Claim[] claims ={new Claim(ClaimTypes.NameIdentifier, login.EMail), new Claim(ClaimTypes.Name, login.EMail)};
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    // Cookie ���u���E�U�[ �Z�b�V�����Ԃŉi�������邩�H�i�u���E�U����Ă����O�A�E�g���Ȃ����ǂ����j
                    IsPersistent = true
                }); ;

            // �Z�b�V�����Ƀ��[�U�[ID���i�[
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // �P��O���[�v�ɑ����Ă���ꍇ�̓O���[�v�I����ʂł̏�������肵�čs���A�O���[�v�I����ʂ��΂�
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
            // �����O���[�v�ɑ����Ă���ꍇ�̓O���[�v�I����ʂ֑J�ڂ���
            Serilog.Log.Information($"GroupId: notyet, UserId:{user.UserId}");
            return RedirectToAction("GroupSet");
        }

        public ActionResult GroupSet()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                // �Z�b�V�����؂�
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId: notyet, UserId:{UserId}");

            // DB����
            UserModel model = new UserModel(_context);
            MUser mUser = model.GetUserByUserId((int)UserId);
            // ��ʕ\�������ɕҏW
            HomeDisp homeDisp = model.GetHomeDisp(mUser, (int)UserId);

            // �O���[�v�̃h���b�v�_�E�����擾
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
                // �Z�b�V�����؂�
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId: notyet, UserId:{UserId}");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "�O���[�v���擾�ł��܂���");
                return View();
            }

            GroupModel model = new GroupModel(_context);
            MGroup group = model.GetGroupByGroupId((int)homeDisp.GroupId);
            
            if (group == null)
            {
                return NotFound();
            }

            // �O���[�v���Z�b�g
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

            // ��ʓ��̓`�F�b�N
            if (!ModelState.IsValid)
            {
                return View(mUserDisp);
            }
            // �Ɩ����̓`�F�b�N
            if (0 < ValidateLogic(mUserDisp))
            {
                return View(mUserDisp);
            }
            try
            {
                // �o�^����
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

            // ���O�A�E�g����
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
                ModelState.AddModelError(nameof(MUserDisp.Password), "�p�X���[�h�Ɗm�F�p�͈�v�����Ă�������");
                retInt++;
            }
            UserModel model = new UserModel(_context);
            Boolean useChk = _context.MUser.Any(u => u.Email.Equals(mUserDisp.Email) && u.status == 1);
            if (useChk)
            {
                ModelState.AddModelError(nameof(MUserDisp.Email), "���Ɏg�p����Ă���ID�ł�");
                retInt++;
            }
            return retInt;
        }
    }
}
