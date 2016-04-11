using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 常旅客信息
    /// </summary>
    public class Traveller
    {
        /// <summary>
        /// id
        /// </summary>
        public string ravellerId { get; set; }
        /// <summary>
        /// 关联的用户表或其他表的ID
        /// </summary>
        public string fkId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public EnumUserIdType idType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string idCard { get; set; }
        /// <summary>
        /// 是否默认常旅客 true = 是 false= 否
        /// </summary>
        public bool isDefault { get; set; }
        /// <summary>
        /// 创建/修改时间
        /// </summary>
        public DateTime modifiedOn { get; set; }
    }
}