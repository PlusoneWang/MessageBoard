namespace MessageBoard.Library.ViewModels.Users
{
    using System.ComponentModel.DataAnnotations;

    public class UserCreateVm
    {
        /// <summary>
        /// 電子郵件
        /// </summary>
        [Display(Name = "電子郵件")]
        [Required(ErrorMessage = "電子郵件為必填項")]
        [StringLength(200, ErrorMessage = "電子郵件不得大於 {1} 個字")]
        [EmailAddress(ErrorMessage = "請填寫正確的電子郵件")]
        public string Email { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [Display(Name = "使用者名稱")]
        [Required(ErrorMessage = "使用者名稱為必填項")]
        [RegularExpression("^.{4,50}$", ErrorMessage = "使用者名稱必須介於4~50字")]
        public string UserName { get; set; }

        /// <summary>
        /// 頭像的Inputbox值
        /// </summary>
        [Required(ErrorMessage = "請選擇頭像")]
        [DataType(DataType.Upload)]
        [MaxLength(1024,ErrorMessage = "最大1MB")]
        public string ImageFile { get; set; }

        /// <summary>
        /// 頭像檔案路徑
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Display(Name = "密碼")]
        [Required(ErrorMessage = "密碼為必填項")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{6,12}$", ErrorMessage = "密碼以英文字母與數字組成，必須同時含有大小寫字母及數字，長度介於6~12個字")]
        public string Password { get; set; }

        /// <summary>
        /// 確認密碼
        /// </summary>
        [Display(Name = "確認密碼")]
        [Required(ErrorMessage = "確認密碼為必填項")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "確認密碼與密碼不相符")]
        public string ConfirmPassword { get; set; }
    }
}
