using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 资源文档模型
    /// </summary>
    public class DocumentsResourceModel
    {
        /// <summary>
        /// 文档编号ID
        /// </summary>
        public string DocumentId { get; set; }
        /// <summary>
        /// 创建/编辑事件
        /// </summary>
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public Nullable<int> Sort { get; set; }
        /// <summary>
        /// 组
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 默认是否禁用为false
        /// </summary>
        protected bool _isdeiabled = false;

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled {
            get { return _isdeiabled; }
            set { _isdeiabled = value; }
        }
    }

    /// <summary>
    /// 查询文档资源请求
    /// </summary>
    public class SearchDocumentsResourceListRequestModel : RequestBase {
        /// <summary>
        /// 文档编号ID
        /// </summary>
        public string DocumentId { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        public List<string> GroupName { get; set; }
        /// <summary>
        /// 关键字 （文档标题或内容关键字）
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public EnumSortOrderType OrderType { get; set; }

        protected bool _getcontent = true;

        /// <summary>
        /// 获取详情内容 默认 true
        /// </summary>
        public bool GetContent {
            get { return _getcontent; }
            set { _getcontent = value; }
        }

        protected int _contentlength = 20;

        /// <summary>
        /// 当不获取文档详情内容时，内容文本截取的长度，默认20
        /// </summary>
        public int ContentLength {
            get { return _contentlength; }
            set { _contentlength = value; }
        }

        protected bool _getdisabled = false;
        /// <summary>
        /// 是否获取已禁用的内容 默认为false
        /// </summary>
        public bool GetDisabled {
            get { return _getdisabled; }
            set { _getdisabled = value; }
        }
    }
    

    /// <summary>
    /// 查询文档列表返回对象
    /// </summary>
    public class SearchDocumentsResourceListReplayModel {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public List<DocumentsResourceModel> rowlist { get; set; }
    }

    /// <summary>
    /// 文档批量及禁用排序请求
    /// </summary>
    public class SortDocumentsRequest {
        /// <summary>
        /// 排序信息（Sort和IsDisabled有效，其余无效）
        /// </summary>
        public List<DocumentsResourceModel> request { get; set; }
    }
}