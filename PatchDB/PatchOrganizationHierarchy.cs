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
    public class PatchOrganizationHierarchy : TblGenInterfaces
    {
        /// <summary>
        /// Provides interfaces to vlfMapLayers table.
        /// </summary>
        public PatchOrganizationHierarchy(SQLExecuter sqlExec)
            : base("vlfOrganizationHierarchy", sqlExec)
        {
        }

        public int deleteOrganizationHierarchy(int organizationId)
        {
            string sql = "DELETE FROM vlfOrganizationHierarchy WHERE OrganizationId=" + organizationId.ToString();
            return sqlExec.SQLExecuteNonQuery(sql);
            
        }

        public int deleteOrganizationHierarchyFleet(int organizationId)
        {
            string sql = "DELETE FROM vlfFleetVehicles WHERE FleetId IN (SELECT FleetID FROM vlfFleet WHERE OrganizationId=" + organizationId.ToString() + " AND FleetType='oh')";
            sqlExec.SQLExecuteNonQuery(sql);

            sql = "DELETE FROM vlfFleetUsers WHERE FleetId IN (SELECT FleetID FROM vlfFleet WHERE OrganizationId=" + organizationId.ToString() + " AND FleetType='oh')";
            sqlExec.SQLExecuteNonQuery(sql);
            
            sql = "DELETE FROM vlfFleet WHERE OrganizationId=" + organizationId.ToString() + " AND FleetType='oh'";
            //sql = "UPDATE vlfFleet SET OrganizationId = 0 -  OrganizationId WHERE OrganizationId=" + organizationId.ToString() + " AND FleetType='oh'";
            return sqlExec.SQLExecuteNonQuery(sql);
        }

        public int deleteOrganizationHierarchyFleetVehicles(int organizationId)
        {
            string sql = "DELETE FROM vlfFleetVehicles WHERE FleetId IN ( SELECT FleetId FROM vlfFleet WHERE OrganizationId=" + organizationId.ToString() + " AND FleetType='oh')";
            return sqlExec.SQLExecuteNonQuery(sql);
        }

        public DataSet GetOrganizationHierarchyByNodecode(int organizationId, string nodecode)
        {
            string sql = "SELECT * FROM vlfOrganizationHierarchy WITH(NOLOCK) WHERE OrganizationId=" + organizationId.ToString() + " AND NodeCode='" + nodecode + "'";
            return sqlExec.SQLExecuteDataset(sql);
        }

        public DataSet SetOrganizationHierarchyParentIdMinus(int organizationId, string nodecode)
        {
            string sql = "UPDATE vlfOrganizationHierarchy SET ParentId = -1 WHERE OrganizationId=" + organizationId.ToString() + " AND NodeCode='" + nodecode + "'";
            return sqlExec.SQLExecuteDataset(sql);
        }

        
        public int SaveOrganizationHierarchy(int organizationId, DataRow row)
        {
            string sql = "OrganizationHierarchyAdd";
            sqlExec.ClearCommandParameters();
            
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, row["OrganizationId"]);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, row["NodeCode"]);
            sqlExec.AddCommandParam("@NodeName", SqlDbType.VarChar, row["Nodename"]);
            sqlExec.AddCommandParam("@IsParent", SqlDbType.Bit, row["IsParent"]);
            sqlExec.AddCommandParam("@ParentNodeCode", SqlDbType.VarChar, row["ParentNodeCode"] == row["NodeCode"] ? null : row["ParentNodeCode"]);
            sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, row["Description"]);
            
            if (sqlExec.RequiredTransaction())
            {
                // 4. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }
            // 5. Executes SQL statement
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public int UpdateOrganizationHierarchyParent(int organizationId, DataRow row)
        {
            string sql = "OrganizationHierarchyUpdateParent";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, row["OrganizationId"]);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, row["NodeCode"]);
            sqlExec.AddCommandParam("@ParentNodeCode", SqlDbType.VarChar, row["ParentNodeCode"] == row["NodeCode"] ? null : row["ParentNodeCode"]);            

            if (sqlExec.RequiredTransaction())
            {
                // 4. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }
            // 5. Executes SQL statement
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public int UpdateOrganizationHierarchy(int organizationId, DataRow row)
        {
            string sql = "OrganizationHierarchyUpdate";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, row["OrganizationId"]);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, row["NodeCode"]);
            sqlExec.AddCommandParam("@ParentNodeCode", SqlDbType.VarChar, row["ParentNodeCode"] == row["NodeCode"] ? null : row["ParentNodeCode"]);
            sqlExec.AddCommandParam("@NodeName", SqlDbType.VarChar, row["NodeName"]);

            if (sqlExec.RequiredTransaction())
            {
                // 4. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }
            // 5. Executes SQL statement
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public int UpdateOrganizationHierarchyFlat(int organizationId)
        {
            string sql = "UpdateOrganizationHierarchyFlat";
            sqlExec.ClearCommandParameters();
            sqlExec.CommandTimeout = 36000;
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);
            

            if (sqlExec.RequiredTransaction())
            {
                // 4. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }
            // 5. Executes SQL statement
            return sqlExec.SPExecuteNonQuery(sql);

        }

        public int BatchSetOrganizationHierarchyAssignmentExpireDateTime(int organizationId, DateTime exd)
        {
            string sql = "OrganizationHierarchyAssignmentBatchSetExpireDateTime";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@expdt", SqlDbType.DateTime, exd);
            

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public int SaveOrganizationHierarchyAssignment(int organizationId, DataRow row, string objectTabelName, DateTime effectiveFrom)
        {
            string firstName = row["FirstName"].ToString().Replace("\'","\'\'");
            string lastName = row["LastName"].ToString().Replace("\'", "\'\'");
             
            string sql = "SELECT DriverId FROM vlfDriver WITH(NOLOCK) WHERE OrganizationId=" + organizationId.ToString() + " AND FirstName='" + firstName + "' AND LastName='" + lastName + "'";
            DataSet ds = sqlExec.SQLExecuteDataset(sql);
            if (ds.Tables[0].Rows.Count == 0)
                return 0;
            int driverID = int.Parse(ds.Tables[0].Rows[0][0].ToString());

            sql = "OrganizationHierarchyAssignmentAdd";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, row["NodeCode"]);
            sqlExec.AddCommandParam("@ObjectTableName", SqlDbType.VarChar, objectTabelName);
            sqlExec.AddCommandParam("@ObjectId", SqlDbType.VarChar, driverID.ToString());
            sqlExec.AddCommandParam("@EffectiveFrom", SqlDbType.VarChar, effectiveFrom);
            

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteNonQuery(sql);

        }

        public int SaveOrganizationHierarchyAssignmentByVehicleDescription(int organizationId, DataRow row, string objectTabelName, DateTime effectiveFrom)
        {
            return SaveOrganizationHierarchyAssignmentByVehicleDescription(organizationId, row, objectTabelName, effectiveFrom, false, false, 0);
        }

        public int SaveOrganizationHierarchyAssignmentByVehicleDescription(int organizationId, DataRow row, string objectTabelName, DateTime effectiveFrom, bool DriverAssignment)
        {
            return SaveOrganizationHierarchyAssignmentByVehicleDescription(organizationId, row, objectTabelName, effectiveFrom, DriverAssignment, false, 0);

        }
        
        public int SaveOrganizationHierarchyAssignmentByVehicleDescription(int organizationId, DataRow row, string objectTabelName, DateTime effectiveFrom, bool DriverAssignment, bool FieldColumns, int userId)
        {
            try
            {
                string sql = "OrganizationHierarchyAssignmentAdd";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, row["NodeCode"]);
                sqlExec.AddCommandParam("@ObjectTableName", SqlDbType.VarChar, objectTabelName);
                sqlExec.AddCommandParam("@ObjectId", SqlDbType.VarChar, row["ObjectID"]);
                sqlExec.AddCommandParam("@EffectiveFrom", SqlDbType.VarChar, effectiveFrom);

                if (DriverAssignment)
                {
                    sqlExec.AddCommandParam("@DriverFirstName", SqlDbType.VarChar, row["DriverFirstName"]);
                    sqlExec.AddCommandParam("@DriverLastName", SqlDbType.VarChar, row["DriverLastName"]);
                }
                else
                {
                    sqlExec.AddCommandParam("@DriverFirstName", SqlDbType.VarChar, "");
                    sqlExec.AddCommandParam("@DriverLastName", SqlDbType.VarChar, "");
                }

                sqlExec.AddCommandParam("@TeamLeaderName", SqlDbType.VarChar, row["TeamLeaderName"] == null ? "" : row["TeamLeaderName"]);

                if (FieldColumns)
                {
                    sqlExec.AddCommandParam("@Equipment", SqlDbType.VarChar, row["Equipment"]);
                    //sqlExec.AddCommandParam("@TeamLeaderName", SqlDbType.VarChar, row["TeamLeaderName"]);
                    sqlExec.AddCommandParam("@ListName", SqlDbType.VarChar, row["ListName"]);
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                    sqlExec.AddCommandParam("@LicensePlate", SqlDbType.VarChar, row["LicensePlate"]);
                    sqlExec.AddCommandParam("@Weight", SqlDbType.Int, row["Weight"]);
                    sqlExec.AddCommandParam("@ConstructYear", SqlDbType.SmallInt, row["ConstructYear"]);
                    sqlExec.AddCommandParam("@FuelType", SqlDbType.VarChar, row["FuelType"]);
                    sqlExec.AddCommandParam("@Manufacturer", SqlDbType.VarChar, row["Manufacturer"]);
                    sqlExec.AddCommandParam("@ModelNumber", SqlDbType.VarChar, row["ModelNumber"]);
                    sqlExec.AddCommandParam("@VehicleType", SqlDbType.VarChar, row["VehicleType"]);
                }
                else
                {
                    sqlExec.AddCommandParam("@Equipment", SqlDbType.VarChar, "");
                    //sqlExec.AddCommandParam("@TeamLeaderName", SqlDbType.VarChar, "");
                    sqlExec.AddCommandParam("@ListName", SqlDbType.VarChar, "");
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                    sqlExec.AddCommandParam("@LicensePlate", SqlDbType.VarChar, "");
                    if (row.Table.Columns.Contains("Weight"))
                        sqlExec.AddCommandParam("@Weight", SqlDbType.Int, row["Weight"]);
                    else
                        sqlExec.AddCommandParam("@Weight", SqlDbType.Int, 0);
                    sqlExec.AddCommandParam("@ConstructYear", SqlDbType.SmallInt, 0);
                    sqlExec.AddCommandParam("@FuelType", SqlDbType.VarChar, "");
                    sqlExec.AddCommandParam("@Manufacturer", SqlDbType.VarChar, "");
                    sqlExec.AddCommandParam("@ModelNumber", SqlDbType.VarChar, "");
                    sqlExec.AddCommandParam("@VehicleType", SqlDbType.VarChar, "");
                }

                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.AttachToTransaction(sql);
                }

                if (DriverAssignment)
                    sqlExec.SPExecuteDataset(sql);
                else
                    sqlExec.SPExecuteNonQuery(sql);

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }


        }

        public int DynamicDriverAssignment(int organizationId, string driverFirstName, string driverLastName, string vehicleDescription, DateTime effectiveFrom)
        {
            string sql = "sp_vlfDynamicDriverAssignmentByVehicleDescriptionDriverName";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@VehicleDescription", SqlDbType.VarChar, vehicleDescription);
            sqlExec.AddCommandParam("@EffectiveFrom", SqlDbType.VarChar, effectiveFrom);
            sqlExec.AddCommandParam("@DriverFirstName", SqlDbType.VarChar, driverFirstName);
            sqlExec.AddCommandParam("@DriverLastName", SqlDbType.VarChar, driverLastName);
            

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }

            return sqlExec.SPExecuteNonQuery(sql);
        }

        public DataSet GetChildrenByNodeCode(int organizationId, string NodeCode)
        {
            string sql = "GetOrganizationHierarchyChildrenByNodeCode";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, NodeCode);

            DataSet ds = sqlExec.SPExecuteDataset(sql);
            return ds;

        }

        public DataSet GetChildrenByNodeCodeUserId(int organizationId, string NodeCode, int UserId)
        {
            string sql = "GetOrganizationHierarchyChildrenByNodeCodeUserID";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, NodeCode);
            sqlExec.AddCommandParam("@UserID", SqlDbType.Int, UserId);

            DataSet ds = sqlExec.SPExecuteDataset(sql);
            return ds;

        }

        public DataSet GetVehicleListByNodeCode(int organizationId, string NodeCode)
        {
            string sql = "GetOrganizationHierarchyAssignmentVehicleListByNodeCode";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, NodeCode);

            DataSet ds = sqlExec.SPExecuteDataset(sql);
            return ds;
        }

        public DataSet GetOrganizationHierarchyByPreferNodeCode(int organizationId, string preferNodecode)
        {
            string sql = "GetOrganizationHierarchyByPreferNodeCode";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@PreferNodeCode", SqlDbType.VarChar, preferNodecode);

            DataSet ds = sqlExec.SPExecuteDataset(sql);
            return ds;
        }

        public bool HasOrganizationHierarchy(int organizationId)
        {
            string sql = "SELECT COUNT(ID) FROM vlfOrganizationHierarchy WITH(NOLOCK) WHERE OrganizationId=" + organizationId.ToString();
            return int.Parse( sqlExec.SQLExecuteScalar(sql).ToString()) != 0;
        }

        public int HierarchyParentSetup(int organizationId)
        {
            string sql = "HierarchyParentSetup";            
            sqlExec.ClearCommandParameters();
            sqlExec.CommandTimeout = 36000;
            sqlExec.AddCommandParam("@OrgId", SqlDbType.Int, organizationId);
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public int OrganizationHierarchyAssignHgiUserToTopLevel(int organizationId)
        {
            string sql = "OrganizationHierarchyAssignHgiUserToTopLevel";
            sqlExec.ClearCommandParameters();
            sqlExec.CommandTimeout = 36000;
            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public string GetOrganizationHierarchyPath(int organizationId, string NodeCode)
        {
            string sql = "sp_GetOrganizationHierarchyPath";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, NodeCode);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            return dt.Tables[0].Rows[0][0].ToString();
        }

        public string GetOrganizationHierarchyNamePath(int organizationId, string nodeCode)
        {
            string sql = "sp_GetOrganizationHierarchyNamePath";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, nodeCode);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            if(dt.Tables[0].Rows.Count > 0)
                return dt.Tables[0].Rows[0][0].ToString();
            return "";
        }

        public DateTime? GetLastEffectiveFrom(int organizationId)
        {
            string sql = "SELECT MAX(EffectiveFrom) AS LastFrom FROM vlfOrganizationHierarchyAssignment WITH(NOLOCK) WHERE OrganizationId=" + organizationId.ToString();
            DataSet dt = sqlExec.SQLExecuteDataset(sql);

            return dt.Tables[0].Rows[0].Field<DateTime?>("LastFrom"); 

        }

        public DataSet SearchOrganizationHierarchy(int organizationId, string s)
        {
            //string sql = "sp_SearchOrganizationHierarchy";

            //sqlExec.ClearCommandParameters();
            //sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            //sqlExec.AddCommandParam("@SearchString", SqlDbType.VarChar, s);

            //return sqlExec.SPExecuteDataset(sql);
            return this.SearchOrganizationHierarchy(organizationId, s, -1);
        }

        public DataSet SearchOrganizationHierarchy(int organizationId, string s, int userId)
        {
            string sql = "sp_SearchOrganizationHierarchy";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@SearchString", SqlDbType.VarChar, s);
            sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);

            return sqlExec.SPExecuteDataset(sql);
        }

        public bool ValidateNodeCodeInUserPreference(int organizationId, int userId, string nodecode)
        {
            string sql = "sp_ValidateNodeCodeInUserPreference";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, nodecode);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            int c = int.Parse(dt.Tables[0].Rows[0][0].ToString());
            return c > 0;            
        }

        public bool OrganizationHierarchyParentChildNodeCode(int organizationId, string parentNodecode, string childNodecode)
        {
            string sql = "OrganizationHierarchyParentChildNodeCode";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@ParentNodeCode", SqlDbType.VarChar, parentNodecode);
            sqlExec.AddCommandParam("@ChildNodeCode", SqlDbType.VarChar, childNodecode);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            int c = int.Parse(dt.Tables[0].Rows[0][0].ToString());
            return c > 0;
        }

        public int AddOrganizationHierarchyNode(int organizationId, string nodeCode, string nodeName,string parentNodeCode, int userId)
        {
            int parentId = 0;
            string sql = "SELECT ID FROM vlfOrganizationHierarchy WITH(NOLOCK) WHERE OrganizationId=" + organizationId + " AND NodeCode='" + parentNodeCode + "'";
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
            {
                parentId = int.Parse(dt.Tables[0].Rows[0][0].ToString());
                if (parentId > 0)
                {
                    /*sql = string.Format("INSERT INTO vlfOrganizationHierarchy(OrganizationId, NodeCode, NodeName, IsParent, ParentNodeCode, ParentId) VALUES({0},'{1}','{2}',0,'{3}',{4})",
                            organizationId, nodeCode, nodeName, parentNodeCode, parentId);
                    sqlExec.SQLExecuteNonQuery(sql);

                    sql = string.Format("DECLARE @fleetId INT; SELECT @fleetId = MAX(FleetId) + 1 FROM dbo.vlfFleet; INSERT INTO dbo.vlfFleet (FleetId, FleetName, OrganizationId, [Description], FleetType, NodeCode) VALUES (@fleetId, '{0}', {1}, '{2}','oh', '{3}')",
                            nodeCode + " - " + nodeName, organizationId, nodeName, nodeCode);

                    sqlExec.SQLExecuteNonQuery(sql);
                     
                    // Add to flat table
                    sql = "SELECT * FROM vlfOrganizationHierarchyFlat WHERE OrganizationId=" + organizationId + " AND ChildNodeCode='" + parentNodeCode + "'";
                    DataSet dt1 = sqlExec.SQLExecuteDataset(sql);
                    foreach (DataRow r in dt1.Tables[0].Rows)
                    {
                        sql = "INSERT INTO vlfOrganizationHierarchyFlat(ParentNodeCode, ChildNodeCode, OrganizationId) VALUES('" + r["ParentNodeCode"] + "','" + nodeCode + "'," + organizationId + ")";
                        sqlExec.SQLExecuteNonQuery(sql);
                    }

                    sql = "INSERT INTO vlfOrganizationHierarchyFlat(ParentNodeCode, ChildNodeCode, OrganizationId) VALUES('" + nodeCode + "','" + nodeCode + "'," + organizationId + ")";
                    sqlExec.SQLExecuteNonQuery(sql); 
                     
                     */
                    sql = "OrganizationHierarchyAddNodeCode";
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                    sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, nodeCode);
                    sqlExec.AddCommandParam("@NodeName", SqlDbType.VarChar, nodeName);
                    sqlExec.AddCommandParam("@ParentNodeCode", SqlDbType.VarChar, parentNodeCode);
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, "");
                    sqlExec.AddCommandParam("@ParentId", SqlDbType.Int, parentId);
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);

                    sqlExec.SPExecuteNonQuery(sql);

                    return 1;

                }
            }

            return 0;
        }

        public int DeleteOrganizationHierarchyNode(int organizationId, string nodeCode)
        {
            DataSet dt = this.GetChildrenByNodeCode(organizationId, nodeCode);
            int deletedRows = 0;
            if (dt.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in dt.Tables[0].Rows)
                {
                    if(nodeCode.Trim() != r["NodeCode"].ToString().Trim())
                        deletedRows += this.DeleteOrganizationHierarchyNode(organizationId, r["NodeCode"].ToString());
                }
            }
            
            //string sql = "OrganizationHierarchyDeleteNodeCode";
            //sqlExec.ClearCommandParameters();
            //sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            //sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, nodeCode);
            //return sqlExec.SPExecuteNonQuery(sql);            
            return this.DeleteOrganizationHierarchySingleNode(organizationId, nodeCode);
        }

        public int DeleteOrganizationHierarchySingleNode(int organizationId, string nodeCode)
        {
            string sql = "OrganizationHierarchyDeleteNodeCode";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, nodeCode);
            return sqlExec.SPExecuteNonQuery(sql); 
        }

        public bool hasNodeCode(int organizationId, string nodeCode)
        {
            string sql = "SELECT Count(*) FROM vlfOrganizationHierarchy WITH(NOLOCK) WHERE OrganizationId=" + organizationId + " AND NodeCode='" + nodeCode + "'";
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            int c = int.Parse(dt.Tables[0].Rows[0][0].ToString());
            return c > 0;
        }

        public bool hasNodeName(int organizationId, string nodeName)
        {
            string sql = "OrganizationHierarchyNodeNameExists";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeName", SqlDbType.VarChar, nodeName);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            int c = int.Parse(dt.Tables[0].Rows[0][0].ToString());
            return c > 0;
        }

        public int GetFleetIdByNodeCode(int organizationId, string nodeCode)
        {
            string sql;
            if (nodeCode != string.Empty)
                sql = "SELECT FleetId FROM vlfFleet WITH(NOLOCK) WHERE OrganizationId=" + organizationId + " AND NodeCode='" + nodeCode + "' AND FleetType='oh'";
            else
                sql = "SELECT TOP 1 f.FleetId FROM vlfFleet f WITH(NOLOCK) INNER JOIN vlfOrganizationHierarchy oh WITH(NOLOCK) ON f.NodeCode=oh.NodeCode  WHERE f.OrganizationId=" + organizationId + " AND oh.OrganizationId=" + organizationId + " AND oh.ParentNodeCode IS NULL";
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
                return int.Parse(dt.Tables[0].Rows[0][0].ToString());
            else
                return 0;                     
        }
        public string GetFleetNameByFleetId(int organizationId, int fleetId)
        {
            string sql = "SELECT FleetName FROM vlfFleet WITH(NOLOCK) WHERE OrganizationId=" + organizationId + " AND FleetId=" + fleetId;
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
                return dt.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty; 
        }


        public int CheckWhetherOrganizationHierarchyNodeCodeIsLeaf(int organizationId, string OrganizationHierarchyNodeCode)
        {
            string sql = "select IsParent from vlfOrganizationHierarchy WITH(NOLOCK) WHERE OrganizationId= " + organizationId + " AND NodeCode= '" + OrganizationHierarchyNodeCode + "'";
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
                return Convert.ToInt32(dt.Tables[0].Rows[0][0]);
            else
                return 0;
        }
        public string GetRootNodeCode(int organizationId)
        {
            string sql = "select NodeCode from vlfOrganizationHierarchy WITH(NOLOCK) WHERE OrganizationId=" + organizationId + " AND ParentNodeCode IS NULL";
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
                return dt.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }
        public int AssignFleetToAllParents(int organizationId, int fleetId, int vehicleId)
        {
            string sql = "OrganizationHierarchyAssignFleetToAllParents";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, vehicleId);
            return sqlExec.SPExecuteNonQuery(sql);    
        }

        public int RemoveVehicleFromFleet(int organizationId, int fleetId, int vehicleId)
        {
            string sql = "OrganizationHierarchyDeleteVehicleFromFleet";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, vehicleId);
            return sqlExec.SPExecuteNonQuery(sql);    
        }

        public string GetOrganizationHierarchyRootNodeCodeUserID(int organizationId, int userId)
        {
            //string sql = "GetOrganizationHierarchyRootNodeCodeUserID";
            //sqlExec.ClearCommandParameters();
            //sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            //sqlExec.AddCommandParam("@UserID", SqlDbType.Int, userId);

            //DataSet dt = sqlExec.SPExecuteDataset(sql);
            //if (dt.Tables[0].Rows.Count > 0)
            //    return dt.Tables[0].Rows[0][0].ToString();
            //return string.Empty;
            return this.GetOrganizationHierarchyRootNodeCodeUserID(organizationId, userId, false);
            
        }

        public string GetOrganizationHierarchyRootNodeCodeUserID(int organizationId, int userId, bool multipleAssignment)
        {
            string sql = "GetOrganizationHierarchyRootNodeCodeUserID";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@UserID", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@MultipleAssignment", SqlDbType.Bit, multipleAssignment ? 1 : 0);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
            {
                if (multipleAssignment)
                {
                    string s = string.Empty;
                    foreach (DataRow dr in dt.Tables[0].Rows)
                    {
                        if (s == string.Empty)
                            s = dr[0].ToString();
                        else
                            s = s + "," + dr[0].ToString();
                    }
                    return s;
                }
                else
                    return dt.Tables[0].Rows[0][0].ToString();
            }
            return string.Empty;

        }

        public DataSet GetOrganizationHierarchyRootByUserID(int organizationId, int userId)
        {
            //string sql = "GetOrganizationHierarchyRootByUserID";
            //sqlExec.ClearCommandParameters();
            //sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            //sqlExec.AddCommandParam("@UserID", SqlDbType.Int, userId);

            //return sqlExec.SPExecuteDataset(sql);
            return this.GetOrganizationHierarchyRootByUserID(organizationId, userId, false);
        }

        public DataSet GetOrganizationHierarchyRootByUserID(int organizationId, int userId, bool multipleAssignment)
        {
            string sql = "GetOrganizationHierarchyRootByUserID";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@UserID", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@MultipleAssignment", SqlDbType.Bit, multipleAssignment ? 1 : 0);

            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetOrganizationHierarchyAllFleetsByUserId(int organizationId, int userId)
        {
            string root = this.GetOrganizationHierarchyRootNodeCodeUserID(organizationId, userId);
            string sql = "GetOrganizationHierarchyAllChildFleetsByNodeCode";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, root);

            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetAllUnassingToOrganizationHierarchyFleetVehiclesInfo(int organizationId)
        {
            string sql = "GetAllUnassingToOrganizationHierarchyFleetVehiclesInfo";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);            

            return sqlExec.SPExecuteDataset(sql);

        }

        public DataSet GetOrganizationHierarchyUnassignedUsersInfo(int organizationId)
        {
            /*string sql = "GetOrganizationHierarchyUnassignedUsersInfo";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);

            return sqlExec.SPExecuteDataset(sql);*/
            return this.GetOrganizationHierarchyUnassignedUsersInfo(organizationId, false, 0);
        }

        public DataSet GetOrganizationHierarchyUnassignedUsersInfo(int organizationId, bool multipleAssignment, int fleetId)
        {
            string sql = "GetOrganizationHierarchyUnassignedUsersInfo";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@MultipleAssignment", SqlDbType.Bit, multipleAssignment ? 1 : 0 );
            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);

            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetOrganizationHierarchyBySql(string sql)
        {
            sql = "SELECT * FROM vlfOrganizationHierarchy WITH(NOLOCK) WHERE " + sql;
            
            return sqlExec.SQLExecuteDataset(sql);
        }

        public string ValidatedNodeCodes(int organizationId, int userId, string nodecodes)
        {
            string[] ss = nodecodes.Split(',');
            string returnnodecodes = "";
            foreach (string s in ss)
            {
                if (this.ValidateUserNodeCode(organizationId, userId, s))
                    returnnodecodes += s + ",";
            }
            return returnnodecodes.Trim(',');
        }

        public bool ValidateUserNodeCode(int organizationId, int userId, string nodecode)
        {
            string sql = "OrganizationHierarchyValidateUserNodeCode";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@UserID", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, nodecode);

            DataSet dt = sqlExec.SPExecuteDataset(sql);

            return int.Parse(dt.Tables[0].Rows[0][0].ToString()) > 0;
            
        }

        public string OrganizationHierarchyGetHierarchyByVehicleId(int organizationId, int vehicleId)
        {
            string sql = "OrganizationHierarchyGetHierarchyByVehicleId";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, vehicleId);            

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
                return dt.Tables[0].Rows[0]["Path"].ToString();
            else
                return "";
        }

        public int GetUserIdByUserName(string userName, int organizationId)
        {
            string sql;
            int userId = 0;
            if (userName != string.Empty)
            {
                sql = "SELECT UserId FROM vlfUser WITH(NOLOCK) WHERE OrganizationId=" + organizationId + " AND UserName='" + userName + "'";

                DataSet dt = sqlExec.SQLExecuteDataset(sql);
                if (dt.Tables[0].Rows.Count > 0)
                    userId = int.Parse(dt.Tables[0].Rows[0][0].ToString());                
            }
            return userId;
        }
    }
}
