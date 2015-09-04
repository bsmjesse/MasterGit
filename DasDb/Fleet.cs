using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfFleet table
	/// </summary>
	public class Fleet : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Fleet(SQLExecuter sqlExec) : base ("vlfFleet", sqlExec)
		{
		}

		/// <summary>
		/// Add new fleet.
		/// </summary>
		/// <param name="fleetName"></param>
		/// <param name="organizationId"></param>
		/// <param name="description"></param>
		/// <returns>int next fleet id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if fleet alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddFleet(string fleetName, int organizationId, string description)
		{
         string prefixMsg = "Error adding a new fleet.";
         int fleetId = VLF.CLS.Def.Const.unassignedIntValue;
			// 1. validates parameters
			if (String.IsNullOrEmpty(fleetName) || 
				(organizationId == VLF.CLS.Def.Const.unassignedIntValue))
             //String.IsNullOrEmpty(description))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: FleetName=" + 
					fleetName + " OrganizationId=" + organizationId + " Description=" + description);
			}

			int rowsAffected = 0;

			// 2. Get next availible index
			fleetId = (int)GetMaxValue("FleetId") + 1;
			
			// 3. Prepares SQL statement
         string sql = //"FleetAdd";
            "INSERT INTO " + tableName +
            " (FleetId, FleetName, OrganizationId, Description) VALUES (@fleetId, @fleetName, @orgId, @descr)";

         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
         sqlExec.AddCommandParam("@fleetName", SqlDbType.VarChar, fleetName.Replace("'", "''"));
         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description.Replace("'", "''"));

			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 4. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 5. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
         if (fleetId == 0)
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This fleet already exists.");
			}
			return fleetId;
		}

      /// <summary>
      /// Add new fleet, assign a user and assign hgi_admin user if exists
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="fleetName"></param>
      /// <param name="organizationId"></param>
      /// <param name="description"></param>
      /// <returns>int next fleet id</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if fleet alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddFleet(int userId, string fleetName, int organizationId, string description)
      {
         string prefixMsg = "Error adding a new fleet.";
         int fleetId = VLF.CLS.Def.Const.unassignedIntValue;
         
         // 1. validates parameters
         if (String.IsNullOrEmpty(fleetName) ||
            (organizationId == VLF.CLS.Def.Const.unassignedIntValue))
         {
            throw new DASAppInvalidValueException("Wrong value for insert SQL: FleetName=" +
               fleetName + " OrganizationId=" + organizationId + " Description=" + description);
         }

         int rowsAffected = 0;

         // 3. Stored procedure name
         string sql = "FleetUserAdd";

         sqlExec.ClearCommandParameters();

         SqlParameter[] sqlParams = new SqlParameter[5];
         sqlParams[0] = new SqlParameter("@userId", userId);
         sqlParams[1] = new SqlParameter("@fleetName", fleetName.Replace("'", "''"));
         sqlParams[2] = new SqlParameter("@orgId", organizationId);
         sqlParams[3] = new SqlParameter("@descr", description.Replace("'", "''"));
         sqlParams[4] = new SqlParameter("@fleetId", fleetId);
         sqlParams[4].Direction = ParameterDirection.Output;

         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 4. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 5. Executes SQL statement
            rowsAffected = sqlExec.SPExecuteNonQuery(sql, sqlParams);
            if (sqlParams[4].Value != null)
               fleetId = Convert.ToInt32(sqlParams[4].Value);
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
         //if(rowsAffected == 0) 
         if (fleetId <= 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This fleet already exists.");
         }
         return fleetId;
      }		

		/// <summary>
		/// Update fleet information.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="fleetName"></param>
		/// <param name="organizationId"></param>
		/// <param name="description"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if fleet alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateInfo(int fleetId,string fleetName,int organizationId,string description)
		{
			// 1. validates parameters
			if((fleetId == VLF.CLS.Def.Const.unassignedIntValue)||
				(fleetName == VLF.CLS.Def.Const.unassignedStrValue)||
				(organizationId == VLF.CLS.Def.Const.unassignedIntValue)||
				(description == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: fleetId=" +
					organizationId + 
					" FleetId=" + fleetId + 
					" FleetName=" + fleetName +
					" OrganizationId=" + organizationId +
					" Description=" + description);
			}
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + 
				" SET FleetName='" + fleetName.Replace("'","''") + "'" +
				", OrganizationId=" + organizationId + 
				", Description='" + description.Replace("'","''") + "'" +
				" WHERE FleetId=" + fleetId;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update fleet id " + fleetId + " name" + fleetName + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update fleet id " + fleetId + " name" + fleetName + " .";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update fleet id " + fleetId + " name" + fleetName + " .";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This fleet already exists.");
			}
		}

        /// <summary>
        /// Delete existing fleet.
        /// </summary>
        /// <returns>rows affected</returns>
        /// <param name="fleetId"></param> 
        /// <exception cref="DASAppResultNotFoundException">Thrown if fleet id does not exist</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteFleetByFleetId(int fleetId)
        {
            return DeleteRowsByIntField("FleetId", fleetId, "fleet id");
        }

        /// <summary>
        /// Delete all existing fleets by org id
        /// </summary>
        /// <returns>rows affected</returns>
        /// <param name="orgId"></param> 
        /// <exception cref="DASAppResultNotFoundException">Thrown if fleet id does not exist</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteOrganizationFleets(int orgId)
        {
            return DeleteRowsByIntField("OrganizationId", orgId, "organization id");
        }

		/// <summary>
		/// Retrieves Fleet info
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>DataSet [FleetId],[FleetName],[FleetDescription],[OrganizationId],[OrganizationName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFleetInfoByFleetId(int fleetId)
		{
			//return GetFleetInfoBy("FleetId",fleetId);
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT FleetId,FleetName,vlfOrganization.OrganizationId,vlfFleet.Description,OrganizationName FROM vlfOrganization,vlfFleet"+
					" WHERE FleetId=" + fleetId +
					" AND vlfFleet.OrganizationId=vlfOrganization.OrganizationId" +
					" ORDER BY FleetName";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by fleet id " + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by fleet id " + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

		/// <summary>
		/// Retrieves Fleets info
		/// </summary>
		/// <returns>DataSet [FleetId],[FleetName],[Description],[OrganizationId],[OrganizationName]</returns>
		/// <param name="organizationName"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFleetsInfoByOrganizationName(string organizationName)
		{
			//return GetFleetInfoBy("OrganizationName",organizationName);
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT FleetId,FleetName,vlfOrganization.OrganizationId,vlfFleet.Description,OrganizationName FROM vlfOrganization,vlfFleet"+
					" WHERE OrganizationName='" + organizationName.Replace("'","''") + "'" +
					" AND vlfFleet.OrganizationId=vlfOrganization.OrganizationId" +
					" ORDER BY FleetName";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve fleets info by organization name " + organizationName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve fleets info by organization name " + organizationName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        /// <summary>
        /// Retrieves All Fleets id for entire organization
        /// </summary>
        /// <param name="orgId" type="int">Organization ID</param>
        /// <returns>DataSet [FleetId]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFleetsByOrganizationId(int orgId)
        {
            DataSet sqlDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT FleetId FROM " + this.tableName +
                    " WHERE OrganizationId = " + orgId.ToString();
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve fleets by organization id " + orgId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve fleets by organization id " + orgId.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

		/// <summary>
		/// Returns fleet id by fleet name.
		/// </summary>
		/// <param name="organizationId"></param> 
		/// <param name="fleetName"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetFleetIdByFleetName(int organizationId,string fleetName)
		{
			int fleetId = VLF.CLS.Def.Const.unassignedIntValue;
            string sql = "";
			try
			{
				// 1. Prepares SQL statement
                if (fleetName == "All Vehicles")
                {
                     sql = "SELECT TOP 1 FleetId FROM vlfFleet" +
                                    " WHERE (FleetName='All Vehicles' OR FleetName='Tous les véhicules' OR FleetName='Tous les vehicules')" +
                                    " AND OrganizationId=" + organizationId;
                }
                else
                {
                    // 1. Prepares SQL statement
                     sql = "SELECT FleetId FROM vlfFleet" +
                                    " WHERE FleetName='" + fleetName.Replace("'", "''") + "'" +
                                    " AND OrganizationId=" + organizationId;
                }

				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				fleetId = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve fleet id by fleet name '" + fleetName + "' and organization id=" + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve fleet id by fleet name '" + fleetName + "' and organization id=" + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return fleetId;
		}

		/// <summary>
		/// Returns fleet name by fleet Id. 	
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetFleetNameByFleetId(int fleetId)
		{
			string fleetName = VLF.CLS.Def.Const.unassignedStrValue;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT FleetName FROM vlfFleet WHERE FleetId=" + fleetId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				fleetName = Convert.ToString(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve fleet name by fleet id=" + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve fleet name by fleet id=" + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return fleetName;
		}
	}
}
