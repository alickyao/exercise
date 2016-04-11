using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 编辑扩展信息请求参数
    /// </summary>
    public class EditInfoExInfoRequestModel : InfoExInfoModel
    {
        /// <summary>
        /// 关联的ID
        /// </summary>
        public string fkId { get; set; }
        /// <summary>
        /// 创建/编辑操作者ID
        /// </summary>
        internal string modifiedBy { get; set; }
    }

    /// <summary>
    /// 扩展信息
    /// </summary>
    public class InfoExInfoModel
    {
        /// <summary>
        /// 扩展信息ID
        /// </summary>
        public string exInfoId { get; set; }
        /// <summary>
        /// 信息类型
        /// </summary>
        public CatInfoModel cat { get; set; }
        /// <summary>
        /// 信息详情
        /// </summary>
        public string detail { get; set; }
        /// <summary>
        /// 创建/编辑时间
        /// </summary>
        public DateTime modifiedOn { get; set; }
        /// <summary>
        /// 创建/编辑用户
        /// </summary>
        public SysUserModel modifyedBy { get; set; }
    }
}