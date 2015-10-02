using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using VLF.DAS;
using VLF.DAS.DB;

namespace VLF.PATCH.DB
{
    public class PatchOrganizationGeozone : TblGenInterfaces
    {
        /// <summary>
        /// Provides interfaces to vlfMapLayers table.
        /// </summary>
        public PatchOrganizationGeozone(SQLExecuter sqlExec)
            : base("vlfOrganizationGeozone", sqlExec)
        {
        }
        // Changes for TimeZone Feature start

        /// <summary>
        /// Get Organization Equipments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet PatchGetOrganizationGeozonesWithStatus_NewTZ(int OrganizationId, int userId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "GetOrganizationGeozonesWithStatusByUserID_NewTimeZone";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GeoZones by OrganizationId=" + OrganizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GeoZones by OrganizationId=" + OrganizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Get Organization Equipments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet PatchGetOrganizationGeozonesWithStatus(int OrganizationId, int userId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "GetOrganizationGeozonesWithStatusByUserID";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GeoZones by OrganizationId=" + OrganizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GeoZones by OrganizationId=" + OrganizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Add new geozone to organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public Int64 PatchAddGeozone_NewTZ(int organizationId, string geozoneName,
                                short type, short geozoneType,
                                short severityId, string description,
                                string email, string phone, float timeZone, bool dayLightSaving,
                                short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool isPublic, int userId)
        {


            Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
            // 1. Prepares SQL statement
            try
            {
                short geozoneId = GetNextFreeGeozoneId(organizationId);
                // 1. Set SQL command
                string sql = "INSERT INTO " + tableName + "( " +
                    "OrganizationId" +
                    ",GeozoneId" +
                    ",GeozoneName" +
                    ",Type" +
                    ",GeozoneType" +
                    ",SeverityId" +
                    ",Description" +
                    ",Email" +
                    ",Phone" +
                    ",TimeZoneNew" +
                    ",DayLightSaving" +
                    ",FormatType" +
                    ",Notify" +
                    ",Warning" +
                    ",Critical" +
                    ",AutoAdjustDayLightSaving,speed, [Public], CreateUserID)" +
                    " VALUES ( @OrganizationId,@GeozoneId,@GeozoneName,@Type,@GeozoneType,@SeverityId,@Description,@Email,@Phone,@TimeZone,@DayLightSaving,@FormatType,@Notify,@Warning,@Critical,@AutoAdjustDayLightSaving,@speed, @Public, @CreateUserID )";
                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@GeozoneId", SqlDbType.SmallInt, geozoneId);
                sqlExec.AddCommandParam("@GeozoneName", SqlDbType.Char, geozoneName);
                sqlExec.AddCommandParam("@Type", SqlDbType.SmallInt, type);
                sqlExec.AddCommandParam("@GeozoneType", SqlDbType.SmallInt, geozoneType);
                sqlExec.AddCommandParam("@SeverityId", SqlDbType.SmallInt, severityId);
                if (description == null || description == "")
                    description = geozoneName;
                sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
                if (email == null || email == "")
                    sqlExec.AddCommandParam("@Email", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Email", SqlDbType.Char, email);

                if (phone == null || phone == "")
                    sqlExec.AddCommandParam("@Phone", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Phone", SqlDbType.Char, phone);



                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Float, timeZone);
                sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(dayLightSaving));
                sqlExec.AddCommandParam("@FormatType", SqlDbType.SmallInt, formatType);
                sqlExec.AddCommandParam("@Notify", SqlDbType.SmallInt, Convert.ToInt16(notify));
                sqlExec.AddCommandParam("@Warning", SqlDbType.SmallInt, Convert.ToInt16(warning));
                sqlExec.AddCommandParam("@Critical", SqlDbType.SmallInt, Convert.ToInt16(critical));
                sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(autoAdjustDayLightSaving));
                sqlExec.AddCommandParam("@speed", SqlDbType.Int, speed);
                sqlExec.AddCommandParam("@Public", SqlDbType.SmallInt, Convert.ToInt16(isPublic));
                sqlExec.AddCommandParam("@CreateUserID", SqlDbType.Int, Convert.ToInt16(userId));

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
                geozoneNo = PatchGetGeozoneNoByGeozoneId(organizationId, geozoneId);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new Geozone to OrganizationId '" + organizationId + " GeozoneName=" + geozoneName + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new Geozone to OrganizationId '" + organizationId + " GeozoneName=" + geozoneName + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return geozoneNo;
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Add new geozone to organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public Int64 PatchAddGeozone(int organizationId, string geozoneName,
                                short type, short geozoneType,
                                short severityId, string description,
                                string email, string phone, int timeZone, bool dayLightSaving,
                                short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool isPublic, int userId)
        {


            Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
            // 1. Prepares SQL statement
            try
            {
                short geozoneId = GetNextFreeGeozoneId(organizationId);
                // 1. Set SQL command
                string sql = "INSERT INTO " + tableName + "( " +
                    "OrganizationId" +
                    ",GeozoneId" +
                    ",GeozoneName" +
                    ",Type" +
                    ",GeozoneType" +
                    ",SeverityId" +
                    ",Description" +
                    ",Email" +
                    ",Phone" +
                    ",TimeZone" +
                    ",DayLightSaving" +
                    ",FormatType" +
                    ",Notify" +
                    ",Warning" +
                    ",Critical" +
                    ",AutoAdjustDayLightSaving,speed, [Public], CreateUserID)" +
                    " VALUES ( @OrganizationId,@GeozoneId,@GeozoneName,@Type,@GeozoneType,@SeverityId,@Description,@Email,@Phone,@TimeZone,@DayLightSaving,@FormatType,@Notify,@Warning,@Critical,@AutoAdjustDayLightSaving,@speed, @Public, @CreateUserID )";
                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@GeozoneId", SqlDbType.SmallInt, geozoneId);
                sqlExec.AddCommandParam("@GeozoneName", SqlDbType.Char, geozoneName);
                sqlExec.AddCommandParam("@Type", SqlDbType.SmallInt, type);
                sqlExec.AddCommandParam("@GeozoneType", SqlDbType.SmallInt, geozoneType);
                sqlExec.AddCommandParam("@SeverityId", SqlDbType.SmallInt, severityId);
                if (description == null || description == "")
                    description = geozoneName;
                sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
                if (email == null || email == "")
                    sqlExec.AddCommandParam("@Email", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Email", SqlDbType.Char, email);

                if (phone == null || phone == "")
                    sqlExec.AddCommandParam("@Phone", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Phone", SqlDbType.Char, phone);



                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Int, timeZone);
                sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(dayLightSaving));
                sqlExec.AddCommandParam("@FormatType", SqlDbType.SmallInt, formatType);
                sqlExec.AddCommandParam("@Notify", SqlDbType.SmallInt, Convert.ToInt16(notify));
                sqlExec.AddCommandParam("@Warning", SqlDbType.SmallInt, Convert.ToInt16(warning));
                sqlExec.AddCommandParam("@Critical", SqlDbType.SmallInt, Convert.ToInt16(critical));
                sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(autoAdjustDayLightSaving));
                sqlExec.AddCommandParam("@speed", SqlDbType.Int, speed);
                sqlExec.AddCommandParam("@Public", SqlDbType.SmallInt, Convert.ToInt16(isPublic));
                sqlExec.AddCommandParam("@CreateUserID", SqlDbType.Int, Convert.ToInt16(userId));

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
                geozoneNo = PatchGetGeozoneNoByGeozoneId(organizationId, geozoneId);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new Geozone to OrganizationId '" + organizationId + " GeozoneName=" + geozoneName + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new Geozone to OrganizationId '" + organizationId + " GeozoneName=" + geozoneName + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return geozoneNo;
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Update geozone info.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASAppResultNotFoundException">Thrown if geozone does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void PatchUpdateGeozone_NewTZ(int organizationId, short geozoneId,
                                    string geozoneName,
                                    short type, Int64 geozoneType,
                                    short severityId, string description,
                                    string email, string phone, float timeZone, bool dayLightSaving,
                                    short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool? isPublic, int userId)
        {



            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = "UPDATE " + tableName +
                        " SET Description='" + description.Replace("'", "''") + "'" +
                        ", GeozoneName='" + geozoneName.Replace("'", "''") + "'" +
                        ", SeverityId=" + severityId +
                        ", Phone='" + phone.Replace("'", "''") + "'" +
                        ", Email='" + email.Replace("'", "''") + "'" +
                    ", TimeZoneNew=" + timeZone +
                    ", DayLightSaving=" + Convert.ToInt16(dayLightSaving) +
                    ", FormatType=" + formatType +
                    ", Notify=" + Convert.ToInt16(notify) +
                    ", Warning=" + Convert.ToInt16(warning) +
                    ", Critical=" + Convert.ToInt16(critical) +
                    ", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
                    ", CreateUserID=" + userId.ToString();

            if (isPublic != null)
                sql += ", [Public]=" + Convert.ToInt16(isPublic);




            //if(email != null && email != "")
            //{
            //    sql += ", Email='" + email.Replace("'","''") + "'" +
            //        ", TimeZone=" + timeZone + 
            //        ", DayLightSaving=" + Convert.ToInt16(dayLightSaving) +
            //        ", FormatType=" + formatType +
            //        ", Notify=" + Convert.ToInt16(notify) +
            //        ", Warning=" + Convert.ToInt16(warning) +
            //        ", Critical=" + Convert.ToInt16(critical) + 
            //        ", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving);
            //}
            //else
            //{
            //    sql += ", Email='',Phone='', TimeZone=0, DayLightSaving=0, FormatType=0, Notify=0, Warning=0, Critical=0, AutoAdjustDayLightSaving=0";
            //}
            // Do not allow to make changes if geozone assigned to the vehicle
            if (PatchIsGeozoneAssigned(organizationId, geozoneId) == false)
            {
                sql += ", GeozoneType=" + geozoneType;
                sql += ", Type=" + type;
            }

            sql += " WHERE organizationId=" + organizationId + " AND GeozoneId=" + geozoneId;

            try
            {
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
                string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
                throw new DASAppResultNotFoundException(prefixMsg + " This geozone does not exist.");
            }
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Update geozone info.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASAppResultNotFoundException">Thrown if geozone does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void PatchUpdateGeozone(int organizationId, short geozoneId,
                                    string geozoneName,
                                    short type, Int64 geozoneType,
                                    short severityId, string description,
                                    string email, string phone, int timeZone, bool dayLightSaving,
                                    short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool? isPublic, int userId)
        {



            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = "UPDATE " + tableName +
                        " SET Description='" + description.Replace("'", "''") + "'" +
                        ", GeozoneName='" + geozoneName.Replace("'", "''") + "'" +
                        ", SeverityId=" + severityId +
                        ", Phone='" + phone.Replace("'", "''") + "'" +
                        ", Email='" + email.Replace("'", "''") + "'" +
                    ", TimeZone=" + timeZone +
                    ", DayLightSaving=" + Convert.ToInt16(dayLightSaving) +
                    ", FormatType=" + formatType +
                    ", Notify=" + Convert.ToInt16(notify) +
                    ", Warning=" + Convert.ToInt16(warning) +
                    ", Critical=" + Convert.ToInt16(critical) +
                    ", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
                    ", CreateUserID=" + userId.ToString();

            if (isPublic != null)
                sql += ", [Public]=" + Convert.ToInt16(isPublic);




            //if(email != null && email != "")
            //{
            //    sql += ", Email='" + email.Replace("'","''") + "'" +
            //        ", TimeZone=" + timeZone + 
            //        ", DayLightSaving=" + Convert.ToInt16(dayLightSaving) +
            //        ", FormatType=" + formatType +
            //        ", Notify=" + Convert.ToInt16(notify) +
            //        ", Warning=" + Convert.ToInt16(warning) +
            //        ", Critical=" + Convert.ToInt16(critical) + 
            //        ", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving);
            //}
            //else
            //{
            //    sql += ", Email='',Phone='', TimeZone=0, DayLightSaving=0, FormatType=0, Notify=0, Warning=0, Critical=0, AutoAdjustDayLightSaving=0";
            //}
            // Do not allow to make changes if geozone assigned to the vehicle
            if (PatchIsGeozoneAssigned(organizationId, geozoneId) == false)
            {
                sql += ", GeozoneType=" + geozoneType;
                sql += ", Type=" + type;
            }

            sql += " WHERE organizationId=" + organizationId + " AND GeozoneId=" + geozoneId;

            try
            {
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
                string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
                throw new DASAppResultNotFoundException(prefixMsg + " This geozone does not exist.");
            }
        }


        /// <summary>
        /// Add geozone set.
        /// </summary>
        /// <param name="geozoneNo"></param>
        /// <param name="dsGeozoneSet"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void PatchAddGeozoneSet(Int64 geozoneNo, DataSet dsGeozoneSet)
        {
            if (dsGeozoneSet == null || dsGeozoneSet.Tables.Count == 0 || dsGeozoneSet.Tables[0].Rows.Count == 0)
                throw new DASAppViolatedIntegrityConstraintsException("Set of geozones is empty.");

            foreach (DataRow ittr in dsGeozoneSet.Tables[0].Rows)
            {
                // 1. Prepares SQL statement
                try
                {
                    // 1. Set SQL command
                    string sql = "INSERT INTO vlfGeozoneSet( " +
                        "GeozoneNo" +
                        ",SequenceNum" +
                        ",Latitude" +
                        ",Longitude)" +
                        " VALUES (@GeozoneNo,@SequenceNum,@Latitude,@Longitude)";
                    // 2. Add parameters to SQL statement
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@GeozoneNo", SqlDbType.BigInt, geozoneNo);
                    sqlExec.AddCommandParam("@SequenceNum", SqlDbType.SmallInt, Convert.ToInt16(ittr["SequenceNum"]));
                    sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, Convert.ToDouble(ittr["Latitude"]));
                    sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, Convert.ToDouble(ittr["Longitude"]));

                    if (sqlExec.RequiredTransaction())
                    {
                        // 3. Attaches SQL to transaction
                        sqlExec.AttachToTransaction(sql);
                    }

                    // 4. Executes SQL statement
                    sqlExec.SQLExecuteNonQuery(sql);
                }
                catch (SqlException objException)
                {
                    string prefixMsg = "Unable to add geozoneNo=" + geozoneNo + " sequenceNum=" + ittr["SequenceNum"].ToString() + " latitude=" + ittr["Latitude"].ToString() + " longitude=" + ittr["Longitude"].ToString() + ".";
                    Util.ProcessDbException(prefixMsg, objException);
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception objException)
                {
                    string prefixMsg = "Unable to add geozoneNo=" + geozoneNo + " sequenceNum=" + ittr["SequenceNum"].ToString() + " latitude=" + ittr["Latitude"].ToString() + " longitude=" + ittr["Longitude"].ToString() + ".";
                    throw new DASException(prefixMsg + " " + objException.Message);
                }
            }
        }

        /// <summary>
        /// Deletes geozone set.
        /// </summary>
        /// <param name="geozoneNo"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int PatchDeleteGeozoneSet(Int64 geozoneNo)
        {
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = "DELETE FROM vlfGeozoneSet WHERE GeozoneNo=" + geozoneNo;

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
                string prefixMsg = "Unable to delete geozone set by geozoneNo=" + geozoneNo;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete geozone set by geozoneNo=" + geozoneNo;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        /// <summary>
        /// Retrieves geozoneNo by geozone id
        /// </summary>
        /// <returns>Int64</returns>
        /// <param name="organizationId"></param> 
        /// <param name="geozoneId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public Int64 PatchGetGeozoneNoByGeozoneId(int organizationId, int geozoneId)
        {
            Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT ISNULL(GeozoneNo," + VLF.CLS.Def.Const.unassignedIntValue + ") AS GeozoneNo FROM vlfOrganizationGeozone WHERE OrganizationId=" + organizationId + " AND GeozoneId=" + geozoneId;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                geozoneNo = Convert.ToInt64(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retrieve geozoneNo by geozone id. ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retrieve geozoneNo by geozone id. " + objException.Message);
            }
            return geozoneNo;
        }

        /// <summary>
        /// Check if geozone assigned to any of vehicles
        /// </summary>
        /// <returns>true if assigned, otherwise false</returns>
        /// <param name="organizationId"></param> 
        /// <param name="geozoneId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public bool PatchIsGeozoneAssigned(int organizationId, short geozoneId)
        {
            int geozoneCount = 0;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT count(*) GeozoneNo" +
                    " FROM vlfVehicleGeozone INNER JOIN vlfOrganizationGeozone ON vlfVehicleGeozone.GeozoneNo = vlfOrganizationGeozone.GeozoneNo" +
                    " WHERE vlfOrganizationGeozone.GeozoneId=" + geozoneId +
                    " AND vlfOrganizationGeozone.OrganizationId=" + organizationId;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                geozoneCount = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to check geozone " + geozoneId + " assignment. ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to check geozone " + geozoneId + " assignment. " + objException.Message);
            }
            if (geozoneCount == 0)
                return false;
            else
                return true;
        }

        public DataSet GetAllUnassignedToFleetGeozonesInfo(int organizationId, int fleetId, int userId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "sp_GetAllUnassignedToFleetGeozonesInfo";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve AllUnassignedToFleetGeozones by OrganizationId=" + organizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve AllUnassignedToFleetGeozones by OrganizationId=" + organizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetAllAssignedToFleetGeozonesInfo(int organizationId, int fleetId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "sp_GetAllAssignedToFleetGeozonesInfo";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);                

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve AllAssignedToFleetGeozonesInfo by OrganizationId=" + organizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve AllAssignedToFleetGeozonesInfo by OrganizationId=" + organizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int AssignGeozoneToFleet(int organizationId, int geozoneNo, int fleetId)
        {
            try
            {

                string sql = "sp_AssignObjectToFleet";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@objectId", SqlDbType.Int, geozoneNo);
                sqlExec.AddCommandParam("@objectName", SqlDbType.VarChar, "geozone");
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@assignToAllVehiclesFleet", SqlDbType.Int, 1);


                return sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to AssignGeozoneToFleet by geozoneNo=" + geozoneNo.ToString() + "; fleetId=" + fleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to AssignGeozoneToFleet by geozoneNo=" + geozoneNo.ToString() + "; fleetId=" + fleetId.ToString();

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return 0;
        }

        public int UnassignGeozoneToFleet(int organizationId, int geozoneNo, int fleetId)
        {
            try
            {

                string sql = "sp_UnassignObjectFromFleet";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@objectId", SqlDbType.Int, geozoneNo);
                sqlExec.AddCommandParam("@objectName", SqlDbType.VarChar, "geozone");
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@unassignFromAllVehiclesFleet", SqlDbType.Int, 1);


                return sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to UnassignGeozoneToFleet by geozoneNo=" + geozoneNo.ToString() + "; fleetId=" + fleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to UnassignGeozoneToFleet by geozoneNo=" + geozoneNo.ToString() + "; fleetId=" + fleetId.ToString();

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return 0;
        }

        public int GetGeozoneNoByGeozoneName(int organizationId, string geozoneName)
        {
            string sql = "sp_GetGeozoneNoByGeozoneName";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@geozoneName", SqlDbType.VarChar, geozoneName);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Tables[0].Rows[0][0].ToString());
            }
            return 0;


        }


        #region Private Interfaces
        /// <summary>
        /// Retrieves next free geozone id
        /// </summary>
        /// <returns>short</returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private short GetNextFreeGeozoneId(int organizationId)
        {
            short nextGeoZone = 0;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT MAX(GeozoneId) FROM vlfOrganizationGeozone WHERE OrganizationId=" + organizationId;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    nextGeoZone = Convert.ToInt16(obj);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retrieve next free geozone id. ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retrieve next free geozone id. " + objException.Message);
            }
            return ++nextGeoZone;
        }

        #endregion
    }
}
