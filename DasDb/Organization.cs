using System;
using System.Collections;	    //for ArrayList
using System.Data;			    // for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to vlfOrganization table.
    /// </summary>
    public class Organization : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Organization(SQLExecuter sqlExec)
            : base("vlfOrganization", sqlExec)
        {
        }

        /// <summary>
        /// Add new organization.
        /// </summary>
        /// <returns>int next organization id</returns>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if organization alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddOrganization(string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
            // 1. validateseters
            if ((organizationName == VLF.CLS.Def.Const.unassignedStrValue) ||
                (organizationName == "") ||
                (contact == VLF.CLS.Def.Const.unassignedStrValue) ||
                (address == VLF.CLS.Def.Const.unassignedStrValue) ||
                (description == VLF.CLS.Def.Const.unassignedStrValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: OrganizationName=" +
                    organizationName + " Contact=" + contact + " Address=" + address + " Description=" + description);
            }

            int rowsAffected = 0;
            // 2. Get next availible index
            organizationId = (int)GetMaxRecordIndex("OrganizationId") + 1;

            // 3. Prepares SQL statement
            string sql = string.Format("INSERT INTO " + tableName
                + " (OrganizationId, OrganizationName, Contact, Address, Description, LogoName, HomePageName, MapGroupId, GeoCodeGroupId)"
                + " VALUES ( {0}, '{1}', '{2}', '{3}', '{4}','{5}','{6}',{7},{8})",
                organizationId,
                organizationName.Trim().Replace("'", "''"),
                contact.Trim().Replace("'", "''"),
                address.Trim().Replace("'", "''"),
                description.Trim().Replace("'", "''"),
                logoName.Trim(), homePageName.Trim(), mapGroupId, geoCodeGroupId);
            try
            {
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new organization.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new organization.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new organization.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This organization already exists.");
            }
            return organizationId;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="superOrganizationId"> if it is not specified, then the extra 2 fields should be 
        ///             the ID for BSM 
        /// </param>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        /// <returns></returns>
        public int AddOrganization(int superOrganizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
            // 1. validateseters
            if ((organizationName == VLF.CLS.Def.Const.unassignedStrValue) ||
               (organizationName == "") ||
               (contact == VLF.CLS.Def.Const.unassignedStrValue) ||
               (address == VLF.CLS.Def.Const.unassignedStrValue) ||
               (description == VLF.CLS.Def.Const.unassignedStrValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: OrganizationName=" +
                   organizationName + " Contact=" + contact + " Address=" + address + " Description=" + description);
            }

            int rowsAffected = 0;
            // 2. Get next availible index
            organizationId = (int)GetMaxRecordIndex("OrganizationId") + 1;

            // 3. Prepares SQL statement
            string sql = string.Format("INSERT INTO " + tableName
                   + " (OrganizationId, OrganizationName, Contact, Address, Description, LogoName, HomePageName, MapGroupId, GeoCodeGroupId, SuperOrganizationId)"
               + " VALUES ( {0}, '{1}', '{2}', '{3}', '{4}','{5}','{6}',{7},{8}, {9})",
               organizationId,
               organizationName.Trim().Replace("'", "''"),
                   contact.Trim().Replace("'", "''"),
                   address.Trim().Replace("'", "''"),
                   description.Trim().Replace("'", "''"),
                   logoName.Trim(), homePageName.Trim(), mapGroupId, geoCodeGroupId,
                   superOrganizationId);
            try
            {
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new organization.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new organization.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new organization.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This organization already exists.");
            }
            return organizationId;
        }


        /// <summary>
        /// Update organization information.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="superOrganizationId"> if it is not specified, then the extra 2 fields should be 
        /// <param name="organizationId"></param>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if organization does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateInfo(int superOrganizationId, int organizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            // 1. validateseters
            if ((organizationId == VLF.CLS.Def.Const.unassignedIntValue) ||
                (organizationName == VLF.CLS.Def.Const.unassignedStrValue) ||
                (contact == VLF.CLS.Def.Const.unassignedStrValue) ||
                (address == VLF.CLS.Def.Const.unassignedStrValue) ||
                (description == VLF.CLS.Def.Const.unassignedStrValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: OrganizationId=" +
                    organizationId +
                    " OrganizationName=" + organizationName +
                    " Contact=" + contact +
                    " Address=" + address +
                    " Description=" + description);
            }
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE " + tableName +
                " SET SuperOrganizationId=" + superOrganizationId +
                ", OrganizationName='" + organizationName.Trim().Replace("'", "''") + "'" +
                ", Contact='" + contact.Trim().Replace("'", "''") + "'" +
                ", Address='" + address.Trim().Replace("'", "''") + "'" +
                ", Description='" + description.Trim().Replace("'", "''") + "'" +
                ", LogoName='" + logoName.Trim() + "'" +
                ", HomePageName='" + homePageName.Trim() + "'" +
                ", MapGroupId=" + mapGroupId +
                ", GeoCodeGroupId=" + geoCodeGroupId +
                " WHERE OrganizationId=" + organizationId;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update organization id " + organizationId + " name" + organizationName + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update organization id " + organizationId + " name" + organizationName + " .";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update organization id " + organizationId + " name" + organizationName + " .";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
        }
        /// <summary>
        /// Update organization information.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="organizationId"></param>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if organization does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateInfo(int organizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            // 1. validateseters
            if ((organizationId == VLF.CLS.Def.Const.unassignedIntValue) ||
                (organizationName == VLF.CLS.Def.Const.unassignedStrValue) ||
                (contact == VLF.CLS.Def.Const.unassignedStrValue) ||
                (address == VLF.CLS.Def.Const.unassignedStrValue) ||
                (description == VLF.CLS.Def.Const.unassignedStrValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: OrganizationId=" +
                    organizationId +
                    " OrganizationName=" + organizationName +
                    " Contact=" + contact +
                    " Address=" + address +
                    " Description=" + description);
            }
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE " + tableName +
                " SET OrganizationName='" + organizationName.Trim().Replace("'", "''") + "'" +
                ", Contact='" + contact.Trim().Replace("'", "''") + "'" +
                ", Address='" + address.Trim().Replace("'", "''") + "'" +
                ", Description='" + description.Trim().Replace("'", "''") + "'" +
                ", LogoName='" + logoName.Trim() + "'" +
                ", HomePageName='" + homePageName.Trim() + "'" +
                ", MapGroupId=" + mapGroupId +
                ", GeoCodeGroupId=" + geoCodeGroupId +
                " WHERE OrganizationId=" + organizationId;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update organization id " + organizationId + " name" + organizationName + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update organization id " + organizationId + " name" + organizationName + " .";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update organization id " + organizationId + " name" + organizationName + " .";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
        }


        /// <summary>
        /// Update Organization History Access Range
        /// </summary>
        
        
        
        public void UpdateOrganizationHistoryAccess(int organizationId, Int16  HistoryInterval)
        {
            
            //Prepares SQL statement
            string sql = "UPDATE vlfOrganization SET HistoryAvailableMonth="+HistoryInterval+" WHERE OrganizationId=" + organizationId;
            int rowsAffected = 0;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update organization history id " + organizationId ;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update organization history id " + organizationId ;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update organization history id " + organizationId ;
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
        }

        /// <summary>
        /// Deletes existing organization.
        /// </summary>
        /// <returns>rows affected</returns>
        /// <param name="organizationName"></param> 
        /// <exception cref="DASAppResultNotFoundException">Thrown if organization does not exist</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteOrganizationByOrganizationName(string organizationName)
        {
            return DeleteRowsByStrField("OrganizationName", organizationName, "organization name");
        }

        /// <summary>
        /// Deletes existing organization.
        /// </summary>
        /// <returns>rows affected</returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASAppResultNotFoundException">Thrown if organization id does not exist</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteOrganizationByOrganizationId(int organizationId)
        {
            return DeleteRowsByIntField("OrganizationId", organizationId, "organization id");
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <returns>DataSet</returns>
        /// <param name="organizationId"></param> 
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapGroupName],[MapPriority],[MapId],[MapEngineName],[MapPath],[MapExternalPath],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationInfoByOrganizationId(int organizationId)
        {
            return GetOrganizationInfoBy("OrganizationId", organizationId);
        }

        /// <summary>
        /// Retrieves all license plates for specific organization
        /// </summary>
        /// <returns>DataSet [LicensePlate]</returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLicensePlatesByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT LicensePlate " +
                    " FROM vlfFleet,vlfVehicleAssignment,vlfFleetVehicles" +
                    " WHERE vlfFleet.OrganizationId=" + organizationId +
                    " AND vlfFleet.FleetId=vlfFleetVehicles.FleetId" +
                    " AND vlfFleetVehicles.VehicleId=vlfVehicleAssignment.VehicleId";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve license plates by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve license plates by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Gets the organization ID owning the given fleet ID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public int GetOrganizationIdByFleetId(int fleetId)
        {
            int orgId = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // Prepares SQL statement
                string sql = "SELECT OrganizationId FROM vlfFleet WHERE FleetId=" + fleetId;
                // Executes SQL statement
                orgId = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization id by fleet id=" + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization id by fleet id=" + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return orgId;
        }

        /// <summary>
        /// Gets the organization name owning the given fleet ID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public String GetOrganizationNameByFleetId(int fleetId)
        {
            String orgName = VLF.CLS.Def.Const.unassignedStrValue;
            try
            {
                // Prepares SQL statement
                string sql = "SELECT OrganizationName FROM vlfFleet f, vlfOrganization o" +
                    " WHERE f.FleetId=" + fleetId +
                    " AND f.OrganizationId=o.OrganizationId";
                // Executes SQL statement
                orgName = Convert.ToString(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization name by fleet id=" + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization name by fleet id=" + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return orgName;
        }

        /// <summary>
        /// Gets the organization ID owning the given box ID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public int GetOrganizationIdByBoxId(int boxId)
        {
            int orgId = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // Prepares SQL statement
                string sql = "SELECT OrganizationId FROM vlfFleet f," +
                    "vlfVehicleAssignment va, vlfFleetVehicles fv" +
                    " WHERE va.BoxId=" + boxId +
                    " AND va.VehicleId=fv.VehicleId" +
                    " AND fv.FleetId=f.FleetId";
                // Executes SQL statement
                orgId = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization id by box id=" + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization id by box id=" + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return orgId;
        }

        /// <summary>
        /// Gets the organization name owning the given box ID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public String GetOrganizationNameByBoxId(int boxId)
        {
            String orgName = VLF.CLS.Def.Const.unassignedStrValue;
            try
            {
                // Prepares SQL statement
                string sql = "SELECT OrganizationName FROM vlfFleet f, vlfOrganization o," +
                    "vlfVehicleAssignment va, vlfFleetVehicles fv" +
                    " WHERE va.BoxId=" + boxId +
                    " AND va.VehicleId=fv.VehicleId" +
                    " AND fv.FleetId=f.FleetId" +
                    " AND f.OrganizationId = o.OrganizationId";
                // Executes SQL statement
                orgName = Convert.ToString(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization name by fleet id=" + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization name by fleet id=" + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return orgName;
        }

        /// <summary>
        /// Retrieves Organization ids
        /// </summary>
        /// <returns>DataSet [OrganizationId]</returns>
        /// <param name="boxId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationIdsByBoxId(int boxId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT OrganizationId " +
                    " FROM vlfVehicleAssignment,vlfFleet,vlfFleetVehicles" +
                    " WHERE vlfVehicleAssignment.BoxId=" + boxId +
                    " AND vlfVehicleAssignment.VehicleId=vlfFleetVehicles.VehicleId" +
                    " AND vlfFleetVehicles.FleetId=vlfFleet.FleetId";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization ids by box Id " + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization ids by box Id " + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves SuperOrganization ids and descriptions
        /// </summary>
        /// <returns></returns>
        public DataSet GetSuperOrganizations()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT OrganizationId,OrganizationName FROM vlfOrganization WHERE OrganizationId = SuperOrganizationId;";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve super organizations. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve super organizations. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <returns>DataSet</returns>
        /// <param name="organizationName"></param> 
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapGroupName],[MapPriority],[MapId],[MapEngineName],[MapPath],[MapExternalPath],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationInfoByOrganizationName(string organizationName)
        {
            return GetOrganizationInfoBy("OrganizationName", organizationName);
        }

        /// <summary>
        /// Retrieves Organization info by user id
        /// </summary>
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[MapGroupId],[MapGroupName]</returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationInfoByUserId(int userId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                //string sql = "SELECT DISTINCT vlfOrganization.OrganizationId, vlfOrganization.OrganizationName, vlfOrganization.Contact, vlfOrganization.Address, vlfOrganization.Description, vlfOrganization.LogoName, vlfOrganization.HomePageName, vlfGeoCodeEnginesGroup.GeoCodeGroupId,  ISNULL(vlfGeoCodeEnginesGroup.GeoCodeGroupName, '') AS GeoCodeGroupName, vlfMapEnginesGroup.MapGroupId,  ISNULL(vlfMapEnginesGroup.MapGroupName, '') AS MapGroupName FROM vlfGeoCodeEnginesGroup INNER JOIN vlfOrganization INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId ON vlfGeoCodeEnginesGroup.GeoCodeGroupId = vlfOrganization.GeoCodeGroupId INNER JOIN vlfMapEnginesGroup ON vlfOrganization.MapGroupId = vlfMapEnginesGroup.MapGroupId WHERE vlfUser.UserId=" + userId;
                string sql = "SELECT DISTINCT vlfOrganization.OrganizationId, vlfOrganization.OrganizationName,  vlfOrganization.Description,  vlfOrganization.HomePageName FROM vlfOrganization INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId  WHERE vlfUser.UserId=" + userId;
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization info by user id " + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization info by user id " + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Returns organization id by organization name.
        /// </summary>
        /// <param name="organizationName" ></param> 
        /// <returns>int</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int GetOrganizationIdByOrganizationName(string organizationName)
        {
            int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
            object result = GetOrganizationInfoFieldBy("OrganizationName", "OrganizationId", organizationName);
            if (result != null)
            {
                organizationId = Convert.ToInt32(result);
            }
            return organizationId;
        }

        /// <summary>
        /// Returns organization name by organization Id. 	
        /// </summary>
        /// <param name="organizationId" ></param> 
        /// <returns>string</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public string GetOrganizationNameByOrganizationId(int organizationId)
        {
            string organizationName = VLF.CLS.Def.Const.unassignedStrValue;
            object result = GetOrganizationInfoFieldBy("OrganizationId", "OrganizationName", organizationId);
            if (result != null)
            {
                organizationName = Convert.ToString(result).Trim();
            }
            return organizationName;
        }

        /////// <summary>
        /////// Returns all users info assigned to the organization. 
        /////// </summary>
        /////// <param name="userId"></param>
        /////// <param name="organizationId"></param>
        /////// <returns>DataSet [UserId],[UserName],[FirstName],[LastName],[PersonId],[ExpiredDate]</returns>
        /////// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /////// <exception cref="DASException">Thrown in all other exception cases.</exception>
        ////public DataSet GetUsersInfoByOrganization(int userId, int organizationId)
        ////{
        ////    DataSet resultDataSet = null;
        ////    try
        ////    {
        ////        // 1. Prepares SQL statement
        ////        string sql = "DECLARE @Timezone int DECLARE @DayLightSaving int SET @Timezone= 0";

        ////        sql += " SELECT DISTINCT vlfUser.UserId,RTRIM(UserName) AS UserName,RTRIM(FirstName) AS FirstName,RTRIM(LastName) AS LastName,vlfUser.PersonId" +
        ////            ",CASE WHEN ExpiredDate IS NULL then 'Unlimited' else convert(varchar,DATEADD(hour,@Timezone,ExpiredDate),101) END AS ExpiredDate,UserStatus" +
        ////            " FROM vlfUser INNER JOIN vlfOrganization ON vlfUser.OrganizationId = vlfOrganization.OrganizationId" +
        ////            " INNER JOIN vlfPersonInfo ON vlfUser.PersonId = vlfPersonInfo.PersonId" +
        ////            " WHERE vlfOrganization.OrganizationId =" + organizationId +
        ////            " AND vlfUser.UserId NOT IN (SELECT vlfUser.UserId FROM vlfUser INNER JOIN vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId WHERE vlfUser.OrganizationId=" + organizationId + " AND (vlfUserGroupAssignment.UserGroupId = 1 or vlfUserGroupAssignment.UserGroupId = 14))" +
        ////           // " AND (ExpiredDate IS NULL OR ExpiredDate > GETDATE()) OR UserStatus='Deactivated'" +
        ////            " ORDER BY LastName,FirstName";
        ////        if (sqlExec.RequiredTransaction())
        ////        {
        ////            // 2. Attaches SQL to transaction
        ////            sqlExec.AttachToTransaction(sql);
        ////        }
        ////        // 3. Executes SQL statement
        ////        resultDataSet = sqlExec.SQLExecuteDataset(sql);
        ////    }
        ////    catch (SqlException objException)
        ////    {
        ////        string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
        ////        Util.ProcessDbException(prefixMsg, objException);
        ////    }
        ////    catch (DASDbConnectionClosed exCnn)
        ////    {
        ////        throw new DASDbConnectionClosed(exCnn.Message);
        ////    }
        ////    catch (Exception objException)GetUsersInfoByOrganizationByLang
        ////    {
        ////        string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
        ////        throw new DASException(prefixMsg + " " + objException.Message);
        ////    }
        ////    return resultDataSet;
        ////}





        /// <summary>
        /// Returns all users info assigned to the organization. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [UserId],[UserName],[FirstName],[LastName],[PersonId],[ExpiredDate]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUsersInfoByOrganization(int userId, int organizationId, bool ChkbxViewDeletedUser)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "GetUsersInfoByOrganization";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@orgid", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@ChkbxViewDeletedUser", SqlDbType.VarChar, ChkbxViewDeletedUser);

                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Retrieves Fleets info
        /// </summary>
        /// <returns>DataSet [FleetId],[FleetName],[Description],[OrganizationId],[OrganizationName]</returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFleetsInfoByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT FleetId, FleetName, vlfOrganization.OrganizationId," + tableName + ".Description, OrganizationName" +
                    " FROM vlfOrganization, vlfFleet" +
                    " WHERE vlfOrganization.OrganizationId=" + organizationId +
                    " AND vlfOrganization.OrganizationId=vlfFleet.OrganizationId" + //AND ISNULL(vlfFleet.FleetType, '') = ''
                    " ORDER BY FleetName";
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve fleets info by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve fleets info by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Gets Fleets by OrganizationId and FleetType
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="FleetType"></param>
        /// <returns>DataSet [FleetId],[FleetName],[Description]</returns>
        public DataSet GetFleetsInfoByOrganizationIdAndType(int OrganizationId, string FleetType)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_FleetsInfoByOrganizationIdAndType_Get";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@OrganizationId", OrganizationId);
                sqlParams[1] = new SqlParameter("@FleetType", FleetType);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Fleets.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Fleets.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;
        }

        /// <summary>
        /// Returns all user assigned to the group for specific organization. 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userGroupId"></param>
        /// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationUsersByUserGroup(int organizationId, short userGroupId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT DISTINCT vlfUser.UserId,UserName,FirstName,LastName" +
                    " FROM vlfUser,vlfPersonInfo,vlfUserGroupAssignment" +
                    " WHERE vlfUserGroupAssignment.UserGroupId=" + userGroupId +
                    " AND vlfUserGroupAssignment.UserId=vlfUser.UserId" +
                    " AND vlfUser.OrganizationId=" + organizationId +
                    " AND vlfUser.PersonId=vlfPersonInfo.PersonId ORDER BY FirstName";
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Returns all user assigned to the group for specific organization. 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userGroupId"></param>
        /// <returns>DataSet [UserId],[UserName],[Password],[FirstName],[LastName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationUsersSecurityByUserGroup(int organizationId, short userGroupId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT DISTINCT vlfUser.UserId,UserName,Password,FirstName,LastName" +
                    " FROM vlfUser,vlfPersonInfo,vlfUserGroupAssignment" +
                    " WHERE vlfUserGroupAssignment.UserGroupId=" + userGroupId +
                    " AND vlfUserGroupAssignment.UserId=vlfUser.UserId" +
                    " AND vlfUser.OrganizationId=" + organizationId +
                    " AND vlfUser.PersonId=vlfPersonInfo.PersonId ORDER BY FirstName";
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <param name="searchFieldName"></param> 
        /// <param name="resultFieldName"></param> 
        /// <param name="searchFieldValue"></param> 
        /// <returns>object</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected object GetOrganizationInfoFieldBy(string searchFieldName, string resultFieldName, int searchFieldValue)
        {
            object resultFieldValue = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT " + resultFieldName + " FROM " + tableName +
                    " WHERE " + searchFieldName + "=" + searchFieldValue;
                //Executes SQL statement
                resultFieldValue = sqlExec.SQLExecuteScalar(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultFieldValue;
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <param name="searchFieldName"></param> 
        /// <param name="resultFieldName"></param> 
        /// <param name="searchFieldValue"></param> 
        /// <returns>object</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected object GetOrganizationInfoFieldBy(string searchFieldName, string resultFieldName, string searchFieldValue)
        {
            object resultFieldValue = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT " + resultFieldName + " FROM " + tableName +
                    " WHERE (" + searchFieldName + "='" + searchFieldValue.Replace("'", "''") + "')";
                //Executes SQL statement
                resultFieldValue = sqlExec.SQLExecuteScalar(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultFieldValue;
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <param name="searchFieldName"></param> 
        /// <param name="searchFieldValue"></param> 
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapGroupName],[MapPriority],[MapId],[MapEngineName],[MapPath],[MapExternalPath],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected DataSet GetOrganizationInfoBy(string searchFieldName, int searchFieldValue)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT vlfOrganization.SuperOrganizationId, vlfOrganization.OrganizationId, vlfOrganization.OrganizationName, vlfOrganization.Contact, vlfOrganization.Address, vlfOrganization.Description, vlfOrganization.LogoName, vlfOrganization.HomePageName, vlfMapEnginesGroup.MapGroupId, vlfMapEnginesGroup.MapGroupName, vlfMapEnginesGroupAssignment.Priority AS MapPriority, vlfMapEngines.MapId, vlfMapEngines.MapEngineName, ISNULL(vlfMapEngines.Path, ' ') AS MapPath, ISNULL(vlfMapEngines.ExternalPath, ' ') AS MapExternalPath, vlfGeoCodeEnginesGroup.GeoCodeGroupId, vlfGeoCodeEnginesGroup.GeoCodeGroupName, vlfGeoCodeEnginesGroupAssignment.Priority AS GeoCodePriority, vlfGeoCodeEngines.GeoCodeId, vlfGeoCodeEngines.GeoCodeEngineName, ISNULL(vlfGeoCodeEngines.Path, ' ') AS GeoPath FROM vlfOrganization INNER JOIN vlfGeoCodeEnginesGroup ON vlfOrganization.GeoCodeGroupId = vlfGeoCodeEnginesGroup.GeoCodeGroupId INNER JOIN vlfGeoCodeEnginesGroupAssignment ON vlfGeoCodeEnginesGroup.GeoCodeGroupId = vlfGeoCodeEnginesGroupAssignment.GeoCodeGroupId INNER JOIN vlfGeoCodeEngines ON vlfGeoCodeEnginesGroupAssignment.GeoCodeId = vlfGeoCodeEngines.GeoCodeId INNER JOIN vlfMapEnginesGroup ON vlfOrganization.MapGroupId = vlfMapEnginesGroup.MapGroupId INNER JOIN vlfMapEnginesGroupAssignment ON vlfMapEnginesGroup.MapGroupId = vlfMapEnginesGroupAssignment.MapGroupId INNER JOIN vlfMapEngines ON vlfMapEnginesGroupAssignment.MapId = vlfMapEngines.MapId WHERE " + searchFieldName + "=" + searchFieldValue;
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <param name="searchFieldName"></param> 
        /// <param name="searchFieldValue"></param> 
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapGroupName],[MapPriority],[MapId],[MapEngineName],[MapPath],[MapExternalPath],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected DataSet GetOrganizationInfoBy(string searchFieldName, string searchFieldValue)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT vlfOrganization.SuperOrganizationId, vlfOrganization.OrganizationId, vlfOrganization.OrganizationName, vlfOrganization.Contact, vlfOrganization.Address, vlfOrganization.Description, vlfOrganization.LogoName, vlfOrganization.HomePageName, vlfMapEnginesGroup.MapGroupId, vlfMapEnginesGroup.MapGroupName, vlfMapEnginesGroupAssignment.Priority AS MapPriority, vlfMapEngines.MapId, vlfMapEngines.MapEngineName, ISNULL(vlfMapEngines.Path, ' ') AS MapPath, ISNULL(vlfMapEngines.ExternalPath, ' ') AS MapExternalPath, vlfGeoCodeEnginesGroup.GeoCodeGroupId, vlfGeoCodeEnginesGroup.GeoCodeGroupName, vlfGeoCodeEnginesGroupAssignment.Priority AS GeoCodePriority, vlfGeoCodeEngines.GeoCodeId, vlfGeoCodeEngines.GeoCodeEngineName, ISNULL(vlfGeoCodeEngines.Path, ' ') AS GeoPath FROM vlfOrganization INNER JOIN vlfGeoCodeEnginesGroup ON vlfOrganization.GeoCodeGroupId = vlfGeoCodeEnginesGroup.GeoCodeGroupId INNER JOIN vlfGeoCodeEnginesGroupAssignment ON vlfGeoCodeEnginesGroup.GeoCodeGroupId = vlfGeoCodeEnginesGroupAssignment.GeoCodeGroupId INNER JOIN vlfGeoCodeEngines ON vlfGeoCodeEnginesGroupAssignment.GeoCodeId = vlfGeoCodeEngines.GeoCodeId INNER JOIN vlfMapEnginesGroup ON vlfOrganization.MapGroupId = vlfMapEnginesGroup.MapGroupId INNER JOIN vlfMapEnginesGroupAssignment ON vlfMapEnginesGroup.MapGroupId = vlfMapEnginesGroupAssignment.MapGroupId INNER JOIN vlfMapEngines ON vlfMapEnginesGroupAssignment.MapId = vlfMapEngines.MapId WHERE (" + searchFieldName + "='" + searchFieldValue.Replace("'", "''") + "')";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves communication information for all organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="commAddressTypeId"></param>
        /// <returns>DataSet [BoxId],[CommAddressValue]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationCommInfo(int organizationId, short commAddressTypeId)
        {
            DataSet resultDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfBox.BoxId, ISNULL(vlfBoxCommInfo.CommAddressValue,'N/A') AS CommAddressValue" +
                    " FROM vlfBoxCommInfo INNER JOIN vlfBox with (nolock) ON vlfBoxCommInfo.BoxId = vlfBox.BoxId" +
                    " WHERE OrganizationId=" + organizationId +
                    " AND CommAddressTypeId=" + commAddressTypeId;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve communication info for organization=" + organizationId + " by coomunication address type=" + commAddressTypeId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve communication info for organization=" + organizationId + " by coomunication address type=" + commAddressTypeId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Get all map engines info
        /// </summary>
        /// <returns>DataSet [MapGroupId],[MapGroupName],[Priority],[MapId],[MapEngineName],[Path],[ExternalPath]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllMapEnginesInfo()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfMapEnginesGroup.MapGroupId, vlfMapEnginesGroup.MapGroupName, vlfMapEnginesGroup.MapGroupDescription, vlfMapEnginesGroupAssignment.Priority, vlfMapEngines.MapId, vlfMapEngines.MapEngineName, vlfMapEngines.Path, vlfMapEngines.ExternalPath FROM vlfMapEnginesGroupAssignment INNER JOIN vlfMapEnginesGroup ON vlfMapEnginesGroupAssignment.MapGroupId = vlfMapEnginesGroup.MapGroupId INNER JOIN vlfMapEngines ON vlfMapEnginesGroupAssignment.MapId = vlfMapEngines.MapId ORDER BY vlfMapEnginesGroup.MapGroupId, vlfMapEnginesGroupAssignment.Priority";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve map engines info. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve map engines info. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get all map engines
        /// </summary>
        /// <returns></returns>
        public DataSet GetMapEngines()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfMapEngines.MapId, vlfMapEngines.MapEngineName, vlfMapEngines.Path, vlfMapEngines.ExternalPath FROM vlfMapEngines";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve map engines";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve map engines";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get all map engines short info
        /// </summary>
        /// <returns>DataSet [MapGroupId],[MapGroupName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllMapEnginesShortInfo()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT MapGroupId, MapGroupName FROM vlfMapEnginesGroup ORDER by MapGroupName";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve map engines short info. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve map engines short info. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get all GeoCode engines info
        /// </summary>
        /// <returns>DataSet [GeoCodeGroupId],[GeoCodeGroupName],[Priority],[GeoCodeId],[GeoCodeEngineName],[Path]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllGeoCodeEnginesInfo()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfGeoCodeEnginesGroup.GeoCodeGroupId, vlfGeoCodeEnginesGroup.GeoCodeGroupName, vlfGeoCodeEnginesGroupAssignment.Priority, vlfGeoCodeEngines.GeoCodeId, vlfGeoCodeEngines.GeoCodeEngineName, vlfGeoCodeEngines.Path FROM vlfGeoCodeEngines INNER JOIN vlfGeoCodeEnginesGroupAssignment ON vlfGeoCodeEngines.GeoCodeId = vlfGeoCodeEnginesGroupAssignment.GeoCodeId INNER JOIN vlfGeoCodeEnginesGroup ON vlfGeoCodeEnginesGroupAssignment.GeoCodeGroupId = vlfGeoCodeEnginesGroup.GeoCodeGroupId ORDER BY vlfGeoCodeEnginesGroup.GeoCodeGroupId, vlfGeoCodeEnginesGroupAssignment.Priority";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve geo code engines info. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve geo code engines info. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get all geocode engines
        /// </summary>
        /// <returns></returns>
        public DataSet GetGeoCodeEngines()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfGeoCodeEngines.GeoCodeId, vlfGeoCodeEngines.GeoCodeEngineName, vlfGeoCodeEngines.Path FROM vlfGeoCodeEngines";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve geo code engines";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve geo code engines";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get all GeoCode engines short info
        /// </summary>
        /// <returns>DataSet [GeoCodeGroupId],[GeoCodeGroupName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllGeoCodeEnginesShortInfo()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT GeoCodeGroupId, GeoCodeGroupName FROM vlfGeoCodeEnginesGroup ORDER BY GeoCodeGroupName";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve geo code short info. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve geo code engines short info. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves information about all organizations 
        /// </summary>
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],[Address],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetAllOrganizationsInfo()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM vlfOrganization ORDER BY OrganizationName";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organizations info. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organizations info. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Retrieves information about all organizations availabel for the userid
        /// user organization and all child organization (if super organization)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationsInfoListByUser(int userId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM vlfOrganization org " +
                            " INNER JOIN vlfUser usr on (org.OrganizationId = usr.OrganizationId) " +
                            " OR (org.SuperOrganizationId = usr.OrganizationId) " +
                            " WHERE usr.UserId = " + userId.ToString() +
                            " ORDER BY org.OrganizationName ";

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organizations info for user: " + userId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organizations info for user: " + userId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get DCL Information based on Organization and vehicle description
        /// </summary>
        /// <returns>DataSet [DCL]</returns>
        /// <param name="boxId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetDclCommInfo(int orgID, string vehDesc)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetBoxDcl";


                sqlExec.AddCommandParam("@orgID", SqlDbType.Int, orgID);
                sqlExec.AddCommandParam("@vehicleDesc", SqlDbType.NVarChar, vehDesc);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);


            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve DCL info for vehicle:  " + vehDesc + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve DCL info for vehicle:  " + vehDesc + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves information about audit groups for all organizations
        /// </summary>
        /// <returns>DataSet [OrganizationId],[AuditGroupId],[GroupName],[MethodName],[Frequency],[Period]</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetAuditGroupInfo()
        {
            DataSet sqlDataSet = null;
            string prefixMsg = "Unable to retrieve audit info for organizations. ";
            try
            {
                //Prepares SQL statement
                //string sql = "SELECT vlfOrganization.OrganizationId, vlfAuditGroup.AuditGroupId, vlfAuditGroup.GroupName, vlfAuditWebMethods.MethodName, vlfAuditGroup.Frequency, vlfAuditGroup.Period FROM vlfAuditGroup RIGHT OUTER JOIN vlfAuditWebMethods ON vlfAuditGroup.AuditGroupId = vlfAuditWebMethods.AuditGroupId LEFT OUTER JOIN vlfOrganization ON vlfAuditGroup.AuditGroupId = vlfOrganization.AuditGroupId";
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset("OrganizationAudits");
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

        /// <summary>
        ///      vlfOrganization has an upper link to the superOrganization 
        ///      from which it extracts field SystemEmailAddress and returns it
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>empty or the actual field</returns>
        public string GetSystemEmailAddress(int boxId)
        {
            string ret = string.Empty;

            try
            {
                //Prepares SQL statement
                string sql = "GetSystemEmailAddress";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@ret", SqlDbType.VarChar, ParameterDirection.Output, 50, ret);

                //Executes SQL statement
                sqlExec.SPExecuteNonQuery(sql);
                ret = (DBNull.Value == sqlExec.ReadCommandParam("@ret")) ? string.Empty : sqlExec.ReadCommandParam("@ret").ToString();
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("GetSystemEmailAddress -> SqlException: Boxid={0}", boxId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("GetSystemEmailAddress -> Exception: Boxid={0}", boxId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            //         Util.BTrace(Util.INF0, "-GetSystemEmailAddress -> for={0} ret={1}", boxId, ret);
            return ret;
        }

        /// <summary>
        ///       this is a function for super organizations
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>
        ///      [OrganizationId], [OrganizationName], [Contact], [Address], [Description], [LogoName], 
        ///      [HomePageName], [MapGroupId], [GeoCodeGroupId], [DefaultMapGroupId], [AuditGroupId]
        ///      [SuperOrganizationId], [SystemEmailAddress]
        /// </returns>
        public DataSet GetOrganizationsMonitoredBy(int organizationId)
        {
            DataSet sqlDataSet = null;
            if (organizationId > 0)
            {
                try
                {
                    //Prepares SQL statement
                    string sql = "";
                    if (organizationId==410)
                        sql = "SELECT * FROM vlfOrganization ORDER BY OrganizationName";
                    else
                        sql = string.Format("SELECT * FROM vlfOrganization WHERE SuperOrganizationId ={0} ORDER BY OrganizationName", organizationId);

                    //Executes SQL statement
                    sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                }
                catch (SqlException objException)
                {
                    string prefixMsg = "Unable to retrieve organizations info. ";
                    Util.ProcessDbException(prefixMsg, objException);
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception objException)
                {
                    string prefixMsg = "Unable to retrieve organizations info. ";
                    throw new DASException(prefixMsg + " " + objException.Message);
                }
            }
            return sqlDataSet;
        }

        /// <summary>
        ///      this function allows a user within a certain group to substitute his credentials 
        ///      with the sub-organization's ones (s)he wants to login
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void GetLoginCredentialsWithinSameGroup(int userid, int organizationId, out string username, out string password)
        {
            password = username = string.Empty;
            try
            {
                //Prepares SQL statement
                string sql = "GetEquivalentCredentials";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@userid", SqlDbType.Int, userid);
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@username", SqlDbType.VarChar, ParameterDirection.Output, 32, username);
                sqlExec.AddCommandParam("@password", SqlDbType.VarChar, ParameterDirection.Output, 32, password);

                //Executes SQL statement
                sqlExec.SPExecuteNonQuery(sql);
                username = (DBNull.Value == sqlExec.ReadCommandParam("@username")) ?
                                  string.Empty : sqlExec.ReadCommandParam("@username").ToString();

                password = (DBNull.Value == sqlExec.ReadCommandParam("@password")) ?
                                              string.Empty : sqlExec.ReadCommandParam("@password").ToString();
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("GetLoginCredentialsWithinSameGroup -> SqlException: userid={0} orgId={1}", userid, organizationId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("GetLoginCredentialsWithinSameGroup -> Exception: userid={0} orgId={1}", userid, organizationId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

        }

        /// <summary>
        /// Gets organization landmarks within latlong rectangle
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="topleftlat"></param>
        /// <param name="topleftlong"></param>
        /// <param name="bottomrightlat"></param>
        /// <param name="bottomrightlong"></param>
        /// <returns></returns>
        public DataSet GetOrganizationLandmarksWithBoundary(int organizationId, double topleftlat, double topleftlong, double bottomrightlat, double bottomrightlong)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "GetOrganizationLandmarksWithBoundary";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@topleftlat", SqlDbType.Float, topleftlat);
                sqlExec.AddCommandParam("@topleftlong", SqlDbType.Float, topleftlong);
                sqlExec.AddCommandParam("@bottomrightlat", SqlDbType.Float, bottomrightlat);
                sqlExec.AddCommandParam("@bottomrightlong", SqlDbType.Float, bottomrightlong);


                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("GetOrganizationLandmarksWithBoundary -> SqlException:  orgId={0}", organizationId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("GetOrganizationLandmarksWithBoundary -> Exception:  orgId={0}", organizationId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Organization Preferences
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationPreferences(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "select NotificationEmailAddress,OrganizationId,RadiusForGPS,MaximumReportingInterval,HistoryTimeRange,WaitingPeriodToGetMessages,Timezone from vlfOrganizationPreferences where OrganizationId=" + organizationId;
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization preferences";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization preferences";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Update Organization Preferences
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="NotificationEmailAddress"></param>
        /// <param name="RadiusForGPS"></param>
        /// <param name="MaximumReportingInterval"></param>
        /// <param name="HistoryTimeRange"></param>
        /// <param name="WaitingPeriodToGetMessages"></param>
        /// <param name="Timezone"></param>
        public void UpdateOrganizationPreferences(int organizationId, string NotificationEmailAddress, int RadiusForGPS, int MaximumReportingInterval, int HistoryTimeRange, int WaitingPeriodToGetMessages, int Timezone)
        {
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE vlfOrganizationPreferences " +
                " SET NotificationEmailAddress='" + NotificationEmailAddress.Trim().Replace("'", "''") + "'" +
                ", RadiusForGPS=" + RadiusForGPS +
                ", MaximumReportingInterval=" + MaximumReportingInterval +
                ", HistoryTimeRange=" + HistoryTimeRange +
                ", WaitingPeriodToGetMessages=" + WaitingPeriodToGetMessages +
                ", Timezone=" + Timezone +
                " WHERE OrganizationId=" + organizationId;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update organization preferences id " + organizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update organization preferences id " + organizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update organization preferences id " + organizationId;
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
        }

        public System.Collections.ArrayList GetListItemsForOrganizations(bool orderByName)
        {
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT OrganizationId, OrganizationName FROM vlfOrganization ORDER BY {0}", (orderByName) ? "OrganizationName" : "OrganizationId");
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                {
                    for (int a = 0; a < sqlDataSet.Tables[0].Rows.Count; a++)
                    {
                        int orgId = 0;
                        int.TryParse((string)sqlDataSet.Tables[0].Rows[a]["OrganizationId"], out orgId);
                        arr.Add(new DisplayIntValueItem(orgId, (string)sqlDataSet.Tables[0].Rows[a]["OrganizationName"]));
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organizations. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organizations. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return arr;
        }


        /// <summary>
        /// Save, update, delete the organization skill        
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="skillName"></param>
        /// <param name="description"></param>
        /// <param name="skillId"></param>        
        /// <param name="delete"></param>
        /// <returns>bool</returns>
        public DataSet SaveSkill(int organizationId, string skillName, string description = null, int skillId = 0, int delete = 0)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "vlfSaveOrganizationSkill";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@skillName", SqlDbType.VarChar, skillName);
                sqlExec.AddCommandParam("@description", SqlDbType.VarChar, description);
                sqlExec.AddCommandParam("@skillId", SqlDbType.Int, skillId);
                sqlExec.AddCommandParam("@delete", SqlDbType.Int, delete);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("vlfSaveOrganizationSkill -> SqlException:  organizationId={0}", organizationId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("vlfSaveOrganizationSkill -> Exception:  OrganizationId={0}", organizationId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get the list of organization skills
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetOrganizationSkills(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM vlfOrganizationSkills WHERE OrganizationId=" + organizationId;


                sqlDataSet = sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("vlfOrganizationSkills -> SqlException:  organizationId={0}", organizationId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("vlfOrganizationSkills -> Exception:  OrganizationId={0}", organizationId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        public DataSet OrganizationIP_Validate(int organizationId, string ip)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "OrganizationIP_Validate";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@ip", SqlDbType.VarChar, ip);
                sqlExec.AddCommandParam("@orgid", SqlDbType.Int, organizationId);
                
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("OrganizationIP_Validate -> SqlException:  organizationId={0}", organizationId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("OrganizationIP_Validate -> Exception:  OrganizationId={0}", organizationId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        //SALMAN Feb 13,2013
        /// <summary>
        /// Returns all users info assigned to the organization. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [UserId],[UserName] (FirstName + LastName)</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUsersNameInfoByOrganization(int userId, int organizationId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "select vlfUser.UserId,RTRIM(FirstName) + ' ' + RTRIM(LastName) AS UserName" +
                    " FROM vlfUser INNER JOIN vlfOrganization ON vlfUser.OrganizationId = vlfOrganization.OrganizationId" +
                    " INNER JOIN vlfPersonInfo ON vlfUser.PersonId = vlfPersonInfo.PersonId" +
                    " WHERE vlfOrganization.OrganizationId =" + organizationId +
                    " AND vlfUser.UserId NOT IN (SELECT vlfUser.UserId FROM vlfUser INNER JOIN vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId WHERE vlfUser.OrganizationId=" + organizationId + " AND (vlfUserGroupAssignment.UserGroupId = 1 OR vlfUserGroupAssignment.UserGroupId = 14))" +
                    " AND (ExpiredDate IS NULL OR ExpiredDate > GETDATE())" +
                    " ORDER BY 2";
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        #region Organization Services

        /// <summary>
        /// Retrieves List of organization Services.
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetOrganizationServices(int OrganizationId, int ServiceID, bool IsBillable)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "usp_OrganizationServices_Get";

                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@IsBillable", SqlDbType.Bit, IsBillable);
                sqlExec.AddCommandParam("@ServiceID", SqlDbType.Int, ServiceID);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization services for OrganizationId:  " + OrganizationId.ToString() + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization services for OrganizationId:  " + OrganizationId.ToString() + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves List of organization Services for add
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetOrganizationServicesForAdd(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "usp_OrganizationServicesForAdd_Get";

                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, OrganizationId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization services for add for OrganizationId:  " + OrganizationId.ToString() + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization services for add for OrganizationId:  " + OrganizationId.ToString() + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Enables organization Service
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="ServiceID"></param>
        /// <returns>OrganizationServiceID</returns>
        public int AddOrganizationService(int OrganizationID, int ServiceID, int UserID)
        {
            int OrganizationServiceID = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, ParameterDirection.Input, OrganizationID);
                sqlExec.AddCommandParam("@ServiceID", SqlDbType.Int, ParameterDirection.Input, ServiceID);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, ParameterDirection.Input, UserID);
                sqlExec.AddCommandParam("@OrganizationServiceID", SqlDbType.Int, ParameterDirection.Output, 4, OrganizationServiceID);

                int res = sqlExec.SPExecuteNonQuery("usp_OrganizationService_Add");

                OrganizationServiceID = (DBNull.Value == sqlExec.ReadCommandParam("@OrganizationServiceID")) ?
                              OrganizationServiceID : Convert.ToInt32(sqlExec.ReadCommandParam("@OrganizationServiceID"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add organization service - OrganizationID={0}, ServiceID={1}.", OrganizationID.ToString(), ServiceID.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add organization Service - OrganizationID={0}, ServiceID={1}.", OrganizationID.ToString(), ServiceID.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return OrganizationServiceID;
        }

        /// <summary>
        /// Disables organization Service
        /// </summary>
        /// <param name="OrganizationServiceID"></param>
        /// <returns>rows affected</returns>
        public int DeleteOrganizationService(int OrganizationServiceID, int UserID)
        {
            int rowsAffected = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationServiceID", SqlDbType.Int, ParameterDirection.Input, OrganizationServiceID);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, ParameterDirection.Input, UserID);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_OrganizationService_Delete");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to disable organization service - OrganizationServiceID={0}.", OrganizationServiceID.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to disable organization service - OrganizationServiceID={0}.", OrganizationServiceID.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Adds default User Group to an Organization
        /// </summary>
        /// <param name="OrganiztionId"></param>
        /// <returns>rows affected</returns>
        public int AddUserGroupsDefault(int OrganiztionId)
        {
            int rowsAffected = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, ParameterDirection.Input, OrganiztionId);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupsDefault_Add");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add default Usergroups - OrganiztionId={0}.", OrganiztionId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add default Usergroups - OrganiztionId={0}.", OrganiztionId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        #endregion

        public DataSet GetAllSensorByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT distinct SensorId,SensorName,SensorAction,AlarmLevelOn,AlarmLevelOff FROM vlfBoxSensorsCfg WHERE BoxId in (SELECT BoxId FROM vlfBox where OrganizationId=" + organizationId + ")";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve sensors by organizationId:{0}.", organizationId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve sensors by organizationId:{0}.", organizationId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        public int AddOrganizationDomainMetadata(int userId, int orgId, long domainId, string metadataName)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "AddOrganizationDomainMetadata";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@DomainId", SqlDbType.BigInt, domainId); 
                sqlExec.AddCommandParam("@MetadataName", SqlDbType.NVarChar, metadataName);

                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to AddOrganizationDomainMetadata organizationId=" + orgId + " userId=" + userId + " metadataName=" + metadataName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to AddOrganizationDomainMetadata organizationId=" + orgId + " userId=" + userId + " metadataName=" + metadataName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }



        public int DeleteOrganizationDomainMetadata(int userId, int orgId, long domainMetadataId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "DeleteOrganizationDomainMetadata";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@DomainMetadataId", SqlDbType.BigInt, domainMetadataId);

                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to DeleteOrganizationDomainMetadata organizationId=" + orgId + " userId=" + userId + " domainMetadataId=" + domainMetadataId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to DeleteOrganizationDomainMetadata organizationId=" + orgId + " userId=" + userId + " domainMetadataId=" + domainMetadataId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        public int UpdateOrganizationDomainMetadata(int userId, int orgId, long domainMetadataId, string newMetadataName)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "UpdateOrganizationDomainMetadata";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@DomainMetadataId", SqlDbType.BigInt, domainMetadataId);
                sqlExec.AddCommandParam("@NewMetadataName", SqlDbType.NVarChar, newMetadataName);

                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to UpdateOrganizationDomainMetadata organizationId=" + orgId + " userId=" + userId + " newMetadataName=" + newMetadataName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to UpdateOrganizationDomainMetadata organizationId=" + orgId + " userId=" + userId + " newMetadataName=" + newMetadataName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }
    }
}
