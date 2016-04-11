using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Linq.SqlClient;
using System.Web;
using cyclonestyle.Models;
using System.Web.Security;

namespace cyclonestyle.DataBase
{
    /// <summary>
    /// 用户信息扩展专用
    /// 常用地址，常用联系人等与用户相关的一些数据库操作
    /// </summary>
    public class MembersExDataBaseManager : BaseSysTemDataBaseManager
    {

        #region  常用旅客管理
        /// <summary>
        ///根据查看的默认常用旅客数量从而可以获取当前FKID下是否有历史数据
        /// </summary>
        /// <param name="fkid"></param>
        /// <returns></returns>
        internal static int RsGetTravellerDefaultCount(string fkid)
        {
            int defaultcount = 0;
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                if (!string.IsNullOrEmpty(fkid))
                {
                    defaultcount = (from c in context.Us_RsTraveller where c.isDefault == true && c.fkId == fkid select c.Us_RsTravellerId).Count();
                }
            }
            return defaultcount;
        }

        /// <summary>
        ///获取常旅客信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<Traveller> RsGetTravellerInfoByfkid(GetUserExInfoListRequest condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {

                List<Traveller> result = (from c in context.Us_RsTraveller
                                          where (condtion.onlyDefalut ? c.isDefault == true : true)
                                          && (string.IsNullOrEmpty(condtion.id) ? true : c.Us_RsTravellerId == condtion.id)
                                          && (string.IsNullOrEmpty(condtion.fkId) ? true : c.fkId == condtion.fkId)
                                          orderby c.isDefault descending, c.modifiedOn descending
                                          select new Traveller
                                          {
                                              fkId = c.fkId,
                                              ravellerId = c.Us_RsTravellerId,
                                              isDefault = c.isDefault,
                                              idCard = c.idCard,
                                              idType = (EnumUserIdType)c.idType,
                                              fullName = c.fullName,
                                              modifiedOn = c.modifiedOn
                                          }).ToList();
                return result;
            }
        }

        /// <summary>
        /// 将FKID下的常旅客的全部设置为非默认
        /// </summary>
        /// <param name="fkid"></param>
        internal static void RsSetTravellertoUndefault(string fkid)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                var result = (from c in context.Us_RsTraveller where c.fkId == fkid select c).ToList();
                foreach (var a in result)
                {
                    a.isDefault = false;
                }
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// 新增/编辑 常用旅客,[isDefault:默认：true,非默认：false]
        /// </summary>
        /// <param name="travellerInfo"></param>
        /// <returns></returns>
        internal static ReplayBase RsSaveTraveller(Traveller travellerInfo)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string NewTravellerId = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(travellerInfo.ravellerId))
                {
                    //新增
                    Us_RsTraveller dbtraveller = new Us_RsTraveller()
                    {
                        modifiedOn = DateTime.Now,
                        fullName = travellerInfo.fullName,
                        idType = (byte)travellerInfo.idType.GetHashCode(),
                        idCard = travellerInfo.idCard,
                        isDefault = travellerInfo.isDefault,
                        fkId = travellerInfo.fkId,
                        Us_RsTravellerId = NewTravellerId
                    };
                    context.Us_RsTraveller.InsertOnSubmit(dbtraveller);
                }
                else
                {
                    //编辑
                    NewTravellerId = travellerInfo.ravellerId;
                    Us_RsTraveller dbtraveller = context.Us_RsTraveller.Single(p => p.Us_RsTravellerId == travellerInfo.ravellerId);
                    dbtraveller.modifiedOn = DateTime.Now;
                    dbtraveller.fullName = travellerInfo.fullName;
                    dbtraveller.idType = (byte)travellerInfo.idType.GetHashCode();
                    dbtraveller.idCard = travellerInfo.idCard;
                    dbtraveller.isDefault = travellerInfo.isDefault;
                    dbtraveller.fkId = travellerInfo.fkId;
                }
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnMessage = NewTravellerId
                };
            }
        }

        /// <summary>
        /// 删除常用旅客
        /// </summary>
        /// <param name="id">常用旅客id</param>
        /// <returns></returns>
        internal static ReplayBase SysDelTraveller(string id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_RsTraveller trave = context.Us_RsTraveller.Single(p => p.Us_RsTravellerId == id);
                context.Us_RsTraveller.DeleteOnSubmit(trave);
                context.SubmitChanges();
                return new ReplayBase() { ReturnMessage = id };
            }
        }
        #endregion


        #region  常用地址管理
        /// <summary>
        ///  删除常用地址（物理删除）
        /// 如果删除的时候默认地址，[则将当前用户最近编辑的一条这是为默认地址（如果有的话）]
        /// </summary>
        /// <param name="id">地址id</param>
        /// <returns></returns>
        internal static ReplayBase DelAddress(string id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_RsAddress addrss = context.Us_RsAddress.Single(p => p.Us_RsAddressId == id);
                context.Us_RsAddress.DeleteOnSubmit(addrss);
                context.SubmitChanges();
                return new ReplayBase() { ReturnMessage = id };
            }
        }


        /// <summary>
        /// 获取地址信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<AddressInfoModel> GetAddressListByFkId(GetUserExInfoListRequest condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {

                List<AddressInfoModel> result = (from c in context.Us_RsAddress
                                                 where (condtion.onlyDefalut ? c.isDefault == true : true)
                                                 && (string.IsNullOrEmpty(condtion.id) ? true : c.Us_RsAddressId == condtion.id)
                                                 && (string.IsNullOrEmpty(condtion.fkId) ? true : c.fkId == condtion.fkId)
                                                 orderby c.isDefault descending, c.modifiedOn descending
                                                 select new AddressInfoModel
                                                 {
                                                     fkId = c.fkId,
                                                     id = c.Us_RsAddressId,
                                                     isDefault = c.isDefault,
                                                     address = c.address,
                                                     linkMan = c.linkMan,
                                                     linkPhone = c.linkPhone,
                                                     postCode = c.postCode,
                                                     locationInfo = BaseSysTemDataBaseManager.RsGetLocaionInfo(new LocationInfoModel() {
                                                         cityInfo = new GeoCityInfoModel() {
                                                             CityCode = c.cityId
                                                         },
                                                         DistrictId = c.districtId
                                                     }),
                                                     modifiedOn = c.modifiedOn
                                                 }).ToList();
                return result;
            }
        }


        /// <summary>
        /// 根据fkid查看是否有默认常用地址
        /// </summary>
        /// <param name="fkid"></param>
        /// <returns></returns>
        internal static bool RsGetAddressHaveDefault(string fkid)
        {
            bool defaults = false;
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                if (!string.IsNullOrEmpty(fkid))
                {
                    if ((from c in context.Us_RsAddress
                         where c.isDefault == true && c.fkId == fkid
                         select c.Us_RsAddressId).Count() > 0)
                    {
                        defaults = true;
                    }
                }
            }
            return defaults;
        }

        /// <summary>
        /// 添加或编辑常用地址
        /// </summary>
        /// <param name="addressinfo">地址信息</param>
        /// <returns></returns>
        internal static ReplayBase EditAddress(AddressInfoModel addressinfo)
        {

            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string NewAddressId = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(addressinfo.id))
                {
                    //新增
                    Us_RsAddress dbaddress = new Us_RsAddress()
                    {
                        modifiedOn = DateTime.Now,
                        Us_RsAddressId = NewAddressId,
                        address = addressinfo.address,
                        cityId = addressinfo.locationInfo.cityInfo.CityCode,
                        districtId = addressinfo.locationInfo.DistrictId,
                        linkMan = addressinfo.linkMan,
                        linkPhone = addressinfo.linkPhone,
                        postCode = addressinfo.postCode,
                        isDefault = addressinfo.isDefault,
                        fkId = addressinfo.fkId,
                    };
                    context.Us_RsAddress.InsertOnSubmit(dbaddress);
                }
                else
                {
                    //编辑
                    NewAddressId = addressinfo.id;
                    Us_RsAddress dbaddress = context.Us_RsAddress.Single(p => p.Us_RsAddressId == addressinfo.id);
                    dbaddress.modifiedOn = DateTime.Now;
                    dbaddress.address = addressinfo.address;
                    dbaddress.cityId = addressinfo.locationInfo.cityInfo.CityCode;
                    dbaddress.districtId = addressinfo.locationInfo.DistrictId;
                    dbaddress.linkMan = addressinfo.linkMan;
                    dbaddress.linkPhone = addressinfo.linkPhone;
                    dbaddress.postCode = addressinfo.postCode;
                    dbaddress.isDefault = addressinfo.isDefault;
                    dbaddress.fkId = addressinfo.fkId;
                }
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnMessage = NewAddressId
                };
            }
        }

        /// <summary>
        /// 将FKID下的地址全部设置为非默认
        /// </summary>
        /// <param name="fkid"></param>
        internal static void RsSetAddresstoUndefault(string fkid)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                var result = (from c in context.Us_RsAddress where c.fkId == fkid select c).ToList();
                foreach (var a in result)
                {
                    a.isDefault = false;
                }
                context.SubmitChanges();
            }
        }



        #endregion


    }
}