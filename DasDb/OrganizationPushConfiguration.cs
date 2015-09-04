/** \file   OrganizationPushConfiguration.cs
 *  \brief  the assumption is that the push is used at the organization level only to send out traffic from mobile
 */ 
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;

namespace VLF.DAS.DB
{
   public class OrganizationPushConfiguration : TblGenInterfaces
   {
      [Flags]
      public enum TypeOfMessages : long
      {
         FROM_MOBILE          = 1,       // CMFIN
         TO_MOBILE            = 2,       // CMFOUT
         ALARMS               = 4,       // CMFAlarm
         MDT_MESSAGES         = 8,       // CMFTextMessage
         MDT_THIRD_PARTY      = 0x10,    // CMFCannedMessage 
         SERVICE_NOTIFICATION = 0x20,    // CMFNotification 
         RULES_VIOLATIONS     = 0x40     // CMFRuleViolation 
      }

      /// <summary>
      /// 
      /// </summary>
      /// <comment>
      ///   THE VALUES HERE HAVE TO BE IDENTIC WITH THE VALUES IN vlfPushType table
      /// </comment>
      public enum PushType : int
      {
         NULL   = 0,      ///< by default is in the database
         FTP    = 1,      ///<   
         SMTP   = 2,
         HTTP   = 3,
         HTTPS  = 4,
         TCP_IP = 5,
         WEB_SERVICE = 6      ///< this needs an extra piece of code to put the message in their format
      }

      internal class PushConfiguration
      {
         static string[] ConfigurationTemplate = {
            string.Empty,                                                   // for NULL
            string.Format("url={0};msg={1};uname={2};pwd={3};"),             // ftp
            string.Format("email={0};msg={1};subj={2};uname={3};pwd={4};"),  // smtp
            string.Format("url={0};msg={1};uname={2};pwd={3};"),             // http
            string.Format("url={0};msg={1};uname={2};pwd={3};"),             // https
            string.Format("ip={0};port={1};msg={2};"),                      // tcp_ip
            string.Format("url={0};msg={1};uname={2};pwd={3};")      // web service
      };

         PushType _type;
         TypeOfMessages _typeOfMessages;

          /*   \brief   a JSON string where all what is needed is stored in a list of (key,value) items
          **/ 
         string _configuration;           

         IPushConverter _packetConverter;
      }

      public interface IPushConverter
      {
         byte[] ToCustomFormat(object obj);        // obj can be any type of packet described by TypeOfMessages
      }

      /// <summary>
      ///      Constructor for vlfOrganizationPushConfiguration which has the schema
      ///      PushId      [long]      -- autoincrement
      ///      OrgId       [int]       -- organization Id
      ///      PushType    [int]       -- see PushType enumeration above
      ///      Configuration [varchar(256)]  -- all parameters amalgamated in a string
      ///      TypeOfMessages [long]         -- if you want to push other type of messages other than CMFIn
      ///      FormatAssembly    [varchar(256)]    -- used to format from CMFIn or notifications or alarms or service maintenance to the third party 
      /// </summary>
      /// <param name="sqlExec"></param>
      public OrganizationPushConfiguration(SQLExecuter sqlExec)
         : base("vlfOrganizationPushConfiguration", sqlExec)
      {

      }

      /// <summary>
      ///      it verifies the checksum for all rows is the same with the one sent as parameter 
      /// </summary>
      /// <param name="chksum"></param>
      /// <returns></returns>
      public bool HasTableChanged(long chksum)
      {
         return chksum != base.GetTableSignature("dbo.vlfOrganizationPushConfiguration", string.Empty);
      }

      public int AddPush(int orgId, int type, string configuration)
      {
         return AddPush(orgId, type, configuration, string.Empty);
      }

      public int AddPush(int orgId, int type, string configuration, string assembly)
      {
         string prefixMsg = string.Format("Unable to add new push config: orgId={0}, pushType={1} config={2} assembly={3}",
                              orgId, ((PushType)type).ToString(), configuration, assembly);
         int rowsAffected = 0 ;

         try
         {
           rowsAffected =  base.AddRow("(OrgId, PushTypeId, Configuration, FormatAssembly) VALUES(@orgId, @pushType, @configuration, @assembly)",
                                     new SqlParameter("@orgId", orgId),
                                     new SqlParameter("@pushType", type),
                                     new SqlParameter("@configuration", configuration),
                                     new SqlParameter("@assembly", assembly));
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

         return rowsAffected;
      }

      public int UpdatePush(long pushConfigId, string columnName, string configuration)
      {
         string prefixMsg = string.Format("Unable to update push config: pushId={0}, columnType={1} newValue={2}",
                              pushConfigId, columnName, configuration);
         int rowsAffected = 0 ;

         try
         {
            rowsAffected = base.UpdateRow("SET " +  columnName + " = @configuration WHERE PushId=@pushId",
                                     new SqlParameter("@configuration", configuration),
                                     new SqlParameter("@pushId", pushConfigId)) ;
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

         return rowsAffected;

      }

      public int DeletePush(long pushConfigId)
      {
         string prefixMsg = string.Format("Unable to delete push: pushId={0}", pushConfigId);
         int rowsAffected = 0; 
         try
         {
            rowsAffected = base.DeleteRowsByField("PushId", pushConfigId);
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

         return rowsAffected;

      }

      public int DeletePush4Organization(int orgId)
      {
         string prefixMsg = string.Format("Unable to delete push for orgId={0}", orgId);
         int rowsAffected = 0; 

         try
         {
            rowsAffected = base.DeleteRowsByIntField("OrgId", orgId, prefixMsg);
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

         return rowsAffected;

      }

      public DataSet GetOrganizationPushConfiguration(int OrgId)
       {
           DataSet sqlDataSet = null;
           string prefixMsg = string.Format("Unable to GetOrganizationPushConfiguration orgId={0}", OrgId);
           try
           {
               sqlExec.ClearCommandParameters();
               //Prepares SQL statement
               sqlExec.AddCommandParam("@OrgId", SqlDbType.Int, OrgId);
               
               //Executes SQL statement
               sqlDataSet = sqlExec.SPExecuteDataset("sp_OrganizationPushConfiguration");
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
           return sqlDataSet;
       }

      public DataSet GetUnassignedPushTypesByOrg(int OrgId)
       {
           
           DataSet sqlDataSet = null;

           try
           {
               // Prepares SQL statement
               string sql = "select PushTypeId,PushName from vlfPushType" +
                      " where PushTypeId not in (Select PushTypeId from vlfOrganizationPushConfiguration " +
                      " where OrgId=" + OrgId+")";
               // Executes SQL statement
               sqlDataSet =sqlExec.SQLExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve PushTypes by OrgId=" + OrgId + ". ";
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve PushTypes by OrgId=" + OrgId + ". ";
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return sqlDataSet;
       }
   }
}
