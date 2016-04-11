using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 用户输入规范
    /// </summary>
    public class ResourceUserInputRuleModel
    {
        /// <summary>
        /// 规则ID
        /// </summary>
        public string RuleId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// 输入方式
        /// </summary>
        public string InputType { get; set; }
        /// <summary>
        /// 可选项，字符串
        /// </summary>
        public string OptionsString { get; set; }

        /// <summary>
        /// 排序依据，越大越靠前
        /// </summary>
        public Nullable<int> Sort { get; set; }

        /// <summary>
        /// 可选项，集合
        /// </summary>
        public List<string> OptionsList { get; set; }

        /// <summary>
        /// 新增/编辑时间
        /// </summary>
        public DateTime ModifiedOn { get; set; }
    }

    /// <summary>
    /// 获取输入规范集请求
    /// </summary>
    public class SearchResourceUserInputRuleRequestModel: RequestBase {
        /// <summary>
        /// 规范集ID，用,隔开 可为空
        /// </summary>
        public string Ids { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public EnumSortOrderType ordertype { get; set; }
    }

    /// <summary>
    /// 用户输入规范列表集
    /// </summary>
    public class SearchResourceUserInputRuleReplayModel {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public List<ResourceUserInputRuleModel> rows { get; set; }
    }

    /// <summary>
    /// 删除用户输入规范项请求
    /// </summary>
    public class DelResourceUserInputRulesReqeust {
        /// <summary>
        /// 需要删除的集合
        /// </summary>
        public List<ResourceUserInputRuleModel> rows { get; set; }
    }
}