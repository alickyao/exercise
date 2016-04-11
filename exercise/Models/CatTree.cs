using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 分类信息For列表
    /// </summary>
    public class CatInfoModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public Nullable<int> sort { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string _parentId { get; set; }

        /// <summary>
        /// 添加/编辑时间
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// 树打开状态
        /// </summary>
        protected string _state = "open";
        /// <summary>
        /// 树节点的打开状态默认为open
        /// </summary>
        public string state {
            get { return _state; }
            set { _state = value; }
        }
    }

    /// <summary>
    /// 分类树查询请求参数
    /// </summary>
    public class SearchCatInfoRequest {
        /// <summary>
        /// 查询深度-为空则不限制深度
        /// </summary>
        public Nullable<int> Dep { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string _parentId { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }
    }
}