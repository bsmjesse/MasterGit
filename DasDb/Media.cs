using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to Media table.
    /// </summary>
    public class Media : TblGenInterfaces
    {
      public Media(SQLExecuter sqlExec)
            : base("Media", sqlExec)
      {
      }
 
      /// <summary>
      /// Delete Media
      /// </summary>
      /// <param name="MediaId"></param>
      /// <returns></returns>
       public int DeleteMedia(int MediaId)
      {
          int rowsAffected = 0;
          try
          {
              string sql = "Media_Delete";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@MediaId", SqlDbType.Int, MediaId);
              sqlExec.AddCommandParam("@RETURN_VALUE", SqlDbType.Int, ParameterDirection.ReturnValue, 0);
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);
              if (sqlExec.ReadCommandParam("@RETURN_VALUE") != null  && sqlExec.ReadCommandParam("@RETURN_VALUE").ToString() == "-1") return -1;
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to Delete Media. MediaId:" + MediaId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to Delete Media. MediaId:" + MediaId;
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          if (rowsAffected == 0)
          {
              string prefixMsg = "Unable to Delete Media. MediaId:" + MediaId;
              throw new DASAppDataAlreadyExistsException(prefixMsg + " This Media does not exist.");
          }
          return rowsAffected;
      }

        /// <summary>
        /// Update Equipment
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
           int rowsAffected = 0;
           try
           {
               string sql = "Media_Update";

               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@MediaId", SqlDbType.Int, MediaId);
               sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
               sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, Description);
               sqlExec.AddCommandParam("@MediaTypeId", SqlDbType.SmallInt, MediaTypeId);
               
               if (Factor1 != null) sqlExec.AddCommandParam("@Factor1", SqlDbType.Float, Factor1);
               else sqlExec.AddCommandParam("@Factor1", SqlDbType.Float, DBNull.Value);
               if (Factor2 != null)  sqlExec.AddCommandParam("@Factor2", SqlDbType.Float, Factor2);
               else sqlExec.AddCommandParam("@Factor2", SqlDbType.Float, DBNull.Value);
               if (Factor3 != null)  sqlExec.AddCommandParam("@Factor3", SqlDbType.Float, Factor3);
               else sqlExec.AddCommandParam("@Factor3", SqlDbType.Float, DBNull.Value);
               if (Factor4 != null)  sqlExec.AddCommandParam("@Factor4", SqlDbType.Float, Factor4);
               else sqlExec.AddCommandParam("@Factor4", SqlDbType.Float, DBNull.Value);
               if (Factor5 != null)  sqlExec.AddCommandParam("@Factor5", SqlDbType.Float, Factor5);
               else sqlExec.AddCommandParam("@Factor5", SqlDbType.Float, DBNull.Value);
               sqlExec.AddCommandParam("@UnitOfMeasureId", SqlDbType.Int, UnitOfMeasureId);
               sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

               rowsAffected = sqlExec.SPExecuteNonQuery(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to update Media to OrganizationId:" + OrganizationId + " Media:" + MediaId;
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to update Media to OrganizationId:" + OrganizationId + " Media:" + MediaId;
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           if (rowsAffected == 0)
           {
               string prefixMsg = "Unable to update Media to OrganizationId:" + OrganizationId + " Media:" + MediaId;
               throw new DASAppDataAlreadyExistsException(prefixMsg + " This media does not exist.");
           }
           return rowsAffected;
       }

        /// <summary>
        /// Add Media
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
           int rowsAffected = 0;
           try
           {
               string sql = "Media_Add";

               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
               sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, Description);
               sqlExec.AddCommandParam("@MediaTypeId", SqlDbType.SmallInt, MediaTypeId);

               if (Factor1 != null) sqlExec.AddCommandParam("@Factor1", SqlDbType.Float, Factor1);
               else sqlExec.AddCommandParam("@Factor1", SqlDbType.Float, DBNull.Value);
               if (Factor2 != null) sqlExec.AddCommandParam("@Factor2", SqlDbType.Float, Factor2);
               else sqlExec.AddCommandParam("@Factor2", SqlDbType.Float, DBNull.Value);
               if (Factor3 != null) sqlExec.AddCommandParam("@Factor3", SqlDbType.Float, Factor3);
               else sqlExec.AddCommandParam("@Factor3", SqlDbType.Float, DBNull.Value);
               if (Factor4 != null) sqlExec.AddCommandParam("@Factor4", SqlDbType.Float, Factor4);
               else sqlExec.AddCommandParam("@Factor4", SqlDbType.Float, DBNull.Value);
               if (Factor5 != null) sqlExec.AddCommandParam("@Factor5", SqlDbType.Float, Factor5);
               else sqlExec.AddCommandParam("@Factor5", SqlDbType.Float, DBNull.Value);
               sqlExec.AddCommandParam("@UnitOfMeasureId", SqlDbType.Int, UnitOfMeasureId);
               sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

               rowsAffected = sqlExec.SPExecuteNonQuery(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to add Media to OrganizationId:" + OrganizationId;
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to add Media to OrganizationId:" + OrganizationId;
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           if (rowsAffected == 0)
           {
               string prefixMsg = "Unable to add Media to OrganizationId:" + OrganizationId;
               throw new DASAppDataAlreadyExistsException(prefixMsg + " This media already exists.");
           }

           return rowsAffected;
       }
       
       /// <summary>
       /// Get Organization Medias
       /// </summary>
       /// <param name="OrganizationId"></param>
       /// <returns></returns>
       public DataSet GetOrganizationMedias(int OrganizationId, int UserId)
       {
           DataSet sqlDataSet = null;
           try
           {

               string sql = "GetOrganizationMedias";

               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
               sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

               //Executes SQL statement
               sqlDataSet = sqlExec.SPExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve Medias by OrganizationId=" + OrganizationId.ToString() + ".";
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve Medias by OrganizationId=" + OrganizationId.ToString() + ".";

               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return sqlDataSet;

       }

       /// <summary>
       /// Get media types
       /// </summary>
       /// <returns></returns>
       public DataSet GetMediaTypes()
       {
           DataSet sqlDataSet = null;
           try
           {

               string sql = "GetMediaTypes";

               //Executes SQL statement
               sqlDataSet = sqlExec.SPExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve Media Types";
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve Media Types";

               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return sqlDataSet;

       }

       /// <summary>
       /// Get Media Factor Names By MediaTypeId
       /// </summary>
       /// <param name="MediaId"></param>
       /// <returns></returns>
       public DataSet GetMediaFactorNamesByMediaTypeId(int MediaTypeId)
       {
           DataSet sqlDataSet = null;
           try
           {
               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@MediaTypeId", SqlDbType.Int, MediaTypeId);


               string sql = "GetMediaFactorNamesByMediaTypeId";

               //Executes SQL statement
               sqlDataSet = sqlExec.SPExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve Media Factor Names";
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve Media Factor Names";

               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return sqlDataSet;


       }

       /// <summary>
       /// Get Media By MediaId
       /// </summary>
       /// <param name="MediaId"></param>
       /// <returns></returns>

       public DataSet GetMediaByMediaId(int MediaId)
       {
           DataSet sqlDataSet = null;
           try
           {
               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@MediaId", SqlDbType.Int, MediaId);


               string sql = "GetMediaByMediaId";

               //Executes SQL statement
               sqlDataSet = sqlExec.SPExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve Media By MediaId=" + MediaId;
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve Media By MediaId=" + MediaId;

               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return sqlDataSet;

       }

       /// <summary>
       /// Get Unit Of Measures By UserId
       /// </summary>
       /// <param name="userID"></param>
       /// <returns></returns>
       public DataSet GetUnitOfMeasuresByUserId(int userID)
       {
           DataSet sqlDataSet = null;
           try
           {
               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@userID", SqlDbType.Int, userID);


               string sql = "GetUnitOfMeasureByUserId";

               //Executes SQL statement
               sqlDataSet = sqlExec.SPExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve Unit Of Measures By UserId=" + userID;
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve Unit Of Measures By UserId=" + userID;

               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return sqlDataSet;

       }

       /// <summary>
       /// Check if media id is in EquipmentMediaAssignment table
       /// </summary>
       /// <param name="MediaId"></param>
       /// <returns></returns>
       public Boolean IsMediaUsedInEquipmentMediaAssignment(int MediaId)
       {
           Boolean isMediaUsed = true;
           try
           {
               string sql = "Select count(MediaId) from EquipmentMediaAssignment where MediaId = " + MediaId;
               object ret = sqlExec.SQLExecuteScalar(sql);
               if (ret == null || ((Int32)ret) == 0)
               {
                   isMediaUsed = false;
               }
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve IsMediaUsedInEquipmentMediaAssignment By MediaId=" + MediaId;
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve IsMediaUsedInEquipmentMediaAssignment By MediaId=" + MediaId;

               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return isMediaUsed;
       }
    }
}
