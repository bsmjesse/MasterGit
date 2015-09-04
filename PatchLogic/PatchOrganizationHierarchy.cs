using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.PATCH.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.PATCH.Logic
{
    public class PatchOrganizationHierarchy : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchOrganizationHierarchy _oh = null;

        public PatchOrganizationHierarchy(string connectionString)
            : base(connectionString)
        {
            _oh = new VLF.PATCH.DB.PatchOrganizationHierarchy(sqlExec);
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public void BatchAddOrganizationHierarchy(int organizationId, DataTable ohdata)
        {
            foreach (DataRow row in ohdata.Rows)
            {
                DataSet h = _oh.GetOrganizationHierarchyByNodecode(organizationId, row["NodeCode"].ToString().Trim());
                if (h.Tables[0].Rows.Count == 0)
                    _oh.SaveOrganizationHierarchy(organizationId, row);
                else if (h.Tables[0].Rows[0]["ParentNodeCode"].ToString().Trim() != row["ParentNodeCode"].ToString().Trim() || h.Tables[0].Rows[0]["NodeName"].ToString().Trim() != row["Nodename"].ToString().Trim())
                    _oh.UpdateOrganizationHierarchy(organizationId, row);

                _oh.SetOrganizationHierarchyParentIdMinus(organizationId, row["NodeCode"].ToString());

            }

            string sql = " OrganizationId=" + organizationId.ToString() + " AND (ParentId <> -1 OR ParentId IS NULL)";
            DataSet hierarchyToDelete = _oh.GetOrganizationHierarchyBySql(sql);
            foreach (DataRow row in hierarchyToDelete.Tables[0].Rows)
            {
                //this.DeleteOrganizationHierarchyNode(organizationId, row["NodeCode"].ToString());
                _oh.DeleteOrganizationHierarchySingleNode(organizationId, row["NodeCode"].ToString());
            }

            _oh.HierarchyParentSetup(organizationId);
            _oh.UpdateOrganizationHierarchyFlat(organizationId);
            _oh.OrganizationHierarchyAssignHgiUserToTopLevel(organizationId);

        }

        public string BatchOrganizationHierarchyAssignment(int organizationId, DataTable ohadata, string objectTabelName, DateTime edatetime)
        {
            DateTime? _lastfrom = _oh.GetLastEffectiveFrom(organizationId);
            if (_lastfrom != null && _lastfrom > edatetime) return "Effective Date cannot be earlier than existing effective date.";
            
            _oh.BatchSetOrganizationHierarchyAssignmentExpireDateTime(organizationId, edatetime);
            foreach (DataRow row in ohadata.Rows)
            {
                //_oh.SaveOrganizationHierarchy(organizationId, row);
                _oh.SaveOrganizationHierarchyAssignment(organizationId, row, objectTabelName, edatetime);
            }

            return string.Empty;

        }

        public string SaveOrganizationHierarchyAssignmentByVehicleDescription(int organizationId, DataTable ohadata, string objectTabelName, DateTime edatetime)
        {
            return SaveOrganizationHierarchyAssignmentByVehicleDescription(organizationId, ohadata, objectTabelName, edatetime, false);
        }

        public string SaveOrganizationHierarchyAssignmentByVehicleDescription(int organizationId, DataTable ohadata, string objectTabelName, DateTime edatetime, bool DriverAssignment)
        {
            return SaveOrganizationHierarchyAssignmentByVehicleDescription(organizationId, ohadata, objectTabelName, edatetime, DriverAssignment, false, 0);

        }

        public string SaveOrganizationHierarchyAssignmentByVehicleDescription(int organizationId, DataTable ohadata, string objectTabelName, DateTime edatetime, bool DriverAssignment, bool FieldColumns, int userId)
        {
            DateTime? _lastfrom = _oh.GetLastEffectiveFrom(organizationId);
            if (_lastfrom != null && _lastfrom > edatetime) return "Effective Date cannot be earlier than existing effective date.";

            _oh.BatchSetOrganizationHierarchyAssignmentExpireDateTime(organizationId, edatetime);
            _oh.deleteOrganizationHierarchyFleetVehicles(organizationId);
            foreach (DataRow row in ohadata.Rows)
            {
                //_oh.SaveOrganizationHierarchy(organizationId, row);
                _oh.SaveOrganizationHierarchyAssignmentByVehicleDescription(organizationId, row, objectTabelName, edatetime, DriverAssignment, FieldColumns, userId);
            }

            return string.Empty;

        }

        public string SaveOrganizationHierarchyDriverAssignmentByVehicleDescriptionDriverName(int organizationId, DataTable ohadata)
        {
            foreach (DataRow row in ohadata.Rows)
            {
                if (row["TransactionDate"] != DBNull.Value)
                {
                    DateTime aftime = Convert.ToDateTime(row["TransactionDate"]);
                    _oh.DynamicDriverAssignment(organizationId, row["FirstName"].ToString(), row["LastName"].ToString(), row["VehicleNumber"].ToString(), aftime);
                }
            }

            return string.Empty;
        }

        public DataSet GetChildrenByNodeCode(int organizationId, string NodeCode)
        {
            return _oh.GetChildrenByNodeCode(organizationId, NodeCode);
        }

        public DataSet GetChildrenByNodeCodeUserId(int organizationId, string NodeCode, int UserId)
        {
            return _oh.GetChildrenByNodeCodeUserId(organizationId, NodeCode, UserId);
        }

        public DataSet GetVehicleListByNodeCode(int organizationId, string NodeCode)
        {
            return _oh.GetVehicleListByNodeCode(organizationId, NodeCode);
        }

        public DataSet GetOrganizationHierarchyByPreferNodeCode(int organizationId, string preferNodecode)
        {
            return _oh.GetOrganizationHierarchyByPreferNodeCode(organizationId, preferNodecode);
        }

        public bool HasOrganizationHierarchy(int organizationId)
        {
            return _oh.HasOrganizationHierarchy(organizationId);
        }

        public string GetOrganizationHierarchyPath(int organizationId, string NodeCode)
        {
            return _oh.GetOrganizationHierarchyPath(organizationId, NodeCode);
        }

        public string GetOrganizationHierarchyNamePath(int organizationId, string nodeCode)
        {
            return _oh.GetOrganizationHierarchyNamePath(organizationId, nodeCode);
        }

        public DataSet SearchOrganizationHierarchy(int organizationId, string s)
        {
            return _oh.SearchOrganizationHierarchy(organizationId, s);
        }
        public DataSet SearchOrganizationHierarchy(int organizationId, string s, int userId)
        {
            return _oh.SearchOrganizationHierarchy(organizationId, s, userId);
        }
        public bool ValidateNodeCodeInUserPreference(int organizationId, int userId, string nodecode)
        {
            return _oh.ValidateNodeCodeInUserPreference(organizationId, userId, nodecode);
        }
        public bool OrganizationHierarchyParentChildNodeCode(int organizationId, string parentNodecode, string childNodecode)
        {
            return _oh.OrganizationHierarchyParentChildNodeCode(organizationId, parentNodecode, childNodecode);
        }
        public string AddOrganizationHierarchyNode(int organizationId, string nodeCode, string nodeName, string parentNodeCode, int userId)
        {
            if (_oh.hasNodeCode(organizationId, nodeCode))
                return "1";
            else if (_oh.hasNodeName(organizationId, nodeName))
                return "2";
            _oh.AddOrganizationHierarchyNode(organizationId, nodeCode, nodeName, parentNodeCode, userId);
            return _oh.GetOrganizationHierarchyPath(organizationId, nodeCode);
        }
        public int DeleteOrganizationHierarchyNode(int organizationId, string nodeCode)
        {
            return _oh.DeleteOrganizationHierarchyNode(organizationId, nodeCode);
        }
        public int GetFleetIdByNodeCode(int organizationId, string nodeCode)
        {
            return _oh.GetFleetIdByNodeCode(organizationId, nodeCode);
        }
        public string GetFleetNameByFleetId(int organizationId, int fleetId)
        {
            return _oh.GetFleetNameByFleetId(organizationId, fleetId);
        }


        public int CheckWhetherOrganizationHierarchyNodeCodeIsLeaf(int organizationId, string OrganizationHierarchyNodeCode)
        {
            return _oh.CheckWhetherOrganizationHierarchyNodeCodeIsLeaf(organizationId, OrganizationHierarchyNodeCode);
        }

        public string GetRootNodeCode(int organizationId)
        {
            return _oh.GetRootNodeCode(organizationId);
        }
        public int AssignFleetToAllParents(int organizationId, int fleetId, int vehicleId)
        {
            return _oh.AssignFleetToAllParents(organizationId, fleetId, vehicleId);
        }

        public int RemoveVehicleFromFleet(int organizationId, int fleetId, int vehicleId)
        {
            return _oh.RemoveVehicleFromFleet(organizationId, fleetId, vehicleId);
        }

        public string GetOrganizationHierarchyRootNodeCodeUserID(int organizationId, int userId)
        {
            return _oh.GetOrganizationHierarchyRootNodeCodeUserID(organizationId, userId);
        }

        public string GetOrganizationHierarchyRootNodeCodeUserID(int organizationId, int userId, bool multipleAssignment)
        {
            return _oh.GetOrganizationHierarchyRootNodeCodeUserID(organizationId, userId, multipleAssignment);
        }

        public DataSet GetOrganizationHierarchyRootByUserID(int organizationId, int userId)
        {
            return _oh.GetOrganizationHierarchyRootByUserID(organizationId, userId);
        }

        public DataSet GetOrganizationHierarchyRootByUserID(int organizationId, int userId, bool multipleAssignment)
        {
            return _oh.GetOrganizationHierarchyRootByUserID(organizationId, userId, multipleAssignment);
        }

        public DataSet GetOrganizationHierarchyAllFleetsByUserId(int organizationId, int userId)
        {
            return _oh.GetOrganizationHierarchyAllFleetsByUserId(organizationId, userId);
        }
        public DataSet GetAllUnassingToOrganizationHierarchyFleetVehiclesInfo(int organizationId)
        {
            return _oh.GetAllUnassingToOrganizationHierarchyFleetVehiclesInfo(organizationId);
        }
        public DataSet GetOrganizationHierarchyUnassignedUsersInfo(int organizationId)
        {
            return _oh.GetOrganizationHierarchyUnassignedUsersInfo(organizationId);
        }
        public DataSet GetOrganizationHierarchyUnassignedUsersInfo(int organizationId, bool multipleAssignment, int fleetId)
        {
            return _oh.GetOrganizationHierarchyUnassignedUsersInfo(organizationId, multipleAssignment, fleetId);
        }
        public string ValidatedNodeCodes(int organizationId, int userId, string nodecodes)
        {
            return _oh.ValidatedNodeCodes(organizationId, userId, nodecodes);
        }
        public string OrganizationHierarchyGetHierarchyByVehicleId(int organizationId, int vehicleId)
        {
            return _oh.OrganizationHierarchyGetHierarchyByVehicleId(organizationId, vehicleId);
        }

        public int GetUserIdByUserName(string userName, int organizationId)
        {
            return _oh.GetUserIdByUserName(userName, organizationId);
        }
    }
}
