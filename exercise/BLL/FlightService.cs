using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.DataBase;
using cyclonestyle.Models;


namespace cyclonestyle.BLL
{
    /// <summary>
    /// 航旅机票服务
    /// </summary>
    public class FlightService
    {
        #region -- 基础数据  机场 航空公司
        /// <summary>
        /// 机场信息
        /// </summary>
        public FlightAirPortInfoModel airPortInfo { get; set; }
        /// <summary>
        /// 获取机场信息
        /// </summary>
        /// <param name="id"></param>
        internal void GetFlightAirPortInfo(string id)
        {
            airPortInfo = BaseSysTemDataBaseManager.RsGetFlightAirPortInfoById(id);
        }
        /// <summary>
        /// 添加/编辑机场信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase EditFlightAirPort(FlightAirPortInfoModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                //判断code是否已用
                int count = BaseSysTemDataBaseManager.RsGetAirPortCodeCount(condtion);
                if (count == 0)
                {
                    result = BaseSysTemDataBaseManager.RsEditFlightAirPort(condtion);
                    if (result.ReturnCode == EnumErrorCode.Success) {
                        //记录系统日志
                        SysManagerService.CreateSysUserLog(new SysUserLogModel()
                        {
                            Describe = "新增/编辑机场信息" + condtion.city + "[" + condtion.code + "]",
                            FkId = result.ReturnMessage,
                            SysUserId = condtion.modifiedBy
                        });
                    }
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "该机场三字码[" + condtion.code + "]已存在";
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
        /// 删除机场信息（直接从数据库中删除）
        /// </summary>
        /// <param name="Id">机场ID</param>
        /// <param name="UserId">操作者ID</param>
        /// <returns></returns>
        internal ReplayBase DelFlightAirPort(string Id,string UserId)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                GetFlightAirPortInfo(Id);
                result = BaseSysTemDataBaseManager.RsDelFlightAirPortById(this.airPortInfo.id);
                if (result.ReturnCode == EnumErrorCode.Success)
                {
                    SysManagerService.CreateSysUserLog(new SysUserLogModel()
                    {
                        Describe = "删除机场" + this.airPortInfo.caption + this.airPortInfo.code,
                        SysUserId = UserId
                    });
                }
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), Id);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 获取机场列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<FlightAirPortInfoModel> GetFlightAirPortList(GetFlightAirPortListRequestModel condtion)
        {
            List<FlightAirPortInfoModel> result = new List<FlightAirPortInfoModel>();
            try
            {
                result = BaseSysTemDataBaseManager.RsGetFlightAirPortList(condtion);
            }
            catch (Exception e) {
                result = new List<FlightAirPortInfoModel>();
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 添加/编辑航空公司信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase EditFlightAirCompany(FlightAirCompanyInfoModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                //判断编码是否存在
                int count = BaseSysTemDataBaseManager.RsGetAirCompanyCodeCount(condtion);
                if (count == 0)
                {
                    result = BaseSysTemDataBaseManager.RsEditFlightAirCompany(condtion);
                    if (result.ReturnCode == EnumErrorCode.Success) {
                        SysManagerService.CreateSysUserLog(new SysUserLogModel()
                        {
                            Describe = "新增/编辑航空公司信息" + condtion.caption + "[" + condtion.code + "]",
                            FkId = result.ReturnMessage,
                            SysUserId = condtion.modifiedBy
                        });
                    }
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "该航空公司编码[" + condtion.code + "]已存在";
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
        /// 航空公司信息
        /// </summary>
        internal FlightAirCompanyInfoModel companyInfo { get; set; }

        /// <summary>
        /// 获取航空公司信息
        /// </summary>
        /// <param name="id"></param>
        public void GetFlightCompanyInfo(string id)
        {
            companyInfo = BaseSysTemDataBaseManager.RsGetFlightCompanyInfoById(id);
        }

        /// <summary>
        /// 删除航空公司信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="UserId">操作员ID</param>
        /// <returns></returns>
        internal ReplayBase DelFlightAirCompany(string id, string UserId)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                GetFlightCompanyInfo(id);
                result = BaseSysTemDataBaseManager.RsDelFlightCompanyById(this.companyInfo.id);
                if (result.ReturnCode == EnumErrorCode.Success)
                {
                    SysManagerService.CreateSysUserLog(new SysUserLogModel()
                    {
                        Describe = "删除航空公司" + this.companyInfo.caption + this.companyInfo.code,
                        SysUserId = UserId
                    });
                }
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), id);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 获取航空公司列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<FlightAirCompanyInfoModel> GetFlightAirCompanyList(GetFlightAirCompanyListRequestModel condtion)
        {
            List<FlightAirCompanyInfoModel> result = new List<FlightAirCompanyInfoModel>();
            try
            {
                result = BaseSysTemDataBaseManager.RsGetFlightAirCompanyList(condtion);
            }
            catch (Exception e)
            {
                result = new List<FlightAirCompanyInfoModel>();
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }
        #endregion
    }
}