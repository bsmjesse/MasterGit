using System;
using System.Data ;			   // for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    public class CustomReportEmailManager : Das
    {
        private VLF.DAS.DB.CustomReportEmail _customReportEmail = null;

        public CustomReportEmailManager(string connectionString)
            : base(connectionString)
		{
            _customReportEmail = new VLF.DAS.DB.CustomReportEmail(sqlExec);
        
        }

        public int CustomReportEmail_Add(string Email, int OrganizationId, string FleetIds, int UserId, int UID)
        {
            return _customReportEmail.CustomReportEmail_Add(Email, OrganizationId, FleetIds, UserId, UID);
        }

        public int CustomReportEmail_Delete(long CustomReportEmailID, int OrganizationId, int UID)
        {
            return _customReportEmail.CustomReportEmail_Delete(CustomReportEmailID, OrganizationId, UID);
        }

        public int CustomReportEmail_Update(string Email, long CustomReportEmailID, int OrganizationId, string FleetIds, int UID)
        {
            return _customReportEmail.CustomReportEmail_Update(Email, CustomReportEmailID, OrganizationId, FleetIds, UID);
        }

        public int CustomReportEmailFleet_Add(long CustomReportEmailID, string FleetIds, int UID)
        {
            return _customReportEmail.CustomReportEmailFleet_Add(CustomReportEmailID, FleetIds, UID);
        }

        public int CustomReportEmailMessage_Add(string CustomReportEmailMessage, int OrganizationId, int UID)
        {
            return _customReportEmail.CustomReportEmailMessage_Add(CustomReportEmailMessage, OrganizationId, UID);
        }

        public DataSet CustomReportEmail_Get(int OrganizationId, Boolean isMessage)
        {
            return _customReportEmail.CustomReportEmail_Get(OrganizationId, isMessage);
        }

        public DataSet CustomReportEmail_GetHGI_USER(int OrganizationId)
        {
            return _customReportEmail.CustomReportEmail_GetHGI_USER(OrganizationId);
        }
    }
}