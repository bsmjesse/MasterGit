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
   public class Landmark : TblGenInterfaces
   {
      #region Public Interfaces
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public Landmark(SQLExecuter sqlExec)
         : base("vlfLandmark", sqlExec)
      {
      }

      /// <summary>
      /// Add new landmark for import.
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="landmarkName"></param>
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <param name="description"></param>
      /// <param name="contactPersonName"></param>
      /// <param name="contactPhoneNum"></param>
      /// <param name="radius"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="streetAddress"></param>
      /// <param name="publicLandmark"></param>
      /// <param name="createUserID"></param>
      /// <param name="categoryName"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public string AddLandmark(int organizationId, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, bool publicLandmark, int createUserID, string categoryName)
      {
          string rvMessage = null;
          int rowsAffected = 0;
          // 1. Prepares SQL statement
          try
          {
              string sql = "vlfLandmark_Add_ForImport";

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


              sqlExec.AddCommandParam("@createUserID", SqlDbType.Int, createUserID);
              sqlExec.AddCommandParam("@public", SqlDbType.Bit, publicLandmark);
              sqlExec.AddCommandParam("@categoryName", SqlDbType.NVarChar, categoryName);

              if (sqlExec.RequiredTransaction())
              {
                  sqlExec.AttachToTransaction(sql);
              }
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);

              if (rowsAffected == 0)
              {
                  rvMessage = "Add failed";
              }
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to add new landmark to OrganizationId '" + organizationId +
                 " LandmarkName=" + landmarkName + ", Latitude=" + latitude + " Longitude=" + longitude +
                 " Description=" + description + " ContactPersonName=" + contactPersonName +
                 " ContactPhoneNum=" + contactPhoneNum + " radius=" + radius + ".";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to add new landmark to OrganizationId '" + organizationId +
                 " LandmarkName=" + landmarkName + ", Latitude=" + latitude + " Longitude=" + longitude +
                 " Description=" + description + " ContactPersonName=" + contactPersonName +
                 " ContactPhoneNum=" + contactPhoneNum + " radius=" + radius + ".";
              throw new DASException(prefixMsg + " " + objException.Message);
          }

          return rvMessage;
      }





      ///// <summary>
      ///// Add new landmark.
      ///// </summary>
      ///// <param name="organizationId"></param>
      ///// <param name="landmarkName"></param>
      ///// <param name="latitude"></param>
      ///// <param name="longitude"></param>
      ///// <param name="description"></param>
      ///// <param name="contactPersonName"></param>
      ///// <param name="contactPhoneNum"></param>
      ///// <param name="radius"></param>
      ///// <param name="email"></param>
      ///// <param name="timeZone"></param>
      ///// <param name="dayLightSaving"></param>
      ///// <param name="autoAdjustDayLightSaving"></param>
      ///// <param name="streetAddress"></param>
      ///// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
      ///// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      ///// <exception cref="DASException">Thrown in all other exception cases.</exception>
      //public void AddLandmark(int organizationId, string landmarkName, double latitude,
      //   double longitude, string description, string contactPersonName,
      //   string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
      //      string streetAddress,bool publicLandmark)
      //{
      //   // 1. Prepares SQL statement
      //   try
      //   {
      //      // Set SQL command
      //      string sql = "INSERT INTO " + tableName + "( " +
      //                  "OrganizationId" +
      //                  ",LandmarkName" +
      //                  ",Latitude" +
      //                  ",Longitude" +
      //                  ",Description" +
      //                  ",ContactPersonName" +
      //                  ",ContactPhoneNum" +
      //                  ",Radius" +
      //                          ",Email,phone,TimeZone,DayLightSaving,AutoAdjustDayLightSaving,StreetAddress,[Public]) VALUES ( @OrganizationId,@LandmarkName,@Latitude,@Longitude,@Description,@ContactPersonName,@ContactPhoneNum,@Radius,@Email,@phone,@TimeZone,@DayLightSaving,@AutoAdjustDayLightSaving,@StreetAddress,@publicLandmark) ";
      //      // Add parameters to SQL statement
      //      sqlExec.ClearCommandParameters();
      //      sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
      //      //sqlExec.AddCommandParam("@LandmarkName",SqlDbType.Char,landmarkName.Replace("'","''"));
      //      sqlExec.AddCommandParam("@LandmarkName", SqlDbType.Char, landmarkName);
      //      sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
      //      sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
      //      if (description == null)
      //         sqlExec.AddCommandParam("@Description", SqlDbType.Char, System.DBNull.Value);
      //      else
      //         //sqlExec.AddCommandParam("@Description",SqlDbType.Char,description.Replace("'","''"));
      //         sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
      //      if (contactPersonName == null)
      //         sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.Char, System.DBNull.Value);
      //      else
      //         //sqlExec.AddCommandParam("@ContactPersonName",SqlDbType.Char,contactPersonName.Replace("'","''"));
      //         sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.Char, contactPersonName);
      //      if (contactPhoneNum == null)
      //         sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.Char, System.DBNull.Value);
      //      else
      //         //sqlExec.AddCommandParam("@ContactPhoneNum",SqlDbType.Char,contactPhoneNum.Replace("'","''"));
      //         sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.Char, contactPhoneNum);
      //      sqlExec.AddCommandParam("@Radius", SqlDbType.Int, radius);
      //      if (email == null || email == "")
      //         sqlExec.AddCommandParam("@Email", SqlDbType.Char, System.DBNull.Value);
      //      else
      //         sqlExec.AddCommandParam("@Email", SqlDbType.Char, email);

      //     if (phone == null || phone == "")
      //         sqlExec.AddCommandParam("@phone", SqlDbType.Char, System.DBNull.Value);
      //     else
      //         sqlExec.AddCommandParam("@phone", SqlDbType.Char, phone);

      //      sqlExec.AddCommandParam("@TimeZone", SqlDbType.Int, timeZone);
      //      sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(dayLightSaving));
      //      sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(autoAdjustDayLightSaving));
      //      sqlExec.AddCommandParam("@publicLandmark", SqlDbType.SmallInt, Convert.ToInt16(publicLandmark));
      //      if (streetAddress == null || streetAddress == "")
      //         sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, System.DBNull.Value);
      //      else
      //         sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, streetAddress);

      //      //Executes SQL statement
      //      sqlExec.SQLExecuteNonQuery(sql);
      //      // 2. Executes SQL statement
      //      //sqlExec.SQLExecuteNonQuery(sql);
      //   }
      //   catch (SqlException objException)
      //   {
      //      string prefixMsg = "Unable to add new landmark to OrganizationId '" + organizationId +
      //         " LandmarkName=" + landmarkName + ", Latitude=" + latitude + " Longitude=" + longitude +
      //         " Description=" + description + " ContactPersonName=" + contactPersonName +
      //         " ContactPhoneNum=" + contactPhoneNum + " radius=" + radius + ".";
      //      Util.ProcessDbException(prefixMsg, objException);
      //   }
      //   catch (DASDbConnectionClosed exCnn)
      //   {
      //      throw new DASDbConnectionClosed(exCnn.Message);
      //   }
      //   catch (Exception objException)
      //   {
      //      string prefixMsg = "Unable to add new landmark to OrganizationId '" + organizationId +
      //         " LandmarkName=" + landmarkName + ", Latitude=" + latitude + " Longitude=" + longitude +
      //         " Description=" + description + " ContactPersonName=" + contactPersonName +
      //         " ContactPhoneNum=" + contactPhoneNum + " radius=" + radius + ".";
      //      throw new DASException(prefixMsg + " " + objException.Message);
      //   }
      //}

      /// <summary>
      /// Add new landmark.
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="landmarkName"></param>
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <param name="description"></param>
      /// <param name="contactPersonName"></param>
      /// <param name="contactPhoneNum"></param>
      /// <param name="radius"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="streetAddress"></param>
      /// <param name="result">True if successful</param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddLandmark(int organizationId, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, ref bool result)
      {
         result = false;
         string prefixMsg = "Unable to add new landmark to OrganizationId '" + organizationId +
            " LandmarkName=" + landmarkName + ", Latitude=" + latitude + " Longitude=" + longitude +
            " Description=" + description + " ContactPersonName=" + contactPersonName +
            " ContactPhoneNum=" + contactPhoneNum + " radius=" + radius + ".";
         // 1. Prepares SQL statement
         try
         {
            // Set SQL command
            string sql = "INSERT INTO " + tableName + "( " +
                         "OrganizationId,LandmarkName,Latitude,Longitude,Description,ContactPersonName,ContactPhoneNum,Radius" +
                         ",Email,Phone,TimeZone,DayLightSaving,AutoAdjustDayLightSaving,StreetAddress) VALUES ( @OrganizationId,@LandmarkName,@Latitude,@Longitude,@Description,@ContactPersonName,@ContactPhoneNum,@Radius,@Email,@Phone,@TimeZone,@DayLightSaving,@AutoAdjustDayLightSaving,@StreetAddress ) ";
            // Add parameters to SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            //sqlExec.AddCommandParam("@LandmarkName",SqlDbType.Char,landmarkName.Replace("'","''"));
            sqlExec.AddCommandParam("@LandmarkName", SqlDbType.Char, landmarkName);
            sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
            sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
            if (description == null)
               sqlExec.AddCommandParam("@Description", SqlDbType.Char, System.DBNull.Value);
            else
               //sqlExec.AddCommandParam("@Description",SqlDbType.Char,description.Replace("'","''"));
               sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
            if (contactPersonName == null)
               sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.Char, System.DBNull.Value);
            else
               //sqlExec.AddCommandParam("@ContactPersonName",SqlDbType.Char,contactPersonName.Replace("'","''"));
               sqlExec.AddCommandParam("@ContactPersonName", SqlDbType.Char, contactPersonName);
            if (contactPhoneNum == null)
               sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.Char, System.DBNull.Value);
            else
               //sqlExec.AddCommandParam("@ContactPhoneNum",SqlDbType.Char,contactPhoneNum.Replace("'","''"));
               sqlExec.AddCommandParam("@ContactPhoneNum", SqlDbType.Char, contactPhoneNum);
            sqlExec.AddCommandParam("@Radius", SqlDbType.Int, radius);
            if (email == null || email == "")
               sqlExec.AddCommandParam("@Email", SqlDbType.Char, System.DBNull.Value);
            else
               sqlExec.AddCommandParam("@Email", SqlDbType.Char, email);

           if (email == null || email == "")
               sqlExec.AddCommandParam("@Phone", SqlDbType.Char, System.DBNull.Value);
           else
               sqlExec.AddCommandParam("@Phone", SqlDbType.Char, phone);

            sqlExec.AddCommandParam("@TimeZone", SqlDbType.Int, timeZone);
            sqlExec.AddCommandParam("@DayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(dayLightSaving));
            sqlExec.AddCommandParam("@AutoAdjustDayLightSaving", SqlDbType.SmallInt, Convert.ToInt16(autoAdjustDayLightSaving));
            if (streetAddress == null || streetAddress == "")
               sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, System.DBNull.Value);
            else
               sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, streetAddress);

            //Executes SQL statement
            if (sqlExec.SQLExecuteNonQuery(sql) == 1) result = true;
         }
         catch (SqlException objException)
         {
            result = false;
            //Util.ProcessDbException(prefixMsg, objException);
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, prefixMsg + " " + objException.Message);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
      }

      /// <summary>
      /// Deletes all landmarks related to specific organization.
      /// </summary>
      /// <returns>rows affected</returns>
      /// <param name="organizationId"></param>
      /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteAllLandMarksByOrganizationId(int organizationId)
      {
         return DeleteRowsByIntField("OrganizationId", organizationId, "organization id");
      }

      /// <summary>
      /// Deletes landmark from organization.
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="landmarkName"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteLandmarkFromOrganization(int organizationId, string landmarkName)
      {
         int rowsAffected = 0;
         // 1. Prepares SQL statement
         string sql = "DELETE FROM " + tableName +
                  " WHERE OrganizationId=" + organizationId +
                  " AND LandmarkName='" + landmarkName.Replace("'", "''") + "'";
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to delete landmark=" + landmarkName + " from organization " + organizationId;
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to delete landmark=" + landmarkName + " from organization " + organizationId;
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// Update landmark info.
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="currLandmarkName"></param>
      /// <param name="newLandmarkName"></param>
      /// <param name="newLatitude"></param>
      /// <param name="newLongitude"></param>
      /// <param name="newDescription"></param>
      /// <param name="newContactPersonName"></param>
      /// <param name="newContactPhoneNum"></param>
      /// <param name="radius"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="streetAddress"></param>
      /// <remarks>
      /// In case of updating landmark info except landmark name, set newLandmarkName to VLF.CLS.Def.Const.unassignedStrValue
      /// </remarks>
      /// <returns>void</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateLandmark(int organizationId, string currLandmarkName,
         string newLandmarkName, double newLatitude,
         double newLongitude, string newDescription,
         string newContactPersonName,
         string newContactPhoneNum,
         int radius,
         string email,string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress,bool publicLandmark)
      {
         // 1. validates parameters
         if ((organizationId == VLF.CLS.Def.Const.unassignedIntValue) ||
            (currLandmarkName == VLF.CLS.Def.Const.unassignedStrValue))
         {
            throw new DASAppInvalidValueException("Wrong value for insert SQL: organization Id=" +
               organizationId + " currLandmarkName=" + currLandmarkName + " newLandmarkName=" + newLandmarkName);
         }
         int rowsAffected = 0;
         //Prepares SQL statement
         string sql = "UPDATE " + tableName +
                  " SET Latitude=" + newLatitude +
                  ", Longitude=" + newLongitude +
                  ", Description='" + newDescription.Replace("'", "''") + "'" +
                  ", ContactPersonName='" + newContactPersonName.Replace("'", "''") + "'" +
                  ", ContactPhoneNum='" + newContactPhoneNum.Replace("'", "''") + "'" +
                  ", Radius=" + radius +
                  ", Email='" + email.Replace("'", "''") + "'" +
                  ", Phone='" + phone.Replace("'", "''") + "'" +
                  ", TimeZone=" + timeZone +
                  ", DayLightSaving=" + Convert.ToInt16(dayLightSaving) +
                  ", Public=" + Convert.ToInt16(publicLandmark) +
                  ", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
                        ", StreetAddress='" + streetAddress.Replace("'", "''") + "'";

         if ((newLandmarkName != VLF.CLS.Def.Const.unassignedStrValue) &&
            (currLandmarkName != newLandmarkName))
            sql += ", LandmarkName='" + newLandmarkName.Replace("'", "''") + "'";

         sql += " WHERE organizationId=" + organizationId +
             " AND LandmarkName='" + currLandmarkName.Replace("'", "''") + "'";
         //currLandmarkName.Replace("'", "['']") + "'";
         try
         {
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update organization Id " + organizationId + " LandmarkName=" + currLandmarkName + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update organization Id " + organizationId + " LandmarkName=" + currLandmarkName + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to update organization Id " + organizationId + " LandmarkName=" + currLandmarkName + ".";
            throw new DASAppResultNotFoundException(prefixMsg + " This landmark does not exist.");
         }
      }

      /// <summary>
      /// Retrieves landmarks info by organization id 
      /// </summary>
      /// <returns>
      /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
      /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [StreetAddress],[Radius]
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetLandMarksInfoByOrganizationId(int organizationId)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Replace(Description,char(39)+''+char(39),char(39)) AS Description,ContactPersonName,ContactPhoneNum,Radius," +
                     "ISNULL(Email,' ') AS Email," +
                     "ISNULL(Phone,' ') AS Phone," +
                     "ISNULL(TimeZone,0) AS TimeZone," +
                     "ISNULL(DayLightSaving,0) AS DayLightSaving," +
                     "ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving," +
                            "ISNULL(StreetAddress,' ') AS StreetAddress,Radius,LandmarkId " +
               " FROM " + tableName +
               " WHERE OrganizationId=" + organizationId + " ORDER BY LandmarkName";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      /// Retrieves landmarks info by box id 
      /// </summary>
      /// <returns>
      /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
      /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [StreetAddress]
      /// </returns>
      /// <param name="boxId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetLandMarksInfoByBoxId(int boxId)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement
             string sql = "SELECT vlfBox.OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Replace(Description,char(39)+''+char(39),char(39)) AS Description,ContactPersonName,ContactPhoneNum,Radius,ISNULL(Email,' ') AS Email,ISNULL(Phone,' ') AS Phone,ISNULL(TimeZone,0) AS TimeZone,ISNULL(DayLightSaving,0) AS DayLightSaving,ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving,ISNULL(StreetAddress,' ') AS StreetAddress" +
                " FROM vlfLandmark INNER JOIN vlfBox with (nolock) ON vlfLandmark.OrganizationId = vlfBox.OrganizationId WHERE vlfBox.BoxId=" + boxId + " ORDER BY LandmarkName";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve info by box Id=" + boxId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve info by box Id=" + boxId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      /// Retrieves landmarks info by organization name 
      /// </summary>
      /// <returns>
      /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
      /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],[StreetAddress]
      /// </returns>
      /// <param name="organizationName"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetLandMarksInfoByOrganizationName(string organizationName)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement				
            string sql = "SELECT vlfOrganization.OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Replace(vlfLandmark.Description,char(39)+''+char(39),char(39)) AS Description,ContactPersonName,ContactPhoneNum,Radius,ISNULL(vlfLandmark.StreetAddress,' ') AS StreetAddress" +
           " FROM vlfOrganization,vlfLandmark" +
           " WHERE vlfOrganization.OrganizationName='" + organizationName.Replace("'", "''") + "'" +
           " AND vlfOrganization.OrganizationId=vlfLandmark.OrganizationId";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve info by organization name=" + organizationName + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve info by organization name=" + organizationName + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      ///     returns all landmarks which are valid (w AND wo an address)
      /// </summary>
      /// <comment>
      ///     extracts also the Radius defined with any landmark !!!
      /// </comment>  
      /// <param name="OrgIds"></param>
      /// <returns></returns>
      public DataSet GetAllLandMarksInfo(int[] OrgIds)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement				
            string sql = string.Format("SELECT OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Radius FROM vlfLandmark WHERE OrganizationId in ({0}) order by OrganizationId", Util.ArrayToString(OrgIds));
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all landmarks info. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all landmarks info. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      /// Retrieves all landmarks info
      /// </summary>
      /// <returns>
      /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllLandMarksInfo()
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement				
            string sql = "SELECT OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude, Radius FROM vlfLandmark order by OrganizationId"; // WHERE StreetAddress IS NULL";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all landmarks info. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all landmarks info. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      /// Update landmark info.
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="landmarkName"></param>
      /// <param name="streetAddress"></param>
      /// <remarks>
      /// In case of updating landmark info except landmark name, set newLandmarkName to VLF.CLS.Def.Const.unassignedStrValue
      /// </remarks>
      /// <returns>void</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateLandmarkStreetAddress(int organizationId, string landmarkName, string streetAddress)
      {
         int rowsAffected = 0;
         //Prepares SQL statement
         string sql = "UPDATE vlfLandmark SET StreetAddress='" + streetAddress.Replace("'", "''") + "' WHERE organizationId=" + organizationId + " AND LandmarkName like '" + landmarkName.Replace("'", "%''%") + "'";
         try
         {
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update organization Id " + organizationId + " LandmarkName=" + landmarkName + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update organization Id " + organizationId + " LandmarkName=" + landmarkName + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to update organization Id " + organizationId + " LandmarkName=" + landmarkName + ".";
            throw new DASAppResultNotFoundException(prefixMsg + " This landmark does not exist.");
         }
      }

      /// <summary>
      /// Retrieves landmark name by location
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <remarks>
      /// If landmark does not exist, returns VLF.CLS.Def.Const.unassignedStrValue
      /// </remarks>
      /// <returns>
      /// string [landmark name]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public string GetLandmarkName(int organizationId, double latitude, double longitude)
      {
         string landmarkName = VLF.CLS.Def.Const.unassignedStrValue;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName" +
               " FROM vlfLandmark" +
               " WHERE OrganizationId=" + organizationId +
               " AND Latitude=" + latitude +
               " AND Longitude=" + longitude;
            //Executes SQL statement
            landmarkName = Convert.ToString(sqlExec.SQLExecuteScalar(sql));
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve landmark name by organization id=" + organizationId + " latitude=" + latitude + " longitude=" + longitude + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve landmark name by organization id=" + organizationId + " latitude=" + latitude + " longitude=" + longitude + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return landmarkName;
      }

      /// <summary>
      /// Retrieves landmark location by landmark name 
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="landmarkName"></param>
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <remarks>
      /// If landmark does not exist, returns latitude=0,longitude=0
      /// </remarks>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void GetLandmarkLocation(int organizationId, string landmarkName, ref double latitude, ref double longitude)
      {
         latitude = 0;
         longitude = 0;
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT Latitude,Longitude" +
               " FROM vlfLandmark" +
               " WHERE OrganizationId=" + organizationId +
               " AND LandmarkName='" + landmarkName.Replace("'", "''") + "'";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            if ((sqlDataSet != null) && (sqlDataSet.Tables.Count > 0) && (sqlDataSet.Tables[0].Rows.Count > 0))
            {
               latitude = Convert.ToDouble(sqlDataSet.Tables[0].Rows[0]["Latitude"]);
               longitude = Convert.ToDouble(sqlDataSet.Tables[0].Rows[0]["Longitude"]);
            }
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve landmark location by organization id=" + organizationId + " landmark name=" + landmarkName + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve landmark location by organization id=" + organizationId + " landmark name=" + landmarkName + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
      }

      /// <summary>
      /// Get Landmark Messages to build report
      /// added 2008/03/25 Max
      /// </summary>
      /// <comment>
      ///   this is based on 4 types of messages - SENSOR, IDLING, EXTENDED_SENSOR, EXTENDED_IDLING
      /// </comment>
      /// <param name="userId"></param>
      /// <param name="licensePlate"></param>
      /// <param name="landmarkName"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns></returns>
      public DataTable GetLandmarkVehicleMessages(int userId, string licensePlate, string landmarkName, DateTime dtFrom, DateTime dtTo)
      {
         DataTable resultSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "GetLandmarkVehicleMessages->Unable to get info by landmark Name = " + landmarkName + ". ";
            SqlParameter[] sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@licenseP", licensePlate);
            sqlParams[2] = new SqlParameter("@landmark", landmarkName);
            sqlParams[3] = new SqlParameter("@dtFrom", dtFrom);
            sqlParams[4] = new SqlParameter("@dtTo", dtTo);

            // SQL statement
            string sql = "ReportLandmark4Vehicle";
            //Executes SQL statement
            resultSet = sqlExec.SPExecuteDataTable(sql, "LandmarkMessages", sqlParams);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultSet;
      }


       public DataSet GetVehicleAtLandmarkStopIdlingAcitivity(int userId, string licensePlate, string landmarkName, DateTime dtFrom, DateTime dtTo)
       {
           DataSet resultSet = null;
           string prefixMsg = "";
           try
           {
               prefixMsg = "GetVehicleAtLandmarkStopIdlingAcitivity->Unable to get info by landmark Name = " + landmarkName + ". ";
               SqlParameter[] sqlParams = new SqlParameter[5];
               sqlParams[0] = new SqlParameter("@LicensePlate", licensePlate);
               sqlParams[1] = new SqlParameter("@landmark", landmarkName);
               sqlParams[2] = new SqlParameter("@fromDate", dtFrom);
               sqlParams[3] = new SqlParameter("@toDate", dtTo);
               sqlParams[4] = new SqlParameter("@userId", userId);

               
               

               // SQL statement
               string sql = "[GetVehicleAtLandmarkStopIdlingActivity]";
               //Executes SQL statement
               sqlExec.CommandTimeout = 72000; 
               resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
               resultSet.Tables[0].TableName = "LandmarkDetails";  
           }
           catch (SqlException objException)
           {
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return resultSet;
       }

       public int DeleteLandmarkCategory(int userId, int orgId, long landmarkId)
       {
           int rowsAffected = 0;
           try
           {
               string sql = "DeleteLandmarkCategory";

               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
               sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
               sqlExec.AddCommandParam("@LandmarkId", SqlDbType.BigInt, landmarkId);

               rowsAffected = sqlExec.SPExecuteNonQuery(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to DeleteLandmarkCategory organizationId=" + orgId + " userId=" + userId + " landmarkId=" + landmarkId;
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to DeleteLandmarkCategory organizationId=" + orgId + " userId=" + userId + " landmarkId=" + landmarkId;
               throw new DASException(prefixMsg + " " + objException.Message);
           }

           return rowsAffected;
       }

       public int SaveLandmarkCategory(int userId, int orgId, long landmarkId, long landmarkCategoryId)
       {
           int rowsAffected = 0;
           try
           {
               string sql = "SaveLandmarkCategory";

               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
               sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
               sqlExec.AddCommandParam("@LandmarkId", SqlDbType.BigInt, landmarkId);
               sqlExec.AddCommandParam("@CategoryId", SqlDbType.BigInt, landmarkCategoryId);

               rowsAffected = sqlExec.SPExecuteNonQuery(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to SaveLandmarkCategory organizationId=" + orgId + " userId=" + userId + " landmarkId=" + landmarkId;
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to SaveLandmarkCategory organizationId=" + orgId + " userId=" + userId + " landmarkId=" + landmarkId;
               throw new DASException(prefixMsg + " " + objException.Message);
           }

           return rowsAffected;
       }

       public DataSet GetLandmarkCategory(int userId, int orgId, long landmarkId)
       {
           DataSet resultSet = null;
           string prefixMsg = "";
           try
           {

               prefixMsg = "GetLandmarkCategory->Unable to get info for userd  = " + userId.ToString() + ". ";
               SqlParameter[] sqlParams = new SqlParameter[3];
               sqlParams[0] = new SqlParameter("@OrganizationId", orgId);
               sqlParams[1] = new SqlParameter("@UserId", userId);
               sqlParams[2] = new SqlParameter("@LandmarkId", landmarkId);

               // SQL statement
               string sql = "[GetLandmarkCategory]";
               //Executes SQL statement
               resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

           }
           catch (SqlException objException)
           {
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return resultSet;
       }

       public DataSet ListOrganizationLandmarkCategory(int userId, int orgId)
       {
           DataSet resultSet = null;
           string prefixMsg = "";
           try
           {
               
               prefixMsg = "ListOrganizationLandmarkCategory->Unable to get info for userd  = " + userId.ToString() + ". ";
               SqlParameter[] sqlParams = new SqlParameter[3];
               sqlParams[0] = new SqlParameter("@OrganizationId", orgId);
               sqlParams[1] = new SqlParameter("@UserId", userId);
               sqlParams[2] = new SqlParameter("@DomainId", 1);

               // SQL statement
               //Exec [dbo].[ListOrganizationDomainMetadata] OrganizationId, UserId, DomainId
               string sql = "[ListOrganizationDomainMetadata]";
               //Executes SQL statement
               resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

           }
           catch (SqlException objException)
           {
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return resultSet;
       }


       public DataSet GetOrganizationLandmark_Public(int userId, int orgId,bool IsPublic  )
       {
           DataSet resultSet = null;
           string prefixMsg = "";
           try
           {
               prefixMsg = "GetOrganizationLandmark_Public->Unable to get info for userd  = " + userId.ToString()  + ". ";
               SqlParameter[] sqlParams = new SqlParameter[3];
               sqlParams[0] = new SqlParameter("@OrgId", orgId);
               sqlParams[1] = new SqlParameter("@UserId", userId);
               sqlParams[2] = new SqlParameter("@Public", IsPublic);
               // SQL statement
               string sql = "[GetOrganizationLandmark_Public]";
               //Executes SQL statement
               resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
               
           }
           catch (SqlException objException)
           {
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return resultSet;
       }

      /// <summary>
      ///   Get Landmark Messages to build report
      ///   added 2008/04/30 Max
      /// </summary>
      /// <comment>
      ///   this is based on 4 types of messages - SENSOR, IDLING, EXTENDED_SENSOR, EXTENDED_IDLING
      /// </comment>
      /// <param name="userId"></param>
      /// <param name="landmarkName"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns></returns>
      public DataTable GetLandmarkMessages(int userId, string landmarkName, DateTime dtFrom, DateTime dtTo)
      {
         DataTable resultSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "GetLandmarkMessages -> Unable to get info by landmark Name = " + landmarkName + ". ";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@landmark", landmarkName);
            sqlParams[2] = new SqlParameter("@dtFrom", dtFrom);
            sqlParams[3] = new SqlParameter("@dtTo", dtTo);

            // SQL statement
            string sql = "ReportLandmark";
            //Executes SQL statement
            resultSet = sqlExec.SPExecuteDataTable(sql, "LandmarkMessages", sqlParams);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultSet;
      }

      /// <summary>
      ///      this returns a pair of consecutive messages for the same vehicle 
      ///      located at the landmark 'landmarkName'
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="landmarkName"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns></returns>
      public DataTable GetLandmarkAllMessages(int userId, int vehicleId, 
                                              string landmarkName, DateTime dtFrom, DateTime dtTo)
      {
         DataTable resultSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "GetLandmarkAllMessages-> Unable to get info by landmark Name = " + landmarkName + ". ";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
            sqlParams[2] = new SqlParameter("@landmark", landmarkName);
            sqlParams[3] = new SqlParameter("@dtFrom", dtFrom);
            sqlParams[4] = new SqlParameter("@dtTo", dtTo);

            // SQL statement
            string sql = "ReportLandmarkExtended";
            //Executes SQL statement
            resultSet = sqlExec.SPExecuteDataTable(sql, "LandmarkAllMessages", sqlParams);

            // the result is BoxId, MsgId, OriginDateTime, CustomProp
            //                      MsgId2, OriginDateTime2, CustomProp2
            // then from here you have to find linked pairs and create another table
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultSet;
      }



      public DataSet GetActivityAtLandmarkSummaryReportPerFleet( int fleetId, DateTime dtFrom, DateTime dtTo,int userId)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "GetActivityAtLandmarkSummaryReportPerFleet->Unable to Landmark Activity Report Per Fleet.";
              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              string sql = "[GetActivityAtLandmarkSummaryReportPerFleetNewStructure]";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }
      public DataSet GetActivityAtLandmarkSummaryReportPerFleet(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "GetActivityAtLandmarkSummaryReportPerFleet->Unable to Landmark Activity Report Per Fleet.";
              SqlParameter[] sqlParams = new SqlParameter[5];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);
              sqlParams[4] = new SqlParameter("@activeVehicles", activeVehicles);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              string sql = "[GetActivityAtLandmarkSummaryReportPerFleetNewStructure_AciveVehicles]";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }




      public DataSet evtFactEventsYear_Report(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "evtFactEventsYear_Report->Unable to get Mower Hours Report.";
              SqlParameter[] sqlParams = new SqlParameter[5];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);
              sqlParams[4] = new SqlParameter("@activeVehicles", activeVehicles);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              //string sql = "[evtFactEventsYear_Report]";
              string sql = "[evtFactEventsYear_Report]";
              
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }



      public DataSet evtFactEventsYear_Report(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles,Int16 vehicleTypeId)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "evtFactEventsYear_Report->Unable to get Mower Hours Report.";
              SqlParameter[] sqlParams = new SqlParameter[6];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);
              sqlParams[4] = new SqlParameter("@activeVehicles", activeVehicles);
              sqlParams[5] = new SqlParameter("@vehicleTypeId", vehicleTypeId);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              //string sql = "[evtFactEventsYear_Report]";
              string sql = "[evtFactEventsYear_Report]";

              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }

     

      public DataSet GetActivityAtLandmarkSummaryReportPerFleetSnow(int fleetId, DateTime dtFrom, DateTime dtTo, int userId)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "GetActivityAtLandmarkSummaryReportPerFleetNewStructureSnow->Unable to Landmark Activity Report Per Fleet.";
              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              string sql = "[GetActivityAtLandmarkSummaryReportPerFleetNewStructureSnow]";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }





      public DataSet GetActivityAtLandmarkSummaryReportPerFleetSnow(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "GetActivityAtLandmarkSummaryReportPerFleetNewStructureSnow->Unable to Landmark Activity Report Per Fleet.";
              SqlParameter[] sqlParams = new SqlParameter[5];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);
              sqlParams[4] = new SqlParameter("@activeVehicles", activeVehicles);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              string sql = "[GetActivityAtLandmarkSummaryReportPerFleetNewStructureSnow__AciveVehicles]";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }






      public DataSet GetActivityAtLandmarkSummaryReportPerFleetSnow_MediaType(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles, Int16 mediaTypeId)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "GetActivityAtLandmarkSummaryReportPerFleetNewStructureSnow->Unable to Landmark Activity Report Per Fleet.";
              SqlParameter[] sqlParams = new SqlParameter[6];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);
              sqlParams[4] = new SqlParameter("@activeVehicles", activeVehicles);
              sqlParams[5] = new SqlParameter("@MediaTypeId", mediaTypeId);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              string sql = "[GetActivityAtLandmarkSummaryReportPerFleetNewStructureSnow_MediaType_AciveVehicles]";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }

      public DataSet GetActivityAtLandmarkSummaryReportPerFleetOnTheFly(long landmarkId, int fleetId, DateTime dtFrom, DateTime dtTo, int userId,long JobId)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "GetActivityAtLandmarkSummaryReportPerFleetOnTheFly->Unable to Landmark Activity Report Per Fleet.";
              SqlParameter[] sqlParams = new SqlParameter[6];
              sqlParams[0] = new SqlParameter("@landmarkId", landmarkId);
              sqlParams[1] = new SqlParameter("@fleetId", fleetId);
              sqlParams[2] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[3] = new SqlParameter("@toDate", dtTo);
              sqlParams[4] = new SqlParameter("@userId", userId);
              sqlParams[5] = new SqlParameter("@jobId", JobId);


              // SQL statement
              //string sql = "[GetActivityAtLandmarkSummaryReportPerFleet]";
              string sql = "[GetActivityAtLandmarkSummaryReportPerFleetOnTheFly]";
              //Executes SQL statement

              
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }


      public DataSet GetActivityAtLandmarkSummaryReportPerFleet201107(int fleetId, DateTime dtFrom, DateTime dtTo, int userId)
      {
          DataSet resultSet = null;
          string prefixMsg = "";
          try
          {
              prefixMsg = "GetActivityAtLandmarkSummaryReportPerFleet->Unable to Landmark Activity Report Per Fleet.";
              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@fleetId", fleetId);
              sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[2] = new SqlParameter("@toDate", dtTo);
              sqlParams[3] = new SqlParameter("@userId", userId);


              // SQL statement
              string sql = "[GetActivityAtLandmarkSummaryReportPerFleet201107]";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
              resultSet.Tables[0].TableName = "LandmarkDetails";
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultSet;
      }

      #endregion
   }
}
