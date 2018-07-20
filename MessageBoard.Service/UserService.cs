namespace MessageBoard.Service
{
    using System;
    using System.Linq;

    using MessageBoard.Library.Models;
    using MessageBoard.Library.ViewModels.Users;

    using Po.Helper;
    using Po.Result;

    public class UserService : BaseService
    {
        /// <summary>
        /// 使用信箱，取得使用者
        /// </summary>
        /// <param name="email">使用者的信箱</param>
        /// <returns>取得結果</returns>
        public PoResult<User> GetUserByEmail(string email)
        {
            try
            {
                var user = this.Database.Users.FirstOrDefault(o => o.Email == email);
                return user == null ? PoResult<User>.DbNotFound() : PoResult<User>.PoSuccess(user);
            }
            catch (Exception e)
            {
                return PoResult<User>.Exception(e);
            }
        }

        /// <summary>
        /// 使用使用者模型物件，新增使用者
        /// </summary>
        /// <param name="userModel">使用者模型物件</param>
        /// <returns>新增結果</returns>
        public PoResult CreateUser(UserCreateVm userModel)
        {
            try
            {
                var isEmailExit = this.Database.Users.Any(o => o.Email == userModel.Email);
                if (isEmailExit)
                    return PoResult.Fail("電子郵件重複");
                var isUserNameExit = this.Database.Users.Any(o => o.UserName == userModel.UserName);
                if (isUserNameExit)
                    return PoResult.Fail("使用者名稱重複重複");
                this.Database.Users.Add(new User
                {
                    Id = Ci.Sequential.Guid.Create(),
                    Email = userModel.Email,
                    UserName = userModel.UserName,
                    Password = PasswordHash(userModel.Password),
                    CreateTime = DateTime.Now,
                    ImageBase64 = userModel.ImageBase64
                });
                this.Database.SaveChanges();
                return PoResult.PoSuccess();
            }
            catch (Exception e)
            {
                return PoResult.Exception(e);
            }
        }

        /// <summary>
        /// 使用信箱、密碼，驗證使用者
        /// </summary>
        /// <param name="email">信箱</param>
        /// <param name="password">密碼</param>
        /// <returns>驗證結果</returns>
        public PoResult VerifyUser(string email, string password)
        {
            try
            {
                var userResult = this.GetUserByEmail(email);
                if (!userResult.Success)
                    return PoResult.Fail("信箱不存在");
                return userResult.Data.Password == PasswordHash(password) ? PoResult.PoSuccess() : PoResult.Fail("密碼錯誤");
            }
            catch (Exception e)
            {
                return PoResult.Exception(e);
            }
        }

        /// <summary>
        /// 使用原始密碼，產生其雜湊值
        /// </summary>
        /// <param name="originalPassword">原始密碼</param>
        /// <returns>雜湊結果</returns>
        private static string PasswordHash(string originalPassword) => originalPassword.ToSha256("7CC5B9E1-1209-49CA-96EB-1A519B0C1FD7");
    }
}
