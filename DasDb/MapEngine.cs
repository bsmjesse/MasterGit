using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;


namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to Map related tables.
	/// </summary>
	public class MapEngine : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public MapEngine(SQLExecuter sqlExec) : base ("",sqlExec)
		{
		}
		
		/// <summary>
		/// Retrieves box geocode engine id.
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [GeoCodeId],[Path]</returns>
		public DataSet GetBoxGeoCodeEngineInfo(int boxId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
            string sql = "SELECT ISNULL(vlfGeoCodeEngines.GeoCodeId, 0) AS GeoCodeId, CASE WHEN vlfGeoCodeEngines.Path IS NULL then '' ELSE RTRIM(vlfGeoCodeEngines.Path) END AS Path FROM vlfBox with (nolock) INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfGeoCodeEnginesGroup ON vlfOrganization.GeoCodeGroupId = vlfGeoCodeEnginesGroup.GeoCodeGroupId INNER JOIN vlfGeoCodeEnginesGroupAssignment ON vlfGeoCodeEnginesGroup.GeoCodeGroupId = vlfGeoCodeEnginesGroupAssignment.GeoCodeGroupId INNER JOIN vlfGeoCodeEngines ON vlfGeoCodeEnginesGroupAssignment.GeoCodeId = vlfGeoCodeEngines.GeoCodeId WHERE vlfBox.BoxId=" + boxId + " ORDER BY vlfGeoCodeEnginesGroupAssignment.Priority";
					
				// 2. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve box geocode info", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve box geocode info" + objException.Message);
			}
			return resultDataSet;
		}
        /// <summary>
        /// Retrieves box geocode engine id.
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet [MapId],[Path],[ExternalPath]</returns>
        public DataSet GetBoxMapEngineInfo(int boxId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
               string sql = "SELECT ISNULL(vlfMapEngines.MapId, 0) AS MapId, CASE WHEN vlfMapEngines.Path IS NULL then '' ELSE RTRIM(vlfMapEngines.Path) END AS Path, CASE WHEN vlfMapEngines.Path IS NULL then '' ELSE RTRIM(vlfMapEngines.ExternalPath) END AS ExternalPath FROM vlfBox with (nolock) INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfMapEnginesGroup ON vlfOrganization.MapGroupId = vlfMapEnginesGroup.MapGroupId INNER JOIN vlfMapEnginesGroupAssignment ON vlfMapEnginesGroup.MapGroupId = vlfMapEnginesGroupAssignment.MapGroupId INNER JOIN vlfMapEngines ON vlfMapEnginesGroupAssignment.MapId = vlfMapEngines.MapId WHERE vlfBox.BoxId =" + boxId + " ORDER BY vlfMapEnginesGroupAssignment.Priority";

                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve box map info", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve box map info" + objException.Message);
            }
            return resultDataSet;
        }
        /// <summary>
        /// Retrieves box GeoCode group Id
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>GeoCode group Id if exist, otherwise VLF.CLS.Def.Const.unassignedIntValue</returns>
        public int GetBoxGeoCodeGroupId(int boxId)
        {
            int retResult = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT vlfGeoCodeEnginesGroup.GeoCodeGroupId FROM vlfBox with (nolock) INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfGeoCodeEnginesGroup ON vlfOrganization.GeoCodeGroupId = vlfGeoCodeEnginesGroup.GeoCodeGroupId WHERE vlfBox.BoxId =" + boxId;

                // 2. Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    retResult = Convert.ToInt32(obj);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve box geocode group id", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve box geocode group id" + objException.Message);
            }
            return retResult;
        }
        /// <summary>
        /// Retrieves Organization GeoCode group Id
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>GeoCode group Id if exist, otherwise VLF.CLS.Def.Const.unassignedIntValue</returns>
       public int GetOrganizationGeoCodeGroupId(int organizationId)
        {
            int retResult = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT GeoCodeGroupId FROM vlfOrganization WHERE OrganizationId=" + organizationId;

                // 2. Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    retResult = Convert.ToInt32(obj);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve box geocode group id", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve box geocode group id" + objException.Message);
            }
            return retResult;
        }
		
		
		/// <summary>
		/// Retrieves user geocode path
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>Dataset [GeoCodeId],[Path]</returns>
        public DataSet GetUserGeoCodeEngineInfo(int userId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
                string sql = "SELECT ISNULL(vlfGeoCodeEngines.GeoCodeId, 0) AS GeoCodeId, CASE WHEN vlfGeoCodeEngines.Path IS NULL then '' ELSE RTRIM(vlfGeoCodeEngines.Path) END AS Path,Priority FROM vlfOrganization INNER JOIN vlfGeoCodeEnginesGroup ON vlfOrganization.GeoCodeGroupId = vlfGeoCodeEnginesGroup.GeoCodeGroupId INNER JOIN vlfGeoCodeEnginesGroupAssignment ON vlfGeoCodeEnginesGroup.GeoCodeGroupId = vlfGeoCodeEnginesGroupAssignment.GeoCodeGroupId INNER JOIN vlfGeoCodeEngines ON vlfGeoCodeEnginesGroupAssignment.GeoCodeId = vlfGeoCodeEngines.GeoCodeId INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId WHERE vlfUser.UserId=" + userId + " ORDER BY vlfGeoCodeEnginesGroupAssignment.Priority";
					
				// 2. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve user geocode info", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve user geocode info" + objException.Message);
			}
			return resultDataSet;
		}
        /// <summary>
        /// Retrieves user GeoCode group Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>GeoCode group Id if exist, otherwise VLF.CLS.Def.Const.unassignedIntValue</returns>
        public int GetUserGeoCodeGroupId(int userId)
        {
            int retResult = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT vlfGeoCodeEnginesGroup.GeoCodeGroupId FROM vlfOrganization INNER JOIN vlfGeoCodeEnginesGroup ON vlfOrganization.GeoCodeGroupId = vlfGeoCodeEnginesGroup.GeoCodeGroupId INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId WHERE vlfUser.UserId =" + userId;

                // 2. Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    retResult = Convert.ToInt32(obj);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve user geocode group id", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve user geocode group id" + objException.Message);
            }
            return retResult;
        }
        /// <summary>
        /// Retrieves user geocode path
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Dataset [MapId],[Path],[ExternalPath]</returns>
        public DataSet GetUserMapEngineInfo(int userId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT ISNULL(vlfMapEnginesGroupAssignment.MapId, 0) AS MapId, CASE WHEN vlfMapEngines.Path IS NULL then '' ELSE RTRIM(vlfMapEngines.Path) END AS Path, CASE WHEN vlfMapEngines.ExternalPath IS NULL then '' ELSE RTRIM(vlfMapEngines.ExternalPath) END AS ExternalPath,MapEngineName,Priority,vlfMapEngines.Description FROM vlfUser INNER JOIN vlfOrganization ON vlfUser.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfMapEnginesGroup ON vlfOrganization.MapGroupId = vlfMapEnginesGroup.MapGroupId INNER JOIN vlfMapEnginesGroupAssignment ON vlfMapEnginesGroup.MapGroupId = vlfMapEnginesGroupAssignment.MapGroupId INNER JOIN vlfMapEngines ON vlfMapEnginesGroupAssignment.MapId = vlfMapEngines.MapId WHERE vlfUser.UserId =" + userId + " ORDER BY vlfMapEnginesGroupAssignment.Priority";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve user map info", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve user map info" + objException.Message);
            }
            return resultDataSet;
        }
        
		#region Map Usage Report
		/// <summary>
		/// Delete Box Map Usage.
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="where"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteBoxMapUsage(int boxId,string where)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM vlfMapBoxUsage WHERE BoxId=" + boxId + where;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete box " + boxId + " map usage.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete box " + boxId + " map usage.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;	
		}
		/// <summary>
		/// Delete User Map Usage.
		/// </summary>
		/// <param name="userId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserMapUsage(int userId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM vlfMapUserUsage WHERE UserId=" + userId;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete user " + userId + " map usage.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete user " + userId + " map usage.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;	
		}
		/// <summary>
		/// Add map/user usage
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="mapTypeId"></param>
		/// <param name="usageYear"></param>
        /// <param name="usageMonth"></param>
        /// <param name="mapId"></param>
        /// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddMapUserUsage(int userId, short mapTypeId, short usageYear, short usageMonth, short mapId)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "DECLARE @MapId int DECLARE @UserId int DECLARE @Quantity int DECLARE @MapTypeId int DECLARE @UsageYear int DECLARE @UsageMonth int"+
				" SET @UserId=" + userId +
				" SET @Quantity=1" +
				" SET @MapTypeId=" + mapTypeId +
				" SET @UsageYear=" + usageYear +
				" SET @UsageMonth=" + usageMonth +
                " SET @MapId=" + mapId +
                " UPDATE vlfMapUserUsage SET Quantity=Quantity+@Quantity WHERE (MapId = @MapId) AND (UserId = @UserId) AND (MapTypeId = @MapTypeId) AND (UsageYear=@UsageYear) AND (UsageMonth=@UsageMonth) if @@ROWCOUNT=0	INSERT INTO vlfMapUserUsage (MapId,UserId,MapTypeId,UsageYear,UsageMonth,Quantity) VALUES (@MapId,@UserId,@MapTypeId,@UsageYear,@UsageMonth,@Quantity)";	

			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add map user " + userId + " usage.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add map user " + userId + " usage.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add map user " + userId + " usage.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The protocol type with cmd out type for current box already exists.");
			}
			return rowsAffected;
		}
		
		/// <summary>
		/// Add map/box usage
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="mapTypeId"></param>
		/// <param name="usageYear"></param>
        /// <param name="usageMonth"></param>
        /// <param name="mapId"></param>
        /// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddMapBoxUsage(int boxId, short mapTypeId, short usageYear, short usageMonth, short mapId)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "DECLARE @MapId int DECLARE @BoxId int DECLARE @Quantity int DECLARE @MapTypeId int DECLARE @UsageYear int DECLARE @UsageMonth int"+
				" SET @BoxId=" + boxId +
				" SET @Quantity=1"+
				" SET @MapTypeId=" + mapTypeId +
				" SET @UsageYear=" + usageYear +
				" SET @UsageMonth=" + usageMonth +
                " SET @MapId=" + mapId +
			    " UPDATE vlfMapBoxUsage SET Quantity=Quantity+@Quantity WHERE (MapId = @MapId) AND (BoxId = @BoxId) AND (MapTypeId = @MapTypeId) AND (UsageYear=@UsageYear) AND (UsageMonth=@UsageMonth) if @@ROWCOUNT=0	INSERT INTO vlfMapBoxUsage (MapId,BoxId,MapTypeId,UsageYear,UsageMonth,Quantity) VALUES (@MapId,@BoxId,@MapTypeId,@UsageYear,@UsageMonth,@Quantity)";
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add map box " + boxId + " usage.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add map box " + boxId + " usage.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add map box " + boxId + " usage.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The protocol type with cmd out type for current box already exists.");
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retrieves organization map usage info.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="mapId"></param>
		/// <param name="usageYear"></param>
		/// <param name="usageMonth"></param>
		/// <returns>DataSet [UserType],[UserName_BoxId],[Map],[StreetAddress],[Totals]</returns>
		public DataSet GetOrganizationMapUsageInfo(int organizationId,int mapId,short usageYear,short usageMonth)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "DECLARE @OrganizationId int DECLARE @MapId int DECLARE @UsageYear int DECLARE @UsageMonth int"+
					" SET @OrganizationId=" + organizationId +
					" SET @MapId=" + mapId +
					" SET @UsageYear=" + usageYear +
					" SET @UsageMonth=" + usageMonth +
					" CREATE TABLE #tmpMapUserUsage(UserId int,Quantity int) CREATE TABLE #tmpMapBoxUsage(BoxId int,Quantity int)"+
					// calculate totals for user
					" INSERT INTO #tmpMapUserUsage SELECT vlfMapUserUsage.UserId,Quantity FROM vlfMapUserUsage INNER JOIN vlfUser ON vlfMapUserUsage.UserId = vlfUser.UserId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.Map + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth"+
					" INSERT INTO #tmpMapUserUsage SELECT vlfMapUserUsage.UserId,Quantity FROM vlfMapUserUsage INNER JOIN vlfUser ON vlfMapUserUsage.UserId = vlfUser.UserId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.StreetAddress + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth"+
					" SELECT UserId,SUM(Quantity) AS UserTotals into #tmpMapUserUsageTotals from #tmpMapUserUsage group by UserId"+
					// separate quantity for map and street address
					" SELECT vlfMapUserUsage.UserId,Quantity AS Map into #tmpMapUserMapUsage FROM vlfMapUserUsage INNER JOIN vlfUser ON vlfMapUserUsage.UserId = vlfUser.UserId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.Map + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth"+
					" SELECT vlfMapUserUsage.UserId,Quantity AS StreetAddress into #tmpMapUserStreetAddressUsage FROM vlfMapUserUsage INNER JOIN vlfUser ON vlfMapUserUsage.UserId = vlfUser.UserId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.StreetAddress + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth"+
					// calculate totals for box
                    " INSERT INTO #tmpMapBoxUsage SELECT vlfMapBoxUsage.BoxId,Quantity FROM vlfMapBoxUsage INNER JOIN vlfBox with (nolock) ON vlfMapBoxUsage.BoxId = vlfBox.BoxId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.Map + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth" +
                    " INSERT INTO #tmpMapBoxUsage SELECT vlfMapBoxUsage.BoxId,Quantity FROM vlfMapBoxUsage INNER JOIN vlfBox with (nolock) ON vlfMapBoxUsage.BoxId = vlfBox.BoxId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.StreetAddress + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth" +
					" SELECT BoxId,SUM(Quantity) AS BoxTotals into #tmpMapBoxUsageTotals from #tmpMapBoxUsage group by BoxId"+
					// separate quantity for map and street address
                    " SELECT vlfMapBoxUsage.BoxId,Quantity AS Map into #tmpMapBoxMapUsage FROM vlfMapBoxUsage INNER JOIN vlfBox with (nolock) ON vlfMapBoxUsage.BoxId = vlfBox.BoxId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.Map + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth" +
					" SELECT vlfMapBoxUsage.BoxId,Quantity AS StreetAddress into #tmpMapBoxStreetAddressUsage FROM vlfMapBoxUsage INNER JOIN vlfBox ON vlfMapBoxUsage.BoxId = vlfBox.BoxId WHERE OrganizationId=@OrganizationId AND MapId=@MapId AND MapTypeId=" + (short)VLF.CLS.Def.Enums.MapTypes.StreetAddress + " AND UsageYear=@UsageYear AND UsageMonth=@UsageMonth"+

					" SELECT DISTINCT 1 AS UserType,RTRIM(vlfUser.UserName) AS UserName_BoxId,isnull(#tmpMapUserMapUsage.Map,0) AS Map,isnull(#tmpMapUserStreetAddressUsage.StreetAddress,0) AS StreetAddress,isnull(UserTotals,0) AS Totals FROM vlfMapUserUsage INNER JOIN vlfUser ON vlfMapUserUsage.UserId = vlfUser.UserId LEFT JOIN #tmpMapUserUsageTotals ON vlfUser.UserId = #tmpMapUserUsageTotals.UserId LEFT JOIN #tmpMapUserMapUsage ON vlfUser.UserId = #tmpMapUserMapUsage.UserId LEFT JOIN #tmpMapUserStreetAddressUsage ON vlfUser.UserId = #tmpMapUserStreetAddressUsage.UserId WHERE OrganizationId=@OrganizationId"+
					" UNION SELECT DISTINCT 2 AS UserType,convert(nvarchar,vlfBox.BoxId) AS UserName_BoxId,isnull(#tmpMapBoxMapUsage.Map,0) AS Map,isnull(#tmpMapBoxStreetAddressUsage.StreetAddress,0) AS StreetAddress,isnull(BoxTotals,0) AS Totals FROM vlfMapBoxUsage INNER JOIN vlfBox ON vlfMapBoxUsage.BoxId = vlfBox.BoxId LEFT JOIN #tmpMapBoxUsageTotals ON vlfBox.BoxId = #tmpMapBoxUsageTotals.BoxId LEFT JOIN #tmpMapBoxMapUsage ON vlfBox.BoxId = #tmpMapBoxMapUsage.BoxId LEFT JOIN #tmpMapBoxStreetAddressUsage ON vlfBox.BoxId = #tmpMapBoxStreetAddressUsage.BoxId WHERE OrganizationId=@OrganizationId"+

					// cleanup all temporary tables
					" DROP TABLE #tmpMapUserUsage DROP TABLE #tmpMapUserMapUsage DROP TABLE #tmpMapUserStreetAddressUsage DROP TABLE #tmpMapUserUsageTotals DROP TABLE #tmpMapBoxUsage DROP TABLE #tmpMapBoxMapUsage DROP TABLE #tmpMapBoxStreetAddressUsage DROP TABLE #tmpMapBoxUsageTotals"; 
					
				// 2. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve organization map usage info", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve organization map usage info" + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Retrieves map types.
		/// </summary>
		/// <returns>DataSet [MapTypeId],[MapTypeName]</returns>
		public DataSet GetMapTypes()
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT * FROM vlfMapType";
					
				// 2. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve organization boxes map usage info", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve organization boxes map usage info" + objException.Message);
			}
			return resultDataSet;
		}
		#endregion
	}
}
