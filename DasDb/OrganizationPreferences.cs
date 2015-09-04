using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to vlfOrganization table.
    /// </summary>
    public class OrganizationPreferences : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public OrganizationPreferences(SQLExecuter sqlExec)
            : base("vlfOrganizationPreferences", sqlExec)
        {
        }

        /// <summary>
        /// Add new organization preference.
        /// </summary>
        public int AddOrganizationPreference(int organizationId, string notificationEmail, int radiusForGps, int maximumReportingInterval, int historyTimerange, int waitingPeriodToGetMessages, int timezone)
        {
            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = string.Format("INSERT INTO " + tableName
                + " (NotificationEmailAddress, OrganizationId, RadiusForGPS, MaximumReportingInterval, HistoryTimeRange, WaitingPeriodToGetMessages, Timezone )"
                + " VALUES ( '{0}', {1}, {2}, {3}, {4}, {5}, {6})",
                notificationEmail,
                organizationId,
                radiusForGps,
                maximumReportingInterval,
                historyTimerange,
                waitingPeriodToGetMessages,
                timezone
                );
            try
            {
                // 2. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This organization preference already exists.");
            }
            return organizationId;
        }




        /// <summary>
        /// Update organization preference.
        /// </summary>
        public int UpdateOrganizationPreference(int organizationId, string notificationEmail, int radiusForGps, int maximumReportingInterval, int historyTimerange, int waitingPeriodToGetMessages, int timezone)
        {
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = string.Format("UPDATE {0} SET NotificationEmailAddress='{1}', RadiusForGPS={2}, MaximumReportingInterval={3}, HistoryTimeRange={4}, WaitingPeriodToGetMessages={5}, Timezone={6} WHERE OrganizationId={7}",
                                        tableName,
                                        notificationEmail,
                                        radiusForGps,
                                        maximumReportingInterval,
                                        historyTimerange,
                                        waitingPeriodToGetMessages,
                                        timezone,
                                        organizationId);

            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update organization preferences for organization id:" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update organization preferences for organization id:" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update organization preferences for organization id:" + organizationId + ".";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
            return rowsAffected;
        }



        /// <summary>
        /// Deletes existing organization preferences.
        /// </summary>

        public int DeleteOrganizationPreferenceByOrganizationId(int organizationId)
        {
            return DeleteRowsByIntField("OrganizationId", organizationId, "organization id");
        }


        /// <summary>
        /// Retrieves Organization info
        /// </summary>

        public DataSet GetOrganizationPreferencesByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT * FROM {0} WHERE OrganizationId={1}", tableName, organizationId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve organization preferences by organizationId:{0}.", organizationId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve organization preferences by organizationId:{0}.", organizationId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }





      /// <summary>
      /// Get Organization Preference Info
      /// </summary>
      /// <param name="organizationId"></param>
      /// <returns></returns>
        public DataSet GetOrganizationSettings(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM vlfOrganizationSettings "+
                    " WHERE OrganizationId=" + organizationId;
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


        /// <summary>
        /// Get Organization Preference Info by Preference
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public string  GetOrganizationSettingsByBoxIdAndPreferenceId(int boxId,Int16 PreferenceId)
        {
            string  PreferenceValue = "";
            try
            {
                //Prepares SQL statement
                object obj = sqlExec.SQLExecuteScalar("SELECT PreferenceValue FROM  dbo.vlfOrganizationSettings INNER JOIN  dbo.vlfBox with (nolock) ON dbo.vlfOrganizationSettings.OrganizationId = dbo.vlfBox.OrganizationId " +
                    " WHERE vlfBox.BoxId=" + boxId + " and OrgPreferenceId=" + PreferenceId);
                if (obj != System.DBNull.Value)
                    PreferenceValue = obj.ToString(); 
                
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by Organization Settings By BoxId=" + boxId + ", PreferenceId=" + PreferenceId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by Organization Settings By BoxId=" + boxId + ", PreferenceId=" + PreferenceId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return PreferenceValue;
        }

        /// <summary>
        /// Delete Organization Settings
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public int DeleteOrganizationSettings(int organizationId)
        {
          
                int rowsAffected = 0;

                //Prepares SQL statement
                string sql = "DELETE FROM vlfOrganizationSettings " + " WHERE OrganizationId=" + organizationId +" and OrgPreferenceId IN (1,2,3,10)";

                try
                {
                    //Executes SQL statement
                    rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
                }
                catch (SqlException objException)
                {
                    string prefixMsg = "Unable to delete organization settings for organization id:" + organizationId + ".";
                    Util.ProcessDbException(prefixMsg, objException);
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception objException)
                {
                    string prefixMsg = "Unable to delete organization settings for organization id:" + organizationId + ".";
                    throw new DASException(prefixMsg + " " + objException.Message);
                }
                if (rowsAffected == 0)
                {
                    string prefixMsg = "Unable to delete organization settings for organization id:" + organizationId + ".";
                    throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
                }
                return rowsAffected;
        }





        /// <summary>
        /// Add/Update organization preference settings
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="preferenceId"></param>
        /// <param name="preferenceValue"></param>
        /// <returns></returns>
        public int OrganizationSettings_Add_Update(int organizationId, int preferenceId, string preferenceValue)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrgId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@PreferenceId", SqlDbType.Int, preferenceId);
                sqlExec.AddCommandParam("@PreferenceValue", SqlDbType.VarChar, preferenceValue);

                rowsAffected = sqlExec.SPExecuteNonQuery("sp_OrganizationSettings_Add_Update");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add/update organization settings organization=" + organizationId + " PreferenceId=" + preferenceId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = prefixMsg = "Unable to add/update organization settings organization=" + organizationId + " PreferenceId=" + preferenceId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }




        public int evtEventSetup_Update(int OrganizationId, int EventTypeID, int Value)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrgId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@EventTypeID", SqlDbType.Int, EventTypeID);
                sqlExec.AddCommandParam("@Value", SqlDbType.Int, Value);

                rowsAffected = sqlExec.SPExecuteNonQuery("evtEventSetup_Update");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add/update organization settings organization=" + OrganizationId + " EventTypeID=" + EventTypeID + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = prefixMsg = "Unable to add/update organization events organization=" + OrganizationId + " EventTypeID=" + EventTypeID + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }



        public DataSet evtEventSetupGet(int OrganizationId, int EventTypeID)
        {
            DataSet sqlDataSet = null;
            string prefixMsg = "Unable to retrieve audit info for organizations. ";
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrgId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@EventTypeID", SqlDbType.Int, EventTypeID);
                
                sqlDataSet = sqlExec.SPExecuteDataset("evtEventSetupGet");
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

        public DataSet GetOrganizationColumnsPreferencesByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                //string sql = "SELECT gridId,STUFF(( SELECT ',' + CONVERT(varchar, columnid) FROM vlfOrganizationColumnsPreference b"+
                //    "WHERE a.gridId= b.gridId"+
                //    "AND Organizationid="+organizationId+
                //    "FOR XML PATH('')), 1, 1, '') as ColumnsList, STUFF(( SELECT ',' + [Active] FROM [vlfOrganizationColumnsPreference] c"+
                //    "where a.gridid = c.gridid"+
                //    "FOR XML PATH('')), 1, 1, '') as ColumnsActiveList from vlfOrganizationColumnsPreference a GROUP BY gridId";
                ////Executes SQL statement
                //sqlDataSet = sqlExec.SQLExecuteDataset(sql);

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationid", SqlDbType.Int, organizationId);


                sqlDataSet = sqlExec.SPExecuteDataset("GetOrganizationColumnsPreference");

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

        
        public DataSet GetOrganizationEventPreferencesByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement              
                string sql = " SELECT distinct EventName, EventTypeID " +
                    " FROM [SentinelFM].[dbo].[vlfOrganizationEventPreference] with (nolock)" +
                    " where [OrganizationId]= '" + organizationId + "'";

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

        
        public DataSet GetOrganizationViolationPreferencesByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement              
                string sql = " SELECT distinct ViolationName, EventTypeID " +
                    " FROM [SentinelFM].[dbo].[vlfOrganizationViolationPreference] with (nolock)" +
                    " where [OrganizationId]= '" + organizationId + "'";

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

       

        public DataSet GetOrganizationSensorPreferencesByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationid", SqlDbType.Int, organizationId);

                sqlDataSet = sqlExec.SPExecuteDataset("GetOrganizationSensorPreference");

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

        public DataSet GetOrganizationSensorByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationid", SqlDbType.Int, organizationId);

                sqlDataSet = sqlExec.SPExecuteDataset("GetOrganizationSensor");

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

        public DataSet GetOrganizationPreferenceByOrganizationIdAndUserId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
               
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);

                sqlDataSet = sqlExec.SPExecuteDataset("UserPreference_Get");

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

        
        public int AddOrganizationViolationPreference(int organizationId, string violationname, int violationid)
        {
            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = string.Format("INSERT INTO vlfOrganizationViolationPreference"
                + " (OrganizationId,EventTypeID, ViolationName)"
                + " VALUES ( {0}, {1}, '{2}')", organizationId,violationid, violationname);
            try
            {
                // 2. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This organization preference already exists.");
            }
            return organizationId;
        }
        
        public int AddOrganizationEventPreference(int organizationId, string eventname, int eventid)
        {
            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = string.Format("INSERT INTO vlfOrganizationEventPreference"
                + " (OrganizationId,EventTypeID, EventName)"
                + " VALUES ( {0}, {1}, '{2}')", organizationId, eventid, eventname);
            try
            {
                // 2. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This organization preference already exists.");
            }
            return organizationId;
        }
        
        public int AddOrganizationSensorPreferene(int organizationId, string sensorname)
        {
            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = string.Format("INSERT INTO vlfOrganizationSensorPreference"
                + " (OrganizationId, SensorName)"
                + " VALUES ( {0}, '{1}')", organizationId, sensorname);
            try
            {
                // 2. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new organization preference.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This organization preference already exists.");
            }
            return organizationId;
        }

        
        public int DeleteOrganizationViolationPreference(int organizationId, string violationname,int violationid)
        {
            int rowsAffected = 0;

            //Prepares SQL statement
            string sql = "DELETE FROM vlfOrganizationViolationPreference " +
                " WHERE OrganizationId=" + organizationId + " and EventTypeID='" + violationid + "'";

            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete organization Violation Preference for organization id:" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete organization Violation Preference for organization id:" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to delete organization Violation Preference for organization id:" + organizationId + ".";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
            return rowsAffected;
        }
        
        public int DeleteOrganizationEventPreference(int organizationId, string eventname, int eventid)
        {
            int rowsAffected = 0;

            //Prepares SQL statement
            string sql = "DELETE FROM vlfOrganizationEventPreference " +
                " WHERE OrganizationId=" + organizationId + " and EventTypeID='" + eventid + "'";

            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete organization Event Preference for organization id:" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete organization Event Preference for organization id:" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to delete organization Event Preference for organization id:" + organizationId + ".";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
            return rowsAffected;
        }
        
        public int DeleteOrganizationSensorPreferene(int organizationId, string sensorname)
        {
            int rowsAffected = 0;

            //Prepares SQL statement
            string sql = "DELETE FROM vlfOrganizationSensorPreference " +
                " WHERE OrganizationId=" + organizationId + " and SensorName='" + sensorname+"'";

            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete organization Sensor Preference for organization id:" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete organization Sensor Preference for organization id:" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to delete organization Sensor Preference for organization id:" + organizationId + ".";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
            return rowsAffected;
        }
    }
}
