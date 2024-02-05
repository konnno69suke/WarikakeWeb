using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using WarikakeWeb.Data;
using WarikakeWeb.Entities;
using WarikakeWeb.ViewModel;

namespace WarikakeWeb.Models
{
    public class UserModel
    {
        WarikakeWebContext _context;

        public UserModel(WarikakeWebContext context) 
        {
            _context = context;
        }

        // 指定したグループのユーザー一覧を取得
        public List<MUser> GetGroupUsers(int GroupId)
        {

            Serilog.Log.Information($"SQL param: {GroupId}");
            List<MUser> users = _context.Database.SqlQuery<MUser>($@"
                        select mu.* 
                        from muser mu 
                        inner join mmember mm on mu.userid = mm.userid 
                        inner join mgroup mg on mm.groupid = mg.groupid
                        where mg.groupid = {GroupId}
                        and mu.status = 1 and mm.status = 1 and mg.status = 1
                        order by mu.userid").ToList();
            return users;
        }

        // ユーザーIDで指定したユーザーを取得
        public MUser GetUserByUserId(int UserId)
        {
            MUser user = _context.MUser.Where(u => u.status == 1 && u.UserId == UserId).FirstOrDefault();
            return user;
        }

        // IDで指定されたユーザーを取得
        public MUser GetUserById(int Id)
        {
            MUser user = _context.MUser.Where(u => u.Id == Id && u.status == 1).FirstOrDefault();
            return user;
        }

        // ログインIDとパスワードでユーザーを取得
        public MUser GetUserByEmailPassWord(string eMail, string password)
        {
            MUser mUser =  _context.MUser.Where(u => u.Email == eMail && u.Password == password && u.status == 1).FirstOrDefault();
            return mUser;
        }

        // グループIDとユーザーIDで指定されたメンバーを取得
        public MMember GetMemberByGroupUser(int GroupId, int UserId)
        {
            MMember mMember = _context.MMember.Where(m => m.GroupId == GroupId && m.UserId == UserId).FirstOrDefault();
            return mMember;
        }

        // 新規登録処理
        public void CreateLogic(int GroupId, int UserId, MUserDisp mUserDisp)
        {
            // ユーザー登録
            DateTime dateTime = DateTime.Now;
            string currPg = "MUsersInsert";
            int newUserId = getNextUserId();

            // パスワードハッシュ対応有無で処理分岐
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string HashAndSalt = configuration["HashAndSalt"];
            if (HashAndSalt.Equals("use"))
            {
                // パスワードをハッシュ化
                byte[] salt = new byte[128 / 8];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
                UserModel model = new UserModel(_context);
                string hash = model.getHasshedPassword(mUserDisp.Password, salt);

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

            // メンバーにも登録
            MMember mMember = new MMember();
            mMember.status = 1;
            mMember.GroupId = GroupId;
            mMember.UserId = newUserId;
            mMember.CreatedDate = dateTime;
            mMember.CreateUser = UserId.ToString();
            mMember.CreatePg = currPg;
            mMember.UpdatedDate = dateTime;
            mMember.UpdateUser = UserId.ToString();
            mMember.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MMember:{mMember.ToString()}");
            _context.Add(mMember);
            _context.SaveChanges();

            _context.SaveChanges();
        }

        // 更新処理
        public void UpdateLogic(MUserDisp mUserDisp, int UserId, int Id)
        {
            DateTime currDate = DateTime.Now;
            string currPg = "MUsersUpdate";
            UserModel model = new UserModel(_context);
            string password;
            if (!mUserDisp.NewPassword.IsNullOrEmpty())
            {
                // パスワード変更の場合
                password = model.getHasshedPassword(mUserDisp.NewPassword, model.getSalt(mUserDisp.Id));
            }
            else
            {
                // パスワード変更なしの場合
                password = model.getHasshedPassword(mUserDisp.Password, model.getSalt(mUserDisp.Id));
            }

            MUser existingUser = GetUserById(Id);
            existingUser.UserName = mUserDisp.UserName;
            existingUser.Password = password;
            existingUser.Email = mUserDisp.Email;
            existingUser.UpdatedDate = currDate;
            existingUser.UpdateUser = UserId.ToString();
            existingUser.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MUser: {existingUser.ToString()}");
            _context.Update(existingUser);
        }
        // ステータス更新処理
        public void StatusChangeLogic(int GroupId, int UserId, int Id)
        {
            string currPg = "MUsersDelete";
            DateTime currTime = DateTime.Now;
            MUser currUser = GetUserById(Id);

            currUser.status = (int)statusEnum.削除;
            currUser.UpdatedDate = currTime;
            currUser.UpdateUser = UserId.ToString();
            currUser.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MUser: {currUser.ToString()}");
            _context.MUser.Update(currUser);

            // パスワードハッシュ対応有の場合、ソルトテーブルもステータス更新
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string HashAndSalt = configuration["HashAndSalt"];
            if (HashAndSalt.Equals("use"))
            {
                MSalt mSalt = _context.MSalt.Where(s => s.UserId == currUser.UserId && s.status == 1).FirstOrDefault();
                mSalt.status = (int)statusEnum.削除;
                mSalt.UpdatedDate = currTime;
                mSalt.UpdateUser = UserId.ToString();
                mSalt.UpdatePg = currPg;
                Serilog.Log.Information($"SQL param: MSalt: {mSalt.ToString()}");
                _context.MSalt.Update(mSalt);
            }

            // ユーザーの論理削除に伴いメンバー情報も更新
            MMember currMember = GetMemberByGroupUser(GroupId, currUser.UserId);

            currMember.status = (int)statusEnum.削除;
            currMember.UpdatedDate = currTime;
            currMember.UpdateUser = UserId.ToString();
            currMember.UpdatePg = currPg;
            Serilog.Log.Information($"SQL param: MUser: {currMember.ToString()}");
            _context.MMember.Update(currMember);
        }


        // ユーザーIDの発番
        private int getNextUserId()
        {
            int UserId = _context.MUser.Any() ? _context.MUser.Max(a => a.UserId) : -0;

            UserId++;

            return UserId;
        }


        // 一覧画面表示向けに編集
        public List<MUserDisp> GetMUserDispList(List<MUser> users)
        {
            List<MUserDisp> disps = new List<MUserDisp>();
            foreach (MUser user in users)
            {
                MUserDisp disp = new MUserDisp();
                disp.Id = user.Id;
                disp.UserName = user.UserName;
                disp.Email = user.Email;
                disp.StartDate = user.StartDate;
                disps.Add(disp);
            }
            return disps;
        }

        // 詳細画面表示向けに編集
        public MUserDisp GetMUserDisp(MUser user)
        {
            MUserDisp disp = new MUserDisp();
            disp.Id = user.Id;
            disp.UserName = user.UserName;
            disp.Email = user.Email;
            disp.StartDate = user.StartDate;

            return disp;
        }

        // ログイン画面向けに編集
        public HomeDisp GetHomeDisp(MUser mUser, int UserId)
        {
            HomeDisp homeDisp = new HomeDisp();
            homeDisp.UserId = (int)UserId;
            homeDisp.UserName = mUser.UserName;
            return homeDisp;
        }

        // ソルトを取得
        public byte[] getSalt(int id)
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

        // ハッシュ化されたパスワードを取得
        public string getHasshedPassword(string passwordStr, byte[] salt)
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
