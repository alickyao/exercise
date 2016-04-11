using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 基础获取列表请求
    /// </summary>
    public class RequestBase {
        /// <summary>
        /// 默认第一页
        /// </summary>
        protected int _page = 1;
        /// <summary>
        /// 默认每页20行
        /// </summary>
        protected int _pagesize = 20;
        /// <summary>
        /// 页码，默认为第1页
        /// </summary>
        public int Page { get { return _page; } set { _page = value; } }
        /// <summary>
        /// 每页显示数量，默认为20行
        /// </summary>
        public int PageSize { get { return _pagesize; } set { _pagesize = value; } }
    }

    /// <summary>
    /// 基础获取列表返回对象
    /// </summary>
    public class ReplayBase {
        protected EnumErrorCode _returncode =  EnumErrorCode.Success;
        /// <summary>
        /// 返回状态 0 成功  1 无数据  2 系统错误
        /// </summary>
        public EnumErrorCode ReturnCode { get{return _returncode;} set{_returncode = value;} }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string ReturnMessage { get; set; }
    }

    /// <summary>
    /// 系统返回状态
    /// </summary>
    public enum EnumErrorCode{
        /// <summary>
        /// 表示成功
        /// </summary>
        Success,
        /// <summary>
        /// 验证错误或无数据
        /// </summary>
        EmptyDate,
        /// <summary>
        /// 服务器错误
        /// </summary>
        ServiceError
    }


    /// <summary>
    /// 标准树
    /// </summary>
    public class Tree
    {
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 显示的文本
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 树属性节点
        /// </summary>
        public TreeAttributes attributes { get; set; }

        /// <summary>
        /// 树打开状态
        /// </summary>
        protected string _state = "open";
        /// <summary>
        /// 树节点的打开状态默认为open
        /// </summary>
        public string state
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<Tree> children { get; set; }
    }
    /// <summary>
    /// 树的属性
    /// </summary>
    public class TreeAttributes
    {
        /// <summary>
        /// 排序依据
        /// </summary>
        public Nullable<int> Sort { get; set; }
    }

    /// <summary>
    /// 组合
    /// </summary>
    public class Combobox {
        /// <summary>
        /// 值
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 显示值
        /// </summary>
        public string text { get; set; }
    }

    /// <summary>
    /// 通用排序方式
    /// </summary>
    public enum EnumSortOrderType
    {
        按时间降序,
        按时间升序,
        按排序号升序,
        按排序号降序,
        按标题升序,
        按标题降序
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum EnumSex { 
        /// <summary>
        /// 用户还没有维护性别
        /// </summary>
        未知,
        先生,
        女士
    }


    /// <summary>
    /// 证件类型（可配合机票预订）
    /// </summary>
    public enum EnumUserIdType
    {
        /// <summary>
        /// 身份证
        /// </summary>
        IdCard,
        /// <summary>
        /// 护照
        /// </summary>
        PassProt,
        /// <summary>
        /// 军官证
        /// </summary>
        Officers,
        /// <summary>
        /// 士兵证
        /// </summary>
        soldier,
        /// <summary>
        /// 台胞证
        /// </summary>
        MTP,
        /// <summary>
        /// 其他
        /// </summary>
        Other
    }
}