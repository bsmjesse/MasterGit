using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfFleetUsers table.
	/// </summary>
	public class FleetUsers : TblConnect2TblsWithoutRules
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public FleetUsers(SQLExecuter sqlExec) : base ("vlfFleetUsers",sqlExec)
		{
		}
		/// <summary>
		/// Add user to fleet
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="userId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddUserToFleet(int fleetId,int userId)
		{
			AddNewRow("FleetId",fleetId,"UserId",userId,"user to fleet");
		}	
		/// <summary>
		/// Delete exist user from all fleets
		/// </summary>
		/// <param name="userId"></param> 
		/// <exception cref="DASException">Thrown DASAppResultNotFoundException if user does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>ows affected</returns>
		public int DeleteUserFromAllFleets(int userId)
		{
			return DeleteRowsByIntField("UserId",userId, "user fleets");
		}
		/// <summary>
		/// Delete all users from the fleet and fleet
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllUsersFromFleet(int fleetId)
		{
			return DeleteRowsByIntField("FleetId",fleetId, "fleet");
		}

		/// <summary>
		/// Delete exist user from the fleet
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <param name="userId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>rows affected</returns>
		public int DeleteUserFromFleet(int fleetId,int userId)
		{
			return DeleteRowsByFields("FleetId",fleetId,"UserId",userId,"fleet user");	
		}
		/// <summary>
		/// Retrieves record count from vlfFleetUsers table.
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount();
			}
		}
		/// <summary>
		/// Returns fleets information by user id. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFleetsInfoByUserId(int userId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT DISTINCT OrganizationName,vlfFleet.FleetId,FleetName,vlfFleet.Description,ISNULL(FleetType,'') as FleetType,ISNULL(NodeCode,'') as NodeCode " +
					" FROM  vlfFleetUsers,vlfOrganization,vlfFleet" +
					" WHERE ( vlfFleetUsers.UserId=" + userId  + ")" +
					" AND (vlfFleet.FleetId=vlfFleetUsers.FleetId)" +
					" AND (vlfFleet.OrganizationId=vlfOrganization.OrganizationId)" +
					" ORDER BY FleetName";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve fleets info by user id " + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve fleets info by user id " + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

		/// <summary>
		/// Returns unassigned fleets information to current user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUnassignedFleetsInfoByUserId(int userId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DISTINCT vlfFleet.FleetId, vlfOrganization.OrganizationName, vlfFleet.FleetName, vlfFleet.Description"+
							" FROM vlfUser INNER JOIN vlfOrganization ON vlfUser.OrganizationId=vlfOrganization.OrganizationId"+
							" INNER JOIN vlfFleet ON vlfOrganization.OrganizationId=vlfFleet.OrganizationId"+
							" WHERE vlfUser.UserId =" + userId + 
							" AND vlfFleet.FleetId NOT IN (SELECT vlfFleet.FleetId"+
														" FROM  vlfFleetUsers,vlfFleet"+
														" WHERE vlfFleetUsers.UserId=" + userId + 
														" AND vlfFleet.FleetId=vlfFleetUsers.FleetId)"+
							" ORDER BY vlfFleet.FleetName";

				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve all unassigned fleets info to user id " + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve all unassigned fleets info to user id " + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Returns users info by fleet id. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],[Contact],[OrganizationId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUsersInfoByFleetId(int fleetId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfUser.UserId,UserName,Password,DriverLicense,FirstName,LastName,Contact,OrganizationName" +
					" FROM vlfUser,vlfOrganization,vlfFleetUsers,vlfFleet,vlfPersonInfo" +  
					" WHERE vlfFleet.FleetId=" + fleetId +
					" AND vlfFleet.FleetId=vlfFleetUsers.FleetId" +
					" AND vlfFleetUsers.UserId=vlfUser.UserId" +
					" AND vlfUser.OrganizationId=vlfOrganization.OrganizationId" +
					" AND vlfUser.PersonId=vlfPersonInfo.PersonId" +
                    " AND vlfUser.UserId NOT IN (SELECT vlfUser.UserId FROM vlfUser INNER JOIN vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId INNER JOIN vlfFleet ON vlfUser.OrganizationId = vlfFleet.OrganizationId WHERE (vlfUserGroupAssignment.UserGroupId = 1 or vlfUserGroupAssignment.UserGroupId = 14) AND vlfFleet.FleetId =" + fleetId + ")" +
					" AND (ExpiredDate IS NULL OR ExpiredDate > GETDATE()) ORDER BY LastName,FirstName";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve users info by fleet id=" + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve users info by fleet id=" + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retieves all users (except HGIAdmin user group) unassigned to the fleet .
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],[Contact],[OrganizationId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUnassignedUsersInfoByFleetId(int fleetId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DISTINCT vlfUser.UserId,UserName,Password,DriverLicense,FirstName,LastName,Country,vlfOrganization.OrganizationId"+
							" FROM vlfFleet INNER JOIN vlfOrganization ON vlfFleet.OrganizationId = vlfOrganization.OrganizationId"+
							" INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId"+
							" INNER JOIN vlfPersonInfo ON vlfUser.PersonId = vlfPersonInfo.PersonId"+
							" WHERE vlfFleet.FleetId=" + fleetId +
							" AND vlfUser.UserId NOT IN (SELECT UserId FROM vlfFleetUsers INNER JOIN vlfFleet ON vlfFleetUsers.FleetId=vlfFleet.FleetId WHERE vlfFleet.FleetId=" + fleetId + ")"+
                            " AND vlfUser.UserId NOT IN (SELECT vlfUser.UserId FROM vlfUser INNER JOIN vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId INNER JOIN vlfFleet ON vlfUser.OrganizationId = vlfFleet.OrganizationId WHERE (vlfUserGroupAssignment.UserGroupId = 1 or vlfUserGroupAssignment.UserGroupId = 14) AND vlfFleet.FleetId =" + fleetId + ")" +
							" AND (ExpiredDate IS NULL OR ExpiredDate > GETDATE()) ORDER BY LastName,FirstName";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve unassigned users info by fleet id=" + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve unassigned users info by fleet id=" + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
	}
}
