using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    public class SAStation : TblOneIntPrimaryKey
    {
 		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
        public SAStation(SQLExecuter sqlExec)
            : base("saStation", sqlExec)
		{
		}

        private const string AddStation_SQL = "INSERT INTO {0}(OrganizationId,Name,LandmarkId,TypeId,StationNumber,Description,LastEditedDatetime,LastEditedUserId,ContractName,PhoneNumber,FaxNumber,Address, EmailAddress) VALUES (@OrganizationId,@Name,@LandmarkId,@TypeId,@StationNumber,@Description,@LastEditedDatetime,@LastEditedUserId,@ContractName,@PhoneNumber,@FaxNumber,@Address,@EmailAddress)";
        /// <summary>
        /// Add new Station.
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddStation(int organizationId, string stationName, long landmarkId, int typeId,
           string stationNumber, string description, DateTime editDatetime,
           int userId, string contractName, string phoneNumber, string faxNumber, string address, string emailAddress)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(AddStation_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@Name", SqlDbType.Char, stationName);
                sqlExec.AddCommandParam("@LandmarkId", SqlDbType.BigInt, landmarkId);
                sqlExec.AddCommandParam("@TypeId", SqlDbType.Int, typeId);
                sqlExec.AddCommandParam("@StationNumber", SqlDbType.Char, stationNumber);
                if (string.IsNullOrEmpty(description))
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
                sqlExec.AddCommandParam("@LastEditedDatetime", SqlDbType.DateTime, editDatetime);
                if (userId <= 0)
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, userId);
                if (string.IsNullOrEmpty(contractName))
                    sqlExec.AddCommandParam("@ContractName", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContractName", SqlDbType.Char, contractName);
                if (string.IsNullOrEmpty(phoneNumber))
                    sqlExec.AddCommandParam("@PhoneNumber", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@PhoneNumber", SqlDbType.Char, phoneNumber);
                if (string.IsNullOrEmpty(faxNumber))
                    sqlExec.AddCommandParam("@FaxNumber", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@FaxNumber", SqlDbType.Char, faxNumber);
                if (string.IsNullOrEmpty(address))
                    sqlExec.AddCommandParam("@Address", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Address", SqlDbType.Char, address);

                if (string.IsNullOrEmpty(emailAddress))
                    sqlExec.AddCommandParam("@EmailAddress", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@EmailAddress", SqlDbType.Char, emailAddress);
                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId +
                   " StationName=" + stationName + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId +
                   " StationName=" + stationName + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string UpdateStation_SQL = "Update {0} set Name = @Name,LandmarkId = @LandmarkId, TypeId=@TypeId, StationNumber=@StationNumber, Description=@Description,LastEditedDatetime=@LastEditedDatetime,LastEditedUserId=@LastEditedUserId,ContractName=@ContractName,PhoneNumber=@PhoneNumber,FaxNumber=@FaxNumber,Address=@Address,EmailAddress=@EmailAddress where stationId = @stationId";
        public void UpdateStation(int stationId, string stationName, long landmarkId, int typeId,
            string stationNumber, string description, DateTime editDatetime,
            int userId, string contractName, string phoneNumber, string faxNumber, string address, string emailAddress)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(UpdateStation_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@stationId", SqlDbType.Int, stationId);
                sqlExec.AddCommandParam("@Name", SqlDbType.Char, stationName);
                sqlExec.AddCommandParam("@LandmarkId", SqlDbType.BigInt, landmarkId);
                sqlExec.AddCommandParam("@TypeId", SqlDbType.Int, typeId);
                sqlExec.AddCommandParam("@StationNumber", SqlDbType.Char, stationNumber);
                if (string.IsNullOrEmpty(description))
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
                sqlExec.AddCommandParam("@LastEditedDatetime", SqlDbType.DateTime, editDatetime);
                if (userId <= 0)
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, userId);
                if (string.IsNullOrEmpty(contractName))
                    sqlExec.AddCommandParam("@ContractName", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContractName", SqlDbType.Char, contractName);
                if (string.IsNullOrEmpty(phoneNumber))
                    sqlExec.AddCommandParam("@PhoneNumber", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@PhoneNumber", SqlDbType.Char, phoneNumber);
                if (string.IsNullOrEmpty(faxNumber))
                    sqlExec.AddCommandParam("@FaxNumber", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@FaxNumber", SqlDbType.Char, faxNumber);
                if (string.IsNullOrEmpty(address))
                    sqlExec.AddCommandParam("@Address", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Address", SqlDbType.Char, address);
                if (string.IsNullOrEmpty(emailAddress))
                    sqlExec.AddCommandParam("@EmailAddress", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@EmailAddress", SqlDbType.Char, emailAddress);

                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update Station to stationId '" + stationId +
                   " StationName=" + stationName + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update Station to stationId '" + stationId +
                   " StationName=" + stationName + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string GetStationsByOrganizationId_SQL = "select StationId, OrganizationId, Name, LandmarkId, TypeId, StationNumber, Description, LastEditedDatetime, LastEditedUserId, ContractName, PhoneNumber, FaxNumber, Address from {0} where OrganizationId = @OrganizationId";
        /// <summary>
        /// Retrieves Stations by organization id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetStationsByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetStationsByOrganizationId_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        private const string GetStationsById_SQL = "select StationId, OrganizationId, Name, LandmarkId, TypeId, StationNumber, Description, LastEditedDatetime, LastEditedUserId, ContractName, PhoneNumber, FaxNumber, Address, EmailAddress from {0} where StationId = @StationId";
        /// <summary>
        /// Retrieves Stations by organization id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetStationById(int stationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetStationsById_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@StationId", SqlDbType.Int, stationId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by stationId=" + stationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by stationId=" + stationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int DeleteStationById(int stationId)
        {
            return DeleteRowsByIntField("StationId", stationId, "Schedule Adherence Station");
        }
    }
}
