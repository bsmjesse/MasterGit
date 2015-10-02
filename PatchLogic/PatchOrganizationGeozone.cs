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
    public class PatchOrganizationGeozone : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchOrganizationGeozone organizationGeozone = null;

        public PatchOrganizationGeozone(string connectionString)
           : base(connectionString)
		{
            organizationGeozone = new VLF.PATCH.DB.PatchOrganizationGeozone(sqlExec);        
        }

        public new void Dispose()
        {
            base.Dispose();
        }
        // Changes for TimeZone Feature start

        /// <summary>
        /// Get Organization Geozones with Status
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet PatchGetOrganizationGeozonesWithStatus_NewTZ(int organizationId, int userId)
        {
            return this.organizationGeozone.PatchGetOrganizationGeozonesWithStatus_NewTZ(organizationId, userId);
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Get Organization Geozones with Status
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet PatchGetOrganizationGeozonesWithStatus(int organizationId, int userId)
        {
            return this.organizationGeozone.PatchGetOrganizationGeozonesWithStatus(organizationId, userId);
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Add new geozone to organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void PatchAddGeozone_NewTZ(int organizationId, string geozoneName,
                          short type, short geozoneType, DataSet dsGeozoneSet,
                          short severityId, string description,
                          string email, string phone, float timeZone, bool dayLightSaving,
                          short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool isPublic, int userId)
        {

            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new geozone
                Int64 geozoneNo = organizationGeozone.PatchAddGeozone_NewTZ(organizationId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, isPublic, userId);
                // 3. Add geozone location
                organizationGeozone.PatchAddGeozoneSet(geozoneNo, dsGeozoneSet);
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }


        // Changes for TimeZone Feature end


        /// <summary>
        /// Add new geozone to organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void PatchAddGeozone(int organizationId, string geozoneName,
                          short type, short geozoneType, DataSet dsGeozoneSet,
                          short severityId, string description,
                          string email, string phone, int timeZone, bool dayLightSaving,
                          short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool isPublic, int userId)
        {

            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new geozone
                Int64 geozoneNo = organizationGeozone.PatchAddGeozone(organizationId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, isPublic, userId);
                // 3. Add geozone location
                organizationGeozone.PatchAddGeozoneSet(geozoneNo, dsGeozoneSet);
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Update geozone info.
        /// </summary>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASAppResultNotFoundException">Thrown if geozone does not exist.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        public void PatchUpdateGeozone_NewTZ(int organizationId, short geozoneId, string geozoneName, short type,
                          Int64 geozoneType, DataSet dsGeozoneSet, short severityId, string description,
                          string email, string phone, float timeZone, bool dayLightSaving, short formatType, bool notify,
                          bool warning, bool critical, bool autoAdjustDayLightSaving, bool? isPublic, int userId)
        {



            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Update unassigned geozone location
                if (organizationGeozone.PatchIsGeozoneAssigned(organizationId, geozoneId) == false)
                {
                    Int64 geozoneNo = organizationGeozone.PatchGetGeozoneNoByGeozoneId(organizationId, geozoneId);
                    if (geozoneNo == VLF.CLS.Def.Const.unassignedIntValue)
                        throw new DASAppViolatedIntegrityConstraintsException("Unable to retrieve GeozoneNo by geozone id=" + geozoneId);
                    organizationGeozone.PatchDeleteGeozoneSet(geozoneNo);
                    organizationGeozone.PatchAddGeozoneSet(geozoneNo, dsGeozoneSet);
                }
                // 3. Update geozone info
                organizationGeozone.PatchUpdateGeozone_NewTZ(organizationId, geozoneId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, isPublic, userId);

                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to update geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }


        // Changes for TimeZone Feature end

        /// <summary>
        /// Update geozone info.
        /// </summary>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASAppResultNotFoundException">Thrown if geozone does not exist.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        public void PatchUpdateGeozone(int organizationId, short geozoneId, string geozoneName, short type,
                          Int64 geozoneType, DataSet dsGeozoneSet, short severityId, string description,
                          string email, string phone, int timeZone, bool dayLightSaving, short formatType, bool notify,
                          bool warning, bool critical, bool autoAdjustDayLightSaving, bool? isPublic, int userId)
        {



            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Update unassigned geozone location
                if (organizationGeozone.PatchIsGeozoneAssigned(organizationId, geozoneId) == false)
                {
                    Int64 geozoneNo = organizationGeozone.PatchGetGeozoneNoByGeozoneId(organizationId, geozoneId);
                    if (geozoneNo == VLF.CLS.Def.Const.unassignedIntValue)
                        throw new DASAppViolatedIntegrityConstraintsException("Unable to retrieve GeozoneNo by geozone id=" + geozoneId);
                    organizationGeozone.PatchDeleteGeozoneSet(geozoneNo);
                    organizationGeozone.PatchAddGeozoneSet(geozoneNo, dsGeozoneSet);
                }
                // 3. Update geozone info
                organizationGeozone.PatchUpdateGeozone(organizationId, geozoneId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, isPublic, userId);

                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to update geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }

        public DataSet GetAllUnassignedToFleetGeozonesInfo(int organizationId, int fleetId, int userId)
        {
            return organizationGeozone.GetAllUnassignedToFleetGeozonesInfo(organizationId, fleetId, userId);
        }

        public DataSet GetAllAssignedToFleetGeozonesInfo(int organizationId, int fleetId)
        {
            return organizationGeozone.GetAllAssignedToFleetGeozonesInfo(organizationId, fleetId);
        }

        public int AssignGeozoneToFleet(int organizationId, int geozoneNo, int fleetId)
        {
            return organizationGeozone.AssignGeozoneToFleet(organizationId, geozoneNo, fleetId);
        }

        public int UnassignGeozoneToFleet(int organizationId, int geozoneNo, int fleetId)
        {
            return organizationGeozone.UnassignGeozoneToFleet(organizationId, geozoneNo, fleetId);
        }

        public int AssignGeozoneToFleetByGeozoneName(int organizationId, string geozoneName, int fleetId)
        {
            int geozoneNo = organizationGeozone.GetGeozoneNoByGeozoneName(organizationId, geozoneName);
            return organizationGeozone.AssignGeozoneToFleet(organizationId, geozoneNo, fleetId);
        }

        public Int64 PatchGetGeozoneNoByGeozoneId(int organizationId, int geozoneId)
        {
            return organizationGeozone.PatchGetGeozoneNoByGeozoneId(organizationId, geozoneId);
        }
    }
}
