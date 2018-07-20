namespace MessageBoard.Library.ViewModels.Users
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 登入ViewModel
    /// </summary>
    public class LoginVm
    {
        /// <summary>
        /// 電子郵件
        /// </summary>
        [DisplayName("電子郵件")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [DisplayName("密碼")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 記住我?
        /// </summary>
        [DisplayName("記住我?")]
        public bool RememberMe { get; set; }
    }
}
