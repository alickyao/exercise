using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.Models;
using cyclonestyle.DataBase;
using Newtonsoft.Json;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 资源文档管理类
    /// </summary>
    public class DocumentResourceService
    {
        /// <summary>
        /// 资源文档
        /// </summary>
        public DocumentsResourceModel Document { get; set; }

        /// <summary>
        /// 新增或保存资源文档
        /// </summary>
        /// <returns></returns>
        internal ReplayBase Save()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.RsSaveDocumentResource(this.Document);
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(),this.Document);
            }
            return result;
        }

        /// <summary>
        /// 查询文档信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchDocumentsResourceListReplayModel SearchDocument(SearchDocumentsResourceListRequestModel condtion)
        {
            SearchDocumentsResourceListReplayModel result = new SearchDocumentsResourceListReplayModel();
            try
            {
                result = BaseSysTemDataBaseManager.RsSearchDocumentResource(condtion);
                if (result.rowlist.Count > 0)
                {
                    //截取内容
                    if (!condtion.GetContent)
                    {
                        foreach (DocumentsResourceModel row in result.rowlist)
                        {
                            row.Content = Helps.GetShotContent(row.Content, condtion.ContentLength);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal static ReplayBase DelDocument(string Id)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.RsDelDocumentResource(Id);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), Id);
            }
            return result;
        }

        /// <summary>
        /// 文档批量排序
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SortDocuments(SortDocumentsRequest condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.RsSortDocumentsResource(condtion);
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
        /// 获取所有已存在的分组
        /// </summary>
        /// <returns></returns>
        internal static List<Combobox> GetAllDocumentsGroupInfo()
        {
            List<Combobox> result = new List<Combobox>();
            try
            {
                result = BaseSysTemDataBaseManager.RsGetAllDocumentsResourceGroupInfoList();
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
            }
            return result;
        }
    }
}