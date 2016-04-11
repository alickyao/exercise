using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 获取常用旅客或者常用地址请求
    /// </summary>
    public class GetUserExInfoListRequest {
        /// <summary>
        /// 常用地址或者旅客的ID 可为空
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 关联的用户或其他ID 可为空
        /// </summary>
        public string fkId { get; set; }
        /// <summary>
        /// 是否只获取默认
        /// </summary>
        public bool onlyDefalut { get; set; }
    }

    /// <summary>
    /// 常用地址
    /// </summary>
    public class AddressInfoModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 关联的用户或其他ID
        /// </summary>
        public string fkId { get; set; }
        /// <summary>
        /// 城市/行政区信息（所在区域信息）
        /// </summary>
        public LocationInfoModel locationInfo { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string postCode { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string linkMan { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string linkPhone { get; set; }
        /// <summary>
        /// 是否默认地址
        /// </summary>
        public bool isDefault { get; set; }
        /// <summary>
        /// 创建/编辑时间
        /// </summary>
        public DateTime modifiedOn { get; set; }
    }
}