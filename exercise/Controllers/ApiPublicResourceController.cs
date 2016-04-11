using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.BLL;
using cyclonestyle.Models;
using Newtonsoft.Json;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 通用资源接口，用户输入项管理、分类树等
    /// </summary>
    public class ApiPublicResourceController : ApiController
    {

        #region -- 用户规范输入项维护

        /// <summary>
        /// 新增/编辑用户输入项
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase SaveUserInputRule(ResourceUserInputRuleModel condtion) {
            PublicResourceService prs = new PublicResourceService();
            prs.UserInputRule = condtion;
            ReplayBase result = prs.SaveUserInputRule();
            if (result.ReturnCode == EnumErrorCode.Success) { 
                //记录到用户事件
                SysManagerService.CreateSysUserLog(new SysUserLogModel()
                {
                    SysUserId = User.Identity.Name,
                    FkId = result.ReturnMessage,
                    Describe = "新增/编辑用户输入规范项目："+ JsonConvert.SerializeObject(condtion)
                });
            }
            return result;
        }

        /// <summary>
        /// 获取用户规范集列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public SearchResourceUserInputRuleReplayModel SearchUserInputRuleList(SearchResourceUserInputRuleRequestModel condtion) {
            SearchResourceUserInputRuleReplayModel result = PublicResourceService.SearchUserInputRuleList(condtion);
            return result;
        }

        /// <summary>
        /// 删除用户规范集（标记删除）
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase DelUserInputRules(DelResourceUserInputRulesReqeust condtion) {
            ReplayBase result = PublicResourceService.DelUserInputRules(condtion);
            if (result.ReturnCode == EnumErrorCode.Success) {
                string Ids = string.Empty;
                foreach (ResourceUserInputRuleModel row in condtion.rows) {
                    Ids += row.RuleId + ",";
                }
                Ids = Ids.Substring(0, Ids.Length - 1);
                SysManagerService.CreateSysUserLog(new SysUserLogModel()
                {
                    SysUserId = User.Identity.Name,
                    FkId = result.ReturnMessage,
                    Describe = "删除用户输入规范项：" + result.ReturnMessage + "【" + Ids + "】"
                });
            }
            return result;
        }

        /// <summary>
        /// 获取已有的输入类型集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Users")]
        public List<Combobox> GetUserInputRuleTypes() {
            List<Combobox> rows = PublicResourceService.GetUserInputRuleTypes();
            return rows;
        }
        #endregion

        #region -- 分类树

        /// <summary>
        /// 新增/保存分类树信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase SaveCatInfo(CatInfoModel info) {
            PublicResourceService prs = new PublicResourceService();
            prs.CatInfo = info;
            ReplayBase result = prs.SaveCatInfo();
            if (result.ReturnCode == EnumErrorCode.Success) { 
                //记录到用户日志
                SysManagerService.CreateSysUserLog(new SysUserLogModel()
                {
                    Describe = "用户新增/编辑分类树信息：" + JsonConvert.SerializeObject(info),
                    FkId = result.ReturnMessage,
                    SysUserId = User.Identity.Name
                });
            }
            return result;
        }

        /// <summary>
        /// 获取分类树集合信息ForGrid
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public List<CatInfoModel> SearchCatInfoList(SearchCatInfoRequest condtion) {
            PublicResourceService prs = new PublicResourceService();
            prs.SearchCatInfoCondtion = condtion;
            List<CatInfoModel> result = prs.SearchCatInfoList();
            return result;
        }

        /// <summary>
        /// 获取分类树集合信息ForTree
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Tree> SearchCatTreeList(SearchCatInfoRequest condtion) {
            PublicResourceService prs = new PublicResourceService();
            prs.SearchCatInfoCondtion = condtion;
            List<Tree> result = prs.SearchCatTreeList();
            return result;
        }


        /// <summary>
        /// 标记删除分类树的节点以及子集
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase DelCatInfo(CatInfoModel row) {
            PublicResourceService prs = new PublicResourceService();
            prs.GetCatInfoById(row.id);
            ReplayBase result = prs.DelCatInfoAndChilds();
            if (result.ReturnCode == EnumErrorCode.Success) {
                SysManagerService.CreateSysUserLog(new SysUserLogModel()
                {
                    SysUserId = User.Identity.Name,
                    Describe = string.Format("删除分类树节【{0}】点及其子节点，" + result.ReturnMessage, prs.CatInfo.caption),
                    FkId = prs.CatInfo.id
                });
            }
            return result;
        }

        /// <summary>
        /// 重新设置分类节点的父ID
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase SetCatInfoParent(CatInfoModel Info) {
            PublicResourceService prs = new PublicResourceService();
            prs.GetCatInfoById(Info.id);
            prs.CatInfo._parentId = string.IsNullOrEmpty(Info._parentId) ? null : Info._parentId;
            return SaveCatInfo(prs.CatInfo);
        }

        #endregion

        #region -- 扩展信息
        /// <summary>
        /// 添加/编辑扩展信息
        /// </summary>
        /// <param name="condtion">参数中的catinfo中的ID为必填项目</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase EditExInfo(EditInfoExInfoRequestModel condtion)
        {
            condtion.modifiedBy = User.Identity.Name;
            ReplayBase result = PublicResourceService.EditExInfo(condtion);
            return result;
        }

        /// <summary>
        /// 删除扩展信息
        /// </summary>
        /// <param name="Id">扩展信息的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ReplayBase DelMemberExInfo(string Id)
        {
            EditInfoExInfoRequestModel condtion = new EditInfoExInfoRequestModel()
            {
                exInfoId = Id,
                modifiedBy = User.Identity.Name
            };
            ReplayBase result = PublicResourceService.DelExInfo(condtion);
            return result;
        }
        #endregion
    }
}
