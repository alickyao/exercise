using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 用户组织结构（部门）信息管理
    /// </summary>
    public class ApiDepartmentsController : ApiController
    {
        /// <summary>
        /// 检查当前名称在数据库中出现的次数
        /// 不允许重复创建名称相同的顶级部门
        /// </summary>
        /// <param name="caption">组织的名称</param>
        /// <param name="orgid">需要排除的组织ID</param>
        /// <returns></returns>
        public int GetDepartmentCapitonNum(string caption,string orgid = null) {
            return MembersService.CheckDepartmentCationNumInRootList(caption, orgid);
        }

        /// <summary>
        /// 创建一个部门/组织，创建成功后返被创建的部门的ID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase CreateDepartment(CreateMembersDepartmentReqeustModel condtion) {
            condtion.createdby = User.Identity.Name;
            ReplayBase result = MembersService.CreateDepartment(condtion);
            return result;
        }

        /// <summary>
        /// 检索部门信息(平铺方式，按递归顺序排序，用户显示部门信息)
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public SearchMemberDepartmentReplay SearchDepartments(SearchMemberDepartmentRequst condtion) {
            MembersService ms = new MembersService();
            SearchMemberDepartmentReplay result = ms.SearchDepartments(condtion);
            return result;
        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public List<Tree> SearchDepartmentsTree(SearchMemberDepartmentRequst condtion) {
            MembersService ms = new MembersService();
            List<Tree> result = ms.SearchDepartmentsTree(condtion);
            return result;
        }

        /// <summary>
        /// 获取部门详情
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public DepInfoModel GetDepInfo(GetDepInfoRequestModel condtion) {
            MembersService ms = new MembersService();
            ms.GetDepBaseInfo(condtion);
            return ms.depInfo;
        }

        /// <summary>
        /// 编辑部门基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase EditDepInfo(EditDepInfoRequestModel condtion) {
            condtion.modifiedby = User.Identity.Name;
            MembersService ms = new MembersService();
            return ms.EditDepBaseInfo(condtion);
        }

        /// <summary>
        /// 检索顶级组织列表（用于后台显示所有组织列表）
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public SearchMemberRootDepartMentReplay SearchRootDepartments(SearchMemberRootDepartmentRequest condtion) {
            MembersService ms = new MembersService();
            SearchMemberRootDepartMentReplay result = ms.SearchRootDepartments(condtion);
            return result;
        }

        /// <summary>
        /// 获取组织详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public OrgInfoModel GetOrgInfoById(string Id) {
            MembersService ms = new MembersService();
            ms.GetOrgBaseInfo(Id);
            return ms.orgInfo;
        }

        /// <summary>
        /// 编辑组织基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase EditOrgInfo(EditOrgInfoRequestModel condtion) {
            condtion.modifiedBy = User.Identity.Name;
            MembersService ms = new MembersService();
            ReplayBase result = ms.EditOrgBaseInfo(condtion);
            return result;
        }

        /// <summary>
        /// 删除一个或多个部门/组织
        /// </summary>
        /// <param name="Ids">部门或组织的ID，可多个用,隔开</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ReplayBase DelDepmentById(string Ids) {
            ReplayBase result = MembersService.DelDepartmentById(Ids);
            return result;
        }

        /// <summary>
        /// 设置用户所属的部门
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase SetMemberDepartments(SetMemberDepartmentsRequestModel condtion) {
            ReplayBase result = MembersService.SetMemberDepartments(condtion);
            return result;
        }

        /// <summary>
        /// 增加部门下的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public ReplayBase SetDepartmentMembers(SetDepartmentMembersRequestModel condtion)
        {
            ReplayBase result = MembersService.SetDepartmentMembers(condtion);
            return result;
        }

        /// <summary>
        /// 编辑部门下的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public ReplayBase EditDepartmentMembers(SetDepartmentMembersRequestModel condtion) {
            ReplayBase result = MembersService.EditDepartmentMembers(condtion);
            return result;
        }

        /// <summary>
        /// 移除部门下的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public ReplayBase DelDepartmentMembers(SetDepartmentMembersRequestModel condtion) {
            ReplayBase result = MembersService.DelDepartmentMembers(condtion);
            return result;
        }
    }
}
