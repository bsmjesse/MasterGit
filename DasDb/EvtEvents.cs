using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;	// for SqlException
using System.Data;			// for DataSet
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;






namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interface to evtEvents table
    /// </summary>
    public class EvtEvents : TblGenInterfaces
    {
        #region Public Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
        public EvtEvents(SQLExecuter sqlExec) : base("evtEvents", sqlExec)
		{
        }

        public DataSet GetAllPretripResult(int organizationId, int userId, DateTime dtFrom, DateTime dtTo)
        {
            DataSet dt = new DataSet();
            try
            {
                
                string sql = "sp_GetPretrip";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, dtFrom);
                sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, dtTo);

                dt = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve pretrip from=" + dtFrom + " to=" + dtTo + " for organizationId=" + organizationId.ToString() + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve pretrip from=" + dtFrom + " to=" + dtTo + " for organizationId=" + organizationId.ToString() + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dt;
        }

        public DataSet GetAllImpactResult(int organizationId, int userId, DateTime dtFrom, DateTime dtTo)
        {
            DataSet dt = new DataSet();
            try
            {

                string sql = "sp_GetImpact";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, dtFrom);
                sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, dtTo);

                dt = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve impact from=" + dtFrom + " to=" + dtTo + " for organizationId=" + organizationId.ToString() + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve impact from=" + dtFrom + " to=" + dtTo + " for organizationId=" + organizationId.ToString() + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dt;
        }

        public DataSet GetEventType()
        {
            DataSet sqlDataSet = null;
            try
            {
                    sqlDataSet = sqlExec.SQLExecuteDataset("SELECT EventTypeID,Description FROM evtEventType");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve EventType. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve EventType. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetFleets(int userId)
        {
            DataSet dt = new DataSet();
            try
            {
                string sql = "SELECT DISTINCT OrganizationName,vlfFleet.FleetId,FleetName,vlfFleet.Description,ISNULL(FleetType,'') as FleetType,ISNULL(NodeCode,'') as NodeCode " +
                   " FROM  vlfFleetUsers,vlfOrganization,vlfFleet" +
                   " WHERE ( vlfFleetUsers.UserId=" + userId + ")" +
                   " AND (vlfFleet.FleetId=vlfFleetUsers.FleetId)" +
                   " AND (vlfFleet.OrganizationId=vlfOrganization.OrganizationId)" +
                   " ORDER BY FleetName";
                //Executes SQL statement
                dt = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Fleets";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Fleets";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dt;
        }

       
        public DataSet GetSelectedEventViolationByOrganization(int organizationId, string table)
        {
            DataSet dt = new DataSet();
            try
            {

                string sql = "GetSelectedEventViolationListByOrganization";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@Table", SqlDbType.VarChar, table);


                dt = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Violation";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Violation";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dt;
        }
       

        public DataSet GetViolationByOrganization(int organizationId, string table)
        {
            DataSet dt = new DataSet();
            try
            {

                string sql = "GetViolationListByOrganization";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@Table", SqlDbType.VarChar, table);
                

                dt = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Violation";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Violation";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dt;
        }

        
        public DataSet GetBoxData(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box, int NoOfDays, int userId, out int totalrecords,string operation)
        {
            totalrecords = 0;           
            DataSet dtBox = new DataSet();
            try
            {
                if (box == 1)
                {
                    string BoxDataSQL = "GetFilteredBoxData";
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, OrganizationId);
                    sqlExec.AddCommandParam("@eventId", SqlDbType.VarChar, events);
                    sqlExec.AddCommandParam("@vehicleId", SqlDbType.VarChar, vehicleId);
                    sqlExec.AddCommandParam("@stDateFrom", SqlDbType.DateTime, dtsd);
                    sqlExec.AddCommandParam("@stDateTo", SqlDbType.DateTime, dttd);
                    sqlExec.AddCommandParam("@NoOfDays", SqlDbType.Int, NoOfDays);
                    sqlExec.AddCommandParam("@operation", SqlDbType.VarChar, operation);
                    dtBox = sqlExec.SPExecuteDataset(BoxDataSQL);
                   
                }
                if (dtBox.Tables.Count > 0)
                    totalrecords = dtBox.Tables[0].Rows.Count;


            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                Util.ProcessDbException(prefixMsg, objException);

            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dtBox;
        }

        public DataSet GetEvent(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box, int NoOfDays, int userId, DataSet BoxDataSet, int RecordsToFetch)
        {
           //totalrecords = 0;
            DataSet dt = new DataSet();
           
            try
            {
                //string countSQL = "GetEventCount";
                //sqlExec.ClearCommandParameters();
                //sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, OrganizationId);
                //sqlExec.AddCommandParam("@eventId", SqlDbType.VarChar, events);
                //sqlExec.AddCommandParam("@vehicleId", SqlDbType.VarChar, vehicleId);
                //sqlExec.AddCommandParam("@stDateFrom", SqlDbType.DateTime, dtsd);
                //sqlExec.AddCommandParam("@stDateTo", SqlDbType.DateTime, dttd);
                //totalrecords = (int)sqlExec.SPExecuteScalar(countSQL);
               string[] eventarr = events.Split(',');
                if (!(box == 1 && eventarr.Length <= 3))
                {
                    string sql = "GetFilteredEvent";
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, OrganizationId);
                    sqlExec.AddCommandParam("@eventId", SqlDbType.VarChar, events);
                    sqlExec.AddCommandParam("@vehicleId", SqlDbType.VarChar, vehicleId);
                    sqlExec.AddCommandParam("@stDateFrom", SqlDbType.DateTime, dtsd);
                    sqlExec.AddCommandParam("@stDateTo", SqlDbType.DateTime, dttd);
                    sqlExec.AddCommandParam("@currentPage", SqlDbType.Int, currentPage);
                    sqlExec.AddCommandParam("@pageLimit", SqlDbType.Int, limit);
                    sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                    sqlExec.AddCommandParam("@recordsToFetch", SqlDbType.Int, RecordsToFetch);
                    dt = sqlExec.SPExecuteDataset(sql);

                    if (BoxDataSet != null && BoxDataSet.Tables.Count > 0 && BoxDataSet.Tables[0].Rows.Count > 0)
                    {
                        if (dt.Tables[0].Rows.Count > 0)
                        {
                            dt.Tables[0].Merge(BoxDataSet.Tables[0]);
                        }
                        else
                        {
                            dt = BoxDataSet;
                        }
                    }
                }
                else
                {
                    dt = BoxDataSet;
                }
               
               
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                Util.ProcessDbException(prefixMsg, objException);
               
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dt;
        }

        public DataSet GetViolation(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box, int NoOfDays, int userId, DataSet BoxDataSet ,int RecordsToFetch)
        {
            
            DataSet dt = new DataSet();            
          
            try
            {
                
                //string countSQL = "GetViolationCount";
                //sqlExec.ClearCommandParameters();
                //sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, OrganizationId);
                //sqlExec.AddCommandParam("@eventId", SqlDbType.VarChar, events);
                //sqlExec.AddCommandParam("@vehicleId", SqlDbType.VarChar, vehicleId);
                //sqlExec.AddCommandParam("@stDateFrom", SqlDbType.DateTime, dtsd);
                //sqlExec.AddCommandParam("@stDateTo", SqlDbType.DateTime, dttd);
                
                //totalrecords = (int)sqlExec.SPExecuteScalar(countSQL);
                 string[] eventarr = events.Split(',');
                 if (!(box == 1 && eventarr.Length <= 3))
                 {
                    string sql = "GetFilteredViolation";
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, OrganizationId);
                    sqlExec.AddCommandParam("@eventId", SqlDbType.VarChar, events);
                    sqlExec.AddCommandParam("@vehicleId", SqlDbType.VarChar, vehicleId);
                    sqlExec.AddCommandParam("@stDateFrom", SqlDbType.DateTime, dtsd);
                    sqlExec.AddCommandParam("@stDateTo", SqlDbType.DateTime, dttd);
                    sqlExec.AddCommandParam("@currentPage", SqlDbType.Int, currentPage);
                    sqlExec.AddCommandParam("@pageLimit", SqlDbType.Int, limit);
                    sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                    sqlExec.AddCommandParam("@recordsToFetch", SqlDbType.Int, RecordsToFetch);
                    dt = sqlExec.SPExecuteDataset(sql);

                 if (BoxDataSet != null && BoxDataSet.Tables.Count > 0 && BoxDataSet.Tables[0].Rows.Count > 0)
                 {
                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        dt.Tables[0].Merge(BoxDataSet.Tables[0]);
                    }
                    else
                    {
                        dt = BoxDataSet;
                    }
                     }
                 }
                 else
                 {
                     dt = BoxDataSet;
                 }
                 
           
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return dt;
        }

        public DataSet GetEventViolationColumns(int userId, string colType)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfPreference.PreferenceId,PreferenceName,PreferenceRule" +
                    " FROM vlfPreference" +
                    " WHERE vlfPreference.PreferenceName= '" + colType + "'";
 
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        public int SetDefaultColumnPreference(int userId, string selectedColumns, int preferenceId)
          {
            int rowsAffected = 0;
            int Count = 0;
            string sql = "";
            try
            {
                string sqlQuery = "SELECT COUNT(*) from vlfUserPreference where userId=" + userId + "and PreferenceId=" + preferenceId;
                //Executes SQL statement
                Count = (int)sqlExec.SQLExecuteScalar(sqlQuery);
                if (Count > 0)
                {
                    //Prepares SQL statement
                     sql = "UPDATE vlfUserPreference SET PreferenceValue=" + "'" + selectedColumns + "'" + " where UserId=" + userId + "and PreferenceId=" + preferenceId;
                }
                else
                {
                    //Prepares SQL statement
                    sql = "insert into vlfUserPreference (UserId,PreferenceId,PreferenceValue) values (" + userId + "," + preferenceId + "," + "'" + selectedColumns + "'" + ")";
                }
                

                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }


        
        public DataSet GetEventViolationRecordsToFetch(int userId, string column)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfPreference.PreferenceId,PreferenceName,PreferenceRule" +
                    " FROM vlfPreference" +
                    " WHERE vlfPreference.PreferenceName= '" + column + "'";

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

       

        public int GetTotalRecordsEvent(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId)
        {
            int totalrecords = 0;
            
            try
            {
               

                string countSQL = "GetEventCount";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@eventId", SqlDbType.VarChar, events);
                sqlExec.AddCommandParam("@vehicleId", SqlDbType.VarChar, vehicleId);
                sqlExec.AddCommandParam("@stDateFrom", SqlDbType.DateTime, dtsd);
                sqlExec.AddCommandParam("@stDateTo", SqlDbType.DateTime, dttd);
                totalrecords = (int)sqlExec.SPExecuteScalar(countSQL);

               
            }
            catch (SqlException objException)
            {
                //string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                //Util.ProcessDbException(prefixMsg, objException);

            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                //string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                //throw new DASException(prefixMsg + " " + objException.Message);
            }

            return totalrecords;
        }

        public int GetTotalRecordsViolation(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId)
        {
           int totalrecords = 0;
           
            try
            {
               

                string countSQL = "GetViolationCount";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@eventId", SqlDbType.VarChar, events);
                sqlExec.AddCommandParam("@vehicleId", SqlDbType.VarChar, vehicleId);
                sqlExec.AddCommandParam("@stDateFrom", SqlDbType.DateTime, dtsd);
                sqlExec.AddCommandParam("@stDateTo", SqlDbType.DateTime, dttd);

                totalrecords = (int)sqlExec.SPExecuteScalar(countSQL);

                

            }
            catch (SqlException objException)
            {
                //string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                //Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                //string prefixMsg = "Unable to retrieve Event from=" + dtsd + " to=" + dttd + " for Organization" + OrganizationId + ". Connection string : " + sqlExec.ConnectionString;
                //throw new DASException(prefixMsg + " " + objException.Message);
            }

            return totalrecords;
        }
        #endregion

        #region Protected Interfaces
        #endregion
    }
}
