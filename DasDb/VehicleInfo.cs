using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfVehicleInfo table.
	/// </summary>
	public class VehicleInfo : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public VehicleInfo(SQLExecuter sqlExec) : base ("vlfVehicleInfo",sqlExec)
		{
		}



        public int SaveVehicleOperationalState(int userId, int orgId, long vehicleId,
            int operationalState, string notes)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "SaveVehicleOperationalState";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, vehicleId);
                sqlExec.AddCommandParam("@OperationalState", SqlDbType.Int, operationalState);
                sqlExec.AddCommandParam("@Notes", SqlDbType.NVarChar, notes);

                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.AttachToTransaction(sql);
                }
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to SaveVehicleOperationalState organizationId=" + orgId + " userId=" + userId + " vehicleId=" + vehicleId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to SaveVehicleOperationalState organizationId=" + orgId + " userId=" + userId + " vehicleId=" + vehicleId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        public DataSet GetVehicleOperationalState(int userId, int orgId, long vehicleId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {

                prefixMsg = "GetVehicleOperationalState->Unable to get info for userid  = " + userId.ToString() + ". ";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@OrganizationId", orgId);
                sqlParams[1] = new SqlParameter("@UserId", userId);
                sqlParams[2] = new SqlParameter("@VehicleId", vehicleId);

                // SQL statement
                string sql = "[GetVehicleOperationalState]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }


        public DataSet GetServiceConfigurationsByLandmarkAndVehicle(int orgId, long vehicleId, long landmarkId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {

                prefixMsg = "GetServiceConfigurationsByLandmarkAndVehicle->Unable to get service config for vehicleid  = " + vehicleId.ToString() + ". ";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@organizationId", orgId);
                sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
                sqlParams[2] = new SqlParameter("@landmarkId", landmarkId);

                // SQL statement
                string sql = "[sp_vlfGetServiceConfigurationsByLandmarkAndVehicle]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }


        public DataSet ListVehiclesInLandmarksForDashboard(int userId, int orgId, long landmarkCategoryId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {

                prefixMsg = "ListVehiclesInLandmarksForDashboard->Unable to get info for userid  = " + userId.ToString() + ". ";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@OrganizationId", orgId);
                sqlParams[1] = new SqlParameter("@UserId", userId);
                sqlParams[2] = new SqlParameter("@LandmarkCategoryId", landmarkCategoryId);

                // SQL statement
                string sql = "[ListVehiclesInLandmarksForDashboard]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        public DataSet GetVehicleAvailabilityByManagerForDashboard(int userId, int orgId, int fleetId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {

                prefixMsg = "GetVehicleAvailabilityByManagerForDashboard->Unable to get info for userid  = " + userId.ToString() + ". ";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@OrganizationId", orgId);
                sqlParams[1] = new SqlParameter("@UserId", userId);
                sqlParams[2] = new SqlParameter("@FleetId", fleetId);

                // SQL statement
                string sql = "[GetVehicleAvailabilityByManagerForDashboard]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        public DataSet ListVehiclesInLandmarkByFleet(int userId, int orgId, int fleetId, long landmarkId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {

                prefixMsg = "ListVehiclesInLandmarkByFleet->Unable to get info for userid  = " + userId.ToString() + ", landmarkId = " + landmarkId.ToString() + ".";
                SqlParameter[] sqlParams = new SqlParameter[4];                
                sqlParams[0] = new SqlParameter("@UserId", userId);
                sqlParams[1] = new SqlParameter("@OrganizationId", orgId);
                sqlParams[2] = new SqlParameter("@FleetId", fleetId);
                sqlParams[3] = new SqlParameter("@LandmarkId", landmarkId);

                // SQL statement
                string sql = "[ListVehiclesInLandmarkByFleet]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Add new vehicle info.
        /// </summary>
        /// <param name="vehicInfo"></param>
        /// <param name="organizationId"></param>
        /// <param name="vehicleExist"></param>
        /// <returns>new vehicle id</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public Int64 AddVehicleInfo_NewTZ(VehicInfo vehicInfo, int organizationId, ref bool vehicleExist)
        {
            vehicleExist = false;
            Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
            // 1. validates parameters
            if ((vehicInfo.vinNum == VLF.CLS.Def.Const.unassignedStrValue) ||
                (vehicInfo.makeModelId == VLF.CLS.Def.Const.unassignedIntValue) ||
                (vehicInfo.vehicleTypeId == VLF.CLS.Def.Const.unassignedIntValue) ||
                (vehicInfo.stateProvince == VLF.CLS.Def.Const.unassignedStrValue) ||
                (vehicInfo.costPerMile == VLF.CLS.Def.Const.unassignedIntValue) ||
                (vehicInfo.description == VLF.CLS.Def.Const.unassignedStrValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: vinNum=" +
                    vehicInfo.vinNum +
                    " MakeModelId=" + vehicInfo.makeModelId +
                    " VehicleTypeId=" + vehicInfo.vehicleTypeId +
                    " StateProvince=" + vehicInfo.stateProvince +
                    " Cost per mile=" + vehicInfo.costPerMile +
                    " Description=" + vehicInfo.description);
            }

            if (GetVehicleIdByDescription(vehicInfo.description, organizationId) != VLF.CLS.Def.Const.unassignedIntValue)
                throw new DASAppViolatedIntegrityConstraintsException("Duplicate vehicle description for organization: " + organizationId + ".");
            // 2. lookup for existing vehicle id by license plate
            vehicleId = GetVehicleIdByVinNumber(vehicInfo.vinNum);
            if (vehicleId == VLF.CLS.Def.Const.unassignedIntValue)
            {
                int rowsAffected = 0;
                // 3. In case of new vehicle get next availible vehicle id and add new vehicle.
                object vid = GetMaxValue("VehicleId");// GetMaxRecordIndex("VehicleId") + 1;
                if (vid != null)
                    vehicleId = Convert.ToInt32(vid) + 1;
                // 4. Prepares SQL statement
                string sql = string.Format("INSERT INTO " + tableName +
                    " (VehicleId,VinNum,MakeModelId,VehicleTypeId,StateProvince," +
                    " ModelYear,Color,Description,CostPerMile,OrganizationId,IconTypeId,Email,TimeZoneNew,DayLightSaving,FormatType,Notify,Warning,Critical,AutoAdjustDayLightSaving,Maintenance,Class)" +
                    " VALUES ( {0},'{1}',{2}, {3},'{4}',{5},'{6}','{7}',{8},{9},{10},'{11}',{12},{13},{14},{15},{16},{17},{18},{19},'{20}')",
                    vehicleId,
                    vehicInfo.vinNum.Replace("'", "''"),
                    vehicInfo.makeModelId,
                    vehicInfo.vehicleTypeId,
                    vehicInfo.stateProvince.Replace("'", "''"),
                    vehicInfo.modelYear,
                    vehicInfo.color.Replace("'", "''"),
                    vehicInfo.description.Replace("'", "''"),
                    vehicInfo.costPerMile,
                    organizationId,
                    vehicInfo.iconTypeId,
                    vehicInfo.email.Replace("'", "''"),
                    vehicInfo.timeZone,
                    vehicInfo.dayLightSaving,
                    vehicInfo.formatType,
                    vehicInfo.notify,
                    vehicInfo.warning,
                    vehicInfo.critical,
                    vehicInfo.autoAdjustDayLightSaving,
                    vehicInfo.maintenance,
                    vehicInfo._class.Replace("'", "''"));
                try
                {
                    if (sqlExec.RequiredTransaction())
                    {
                        // 5. Attach current command SQL to transaction
                        sqlExec.AttachToTransaction(sql);
                    }

                    // 6. Executes SQL statement
                    rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
                }
                catch (SqlException objException)
                {
                    string prefixMsg = "Unable to add new vehicle with vin number '" + vehicInfo.vinNum + "'.";
                    Util.ProcessDbException(prefixMsg, objException);
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception objException)
                {
                    string prefixMsg = "Unable to add new vehicle with vin number '" + vehicInfo.vinNum + "'.";
                    throw new DASException(prefixMsg + " " + objException.Message);
                }
                if (rowsAffected == 0)
                {
                    string prefixMsg = "Unable to add new vehicle with vin number '" + vehicInfo.vinNum + "'.";
                    throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle already exists.");
                }
            }
            else
            {
                vehicleExist = true;
            }
            return vehicleId;
        }
        // Changes for TimeZone Feature ens
		/// <summary>
		/// Add new vehicle info.
		/// </summary>
		/// <param name="vehicInfo"></param>
		/// <param name="organizationId"></param>
		/// <param name="vehicleExist"></param>
		/// <returns>new vehicle id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 AddVehicleInfo(VehicInfo vehicInfo,int organizationId,ref bool vehicleExist)
		{
			vehicleExist = false;
			Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue; 
			// 1. validates parameters
			if(	(vehicInfo.vinNum == VLF.CLS.Def.Const.unassignedStrValue)||
				(vehicInfo.makeModelId == VLF.CLS.Def.Const.unassignedIntValue)||
				(vehicInfo.vehicleTypeId == VLF.CLS.Def.Const.unassignedIntValue)||
				(vehicInfo.stateProvince == VLF.CLS.Def.Const.unassignedStrValue)||
				(vehicInfo.costPerMile == VLF.CLS.Def.Const.unassignedIntValue)||
				(vehicInfo.description == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: vinNum=" + 
					vehicInfo.vinNum + 
					" MakeModelId=" + vehicInfo.makeModelId + 
					" VehicleTypeId=" + vehicInfo.vehicleTypeId +
					" StateProvince=" + vehicInfo.stateProvince +
					" Cost per mile=" + vehicInfo.costPerMile +
					" Description=" + vehicInfo.description);
			}

			if(GetVehicleIdByDescription(vehicInfo.description,organizationId) != VLF.CLS.Def.Const.unassignedIntValue)
				throw new DASAppViolatedIntegrityConstraintsException("Duplicate vehicle description for organization: " + organizationId + ".");
			// 2. lookup for existing vehicle id by license plate
			vehicleId = GetVehicleIdByVinNumber(vehicInfo.vinNum);
			if(vehicleId == VLF.CLS.Def.Const.unassignedIntValue)
			{
				int rowsAffected = 0;
				// 3. In case of new vehicle get next availible vehicle id and add new vehicle.
            object vid = GetMaxValue("VehicleId");// GetMaxRecordIndex("VehicleId") + 1;
            if (vid != null)
               vehicleId = Convert.ToInt32(vid) + 1;
				// 4. Prepares SQL statement
				string sql = string.Format("INSERT INTO " + tableName +
					" (VehicleId,VinNum,MakeModelId,VehicleTypeId,StateProvince," +
					" ModelYear,Color,Description,CostPerMile,OrganizationId,IconTypeId,Email,TimeZone,DayLightSaving,FormatType,Notify,Warning,Critical,AutoAdjustDayLightSaving,Maintenance,Class)" +
                    " VALUES ( {0},'{1}',{2}, {3},'{4}',{5},'{6}','{7}',{8},{9},{10},'{11}',{12},{13},{14},{15},{16},{17},{18},{19},'{20}')",
					vehicleId,
					vehicInfo.vinNum.Replace("'","''"),
					vehicInfo.makeModelId,
					vehicInfo.vehicleTypeId,
					vehicInfo.stateProvince.Replace("'","''"),
					vehicInfo.modelYear,
					vehicInfo.color.Replace("'","''"),
					vehicInfo.description.Replace("'","''"),
					vehicInfo.costPerMile,
					organizationId,
					vehicInfo.iconTypeId,
					vehicInfo.email.Replace("'","''"),
					vehicInfo.timeZone,
					vehicInfo.dayLightSaving,
					vehicInfo.formatType,
					vehicInfo.notify,
					vehicInfo.warning,
					vehicInfo.critical,
					vehicInfo.autoAdjustDayLightSaving,
                    vehicInfo.maintenance,
                    vehicInfo._class.Replace("'", "''"));
				try
				{
					if(sqlExec.RequiredTransaction())
					{
						// 5. Attach current command SQL to transaction
						sqlExec.AttachToTransaction(sql);
					}

					// 6. Executes SQL statement
					rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
				}
				catch (SqlException objException) 
				{
					string prefixMsg = "Unable to add new vehicle with vin number '" + vehicInfo.vinNum + "'.";
					Util.ProcessDbException(prefixMsg,objException);
				}
				catch(DASDbConnectionClosed exCnn)
				{
					throw new DASDbConnectionClosed(exCnn.Message);
				}
				catch(Exception objException)
				{
					string prefixMsg = "Unable to add new vehicle with vin number '" + vehicInfo.vinNum + "'.";
					throw new DASException(prefixMsg + " " + objException.Message);
				}
				if(rowsAffected == 0) 
				{
					string prefixMsg = "Unable to add new vehicle with vin number '" + vehicInfo.vinNum + "'.";
					throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle already exists.");
				}
			}
			else
			{
				vehicleExist = true;
			}
			return vehicleId;
		}
        /// <summary>
        /// Add new vehicle info.
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="weekdayStart"></param>
        /// <param name="weekdayEnd"></param>
        /// <param name="weekendStart"></param>
        /// <param name="weekendEnd"></param>
        /// <returns>new vehicle id</returns>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if vehicle does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddVehicleWorkingHours(Int64 vehicleId, int weekdayStart, int weekdayEnd, int weekendStart, int weekendEnd)
        {
            int rowsAffected = 0;
            if(weekdayStart >= weekdayEnd)
                throw new DASAppInvalidValueException("WeekdayStart: " + weekdayStart + " bigger then or equel to weekdayEnd: " + weekdayEnd);
            if (weekendStart >= weekendEnd)
                throw new DASAppInvalidValueException("WeekendStart: " + weekendStart + " bigger then or equel to weekendEnd: " + weekendEnd);
            
            if (weekdayStart == VLF.CLS.Def.Const.unassignedShortValue || weekdayEnd == VLF.CLS.Def.Const.unassignedShortValue)
            {
                weekdayStart = new TimeSpan(0, 0, 0, 0).Milliseconds;
                weekdayEnd = new TimeSpan(0, 23, 59, 59, 999).Milliseconds;
            }
            if (weekendStart == VLF.CLS.Def.Const.unassignedShortValue || weekendEnd == VLF.CLS.Def.Const.unassignedShortValue ||
                weekendStart > weekendEnd)
            {
                weekendStart = new TimeSpan(0, 0, 0, 0).Milliseconds;
                weekendEnd = new TimeSpan(0, 23, 59, 59, 999).Milliseconds;
            }
            // 1. Prepares SQL statement
            string sql = string.Format("INSERT INTO vlfVehicleWorkingHours (VehicleId,WeekdayStart,WeekdayEnd,WeekendStart,WeekendEnd) VALUES ( {0},{1},{2},{3},{4})",
                vehicleId, weekdayStart, weekdayEnd, weekendStart, weekendEnd);
            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }

                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add vehicle " + vehicleId + " working hours with vin number.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add vehicle " + vehicleId + " working hours with vin number.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add vehicle " + vehicleId + " working hours with vin number.";
                throw new DASAppViolatedIntegrityConstraintsException (prefixMsg + " Vehicle " + vehicleId + " does not exist.");
            }
            return rowsAffected;
        }
        /// <summary>
        /// Updates vehicle working hours.
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="weekdayStart"></param>
        /// <param name="weekdayEnd"></param>
        /// <param name="weekendStart"></param>
        /// <param name="weekendEnd"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if vehicle does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateVehicleWorkingHours(Int64 vehicleId, int weekdayStart, int weekdayEnd, int weekendStart, int weekendEnd)
        {
            int rowsAffected = 0;
            if (weekdayStart >= weekdayEnd)
                throw new DASAppInvalidValueException("WeekdayStart: " + weekdayStart + " bigger then or equel to weekdayEnd: " + weekdayEnd);
            if (weekendStart >= weekendEnd)
                throw new DASAppInvalidValueException("WeekendStart: " + weekendStart + " bigger then or equel to weekendEnd: " + weekendEnd);

            if (weekdayStart == VLF.CLS.Def.Const.unassignedShortValue || weekdayEnd == VLF.CLS.Def.Const.unassignedShortValue)
            {
                weekdayStart = new TimeSpan(0, 0, 0, 0).Milliseconds;
                weekdayEnd = new TimeSpan(0, 23, 59, 59, 999).Milliseconds;
            }
            if (weekendStart == VLF.CLS.Def.Const.unassignedShortValue || weekendEnd == VLF.CLS.Def.Const.unassignedShortValue ||
                weekendStart > weekendEnd)
            {
                weekendStart = new TimeSpan(0, 0, 0, 0).Milliseconds;
                weekendEnd = new TimeSpan(0, 23, 59, 59, 999).Milliseconds;
            }
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfVehicleWorkingHours SET WeekdayStart=" + weekdayStart +
                ",WeekdayEnd=" + weekdayEnd +
                ",WeekendStart=" + weekendStart +
                ",WeekendEnd=" + weekendEnd +
                " WHERE VehicleId=" + vehicleId;

            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update vehicle " + vehicleId + " working hours.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update vehicle " + vehicleId + " working hours.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update vehicle " + vehicleId + " working hours.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This vehicle doesn't exist.");
            }
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Updates vehicle information.
        /// </summary>
        /// <param name="vehicInfo"></param>
        /// <param name="vehicleId"></param>
        /// <param name="organizationId"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateVehicleInfo_NewTZ(VehicInfo vehicInfo, Int64 vehicleId, int organizationId)
        {
            if ((vehicInfo.vinNum == VLF.CLS.Def.Const.unassignedStrValue) ||
               (vehicInfo.makeModelId == VLF.CLS.Def.Const.unassignedIntValue) ||
               (vehicInfo.vehicleTypeId == VLF.CLS.Def.Const.unassignedIntValue) ||
               (vehicInfo.stateProvince == VLF.CLS.Def.Const.unassignedStrValue) ||
               (vehicleId == VLF.CLS.Def.Const.unassignedIntValue) ||
               (vehicInfo.costPerMile == VLF.CLS.Def.Const.unassignedIntValue) ||
               (vehicInfo.description == VLF.CLS.Def.Const.unassignedStrValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: vinNum=" +
                   vehicInfo.vinNum +
                   " MakeModelId=" + vehicInfo.makeModelId +
                   " VehicleTypeId=" + vehicInfo.vehicleTypeId +
                   " StateProvince=" + vehicInfo.stateProvince +
                   " VehicleId=" + vehicleId +
                   " Cost per mile=" + vehicInfo.costPerMile +
                   " Description=" + vehicInfo.description);
            }
            Int64 checkVehicleId = GetVehicleIdByDescription(vehicInfo.description, organizationId);
            if (checkVehicleId != VLF.CLS.Def.Const.unassignedIntValue && checkVehicleId != vehicleId)
                throw new DASAppViolatedIntegrityConstraintsException("Duplicate vehicle description for organization: " + organizationId + ".");

            int rowsAffected = 0;
            // 1. Prepares SQL statement
            System.Text.StringBuilder sql = new System.Text.StringBuilder("UPDATE ");
            sql.Append(tableName);
            sql.Append(" SET VinNum='"); sql.Append(vehicInfo.vinNum.Replace("'", "''")); sql.Append("'");
            sql.Append(", MakeModelId="); sql.Append(vehicInfo.makeModelId);
            sql.Append(", VehicleTypeId="); sql.Append(vehicInfo.vehicleTypeId);
            sql.Append(", StateProvince='"); sql.Append(vehicInfo.stateProvince.Replace("'", "''")); sql.Append("'");
            sql.Append(", ModelYear="); sql.Append(vehicInfo.modelYear);
            sql.Append(", Color='"); sql.Append(vehicInfo.color.Replace("'", "''")); sql.Append("'");
            sql.Append(", Description='"); sql.Append(vehicInfo.description.Replace("'", "''")); sql.Append("'");
            sql.Append(", CostPerMile="); sql.Append(vehicInfo.costPerMile);
            sql.Append(", IconTypeId="); sql.Append(vehicInfo.iconTypeId);
            sql.Append(", Email='"); sql.Append(vehicInfo.email.Replace("'", "''")); sql.Append("'");
            sql.Append(", Phone='"); sql.Append(vehicInfo.phone.Replace("'", "''")); sql.Append("'");
            sql.Append(", TimeZoneNew="); sql.Append(vehicInfo.timeZoneNew);
            sql.Append(", DayLightSaving="); sql.Append(vehicInfo.dayLightSaving);
            sql.Append(", FormatType="); sql.Append(vehicInfo.formatType);
            sql.Append(", Notify="); sql.Append(vehicInfo.notify);
            sql.Append(", Warning="); sql.Append(vehicInfo.warning);
            sql.Append(", Critical="); sql.Append(vehicInfo.critical);
            sql.Append(", AutoAdjustDayLightSaving="); sql.Append(vehicInfo.autoAdjustDayLightSaving);
            // 4 new fields
            sql.Append(", Field1='"); sql.Append(vehicInfo.field1.Replace("'", "''")); sql.Append("'");
            sql.Append(", Field2='"); sql.Append(vehicInfo.field2.Replace("'", "''")); sql.Append("'");
            sql.Append(", Field3='"); sql.Append(vehicInfo.field3.Replace("'", "''")); sql.Append("'");
            sql.Append(", Field4='"); sql.Append(vehicInfo.field4.Replace("'", "''")); sql.Append("'");
            sql.Append(", Maintenance="); sql.Append(vehicInfo.maintenance);
            sql.Append(", Class='"); sql.Append(vehicInfo._class.Replace("'", "''")); sql.Append("'");
            sql.Append(" FROM "); sql.Append(tableName);
            sql.Append(" WHERE VehicleId="); sql.Append(vehicleId);

            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql.ToString());
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql.ToString());
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Cannot update vehicle with vin number '" + vehicInfo.vinNum + "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Cannot update vehicle with vin number '" + vehicInfo.vinNum + "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Cannot update vehicle with vin number '" + vehicInfo.vinNum + "'.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
            }
        }
        // Changes for TimeZone Feature end
        /// <summary>
        /// Updates vehicle information.
        /// </summary>
        /// <param name="vehicInfo"></param>
        /// <param name="vehicleId"></param>
        /// <param name="organizationId"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateVehicleInfo(VehicInfo vehicInfo, Int64 vehicleId, int organizationId)
        {
           if ((vehicInfo.vinNum == VLF.CLS.Def.Const.unassignedStrValue) ||
              (vehicInfo.makeModelId == VLF.CLS.Def.Const.unassignedIntValue) ||
              (vehicInfo.vehicleTypeId == VLF.CLS.Def.Const.unassignedIntValue) ||
              (vehicInfo.stateProvince == VLF.CLS.Def.Const.unassignedStrValue) ||
              (vehicleId == VLF.CLS.Def.Const.unassignedIntValue) ||
              (vehicInfo.costPerMile == VLF.CLS.Def.Const.unassignedIntValue) ||
              (vehicInfo.description == VLF.CLS.Def.Const.unassignedStrValue))
           {
              throw new DASAppInvalidValueException("Wrong value for insert SQL: vinNum=" +
                 vehicInfo.vinNum +
                 " MakeModelId=" + vehicInfo.makeModelId +
                 " VehicleTypeId=" + vehicInfo.vehicleTypeId +
                 " StateProvince=" + vehicInfo.stateProvince +
                 " VehicleId=" + vehicleId +
                 " Cost per mile=" + vehicInfo.costPerMile +
                 " Description=" + vehicInfo.description);
           }
           Int64 checkVehicleId = GetVehicleIdByDescription(vehicInfo.description, organizationId);
           if (checkVehicleId != VLF.CLS.Def.Const.unassignedIntValue && checkVehicleId != vehicleId)
              throw new DASAppViolatedIntegrityConstraintsException("Duplicate vehicle description for organization: " + organizationId + ".");

           int rowsAffected = 0;
           // 1. Prepares SQL statement
           System.Text.StringBuilder sql = new System.Text.StringBuilder("UPDATE ");
           sql.Append(tableName);
           sql.Append(" SET VinNum='"); sql.Append(vehicInfo.vinNum.Replace("'", "''")); sql.Append("'");
           sql.Append(", MakeModelId="); sql.Append(vehicInfo.makeModelId);
           sql.Append(", VehicleTypeId="); sql.Append(vehicInfo.vehicleTypeId);
           sql.Append(", StateProvince='"); sql.Append(vehicInfo.stateProvince.Replace("'", "''")); sql.Append("'");
           sql.Append(", ModelYear="); sql.Append(vehicInfo.modelYear);
           sql.Append(", Color='"); sql.Append(vehicInfo.color.Replace("'", "''")); sql.Append("'");
           sql.Append(", Description='"); sql.Append(vehicInfo.description.Replace("'", "''")); sql.Append("'");
           sql.Append(", CostPerMile="); sql.Append(vehicInfo.costPerMile);
           sql.Append(", IconTypeId="); sql.Append(vehicInfo.iconTypeId);
           sql.Append(", Email='"); sql.Append(vehicInfo.email.Replace("'", "''")); sql.Append("'");
           sql.Append(", Phone='"); sql.Append(vehicInfo.phone.Replace("'", "''")); sql.Append("'");
           sql.Append(", TimeZone="); sql.Append(vehicInfo.timeZone);
           sql.Append(", DayLightSaving="); sql.Append(vehicInfo.dayLightSaving);
           sql.Append(", FormatType="); sql.Append(vehicInfo.formatType);
           sql.Append(", Notify="); sql.Append(vehicInfo.notify);
           sql.Append(", Warning="); sql.Append(vehicInfo.warning);
           sql.Append(", Critical="); sql.Append(vehicInfo.critical);
           sql.Append(", AutoAdjustDayLightSaving="); sql.Append(vehicInfo.autoAdjustDayLightSaving);
           // 4 new fields
           sql.Append(", Field1='"); sql.Append(vehicInfo.field1.Replace("'", "''")); sql.Append("'");
           sql.Append(", Field2='"); sql.Append(vehicInfo.field2.Replace("'", "''")); sql.Append("'");
           sql.Append(", Field3='"); sql.Append(vehicInfo.field3.Replace("'", "''")); sql.Append("'");
           sql.Append(", Field4='"); sql.Append(vehicInfo.field4.Replace("'", "''")); sql.Append("'");
           sql.Append(", Maintenance="); sql.Append(vehicInfo.maintenance);
           sql.Append(", Class='"); sql.Append(vehicInfo._class.Replace("'", "''")); sql.Append("'"); 
           sql.Append(" FROM "); sql.Append(tableName);
           sql.Append(" WHERE VehicleId="); sql.Append(vehicleId);

           try
           {
              if (sqlExec.RequiredTransaction())
              {
                 // 2. Attach current command SQL to transaction
                 sqlExec.AttachToTransaction(sql.ToString());
              }
              // 3. Executes SQL statement
              rowsAffected = sqlExec.SQLExecuteNonQuery(sql.ToString());
           }
           catch (SqlException objException)
           {
              string prefixMsg = "Cannot update vehicle with vin number '" + vehicInfo.vinNum + "'.";
              Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
              throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
              string prefixMsg = "Cannot update vehicle with vin number '" + vehicInfo.vinNum + "'.";
              throw new DASException(prefixMsg + " " + objException.Message);
           }
           if (rowsAffected == 0)
           {
              string prefixMsg = "Cannot update vehicle with vin number '" + vehicInfo.vinNum + "'.";
              throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
           }
        }
      
        /// <summary>
        /// Updates vehicle additional information.
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="field1"></param>
        /// <param name="field2"></param>
        /// <param name="field3"></param>
        /// <param name="field4"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if vehicle does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateVehicleAdditionalInfo(Int64 vehicleId, string field1, string field2, string field3, string field4, string field5, int? vehicleWeight, string vehicleWtUnit, float? fuelCapacity, float? fuelBurnRate)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfVehicleInfo SET Field1='" + field1.Replace("'", "''") + "'" +
                ",Field2='" + field2.Replace("'", "''") + "'" +
                ",Field3='" + field3.Replace("'", "''") + "'" +
                ",Field4='" + field4.Replace("'", "''") + "'" +
                ",Field5='" + field5.Replace("'", "''") + "'" +
                //" FROM vlfVehicleInfo WHERE VehicleId=" + vehicleId;
                ",VehicleWeight='" + vehicleWeight + "'" +
                ",VehicleWtUnit='" + vehicleWtUnit + "'" +
                ",FuelCapacity='" + fuelCapacity + "'" +
                ",FuelBurnRate='" + fuelBurnRate + "'" +
                ",UpdateDate = GETUTCDATE()" +
                " WHERE VehicleId=" + vehicleId;

            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update vehicle " + vehicleId + " additional info.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update vehicle " + vehicleId + " additional info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update vehicle " + vehicleId + " additional info.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This vehicle doesn't exist.");
            }
        }

        public void Update3rdPartyVehicleAdditionalInfo(Int64 vehicleId, string EquipNbr, string SAPEquipNbr, string LegacyEquipNbr, string ObjectType, string DOTNbr, string EquipCategory,
                         DateTime? AcquireDate, DateTime? RetireDate, DateTime? SoldDate, string ObjectPrefix, string OwningDistrict, string ProjectNbr,
                         decimal? TotalCtrReading_1, decimal? TotalCtrReading_2, string CtrReadingUom_1, string CtrReadingUom_2, string ShortDesc)
        {
            int rowsAffected = 0;
            // Prepares SQL statement (SP) and Execute

            try
            {
                SqlParameter[] paramsTPVInfo = new SqlParameter[18];
                paramsTPVInfo[0] = new SqlParameter("@VehicleId", vehicleId);
                paramsTPVInfo[1] = new SqlParameter("@EquipNbr", EquipNbr.Trim().Replace('\'', '"'));
                paramsTPVInfo[2] = new SqlParameter("@SAPEquipNbr", SAPEquipNbr.Trim().Replace('\'', '"'));
                paramsTPVInfo[3] = new SqlParameter("@LegacyEquipNbr", LegacyEquipNbr.Trim().Replace('\'', '"'));
                paramsTPVInfo[4] = new SqlParameter("@ObjectType", ObjectType.Trim().Replace('\'', '"'));
                paramsTPVInfo[5] = new SqlParameter("@DOTNbr", DOTNbr.Trim().Replace('\'', '"'));
                paramsTPVInfo[6] = new SqlParameter("@EquipCategory", EquipCategory.Trim().Replace('\'', '"'));
                paramsTPVInfo[7] = new SqlParameter("@AcquireDate", AcquireDate);
                paramsTPVInfo[8] = new SqlParameter("@RetireDate", RetireDate);
                paramsTPVInfo[9] = new SqlParameter("@SoldDate", SoldDate);
                paramsTPVInfo[10] = new SqlParameter("@ObjectPrefix", ObjectPrefix.Trim().Replace('\'', '"'));
                paramsTPVInfo[11] = new SqlParameter("@OwningDistrict", OwningDistrict.Trim().Replace('\'', '"'));
                paramsTPVInfo[12] = new SqlParameter("@ProjectNbr", ProjectNbr.Trim().Replace('\'', '"'));
                paramsTPVInfo[13] = new SqlParameter("@TotalCtrReading_1", TotalCtrReading_1);
                paramsTPVInfo[14] = new SqlParameter("@TotalCtrReading_2", TotalCtrReading_2);
                paramsTPVInfo[15] = new SqlParameter("@CtrReadingUom_1", CtrReadingUom_1.Trim().Replace('\'', '"'));
                paramsTPVInfo[16] = new SqlParameter("@CtrReadingUom_2", CtrReadingUom_2.Trim().Replace('\'', '"'));
                paramsTPVInfo[17] = new SqlParameter("@ShortDesc", ShortDesc.Trim().Replace('\'', '"'));

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_3rdPartyVehicleInfo_Update", paramsTPVInfo);
            }
            //try
            //{
            //    if (sqlExec.RequiredTransaction())
            //    {
            //        // 2. Attach current command SQL to transaction
            //        sqlExec.AttachToTransaction(sql);
            //    }
            //    // 3. Executes SQL statement
            //    rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            //}
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update 3rd party vehicle " + vehicleId + " additional info.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update 3rd party vehicle " + vehicleId + " additional info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update 3rd party vehicle " + vehicleId + " additional info.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This vehicle doesn't exist.");
            }
        }

        /// <summary>
		/// Set DayLight Savings.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetDayLightSaving(bool dayLightSaving)
		{
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfVehicleInfo SET DayLightSaving=" + Convert.ToInt16(dayLightSaving) + " WHERE AutoAdjustDayLightSaving=1";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update dayLightSaving=" + dayLightSaving.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update dayLightSaving=" + dayLightSaving.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Set AutoAdjustDayLightSaving.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="vehicleId"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetAutoAdjustDayLightSaving(Int64 vehicleId,bool autoAdjustDayLightSaving,bool dayLightSaving)
		{
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfVehicleInfo SET AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
							" ,DayLightSaving=" + Convert.ToInt16(dayLightSaving) + 
							" WHERE VehicleId=" + vehicleId;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update autoAdjustDayLightSaving=" + autoAdjustDayLightSaving.ToString() + " dayLightSaving=" + dayLightSaving.ToString() + " vehicleId=" + vehicleId .ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update autoAdjustDayLightSaving=" + autoAdjustDayLightSaving.ToString() + " dayLightSaving=" + dayLightSaving.ToString() + " vehicleId=" + vehicleId .ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Deletes existing vehicle.
		/// </summary>
		/// <param name="vehicleId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteVehicInfo(Int64 vehicleId)
		{
			return DeleteRowsByIntField("VehicleId",vehicleId, "vehicle id");		
		}
        /// <summary>
        /// Deletes vehicle working hours
        /// </summary>
        /// <param name="vehicleId"></param> 
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteVehicleWorkingHours(Int64 vehicleId)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "DELETE FROM vlfVehicleWorkingHours WHERE VehicleId=" + vehicleId;
            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete vehicle " + vehicleId + " working hours.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete vehicle " + vehicleId + " working hours.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns vehicle information by vehicle id. 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
        /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
        /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],
        /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving],
        /// [VehicleTypeId]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehicleInfoByVehicleId_NewTZ(Int64 vehicleId)
        {
            return GetActiveVehicleInfoBy_NewTZ("VehicleId", vehicleId);
        }
        //Changes for TimeZone Feature end
		/// <summary>
		/// Returns vehicle information by vehicle id. 
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
		/// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
		/// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],
		/// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving],
		/// [VehicleTypeId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehicleInfoByVehicleId(Int64 vehicleId)
		{
			return GetActiveVehicleInfoBy("VehicleId",vehicleId);
		}
        
        /// <summary>
        /// Returns vehicle additional information. 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[Field1],[Field2],[Field3],[Field4]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehicleAdditionalInfo(Int64 vehicleId)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "SELECT vlfVehicleInfo.VehicleId,ISNULL(vlfVehicleInfo.Field1,' ') AS Field1,ISNULL(vlfVehicleInfo.Field2,' ') AS Field2,ISNULL(vlfVehicleInfo.Field3,' ') AS Field3,ISNULL(vlfVehicleInfo.Field4,' ') AS Field4,ISNULL(vlfVehicleInfo.Field5,' ') AS Field5,ISNULL(VehicleWeight,-1) AS VehicleWeight,ISNULL(VehicleWtUnit,'') AS VehicleWtUnit,ISNULL(FuelCapacity,0) AS FuelCapacity,ISNULL(FuelBurnRate,0) AS FuelBurnRate FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId WHERE vlfVehicleInfo.VehicleId =" + vehicleId;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retreive vehicle " + vehicleId + " additional info.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retreive vehicle " + vehicleId + " additional info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Returns 3rd party vehicle additional information. 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[Equip#],[EquipCat],[Dates],[Readings</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet Get3rdPartyVehicleAdditionalInfo(Int64 vehicleId)
        {
            DataSet resultDataSet = null;
            try
            {
                //Executes SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);
                resultDataSet = sqlExec.SPExecuteDataset("usp_3rdPartyVehicleInfo_Get");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retreive 3rd party vehicle " + vehicleId + " additional info.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retreive 3rd party vehicle " + vehicleId + " additional info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Returns vehicle working hours. 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[WeekdayStart],[WeekdayEnd],[WeekendStart],[WeekendEnd]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehicleWorkingHours(Int64 vehicleId)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "SELECT VehicleId, WeekdayStart, WeekdayEnd, WeekendStart, WeekendEnd FROM vlfVehicleWorkingHours WHERE VehicleId =" + vehicleId;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retreive vehicle " + vehicleId + " working hours.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retreive vehicle " + vehicleId + " working hours.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }		
		/// <summary>
		/// Retrieves vehicle organization. 
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <returns>OrganizationId</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetVehicleOrganization(Int64 vehicleId)
		{
			int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				string sql = "SELECT OrganizationId FROM vlfVehicleInfo WHERE VehicleId=" + vehicleId;

            // 2. Attach current command SQL to transaction
            if (sqlExec.RequiredTransaction())
					sqlExec.AttachToTransaction(sql);

				//Executes SQL statement
				organizationId = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicles organization. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicles organization. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return organizationId;
		}		
		/// <summary>
		/// Returns vehicle information by box id. 
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
		/// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
		/// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName]
		/// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving],
		/// [VehicleTypeId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehicleInfoByBoxId(int boxId)
		{
			return GetActiveVehicleInfoBy("BoxId",boxId);
		}



      /// <summary>
      ///         the first function to call an udf --> dbo.GetVehicleTypeIdByBoxId
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns></returns>
      /// <comment>
      ///         added to be used for UpdateStatus of the vehicle (gb) 02/05/2011
      /// </comment>
      public short GetVehicleTypeIdByBoxId(int boxId)
      {
         string prefixMsg = string.Format("Unable to retrieve vehicle typeid for BID={0}", boxId);                           
         object objRet = null;
         Int16 retResult = VLF.CLS.Def.Const.unassignedShortValue;
         try
         {
            // lookup in the active assignments.
            // 2. Retrieves vehicle information.
            objRet = sqlExec.SQLExecuteScalar(string.Format("select dbo.GetVehicleTypeIdByBoxId({0})", boxId));
            if (objRet != System.DBNull.Value)
               retResult = Convert.ToInt16(objRet);
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

         return retResult;
      }

        // Changes for TimeZone feature start
      /// <summary>
      /// Returns vehicle information by license plate. 
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
      /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
      /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],
      /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving],
      /// [VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleInfoByLicensePlate_NewTZ(string licensePlate)
      {
          return GetActiveVehicleInfoBy_NewTZ("LicensePlate", licensePlate);
      }
      // Changes for TimeZone feature end


		/// <summary>
		/// Returns vehicle information by license plate. 
		/// </summary>
		/// <param name="licensePlate"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
		/// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
		/// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],
		/// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving],
		/// [VehicleTypeId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehicleInfoByLicensePlate(string licensePlate)
		{
			return GetActiveVehicleInfoBy("LicensePlate",licensePlate);
		}	
		/// <summary>
		/// Returns all vehicles information. 
		/// </summary>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllVehiclesInfo()
		{
			return GetActiveVehicleInfoBy("*",null);
		}		
		/// <summary>
		/// Get vehicle id by vin number from vehicle information table.
		/// </summary>
		/// <param name="vinNum"></param>
		/// <returns>Int64</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 GetVehicleIdByVinNumber(string vinNum)
		{
			DataSet sqlDataSet = null;
			Int64 retResult = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// 1. Prepares SQL statement to VehicleAssignment table.
				string sql = "SELECT VehicleId FROM " + tableName +
					" WHERE (VinNum='" + vinNum.Replace("'","''") + "')";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle id by vin number '" + vinNum + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle id by vin number '" + vinNum + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				//Retrieves info from Table[0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					retResult = Convert.ToInt32(currRow[0]);
					break;
				}
			}
			return retResult;
		}
		/// <summary>
		/// Get vehicle id by description from vehicle information table.
		/// </summary>
      /// <param name="description">Vehicle description</param>
      /// <param name="organizationId">organization Id</param>
      /// <returns>Int64 Vehicle Id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 GetVehicleIdByDescription(string description, int organizationId)
		{
			//DataSet sqlDataSet = null;
         Object resultValue = null;
			Int64 retResult = VLF.CLS.Def.Const.unassignedIntValue;
         string prefixMsg = "Unable to retrieve vehicle id by description '" + description + "'.";
            
			try
			{
				// 1. Prepares SQL statement to VehicleAssignment table.
            /*
				string sql = "SELECT VehicleId FROM " + tableName +
					" WHERE Description='" + description.Replace("'","''") + "'" +
					" AND OrganizationId=" + organizationId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
            */
            resultValue = 
               base.GetValueByFilter("VehicleId", "Description = @descr AND OrganizationId = @org",
               new SqlParameter("@descr", description), new SqlParameter("@org", organizationId));
            //sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			//if(sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0 && sqlDataSet.Tables[0].Rows.Count > 0)
         if (resultValue != null)
			{
            retResult = Convert.ToInt64(resultValue);
            //Convert.ToInt32(sqlDataSet.Tables[0].Rows[0][0]);
			}
			return retResult;
		}
		/// <summary>
		/// Get vehicle information field by license plate
		/// </summary>
		/// <param name="resultFieldName"></param>
		/// <param name="licensePlate"></param>
		/// <returns>Int64</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 GetVehicleInfoIntFieldByLicensePlate(string resultFieldName,string licensePlate)
		{
			DataSet sqlDataSet = null;
			Int64 retResult = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// lookup in the active assignments.
				// 1. Prepares SQL statement to VehicleAssignment table.
				string sql = "SELECT " + resultFieldName + " FROM vlfVehicle,vlfVehicleAssignment" +
					" WHERE (vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId)" +
					" AND (vlfVehicleAssignment.LicensePlate='" + licensePlate.Replace("'","''") + "')";
			
				// 2. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle information by license plate '" + licensePlate + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle information by license plate '" + licensePlate + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				//Retrieves info from Table[0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					retResult = Convert.ToInt64(currRow[0]);
					break;
				}
			}
			return retResult;
		}
		/// <summary>
		/// Get vehicle information field by license plate
		/// </summary>
		/// <param name="resultFieldName"></param>
		/// <param name="licensePlate"></param>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetVehicleInfoStrFieldByLicensePlate(string resultFieldName,string licensePlate)
		{
			DataSet sqlDataSet = null;
			string retResult = VLF.CLS.Def.Const.unassignedStrValue;
			try
			{
				// lookup in the active assignments.
				// 1. Prepares SQL statement to VehicleAssignment table.
				string sql = "SELECT " + resultFieldName + " FROM vlfVehicleInfo,vlfVehicleAssignment" +
					" WHERE (vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId)" +
					" AND (vlfVehicleAssignment.LicensePlate='" + licensePlate.Replace("'","''") + "')";
			
				// 2. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle information by license plate '" + licensePlate + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle information by license plate '" + licensePlate + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				//Retrieves info from Table[0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					retResult = Convert.ToString(currRow[0]);
					break;
				}
			}
			return retResult;
		}
		/// <summary>
		/// Updates field value by field name.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="licensePlate"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetVehicleInfoStrFieldByLicensePlate(string fieldName,string fieldValue,string licensePlate)
		{
			int rowsAffected = 0;
			try
			{
				string sql = "UPDATE " + tableName + " SET " + 
					fieldName + "='" + fieldValue.Replace("'","''") + "'" +
					" FROM vlfVehicleAssignment,vlfVehicleInfo" +
					" WHERE (vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId)" +
					" AND (vlfVehicleAssignment.LicensePlate='" + licensePlate + "')";
			
				if(sqlExec.RequiredTransaction())
				{
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				throw new DASAppResultNotFoundException(prefixMsg + " The vehicle license plate='" + licensePlate + "' does not exists.");
			}
		}
		/// <summary>
		/// Updates field value by field name.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="licensePlate"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetVehicleInfoDoubleFieldByLicensePlate(string fieldName,double fieldValue,string licensePlate)
		{
			int rowsAffected = 0;
			try
			{
				string sql = "UPDATE " + tableName + " SET " + 
					fieldName + "=" + fieldValue.ToString() + 
					" FROM vlfVehicleAssignment,vlfVehicleInfo" +
					" WHERE vlfVehicleAssignment.LicensePlate='" + licensePlate + "'" +
					" AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId";
			
				if(sqlExec.RequiredTransaction())
				{
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				throw new DASAppResultNotFoundException(prefixMsg + " The vehicle license plate='" + licensePlate + "' does not exists.");
			}
		}
		/// <summary>
		/// Updates field value by field name.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="licensePlate"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetVehicleInfoIntFieldByLicensePlate(string fieldName,Int64 fieldValue,string licensePlate)
		{
			int rowsAffected = 0;
			try
			{
				string sql = "UPDATE " + tableName + " SET " + 
					fieldName + "=" + fieldValue +
					" FROM vlfVehicleAssignment,vlfVehicleInfo" +
					" WHERE (vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId)" +
					" AND (vlfVehicleAssignment.LicensePlate='" + licensePlate + "')";
			
				if(sqlExec.RequiredTransaction())
				{
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to set new " + fieldName + " by license plate='" + licensePlate + "'. ";
				throw new DASAppResultNotFoundException(prefixMsg + " The vehicle license plate='" + licensePlate + "' does not exists.");
			}
		}
		/// <summary>
		/// Get all unassigned vehicle info
		/// </summary>
		/// <returns>DataSet [VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUnassignedVehiclesInfo(int organizationId)
		{
			DataSet resultDataSet = null;
			try
			{
				string sql = "SELECT VehicleId,VinNum,vlfVehicleInfo.MakeModelId,MakeName,ModelName,VehicleTypeName,StateProvince,ModelYear,Color,Description,CostPerMile" +
					" FROM vlfVehicleInfo,vlfMakeModel,vlfMake,vlfModel,vlfVehicleType" +
					" WHERE vlfVehicleInfo.VehicleId NOT IN (SELECT DISTINCT VehicleId FROM vlfVehicleAssignment)" +
					" AND vlfVehicleInfo.MakeModelId=vlfMakeModel.MakeModelId" +
					" AND vlfMakeModel.MakeId=vlfMake.MakeId" +
					" AND vlfMakeModel.ModelId=vlfModel.ModelId" +
					" AND vlfVehicleInfo.VehicleTypeId=vlfVehicleType.VehicleTypeId" + 
					" AND vlfVehicleInfo.OrganizationId=" + organizationId;
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve unassign vehicles info. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve unassign vehicles info. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Returns vehicle information by query field. 
		/// </summary>
		/// <remarks>
		/// Suitable only for query fields:
		/// - "LicensePlate"
		/// - "VehicleId"
		/// - "BoxId"
		/// </remarks>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
		/// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
		/// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName]
		/// [OrganizationName]
		/// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving],
		/// [VehicleTypeId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /*
            protected DataSet GetActiveVehicleInfoBy(string fieldName, object fieldValue)
            {
               DataSet resultDataSet = null;
               try
               {
                  sqlExec.ClearCommandParameters() ;
                  switch (fieldName) 
                  {
                     case "LicensePlate":                        
                        sqlExec.AddCommandParam("@LicensePlate", SqlDbType.Char, Convert.ToString(fieldValue).Replace("'", "''"));
                        return sqlExec.SPExecuteDataset("sp_GetActiveVehicleInfoByLicensePlate");
                     case "BoxId":
                        sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, Convert.ToInt64(fieldValue));
                        return sqlExec.SPExecuteDataset("sp_GetActiveVehicleInfoByBoxId");
                     case "VehicleId":
                        sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, Convert.ToInt64(fieldValue));
                        return sqlExec.SPExecuteDataset("sp_GetActiveVehicleInfoByVehicleId");
                     case "*":
                        return sqlExec.SPExecuteDataset("sp_GetAllVehiclesInfo");
                     default:
                        return null ;
                  }
               }
               catch (SqlException objException)
               {            
                  Util.ProcessDbException("GetActiveVehicleInfoBy -> SqlException ", objException);
               }
               catch (DASDbConnectionClosed exCnn)
               {
                  throw new DASDbConnectionClosed(exCnn.Message);
               }
               catch (Exception objException)
               {
                  Util.BTrace(Util.ERR2, "GetActiveVehicleInfoBy -> Exception[0]", objException.Message);            
                  throw new DASException("GetActiveVehicleInfoBy -> Exception " + objException.Message);
               }
               return resultDataSet;
            }
       */

        // Changes for TimeZone Feature start
        protected DataSet GetActiveVehicleInfoBy_NewTZ(string fieldName, object fieldValue)
        {
            DataSet resultDataSet = null;
            string firstWhere = "";
            try
            {
                if (fieldName == "LicensePlate")
                    firstWhere = "(vlfVehicleAssignment.LicensePlate='" + Convert.ToString(fieldValue).Replace("'", "''") + "') AND ";
                else if ((fieldName == "BoxId") || (fieldName == "VehicleId"))
                    firstWhere = "(vlfVehicleAssignment." + fieldName + "=" + Convert.ToInt64(fieldValue) + ") AND ";
                else if (fieldName != "*")
                    return null;
                string sql = "SELECT LicensePlate,vlfBox.BoxId,vlfVehicleAssignment.VehicleId,VinNum," +
                         "vlfVehicleInfo.MakeModelId,MakeName,ModelName,VehicleTypeName," +
                         "StateProvince,ModelYear,Color,vlfVehicleInfo.Description,CostPerMile,vlfVehicleInfo.OrganizationId," +
                         "vlfVehicleInfo.IconTypeId,vlfIconType.IconTypeName," +
                         "vlfOrganization.OrganizationName," +
                         "ISNULL(vlfVehicleInfo.Email,' ') AS Email," +
                         "ISNULL(vlfVehicleInfo.Phone,' ') AS Phone," +
                         "ISNULL(vlfVehicleInfo.TimeZoneNew,0) AS TimeZone," +
                         "ISNULL(vlfVehicleInfo.DayLightSaving,0) AS DayLightSaving," +
                         "ISNULL(vlfVehicleInfo.FormatType,0) AS FormatType," +
                         "ISNULL(vlfVehicleInfo.Notify,0) AS Notify," +
                         "ISNULL(vlfVehicleInfo.Warning,0) AS Warning," +
                         "ISNULL(vlfVehicleInfo.Critical,0) AS Critical," +
                         "ISNULL(vlfVehicleInfo.AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving," +
                         "vlfVehicleInfo.VehicleTypeId," +
                         "vlfBox.FwAttributes1," +
                    // new 4 fields - Max 8/14/2006
                          "ISNULL(vlfVehicleInfo.Field1,' ') AS Field1," +
                          "ISNULL(vlfVehicleInfo.Field2,' ') AS Field2," +
                          "ISNULL(vlfVehicleInfo.Field3,' ') AS Field3," +
                          "ISNULL(vlfVehicleInfo.Field4,' ') AS Field4, " +
                    // end new 4 fields 
                         "ISNULL(vlfVehicleInfo.Maintenance,0) AS Maintenance, " +
                          "DBO.UDFServiceConfigIDByVehicleId(vlfVehicleAssignment.VehicleId) AS ServiceConfigID, " +
                          "ISNULL(LastEngineHour,0) as EngineHour, " +
                          "case when ISNULL(lastTVD,0)<>0 then lastTVD else ISNULL(LastOdo,0) end as Odo, " +
                          "ISNULL(vlfVehicleInfo.Class,' ') AS Class " +
                         " FROM vlfVehicleAssignment,vlfVehicleInfo,vlfMakeModel,vlfMake,vlfModel,vlfVehicleType,vlfIconType, vlfOrganization,vlfBox with (nolock)" +
                         " WHERE " + firstWhere +
                         " vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
                         " AND vlfVehicleInfo.MakeModelId=vlfMakeModel.MakeModelId" +
                         " AND vlfMakeModel.MakeId=vlfMake.MakeId" +
                         " AND vlfMakeModel.ModelId=vlfModel.ModelId" +
                         " AND vlfVehicleInfo.VehicleTypeId=vlfVehicleType.VehicleTypeId" +
                         " AND vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId" +
                         " AND vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                         " AND vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId";
                //Executes SQL statement

                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "";
                if (firstWhere != "*")
                    prefixMsg = "Unable to retreive vehicle info by " + fieldName + ". ";
                else
                    prefixMsg = "Unable to retreive vehicles info. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "";
                if (firstWhere != "*")
                    prefixMsg = "Unable to retreive vehicle info by " + fieldName + ". ";
                else
                    prefixMsg = "Unable to retreive vehicles info. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }	
      // Changes for timezone Feature end

      protected DataSet GetActiveVehicleInfoBy(string fieldName,object fieldValue)
            {
               DataSet resultDataSet = null; 
               string firstWhere = "";
               try
               {
                  if(fieldName == "LicensePlate")
                     firstWhere = "(vlfVehicleAssignment.LicensePlate='" +  Convert.ToString(fieldValue).Replace("'","''") + "') AND ";
                  else if((fieldName == "BoxId")||(fieldName == "VehicleId"))
                     firstWhere = "(vlfVehicleAssignment." + fieldName + "=" + Convert.ToInt64(fieldValue) + ") AND ";
                  else if(fieldName != "*")
                     return null;
                  string sql = "SELECT LicensePlate,vlfBox.BoxId,vlfVehicleAssignment.VehicleId,VinNum,"+
                           "vlfVehicleInfo.MakeModelId,MakeName,ModelName,VehicleTypeName,"+
                           "StateProvince,ModelYear,Color,vlfVehicleInfo.Description,CostPerMile,vlfVehicleInfo.OrganizationId,"+
                           "vlfVehicleInfo.IconTypeId,vlfIconType.IconTypeName,"+
                           "vlfOrganization.OrganizationName,"+
                           "ISNULL(vlfVehicleInfo.Email,' ') AS Email,"+
                           "ISNULL(vlfVehicleInfo.Phone,' ') AS Phone," +
                           "ISNULL(vlfVehicleInfo.TimeZone,0) AS TimeZone,"+
                           "ISNULL(vlfVehicleInfo.DayLightSaving,0) AS DayLightSaving,"+
                           "ISNULL(vlfVehicleInfo.FormatType,0) AS FormatType,"+
                           "ISNULL(vlfVehicleInfo.Notify,0) AS Notify,"+
                           "ISNULL(vlfVehicleInfo.Warning,0) AS Warning,"+
                           "ISNULL(vlfVehicleInfo.Critical,0) AS Critical,"+
                           "ISNULL(vlfVehicleInfo.AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving,"+
                           "vlfVehicleInfo.VehicleTypeId,"+
                           "vlfBox.FwAttributes1," +
                    // new 4 fields - Max 8/14/2006
                            "ISNULL(vlfVehicleInfo.Field1,' ') AS Field1," +
                            "ISNULL(vlfVehicleInfo.Field2,' ') AS Field2," +
                            "ISNULL(vlfVehicleInfo.Field3,' ') AS Field3," +
                            "ISNULL(vlfVehicleInfo.Field4,' ') AS Field4, " +
                      // end new 4 fields 
                           "ISNULL(vlfVehicleInfo.Maintenance,0) AS Maintenance, " +
                            "DBO.UDFServiceConfigIDByVehicleId(vlfVehicleAssignment.VehicleId) AS ServiceConfigID, " +
                            "ISNULL(LastEngineHour,0) as EngineHour, " +
                            "case when ISNULL(lastTVD,0)<>0 then lastTVD else ISNULL(LastOdo,0) end as Odo, " +
                            "ISNULL(vlfVehicleInfo.Class,' ') AS Class " +
                           " FROM vlfVehicleAssignment,vlfVehicleInfo,vlfMakeModel,vlfMake,vlfModel,vlfVehicleType,vlfIconType, vlfOrganization,vlfBox with (nolock)" +
                           " WHERE " + firstWhere +
                           " vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
                           " AND vlfVehicleInfo.MakeModelId=vlfMakeModel.MakeModelId" +
                           " AND vlfMakeModel.MakeId=vlfMake.MakeId" +
                           " AND vlfMakeModel.ModelId=vlfModel.ModelId" +
                           " AND vlfVehicleInfo.VehicleTypeId=vlfVehicleType.VehicleTypeId" +
                           " AND vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId" +
                           " AND vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                           " AND vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId";
                  //Executes SQL statement
                  
                  resultDataSet = sqlExec.SQLExecuteDataset(sql);
               }
               catch (SqlException objException) 
               {
                  string prefixMsg = "";
                  if(firstWhere != "*")
                     prefixMsg = "Unable to retreive vehicle info by " + fieldName + ". ";
                  else
                     prefixMsg = "Unable to retreive vehicles info. ";
                  Util.ProcessDbException(prefixMsg, objException);
               }
               catch(DASDbConnectionClosed exCnn)
               {
                  throw new DASDbConnectionClosed(exCnn.Message);
               }
               catch(Exception objException)
               {
                  string prefixMsg = "";
                  if(firstWhere != "*")
                     prefixMsg = "Unable to retreive vehicle info by " + fieldName + ". ";
                  else
                     prefixMsg = "Unable to retreive vehicles info. ";
                  throw new DASException(prefixMsg + " " + objException.Message);
               }
               return resultDataSet;
            }	
      
      /// <summary>
		/// Gets all Vehicles active assignment information
		/// </summary>
		/// <remarks>
		/// TableName	= "AllActiveVehiclesAssignments"
		/// DataSetName = "Vehicle"
		/// </remarks>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],
		/// [MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],
		/// [ModelYear],[Color],[Description],[CostPerMile],
		/// [IconTypeId],[IconTypeName],
		/// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllVehiclesActivesInfo(int organizationId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT DISTINCT vlfVehicleAssignment.BoxId,vlfVehicleAssignment.LicensePlate,vlfVehicleAssignment.VehicleId,vlfVehicleInfo.VinNum,vlfVehicleInfo.MakeModelId,"+ 
							"vlfMake.MakeName, vlfModel.ModelName, vlfVehicleType.VehicleTypeName, vlfVehicleInfo.StateProvince, vlfVehicleInfo.ModelYear,"+
							"vlfVehicleInfo.Color, vlfVehicleInfo.Description, vlfVehicleInfo.CostPerMile,vlfIconType.IconTypeId,IconTypeName,"+
							"ISNULL(vlfVehicleInfo.Email,' ') AS Email,"+
							"ISNULL(vlfVehicleInfo.TimeZone,0) AS TimeZone,"+
							"ISNULL(vlfVehicleInfo.DayLightSaving,0) AS DayLightSaving,"+
							"ISNULL(vlfVehicleInfo.FormatType,0) AS FormatType,"+
							"ISNULL(vlfVehicleInfo.Notify,0) AS Notify,"+
							"ISNULL(vlfVehicleInfo.Warning,0) AS Warning,"+
							"ISNULL(vlfVehicleInfo.Critical,0) AS Critical,"+
							"ISNULL(vlfVehicleInfo.AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving"+
							" FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId"+
							" INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId"+
							" INNER JOIN vlfMakeModel ON vlfVehicleInfo.MakeModelId = vlfMakeModel.MakeModelId"+
							" INNER JOIN vlfModel ON vlfMakeModel.ModelId = vlfModel.ModelId"+
							" INNER JOIN vlfMake ON vlfMakeModel.MakeId = vlfMake.MakeId"+
							" INNER JOIN vlfIconType ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId"+
							" WHERE vlfVehicleInfo.OrganizationId=" + organizationId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve active vehicles info for organization " + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve active vehicles info for organization " + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}	
		/// </summary>
		/// <remarks>
		/// TableName	= "AllActiveVehiclesAssignments"
		/// DataSetName = "Vehicle"
		/// </remarks>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],
		/// [MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],
		/// [ModelYear],[Color],[Description],[CostPerMile],
		/// [IconTypeId],[IconTypeName],
		/// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserAllVehiclesActiveInfo(int userId)
		{
			DataSet resultDataSet = null;
			try
			{

                //sqlExec.CommandTimeout = 600; 
                //// 1. Prepares SQL statement
                //string sql = "SELECT DISTINCT vlfVehicleAssignment.BoxId,vlfVehicleAssignment.LicensePlate,vlfVehicleAssignment.VehicleId,vlfVehicleInfo.VinNum,vlfVehicleInfo.MakeModelId,"+ 
                //    "vlfMake.MakeName, vlfModel.ModelName, vlfVehicleType.VehicleTypeName, vlfVehicleInfo.StateProvince, vlfVehicleInfo.ModelYear,"+
                //    "vlfVehicleInfo.Color, vlfVehicleInfo.Description, vlfVehicleInfo.CostPerMile,vlfIconType.IconTypeId,IconTypeName,"+
                //    "ISNULL(vlfVehicleInfo.Email,' ') AS Email,"+
                //    "ISNULL(vlfVehicleInfo.TimeZone,0) AS TimeZone,"+
                //    "ISNULL(vlfVehicleInfo.DayLightSaving,0) AS DayLightSaving,"+
                //    "ISNULL(vlfVehicleInfo.FormatType,0) AS FormatType,"+
                //    "ISNULL(vlfVehicleInfo.Notify,0) AS Notify,"+
                //    "ISNULL(vlfVehicleInfo.Warning,0) AS Warning,"+
                //    "ISNULL(vlfVehicleInfo.Critical,0) AS Critical,"+
                //    "ISNULL(vlfVehicleInfo.AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving"+
                //    " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId"+
                //    " INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId"+
                //    " INNER JOIN vlfMakeModel ON vlfVehicleInfo.MakeModelId = vlfMakeModel.MakeModelId"+
                //    " INNER JOIN vlfModel ON vlfMakeModel.ModelId = vlfModel.ModelId"+
                //    " INNER JOIN vlfMake ON vlfMakeModel.MakeId = vlfMake.MakeId"+
                //    " INNER JOIN vlfIconType ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId"+
                //    " INNER JOIN vlfFleetVehicles ON vlfVehicleInfo.VehicleId = vlfFleetVehicles.VehicleId"+
                //    " INNER JOIN vlfFleetUsers ON vlfFleetVehicles.FleetId = vlfFleetUsers.FleetId"+
                //    " WHERE vlfFleetUsers.UserId=" + userId +
                //    " ORDER BY vlfVehicleInfo.Description";
                //if(sqlExec.RequiredTransaction())
                //{
                //    // 2. Attaches SQL to transaction
                //    sqlExec.AttachToTransaction(sql);
                //}
                //// 3. Executes SQL statement
                //resultDataSet = sqlExec.SQLExecuteDataset(sql);


                string sql = "GetUserAllVehiclesActiveInfo";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@userId", userId);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);

			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve active vehicles info for user " + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve active vehicles info for user " + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}	
		/// <summary>
		/// Gets all vehicles active assignment configuration information for current organization
		/// </summary>
		/// <param name="organizationId"></param>
        /// <returns>DataSet [Description],[BoxId],[FwId],[FwName],[FwDateReleased],[CommModeId],[BoxProtocolTypeId],[FwTypeId],[OAPPort]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetOrganizationAllActiveVehiclesCfgInfo(int organizationId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
                string sql = "SELECT DISTINCT vlfVehicleInfo.Description, vlfBox.BoxId, vlfFirmwareChannels.FwId, vlfFirmware.FwName, vlfFirmware.FwDateReleased, vlfChannels.CommModeId, vlfChannels.BoxProtocolTypeId, vlfFirmware.FwTypeId,OAPPort FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfVehicleInfo.OrganizationId =" + organizationId + " AND vlfFirmwareChannels.ChPriority = 0 AND (vlfFirmware.FwTypeId = " + (short)VLF.CLS.Def.Enums.FirmwareType.SentinelFM + " OR vlfFirmware.FwTypeId = " + (short)VLF.CLS.Def.Enums.FirmwareType.Bantek + ") ORDER BY vlfFirmware.FwDateReleased DESC, vlfVehicleInfo.Description";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve active vehicles info for organization " + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve active vehicles info for organization " + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

        public DataSet GetInspectionItemsByGroup(int groupId)
        {
            DataSet resultDataSet = null;
            string filter = null;
            string sql = null;
            try
            {

                sql = @"select InspectionItemID, Defect from LogData_InspectionGroup g (nolock)
                        inner join  LogData_InspectionCategory c (nolock)
                        on g.GroupId = c.GroupId
                        inner join LogData_InspectionItem i (nolock)
                        on c.CategoryID = i.CategoryID
                        where g.GroupId = "
                        + groupId.ToString() +
                        " order by Defect";

                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Inspection Items";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization vehicles";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultDataSet;
        }

        public string checkPPCID(string driverId, DateTime insDate)
        {
            try
            {
                string sql = string.Format(@"select PPCID from (select top 1 PPCID, case PPCID when 'Phone' then 1 when 'OCR' then 2 else 0 end ord
                            from 
                                LogData_Reference r (nolock) 
                                inner join LogData_Inspection i (nolock)
                                on r.RefID = i.RefID
                            where
                                r.driver='{0}' and i.InsTime='{1}'
                                order by ord asc) a
                            ", driverId, insDate);

                //Executes SQL statement
                object ret = sqlExec.SQLExecuteScalar(sql);
                if (ret == null)
                {
                    return "";
                }
                else
                {
                    return ret.ToString();
                }
            }
            catch (SqlException objException)
            {
                throw new Exception(objException.Message);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicles organization. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }		


        public void SaveInspectionCN(
                DateTime insTime,
                string equipID,
                int odometer,
                string defect,
                string driverId,
                bool signed,
                int attachementId,
                bool IsSatisfactoryDrive,
                bool IsDefectsCorrected,
                bool IsMechanicSignedDefect,
                bool IsDriverSignedDefect,
                string Remarks,
                string fileDestPath,
                int inspectionGroupId
            )
        {
            try
            {
                string sql = "LogData_ProcessInspectionAndDefects_CN";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@InsTime", SqlDbType.DateTime, insTime);
                sqlExec.AddCommandParam("@EquipID", SqlDbType.VarChar, equipID);
                sqlExec.AddCommandParam("@Odometer", SqlDbType.Int, odometer);
                sqlExec.AddCommandParam("@Defect", SqlDbType.VarChar, defect);
                sqlExec.AddCommandParam("@DriverId", SqlDbType.VarChar, driverId);
                sqlExec.AddCommandParam("@Signed", SqlDbType.Bit, signed);
                sqlExec.AddCommandParam("@AttachmentId", SqlDbType.Int, attachementId);
                sqlExec.AddCommandParam("@IsSatisfactoryDrive", SqlDbType.Bit, IsSatisfactoryDrive);
                sqlExec.AddCommandParam("@IsDefectsCorrected", SqlDbType.Bit, IsDefectsCorrected);
                sqlExec.AddCommandParam("@IsMechanicSignedDefect", SqlDbType.Bit, IsMechanicSignedDefect);
                sqlExec.AddCommandParam("@IsDriverSignedDefect", SqlDbType.Bit, IsDriverSignedDefect);
                sqlExec.AddCommandParam("@Remarks", SqlDbType.NVarChar, Remarks);
                sqlExec.AddCommandParam("@fileDestPath", SqlDbType.VarChar, fileDestPath);
                sqlExec.AddCommandParam("@inspectionGroupId", SqlDbType.VarChar, driverId);

                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.AttachToTransaction(sql);
                }

                sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                throw new DASException(objException.Message);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to SaveInspectionCN Driver Id=" + driverId + " insTime=" + insTime.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

        }

        public DataSet GetCNVehicleInfoByType(int Type)
        { //1-Truck, 2-Bus, 3-Truck and Bus, 4-Trailer
            DataSet resultDataSet = null;
            string filter = null;
            string sql = null;
            try
            {

                switch (Type)
                {
                    case 1:
                        filter = "NOT vt.VehicleTypeName Like '%Bus%' AND NOT vt.VehicleTypeName Like '%Trailer%'";
                        break;
                    case 2:
                        filter = "vt.VehicleTypeName Like '%Bus%'";
                        break;
                    case 3:
                        filter = "NOT vt.VehicleTypeName Like '%Trailer%'";
                        break;
                    case 4:
                        filter = "vt.VehicleTypeName Like '%Trailer%'";
                        break;
                }

                sql = @"select v.VehicleId, v.[Description], LicensePlate 
                    from vlfVehicleInfo v (nolock) 
                    inner join vlfVehicleType vt (nolock) 
                    on v.VehicleTypeId = vt.VehicleTypeId 
                    inner join vlfVehicleAssignment va (nolock) 
                    on v.VehicleId = va.VehicleId 
                    where v.OrganizationId=123 AND " + filter
                    + " order by v.[Description], LicensePlate";

                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve organization vehicles";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve organization vehicles";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultDataSet;
        }
        /// <summary>
      /// Get all organization vehicles
      /// </summary>
      /// <returns>DataSet [VehicleId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetOrganizationVehicles(int orgId)
      {
         DataSet resultDataSet = null;
         try
         {
            string sql = "SELECT VehicleId FROM " + this.tableName +
                " WHERE OrganizationId=" + orgId.ToString();
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve organization vehicles";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve organization vehicles";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }      
		/// <summary>
		/// Gets all vehicles active assignment configuration information for current fleet
		/// </summary>
		/// <param name="fleetId"></param>
        /// <returns>DataSet [Description],[BoxId],[FwId],[FwName],[FwDateReleased],[CommModeId],[BoxProtocolTypeId],[FwTypeId],[OAPPort]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFleetAllActiveVehiclesCfgInfo(int fleetId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
                string sql = "SELECT DISTINCT vlfVehicleInfo.Description, vlfBox.BoxId, vlfFirmwareChannels.FwId, vlfFirmware.FwName, vlfFirmware.FwDateReleased, vlfChannels.CommModeId, vlfChannels.BoxProtocolTypeId, vlfFirmware.FwTypeId, OAPPort FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfFleetVehicles ON vlfVehicleInfo.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFleetVehicles.FleetId =" + fleetId + " AND vlfFirmwareChannels.ChPriority = 0 AND (vlfFirmware.FwTypeId = " + (short)VLF.CLS.Def.Enums.FirmwareType.SentinelFM + " OR vlfFirmware.FwTypeId = " + (short)VLF.CLS.Def.Enums.FirmwareType.Bantek + ") ORDER BY vlfFirmware.FwDateReleased DESC, vlfVehicleInfo.Description";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve active vehicles info for fleet " + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve active vehicles info for fleet " + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}	
		/// <summary>
		/// Gets vehicle active assignment configuration information
		/// </summary>
		/// <param name="vehicleId"></param>
        /// <returns>DataSet [Description],[BoxId],[FwId],[FwName],[FwDateReleased],[CommModeId],[BoxProtocolTypeId],[FwTypeId],[OAPPort]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetActiveVehicleCfgInfo(Int64 vehicleId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
                string sql = "SELECT DISTINCT vlfVehicleInfo.Description, vlfBox.BoxId, vlfFirmwareChannels.FwId, vlfFirmware.FwName, vlfFirmware.FwDateReleased, vlfChannels.CommModeId, vlfChannels.BoxProtocolTypeId, vlfFirmware.FwTypeId, OAPPort FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfVehicleInfo.VehicleId = " + vehicleId + " AND vlfFirmwareChannels.ChPriority = 0 AND (vlfFirmware.FwTypeId = " + (short)VLF.CLS.Def.Enums.FirmwareType.SentinelFM + " OR vlfFirmware.FwTypeId = " + (short)VLF.CLS.Def.Enums.FirmwareType.Bantek + ") ORDER BY vlfFirmware.FwDateReleased DESC, vlfVehicleInfo.Description";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve active vehicles info for vehicle " + vehicleId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve active vehicles info for vehicle " + vehicleId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}


        /// <summary>
        /// Get Vehicle Last Known Position Info By BoxId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public DataSet GetVehicleLastKnownPositionInfoByBoxId(int userId, int boxId, string language)
        {
            DataSet resultDataSet = null;
            try
            {

                string sql = "sp_GetVehicleLastKnownPositionInfoByBoxId";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@boxId", boxId);
                sqlParams[2] = new SqlParameter("@language", language);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicle info by GetVehicleLastKnownPositionInfoByBoxId - BoxId" + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicle info by GetVehicleLastKnownPositionInfoByBoxId - BoxId" + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }


       /// <summary>
       /// Get Vehicle/Box BSM Maintenance information
       /// </summary>
       /// <param name="vehicleId"></param>
       /// <returns></returns>
        public DataSet GetVehicleExtraServiceHistoryByVehicleId(Int64  vehicleId)
        {
            DataSet resultDataSet = null;
            try
            {

                string sql = "GetVehicleExtraServiceHistory";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
                
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleExtraServiceHistoryByVehicleId: vehicleId" + vehicleId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicle info by GetVehicleExtraServiceHistoryByVehicleId:  vehicleId" + vehicleId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Update Vehicle/Box BSM Maintenance information
        /// </summary>
        /// <param name="VehicleId"></param>
        /// <param name="Field1"></param>
        /// <param name="Field2"></param>
        /// <param name="Field3"></param>
        /// <param name="Field4"></param>
        /// <param name="Field5"></param>
        /// <returns></returns>
        public int VehicleExtraServiceHistory_Add_Update(Int64 VehicleId, string Field1, string Field2, string Field3, string Field4, string Field5, string Field6, string Field7,
            string Field8, string Field9, string Field10, string Field11, string Field12, string Field13, string Field14, string Field15, string Field16)
        {
            int rowsAffected = 0;
            try
            {

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, VehicleId);
                sqlExec.AddCommandParam("@Field1", SqlDbType.VarChar, Field1);
                sqlExec.AddCommandParam("@Field2", SqlDbType.VarChar, Field2);
                sqlExec.AddCommandParam("@Field3", SqlDbType.VarChar, Field3);
                sqlExec.AddCommandParam("@Field4", SqlDbType.VarChar, Field4);
                sqlExec.AddCommandParam("@Field5", SqlDbType.VarChar, Field5);
                sqlExec.AddCommandParam("@Field6", SqlDbType.VarChar, Field6);
                sqlExec.AddCommandParam("@Field7", SqlDbType.VarChar, Field7);
                sqlExec.AddCommandParam("@Field8", SqlDbType.VarChar, Field8);
                sqlExec.AddCommandParam("@Field9", SqlDbType.VarChar, Field9);
                sqlExec.AddCommandParam("@Field10", SqlDbType.VarChar, Field10);
                sqlExec.AddCommandParam("@Field11", SqlDbType.VarChar, Field11);
                sqlExec.AddCommandParam("@Field12", SqlDbType.VarChar, Field12);
                sqlExec.AddCommandParam("@Field13", SqlDbType.VarChar, Field13);
                sqlExec.AddCommandParam("@Field14", SqlDbType.VarChar, Field14);
                sqlExec.AddCommandParam("@Field15", SqlDbType.VarChar, Field15);
                sqlExec.AddCommandParam("@Field16", SqlDbType.VarChar, Field16);

              
                
                rowsAffected = sqlExec.SPExecuteNonQuery("VehicleExtraServiceHistory_AddUpdate");
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add or Update VehicleExtraServiceHistory. VehicleId: " + VehicleId ;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add or Update VehicleExtraServiceHistory. VehicleId: " + VehicleId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }


        /// <summary>
        /// Deletes Extra Service History
        /// </summary>
        /// <param name="vehicleId"></param> 
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteVehicleExtraServiceHistory(Int64 vehicleId)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "DELETE FROM vlfVehicleExtraServiceHistory WHERE VehicleId=" + vehicleId;
            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete vehicle " + vehicleId + " Extra Service History.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete vehicle " + vehicleId + " Extra Service History.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }


        /// <summary>
        /// Get Vehicle/Box BSM Extra information like: Status, Firmware
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public DataSet GetBoxExtraInfo(Int64 vehicleId)
        {
            DataSet resultDataSet = null;
            try
            {

                string sql = "GetBoxExtraInfo";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);

                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetBoxExtraInfo: vehicleId" + vehicleId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicle info by GetBoxExtraInfo:  vehicleId" + vehicleId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Swap Vehicle
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public int swapVehicle(Int64 vehicleId)
        {
            int rowsAffected = 0;
            
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@vehicleId", vehicleId);

                rowsAffected = sqlExec.SPExecuteNonQuery("sp_swapBox", param);
            }
            
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to swap vehicle " + vehicleId + ".";
                Util.ProcessDbException(prefixMsg, objException);
                rowsAffected = 0;
            }
            catch
            {
                rowsAffected = 0;
            }
            
            return rowsAffected;
        }

        /// <summary>
        /// Gets vehicle device statuses
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="VehicleId"></param>
        /// <returns>dataset</returns>
        public DataSet GetVehicleDeviceStatuses(int UserId, int VehicleId)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "usp_VehicleStatuses_Get";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@vehicleId", VehicleId);
                sqlParams[1] = new SqlParameter("@UserId", UserId);

                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicle statuses by GetVehicleDeviceStatuses - VehicleId: " + VehicleId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicle statuses by GetVehicleDeviceStatuses - VehicleId: " + VehicleId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Updates vehicle device status
        /// </summary>
        /// <param name="VehicleDeviceStatusID"></param>
        /// <param name="StatusDate"></param>
        /// <param name="AuthorizationNo"></param>
        /// <param name="VehicleId"></param>
        /// <param name="UserId"></param>
        /// <returns>updated VehicleId</returns>
        public int UpdateVehicleDeviceStatus(int VehicleDeviceStatusID, string StatusDate, string AuthorizationNo, int VehicleId, int UserId, string Address, double Latitude, double Longitude)
        {
            int UpdatedVehicleID = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@VehicleDeviceStatusID", SqlDbType.Int, ParameterDirection.Input, VehicleDeviceStatusID);
                sqlExec.AddCommandParam("@StatusDate", SqlDbType.VarChar, ParameterDirection.Input, StatusDate);
                sqlExec.AddCommandParam("@AuthorizationNo", SqlDbType.VarChar, ParameterDirection.Input, AuthorizationNo);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, ParameterDirection.Input, VehicleId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@Address", SqlDbType.VarChar, ParameterDirection.Input, Address);
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, ParameterDirection.Input, Latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, ParameterDirection.Input, Longitude);
                sqlExec.AddCommandParam("@UpdatedVehicleID", SqlDbType.Int, ParameterDirection.Output, 4, UpdatedVehicleID);

                int res = sqlExec.SPExecuteNonQuery("usp_VehicleStatus_Update");

                UpdatedVehicleID = (DBNull.Value == sqlExec.ReadCommandParam("@UpdatedVehicleID")) ?
                              UpdatedVehicleID : Convert.ToInt32(sqlExec.ReadCommandParam("@UpdatedVehicleID"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update vehicle device status - VehicleDeviceStatusID={0}, VehicleId={1}, UserId={2}.", VehicleDeviceStatusID.ToString(), VehicleId.ToString(), UserId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update vehicle device status - VehicleDeviceStatusID={0}, VehicleId={1}, UserId={2}.", VehicleDeviceStatusID.ToString(), VehicleId.ToString(), UserId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return UpdatedVehicleID;
        }

        /// <summary>
        ///         Returns offset for Engine Hours in seconds
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        /// <comment>
        ///         used by Red-D-Arc and similar organizations
        /// </comment>
        public int GetVehicleEngineHourOffset(long vehicleId)
        {
            string prefixMsg = string.Format("Unable to retrieve vehicle engine hour offset for vehicleId={0}", vehicleId);
            object objRet = null;
            Int32 retResult = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                objRet = sqlExec.SQLExecuteScalar(string.Format("SELECT ISNULL(dbo.udf_GetVehicleEngineHourOffset({0}), 0)", vehicleId));
                if (objRet != System.DBNull.Value)
                    retResult = Convert.ToInt32(objRet);
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

            return retResult;
        }

        private string getOCRLogImagePath(string refId)
        {
            DataSet sqlDataSet = null;
            string retResult = null;

            try
            {

                // lookup in the active assignments.
                // 1. Prepares SQL statement to VehicleAssignment table.
                string sql = @"select top 1 LogImagePath
                            from LogData_Log_CN lcn (nolock)
                            where RefId=" + refId;

                // 2. Retrieves vehicle assignment information.
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "(getOCRLogImagePath) Unable to OCR log image file  refId='" + refId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "(getOCRLogImagePath) Unable to OCR log image file  refId='" + refId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if ((sqlDataSet != null) && (sqlDataSet.Tables[0].Rows.Count > 0))
            {
                //Retrieves info from Table[0]
                DataRow currRow = sqlDataSet.Tables[0].Rows[0];
                retResult = Convert.ToString(currRow[0]);
            }
            sqlDataSet = null;

            return retResult;
        }

        private string getOCRInspectionImagePath(string refId, string time)
        {
            DataSet sqlDataSet = null;
            string retResult = null;

            try
            {
                if (time == null || time.Length < 5)
                {
                    return null;
                }

                string[] ar = time.Split(':');
                if (ar.Length != 2)
                {
                    return null;
                }

                // lookup in the active assignments.
                // 1. Prepares SQL statement to VehicleAssignment table.
                string sql = @"select top 1 InspectionImagePath
                            from LogData_Inspection_CN icn (nolock)
                            inner join LogData_Inspection i (nolock)
                            on icn.InspectionRowId = i.RowID
                            where RefId=" + refId + " and DATEPART(hour, InsTime) = " + ar[0] + " and DATEPART(minute,InsTime) = " + ar[1]
                            + " order by InsTime desc";

                // 2. Retrieves vehicle assignment information.
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "(getOCRInspectionImagePath) Unable to OCR inspection image file  refId='" + refId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "(getOCRInspectionImagePath) Unable to OCR inspection image file  refId='" + refId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if ((sqlDataSet != null) && (sqlDataSet.Tables[0].Rows.Count > 0))
            {
                //Retrieves info from Table[0]
                DataRow currRow = sqlDataSet.Tables[0].Rows[0];
                retResult = Convert.ToString(currRow[0]);
            }

            sqlDataSet = null;

            return retResult;
        }

        public string GetOCRImagePath(string refId, string time)
        {
            string retResult = null;
            if (string.IsNullOrEmpty(time))
            {
                retResult = getOCRLogImagePath(refId);
            }
            else
            {
                retResult = getOCRInspectionImagePath(refId, time);
            }
            return retResult;
        }

	}
}
