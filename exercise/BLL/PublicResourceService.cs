using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.Models;
using cyclonestyle.DataBase;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 公用资源管理服务，用户输入项、等
    /// </summary>
    public class PublicResourceService
    {

        #region -- 用户规范输入项维护

        /// <summary>
        /// 用户输入规范
        /// </summary>
        public ResourceUserInputRuleModel UserInputRule { get; set; }

        /// <summary>
        /// 新增/保存一条用户输入规范
        /// </summary>
        /// <returns></returns>
        internal ReplayBase SaveUserInputRule()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.RsSaveUserInputRule(this.UserInputRule);
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), this.UserInputRule);
            }
            return result;
        }

        /// <summary>
        /// 获取用户输入规范项列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchResourceUserInputRuleReplayModel SearchUserInputRuleList(SearchResourceUserInputRuleRequestModel condtion)
        {
            SearchResourceUserInputRuleReplayModel result = new SearchResourceUserInputRuleReplayModel();
            try
            {
                result = BaseSysTemDataBaseManager.RsSearchUserInputRuleList(condtion);
                if (result.rows.Count > 0) {
                    foreach (ResourceUserInputRuleModel row in result.rows) {
                        if (!string.IsNullOrEmpty(row.OptionsString)) {
                            row.OptionsList = row.OptionsString.Split(',').ToList();
                        }
                    }
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }


        /// <summary>
        /// 标记删除用户输入规范项
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DelUserInputRules(DelResourceUserInputRulesReqeust condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                List<string> Ids = new List<string>();
                if (condtion.rows.Count > 0)
                {
                    foreach (ResourceUserInputRuleModel row in condtion.rows) {
                        Ids.Add(row.RuleId);
                    }
                    result = BaseSysTemDataBaseManager.RsDelUserInputRules(Ids);
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "删除行的集合不能为空";
                }
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 获取已有的输入类型集合
        /// </summary>
        /// <returns></returns>
        internal static List<Combobox> GetUserInputRuleTypes()
        {
            try
            {
                List<Combobox> result = BaseSysTemDataBaseManager.RsGetUserInputRuleTypes();
                return result;
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
                return new List<Combobox>();
            }
        }
        #endregion

        #region -- 分类树

        /// <summary>
        /// 分类树信息
        /// </summary>
        public CatInfoModel CatInfo { get; set; }

        /// <summary>
        /// 新增/保存分类树节点
        /// </summary>
        /// <returns></returns>
        internal ReplayBase SaveCatInfo()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.RsSaveCatInfo(this.CatInfo);
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), this.CatInfo);
            }
            return result;
        }

        

        /// <summary>
        /// 查询请求
        /// </summary>
        public SearchCatInfoRequest SearchCatInfoCondtion { get; set; }
        /// <summary>
        /// 分类树返回集合
        /// </summary>
        public List<CatInfoModel> SearchCatInfoListReplay { get; set; }
        /// <summary>
        /// 分类树返回树形集合体
        /// </summary>
        public List<Tree> SearchCatTreeListReplay { get; set; }

        /// <summary>
        /// 当前深度
        /// </summary>
        public int Dep = 1;

        /// <summary>
        /// 获取分类树信息
        /// </summary>
        /// <param name="id">分类信息ID</param>
        internal void GetCatInfoById(string id)
        {
            try
            {
                this.CatInfo = BaseSysTemDataBaseManager.RsGetCatInfoById(id);
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), id);
            }
        }

        /// <summary>
        /// 查询分类树集合
        /// </summary>
        /// <returns></returns>
        internal List<CatInfoModel> SearchCatInfoList()
        {
            SearchCatInfoListReplay = new List<CatInfoModel>();
            try
            {
                //获取集合
                List<CatInfoModel> root = BaseSysTemDataBaseManager.RsSearchCatInfoList(this.SearchCatInfoCondtion);
                if (root.Count > 0)
                { 
                    //取子集
                    foreach (CatInfoModel row in root) {
                        if (!string.IsNullOrEmpty(this.SearchCatInfoCondtion.id) || !string.IsNullOrEmpty(this.SearchCatInfoCondtion.KeyWords)) {
                            row._parentId = null;
                        }
                        Dep = 1;
                        GetCatChildInfo(row);
                    }
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), this.SearchCatInfoCondtion);
            }
            return SearchCatInfoListReplay;
        }
        /// <summary>
        /// 获取子集信息
        /// </summary>
        /// <param name="row"></param>
        private void GetCatChildInfo(CatInfoModel row)
        {
            if (Dep <= SearchCatInfoCondtion.Dep || SearchCatInfoCondtion.Dep == null)
            {
                if (Dep >= 10)
                {
                    row.state = "closed";
                }
                SearchCatInfoListReplay.Add(row);
                Dep++;
                List<CatInfoModel> childs = BaseSysTemDataBaseManager.RsSearchCatInfoList(new SearchCatInfoRequest()
                {
                    _parentId = row.id
                });
                if (childs.Count > 0)
                {
                    foreach (CatInfoModel child in childs)
                    {
                        GetCatChildInfo(child);
                    }
                }
            }
        }

        /// <summary>
        /// 查询分类树树形集合
        /// </summary>
        /// <returns></returns>
        internal List<Tree> SearchCatTreeList()
        {
            SearchCatTreeListReplay = new List<Tree>();
            try
            {
                List<CatInfoModel> root = BaseSysTemDataBaseManager.RsSearchCatInfoList(this.SearchCatInfoCondtion);
                if (root.Count > 0) {
                    //取子集
                    foreach (CatInfoModel row in root)
                    {
                        Dep = 1;
                        Tree Node = new Tree()
                        {
                            attributes = new TreeAttributes() {
                                Sort = row.sort
                            },
                            id = row.id,
                            text = row.caption,
                            children = GetCatChildTreeInfo(row.id)
                        };
                        SearchCatTreeListReplay.Add(Node);
                    }
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), this.SearchCatInfoCondtion);
            }
            return SearchCatTreeListReplay;
        }
        /// <summary>
        /// 获取树的子节点
        /// </summary>
        /// <param name="Id">ID</param>
        /// <returns></returns>
        private List<Tree> GetCatChildTreeInfo(string Id)
        {
            List<Tree> Tree = new List<Tree>();

            if (Dep <= SearchCatInfoCondtion.Dep || SearchCatInfoCondtion.Dep == null)
            {
                Dep++;
                List<CatInfoModel> childs = BaseSysTemDataBaseManager.RsSearchCatInfoList(new SearchCatInfoRequest()
                {
                    _parentId = Id
                });
                if (childs.Count > 0)
                {
                    foreach (CatInfoModel child in childs)
                    {
                        Tree Node = new Tree()
                        {
                            attributes = new TreeAttributes()
                            {
                                Sort = child.sort
                            },
                            id = child.id,
                            text = child.caption,
                            state = Dep >= 10 ? "closed" : "open",
                            children = GetCatChildTreeInfo(child.id)
                        };
                        Tree.Add(Node);
                    }
                }
            }
            return Tree;
        }

        /// <summary>
        /// 标记删除分类树及其他的子集
        /// </summary>
        /// <returns></returns>
        internal ReplayBase DelCatInfoAndChilds()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                List<string> Ids = new List<string>();
                //获取他自己和自己
                SearchCatInfoCondtion = new SearchCatInfoRequest() { 
                    id = this.CatInfo.id
                };
                SearchCatInfoList();
                if (this.SearchCatInfoListReplay.Count > 0)
                {
                    foreach (CatInfoModel info in this.SearchCatInfoListReplay)
                    {
                        Ids.Add(info.id);
                    }
                    result = BaseSysTemDataBaseManager.RsDelCatTree(Ids);
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "没有找到需要删除的数据";
                }
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), this.CatInfo);
            }
            return result;
        }

        #endregion

        #region -- 扩展信息

        /// <summary>
        /// 新增或编辑用户扩展信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase EditExInfo(EditInfoExInfoRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.RsEditExInfo(condtion);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }


        /// <summary>
        /// 标记删除用户扩展信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DelExInfo(EditInfoExInfoRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.RsDelExInfo(condtion);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }
        #endregion
    }
}