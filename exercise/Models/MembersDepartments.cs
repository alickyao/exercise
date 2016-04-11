using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 创建部门请求
    /// </summary>
    public class CreateMembersDepartmentReqeustModel
    {
        /// <summary>
        /// 操作者
        /// </summary>
        internal string createdby { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 父级部门的ID（可为空，为空则创建一个顶级组织信息）
        /// </summary>
        public string pid { get; set; }
    }

    /// <summary>
    /// 部门检索基础项
    /// </summary>
    public class SearchMemberDepartmentBaseRequest {
        /// <summary>
        /// 部门的ID
        /// </summary>
        public string departmentId { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public EnumSortOrderType ordertype { get; set; }

        /// <summary>
        /// 部门的名称（关键字）
        /// </summary>
        public string caption { get; set; }

        /// <summary>
        /// 默认不显示已经被禁用的条目
        /// </summary>
        protected bool _showdisabled = false;

        /// <summary>
        /// 是否显示被禁用的条目,设置为true时被禁用的条目也会显示出来,默认为false
        /// </summary>
        public bool showdisabled
        {
            get { return _showdisabled; }
            set { _showdisabled = value; }
        }

        /// <summary>
        /// 默认不显示前端显示设置为false的条目
        /// </summary>
        protected bool _showontheui = false;

        /// <summary>
        /// 是否只显示设置为前端显示的条目，设置为true时，被设置为不在前端显示的条目不会被显示出来，默认为false
        /// </summary>
        public bool showontheui
        {
            get { return _showontheui; }
            set { _showontheui = value; }
        }
    }

    /// <summary>
    /// 检索部门树信息请求参数
    /// </summary>
    public class SearchMemberDepartmentRequst : SearchMemberDepartmentBaseRequest
    {
        /// <summary>
        /// 父级的ID
        /// </summary>
        public string _parentId { get; set; }
        /// <summary>
        /// 默认加子集
        /// </summary>
        protected bool _getchild = false;
        /// <summary>
        /// 是否加载子集，默认为false
        /// </summary>
        public bool getchild
        {
            get { return _getchild; }
            set { _getchild = value; }
        }
    }
    /// <summary>
    /// 检索根组织请求
    /// </summary>
    public class SearchMemberRootDepartmentRequest : SearchMemberDepartmentBaseRequest
    {
        
        /// <summary>
        /// 页码信息
        /// </summary>
        public RequestBase page { get; set; }
    }

    /// <summary>
    /// 会员顶级组织查询返回对象
    /// </summary>
    public class SearchMemberRootDepartMentReplay {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public List<MemberDepartmentInfoModel> rows { get; set; }
    }

    /// <summary>
    /// 检索部门/组织返回对象
    /// </summary>
    public class SearchMemberDepartmentReplay {
        /// <summary>
        /// 行
        /// </summary>
        public List<MemberDepartmentBaseInfoModel> rows { get; set; }
    }

    /// <summary>
    /// 编辑组织请求
    /// </summary>
    public class EditOrgInfoRequestModel {
        /// <summary>
        /// 组织ID
        /// </summary>
        public string depmentid { get; set; }
        /// <summary>
        /// 组织名称
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool isdisabled { get; set; }
        /// <summary>
        /// 是否显示在前端网站或APP中显示该企业
        /// </summary>
        public bool isshow { get; set; }
        /// <summary>
        /// 所有者用户ID
        /// </summary>
        public string linkUserId { get; set; }
        /// <summary>
        /// 编辑者
        /// </summary>
        internal string modifiedBy { get; set; }
    }

    /// <summary>
    /// 组织信息
    /// </summary>
    public class OrgInfoModel {
        /// <summary>
        /// 基础信息
        /// </summary>
        public MemberDepartmentInfoModel baseInfo { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public List<InfoExInfoModel> exList { get; set; }
    }

    /// <summary>
    /// 会员组织信息
    /// </summary>
    public class MemberDepartmentInfoModel : MemberDepartmentBaseInfoModel {
        /// <summary>
        /// 部门所有者
        /// </summary>
        public SysUserModel linkUserinfo { get; set; }
    }

    /// <summary>
    /// 获取部门详情请求
    /// </summary>
    public class GetDepInfoRequestModel {
        /// <summary>
        /// 部门ID（必填）
        /// </summary>
        public string depmentid { get; set; }
    }

    /// <summary>
    /// 部门详情
    /// </summary>
    public class DepInfoModel {
        /// <summary>
        /// 部门基础信息
        /// </summary>
        public  MemberDepartmentBaseInfoModel baseinfo { get; set; }
        /// <summary>
        /// 部门的子集
        /// </summary>
        public List<MemberDepartmentBaseInfoModel> children { get; set; }
        /// <summary>
        /// 所属组织基础信息
        /// </summary>
        public MemberDepartmentInfoModel orginfo { get; set; }

    }

    /// <summary>
    /// 编辑部门请求参数
    /// </summary>
    public class EditDepInfoRequestModel {

        /// <summary>
        /// 部门ID
        /// </summary>
        public string depmentid { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool isdisabled { get; set; }

        /// <summary>
        /// 创建或编辑的用户ID
        /// </summary>
        internal string modifiedby { get; set; }
    }

    /// <summary>
    /// 部门基础数据
    /// </summary>
    public class MemberDepartmentBaseInfoModel {
        /// <summary>
        /// 部门ID
        /// </summary>
        public string depmentid { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string caption { get; set; }

        /// <summary>
        /// 部门下的用户数量
        /// </summary>
        public int memberNum { get; set; }
        /// <summary>
        /// 组织/部门下的用户总计（包含子部门数量统计）
        /// </summary>
        public int memberNumCount { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string _parentId { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool isdisabled { get; set; }
        /// <summary>
        /// 是否显示在前端网站或APP中显示该企业（作为组织（根节点）时该值有效）
        /// </summary>
        public bool isshow { get; set; }
        /// <summary>
        /// 创建/编辑时间
        /// </summary>
        public DateTime modifiedon { get; set; }
        /// <summary>
        /// 创建或编辑的用户ID
        /// </summary>
        internal string modifiedby { get; set; }
        /// <summary>
        /// 当为根节点时，部门的所有者ID
        /// </summary>
        internal string linkuserid { get; set; }
        /// <summary>
        /// 是否在树中默认显示为展开,默认设置为open
        /// </summary>
        protected string _state = "open";
        /// <summary>
        /// 是否在树中默认显示为展开 open 为展开 close为不展开，默认为open
        /// </summary>
        public string state {
            get { return _state; }
            set { _state = value; }
        }
    }

    /// <summary>
    /// 设置用户的部门请求
    /// </summary>
    public class SetMemberDepartmentsRequestModel
    {
        /// <summary>
        /// 用户的ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 根部门限定，如果传入该值，则当前用户在其他组织下的部门不会被操作
        /// </summary>
        public string rootDepartmentId { get; set; }

        /// <summary>
        /// 用户所属的部门
        /// </summary>
        public List<MemberDepartmentSetRequestInfoModel> departments { get; set; }
    }
    /// <summary>
    /// 设置部门的用户请求
    /// </summary>
    public class SetDepartmentMembersRequestModel {
        /// <summary>
        /// 部门的ID
        /// </summary>
        public string departmentId { get; set; }
        /// <summary>
        /// 部门下的用户设置
        /// </summary>
        public List<DepartmentMembersSetRequestInfoModel> users { get; set; }
    }
    /// <summary>
    /// 设置用户所属部门设置参数
    /// </summary>
    public class MemberDepartmentSetRequestInfoModel {
        /// <summary>
        /// 部门的ID
        /// </summary>
        public string departmentId { get; set; }
        /// <summary>
        /// 是否为该部门的管理员
        /// </summary>
        public bool isCaption { get; set; }
    }
    /// <summary>
    /// 设置部门下用户的设置参数
    /// </summary>
    public class DepartmentMembersSetRequestInfoModel
    {
        /// <summary>
        /// 用户的ID
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 是否为该部门的管理员
        /// </summary>
        public bool isCaption { get; set; }
    }
    /// <summary>
    /// 部门的根接单信息
    /// </summary>
    public class DepartmentsrootParentInfoModel {
        /// <summary>
        /// 部门的ID
        /// </summary>
        public string departmentId { get; set; }

        /// <summary>
        /// 部门的根部门信息
        /// </summary>
        public string orgId { get; set; }
        /// <summary>
        /// 当前部门的路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 当前部门的路径，显示值
        /// </summary>
        public string pathText { get; set; }
    }
    /// <summary>
    /// 用户所在部门的信息
    /// </summary>
    public class UserDepartmentInfoList : DepartmentsrootParentInfoModel {
        /// <summary>
        /// 是否是部门的主管
        /// </summary>
        public bool isCaption { get; set; }
    }

    /// <summary>
    /// 批量更新用户所在部门路径字段（用于修改组织/部门或调整部门位置后）
    /// </summary>
    public class EditUserDepartmentsPathRequestModel {
        /// <summary>
        /// 组织ID
        /// </summary>
        public string orgId { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public List<string> departments { get; set; }
    }
}