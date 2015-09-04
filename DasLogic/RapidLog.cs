using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VLF.DAS.DB;

namespace VLF.DAS.Logic
{
    class RapidLog : Das
    {
        public RapidLog(string connectionString) : base (connectionString)
		{
		}

        public void SaveDriver(string connectionStr, string employeeID, string firstName, string lastName, string CompanyCode,
        string license, string state, DateTime licenseExpired, string country)
        {
            VLF.DAS.DB.RapidLog rapidLog = new VLF.DAS.DB.RapidLog();
            rapidLog.SaveDriver(connectionStr, employeeID, firstName, lastName, CompanyCode,
                                license, state, licenseExpired, country);
        }
    }
}
