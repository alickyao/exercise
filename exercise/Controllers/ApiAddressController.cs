using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;
using cyclonestyle.DataBase;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 常用地址管理
    /// </summary>
    public class ApiAddressController : ApiController
    {
        /// <summary>
        /// 添加或编辑常用地址
        /// </summary>
        /// <param name="condtion">地址信息</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase EditAddress(AddressInfoModel condtion)
        {
            ReplayBase result = MembersExService.EditAddress(condtion);
            return result;
        }


        /// <summary>
        /// 获取常用地址，如果没有匹配的常用地址则返回长度为空的集合
        /// </summary>
        /// <param name="condtion">请求中的id或者fkId 二选一必填一项</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public List<AddressInfoModel> GetAddressListByFkId(GetUserExInfoListRequest condtion)
        {
            //关于地域信息的赋值，请在数据库层调用 UsungSysTemDataBaseManager.RsGetLocaionInfo方法获取
            //地域信息数据来源[ApiLocation接口]LocationService.cs，开发人员可参考
            List<AddressInfoModel> result = MembersExService.GetAddressListByFkId(condtion); 
            return result;
        }


        /// <summary>
        /// 设置某地址为默认地址
        /// </summary>
        /// <param name="Id">地址的ID</param> 
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ReplayBase SetAddressToDefault(string Id)
        {
            ReplayBase result = MembersExService.SetAddressToDefault(Id.Trim()); 
            return result;
        }
        /// <summary>
        /// 删除常用地址（物理删除）
        /// 如果删除的时候默认地址，[则将当前用户最近编辑的一条这是为默认地址（如果有的话）]
        /// </summary>
        /// <param name="Id">常用地址的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ReplayBase DelAddress(string Id)
        {
             ReplayBase result = MembersExService.DelAddress(Id.Trim()); 
            return result;
        }
    }
}
