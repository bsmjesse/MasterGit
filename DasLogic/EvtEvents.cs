using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VLF.ERR;
using VLF.CLS.Def;
using VLF.DAS.DB;
using System.Data;
using System.Data.SqlClient;
using VLF.CLS;			// for DataSet

namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to events functionality in database
    /// </summary>
    public class EvtEvents : Das
    {
        private VLF.DAS.DB.EvtEvents evtEvents = null;
        #region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
        public EvtEvents(string connectionString) : base(connectionString)
		{
            evtEvents = new VLF.DAS.DB.EvtEvents(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

        #region Public Interfaces
        public DataSet GetAllPretripResult(int organizationId, int userId, DateTime dtFrom, DateTime dtTo)
        {
            return evtEvents.GetAllPretripResult(organizationId, userId, dtFrom, dtTo);
        }

        public DataSet GetAllImpactResult(int organizationId, int userId, DateTime dtFrom, DateTime dtTo)
        {
            return evtEvents.GetAllImpactResult(organizationId, userId, dtFrom, dtTo);
        }

        public DataSet GetEventType()
        {
            return evtEvents.GetEventType();
        }

        public DataSet GetFleets(int userId)
        {
            return evtEvents.GetFleets(userId);
        }

       
        public DataSet GetSelectedEventViolationByOrganization(int organizationId, string table)
        {
            return evtEvents.GetSelectedEventViolationByOrganization(organizationId, table);
        }
        
        public DataSet GetViolationByOrganization(int organizationId, string table)
        {
            return evtEvents.GetViolationByOrganization(organizationId, table);
        }
       
        public int GetTotalRecordsEvent(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId)
        {
            return evtEvents.GetTotalRecordsEvent(events, vehicleId, dtsd, dttd, OrganizationId);
        }

       
        public DataSet GetBoxData(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box, int NoOfDays, int userId, out int totalrecords, string operation)
        {
            return evtEvents.GetBoxData(events, vehicleId, dtsd, dttd, OrganizationId, currentPage, limit, box, NoOfDays, userId, out totalrecords,operation);
        }

        public DataSet GetEvent(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box, int NoOfDays,  int userId ,DataSet BoxDataSet,int RecordsToFetch)
        {
            return evtEvents.GetEvent(events, vehicleId, dtsd, dttd, OrganizationId, currentPage, limit, box, NoOfDays, userId, BoxDataSet, RecordsToFetch);
        }

        public int GetTotalRecordsViolation(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId)
        {
            return evtEvents.GetTotalRecordsViolation(events, vehicleId, dtsd, dttd, OrganizationId);
        }

        public DataSet GetViolation(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box, int NoOfDays, int userId, DataSet BoxDataSet,int RecordsToFetch)
        {
            return evtEvents.GetViolation(events, vehicleId, dtsd, dttd, OrganizationId, currentPage, limit, box, NoOfDays, userId, BoxDataSet,RecordsToFetch);
        }

        public DataSet GetEventViolationColumns(int userId, string colType)
        {
            return evtEvents.GetEventViolationColumns(userId, colType);
        }

     
        public DataSet GetEventViolationRecordsToFetch(int userId, string column)
        {
            return evtEvents.GetEventViolationRecordsToFetch(userId, column);
        }


        public int SetDefaultColumnPreference(int userId, string sSelectedColumns, int preferenceId)
        {
            return evtEvents.SetDefaultColumnPreference(userId, sSelectedColumns, preferenceId);
        }


        

        #endregion
    }
}
