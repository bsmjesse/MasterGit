using System;
using System.Data ;			   // for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Collections;
using System.Collections.Generic;

namespace VLF.DAS.Logic
{
    public class WorkingHoursManager : Das
    {
        private VLF.DAS.DB.WorkingHours _WorkingHours = null;
        public WorkingHoursManager(string connectionString)
            : base(connectionString)
        {
            _WorkingHours = new VLF.DAS.DB.WorkingHours(sqlExec);

        }

        public int FleetWorkingHours_add(int FleetID, string Email, string WorkingHrs, string Exeption, int OrganizationId, int WorkingHoursRangeId, int Timezone, Int64 UID)
        {
            return _WorkingHours.FleetWorkingHours_add(FleetID, Email, WorkingHrs, Exeption, OrganizationId, WorkingHoursRangeId, Timezone, UID);
        }

        //public int CheckWorkingHoursUTC(ArrayList FromTime, ArrayList ToTime, ArrayList WorkingHrs, int OrganizationId)
        //{
        //    return _WorkingHours.CheckWorkingHoursUTC(FromTime, ToTime, WorkingHrs, OrganizationId);
        //}

        public DataSet GetWorkingHoursUTCByFleetId(string FleetId)
        {
            return _WorkingHours.GetWorkingHoursUTCByFleetId(FleetId);
        }

        public DataSet FleetWorkingHours_Report(int FleetId, DateTime MinDateTime)
        {
            return _WorkingHours.FleetWorkingHours_Report(FleetId, MinDateTime);
        }
    }
}
