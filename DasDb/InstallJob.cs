using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VLF.CLS;
using VLF.ERR;
using System.Data.SqlClient;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Install jobs
   /// </summary>
   public class InstallJob : TblOneIntPrimaryKey
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public InstallJob(SQLExecuter sqlExec) : base("InstallJobs", sqlExec)
      {
      }

      /// <summary>
      /// Add new install job to db
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="lastModified"></param>
      /// <param name="status"></param>
      /// <param name="installer"></param>
      /// <param name="description"></param>
      /// <param name="xmlData"></param>
      /// <returns></returns>
      public int AddJob(int boxId, DateTime lastModified, string status, string installer, string description, string xmlData)
      {
         int rowsAffected = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to add new job for '" + boxId + "' box.";
            // 1. Set SQL command
            string sql = "INSERT INTO dbo.InstallJobs (BoxId, LastModified, Status, Installer, Description, XMLData) VALUES(@BoxId, @LastModified, @Status, @Installer, @Description, @XMLData)";

            // 2. Add parameters to SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
            sqlExec.AddCommandParam("@Status", SqlDbType.VarChar, status);
            sqlExec.AddCommandParam("@Installer", SqlDbType.VarChar, installer);
            sqlExec.AddCommandParam("@LastModified", SqlDbType.DateTime, lastModified);
            sqlExec.AddCommandParam("@XMLData", SqlDbType.VarChar, xmlData);

            if (sqlExec.RequiredTransaction())
            {
               // 3. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 4. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This job already exists.");
         }
         return rowsAffected;
      }


   }
}
