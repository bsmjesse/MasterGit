using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using System.Text;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to vlfBox table.
    /// </summary>
    /// <comment> I added the option to queue some of the NonQuery statements in queue
    ///           and have a timer execute them at once
    /// </comment>
    public class Box : TblOneIntPrimaryKey
    {
       static DataTable _memBox;    ///< this is a temporay cache used to calculate 
                                    ///  the current status and other information like
                                    ///  

#if BULK_SUPPORT
      static StringBuilder    strNonQueryStatements ;       ///< add operations to this string
      static System.Threading.Timer tmrExecutor ;           ///< the timer is coming every 30 seconds and send the messages to the server
      unsigned int            requests ;                    ///< how many updates/inserts are in strNonQueryStatements
#endif
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Box(SQLExecuter sqlExec)
            : base("vlfBox", sqlExec)
        {
        }

        /// <summary>
        /// Add new box.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fwChId"></param>
        /// <param name="boxArmed"></param>
        /// <param name="boxActive"></param>
        /// <param name="organizationId"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if box already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddBox(int boxId, short fwChId, bool boxArmed, bool boxActive, int organizationId)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = "INSERT INTO vlfBox(BoxId, FwChId, BoxArmed, BoxActive, OrganizationId) VALUES ( @BoxId, @FwChId, @BoxArmed, @BoxActive, @OrganizationId )";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@FwChId", SqlDbType.SmallInt, fwChId);
                sqlExec.AddCommandParam("@BoxArmed", SqlDbType.SmallInt, Convert.ToInt16(boxArmed));
                sqlExec.AddCommandParam("@BoxActive", SqlDbType.SmallInt, Convert.ToInt16(boxActive));
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);

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
                string prefixMsg = "Unable to add new '" + boxId + "' box.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new '" + boxId + "' box.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new '" + boxId + "' box.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
            }
        }

        /// <summary>
        /// Add new box with firmware features mask field
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fwChId"></param>
        /// <param name="boxArmed"></param>
        /// <param name="boxActive"></param>
        /// <param name="organizationId"></param>
        /// <param name="featureMask"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if box already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddBox(int boxId, short fwChId, bool boxArmed, bool boxActive, int organizationId, long featureMask)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = "INSERT INTO vlfBox(BoxId, FwChId, BoxArmed, BoxActive, OrganizationId, FwAttributes1) VALUES (@BoxId, @FwChId, @BoxArmed, @BoxActive, @OrganizationId, @features)";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@FwChId", SqlDbType.SmallInt, fwChId);
                sqlExec.AddCommandParam("@BoxArmed", SqlDbType.SmallInt, Convert.ToInt16(boxArmed));
                sqlExec.AddCommandParam("@BoxActive", SqlDbType.SmallInt, Convert.ToInt16(boxActive));
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@features", SqlDbType.BigInt, featureMask);

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
                string prefixMsg = "Unable to add new '" + boxId + "' box.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new '" + boxId + "' box.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new '" + boxId + "' box.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
            }
        }

        /// <summary>
        /// Delete existing box.
        /// Throws exception in case of wrong result (see TblOneIntPrimaryKey class).
        /// </summary>
        /// <param name="boxId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteRecord(int boxId)
        {
            return DeleteRowsByIntField("BoxId", boxId, "box");
        }

        /// <summary>
        /// Retrieves all assigned/free boxes ids.
        /// </summary>
        /// <remarks>
        /// Assgned is true
        /// Free is false
        /// </remarks>
        /// <returns>[BoxId]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="assigned"></param>
        /// <param name="organizationId"></param>
        public ArrayList GetAllAssignedBoxIds(bool assigned, int organizationId)
        {
            ArrayList resultList = null;
            DataSet sqlDataSet = GetAllAssignedBoxIdsDs(assigned, organizationId);
            if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
            {
                resultList = new ArrayList();
                //Retrieves box id from result DataSet
                foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
                {
                    resultList.Add(Convert.ToInt32(currRow["BoxId"]));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Retrieves all assigned boxes ids.
        /// </summary>
        /// <remarks>
        /// Assgned is true
        /// Free is false
        /// </remarks>
        /// <returns>[BoxId]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="assigned"></param>
        /// <param name="organizationId"></param>
        public DataSet GetAllAssignedBoxIdsDs(bool assigned, int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                if (assigned == false)
                    sqlDataSet = sqlExec.SQLExecuteDataset("SELECT BoxId FROM vlfBox with (nolock) WHERE OrganizationId=" + organizationId + " AND BoxId NOT IN (SELECT BoxId FROM vlfVehicleAssignment) ORDER BY vlfBox.BoxId");
                else
                   sqlDataSet = sqlExec.SQLExecuteDataset("SELECT BoxId FROM vlfBox with (nolock) WHERE OrganizationId=" + organizationId + " AND BoxId IN (SELECT BoxId FROM vlfVehicleAssignment) ORDER BY vlfBox.BoxId");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve all unassigned boxes. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve all unassigned boxes. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves all assigned/free boxes info.
        /// </summary>
        /// <remarks>
        /// Assgned is true
        /// Free is false
        /// </remarks>
        /// <returns>DataSet [BoxId],[FwChId],[FwId],[FwName],[BoxHwTypeId],[BoxHwTypeName],[ChName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName],[OAPPort]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="assigned"></param>
        /// <param name="organizationId"></param>
        /// <comment>  added the option to get all boxes assigned in all organizations
        /// </comment>
        public DataSet GetAllAssignedBoxesInfo(bool assigned, int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "SELECT vlfBox.BoxId, vlfBox.OrganizationId, vlfFirmwareChannelReference.FwChId, vlfFirmware.FwId, vlfFirmware.FwName, vlfBoxHwType.BoxHwTypeId, vlfBoxHwType.BoxHwTypeName, vlfChannels.ChName, vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfCommMode.CommModeId, vlfCommMode.CommModeName,OAPPort FROM vlfBoxHwType INNER JOIN vlfFirmware ON vlfBoxHwType.BoxHwTypeId = vlfFirmware.BoxHwTypeId INNER JOIN vlfFirmwareChannels ON vlfFirmware.FwId = vlfFirmwareChannels.FwId INNER JOIN vlfChannels INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfFirmwareChannelReference ON vlfFirmwareChannels.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId";
                string sql2;
                if (assigned == false)
                    sql2 = sql + " WHERE vlfBox.BoxId NOT IN (SELECT BoxId FROM vlfVehicleAssignment) " +
                       ((organizationId != -1) ?
                           string.Format("AND vlfBox.OrganizationId={0} ORDER BY vlfBox.BoxId", organizationId) : " ORDER BY vlfBox.BoxId");
                else
                    sql2 = sql + " WHERE vlfBox.BoxId IN (SELECT BoxId FROM vlfVehicleAssignment) " +
                       ((organizationId != -1) ?
                       string.Format("AND vlfBox.OrganizationId={0} ORDER BY vlfBox.BoxId", organizationId) : " ORDER BY vlfBox.BoxId");

                sqlDataSet = sqlExec.SQLExecuteDataset(sql2);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve boxes. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve boxes. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves all assigned/free boxes info.
        /// </summary>
        /// <remarks>
        /// Assgned is true
        /// Free is false
        /// </remarks>
        /// <returns>DataSet [BoxId],[BoxHwTypeName],[BoxProtocolTypeName],[CommModeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="assigned"></param>
        /// <param name="organizationId"></param>
        public DataSet GetAllAssignedBoxIdsInfo(bool assigned, int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                if (assigned == false)
                   sqlDataSet = sqlExec.SQLExecuteDataset("SELECT BoxId FROM vlfBox with (nolock) WHERE OrganizationId=" + organizationId + " AND BoxId NOT IN (SELECT BoxId FROM vlfVehicleAssignment) ORDER BY vlfBox.BoxId");
                else
                   sqlDataSet = sqlExec.SQLExecuteDataset("SELECT BoxId FROM vlfBox with (nolock)WHERE OrganizationId=" + organizationId + " AND BoxId IN (SELECT BoxId FROM vlfVehicleAssignment) ORDER BY vlfBox.BoxId");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve boxes. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve boxes. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrives box organization
        /// </summary>
        /// <returns>OrganizationId</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="boxId"></param>
        public int GetBoxOrganization(int boxId)
        {
            int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
               object obj = sqlExec.SQLExecuteScalar("SELECT OrganizationId FROM vlfBox with (nolock) WHERE BoxId=" + boxId);
                if (obj != System.DBNull.Value)
                    organizationId = Convert.ToInt32(obj);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box organization. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box organization. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return organizationId;
        }

        /// <summary>
        /// Retrives box information by communication info
        /// </summary>
        /// <returns>DataSet [BoxId],[CommAddressTypeId],[CommAddressTypeName],[OrganizationId],[OrganizationName],[Description],[FleetName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="commInfo"></param>
        public DataSet GetBoxInfoByCommInfo(string commInfo)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlDataSet = sqlExec.SQLExecuteDataset("SELECT vlfBoxCommInfo.BoxId, vlfBoxCommInfo.CommAddressTypeId, vlfCommAddressType.CommAddressTypeName, vlfBox.OrganizationId, vlfOrganization.OrganizationName, ISNULL(vlfVehicleInfo.Description,'N/A') AS Description, ISNULL(vlfFleet.FleetName,'N/A') AS FleetName FROM vlfCommAddressType INNER JOIN vlfBoxCommInfo INNER JOIN vlfBox with (nolock) ON vlfBoxCommInfo.BoxId = vlfBox.BoxId INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId ON vlfCommAddressType.CommAddressTypeId = vlfBoxCommInfo.CommAddressTypeId LEFT OUTER JOIN vlfFleet INNER JOIN vlfFleetVehicles ON vlfFleet.FleetId = vlfFleetVehicles.FleetId RIGHT OUTER JOIN vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId ON vlfBox.BoxId = vlfVehicleAssignment.BoxId WHERE vlfBoxCommInfo.CommAddressValue = '" + commInfo + "' ORDER BY vlfBoxCommInfo.BoxId");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve boxes by commInfo. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve boxes by commInfo. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get box last valid GPS datetime
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>[OriginDateTime]/// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DateTime GetBoxLastValidGpsDateTime(int boxId)
        {
            DateTime retResult = VLF.CLS.Def.Const.unassignedDateTime;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT LastValidDateTime FROM vlfBox with (nolock) WHERE BoxId=" + boxId;
                //Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    retResult = Convert.ToDateTime(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " last valid date time. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " last valid date time. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return retResult;
        }

        /// <summary>
        /// Get box last communicated datetime
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>[OriginDateTime]/// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DateTime GetBoxLastCommunicatedDateTime(int boxId)
        {
            DateTime retResult = VLF.CLS.Def.Const.unassignedDateTime;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT LastCommunicatedDateTime FROM vlfBox with (nolock) WHERE BoxId=" + boxId;
                //Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    retResult = Convert.ToDateTime(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " last communicated date time. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " last communicated date time. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return retResult;
        }

        /// <summary>
        /// Get box last information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <returns>DataSet [BoxId],
        /// [OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[StreetAddress],
        /// [LastCommunicatedDateTime],[SensorMask],[BoxArmed],[GeoFenceEnabled],
        /// [LastStatusDateTime],[BoxActive]
        /// [LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxLastInfo(int userId, int boxId)
        {
            DataSet resultDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                        " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                        " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " IF @Unit IS NULL SET @Unit=1" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                    " SELECT vlfBox.BoxId," +
                    "CASE WHEN vlfBox.LastValidDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastValidDateTime) END AS OriginDateTime," +
                    "ISNULL(vlfBox.LastLatitude,0) AS Latitude," +
                    "ISNULL(vlfBox.LastLongitude,0) AS Longitude," +
                    "CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit,1) END AS Speed," +
                    "ISNULL(vlfBox.LastHeading,0) AS Heading," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

                    "CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime," +
                    "ISNULL(vlfBox.LastSensorMask,0) AS SensorMask," +
                    "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
                    "CASE WHEN vlfBox.GeoFenceEnabled=0 then 'false' ELSE 'true' END AS GeoFenceEnabled," +
                    "CASE WHEN vlfBox.LastStatusDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,DATEADD(hour,@Timezone,vlfBox.LastStatusDateTime)) END AS LastStatusDateTime," +
                    "CASE WHEN vlfBox.BoxActive=0 then 'false' ELSE 'true' END AS BoxActive," +
                    "ISNULL(LastStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSensor," +
                    "ISNULL(LastStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSpeed," +
                    "CASE WHEN vlfBox.PrevStatusDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,DATEADD(hour,@Timezone,vlfBox.PrevStatusDateTime)) END AS PrevStatusDateTime," +
               "CASE WHEN vlfBox.DormantDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,DATEADD(hour,@Timezone,vlfBox.DormantDateTime)) END AS DormantDateTime," +
                    "ISNULL(PrevStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSensor," +
                    "ISNULL(PrevStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSpeed" +
                    " FROM vlfBox with (nolock) WHERE vlfBox.BoxId=" + boxId;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Get box last information
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet [BoxId],
        /// [OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[StreetAddress],
        /// [LastCommunicatedDateTime],[SensorMask],[BoxArmed],[GeoFenceEnabled],
        /// [LastStatusDateTime],[BoxActive],
        /// [LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxLastInfoSP(int boxId)
        {
            DataSet resultDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetBoxLastInfo");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        public DataSet GetBoxLastInfoSPSLS(int boxId)
        {
           DataSet resultDataSet = null;
           try
           {
              sqlExec.ClearCommandParameters();
              sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
              resultDataSet = sqlExec.SPExecuteDataset("sp_GetBoxLastInfoSLS");
           }
           catch (SqlException objException)
           {
              string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
              Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
              throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
              string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
              throw new DASException(prefixMsg + " " + objException.Message);
           }
           return resultDataSet;
        }

        public void GetSLSVehicleInfo(int boxId, 
                                      out string licensePlate,
                                      out short vehicleType,
                                      out DateTime lastValidDateTime)
        {
           string prefixMsg = string.Format("GetSLSVehicleInfo, error -> BID={0}", boxId);

           licensePlate = string.Empty;
           lastValidDateTime = VLF.CLS.Def.Const.unassignedDateTime; // new DateTime(2000, 01, 01); // 
           vehicleType = 0;

           try
           {
              // 1. Prepares SQL statement
              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
              sqlExec.AddCommandParam("@licensePlate", SqlDbType.VarChar, 
                                       ParameterDirection.Output, 20, licensePlate);
              sqlExec.AddCommandParam("@vehicleType", SqlDbType.SmallInt, 
                                       ParameterDirection.Output, vehicleType);
              sqlExec.AddCommandParam("@lastValidDateTime", SqlDbType.DateTime, 
                                       ParameterDirection.Output, lastValidDateTime);

              // 2. Executes SQL statement
              int res = sqlExec.SPExecuteNonQuery("sp_GetSLSVehicleInfo");
              licensePlate = (DBNull.Value == sqlExec.ReadCommandParam("@licensePlate")) ?
                               string.Empty : sqlExec.ReadCommandParam("@licensePlate").ToString();

              lastValidDateTime = (DBNull.Value == sqlExec.ReadCommandParam("@lastValidDateTime")) ?
                               lastValidDateTime : Convert.ToDateTime(sqlExec.ReadCommandParam("@lastValidDateTime").ToString());

              vehicleType = (DBNull.Value == sqlExec.ReadCommandParam("@vehicleType")) ?
                              vehicleType : Convert.ToInt16(sqlExec.ReadCommandParam("@vehicleType").ToString());

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
        }


        /// <summary>
        ///      there is a problem in the way the datetime is extracted - without milliseconds
        ///      
        ///         DateTime myDate = DateTime.MinValue; //=> 1/1/0001
        ///         SqlDateTime mySqlDate = SqlDateTime.MinValue; //=> 1/1/1753
        //          also note that SQL Server's smalldatetime min value is 1/1/1900
        /// </summary>
        /// <comment>
        ///         convert(varchar,getdate(),108 is only seconds 
        ///         convert(varchar,getdate(),114) is miliseconds
        /// </comment>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public DataSet GetBoxLastInfo(int boxId)
        {
            DataSet resultDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @ResolveLandmark int SET @ResolveLandmark=0 SELECT BoxId,CASE WHEN LastValidDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,LastValidDateTime,101) +' '+ convert(varchar,LastValidDateTime,108) END AS OriginDateTime,ISNULL(LastLatitude,0) AS Latitude,ISNULL(LastLongitude,0) AS Longitude,ISNULL(LastSpeed,0) AS Speed,ISNULL(vlfBox.LastHeading,0) AS Heading," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +
                    "CASE WHEN LastCommunicatedDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,LastCommunicatedDateTime,101) +' '+ convert(varchar,LastCommunicatedDateTime,108) END AS LastCommunicatedDateTime,LastSensorMask AS SensorMask,CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed,CASE WHEN vlfBox.GeoFenceEnabled=0 then 'false' ELSE 'true' END AS GeoFenceEnabled," +
                    "CASE WHEN LastStatusDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,LastStatusDateTime,101) +' '+ convert(varchar,LastStatusDateTime,108) END AS LastStatusDateTime," +
                    "CASE WHEN BoxActive=0 then 'false' ELSE 'true' END AS BoxActive," +
                    "LastStatusSensor,LastStatusSpeed," +
               "CASE WHEN DormantDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,DormantDateTime,101)+' '+ convert(varchar,DormantDateTime,108) END AS DormantDateTime," +
               "CASE WHEN PrevStatusDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,PrevStatusDateTime,101) +' '+ convert(varchar,PrevStatusDateTime,108) END AS PrevStatusDateTime," +
                    "PrevStatusSensor,PrevStatusSpeed FROM vlfBox with (nolock) WHERE BoxId=" + boxId;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " status information. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Get box next sensor status information
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="currCommunicatedDateTime"></param>
        /// <returns>DataSet [OriginDateTime],[Speed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetNextSensorStatus(int boxId, DateTime currCommunicatedDateTime)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT TOP 1 OriginDateTime,Speed FROM vlfMsgInHst with (nolock) WHERE BoxId=" + boxId + " AND OriginDateTime > '" + currCommunicatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                    " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
               " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
               " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
               " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                    ") ORDER BY OriginDateTime";
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box=" + boxId + " sensor status by currCommunicatedDateTime=" + currCommunicatedDateTime.ToString() + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box=" + boxId + " sensor status by currCommunicatedDateTime=" + currCommunicatedDateTime.ToString() + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /** \fn         GetNextSensorStatusSP
         *  \comment    replace a long query on the server side 
         *  \returns    false if no record is found
         */ 
        public bool GetNextSensorStatusSP(int boxId, DateTime currCommunicatedDateTime, 
                                        out int speed, out DateTime originDateTime )
        {
           string prefixMsg = string.Format("GetNextSensorStatusSP, error -> BID={0} DT={1}", boxId, currCommunicatedDateTime);
           speed = -1;
           originDateTime = VLF.CLS.Def.Const.unassignedDateTime; // new DateTime(2000, 01, 01);

           try
           {
              // 1. Prepares SQL statement

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
              sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, currCommunicatedDateTime);
              sqlExec.AddCommandParam("@speed", SqlDbType.Int, ParameterDirection.Output, speed);
              sqlExec.AddCommandParam("@foundOriginDateTime", SqlDbType.DateTime, ParameterDirection.Output, originDateTime);

              // 2. Executes SQL statement
              int res = sqlExec.SPExecuteNonQuery("sp_GetNextSensorStatus");
              speed = (DBNull.Value == sqlExec.ReadCommandParam("@speed")) ?
                               -1 : Convert.ToInt32(sqlExec.ReadCommandParam("@speed").ToString());
              originDateTime = (DBNull.Value == sqlExec.ReadCommandParam("@foundOriginDateTime")) ?
                               originDateTime : Convert.ToDateTime(sqlExec.ReadCommandParam("@foundOriginDateTime").ToString());

              
              return (speed > -1);
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
           return false;
        }

        /// <summary>
        /// Get box last street address
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>street address</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public string GetVehicleLastStreetAddress(int boxId)
        {
            string streetAddress = "";
            DataSet resultDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @ResolveLandmark int SET @ResolveLandmark=0 SELECT ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'') AS StreetAddress FROM vlfBox WHERE vlfBox.BoxId=" + boxId;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " street addres. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " street addres. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0)
                streetAddress = resultDataSet.Tables[0].Rows[0]["StreetAddress"].ToString().TrimEnd();
            return streetAddress;
        }

        /// <summary>
        /// Get box last status information
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet [SensorMask],[LastStatusDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxLastSensorMask(int boxId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
               string sql = " SELECT LastSensorMask FROM vlfBox with (nolock) WHERE vlfBox.BoxId=" + boxId;

                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " last status information. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box " + boxId + " last status information. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Returns true if box is armed.
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>true if armed, otherwise false</returns>
        /// <exception cref="DASAppWrongResultException">Thrown if multiple records have been found.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public bool IsArmed(int boxId)
        {
            bool boxArmed = false;
            DataSet resultDataSet = null;
            string prefixMsg = "Unable to retrieve box status Id by box id=" + boxId + ". ";
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT BoxArmed FROM " + tableName + " WHERE BoxId=" + boxId;

                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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

            if (resultDataSet.Tables[0].Rows.Count > 1)
            {
                throw new DASAppWrongResultException(prefixMsg + " Multiple records have been found.");
            }
            else
            {
                //Retrieves info from Table[0].[0][0]
                foreach (DataRow currRow in resultDataSet.Tables[0].Rows)
                {
                    boxArmed = Convert.ToBoolean(currRow[0]);
                    break;
                }
            }
            return boxArmed;
        }

        /// <summary>
        /// Returns true if box is active.
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>true if active, otherwise false</returns>
        /// <exception cref="DASAppWrongResultException">Thrown if multiple records have been found.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public bool IsActive(int boxId)
        {
            bool boxActive = false;
            DataSet resultDataSet = null;
            string prefixMsg = "Unable to retrieve box state Id by box id=" + boxId + ". ";
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT BoxActive FROM " + tableName + " WHERE BoxId=" + boxId;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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

            //Throws exception in case of wrong result
            if (resultDataSet.Tables[0].Rows.Count > 1)
            {
                throw new DASAppWrongResultException(prefixMsg + " Multiple records have been found.");
            }
            else
            {
                //Retrieves info from Table[0].[0][0]
                foreach (DataRow currRow in resultDataSet.Tables[0].Rows)
                {
                    boxActive = Convert.ToBoolean(currRow[0]);
                    break;
                }
            }
            return boxActive;
        }

        /// <summary>
        /// Update box FwChId.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fwChId"></param>
        /// <exception cref="DASAppResultNotFoundException">Thrown if box or configuration is incorrect.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateBoxFwChId(int boxId, short fwChId)
        {
            Util.BTrace(Util.INF0, "-- Box.UpdateBoxFwChId -> boxId[{0}] FwChId[{1}]", boxId, fwChId);
            int rowsAffected = 0;
            try
            {
                // 1. Prepares SQL statement
                string sql = "UPDATE " + tableName +
                            " SET FwChId=" + fwChId +
                            " WHERE BoxId=" + boxId;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to set new box fwChId='" + fwChId +
                    " to the box='" + boxId + "'. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                string prefixMsg = "Unable to set new box fwChId='" + fwChId +
                    " to the box='" + boxId + "'. ";
                throw new DASDbConnectionClosed(prefixMsg + exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to set new box fwChId='" + fwChId +
                    " to the box='" + boxId + "'. ";
                throw new DASException(prefixMsg + objException.Message);
            }

            //Throws exception in case of wrong result
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to set new box fwChId='" + fwChId +
                    " to the box='" + boxId + "'. ";
                throw new DASAppResultNotFoundException(prefixMsg + " Wrong box id='" + boxId + "' or fwChId='" + fwChId + "'.");
            }
        }

        /// <summary>
        /// Update state(Armed/Disarmed).
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="boxArmed"></param>
        /// <exception cref="DASAppResultNotFoundException">Thrown if box or configuration is incorrect.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void SetArmed(int boxId, bool boxArmed)
        {
            int rowsAffected = 0;
            string prefixMsg = "Unable to set arm='" + boxArmed +
                " to the box='" + boxId + "'. ";
            try
            {
                //Prepares SQL statement
                string sql = "UPDATE " + tableName + " SET BoxArmed=" + Convert.ToInt16(boxArmed)
                    + " WHERE BoxId=" + boxId;
                //Executes SQL statement
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

            //Throws exception in case of wrong result
            if (rowsAffected == 0)
            {
                throw new DASAppResultNotFoundException(prefixMsg + " The box id='" + boxId + "' does not exist.");
            }
        }

        /// <summary>
        /// Update status 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="boxActive"></param>
        /// <exception cref="DASAppResultNotFoundException">Thrown if box or configuration is incorrect.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void SetActive(int boxId, bool boxActive)
        {
            int rowsAffected = 0;
            string prefixMsg = "Unable to set arm='" + boxActive +
                " to the box='" + boxId + "'. ";
            try
            {
                //Prepares SQL statement
                string sql = "UPDATE " + tableName + " SET BoxActive=" + Convert.ToInt16(boxActive)
                    + " WHERE BoxId=" + boxId;
                //Executes SQL statement
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

            //Throws exception in case of wrong result
            if (rowsAffected == 0)
            {
                throw new DASAppResultNotFoundException(prefixMsg + " The box id='" + boxId + "' does not exist.");
            }
        }

        public void UpdateLastAndPreviousSensorSpeedDateTime(int boxId,                                                             
                                                             short lastStatusSensor,
                                                             short lastStatusSpeed,
                                                             DateTime lastStatusDateTime,
                                                             short prevStatusSensor,
                                                             short prevStatusSpeed,
                                                             DateTime prevStatusDateTime)
        {
           string prefixMsg = string.Format("UpdateLastAndPreviousSensorSpeedDateTime - > box={0}", boxId);

           int rowsAffected = 0;
           // 1. Prepares SQL statement 
           string sql = (lastStatusSensor == -1 ? 
                "UPDATE vlfBox SET LastStatusDateTime='" + lastStatusDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",LastStatusSpeed=" + lastStatusSpeed +
                ",PrevStatusDateTime='" + prevStatusDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",PrevStatusSensor=" + prevStatusSensor +
                ",PrevStatusSpeed=" + prevStatusSpeed +
                " WHERE BoxId=" + boxId : 
 
                "UPDATE vlfBox SET LastStatusDateTime='" + lastStatusDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",LastStatusSensor=" + lastStatusSensor +
                ",LastStatusSpeed=" + lastStatusSpeed +
                ",PrevStatusDateTime='" + prevStatusDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",PrevStatusSensor=" + prevStatusSensor +
                ",PrevStatusSpeed=" + prevStatusSpeed +
                " WHERE BoxId=" + boxId );
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
              throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
           }
        }


        public void UpdateLastSensorSpeedDateTime(int boxId, short statusSensor, short statusSpeed,
                                                  DateTime dateTime)
        {
           string prefixMsg = string.Format("UpdateLastSensorSpeedDateTime - > box={0}", boxId);
 
           int rowsAffected = 0;
           // 1. Prepares SQL statement 
           string sql = (statusSensor == -1 ? 
                ("UPDATE vlfBox SET LastStatusDateTime='" + dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",LastStatusSpeed=" + statusSpeed +
                " WHERE BoxId=" + boxId) :

                 ("UPDATE vlfBox SET LastStatusDateTime='" + dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",LastStatusSensor=" + statusSensor +
                ",LastStatusSpeed=" + statusSpeed +
                " WHERE BoxId=" + boxId) );
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
              throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
           }
        }

        public void UpdatePrevSensorSpeedDateTime(int boxId, short statusSensor, short statusSpeed,
                                                  DateTime dateTime)
        {
           string prefixMsg = string.Format("UpdatePrevSensorSpeedDateTime - > box={0}", boxId);

           int rowsAffected = 0;
           // 1. Prepares SQL statement
           string sql = "UPDATE vlfBox SET PrevStatusDateTime='" + dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",PrevStatusSensor=" + statusSensor +
                ",PrevStatusSpeed=" + statusSpeed +
                " WHERE BoxId=" + boxId;
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
              throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
           }
        }

        /// <summary>
        /// Update box sensor status.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="boxArmed"></param>
        /// <param name="lastCommunicatedDateTime"></param>
        /// <param name="sensorMask"></param>
        /// <param name="geoFenceEnabled"></param>
        /// <param name="boxActive"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateStatus(int boxId, bool boxArmed,
                                DateTime lastCommunicatedDateTime,
                                Int64 sensorMask, bool geoFenceEnabled, bool boxActive)
        {
            int rowsAffected = 0;
            short armed = 0;
            if (boxArmed == true)
                armed = 1;
            short active = 0;
            if (boxActive == true)
                active = 1;
            short geoFence = 0;
            if (geoFenceEnabled == true)
                geoFence = 1;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET BoxArmed=" + armed +
                ",LastCommunicatedDateTime='" + lastCommunicatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",LastSensorMask=" + sensorMask +
                ",GeoFenceEnabled=" + geoFence +
                ",BoxActive=" + active +
                " WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box dormant status.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="originatedDateTime"></param>
        /// <param name="currDormantStatus"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateDormantStatus(int boxId, DateTime originatedDateTime, short currDormantStatus)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET Dormant=" + currDormantStatus +
                ",DormantDateTime='" + originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                " WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' dormant(" + currDormantStatus + ") status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' dormant(" + currDormantStatus + ") status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' dormant(" + currDormantStatus + ") status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box current sensor status
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="lastStatusSensor"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateLastStatusSensor(int boxId, short lastStatusSensor)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET LastStatusSensor=" + lastStatusSensor + " WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' current sensor status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' current sensor status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' current sensor status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box previous sensor status
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="prevStatusSensor"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdatePrevStatusSensor(int boxId, short prevStatusSensor)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET PrevStatusSensor=" + prevStatusSensor + " WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' previous sensor status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' previous sensor status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' previous sensor status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box current speed status
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="lastStatusSensor"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateLastSpeedStatus(int boxId, short lastStatusSpeed)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET LastStatusSpeed=" + lastStatusSpeed + " WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' current speed status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' current speed status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' current speed status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box previous speed status
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="prevStatusSpeed"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdatePrevSpeedStatus(int boxId, short prevStatusSpeed)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET PrevStatusSpeed=" + prevStatusSpeed + " WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' previous speed status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' previous speed status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' previous speed status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box sensor status datetime.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="lastStatusDateTime"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateLastStatusDateTime(int boxId, DateTime lastStatusDateTime)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET LastStatusDateTime='" + lastStatusDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "' WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' last status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' last status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' last status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box prev sensor status datetime.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="prevStatusDateTime"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdatePrevStatusDateTime(int boxId, DateTime prevStatusDateTime)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfBox SET PrevStatusDateTime='" + prevStatusDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "' WHERE BoxId=" + boxId;
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
                string prefixMsg = "Unable to update box '" + boxId + "' last status.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' last status.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box '" + boxId + "' last status.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exist.");
            }
        }

        /// <summary>
        /// Update box position.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="lastValidDateTime"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdatePosition(int boxId, DateTime lastValidDateTime,
                                double latitude, double longitude)
        {
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE vlfBox SET LastValidDateTime='" + lastValidDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",LastLatitude=" + latitude +
                ",LastLongitude=" + longitude +
                ",LastStreetAddress=NULL" +
                ",NearestLandmark=NULL" +
                ",LSD=NULL" +
                " WHERE BoxId=" + boxId;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update box position '" + boxId + "' information.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box position '" + boxId + "' information.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box position '" + boxId + "' information.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exists.");
            }
        }

        /// <summary>
        /// Update box position.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="lastValidDateTime"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="speed"></param>
        /// <param name="heading"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdatePosition(int boxId, DateTime lastValidDateTime,
                                    double latitude, double longitude, short speed, short heading)
        {
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE vlfBox SET LastValidDateTime='" + lastValidDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                ",LastLatitude=" + latitude +
                ",LastLongitude=" + longitude +
                ",LastStreetAddress=NULL" +
                ",NearestLandmark=NULL" +
                ",LSD=NULL" +
                ",LastSpeed=" + speed +
                ",LastHeading=" + heading +
                " WHERE BoxId=" + boxId;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update box position '" + boxId + "' information.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update box position '" + boxId + "' information.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update box position '" + boxId + "' information.";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This box does not exists.");
            }
        }

        /// <summary>
        /// Retrieves records with empty StreetAddresses
        /// </summary>
        /// <returns>DataSet [BoxId],[Latitude],[Longitude]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="cmdTimeOut"></param>
        public DataSet GetEmptyStreetAddress(int cmdTimeOut)
        {
            DataSet sqlDataSet = null;
            int currCmdTimeOut = sqlExec.CommandTimeout;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT top 500 BoxId,LastLatitude AS Latitude,LastLongitude AS Longitude FROM vlfBox with (nolock) WHERE BoxId > 0 AND LastStreetAddress IS NULL AND LastLatitude<>0 AND LastLongitude<>0";
                // 2. Executes SQL statement
                sqlExec.CommandTimeout = cmdTimeOut;
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve records with empty StreetAddress.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve records with empty StreetAddress. " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            // 3. Return result
            return sqlDataSet;
        }
        /// <summary>
        /// Retrieves records with empty NearestLandmark
        /// </summary>
        /// <returns>DataSet [BoxId],[Latitude],[Longitude]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="cmdTimeOut"></param>
        public DataSet GetEmptyNearestLandmark()
        {
            DataSet sqlDataSet = null;
            int currCmdTimeOut = sqlExec.CommandTimeout;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT BoxId,LastLatitude AS Latitude,LastLongitude AS Longitude FROM vlfBox with (nolock) WHERE BoxId > 0 AND NearestLandmark IS NULL AND LastLatitude<>0 AND LastLongitude<>0";
                // 2. Executes SQL statement
                sqlExec.CommandTimeout = 600;
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve records with empty StreetAddress.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve records with empty StreetAddress. " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            // 3. Return result
            return sqlDataSet;
        }
        /// <summary>
        /// Updates record with street address and/or nearestLandmark
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="streetAddress"></param>
        /// <param name="cmdTimeOut"></param>
        /// <param name="nearestLandmark"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if street address already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateStreetAddress(int boxId, string streetAddress, int cmdTimeOut, string nearestLandmark)
        {
            int rowsAffected = 0;
            string sql = "";
            int currCmdTimeOut = sqlExec.CommandTimeout;
            try
            {
                // 1. Prepares SQL statement
                sql = "UPDATE vlfBox SET LastStreetAddress='" + streetAddress.Replace("'", "''") + "'";
                if (nearestLandmark != "")
                    sql += ",NearestLandmark='" + nearestLandmark.Replace("'", "''") + "'";
                sql += " WHERE BoxId=" + boxId;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                sqlExec.CommandTimeout = cmdTimeOut;
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " Street address already exists.");
            }
        }

        /// <summary>
        /// Updates record with street address and/or nearestLandmark
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="streetAddress"></param>        
        public void UpdateStreetAddress(int boxId, float lat, float lon, string streetAddress)
        {
            int rowsAffected = 0;
            string sql = "";
            try
            {
                sql = "sp_UpdateBoxStreetAddress";
                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@Lat", SqlDbType.Float, lat);
                sqlExec.AddCommandParam("@Lon", SqlDbType.Float, lon);
                sqlExec.AddCommandParam("@Address", SqlDbType.VarChar, streetAddress);

                if (sqlExec.RequiredTransaction())
                {
                    // 4. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 5. Executes SQL statement
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            
        }
        /// <summary>
        /// Updates record with nearestLandmark
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="nearestLandmark"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if street address already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateNearestLandmark(int boxId, string nearestLandmark)
        {
            int rowsAffected = 0;
            string sql = "";
            int currCmdTimeOut = sqlExec.CommandTimeout;
            try
            {
                // 1. Prepares SQL statement
                sql = "UPDATE vlfBox SET NearestLandmark='" + nearestLandmark.Replace("'", "''") + "' WHERE BoxId=" + boxId;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                sqlExec.CommandTimeout = 600;
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + ".";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " Street address already exists.");
            }
        }

        #region Box Configuration Info

        /// <summary>
        /// Get box configuration information. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet[BoxId], [FwChId], [MaxMsgs], [MaxTxtMsgs],
        ///					[BoxHwTypeId], [BoxHwTypeName], [MaxSensorsNum], [MaxOutputsNum] // HW type information
        ///					[BoxProtocolTypeId], [BoxProtocolTypeName], [SessionTimeOut - new 2007-09-10] // box protocol type information
        ///					[CommModeId], [CommModeName], [ChPriority],
        ///					[FwTypeId], [FwLocalPath], [FwOAPPath], [FwDateReleased], [MaxGeozones],
        ///               [OAPPort]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxConfigInfo(int boxId)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "Unable to retrieve box " + boxId + " configuration information. ";
                //Prepares SQL statement
                string sql = "SELECT dbo.vlfBox.BoxId, dbo.vlfFirmwareChannels.FwChId, dbo.vlfBoxHwType.BoxHwTypeId, dbo.vlfBoxHwType.BoxHwTypeName,dbo.vlfBoxHwType.MaxSensorsNum, dbo.vlfBoxHwType.MaxOutputsNum, dbo.vlfBoxProtocolType.BoxProtocolTypeId,dbo.vlfBoxProtocolType.BoxProtocolTypeName, dbo.vlfBoxProtocolType.SessionTimeOut, dbo.vlfCommMode.CommModeId, dbo.vlfCommMode.CommModeName,dbo.vlfFirmwareChannels.ChPriority, dbo.vlfFirmware.FwTypeId, dbo.vlfFirmware.FwLocalPath, dbo.vlfFirmware.FwOAPPath,dbo.vlfFirmware.FwDateReleased, dbo.vlfFirmware.MaxGeozones, dbo.vlfBoxSettings.MaxMsgs, dbo.vlfBoxSettings.MaxTxtMsgs,dbo.vlfFirmware.OAPPort, dbo.vlfDcl.DclId, dbo.vlfBox.FwAttributes1 FROM dbo.vlfBoxProtocolGroup INNER JOIN dbo.vlfDcl ON dbo.vlfBoxProtocolGroup.BoxProtocolGroupId = dbo.vlfDcl.BoxProtocolGroupId INNER JOIN dbo.vlfCommMode ON dbo.vlfDcl.CommModeId = dbo.vlfCommMode.CommModeId INNER JOIN dbo.vlfBox with (nolock) INNER JOIN dbo.vlfFirmwareChannelReference ON dbo.vlfBox.FwChId = dbo.vlfFirmwareChannelReference.FwChId INNER JOIN dbo.vlfFirmwareChannels ON dbo.vlfFirmwareChannelReference.FwChId = dbo.vlfFirmwareChannels.FwChId INNER JOIN  dbo.vlfFirmware ON dbo.vlfFirmwareChannels.FwId = dbo.vlfFirmware.FwId INNER JOIN dbo.vlfChannels ON dbo.vlfFirmwareChannels.ChId = dbo.vlfChannels.ChId INNER JOIN  dbo.vlfBoxHwType ON dbo.vlfFirmware.BoxHwTypeId = dbo.vlfBoxHwType.BoxHwTypeId ON dbo.vlfCommMode.CommModeId = dbo.vlfChannels.CommModeId INNER JOIN dbo.vlfBoxlProtocolGroupAssignment ON dbo.vlfBoxProtocolGroup.BoxProtocolGroupId = dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolGroupId AND dbo.vlfChannels.BoxProtocolTypeId = dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId INNER JOIN dbo.vlfBoxProtocolType ON dbo.vlfChannels.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId AND dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN  dbo.vlfBoxSettings ON dbo.vlfCommMode.CommModeId = dbo.vlfBoxSettings.CommModeId AND dbo.vlfBox.BoxId = dbo.vlfBoxSettings.BoxId AND  dbo.vlfBoxProtocolType.BoxProtocolTypeId = dbo.vlfBoxSettings.BoxProtocolTypeId WHERE vlfBox.BoxId =" + boxId + " ORDER BY vlfFirmwareChannels.ChPriority ";

                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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
            return resultDataSet;
        }


        /// <summary>
        /// Get box configuration information. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet[BoxId],[FwChId],[MaxMsgs],[MaxTxtMsgs],
        ///					[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum] // HW type information
        ///					[BoxProtocolTypeId],[BoxProtocolTypeName] // box protocol type information
        ///					[CommModeId],[CommModeName],[ChPriority],
        ///					[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],
        ///               [OAPPort]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxConfigInfoFeatures(int boxId)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "Unable to retrieve box " + boxId + " configuration information. ";
                //Prepares SQL statement
                string sql = "SELECT dbo.vlfBox.BoxId, dbo.vlfFirmwareChannels.FwChId, dbo.vlfBoxHwType.BoxHwTypeId, dbo.vlfBoxHwType.BoxHwTypeName,dbo.vlfBoxHwType.MaxSensorsNum, dbo.vlfBoxHwType.MaxOutputsNum, dbo.vlfBoxProtocolType.BoxProtocolTypeId,dbo.vlfBoxProtocolType.BoxProtocolTypeName, dbo.vlfCommMode.CommModeId, dbo.vlfCommMode.CommModeName,dbo.vlfFirmwareChannels.ChPriority, dbo.vlfFirmware.FwTypeId, dbo.vlfFirmware.FwLocalPath, dbo.vlfFirmware.FwOAPPath,dbo.vlfFirmware.FwDateReleased, dbo.vlfFirmware.MaxGeozones, dbo.vlfBoxSettings.MaxMsgs, dbo.vlfBoxSettings.MaxTxtMsgs,dbo.vlfFirmware.OAPPort, dbo.vlfDcl.DclId, dbo.vlfBox.FwAttributes1 FROM dbo.vlfBoxProtocolGroup INNER JOIN dbo.vlfDcl ON dbo.vlfBoxProtocolGroup.BoxProtocolGroupId = dbo.vlfDcl.BoxProtocolGroupId INNER JOIN dbo.vlfCommMode ON dbo.vlfDcl.CommModeId = dbo.vlfCommMode.CommModeId INNER JOIN dbo.vlfBox with (nolock) INNER JOIN dbo.vlfFirmwareChannelReference ON dbo.vlfBox.FwChId = dbo.vlfFirmwareChannelReference.FwChId INNER JOIN dbo.vlfFirmwareChannels ON dbo.vlfFirmwareChannelReference.FwChId = dbo.vlfFirmwareChannels.FwChId INNER JOIN  dbo.vlfFirmware ON dbo.vlfFirmwareChannels.FwId = dbo.vlfFirmware.FwId INNER JOIN dbo.vlfChannels ON dbo.vlfFirmwareChannels.ChId = dbo.vlfChannels.ChId INNER JOIN  dbo.vlfBoxHwType ON dbo.vlfFirmware.BoxHwTypeId = dbo.vlfBoxHwType.BoxHwTypeId ON dbo.vlfCommMode.CommModeId = dbo.vlfChannels.CommModeId INNER JOIN dbo.vlfBoxlProtocolGroupAssignment ON dbo.vlfBoxProtocolGroup.BoxProtocolGroupId = dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolGroupId AND dbo.vlfChannels.BoxProtocolTypeId = dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId INNER JOIN dbo.vlfBoxProtocolType ON dbo.vlfChannels.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId AND dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN  dbo.vlfBoxSettings ON dbo.vlfCommMode.CommModeId = dbo.vlfBoxSettings.CommModeId AND dbo.vlfBox.BoxId = dbo.vlfBoxSettings.BoxId AND  dbo.vlfBoxProtocolType.BoxProtocolTypeId = dbo.vlfBoxSettings.BoxProtocolTypeId WHERE vlfBox.BoxId =" + boxId + " ORDER BY vlfFirmwareChannels.ChPriority ";

                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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
            return resultDataSet;
        }


        /// <summary>
        /// Get box configuration information - dcl excuded - used by Management Console 
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet[BoxId],[FwChId],[MaxMsgs],[MaxTxtMsgs],
        ///					[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum] // HW type information
        ///					[BoxProtocolTypeId],[BoxProtocolTypeName] // box protocol type information
        ///					[CommModeId],[CommModeName],[ChPriority],
        ///					[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],
        ///               [OAPPort]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxConfiguration(int boxId)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "Unable to retrieve box " + boxId + " configuration information. ";
                //Prepares SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@boxid", SqlDbType.Int, boxId);
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT dbo.vlfBox.BoxId, vlfFirmware.FwId, dbo.vlfFirmwareChannels.FwChId, dbo.vlfBoxHwType.BoxHwTypeId, dbo.vlfBoxHwType.BoxHwTypeName, dbo.vlfBoxHwType.MaxSensorsNum, dbo.vlfBoxHwType.MaxOutputsNum, dbo.vlfBoxProtocolType.BoxProtocolTypeId, dbo.vlfBoxProtocolType.BoxProtocolTypeName, dbo.vlfCommMode.CommModeId, dbo.vlfCommMode.CommModeName, dbo.vlfFirmwareChannels.ChPriority, dbo.vlfFirmware.FwTypeId, dbo.vlfFirmware.FwLocalPath, dbo.vlfFirmware.FwOAPPath, dbo.vlfFirmware.FwDateReleased, dbo.vlfFirmware.MaxGeozones, dbo.vlfBoxSettings.MaxMsgs, dbo.vlfBoxSettings.MaxTxtMsgs, dbo.vlfFirmware.OAPPort, dbo.vlfChannels.ChId, dbo.vlfChannels.ChName");
                sql.AppendLine("FROM dbo.vlfBoxlProtocolGroupAssignment");
                sql.AppendLine("INNER JOIN dbo.vlfBox with (nolock) INNER JOIN dbo.vlfFirmwareChannelReference ON dbo.vlfBox.FwChId = dbo.vlfFirmwareChannelReference.FwChId");
                sql.AppendLine("INNER JOIN dbo.vlfFirmwareChannels ON dbo.vlfFirmwareChannelReference.FwChId = dbo.vlfFirmwareChannels.FwChId");
                sql.AppendLine("INNER JOIN dbo.vlfFirmware ON dbo.vlfFirmwareChannels.FwId = dbo.vlfFirmware.FwId");
                sql.AppendLine("INNER JOIN dbo.vlfChannels ON dbo.vlfFirmwareChannels.ChId = dbo.vlfChannels.ChId");
                sql.AppendLine("INNER JOIN dbo.vlfBoxHwType ON dbo.vlfFirmware.BoxHwTypeId = dbo.vlfBoxHwType.BoxHwTypeId");
                sql.AppendLine("INNER JOIN dbo.vlfCommMode ON dbo.vlfChannels.CommModeId = dbo.vlfCommMode.CommModeId ON dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = dbo.vlfChannels.BoxProtocolTypeId");
                sql.AppendLine("INNER JOIN dbo.vlfBoxProtocolType ON dbo.vlfChannels.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId AND dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId");
                sql.AppendLine("INNER JOIN dbo.vlfBoxSettings ON dbo.vlfCommMode.CommModeId = dbo.vlfBoxSettings.CommModeId AND dbo.vlfBox.BoxId = dbo.vlfBoxSettings.BoxId AND dbo.vlfBoxProtocolType.BoxProtocolTypeId = dbo.vlfBoxSettings.BoxProtocolTypeId");
                sql.Append("WHERE vlfBox.BoxId = @boxid ORDER BY vlfFirmwareChannels.ChPriority");

                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
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
            return resultDataSet;
        }

        /// <summary>
        /// Get box configuration information incl. features - used by Management Console 
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet[BoxId],[FwChId],[MaxMsgs],[MaxTxtMsgs],
        ///					[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum] // HW type information
        ///					[BoxProtocolTypeId],[BoxProtocolTypeName] // box protocol type information
        ///					[CommModeId],[CommModeName],[ChPriority],
        ///					[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],
        ///               [OAPPort]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxConfigurationFeatures(int boxId)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "Unable to retrieve box " + boxId + " configuration information. ";
                //Prepares SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@boxid", SqlDbType.Int, boxId);
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT dbo.vlfBox.BoxId, vlfFirmware.FwId, dbo.vlfFirmwareChannels.FwChId, dbo.vlfBoxHwType.BoxHwTypeId, dbo.vlfBoxHwType.BoxHwTypeName, dbo.vlfBoxHwType.MaxSensorsNum, dbo.vlfBoxHwType.MaxOutputsNum, dbo.vlfBoxProtocolType.BoxProtocolTypeId, dbo.vlfBoxProtocolType.BoxProtocolTypeName, dbo.vlfCommMode.CommModeId, dbo.vlfCommMode.CommModeName, dbo.vlfFirmwareChannels.ChPriority, dbo.vlfFirmware.FwTypeId, dbo.vlfFirmware.FwLocalPath, dbo.vlfFirmware.FwOAPPath, dbo.vlfFirmware.FwDateReleased, dbo.vlfFirmware.MaxGeozones, dbo.vlfBoxSettings.MaxMsgs, dbo.vlfBoxSettings.MaxTxtMsgs, dbo.vlfFirmware.OAPPort, dbo.vlfChannels.ChId, dbo.vlfChannels.ChName, dbo.vlfBox.FwAttributes1");
                sql.AppendLine("FROM dbo.vlfBoxlProtocolGroupAssignment");
                sql.AppendLine("INNER JOIN dbo.vlfBox with (nolock) INNER JOIN dbo.vlfFirmwareChannelReference ON dbo.vlfBox.FwChId = dbo.vlfFirmwareChannelReference.FwChId");
                sql.AppendLine("INNER JOIN dbo.vlfFirmwareChannels ON dbo.vlfFirmwareChannelReference.FwChId = dbo.vlfFirmwareChannels.FwChId");
                sql.AppendLine("INNER JOIN dbo.vlfFirmware ON dbo.vlfFirmwareChannels.FwId = dbo.vlfFirmware.FwId");
                sql.AppendLine("INNER JOIN dbo.vlfChannels ON dbo.vlfFirmwareChannels.ChId = dbo.vlfChannels.ChId");
                sql.AppendLine("INNER JOIN dbo.vlfBoxHwType ON dbo.vlfFirmware.BoxHwTypeId = dbo.vlfBoxHwType.BoxHwTypeId");
                sql.AppendLine("INNER JOIN dbo.vlfCommMode ON dbo.vlfChannels.CommModeId = dbo.vlfCommMode.CommModeId ON dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = dbo.vlfChannels.BoxProtocolTypeId");
                sql.AppendLine("INNER JOIN dbo.vlfBoxProtocolType ON dbo.vlfChannels.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId AND dbo.vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId");
                sql.AppendLine("INNER JOIN dbo.vlfBoxSettings ON dbo.vlfCommMode.CommModeId = dbo.vlfBoxSettings.CommModeId AND dbo.vlfBox.BoxId = dbo.vlfBoxSettings.BoxId AND dbo.vlfBoxProtocolType.BoxProtocolTypeId = dbo.vlfBoxSettings.BoxProtocolTypeId");
                sql.Append("WHERE vlfBox.BoxId = @boxid ORDER BY vlfFirmwareChannels.ChPriority");

                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
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
            return resultDataSet;
        }

        /// <summary>
        /// Get firmware channel
        /// </summary>
        /// <param name="selectedFwId"></param>
        /// <param name="selectedPrimeCommMode"></param>
        /// <param name="selectedSecCommMode"></param>
        /// <returns>fwChId</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public short GetFwChId(short selectedFwId, short selectedPrimeCommMode, short selectedSecCommMode)
        {
            short currFwChId = VLF.CLS.Def.Const.unassignedShortValue;
            string sql = "";
            DataSet dsPrimeCommModes = null;
            try
            {
                //Prepares SQL statement
                sql = "SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.FwId = " + selectedFwId + " AND vlfChannels.CommModeId=" + selectedPrimeCommMode + " AND vlfFirmwareChannels.ChPriority = 0 ORDER BY vlfFirmwareChannels.FwChId, vlfFirmwareChannels.ChPriority";
                //Executes SQL statement
                dsPrimeCommModes = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve FwChId by FwId=" + selectedFwId + " PrimeCommMode=" + selectedPrimeCommMode + " SecCommMode=" + selectedSecCommMode + " . ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve FwChId by FwId=" + selectedFwId + " PrimeCommMode=" + selectedPrimeCommMode + " SecCommMode=" + selectedSecCommMode + " . ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (dsPrimeCommModes != null && dsPrimeCommModes.Tables.Count > 0 && dsPrimeCommModes.Tables[0].Rows.Count > 0)
            {
                // [FwChId],[CommModeId]
                foreach (DataRow primeIttr in dsPrimeCommModes.Tables[0].Rows)
                {
                    currFwChId = Convert.ToInt16(primeIttr["FwChId"]);
                    sql = "SELECT COUNT(*) FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfChannels.CommModeId=" + selectedSecCommMode + " AND vlfFirmwareChannels.ChPriority=1 AND vlfFirmwareChannels.FwChId=" + currFwChId;
                    if (selectedSecCommMode == VLF.CLS.Def.Const.unassignedShortValue)
                    {
                        if (IsPrimaryOnlyModeExist(selectedFwId, currFwChId))
                            return currFwChId;
                    }
                    else
                    {
                        int recordCount = (int)sqlExec.SQLExecuteScalar(sql);
                        if (recordCount == 1)
                            return currFwChId;
                    }
                }
                // configuration does not exist
                currFwChId = VLF.CLS.Def.Const.unassignedShortValue;
            }
            return currFwChId;
        }
        /// <summary>
        /// Checks if primary only communication mode is exist
        /// </summary>
        /// <param name="fwId"></param>
        /// <param name="fwChId"></param>
        /// <returns>true if existonly primary comm mode exist, otherwise returns false</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        private bool IsPrimaryOnlyModeExist(short fwId, short fwChId)
        {
            bool retResult = false;
            try
            {
                string sql = "SELECT COUNT(*) FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.FwId=" + fwId + " AND vlfFirmwareChannels.FwChId=" + fwChId + " AND vlfFirmwareChannels.ChPriority=0";
                int primeCount = (int)sqlExec.SQLExecuteScalar(sql);

                sql = "SELECT COUNT(*) FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.FwId=" + fwId + " AND vlfFirmwareChannels.FwChId=" + fwChId + " AND vlfFirmwareChannels.ChPriority=1";
                int secCount = (int)sqlExec.SQLExecuteScalar(sql);
                if (primeCount == 1 && secCount == 0)
                    retResult = true;
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve FwChId by FwId=" + fwId + " FwChId=" + fwChId + " . ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve FwChId by FwId=" + fwId + " FwChId=" + fwChId + " . ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return retResult;
        }
        /// <summary>
        /// Get box firmware info. 	
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet [BoxId],[FwId],[FwName],[FwChId],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],[OAPPort]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxFwChInfo(int boxId)
        {
            DataSet dsResult = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT vlfBox.BoxId, vlfFirmware.FwId, vlfFirmware.FwName, vlfFirmwareChannelReference.FwChId, vlfBoxHwType.BoxHwTypeId, vlfBoxHwType.BoxHwTypeName, vlfBoxHwType.MaxSensorsNum, vlfBoxHwType.MaxOutputsNum,vlfFirmware.FwTypeId,vlfFirmware.FwLocalPath,vlfFirmware.FwOAPPath, vlfFirmware.FwDateReleased, vlfFirmware.MaxGeozones, OAPPort FROM vlfFirmware INNER JOIN vlfFirmwareChannels ON vlfFirmware.FwId = vlfFirmwareChannels.FwId INNER JOIN vlfFirmwareChannelReference ON vlfFirmwareChannels.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId WHERE BoxId=" + boxId;
                //Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve box FwChId by box id=" + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve box FwChId by box id=" + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;
        }

        /// <summary>
        /// Get firmware channel
        /// </summary>
        /// <param name="selectedFwId"></param>
        /// <param name="protocolTypeId"></param>
        /// <returns>fwChId</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public short GetFwChId(short selectedFwId, short protocolTypeId)
        {
            short currFwChId = VLF.CLS.Def.Const.unassignedShortValue;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId WHERE vlfFirmwareChannels.FwId=" +
                   selectedFwId.ToString() + " AND vlfChannels.BoxProtocolTypeId=" + protocolTypeId.ToString() + " ORDER BY vlfFirmwareChannels.FwChId, vlfFirmwareChannels.ChPriority";
                //Executes SQL statement
                currFwChId = (short)sqlExec.SQLExecuteScalar(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve FwChId by ProtocolTypeId=" + protocolTypeId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve FwChId by FwId=" + selectedFwId.ToString() + " ProtocolTypeId=" + protocolTypeId.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return currFwChId;
        }

        /// <summary>
        /// GetBoxServerConfig by protocol and comm. mode
        /// </summary>
        /// <param name="commModeId"></param>
        /// <param name="protocolTypeId"></param>
        /// <returns></returns>
        public DataSet GetBoxServerConfig(int commModeId, int protocolTypeId)
        {
            DataSet dsResult = null;
            string prefixMsg = "Unable to retrieve server info";
            try
            {
                //Prepares SQL statement
                string sql = "SELECT IPExternal, PortExternal, IPInternal, PortInternal FROM ServerConfigCommProtocol WHERE CommModeID=" + commModeId + " AND BoxProtocolTypeID=" + protocolTypeId;
                //Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
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
            return dsResult;
        }
        #endregion

        /// <summary>
        /// Get boxes for org. (only primary channels)
        /// </summary>
        /// <param name="assigned">True for assigned boxes, false for unassigned</param>
        /// <param name="organizationId">Org. id</param>
        /// <returns>Boxes DataSet</returns>
        public DataSet GetBoxesInfo(bool assigned, int organizationId)
        {
            DataSet dsBoxes = null;
            string prefixMsg = "Unable to retrieve boxes. ";
            try
            {
                string not = "";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT vlfBox.BoxId, vlfBox.OrganizationId, vlfFirmwareChannelReference.FwChId, vlfFirmware.FwId, vlfFirmware.FwName, vlfBoxHwType.BoxHwTypeId, vlfBoxHwType.BoxHwTypeName, vlfChannels.ChName, vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfCommMode.CommModeId, vlfCommMode.CommModeName, OAPPort");
                sql.AppendLine("FROM vlfBoxHwType INNER JOIN vlfFirmware ON vlfBoxHwType.BoxHwTypeId = vlfFirmware.BoxHwTypeId INNER JOIN vlfFirmwareChannels ON vlfFirmware.FwId = vlfFirmwareChannels.FwId INNER JOIN vlfChannels INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfFirmwareChannelReference ON vlfFirmwareChannels.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId");
                not = assigned ? "" : "NOT";
                sql.AppendFormat("WHERE vlfBox.BoxId {0} IN (SELECT BoxId FROM vlfVehicleAssignment) AND vlfFirmwareChannels.ChPriority = 0", not);
                sql.AppendLine();
                if (organizationId != -1)
                    sql.AppendLine("AND vlfBox.OrganizationId = @orgId");
                sql.Append("ORDER BY vlfBox.BoxId");

                dsBoxes = sqlExec.SQLExecuteDataset(sql.ToString());
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
            return dsBoxes;
        }


        //Get BoxId based on BlackBerry PIN
        public int GetBoxId4BB(int PIN)
        {
            int boxId = 0;
            string prefixMsg = "Unable to retrieve boxId for BB.";
            try
            {


                string sql = "SELECT  dbo.vlfPeripheralBoxAssigment.BoxId FROM dbo.vlfPeripheralBoxAssigment INNER JOIN dbo.vlfBlackBerry ON dbo.vlfPeripheralBoxAssigment.PeripheralId = dbo.vlfBlackBerry.BlackBerryId" +
                    " WHERE   dbo.vlfPeripheralBoxAssigment.TypeId = 6 and dbo.vlfBlackBerry.PIN=" + PIN;
                boxId = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
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
            return boxId;
        }


        public int BoxExtraInfo_AddUpdate(int BoxId, DateTime Timestamp, Int16 MsgTypeId, string CustomProp, Int64 SensorMask)
        {
            int rowsAffected = 0;
            try
            {

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, BoxId);
                sqlExec.AddCommandParam("@Timestamp", SqlDbType.DateTime, Timestamp);
                sqlExec.AddCommandParam("@MsgTypeId", SqlDbType.SmallInt, MsgTypeId);
                sqlExec.AddCommandParam("@CustomProp", SqlDbType.VarChar, CustomProp);
                sqlExec.AddCommandParam("@SensorMask", SqlDbType.BigInt, SensorMask);
                rowsAffected = sqlExec.SPExecuteNonQuery("vlfBoxExtraInfo_AddUpdate");
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add or Update BoxExtraInfo. BoxId: " + BoxId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add or Update BoxExtraInfo. BoxId: " + BoxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }


        public int BoxCmdHist_Add(int BoxId, int BoxCmdOutTypeId, DateTime DateTimeSent, string CustomProp, int UserId)
        {
            int rowsAffected = 0;
            try
            {

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, BoxId);
                sqlExec.AddCommandParam("@BoxCmdOutTypeId", SqlDbType.SmallInt, BoxCmdOutTypeId);
                sqlExec.AddCommandParam("@DateTimeSent", SqlDbType.DateTime, DateTimeSent);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                sqlExec.AddCommandParam("@CustomProp", SqlDbType.VarChar, CustomProp);
                rowsAffected = sqlExec.SPExecuteNonQuery("BoxCmdHist_Add");
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add to vlfBoxCmdHist. BoxId: " + BoxId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add to vlfBoxCmdHist. BoxId: " + BoxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }


        public int BoxCmdHist_Update(int BoxId, int BoxCmdOutTypeId, DateTime DateTimeAck)
        {
            int rowsAffected = 0;
            try
            {

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, BoxId);
                sqlExec.AddCommandParam("@BoxCmdOutTypeId", SqlDbType.SmallInt, BoxCmdOutTypeId);
                sqlExec.AddCommandParam("@DateTimeAck", SqlDbType.DateTime, DateTimeAck);
                rowsAffected = sqlExec.SPExecuteNonQuery("BoxCmdHist_Update");
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Update vlfBoxCmdHist. BoxId: " + BoxId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Update vlfBoxCmdHist. BoxId: " + BoxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }


        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public DataSet GetLastCommunicatedDateTimeFromHistory(DateTime dDate, Int32 boxId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "GetLastCommunicatedDateTime";
                sqlExec.AddCommandParam("@DDate", SqlDbType.DateTime, dDate);
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetLastCommunicatedDateTimeFromHistory by boxId=" + boxId ;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetLastCommunicatedDateTimeFromHistory by boxId=" + boxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
    }
}
