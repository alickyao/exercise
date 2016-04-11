using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;
using System.Data;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 常用旅客管理
    /// </summary>
    public class ApiTravellerController : ApiController
    {

        /// <summary>
        /// 获取idtype枚举信息
        /// </summary>
        /// <returns></returns>
        public List<Combobox> GetidTypeData()
        {
            List<Combobox> dd = new List<Combobox>();
            dd = new List<Combobox>() {
                new Combobox() {
                    id = EnumUserIdType.IdCard.GetHashCode().ToString(),
                    text = "身份证"
                },new Combobox() {
                    id = EnumUserIdType.PassProt.GetHashCode().ToString(),
                    text = "护照"
                },new Combobox() {
                    id = EnumUserIdType.Officers.GetHashCode().ToString(),
                    text = "军官证"
                },new Combobox() {
                    id = EnumUserIdType.soldier.GetHashCode().ToString(),
                    text = "士兵证"
                },new Combobox() {
                    id = EnumUserIdType.MTP.GetHashCode().ToString(),
                    text = "台胞证"
                },new Combobox() {
                    id = EnumUserIdType.Other.GetHashCode().ToString(),
                    text = "其他"
                }
            };
            return dd;
        }
        /// <summary>
        /// 获取常旅客信息
        /// </summary>
        /// <param name="condtion">请求中的id或者fkId 二选一必填一项</param>
        /// <returns>如果该用户没有默认的旅客信息则返回长度为0的数组，每个用户最多只能有一个默认的常旅客信息</returns>
        [HttpPost]
        [Authorize]
        public List<Traveller> RsGetTravellerInfoByfkid(GetUserExInfoListRequest condtion)
        {
            List<Traveller> result = MembersExService.RsGetTravellerInfoByfkid(condtion);
            return result;
        }
        /// <summary>
        /// 新增/编辑 常用旅客
        /// </summary>
        /// <param name="travellerInfo">常用旅客信息对象</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        //[Authorize(Roles = "Admin,Users")]
        public ReplayBase RsSaveTraveller(Traveller travellerInfo)
        {
            ReplayBase rep = MembersExService.RsSaveTraveller(travellerInfo);
            return rep;
        }
        /// <summary>
        /// 设置旅客为默认常用旅客
        /// </summary>
        /// <param name="Id">常用旅客的ID</param> 
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ReplayBase SetTravellerToDefault(string Id)
        {
            ReplayBase rep = MembersExService.SetTravellerToDefault(Id);
            return rep;
        }


        /// <summary>
        /// 删除常用旅客（物理删除）
        /// 如果删除的是默认常用旅客，[则将当前用户最近编辑的一条这是为默认旅客（如果有的话）]
        /// </summary>
        /// <param name="Id">常用旅客的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ReplayBase DelTraveller(string Id)
        {
            ReplayBase result = MembersExService.DelTraveller(Id);
            return result;
        }

    }
}
