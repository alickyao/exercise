using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 用户登录请求
    /// </summary>
    public class RequestUserLogoModel {
        /// <summary>
        /// 登录名（用户名）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string UserPassWord { get; set; }
    }

    /// <summary>
    /// 修改用户密码请求
    /// </summary>
    public class RequestChangeUserPwdModel
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        internal string UserId { get; set; }
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
    }
}