using System;
using System.Collections.Generic;
using System.Text;
using VLF.DAS.DB;
using System.Data;
using System.Data.SqlClient;

namespace VLF.DAS.Logic
{
   /// <summary>
   /// Methods for Sensor Profiles configuration
   /// </summary>
   public partial class SystemConfig
   {
      /// <summary>
      /// Delete Sensor Profile
      /// delete row from master table only, detail rows are deleted autom. using cascade deleting
      /// </summary>
      /// <param name="profileId"></param>
      public int DeleteSensorProfile(short profileId)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         return sensorProfile.DeleteRowsByIntField("ProfileId", profileId, "Sensor Profile");
      }

      /// <summary>
      /// Add new Sensor Profile
      /// </summary>
      /// <param name="hwTypeId"></param>
      /// <param name="profileName"></param>
      /// <param name="profileDescription"></param>
      /// <param name="dtSensors"></param>
      /// <returns></returns>
      public short AddSensorProfile(short hwTypeId, string profileName, string profileDescription, DataTable dtSensors)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         return sensorProfile.AddSensorProfile(hwTypeId, profileName, profileDescription, dtSensors);
      }

      /// <summary>
      /// Get all sensor profiles without sensor sets
      /// </summary>
      /// <returns></returns>
      public DataSet GetAllSensorProfiles()
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         return sensorProfile.GetRowsBySql("SELECT * FROM View_SensorProfileList", null);
      }

      /// <summary>
      /// Get all sensor profiles includes sensor sets
      /// </summary>
      /// <returns></returns>
      public DataSet GetFullSensorProfiles()
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         return sensorProfile.GetRowsBySql("SELECT * FROM View_SensorProfiles", null);
      }

      /// <summary>
      /// Get hardware type profiles include sensor sets
      /// </summary>
      /// <param name="hwTypeId">Hardware Type Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetHwTypeSensorProfiles(short hwTypeId)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         SqlParameter[] sqlParams = new SqlParameter[] { new SqlParameter("@hwTypeId", hwTypeId) };
         return sensorProfile.GetRowsBySql("SELECT * FROM View_SensorProfileList WHERE BoxHwTypeId = @hwTypeId", sqlParams);
      }

      /// <summary>
      /// Get Sensor Profile by ID
      /// </summary>
      /// <param name="profileId">Profile Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetSensorProfile(short profileId)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         SqlParameter[] sqlParams = new SqlParameter[] { new SqlParameter("@pid", profileId) };
         return sensorProfile.GetRowsBySql("SELECT * FROM View_SensorProfiles WHERE ProfileId = @pid", sqlParams);
      }

      /// <summary>
      /// Get Sensor Profile by name
      /// </summary>
      /// <param name="profileName">Profile Name</param>
      /// <returns>DataSet</returns>
      public DataSet GetSensorProfile(string profileName)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         SqlParameter[] sqlParams = new SqlParameter[] { new SqlParameter("@pname", profileName) };
         return sensorProfile.GetRowsBySql("SELECT * FROM View_SensorProfiles WHERE ProfileName = @pname", sqlParams);
      }

      /// <summary>
      /// Get Sensor Profile Name from set of sensors and hw type
      /// </summary>
      /// <param name="hwTypeId"></param>
      /// <param name="dtSensors"></param>
      /// <returns></returns>
      public string GetSensorProfileName(short hwTypeId, DataTable dtSensors)
      {
         string profile = "";
         bool match = false;
         // get profiles list
         DataSet dsProfileList = GetHwTypeSensorProfiles(hwTypeId);
         DataTable dtProfile = null;
         foreach (DataRow dRow in dsProfileList.Tables[0].Rows)
         {
            match = false;
            // get profile sensors set
            dtProfile = GetSensorProfile(Convert.ToInt16(dRow["ProfileId"])).Tables[0];
            if (dtProfile.Rows.Count == dtSensors.Rows.Count)
            {
               match = true;
               // compare all sensors
               for (int i = 0; i < dtSensors.Rows.Count; i++)
               {
                  if (!dtProfile.Rows[i]["SensorId"].Equals(dtSensors.Rows[i]["SensorId"]))
                  {
                     match = false;
                     break;
                  }
               }
            }
            
            if (match) break;
         }
         if (match)
            profile = dtProfile.Rows[0]["ProfileName"].ToString();
         return profile;
      }

      /// <summary>
      /// Update profile sensors set only
      /// </summary>
      /// <param name="profileId">Profile Id</param>
      /// <param name="dtSensors">Profile new sensors DataTable</param>
      public void UpdateSensorProfileData(short profileId, DataTable dtSensors)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         foreach (DataRow drSensor in dtSensors.Rows)
         {
            drSensor["ProfileId"] = profileId;
         }
         sensorProfile.UpdateSensorProfile(profileId, dtSensors);
      }

      /// <summary>
      /// Update profile name and / or description only
      /// </summary>
      /// <param name="profileId">Profile Id to update</param>
      /// <param name="profileName">Profile new Name</param>
      /// <param name="profileDescription">Profile new Description</param>
      public void UpdateSensorProfile(short profileId, string profileName, string profileDescription)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         sensorProfile.UpdateSensorProfile(profileId, profileName, profileDescription, null);
      }

      /// <summary>
      /// Update profile name, description and sensors set
      /// </summary>
      /// <param name="profileId">Profile Id to update</param>
      /// <param name="profileName">Profile new Name</param>
      /// <param name="profileDescription">Profile new Description</param>
      /// <param name="dtSensors">Profile new sensors DataTable</param>
      public void UpdateSensorProfileFull(short profileId, string profileName, string profileDescription, DataTable dtSensors)
      {
         SensorProfile sensorProfile = new SensorProfile(this.sqlExec);
         sensorProfile.UpdateSensorProfile(profileId, profileName, profileDescription, dtSensors);
      }
   }
}
