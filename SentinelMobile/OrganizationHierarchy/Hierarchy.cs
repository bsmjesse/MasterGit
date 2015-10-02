using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VLF.DAS;

namespace OrganizationHierarchy
{
    public class Hierarchy : IDisposable
    {
        protected SQLExecuter sqlExec;

        public Hierarchy(string sconnectionstring)
        {
            this.sqlExec = new SQLExecuter(sconnectionstring);            
        }

        public void Dispose()
        {
            if (null != this.sqlExec)
                this.sqlExec.Dispose();
        }

        public int GetFleetIdByNodeCode(string nodecode, int organizationId)
        {
            int fleetId = 0;
            try
            {
                string sql = string.Format("SELECT fleetId FROM vlfFleet WHERE OrganizationId={0} AND NodeCode='{1}' AND FleetType='oh'", organizationId, nodecode);
                DataSet ds = sqlExec.SQLExecuteDataset(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    fleetId = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                }
            }
            catch { }
            return fleetId;
        }

        public DataSet SearchOrganizationHierarchy(int organizationId, string s)
        {
            string sql = "sp_SearchOrganizationHierarchy";
            
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@SearchString", SqlDbType.VarChar, s);

            return sqlExec.SPExecuteDataset(sql);
        }

        public string GetFleetNameByFleetId(int organizationId, int fleetId)
        {
            string fleetName = string.Empty;
            try
            {
                string sql = "SELECT FleetName FROM vlfFleet WHERE OrganizationId=" + organizationId.ToString() + " AND FleetId=" + fleetId.ToString();
                DataSet ds = sqlExec.SQLExecuteDataset(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    fleetName = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch { }
            return fleetName;

        }

        public int swapVehicle(Int64 vehicleId)
        {
            int rowsAffected = 0;

            try
            {
                string sql = "sp_swapBox";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@vehicleId", SqlDbType.Int, vehicleId);

                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }            
            catch
            {
                rowsAffected = 0;
            }

            return rowsAffected;
        }
    }
}
