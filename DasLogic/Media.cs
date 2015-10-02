using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to driver functionality in database
    /// </summary>
    public class MediaManager : Das
    {
        private VLF.DAS.DB.Media _media = null;

        public MediaManager(string connectionString)
           : base(connectionString)
		{
            _media = new VLF.DAS.DB.Media(sqlExec);
        
        }

       /// <summary>
       /// Delete media
       /// </summary>
       /// <param name="MediaId"></param>
       /// <returns></returns>
 
       public int DeleteMedia(int MediaId)
       {
           return _media.DeleteMedia(MediaId);
       }

       /// <summary>
       /// Update media
       /// </summary>
       /// <param name="MediaId"></param>
       /// <param name="OrganizationId"></param>
       /// <param name="Description"></param>
       /// <param name="MediaTypeId"></param>
       /// <param name="Factor1"></param>
       /// <param name="Factor2"></param>
       /// <param name="Factor3"></param>
       /// <param name="Factor4"></param>
       /// <param name="Factor5"></param>
       /// <returns></returns>

       public int UpdateMedia(int MediaId, int OrganizationId, string Description, int MediaTypeId, 
           double? Factor1, double? Factor2, double? Factor3, double? Factor4, double? Factor5,
           int UnitOfMeasureId, int UserId)
       {
           return _media.UpdateMedia(MediaId, OrganizationId, Description, MediaTypeId, Factor1, Factor2, Factor3, Factor4, Factor5, UnitOfMeasureId, UserId);
       }

       /// <summary>
       /// Add media
       /// </summary>
       /// <param name="OrganizationId"></param>
       /// <param name="Description"></param>
       /// <param name="MediaTypeId"></param>
       /// <param name="Factor1"></param>
       /// <param name="Factor2"></param>
       /// <param name="Factor3"></param>
       /// <param name="Factor4"></param>
       /// <param name="Factor5"></param>
       /// <returns></returns>
       public int AddMedia(int OrganizationId, string Description, int MediaTypeId, 
           double? Factor1, double? Factor2, double? Factor3, double? Factor4, double? Factor5,
           int UnitOfMeasureId, int UserId)
       {
           return _media.AddMedia(OrganizationId, Description, MediaTypeId, Factor1, Factor2, Factor3, Factor4, Factor5, UnitOfMeasureId, UserId);
       }

       /// <summary>
       /// Get organization medias
       /// </summary>
       /// <param name="OrganizationId"></param>
       /// <returns></returns>
       public DataSet GetOrganizationMedias(int OrganizationId, int UserId)
       {
           return _media.GetOrganizationMedias(OrganizationId, UserId);
       }

       /// <summary>
       /// Get Media Types
       /// </summary>
       /// <returns></returns>
       public DataSet GetMediaTypes()
       {
           return _media.GetMediaTypes();
       }

       /// <summary>
       /// Get Media Factor Names By MediaTypeId
       /// </summary>
       /// <param name="MediaTypeId"></param>
       /// <returns></returns>
       public DataSet GetMediaFactorNamesByMediaTypeId(int MediaTypeId)
       {
           return _media.GetMediaFactorNamesByMediaTypeId(MediaTypeId);
       }

       /// <summary>
       /// Get Media By MediaId
       /// </summary>
       /// <param name="MediaId"></param>
       /// <returns></returns>
       public DataSet GetMediaByMediaId(int MediaId)
       {
           return _media.GetMediaByMediaId(MediaId);
       }

       /// <summary>
       /// Get Unit Of Measures By UserId
       /// </summary>
       /// <param name="userID"></param>
       /// <returns></returns>
       public DataSet GetUnitOfMeasuresByUserId(int userID)
       {
           return _media.GetUnitOfMeasuresByUserId(userID);
       }

       /// <summary>
       /// Check if media id is in EquipmentMediaAssignment table
       /// </summary>
       /// <param name="MediaId"></param>
       /// <returns></returns>
       public Boolean IsMediaUsedInEquipmentMediaAssignment(int MediaId)
       {
           return _media.IsMediaUsedInEquipmentMediaAssignment(MediaId);
       }
    }
}
