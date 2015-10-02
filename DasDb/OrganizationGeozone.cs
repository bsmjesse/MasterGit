using System;
using System.Data.SqlClient ;	// for SqlException
using System.Data ;			// for DataSet
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfOrganization table.
	/// </summary>
	public class OrganizationGeozone : TblGenInterfaces
	{
		#region Public Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public OrganizationGeozone(SQLExecuter sqlExec) : base ("vlfOrganizationGeozone",sqlExec)
		{
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
        public Int64 AddGeozone_NewTZ(int organizationId, string geozoneName,
                                short type, short geozoneType,
                                short severityId, string description,
                                string email, string phone, float timeZone, bool dayLightSaving,
                                short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool publicGeozone, int userId)
        {


            Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
            // 1. Prepares SQL statement
            try
            {
                int timeZoneOld = (int)timeZone;
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
                    ",TimeZoneNew" +
                    ",DayLightSaving" +
                    ",FormatType" +
                    ",Notify" +
                    ",Warning" +
                    ",Critical" +
                    ",AutoAdjustDayLightSaving,speed,[Public],CreateUserID)" +
                    " VALUES ( @OrganizationId,@GeozoneId,@GeozoneName,@Type,@GeozoneType,@SeverityId,@Description,@Email,@Phone,@TimeZone,@TimeZoneNew,@DayLightSaving,@FormatType,@Notify,@Warning,@Critical,@AutoAdjustDayLightSaving,@speed,@publicGeozone,@CreateUserID)";
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


                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Float, timeZoneOld);
                sqlExec.AddCommandParam("@TimeZoneNew", SqlDbType.Float, timeZone);
                sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(dayLightSaving));
                sqlExec.AddCommandParam("@FormatType", SqlDbType.SmallInt, formatType);
                sqlExec.AddCommandParam("@Notify", SqlDbType.SmallInt, Convert.ToInt16(notify));
                sqlExec.AddCommandParam("@Warning", SqlDbType.SmallInt, Convert.ToInt16(warning));
                sqlExec.AddCommandParam("@Critical", SqlDbType.SmallInt, Convert.ToInt16(critical));
                sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(autoAdjustDayLightSaving));
                sqlExec.AddCommandParam("@speed", SqlDbType.Int, speed);
                sqlExec.AddCommandParam("@publicGeozone", SqlDbType.SmallInt, Convert.ToInt16(publicGeozone));
                sqlExec.AddCommandParam("@CreateUserID", SqlDbType.Int, userId);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
                geozoneNo = GetGeozoneNoByGeozoneId(organizationId, geozoneId);
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
        public Int64 AddGeozone(int organizationId, string geozoneName,
                                short type, short geozoneType,
                                short severityId, string description,
                                string email, string phone, int timeZone, bool dayLightSaving,
                                short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool publicGeozone, int userId)
        {

        
			Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
			// 1. Prepares SQL statement
			try
			{
				short geozoneId = GetNextFreeGeozoneId(organizationId);
				// 1. Set SQL command
				string sql = "INSERT INTO " + tableName + "( "  +
					"OrganizationId"	+
					",GeozoneId"		+
					",GeozoneName"		+
					",Type"				+
					",GeozoneType"		+
					",SeverityId"		+
					",Description"		+
					",Email"			+
                    ",Phone"            +
					",TimeZone"			+
					",DayLightSaving"	+
					",FormatType"		+
					",Notify"			+
					",Warning"			+
					",Critical"			+
                    ",AutoAdjustDayLightSaving,speed,[Public],CreateUserID)" +
                    " VALUES ( @OrganizationId,@GeozoneId,@GeozoneName,@Type,@GeozoneType,@SeverityId,@Description,@Email,@Phone,@TimeZone,@DayLightSaving,@FormatType,@Notify,@Warning,@Critical,@AutoAdjustDayLightSaving,@speed,@publicGeozone,@CreateUserID)";
				// 2. Add parameters to SQL statement
				sqlExec.ClearCommandParameters();
				sqlExec.AddCommandParam("@OrganizationId",SqlDbType.Int,organizationId);
				sqlExec.AddCommandParam("@GeozoneId",SqlDbType.SmallInt,geozoneId);
				sqlExec.AddCommandParam("@GeozoneName",SqlDbType.Char,geozoneName);
				sqlExec.AddCommandParam("@Type",SqlDbType.SmallInt,type);
				sqlExec.AddCommandParam("@GeozoneType",SqlDbType.SmallInt,geozoneType);
				sqlExec.AddCommandParam("@SeverityId",SqlDbType.SmallInt,severityId);
				if(description == null || description == "")
					description = geozoneName;
				sqlExec.AddCommandParam("@Description",SqlDbType.Char,description);
				if(email == null || email == "")
					sqlExec.AddCommandParam("@Email",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@Email",SqlDbType.Char,email);

                if (phone == null || phone == "")
                    sqlExec.AddCommandParam("@Phone", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Phone", SqlDbType.Char, phone);



				sqlExec.AddCommandParam("@TimeZone",SqlDbType.Int,timeZone);
				sqlExec.AddCommandParam("@DayLightSaving",SqlDbType.SmallInt,Convert.ToInt16(dayLightSaving));
				sqlExec.AddCommandParam("@FormatType",SqlDbType.SmallInt,formatType);
				sqlExec.AddCommandParam("@Notify",SqlDbType.SmallInt,Convert.ToInt16(notify));
				sqlExec.AddCommandParam("@Warning",SqlDbType.SmallInt,Convert.ToInt16(warning));
				sqlExec.AddCommandParam("@Critical",SqlDbType.SmallInt,Convert.ToInt16(critical));
				sqlExec.AddCommandParam("@AutoAdjustDayLightSaving",SqlDbType.SmallInt,Convert.ToInt16(autoAdjustDayLightSaving));
                sqlExec.AddCommandParam("@speed", SqlDbType.Int, speed);
                sqlExec.AddCommandParam("@publicGeozone", SqlDbType.SmallInt, Convert.ToInt16(publicGeozone));
                sqlExec.AddCommandParam("@CreateUserID", SqlDbType.Int, userId);
				
				if(sqlExec.RequiredTransaction())
				{
					// 3. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 4. Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
				geozoneNo = GetGeozoneNoByGeozoneId(organizationId,geozoneId);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to add new Geozone to OrganizationId '" + organizationId + " GeozoneName=" + geozoneName + "." ;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to add new Geozone to OrganizationId '" + organizationId + " GeozoneName=" + geozoneName + "." ;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return geozoneNo;
		}
		/// <summary>
		/// Add geozone set.
		/// </summary>
		/// <param name="geozoneNo"></param>
		/// <param name="dsGeozoneSet"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddGeozoneSet(Int64 geozoneNo,DataSet dsGeozoneSet)
		{
			if(dsGeozoneSet == null || dsGeozoneSet.Tables.Count == 0 || dsGeozoneSet.Tables[0].Rows.Count == 0)
				throw new DASAppViolatedIntegrityConstraintsException("Set of geozones is empty.");

			foreach(DataRow ittr in dsGeozoneSet.Tables[0].Rows)
			{
				// 1. Prepares SQL statement
				try
				{
					// 1. Set SQL command
					string sql = "INSERT INTO vlfGeozoneSet( "  +
						"GeozoneNo"	+
						",SequenceNum"	+
						",Latitude"		+
						",Longitude)"	+
						" VALUES (@GeozoneNo,@SequenceNum,@Latitude,@Longitude)";
					// 2. Add parameters to SQL statement
					sqlExec.ClearCommandParameters();
					sqlExec.AddCommandParam("@GeozoneNo",SqlDbType.BigInt,geozoneNo);
					sqlExec.AddCommandParam("@SequenceNum",SqlDbType.SmallInt,Convert.ToInt16(ittr["SequenceNum"]));
					sqlExec.AddCommandParam("@Latitude",SqlDbType.Float,Convert.ToDouble(ittr["Latitude"]));
					sqlExec.AddCommandParam("@Longitude",SqlDbType.Float,Convert.ToDouble(ittr["Longitude"]));
					
					if(sqlExec.RequiredTransaction())
					{
						// 3. Attaches SQL to transaction
						sqlExec.AttachToTransaction(sql);
					}
					
					// 4. Executes SQL statement
					sqlExec.SQLExecuteNonQuery(sql);
				}
				catch (SqlException objException) 
				{
					string prefixMsg =	"Unable to add geozoneNo=" + geozoneNo + " sequenceNum=" + ittr["SequenceNum"].ToString() + " latitude=" + ittr["Latitude"].ToString() + " longitude=" + ittr["Longitude"].ToString() + "." ;
					Util.ProcessDbException(prefixMsg,objException);
				}
				catch(DASDbConnectionClosed exCnn)
				{
					throw new DASDbConnectionClosed(exCnn.Message);
				}
				catch(Exception objException)
				{
					string prefixMsg =	"Unable to add geozoneNo=" + geozoneNo + " sequenceNum=" + ittr["SequenceNum"].ToString() + " latitude=" + ittr["Latitude"].ToString() + " longitude=" + ittr["Longitude"].ToString() + "." ;
					throw new DASException(prefixMsg + " " + objException.Message);
				}
			}
		}
		/// <summary>
		/// Deletes geozone from organization.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="geozoneId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteGeozoneFromOrganization(int organizationId,short geozoneId)
		{
			// Do not allow to make changes if geozone assigned to the vehicle
			//if(IsGeozoneAssigned(organizationId,geozoneId))
			//	throw new DASAppViolatedIntegrityConstraintsException("Unable to delete geozone " + geozoneId + " related to organization " + organizationId + ". Geozone is in used.");
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + 
				" WHERE OrganizationId=" + organizationId + " AND GeozoneId=" + geozoneId;

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
				string prefixMsg = "Unable to delete geozoneId=" + geozoneId + " from organization " + organizationId;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete geozoneId=" + geozoneId + " from organization " + organizationId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Deletes geozone set.
		/// </summary>
		/// <param name="geozoneNo"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteGeozoneSet(Int64 geozoneNo)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM vlfGeozoneSet WHERE GeozoneNo=" + geozoneNo;

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
				string prefixMsg = "Unable to delete geozone set by geozoneNo=" + geozoneNo;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete geozone set by geozoneNo=" + geozoneNo;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
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
        public void UpdateGeozone_NewTZ(int organizationId, short geozoneId,
                                    string geozoneName,
                                    short type, Int64 geozoneType,
                                    short severityId, string description,
                                    string email, string phone, float timeZone, bool dayLightSaving,
                                    short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool publicGeozone, int userId)
        {



            int rowsAffected = 0;
            int timeZoneOld = (int)timeZone;
            // 1. Prepares SQL statement
            string sql = "UPDATE " + tableName +
                        " SET Description='" + description.Replace("'", "''") + "'" +
                        ", GeozoneName='" + geozoneName.Replace("'", "''") + "'" +
                        ", SeverityId=" + severityId +
                        ", Phone='" + phone.Replace("'", "''") + "'" +
                        ", Email='" + email.Replace("'", "''") + "'" +
                    ", TimeZoneNew=" + timeZone +
                    ", TimeZone=" + timeZoneOld +
                    ", DayLightSaving=" + Convert.ToInt16(dayLightSaving) +
                    ", FormatType=" + formatType +
                    ", Notify=" + Convert.ToInt16(notify) +
                    ", Warning=" + Convert.ToInt16(warning) +
                    ", Critical=" + Convert.ToInt16(critical) +
                    ", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
                    ", [Public]=" + Convert.ToInt16(publicGeozone);
            //", CreateUserID=" + userId;




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
            if (IsGeozoneAssigned(organizationId, geozoneId) == false)
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
        public void UpdateGeozone(int organizationId, short geozoneId,
                                    string geozoneName,
                                    short type, Int64 geozoneType,
                                    short severityId, string description,
                                    string email, string phone, int timeZone, bool dayLightSaving,
                                    short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool publicGeozone, int userId)
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
                    ", [Public]=" + Convert.ToInt16(publicGeozone);
                    //", CreateUserID=" + userId;
            



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
			if(IsGeozoneAssigned(organizationId,geozoneId) == false)
			{
				sql +=	", GeozoneType=" + geozoneType;
				sql +=	", Type=" + type;
			}
				
			sql += " WHERE organizationId=" + organizationId + " AND GeozoneId=" + geozoneId;

			try
			{
				if(sqlExec.RequiredTransaction())
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
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update organization Id " + organizationId + " GeozoneId=" + geozoneId + ".";
				throw new DASAppResultNotFoundException(prefixMsg + " This geozone does not exist.");
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
			string sql = "UPDATE vlfOrganizationGeozone SET DayLightSaving=" + Convert.ToInt16(dayLightSaving) + " WHERE AutoAdjustDayLightSaving=1";
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
		/// <param name="organizationId"></param>
		/// <param name="geozoneId"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetAutoAdjustDayLightSaving(int organizationId,short geozoneId,bool autoAdjustDayLightSaving,bool dayLightSaving)
		{
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfFleetEmails SET AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
				" ,DayLightSaving=" + Convert.ToInt16(dayLightSaving) + 
				" WHERE organizationId=" + organizationId + " AND GeozoneId=" + geozoneId;
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
				string prefixMsg = "Unable to update autoAdjustDayLightSaving=" + autoAdjustDayLightSaving.ToString() + " dayLightSaving=" + dayLightSaving.ToString() + " organizationId=" + organizationId.ToString() + " geozoneId=" + geozoneId +".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update autoAdjustDayLightSaving=" + autoAdjustDayLightSaving.ToString() + " dayLightSaving=" + dayLightSaving.ToString() + " organizationId=" + organizationId.ToString() + " geozoneId=" + geozoneId +".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}

		/// <summary>
		/// Retrieves geozones info by organization id 
		/// </summary>
		/// <returns>
		/// DataSet [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],
		/// [GeozoneType],[GeozoneTypeName],[SeverityId],[Description],[Deleted]
		/// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]</returns>
		/// <param name="organizationId"></param> 
		/// <param name="dsGeozonesList"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetOrganizationGeozonesInfo(int organizationId,DataSet dsGeozonesList)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfOrganizationGeozone.GeozoneNo, OrganizationId, GeozoneId, GeozoneName, Type, vlfGeozoneType.GeozoneType,GeozoneTypeName,SeverityId, AlarmLevelName AS SeverityName, Description, Deleted,"+
							"ISNULL(Email,' ') AS Email,"+
                            "ISNULL(Phone,' ') AS Phone," +
							"ISNULL(TimeZone,0) AS TimeZone,"+
							"ISNULL(DayLightSaving,0) AS DayLightSaving,"+
							"ISNULL(FormatType,0) AS FormatType,"+
							"ISNULL(Notify,0) AS Notify,"+
							"ISNULL(Warning,0) AS Warning,"+
							"ISNULL(Critical,0) AS Critical,"+
							"ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving"+
							" FROM vlfOrganizationGeozone INNER JOIN vlfGeozoneType ON vlfOrganizationGeozone.GeozoneType = vlfGeozoneType.GeozoneType"+
                            " INNER JOIN vlfSeverity ON vlfOrganizationGeozone.SeverityId = vlfSeverity.AlarmLevel"+
							" WHERE OrganizationId=" + organizationId;
				if(dsGeozonesList != null && dsGeozonesList.Tables.Count > 0 && dsGeozonesList.Tables[0].Rows.Count > 0)
				{
					sql += " AND (";
					for(int index=0; index< dsGeozonesList.Tables[0].Rows.Count; ++index)
					{
						if(index > 0)
							sql += " OR ";
						sql += "GeozoneId=" + dsGeozonesList.Tables[0].Rows[index]["GeozoneId"].ToString() ;
					}
					sql += ")";
				}
				sql += " ORDER BY GeozoneName";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);

				// In case of list of geozone, prepares custom result
				if(	sqlDataSet != null && sqlDataSet.Tables.Count > 0 && sqlDataSet.Tables[0].Rows.Count > 0 &&
					dsGeozonesList != null && dsGeozonesList.Tables.Count > 0 && dsGeozonesList.Tables[0].Rows.Count > 0)
				{
					object[] objRow = null;
					DataRow[] retResult = null;
					int geozoneId = 0;
					foreach(DataRow ittr in dsGeozonesList.Tables[0].Rows)
					{
						geozoneId = Convert.ToInt32(ittr["GeozoneId"]);
						retResult = sqlDataSet.Tables[0].Select("GeozoneId=" + geozoneId);
						if(retResult == null || retResult.Length == 0)
						{
							objRow = new object[sqlDataSet.Tables[0].Columns.Count] ;
							objRow[0] = -1; //[GeozoneNo]
							objRow[1] = organizationId; //[OrganizationId]
							objRow[2] = geozoneId; //[GeozoneId]
							objRow[3] = VLF.CLS.Def.Const.unknownGeozoneName;//[GeozoneName]
							objRow[4] = 0;	//[Type]
							objRow[5] = 0;	//[GeozoneType]
							objRow[6] = 0;	//[GeozoneTypeName]
							objRow[7] = 0;	//[SeverityId]
							objRow[8]= "";	//[Description]
							objRow[9]= 0;	//[Deleted]
							objRow[10]= "";	//[Email]
							objRow[11]= 0;	//[TimeZone]
							objRow[12]= 0;	//[DayLightSaving]
							objRow[13]= 0;	//[FormatType]
							objRow[14]= 0;	//[Notify]
							objRow[15]= 0;	//[Warning]
							objRow[16]= 0;	//[Critical]
							objRow[17]= 0;	//[AutoAdjustDayLightSaving]
							sqlDataSet.Tables[0].Rows.Add(objRow);
						}
					}
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves geozone info by organization id and geozone Id 
		/// </summary>
		/// <returns>
		/// DataSet [GeozoneNo],[GeozoneId],[Type],[GeozoneType],[SequenceNum],[Latitude],[Longitude],[GeozoneName]</returns>
		/// <param name="organizationId"></param> 
		/// <param name="geozoneId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetOrganizationGeozoneInfo(int organizationId,short geozoneId)
		{
			DataSet sqlDataSet = null;
			try
			{
                // Prepares SQL statement
				string sql = "SELECT vlfOrganizationGeozone.GeozoneNo,vlfOrganizationGeozone.GeozoneId,Type,GeozoneType,SequenceNum,Latitude,Longitude,GeozoneName"+
							" FROM vlfOrganizationGeozone INNER JOIN vlfGeozoneSet ON vlfOrganizationGeozone.GeozoneNo = vlfGeozoneSet.GeozoneNo"+ 
							" WHERE OrganizationId=" + organizationId + " AND GeozoneId=" + geozoneId + " ORDER BY SequenceNum";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + " and geozone Id=" + geozoneId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + " and geozone Id=" + geozoneId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Check if geozone assigned to any of vehicles
		/// </summary>
		/// <returns>true if assigned, otherwise false</returns>
		/// <param name="organizationId"></param> 
		/// <param name="geozoneId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool IsGeozoneAssigned(int organizationId,short geozoneId)
		{
			int geozoneCount = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT count(*) GeozoneNo"+
					" FROM vlfVehicleGeozone INNER JOIN vlfOrganizationGeozone ON vlfVehicleGeozone.GeozoneNo = vlfOrganizationGeozone.GeozoneNo"+
					" WHERE vlfOrganizationGeozone.GeozoneId=" + geozoneId +
					" AND vlfOrganizationGeozone.OrganizationId=" + organizationId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				geozoneCount = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to check geozone " + geozoneId +" assignment. ", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to check geozone " + geozoneId +" assignment. " + objException.Message);
			}
			if(geozoneCount == 0)
				return false;
			else
				return true;
		}
		/// <summary>
		/// Retrieves geozone types
		/// </summary>
		/// <returns>
		/// DataSet [GeozoneType],[GeozoneTypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetGeozoneTypes()
		{
			DataSet sqlDataSet = null;
			try
			{
				// Prepares SQL statement
				string sql = "SELECT * FROM vlfGeozoneType ORDER BY GeozoneTypeName";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve geozone types. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve geozone types. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves geozoneNo by geozone id
		/// </summary>
		/// <returns>Int64</returns>
		/// <param name="organizationId"></param> 
		/// <param name="geozoneId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 GetGeozoneNoByGeozoneId(int organizationId,int geozoneId)
		{
			Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT ISNULL(GeozoneNo," + VLF.CLS.Def.Const.unassignedIntValue + ") AS GeozoneNo FROM vlfOrganizationGeozone WHERE OrganizationId=" + organizationId + " AND GeozoneId=" + geozoneId;
				if(sqlExec.RequiredTransaction())
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
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retrieve geozoneNo by geozone id. " + objException.Message);
			}
			return geozoneNo;
		}





        /// <summary>
        /// Get Organization Equipments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationGeozonesWithStatus(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "GetOrganizationGeozonesWithStatus";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

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





        public DataSet GetOrganizationGeoZone_Public(int userId, int orgId, bool IsPublic)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetOrganizationGeoZOne_Public->Unable to get info for userd  = " + userId.ToString() + ". ";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@OrgId", orgId);
                sqlParams[1] = new SqlParameter("@UserId", userId);
                sqlParams[2] = new SqlParameter("@Public", IsPublic);
                // SQL statement
                string sql = "[GetOrganizationGeoZone_Public]";
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

		#endregion

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
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				object obj = sqlExec.SQLExecuteScalar(sql);
				if(obj != System.DBNull.Value)
					nextGeoZone = Convert.ToInt16(obj);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retrieve next free geozone id. ", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retrieve next free geozone id. " + objException.Message);
			}
			return ++nextGeoZone;
		}
		
		#endregion
	}
}
