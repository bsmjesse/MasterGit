using System;
using VLF.ERR;
using VLF.DAS.DB;
using System.Data;			// for DataSet
using System.Collections;
using System.Text;	      // for ArrayList


namespace VLF.DAS.Logic
{
   /// <summary>
   ///      this is the class describing the push methods used at the organization level 
   /// </summary>
   public class PushConfiguration  : Das
	{
      private VLF.DAS.DB.OrganizationPushConfiguration pushConfig = null;


      #region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
      public PushConfiguration(string connectionString)
         : base(connectionString)
		{
         pushConfig = new VLF.DAS.DB.OrganizationPushConfiguration(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region Public Interfaces


      public int AddPush(int orgId, int type, string configuration)
      {
         return pushConfig.AddPush(orgId, type, configuration);
      }

      public int UpdatePushAssembly(long pushConfigId, string configuration)
      {
         return pushConfig.UpdatePush(pushConfigId, "FormatAssembly", configuration);
      }

      public int UpdatePushConfiguration(long pushConfigId, string configuration)
      {
         return pushConfig.UpdatePush(pushConfigId, "Configuration", configuration);
      }

      public int DeletePush(long pushConfigId)
      {
         return pushConfig.DeletePush(pushConfigId);
      }

      public int DeletePush4Organization(int orgid)
      {
         return pushConfig.DeletePush4Organization(orgid);
      }

      /// <summary>
      ///      it returns  [pushId] [pushType] [configuration] [typeOfMessages] [assembly]
      /// </summary>
      /// <param name="pushId">
      ///      if pushId is -1, bring all rows 
      /// </param>
      /// <returns></returns>
      public DataSet GetPushConfiguration(long pushId)
      {
         return
            (pushId != -1) ? pushConfig.GetRowsByFilter("WHERE pushid = " + pushId) :
                             pushConfig.GetAllRecords() ; 
      }

      /// <summary>
      ///      returns [BoxId] [OrganizationId]
      /// </summary>
      /// <param name="orgId"></param>
      /// <returns></returns>
      public DataSet GetAllBoxesWithPush(int orgId)
      {
         string sql = (-1 == orgId ) ? 
                        @"SELECT DISTINCT BoxId, OrganizationId from dbo.vlfBox 
                           INNER JOIN dbo.vlfOrganizationPushConfiguration
                              ON vlfOrganizationPushConfiguration.OrgId = vlfBox.OrganizationId" : 
                        @"SELECT DISTINCT BoxId, OrganizationId from dbo.vlfBox 
                           INNER JOIN dbo.vlfOrganizationPushConfiguration
                              ON vlfOrganizationPushConfiguration.OrgId = vlfBox.OrganizationId where vlfOrganizationPushConfiguration = " + orgId;

         return pushConfig.GetRowsBySql(sql, null);
      }
      /// <summary>
      ///      Get PushCon figuration By Organization
      /// </summary>
      /// <param name="orgId"></param>
      /// <returns></returns>
      public DataSet GetPushConfigurationByOrg(int orgId)
      {
          return pushConfig.GetOrganizationPushConfiguration(orgId); 
      }

      /// <summary>
      ///      Get Unassigned Push Types for Organization
      /// </summary>
      /// <param name="orgId"></param>
      /// <returns></returns>
      public DataSet GetUnassignedPushTypesByOrg(int orgId)
      {
           return pushConfig.GetUnassignedPushTypesByOrg(orgId); 
      }

      /// <summary>
      ///      it prevents reloading the configuration for a specific query
      /// </summary>
      /// <param name="chksum"></param>
      /// <returns></returns>
      public bool HasTableChanged(long chksum)
      {
         return pushConfig.HasTableChanged(chksum);
      }

      /// <summary>
      ///      returns the checksum for the filter 
      /// </summary>
      /// <param name="filter"></param>
      /// <returns></returns>
      public long GetQueryChecksum(string filter)
      {
         return pushConfig.GetTableSignature(pushConfig.TableName, filter);
      }

      #endregion
   }
}
