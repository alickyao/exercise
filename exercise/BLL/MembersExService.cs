using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using cyclonestyle.DataBase;
using cyclonestyle.Models;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 用户扩展信息服务
    /// 常用地址，常用联系人等与用户相关的一些方法
    /// </summary>
    public class MembersExService
    {
        #region  常用旅客管理
        /// <summary>
        /// 新增/编辑 常用旅客 
        /// </summary>
        /// <param name="travellerInfo"></param>
        /// <returns></returns>
        internal static ReplayBase RsSaveTraveller(Traveller travellerInfo)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                //先判断当前FKID有没有常用旅客为默认旅客
                if (MembersExDataBaseManager.RsGetTravellerDefaultCount(travellerInfo.fkId.Trim()) > 0) //有历史
                {
                    //有历史 设置为默认 travellerInfo.isdefault  -  历史全部设置为非默认-设置当前为默认 isdefalut =true 
                    if (travellerInfo.isDefault == true)
                    {
                        try
                        {
                            MembersExDataBaseManager.RsSetTravellertoUndefault(travellerInfo.fkId);
                        }
                        catch (Exception e)
                        {
                            result.ReturnCode = EnumErrorCode.ServiceError;
                            result.ReturnMessage = "服务器错误 500";
                            SysManagerService.SysSaveErrorLogMsg(e.ToString(), travellerInfo);
                        }
                    }
                }
                else//无历史
                {
                    //无历史 设置为默认 - isDefalut=true 
                    //无历史 设置为非默认 isDefalut=false  
                    travellerInfo.isDefault = true;
                }
                result = MembersExDataBaseManager.RsSaveTraveller(travellerInfo);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), travellerInfo);
            }
            return result;
        }


        /// <summary>
        /// 获取常旅客信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<Traveller> RsGetTravellerInfoByfkid(GetUserExInfoListRequest condtion)
        {
            List<Traveller> result;
            try
            {
                if (string.IsNullOrEmpty(condtion.id) && string.IsNullOrEmpty(condtion.fkId))
                {
                    return new List<Traveller>();
                }
                result = MembersExDataBaseManager.RsGetTravellerInfoByfkid(condtion);
                return result;
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                return new List<Traveller>();
            }
        }


        /// <summary>
        /// 将FKID下的常旅客的地址全部设置为非默认
        /// </summary>
        /// <param name="fkid"></param>
        internal static void RsSetTravellertoUndefault(string fkid)
        {
            try
            {
                MembersExDataBaseManager.RsSetTravellertoUndefault(fkid);
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
            }
        }


        /// <summary>
        /// 根据查看的默认常用旅客数量从而可以获取当前FKID下是否有历史数据
        /// </summary>
        /// <param name="fkid"></param>
        /// <returns></returns>
        internal static int RsGetTravellerDefaultCount(string fkid)
        {
            int defaultcount = 0;
            try
            {
                defaultcount = MembersExDataBaseManager.RsGetTravellerDefaultCount(fkid);
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), fkid);
            }
            return defaultcount;

        }

        /// <summary>
        /// 设置旅客为默认常用旅客
        /// </summary>
        /// <param name="id">常用旅客的ID</param> 
        /// <returns></returns>
        internal static ReplayBase SetTravellerToDefault(string id)
        {
            ReplayBase result = new ReplayBase();

            //通过id获取fkid
            List<Traveller> trave = MembersExService.RsGetTravellerInfoByfkid(new GetUserExInfoListRequest() { id = id, onlyDefalut = false });
            if (trave.Count > 0)
            {

                Traveller travellerInfo = new Traveller()
                {
                    ravellerId = id,
                    fkId = trave[0].fkId,
                    fullName = trave[0].fullName,
                    idCard = trave[0].idCard,
                    idType = trave[0].idType,
                    isDefault = true
                };
                result = MembersExService.RsSaveTraveller(travellerInfo);
            }
            else
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500"; 
            }

            return result;

        }

        /// <summary>
        /// 删除常用旅客（物理删除）
        /// 如果删除的是默认常用旅客，[则将当前用户最近编辑的一条这是为默认旅客（如果有的话）]
        /// </summary>
        /// <param name="id">常用旅客id</param>
        /// <returns></returns>
        internal static ReplayBase DelTraveller(string id)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                //根据id获取旅客信息，判断该旅客是否是默认常用旅客

                List<Traveller> trave = MembersExService.RsGetTravellerInfoByfkid(new GetUserExInfoListRequest() { id = id, onlyDefalut = true });
                if (trave.Count > 0)
                {
                    //删除之后再将最新操作的一个旅客设置为默认旅客
                    //1,删除旅客
                    MembersExDataBaseManager.SysDelTraveller(id);
                    //2,找到用户id下剩余的旅客
                    List<Traveller> list_trave = MembersExService.RsGetTravellerInfoByfkid(new GetUserExInfoListRequest() { fkId = trave[0].fkId, onlyDefalut = false });
                    //3,取到最新操作的一个旅客，并且修改为默认旅客(如果还有旅客)
                    if (list_trave.Count > 0)
                    {
                        Traveller tt = list_trave[0];
                        tt.isDefault = true;
                        MembersExService.RsSaveTraveller(tt);
                    }

                }
                else
                {
                    //直接删除
                    MembersExDataBaseManager.SysDelTraveller(id);
                }

            }
            catch (Exception e)
            {

                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), id);
            }
            return result;
        }

        #endregion



        #region  常用地址管理
        /// <summary>
        /// 删除常用地址（物理删除）
        /// 如果删除的时候默认地址，[则将当前用户最近编辑的一条这是为默认地址（如果有的话）]
        /// </summary>
        /// <param name="id">地址id</param>
        /// <returns></returns>
        internal static ReplayBase DelAddress(string id)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                //根据id获取地址信息，判断该地址是否是默认地址

                List<AddressInfoModel> address = MembersExService.GetAddressListByFkId(new GetUserExInfoListRequest() { id = id, onlyDefalut = true });
                if (address.Count > 0)
                {
                    //删除之后再将最新操作的一个地址设置为默认地址
                    //1,删除地址
                    MembersExDataBaseManager.DelAddress(id);
                    //2,找到用户id下剩余的地址
                    List<AddressInfoModel> list_address = MembersExService.GetAddressListByFkId(new GetUserExInfoListRequest() { fkId = address[0].fkId, onlyDefalut = false });
                    //3,取到最新操作的一个地址，并且修改为默认地址(如果还有地址)
                    if (list_address.Count > 0)
                    {
                        AddressInfoModel tt = list_address[0];
                        tt.isDefault = true;
                        MembersExService.EditAddress(tt);
                    }
                }
                else
                {
                    //直接删除
                    MembersExDataBaseManager.DelAddress(id);
                }

            }
            catch (Exception e)
            {

                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), id);
            }
            return result;
        }


        /// <summary>
        /// 设置某地址为默认地址
        /// </summary>
        /// <param name="id">地址id</param>
        /// <returns></returns>
        internal static ReplayBase SetAddressToDefault(string id)
        {
            ReplayBase result = new ReplayBase();
            //通过id获取fkid
            List<AddressInfoModel> address = MembersExService.GetAddressListByFkId(new GetUserExInfoListRequest() { id = id, onlyDefalut = false });
            if (address.Count > 0)
            {

                AddressInfoModel addressInfo = new AddressInfoModel()
                {
                    id = id,
                    fkId = address[0].fkId,
                    address = address[0].address,
                    linkMan = address[0].linkMan,
                    linkPhone = address[0].linkPhone,
                    postCode = address[0].postCode,
                    locationInfo = new LocationInfoModel() { DistrictId = address[0].locationInfo.DistrictId, cityInfo = new GeoCityInfoModel() { CityCode = address[0].locationInfo.cityInfo.CityCode } },
                    isDefault = true
                };
                result = MembersExService.EditAddress(addressInfo);
            }
            else {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
        }

        /// <summary>
        /// 添加或编辑常用地址
        /// </summary>
        /// <param name="addressinfo">地址信息</param>
        /// <returns></returns>
        internal static ReplayBase EditAddress(AddressInfoModel addressinfo)
        {
            ReplayBase rep = new ReplayBase();
            try
            {
                //先判断当前FKID有没有常用地址为默认地址
                if (MembersExDataBaseManager.RsGetAddressHaveDefault(addressinfo.fkId.Trim())) //有历史
                {
                    //有历史 设置为默认 addressinfo.isdefault  -  历史全部设置为非默认-设置当前为默认 isdefalut =true 
                    if (addressinfo.isDefault == true)
                    {
                        try
                        {
                            MembersExDataBaseManager.RsSetAddresstoUndefault(addressinfo.fkId.Trim());
                        }
                        catch (Exception e)
                        {
                            rep.ReturnCode = EnumErrorCode.ServiceError;
                            rep.ReturnMessage = "服务器错误 500";
                            SysManagerService.SysSaveErrorLogMsg(e.ToString(), addressinfo);
                        }
                    }
                }
                else//无历史
                {
                    //无历史 设置为默认 - isDefalut=true 
                    //无历史 设置为非默认 isDefalut=false  
                    addressinfo.isDefault = true;
                }
                rep = MembersExDataBaseManager.EditAddress(addressinfo);
            }
            catch (Exception e)
            {

                rep.ReturnCode = EnumErrorCode.ServiceError;
                rep.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), addressinfo);
            }
            return rep;
        }


        /// <summary>
        /// 获取常用地址，如果没有匹配的常用地址则返回长度为空的集合
        /// </summary>
        /// <param name="condtion">请求中的id或者fkId 二选一必填一项</param>
        /// <returns></returns>
        internal static List<AddressInfoModel> GetAddressListByFkId(GetUserExInfoListRequest condtion)
        {
            List<AddressInfoModel> result;
            try
            {
                if (string.IsNullOrEmpty(condtion.id) && string.IsNullOrEmpty(condtion.fkId))
                {
                    return new List<AddressInfoModel>();
                }
                result = MembersExDataBaseManager.GetAddressListByFkId(condtion);
                return result;
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                return new List<AddressInfoModel>();
            }
        }


        #endregion

    }
}