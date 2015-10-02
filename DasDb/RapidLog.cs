using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VLF.CLS;
using VLF.ERR;
using System.Data;
using System.Data.SqlClient;

namespace VLF.DAS.DB
{
    public class RapidLog
    {
        public void SaveDriver(string connectionStr, string employeeID, string firstName, string lastName, string CompanyCode,
                string license, string state, DateTime licenseExpired, string country)
        {
            Util.BTrace(Util.INF0, ">> RapidLog.SaveDriver -> EmployeeId[{0}] ", employeeID);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Upsert_driver", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IdNumber", SqlDbType.VarChar);
                        cmd.Parameters["@IdNumber"].Value = employeeID;

                        cmd.Parameters.Add("@FName", SqlDbType.VarChar);
                        cmd.Parameters["@FName"].Value = firstName;

                        cmd.Parameters.Add("@LName", SqlDbType.VarChar);
                        cmd.Parameters["@LName"].Value = lastName;

                        cmd.Parameters.Add("@CompanyId", SqlDbType.VarChar);
                        cmd.Parameters["@CompanyId"].Value = CompanyCode;

                        cmd.Parameters.Add("@DriverLicenseNumber", SqlDbType.VarChar);
                        cmd.Parameters["@DriverLicenseNumber"].Value = license;

                        cmd.Parameters.Add("@DlStateNum", SqlDbType.VarChar);
                        cmd.Parameters["@DlStateNum"].Value = "";

                        cmd.Parameters.Add("@DlExp", SqlDbType.DateTime);
                        cmd.Parameters["@DlExp"].Value = licenseExpired;

                        cmd.Parameters.Add("@Country", SqlDbType.VarChar);
                        cmd.Parameters["@Country"].Value = country;

                        cmd.ExecuteScalar();
                        Util.BTrace(Util.INF0, "RapidLog.SaveDriver -> after store procedure ");

                    }
                }
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("RapidLog.SaveDriver -> SQLException", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("RapidLog.SaveDriver -> Exception" + objException.Message);
            }

            Util.BTrace(Util.INF0, "<< RapidLog.SaveDriver -> RET [{0}]", "");

        }
    }
}
