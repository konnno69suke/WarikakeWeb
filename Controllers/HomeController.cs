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

            // �v���F����DB����
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

            
            GroupModel gModel = new GroupModel(_context);
            List<MMember> members = gModel.GetMemberListByUserId(user.UserId);
            if (members == null || members.Count == 0)
            {
                // �O���[�v�ɑ����Ă��Ȃ��ꍇ�̓O���[�v�\���E�O���[�v�쐬��ʂ֑J�ڂ���
                Serilog.Log.Information($"GroupId:notyet, UserId:{user.UserId}");
                return RedirectToAction("GroupRequest");
            }
            else if (members.Count == 1)
            {
                // �P��O���[�v�ɑ����Ă���ꍇ�̓O���[�v�I����ʂł̏�������肵�čs���A�O���[�v�I����ʂ��΂�
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

        public ActionResult GroupRequest()
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
                // �Z�b�V�����؂�
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId: notyet, UserId:{UserId}");

            // ��ʓ��̓`�F�b�N
            if (!ModelState.IsValid)
            {
                return View(groupRequestDisp);
            }

            // �V�K�쐬���Q���\�����ŏ�������
            if (groupRequestDisp.NewOrJoin.Equals("New")) 
            {
                // ���̓`�F�b�N
                if (groupRequestDisp.NewGroupName.IsNullOrEmpty())
                {
                    ModelState.AddModelError(nameof(GroupRequestDisp.NewGroupName), "�V�K�O���[�v������͂��Ă�������");
                    return View(groupRequestDisp);
                }
                // �d���`�F�b�N
                if(0 < DupliCheck(groupRequestDisp, (int)UserId))
                {
                    return View(groupRequestDisp);
                }
                try
                {
                    // �O���[�v�쐬
                    GroupModel gModel = new GroupModel(_context);
                    gModel.CreateLogic(groupRequestDisp.NewGroupName, (int)UserId);

                    //�쐬�����O���[�v��ID���擾���Z�b�V�����ɃZ�b�g
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
                // ���̓`�F�b�N
                int retInt = 0;
                if (groupRequestDisp.GroupName.IsNullOrEmpty())
                {
                    ModelState.AddModelError(nameof(GroupRequestDisp.GroupName), "�Q���O���[�v������͂��Ă�������");
                    retInt++;
                }
                if (groupRequestDisp.TempKey.IsNullOrEmpty())
                {
                    ModelState.AddModelError(nameof(GroupRequestDisp.TempKey), "�ꎞ�L�[����͂��Ă�������");
                    retInt++;
                }
                if(retInt > 0)
                {
                    return View(groupRequestDisp);
                }

                // �\���`�F�b�N
                if (0 < RequestCheck(groupRequestDisp, (int)UserId))
                {
                    return View(groupRequestDisp);
                }
                // �o�^����
                try
                {
                    UserModel model = new UserModel(_context);
                    MUser mUser = model.GetUserByUserId((int)UserId);
                    MUserDisp mUserDisp = model.GetMUserDisp(mUser);
                    model.CreateLogic((int)groupRequestDisp.GroupId, (int)UserId, mUserDisp);

                    // �O���[�v���Z�b�g
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

            // ���݃`�F�b�N���}�X�^�擾
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
                ModelState.AddModelError(nameof(PassRequestDisp.GroupName), "�O���[�v�����O�C��ID��������܂���");
                return View();
            }
            bool isMember = gModel.chkGroupMember(group.GroupId, user.UserId);
            if (!isMember)
            {
                retInt += 1;
            }
            if (retInt > 0)
            {
                ModelState.AddModelError(nameof(PassRequestDisp.GroupName), "�O���[�v�����O�C��ID��������܂���");
                return View();
            }


            try
            {
                // �\������
                RequestModel rModel = new RequestModel(_context);
                string tempKey = rModel.CreateRequest(group.GroupId, user.UserId, group.UserId, (int)ReqTypeEnum.�p�X���[�h�\��);
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

                // DB�X�V���V�p�X���[�h��\��
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

        public int RequestCheck(GroupRequestDisp groupRequestDisp, int UserId)
        {
            int retInt = 0;

            GroupModel model = new GroupModel(_context);
            MGroup group = model.GetDupliGroupByGroupName(groupRequestDisp.GroupName, UserId);
            if (group == null)
            {
                ModelState.AddModelError(nameof(HomeDisp.GroupName), "�O���[�v��������Ă��܂�");
                retInt++;
            }
            else
            {
                groupRequestDisp.GroupId = group.GroupId;

                RequestModel rModel = new RequestModel(_context);
                TRequest request = rModel.GetGroupRequest(group.GroupId, (int)ReqTypeEnum.�O���[�v�\��, groupRequestDisp.TempKey);
                if (request == null)
                {
                    ModelState.AddModelError(nameof(HomeDisp.GroupName), "�O���[�v����̏��҂��m�F�ł��܂���");
                    retInt++;
                }
                else if (request.LimitDate < DateTime.Now)
                {
                    ModelState.AddModelError(nameof(HomeDisp.GroupName), "���Ҋ������؂�Ă��܂�");
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
                ModelState.AddModelError(nameof(GroupRequestDisp.NewGroupName), "���Ɏg�p����Ă���O���[�v���ł�");
                retInt++;
            }
            return retInt;
        }

    }
}
