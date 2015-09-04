using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{
    public class WorkingHours: TblGenInterfaces
    {
        public WorkingHours(SQLExecuter sqlExec)
            : base("MCCGroup", sqlExec)
      {
      }
        public int FleetWorkingHours_add(int FleetID, string Email, string WorkingHrs, string Exeption, int OrganizationId, int WorkingHoursRangeId, int Timezone, Int64 UID)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "FleetWorkingHours_add";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@FleetID", SqlDbType.Int, FleetID);
                sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, Email);
                sqlExec.AddCommandParam("@WorkingHrs", SqlDbType.VarChar, WorkingHrs, -1);
                sqlExec.AddCommandParam("@Exeption", SqlDbType.VarChar, Exeption, -1);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@WorkingHoursRangeId", SqlDbType.Int, WorkingHoursRangeId);
                sqlExec.AddCommandParam("@Timezone", SqlDbType.Int, Timezone);
                sqlExec.AddCommandParam("@UID", SqlDbType.BigInt, UID);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add FleetWorkingHours to OrganizationId:" + OrganizationId + " FleetID:" + FleetID.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add FleetWorkingHours to OrganizationId:" + OrganizationId + " FleetID:" + FleetID.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        //public int CheckWorkingHoursUTC(ArrayList FromTime, ArrayList ToTime, ArrayList WorkingHrs, int OrganizationId)
        //{
        //    if (FromTime.Count <= 0) return -1;
        //    DataSet sqlDataSet = null;
        //    try
        //    {
        //        string sql = string.Format("Select wh0.WorkingHoursRangeId from WorkingHoursUTC wh0");

        //        int index = 0;
        //        for (index = 1; index < FromTime.Count; index++)
        //        {
        //            sql = string.Format("{0} inner join WorkingHoursUTC {1} on {1}.WorkingHoursRangeId = wh0.WorkingHoursRangeId and " +
        //                    "{1}.[From]={2} and {1}.[To]={3} and {1}.WorkingDays='{4}' and {1}.OrganizationId={5} ", sql, "wh" + index.ToString(),
        //                    FromTime[index], ToTime[index], WorkingHrs[index], OrganizationId);
        //        }
        //        string sql1 = string.Format(" and {0}= (Select count(WorkingHoursRangeId) from WorkingHoursUTC whc where whc.OrganizationId={1} and whc.WorkingHoursRangeId = wh0.WorkingHoursRangeId)",
        //            FromTime.Count, OrganizationId);
        //        sql = sql + string.Format(" where wh0.[From]={0} and wh0.[To]={1} and wh0.WorkingDays='{2}' and wh0.OrganizationId={3} and wh0.WorkingHoursRangeId != 1000",
        //              FromTime[0], ToTime[0], WorkingHrs[0], OrganizationId) + sql1;
        //        //sql = string.Format("if exists({0}) select 1 else select 0;", sql);
        //        sqlDataSet = sqlExec.SQLExecuteDataset(sql);
        //    }
        //    catch (SqlException objException)
        //    {
        //        string prefixMsg = "Unable to retrieve CheckWorkingHoursUTC By OrganizationId =" + OrganizationId.ToString();
        //        Util.ProcessDbException(prefixMsg, objException);
        //    }
        //    catch (DASDbConnectionClosed exCnn)
        //    {
        //        throw new DASDbConnectionClosed(exCnn.Message);
        //    }
        //    catch (Exception objException)
        //    {
        //        string prefixMsg = "Unable to retrieve CheckWorkingHoursUTC By OrganizationId =" + OrganizationId.ToString();

        //        throw new DASException(prefixMsg + " " + objException.Message);
        //    }
        //    int ret = -1;
        //    if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
        //    {
        //        if (sqlDataSet.Tables[0].Rows[0]["WorkingHoursRangeId"] != DBNull.Value)
        //        {
        //            int.TryParse(sqlDataSet.Tables[0].Rows[0]["WorkingHoursRangeId"].ToString(), out ret);
        //        }
        //    }
        //    return ret;
        //}

        public DataSet GetWorkingHoursUTCByFleetId(string FleetId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Format("Select FleetWorkingHoursID as ID, Email, TimeZone from FleetWorkingHours where fleetId = {0} ;", FleetId);
                sql = sql + string.Format("Select distinct WorkingHours.* from WorkingHours inner join FleetWorkingHours " +
                              " on WorkingHours.WorkingHoursRangeId = FleetWorkingHours.WorkingHoursRangeId and FleetWorkingHours.fleetId = {0} and WorkingHours.fleetId = {0} and Inverse = 1", FleetId);
                sql = sql + string.Format("Select a.VehicleId from VehicleNotification a " +
                              "where a.FleetId = {0} and a.AfterHoursSuppressed = 1", FleetId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetWorkingHoursByUTC By FleetId =" + FleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetWorkingHoursByUTC By FleetId =" + FleetId.ToString();

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet FleetWorkingHours_Report(int FleetId, DateTime MinDateTime)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "FleetWorkingHours_Report";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);
                sqlExec.AddCommandParam("@MinDateTime", SqlDbType.DateTime, MinDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Retrive FleetWorkingHours_ReportFleet. FleetID:" + FleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Retrive FleetWorkingHours_ReportFleet. FleetID:" + FleetId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
    }
}
