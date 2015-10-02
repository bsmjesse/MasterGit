using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.DAS.DB;

namespace VLF.DAS.Logic
{
    /// <summary>
    /// Organization Products Interface
    /// </summary>
    public partial class Organization
    {

        /// <summary>
        /// Add a new preferences to organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="notificationEmail"></param>
        /// <param name="radiusForGps"></param>
        /// <param name="maximumReportingInterval"></param>
        /// <param name="historyTimerange"></param>
        /// <param name="waitingPeriodToGetMessages"></param>
        /// <param name="timezone"></param>
        /// <returns>records affected counts</returns>
        public int AddPreference(int organizationId, string notificationEmail, int radiusForGps, int maximumReportingInterval, int historyTimerange, int waitingPeriodToGetMessages, int timezone)
        {
            return preferences.AddOrganizationPreference(organizationId, notificationEmail, radiusForGps, maximumReportingInterval, historyTimerange, waitingPeriodToGetMessages, timezone);
        }

        /// <summary>
        /// updates organization preferences
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="notificationEmail"></param>
        /// <param name="radiusForGps"></param>
        /// <param name="maximumReportingInterval"></param>
        /// <param name="historyTimerange"></param>
        /// <param name="waitingPeriodToGetMessages"></param>
        /// <param name="timezone"></param>
        /// <returns>records affected counts</returns>

        public int UpdatePreference(int organizationId, string notificationEmail, int radiusForGps, int maximumReportingInterval, int historyTimerange, int waitingPeriodToGetMessages, int timezone)
        {
            return preferences.UpdateOrganizationPreference(organizationId, notificationEmail, radiusForGps, maximumReportingInterval, historyTimerange, waitingPeriodToGetMessages, timezone);
        }

        /// <summary>
        /// Get Organization Preferences
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationPreferences(int organizationId)
        {
            return preferences.GetOrganizationPreferencesByOrganizationId(organizationId);
        }



        public int UpdateOrganizationSettings(int organizationId, int preferenceId, string preferenceValue)
        {
            return preferences.OrganizationSettings_Add_Update(organizationId, preferenceId, preferenceValue);
        }

        public int evtEventSetup_Update(int OrganizationId, int EventTypeID, int Value)
        {
            return preferences.evtEventSetup_Update(OrganizationId, EventTypeID, Value);
        }

        public DataSet evtEventSetupGet(int OrganizationId, int EventTypeID)
        {
            return preferences.evtEventSetupGet(OrganizationId, EventTypeID);
        }


        /// <summary>
        /// Get Organization Preferences
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationSettings(int organizationId)
        {
            return preferences.GetOrganizationSettings(organizationId);
        }

        /// <summary>
        /// Get Organization Settings By Preferences
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public string GetOrganizationSettingsByBoxIdAndPreferenceId(int boxId, Int16 PreferenceId)
        {
            return preferences.GetOrganizationSettingsByBoxIdAndPreferenceId(boxId, PreferenceId);
        }
        /// <summary>
        /// Delete Organization Settings - Restoring the Defaults
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public int DeleteOrganizationSettings(int organizationId)
        {
            return preferences.DeleteOrganizationSettings(organizationId);
        }

        /// <summary>
        /// Get Organization Columns Preferences
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationColumnsPreferences(int organizationId)
        {
            return preferences.GetOrganizationColumnsPreferencesByOrganizationId(organizationId);
        }

       
        public DataSet GetOrganizationEventPreferences(int organizationId)
        {
            return preferences.GetOrganizationEventPreferencesByOrganizationId(organizationId);
        }
        
        public DataSet GetOrganizationViolationPreferences(int organizationId)
        {
            return preferences.GetOrganizationViolationPreferencesByOrganizationId(organizationId);
        }
       

        public DataSet GetOrganizationSensorPreferences(int organizationId)
        {
            return preferences.GetOrganizationSensorPreferencesByOrganizationId(organizationId);
        }

        public DataSet GetOrganizationSensor(int organizationId)
        {
            return preferences.GetOrganizationSensorByOrganizationId(organizationId);
        }

        public DataSet GetOrganizationPreferenceByOrganizationIdAndUserId(int userid, int organizationId)
        {
            return preferences.GetOrganizationPreferenceByOrganizationIdAndUserId( organizationId);
        }
        
        public int AddOrganizationViolationPreference(int organizationId, string violationname, int violationid)
        {
            return preferences.AddOrganizationViolationPreference(organizationId, violationname, violationid);
        }
        

        
        public int AddOrganizationEventPreference(int organizationId, string eventname, int eventid)
        {
            return preferences.AddOrganizationEventPreference(organizationId, eventname, eventid);
        }
        

        public int AddOrganizationSensorPreferene(int organizationId, string sensorname)
        {
            return preferences.AddOrganizationSensorPreferene(organizationId, sensorname);
        }

        
        public int DeleteOrganizationViolationPreference(int organizationId, string violationname, int violationid)
        {
            return preferences.DeleteOrganizationViolationPreference(organizationId, violationname, violationid);
        }
        
        public int DeleteOrganizationEventPreference(int organizationId, string eventname, int eventid)
        {
            return preferences.DeleteOrganizationEventPreference(organizationId, eventname, eventid);
        }
       
        public int DeleteOrganizationSensorPreferene(int organizationId, string sensorname)
        {
            return preferences.DeleteOrganizationSensorPreferene(organizationId, sensorname);
        }

    }
}
