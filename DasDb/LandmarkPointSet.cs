using System;
using System.Data.SqlClient;	// for SqlException
using System.Data;			// for DataSet
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to vlfLandmark table.
    /// </summary>
    public class LandmarkPointSet : TblGenInterfaces
    {
        public LandmarkPointSet(SQLExecuter sqlExec)
            : base("vlfLandmarkPointSet", sqlExec)
      {
      }
        public DataSet GetLandmarkPointSetByLandmarkId(long LandmarkId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "Select * from vlfLandmarkPointSet " +
                   " WHERE LandmarkId=" + LandmarkId.ToString() + " order by SequenceNum ";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetLandmarkPointSetByLandmarkId by LandmarkId id=" + LandmarkId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetLandmarkPointSetByLandmarkId by LandmarkId id=" + LandmarkId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetLandmarkPointSetByLandmarkName(string LandmarkName, int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                LandmarkName = LandmarkName.Replace("'", "''");
                //Prepares SQL statement
                string sql = string.Format("Select * from vlfLandmarkPointSet " +
                   " WHERE LandmarkId= (Select LandmarkId from vlfLandmark where LandmarkName='{0}' and OrganizationId = {1} )",
                   LandmarkName, OrganizationId
                   );
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetLandmarkPointSetByLandmarkId by LandmarkName =" + LandmarkName + " OrganizationId=" + OrganizationId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetLandmarkPointSetByLandmarkId by LandmarkName =" + LandmarkName + " OrganizationId=" + OrganizationId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetLandmarkPointSetByOrganizationId(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "Select Replace(LandmarkName,char(39)+''+char(39),char(39)) as LandmarkName, vlfLandmarkPointSet.* from vlfLandmark inner join vlfLandmarkPointSet on vlfLandmark.LandmarkId = vlfLandmarkPointSet.LandmarkId " +
                   " WHERE OrganizationId=" + OrganizationId.ToString() + " and vlfLandmark.Radius =-1 order by LandmarkName, SequenceNum ";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetLandmarkPointSetByOrganizationId by OrganizationId id=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetLandmarkPointSetByOrganizationId by OrganizationId id=" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        // Changes for TimeZone Feature start

        public int vlfLandmarkPointSet_Add_NewTZ(int organizationId, string landmarkName, double latitude,
        double longitude, string description, string contactPersonName,
        string contactPhoneNum, int radius, string email, string phone, float timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
           string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "vlfLandmarkPointSet_Add_NewTimeZone";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@LandmarkName", SqlDbType.VarChar, landmarkName);
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
                if (description == null)
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
                if (contactPersonName == null)
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, contactPersonName);
                if (contactPhoneNum == null)
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, contactPhoneNum);
                sqlExec.AddCommandParam("@Radius", SqlDbType.Int, radius);
                if (email == null || email == "")
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, email);

                if (phone == null || phone == "")
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, phone);

                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Float, timeZone);
                sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.Bit, dayLightSaving);
                sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.Bit, autoAdjustDayLightSaving);
                if (streetAddress == null || streetAddress == "")
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, streetAddress);

                sqlExec.AddCommandParam("@pointSets", SqlDbType.VarChar, pointSets);
                sqlExec.AddCommandParam("@public", SqlDbType.Bit, landmarkPublic);
                sqlExec.AddCommandParam("@newCategoryId", SqlDbType.BigInt, categoryId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Add organizationId=" + organizationId + " LandmarkName=" + landmarkName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Add organizationId=" + organizationId + " LandmarkName=" + landmarkName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }

        // Changes for TimeZone Feature end

        public int vlfLandmarkPointSet_Add(int organizationId, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "vlfLandmarkPointSet_Add";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@LandmarkName", SqlDbType.VarChar, landmarkName);
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
                if (description == null)
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
                if (contactPersonName == null)
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, contactPersonName);
                if (contactPhoneNum == null)
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, contactPhoneNum);
                sqlExec.AddCommandParam("@Radius", SqlDbType.Int, radius);
                if (email == null || email == "")
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, email);

                if (phone == null || phone == "")
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, phone);

                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Int, timeZone);
                sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.Bit, dayLightSaving);
                sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.Bit, autoAdjustDayLightSaving);
                if (streetAddress == null || streetAddress == "")
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, streetAddress);

                sqlExec.AddCommandParam("@pointSets", SqlDbType.VarChar, pointSets);
                sqlExec.AddCommandParam("@public", SqlDbType.Bit, landmarkPublic);
                sqlExec.AddCommandParam("@newCategoryId", SqlDbType.BigInt, categoryId);

                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.AttachToTransaction(sql);
                }
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Add organizationId=" + organizationId + " LandmarkName=" + landmarkName ;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Add organizationId=" + organizationId + " LandmarkName=" + landmarkName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }


        // Changes for TimeZone Feature start

        public int vlfLandmarkPointSet_Update_NewTZ(int organizationId, string currLandmarkName, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, float timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "vlfLandmarkPointSet_Update_NewTimeZone";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@currLandmarkName", SqlDbType.VarChar, currLandmarkName);
                sqlExec.AddCommandParam("@LandmarkName", SqlDbType.VarChar, landmarkName);
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
                if (description == null)
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
                if (contactPersonName == null)
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, contactPersonName);
                if (contactPhoneNum == null)
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, contactPhoneNum);
                sqlExec.AddCommandParam("@Radius", SqlDbType.Int, radius);
                if (email == null || email == "")
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, email);

                if (phone == null || phone == "")
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, phone);

                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Float, timeZone);
                sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.Bit, dayLightSaving);
                sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.Bit, autoAdjustDayLightSaving);
                if (streetAddress == null || streetAddress == "")
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, streetAddress);

                sqlExec.AddCommandParam("@pointSets", SqlDbType.VarChar, pointSets);
                sqlExec.AddCommandParam("@public", SqlDbType.Bit, landmarkPublic);
                sqlExec.AddCommandParam("@newCategoryId", SqlDbType.BigInt, categoryId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Update organizationId=" + organizationId + " LandmarkName=" + landmarkName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Update organizationId=" + organizationId + " LandmarkName=" + landmarkName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }

        // Changes for TimeZone Feature end

        public int vlfLandmarkPointSet_Update(int organizationId, string currLandmarkName, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "vlfLandmarkPointSet_Update";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@currLandmarkName", SqlDbType.VarChar, currLandmarkName);
                sqlExec.AddCommandParam("@LandmarkName", SqlDbType.VarChar, landmarkName);
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
                if (description == null)
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
                if (contactPersonName == null)
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.VarChar, contactPersonName);
                if (contactPhoneNum == null)
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.VarChar, contactPhoneNum);
                sqlExec.AddCommandParam("@Radius", SqlDbType.Int, radius);
                if (email == null || email == "")
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, email);

                if (phone == null || phone == "")
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@phone", SqlDbType.VarChar, phone);

                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Int, timeZone);
                sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.Bit, dayLightSaving);
                sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.Bit, autoAdjustDayLightSaving);
                if (streetAddress == null || streetAddress == "")
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.VarChar, streetAddress);

                sqlExec.AddCommandParam("@pointSets", SqlDbType.VarChar, pointSets);
                sqlExec.AddCommandParam("@public", SqlDbType.Bit, landmarkPublic);
                sqlExec.AddCommandParam("@newCategoryId", SqlDbType.BigInt, categoryId);

                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.AttachToTransaction(sql);
                }
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Update organizationId=" + organizationId + " LandmarkName=" + landmarkName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Update organizationId=" + organizationId + " LandmarkName=" + landmarkName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }

        public int vlfLandmarkPointSet_Delete(int organizationId, string LandmarkName)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "vlfLandmarkPointSet_Delete";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@LandmarkName", SqlDbType.VarChar, LandmarkName);

                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.AttachToTransaction(sql);
                }
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Delete organizationId=" + organizationId + " LandmarkName=" + LandmarkName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Delete organizationId=" + organizationId + " LandmarkName=" + LandmarkName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }

        public long vlfLandmarkPostCommOnTheFly_Add(int OrganizationId, int UserId, int FleetId, DateTime Start,
            DateTime Finish, double Latitude, double Longitude, int Radius, string Email, string pointSets, string StreetAddress)
        {
            long rowsAffected = -1;
            try
            {
                string sql = "vlfLandmarkPostCommOnTheFly_Add";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);
                sqlExec.AddCommandParam("@Start", SqlDbType.DateTime, Start);
                sqlExec.AddCommandParam("@Finish", SqlDbType.DateTime, Finish);
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, Latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, Longitude);
                sqlExec.AddCommandParam("@Radius", SqlDbType.Int, Radius);
                sqlExec.AddCommandParam("@Email", SqlDbType.NVarChar, Email);
                sqlExec.AddCommandParam("@pointSets", SqlDbType.VarChar, pointSets);
                sqlExec.AddCommandParam("@StreetAddress", SqlDbType.NVarChar, StreetAddress);
                sqlExec.AddCommandParam("@RETURN_VALUE", SqlDbType.VarChar, ParameterDirection.ReturnValue, string.Empty);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
                if (sqlExec.ReadCommandParam("@RETURN_VALUE") != null) rowsAffected = long.Parse(sqlExec.ReadCommandParam("@RETURN_VALUE").ToString());
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Add organizationId=" + OrganizationId + " UserId=" + UserId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to vlfLandmarkPointSet_Add organizationId=" + OrganizationId + " UserId=" + UserId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }
    }
}
