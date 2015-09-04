using System;
using System.Collections.Generic;
using System.Text;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
//using VLF.ERR;
//using VLF.CLS;
//using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
   public class Form : Das
   {
      private int formID;
      private int versionID;
      private int organizationID;

      # region Properties

      public int GetOrganization
      {
         get { return organizationID; }
      }

      public int GetFormID
      {
         get { return formID; }
      }

      public int GetVersinoID
      {
         get { return versionID; }
      }

      # endregion

      #region General Interfaces

      /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
      public Form(string connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}

		#endregion

      # region Form interfaces

      /// <summary>
      /// Add a new form to an organization
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// returned value: a new form id
      public int AddForm(int orgID)
      {
         int newID = GetMaxFormID(orgID) + 1;
         SqlParameter[] paramList = { new SqlParameter("@orgID", orgID), new SqlParameter("@formID", newID), new SqlParameter("@versionID", 1) };
         /* TODO: Additional form params and fields */
         sqlExec.SPExecuteNonQuery("sp_AddForm", paramList);
         return newID;
      }

      /// <summary>
      /// Update existing form info
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// <param name="formID">Form id</param>
      /// returned value: a new version id
      public int UpdateForm(int orgID, int formID)
      {
         int versionID = GetFormVersion(orgID, formID) + 1;
         SqlParameter[] paramList = { new SqlParameter("@orgID", orgID), 
            new SqlParameter("@formID", formID), 
            new SqlParameter("@versionID", versionID) 
            /* TODO: Update form params and fields */ 
         };
         sqlExec.SPExecuteNonQuery("sp_UpdateForm", paramList);
         return versionID;
      }

      /// <summary>
      /// Delete existing form
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// <param name="formID">Form id</param>
      public void DeleteForm(int orgID, int formID)
      {
         SqlParameter[] paramList = { new SqlParameter("@orgID", orgID), new SqlParameter("@formID", formID) };
         sqlExec.SPExecuteNonQuery("sp_DeleteForm", paramList);
      }

      /// <summary>
      /// Return max form id
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// <returns>Max form id value</returns>
      private int GetMaxFormID(int orgID)
      {
         return (int)sqlExec.SPExecuteScalar("sp_GetMaxFormID", null);
      }

      /// <summary>
      /// Return current version id
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// <param name="formID">Form id</param>
      /// <returns>Version id of the form</returns>
      private int GetFormVersion(int orgID, int formID)
      {
         SqlParameter[] paramList = { new SqlParameter("@orgID", orgID), new SqlParameter("@formID", formID) };
         return (int)sqlExec.SPExecuteScalar("sp_GetFormVersion", paramList);
      }

      /// <summary>
      /// Return form info
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// <param name="formID">Form id</param>
      /// <returns>Form info dataset</returns>
      public DataSet GetFormInfo(int orgID, int formID)
      {
         SqlParameter[] paramList = { new SqlParameter("@orgID", orgID), new SqlParameter("@formID", formID) };
         return sqlExec.SPExecuteDataset("sp_GetFormInfo", paramList);
      }

      # endregion

      # region Data interfaces

      /// <summary>
      /// Get data for a form
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// <param name="formID">Form id</param>
      /// <returns>Data for a specific form of an org.</returns>
      public DataSet GetData(int orgID, int formID)
      {
         SqlParameter[] paramList = { new SqlParameter("@orgID", orgID), new SqlParameter("@formID", formID) };
         return sqlExec.SPExecuteDataset("sp_GetData", paramList);
      }

      /// <summary>
      /// Insert a new row in the data table
      /// </summary>
      /// <param name="orgID">Organization id</param>
      /// <param name="vehID">Vehicle id</param>
      /// <param name="formID">Form id</param>
      /// <param name="fieldID">Field id</param>
      /// <param name="dir">Message direction</param>
      /// <param name="value">Message value</param>
      /// <param name="dateSent">Message date</param>
      public void InsertData(int orgID, int vehID, int formID, int fieldID, short dir, string value, DateTime dateSent)
      {
         SqlParameter[] paramList = { new SqlParameter("@orgID", orgID), new SqlParameter("@vehID", vehID), 
               new SqlParameter("@formID", formID), new SqlParameter("@fieldID", fieldID),
               new SqlParameter("@dir", dir), new SqlParameter("@value", value), 
               new SqlParameter("@datesent", dateSent) };

         sqlExec.SPExecuteNonQuery("sp_InsertData", paramList);
      }

      # endregion
   }
}
