using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.Models;
using cyclonestyle.DataBase;
using System.Text.RegularExpressions;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 会员，以及会员组织服务
    /// </summary>
    public class MembersService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MembersService() {
            this.orgInfo = new OrgInfoModel();
        }

        #region -- 会员相关
        /// <summary>
        /// 会员基础信息
        /// </summary>
        public MembersBaseInfoModel MemberBaseInfo;

        /// <summary>
        /// 会员完整信息
        /// </summary>
        public MembersInfoModel memberInfo;
        
        /// <summary>
        /// 获取用户基础信息
        /// </summary>
        /// <param name="condtion">用户ID与组织ID，传入组织ID会对用户所属部门产生影响</param>
        public void GetMemberBaseInfo(GetMembersInfoRequestModel condtion)
        {
            try
            {
                MembersBaseInfoModel Info = BaseSysTemDataBaseManager.MemberGetMemberBaseInfo(condtion);
                Info.numOfDep = GetMemberNumofDep(Info);
                this.MemberBaseInfo = Info;
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="condtion"></param>
        public void GetMemberInfo(GetMembersInfoRequestModel condtion) {
            GetMemberBaseInfo(condtion);
            try
            {
                //用户扩展信息
                if (this.MemberBaseInfo != null) {
                    this.memberInfo = new MembersInfoModel() {
                        baseInfo = this.MemberBaseInfo,
                        exList = BaseSysTemDataBaseManager.RsGetExInfoList(condtion.userId)
                    };
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
        }

        /// <summary>
        /// 编辑用户基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal ReplayBase EditMemberBaseInfo(MembersBaseInfoModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.MemberEditMemberBaseInfo(condtion);
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 计算用户所属组织与部门统计
        /// </summary>
        /// <param name="user">用户信息，其中的userDepartmentList属性不能为空</param>
        /// <returns></returns>
        private static MemberNumOfDepModel GetMemberNumofDep(MembersBaseInfoModel user)
        {
            //计算所属公司/部门的数量
            MemberNumOfDepModel numOfDep = new MemberNumOfDepModel()
            {
                numofcompany = (from c in user.userDepartmentList
                                group c by new { c.orgId } into g
                                select g.Key.orgId
                                ).Count(),
                numofdepartments = (from c in user.userDepartmentList
                                    group c by new { c.departmentId } into g
                                    select g.Key.departmentId).Count()
            };
            return numOfDep;
        }


        /// <summary>
        /// 会员自助注册
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal RegisterMembersReplayModel RegisterMembers(Models.RegisterMembersRequestModel condtion)
        {
            RegisterMembersReplayModel result = new RegisterMembersReplayModel();
            try
            {
                //请求参数验证
                if (string.IsNullOrEmpty(condtion.LoginName) || string.IsNullOrEmpty(condtion.PassWord)) {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "登录名和密码不能为空";
                    return result;
                }
                if (condtion.LoginName.Length < 6) {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "登录名必须大于6位";
                    return result;
                }
                Regex r = new Regex("^[a-zA-Z0-9_]+$");
                if (!r.IsMatch(condtion.LoginName)) {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "登录名只能由字母数字与下划线组成";
                    return result;
                }

                int count = SysManagerService.CheckLoginNameisExist(condtion.LoginName);
                if (count == 0)
                {
                    result = BaseSysTemDataBaseManager.MemberRegisterMembers(condtion);
                    if (result.ReturnCode == EnumErrorCode.Success)
                    {
                        //注册成功后获取用户基础信息
                        GetMemberBaseInfo(new GetMembersInfoRequestModel() {
                            userId = result.UserInfo.UserId
                        });
                        result.UserInfo = this.MemberBaseInfo;
                    }
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "该登录名已被使用";
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
        /// 用户登录验证
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal RegisterMembersReplayModel CheckMemberLoginNameandPwd(RequestUserLogoModel condtion)
        {
            RegisterMembersReplayModel result = new RegisterMembersReplayModel();
            try
            {
                result = BaseSysTemDataBaseManager.MemberCheckMemberLoginNameandPwd(condtion);
                if (result.ReturnCode == EnumErrorCode.Success) {
                    //登陆成功后获取用户基础信息
                    GetMemberBaseInfo(new GetMembersInfoRequestModel() {
                        userId = result.UserInfo.UserId
                    });
                    result.UserInfo = this.MemberBaseInfo;
                }
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
        /// 会员检索
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchMembersreplayModel SearchMembersList(SearchMembersRequestModel condtion)
        {
            SearchMembersreplayModel result = new SearchMembersreplayModel();
            try
            {
                result = BaseSysTemDataBaseManager.MemberSearchMembersList(condtion);
                if (result.total > 0) {
                    foreach(MembersBaseInfoModel user in result.rows){
                        user.numOfDep = GetMemberNumofDep(user);
                    }
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.rows = new List<MembersBaseInfoModel>();
            }
            return result;
        }

        /// <summary>
        /// 根据用户的ID，登录名或者部门获取用户列表（不翻页，可用于发送短信，推送，批量设置用户等操作）
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<MembersBaseInfoModel> GetMembersList(GetMembersListRequstModel condtion)
        {
            List<MembersBaseInfoModel> result = new List<MembersBaseInfoModel>();
            try
            {
                //如果有设置部门
                if (condtion.depIds.Count > 0) {
                    List<string> depuserids = new List<string>();
                    if (condtion.getChilds)
                    {
                        //需要获取子部门的ID
                        List<string> depids = new List<string>();
                        foreach (string depid in condtion.depIds)
                        {
                            SearchMemberDepartmentRequst dq = new SearchMemberDepartmentRequst() {
                                getchild = true,
                                departmentId = depid
                            };
                            MembersService ms = new MembersService();
                            SearchMemberDepartmentReplay dp = ms.SearchDepartments(dq);
                            if (dp.rows.Count > 0) {
                                foreach (MemberDepartmentBaseInfoModel m in dp.rows) {
                                    if (!depids.Contains(m.depmentid)) {
                                        depids.Add(m.depmentid);
                                    }
                                }
                            }
                        }
                        condtion.depIds = depids;
                    }
                    SearchMembersRequestModel d = new SearchMembersRequestModel()
                    {
                        departmentIds = condtion.depIds,
                        getOtherOrgDepartmentInfo = false
                    };
                    depuserids = BaseSysTemDataBaseManager.DepGetUserIdListBydepartment(d);
                    foreach (string depuserid in depuserids) {
                        if (!condtion.userIds.Contains(depuserid)) {
                            condtion.userIds.Add(depuserid);
                        }
                    }
                }
                if (condtion.userIds.Count == 0 && condtion.loginNames.Count == 0)
                {
                    //判断用户ID或者loginName是否有值，如果没有则直接返回一个空的数组
                    return result;
                }
                else {
                    result = BaseSysTemDataBaseManager.MemberGetMemberListByIdsOrLoginName(condtion);
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result = new List<MembersBaseInfoModel>();
            }
            return result;
        }

        #endregion

        #region -- 组织相关

        /// <summary>
        /// 检查组织名称在跟节点中出现的次数
        /// </summary>
        /// <param name="caption">部门名称</param>
        /// <param name="departmentid">如果是编辑状态时，可排除当前被编辑的部门ID</param>
        /// <returns></returns>
        internal static int CheckDepartmentCationNumInRootList(string caption, string departmentid = null)
        {
            return BaseSysTemDataBaseManager.DepCheckDepartmentCationNumInRootList(caption, departmentid);
        }

        /// <summary>
        /// 新增一个部门/组织，顶级部门名称不能再数据库中重复
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase CreateDepartment(CreateMembersDepartmentReqeustModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                int num = 0;
                //如果父级为空则需要检查根节点是否有同名的组织
                if (string.IsNullOrEmpty(condtion.pid)) {
                    num = CheckDepartmentCationNumInRootList(condtion.caption);
                }
                if (num == 0)
                {
                    //保存信息
                    result = BaseSysTemDataBaseManager.DepCreateDepartment(condtion);
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "该组织名称已存在，不能重复创建";
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }


        /// <summary>
        /// 编辑组织基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal ReplayBase EditOrgBaseInfo(EditOrgInfoRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                int num = 0;
                //检查是否有重名
                num = CheckDepartmentCationNumInRootList(condtion.caption, condtion.depmentid);
                if (num == 0)
                {
                    GetOrgBaseInfo(condtion.depmentid);
                    result = BaseSysTemDataBaseManager.DepEditOrgBaseInfo(condtion);//保存信息
                    if (result.ReturnCode == EnumErrorCode.Success) {
                        if (this.orgInfo.baseInfo.caption != condtion.caption)
                        {
                            SearchMemberDepartmentReplay dr = SearchDepartments(new SearchMemberDepartmentRequst() {
                                departmentId = condtion.depmentid,
                                getchild =true,
                                showdisabled = true,
                                showontheui = false
                            });
                            List<string> depmentids = new List<string>();
                            foreach (MemberDepartmentBaseInfoModel d in dr.rows) {
                                depmentids.Add(d.depmentid);
                            }
                            BaseSysTemDataBaseManager.DepEditUserDepmentPath(depmentids);
                        }
                    }
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "该组织名称已存在，不能重复创建";
                }
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 编辑部门基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal ReplayBase EditDepBaseInfo(EditDepInfoRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                GetDepBaseInfo(new GetDepInfoRequestModel() {
                    depmentid = condtion.depmentid
                });
                result = BaseSysTemDataBaseManager.DepEditDepBaseInfo(condtion);//保存信息
                if (result.ReturnCode == EnumErrorCode.Success)
                {
                    if (this.depInfo.baseinfo.caption != condtion.caption)
                    {
                        SearchMemberDepartmentReplay dr = SearchDepartments(new SearchMemberDepartmentRequst()
                        {
                            _parentId = condtion.depmentid,
                            getchild = true,
                            showdisabled = true,
                            showontheui = false
                        });
                        List<string> depmentids = new List<string>();
                        depmentids.Add(condtion.depmentid);
                        foreach (MemberDepartmentBaseInfoModel d in dr.rows)
                        {
                            depmentids.Add(d.depmentid);
                        }
                        BaseSysTemDataBaseManager.DepEditUserDepmentPath(depmentids);
                    }
                }
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }


        /// <summary>
        /// 标记删除部门/组织，可一次标记删除多个（非递归）
        /// </summary>
        /// <param name="ids">多个ID用,隔开</param>
        /// <returns></returns>
        internal static ReplayBase DelDepartmentById(string ids)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    string[] id = ids.Split(',');
                    result = BaseSysTemDataBaseManager.DepDelDepartmentById(id);
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "参数必填";
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), ids);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 部门列表
        /// </summary>
        internal List<MemberDepartmentBaseInfoModel> departmentslist { get; set; }

        /// <summary>
        /// 检索部门请求参数
        /// </summary>
        internal SearchMemberDepartmentRequst searchdepartmentscondtion { get; set; }

        /// <summary>
        /// 平铺方式检索部门信息（不翻页用于获取某组织下部门）
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal SearchMemberDepartmentReplay SearchDepartments(SearchMemberDepartmentRequst condtion)
        {
            searchdepartmentscondtion = condtion;
            SearchMemberDepartmentReplay resut = new SearchMemberDepartmentReplay();
            try
            {
                List<MemberDepartmentBaseInfoModel> roollist = BaseSysTemDataBaseManager.DepSearchDepartments(condtion);
                if (condtion.getchild)
                {
                    if (roollist.Count > 0)
                    {
                        departmentslist = new List<MemberDepartmentBaseInfoModel>();
                        foreach (MemberDepartmentBaseInfoModel d in roollist)
                        {
                            getchlidDepartments(d);
                        }
                    }
                    else
                    {
                        departmentslist = new List<MemberDepartmentBaseInfoModel>();
                    }
                    resut.rows = departmentslist;
                }
                else {
                    departmentslist = roollist;
                    resut.rows = roollist;
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                resut.rows = new List<MemberDepartmentBaseInfoModel>();
            }
            return resut;
        }

        /// <summary>
        /// 部门树
        /// </summary>
        protected List<Tree> departmenttree { get; set; }

        /// <summary>
        /// 获取组织/部门树
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal List<Tree> SearchDepartmentsTree(SearchMemberDepartmentRequst condtion)
        {
            List<Tree> result = new List<Tree>();
            List<MemberDepartmentBaseInfoModel> rootlist = BaseSysTemDataBaseManager.DepSearchDepartments(condtion);
            foreach (MemberDepartmentBaseInfoModel root in rootlist) {
                result.Add(new Tree()
                {
                    attributes = new TreeAttributes()
                    {
                        Sort = 0
                    },
                    id = root.depmentid,
                    text = root.caption,
                    children = condtion.getchild ? SearchDepartmentsTree(new SearchMemberDepartmentRequst()
                    {
                        _parentId = root.depmentid,
                        ordertype = condtion.ordertype,
                        showdisabled = condtion.showdisabled,
                        showontheui = condtion.showontheui,
                        getchild = condtion.getchild
                    }) : new List<Tree>()
                });
            }
            return result;
        }

        /// <summary>
        /// 递归获取部门子集
        /// </summary>
        /// <param name="d"></param>
        private void getchlidDepartments(MemberDepartmentBaseInfoModel d)
        {
            departmentslist.Add(d);
            List<MemberDepartmentBaseInfoModel> childlist = BaseSysTemDataBaseManager.DepSearchDepartments(new SearchMemberDepartmentRequst() {
                _parentId = d.depmentid,
                ordertype = EnumSortOrderType.按标题升序,
                showdisabled = searchdepartmentscondtion.showdisabled,
                showontheui = searchdepartmentscondtion.showontheui
            });
            if (childlist.Count > 0) {
                foreach (MemberDepartmentBaseInfoModel c in childlist) {
                    getchlidDepartments(c);
                }
            }
        }

        /// <summary>
        /// 检索根组织(可翻页，后台列表与企业名片数据展示)
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal SearchMemberRootDepartMentReplay SearchRootDepartments(SearchMemberRootDepartmentRequest condtion)
        {
            try
            {
                SearchMemberRootDepartMentReplay result = BaseSysTemDataBaseManager.DepSearchRootDepartments(condtion);
                return result;
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                return new SearchMemberRootDepartMentReplay()
                {
                    rows = new List<MemberDepartmentInfoModel>()
                };
            }
        }

        /// <summary>
        /// 组织详情
        /// </summary>
        public OrgInfoModel orgInfo { get; set; }

        

        /// <summary>
        /// 获取组织基础信息
        /// </summary>
        internal void GetOrgBaseInfo(string orgId)
        {
            try
            {
                this.orgInfo.baseInfo = BaseSysTemDataBaseManager.DepGetOrgBaseInfoById(orgId);
                this.orgInfo.exList = BaseSysTemDataBaseManager.RsGetExInfoList(orgId);
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), orgId);
            }
        }

        /// <summary>
        /// 部门详情
        /// </summary>
        internal DepInfoModel depInfo;

        /// <summary>
        /// 获取部门基础信息
        /// </summary>
        /// <param name="condtion"></param>
        internal void GetDepBaseInfo(GetDepInfoRequestModel condtion)
        {
            try
            {
                depInfo = BaseSysTemDataBaseManager.DepGetDepBaseInfoById(condtion.depmentid);
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
        }


        /// <summary>
        /// 设置用户的部门
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SetMemberDepartments(SetMemberDepartmentsRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.DepSetMemberDepartments(condtion);
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 增加部门的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SetDepartmentMembers(SetDepartmentMembersRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.DepSetDepartmentMembers(condtion);
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 编辑部门下的用户信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase EditDepartmentMembers(SetDepartmentMembersRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.DepEditDepartmentMembers(condtion);
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 移除部门下的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DelDepartmentMembers(SetDepartmentMembersRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.DepDelDepartmentMembers(condtion);
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        #endregion
    }
}