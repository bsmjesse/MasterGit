using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfPersonInfo table.
	/// </summary>
	public class PersonInfo : TblGenInterfaces 
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public PersonInfo(SQLExecuter sqlExec) : base ("vlfPersonInfo", sqlExec)
		{
		}
		
        /// <summary>
		/// Add new person information.
		/// </summary>
		/// <param name="info"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver license or person alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddPerson(ref PersonInfoStruct info)
		{
			// "def_Id_" + DateTime.MaxValue.Ticks.ToString() -> "def_Id_3155378975999999999"
			if(info.personId == "")
				info.personId = "def_Id_" + DateTime.Now.Ticks.ToString(); // generate default
			// 1. validates parameters
			if(	(info.firstName == VLF.CLS.Def.Const.unassignedStrValue)||
				(info.lastName == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException(
					"Wrong value for insert SQL: PersonId=" + info.personId + 
					" first name=" + info.firstName + 
					" last name=" + info.lastName);
			}

			// 2. Check if driver license is unique (only if person has driver license)
			if(	(info.driverLicense != VLF.CLS.Def.Const.unassignedStrValue)&&
				(info.driverLicense != "")&&
				(info.driverLicense.TrimEnd() != ""))
			{
				if(IsDriverLicenseExist(info.driverLicense.TrimEnd()))
				{
					string prefixMsg = "Unable to add new person information.";
					throw new DASAppDataAlreadyExistsException(prefixMsg + " Person with driver license " + info.driverLicense + " already exists.");
				}
			}

			int rowsAffected = 0;		
			// 3. Prepares SQL statement
			string sql = "INSERT INTO vlfPersonInfo" +
				" (PersonId,DriverLicense,FirstName,LastName,MiddleName" +
				",Address,City,StateProvince,Country" +
				",PhoneNo1,PhoneNo2,CellNo,LicenseEndorsements" +
				",Height,Weight,Gender,EyeColor,HairColor" +
				",IdMarks,Certifications,Description" ;
			if(info.birthday != VLF.CLS.Def.Const.unassignedDateTime)
				sql += ",Birthday";
			if(info.licenseExpDate != VLF.CLS.Def.Const.unassignedDateTime)
				sql += ",LicenseExpDate";
			
			string stateProvince = info.userContactInfo.stateProvince;
			if(stateProvince == "")
				stateProvince = "Other";

			sql += ") VALUES (" + 
				"'" + info.personId.Replace("'","''") + "'" + 
				",'" + info.driverLicense.Replace("'","''") + "'" + 
				",'" + info.firstName.Replace("'","''") + "'" + 
				",'" + info.lastName.Replace("'","''") + "'" +
				",'" + info.middleName.Replace("'","''") + "'" +
				",'" + info.userContactInfo.address.Replace("'","''") + "'" +
				",'" + info.userContactInfo.city.Replace("'","''") + "'" +
				",'" + stateProvince.Replace("'","''") + "'" +
				",'" + info.userContactInfo.country.Replace("'","''") + "'" +
				",'" + info.userContactInfo.phoneNo1.Replace("'","''") + "'" +
				",'" + info.userContactInfo.phoneNo2.Replace("'","''") + "'" +
				",'" + info.userContactInfo.cellNo.Replace("'","''") + "'" +
				",'" + info.licenseEndorsements.Replace("'","''") + "'" +
				"," + info.height + 
				"," + info.weight + 
				",'" + info.gender.Replace("'","''") + "'" +
				",'" + info.eyeColor.Replace("'","''") + "'" +
				",'" + info.hairColor.Replace("'","''") + "'" +
				",'" + info.idMarks.Replace("'","''") + "'" +
				",'" + info.certifications.Replace("'","''") + "'" +
				",'" + info.description.Replace("'","''") + "'";
			if(info.birthday != VLF.CLS.Def.Const.unassignedDateTime)
				sql += ",'" + info.birthday + "'";
			if(info.licenseExpDate != VLF.CLS.Def.Const.unassignedDateTime)
				sql += ",'" + info.licenseExpDate + "'";
			sql +=")";

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
				string prefixMsg = "Unable to add new person information.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new person information.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new person information.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This person already exists.");
			}
		}		
		
        /// <summary>
		/// Update person information.
		/// </summary>
		/// <param name="info"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if person does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int UpdateInfo(PersonInfoStruct info)
		{
			int rowsAffected = 0;
			// 1. validates parameters
			if(	(info.personId == VLF.CLS.Def.Const.unassignedStrValue)||
				(info.lastName == VLF.CLS.Def.Const.unassignedStrValue)||
				(info.firstName == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: person Id=" +
					info.personId + " last name=" + info.lastName + " first name=" + info.firstName);
			}
			//Prepares SQL statement
			string sql = "UPDATE vlfPersonInfo SET " +
						" FirstName='" + info.firstName.Replace("'","''") + "'" +
						", LastName='" + info.lastName.Replace("'","''") + "'";
						
			if(info.driverLicense != VLF.CLS.Def.Const.unassignedStrValue)
				sql +=  ", DriverLicense='" + info.driverLicense.Replace("'","''") + "'";
			if(info.middleName != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", MiddleName='" + info.middleName.Replace("'","''") + "'";
			if(info.birthday != VLF.CLS.Def.Const.unassignedDateTime)
				sql += ", Birthday='" + info.birthday + "'";
			if(info.userContactInfo.address != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", Address='" + info.userContactInfo.address.Replace("'","''") + "'";
			if(info.userContactInfo.city != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", City='" + info.userContactInfo.city.Replace("'","''") + "'";
			if(info.userContactInfo.stateProvince != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", StateProvince='" + info.userContactInfo.stateProvince.Replace("'","''") + "'";
			if(info.userContactInfo.country != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", Country='" + info.userContactInfo.country.Replace("'","''") + "'";
			if(info.userContactInfo.phoneNo1 != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", PhoneNo1='" + info.userContactInfo.phoneNo1.Replace("'","''") + "'";
			if(info.userContactInfo.phoneNo2 != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", PhoneNo2='" + info.userContactInfo.phoneNo2.Replace("'","''") + "'";
			if(info.userContactInfo.cellNo != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", CellNo='" + info.userContactInfo.cellNo.Replace("'","''") + "'";
			if(info.licenseExpDate != VLF.CLS.Def.Const.unassignedDateTime)
				sql += ", LicenseExpDate='" + info.licenseExpDate + "'";
			if(info.licenseEndorsements != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", LicenseEndorsements='" + info.licenseEndorsements.Replace("'","''") + "'";
			if(info.height != VLF.CLS.Def.Const.unassignedIntValue)
				sql += ", Height=" + info.height;
			if(info.weight != VLF.CLS.Def.Const.unassignedIntValue)
				sql += ", Weight=" + info.weight;
			if(info.gender != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", Gender='" + info.gender.Replace("'","''") + "'";
			if(info.eyeColor != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", EyeColor='" + info.eyeColor.Replace("'","''") + "'";
			if(info.hairColor != VLF.CLS.Def.Const.unassignedStrValue)
			 	sql += ", HairColor='" + info.hairColor.Replace("'","''") + "'";
			if(info.idMarks != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", IdMarks='" + info.idMarks.Replace("'","''") + "'";
			if(info.certifications != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", Certifications='" + info.certifications.Replace("'","''") + "'";
			if(info.description != VLF.CLS.Def.Const.unassignedStrValue)
				sql += ", Description='" + info.description.Replace("'","''") + "'";
			sql += " WHERE PersonId='" + info.personId.Replace("'","''") + "'";
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update person info by person Id=" + info.personId + " .";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update person info by person Id=" + info.personId + " .";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update person info by person Id=" + info.personId + " .";
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This person does not exist.");
			}
			return rowsAffected;
		}		
		
        /// <summary>
		/// Update person information.
		/// </summary>
		/// <param name="personId"></param>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if person does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int UpdateInfo(string personId, string firstName, string lastName)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE vlfPersonInfo SET " +
				" FirstName='" + firstName.Replace("'","''") + "'" +
				", LastName='" + lastName.Replace("'","''") + "'" +
				" WHERE PersonId='" + personId.Replace("'","''") + "'";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 4. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update person info by person Id=" + personId + " .";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update person info by person Id=" + personId + " .";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update person info by person Id=" + personId + " .";
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This person does not exist.");
			}
			return rowsAffected;
		}		

		/// <summary>
		/// Retrieves user info
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [PersonId],[DriverLicense],[FirstName],[LastName],[MiddleName],
		/// [Birthday],[Address],[City],[StateProvince],[Country],
		/// [PhoneNo1],[PhoneNo2],[CellNo],[LicenseExpDate],[LicenseEndorsements],
		/// [Height],[Weight],[Gender],[EyeColor],[HairColor],[IdMarks],[Certifications],
		/// [Description]
		/// </returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetPersonInfoByDriverLicense(string driverLicense)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM vlfPersonInfo" + 
					" WHERE DriverLicense='" + driverLicense.Replace("'","''") + "'";
					
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}		

        /// <summary>
		/// Retrieves person info
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [PersonId],[DriverLicense],[FirstName],[LastName],[MiddleName],
		/// [Birthday],[Address],[City],[StateProvince],[Country],
		/// [PhoneNo1],[PhoneNo2],[CellNo],[LicenseExpDate],[LicenseEndorsements],
		/// [Height],[Weight],[Gender],[EyeColor],[HairColor],[IdMarks],[Certifications],
		/// [Description]
		/// </returns>
		/// <param name="personId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetPersonInfoByPersonId(string personId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM vlfPersonInfo" + 
					" WHERE PersonId='" + personId.Replace("'","''") + "'";
					
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by person Id=" + personId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by person Id=" + personId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}		
		
        /// <summary>
		/// Retrieves all persons info by organization id
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [PersonId],[DriverLicense],[FirstName],[LastName],[MiddleName],
		/// [Birthday],[Address],[City],[StateProvince],[Country],
		/// [PhoneNo1],[PhoneNo2],[CellNo],[LicenseExpDate],[LicenseEndorsements],
		/// [Height],[Weight],[Gender],[EyeColor],[HairColor],[IdMarks],[Certifications],
		/// [Description]
		/// </returns>
		/// <param name="organizationId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllPersonsInfoByOrganizationId(int organizationId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DISTINCT vlfPersonInfo.PersonId,DriverLicense,FirstName,LastName,MiddleName," +
							"Birthday,Address,City,StateProvince,Country,PhoneNo1,PhoneNo2," +
							"CellNo,LicenseExpDate,LicenseEndorsements,Height,Weight,Gender," +
							"EyeColor,HairColor,IdMarks,Certifications,vlfPersonInfo.Description" +
							" FROM vlfPersonInfo,vlfFleet,vlfFleetVehicles,vlfVehicleAssignment,vlfDriverAssignment" + 
							" WHERE vlfFleet.OrganizationId=" + organizationId +
							" AND vlfFleet.FleetId=vlfFleetVehicles.FleetId" +
							" AND vlfFleetVehicles.VehicleId=vlfVehicleAssignment.VehicleId" +
							" AND vlfVehicleAssignment.LicensePlate=vlfDriverAssignment.LicensePlate" +
							" AND vlfDriverAssignment.PersonId=vlfPersonInfo.PersonId";
					
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve all person info by organization id=" + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve all person info by organization id=" + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}		
		
        /// <summary>
		/// Retrieves all unassigned to organization persons info
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [PersonId],[DriverLicense],[FirstName],[LastName],[MiddleName],
		/// [Birthday],[Address],[City],[StateProvince],[Country],
		/// [PhoneNo1],[PhoneNo2],[CellNo],[LicenseExpDate],[LicenseEndorsements],
		/// [Height],[Weight],[Gender],[EyeColor],[HairColor],[IdMarks],[Certifications],
		/// [Description]
		/// </returns>
		/// <param name="organizationId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUnassignedPersonsInfoByOrganizationId(int organizationId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DISTINCT PersonId,DriverLicense,FirstName,LastName,MiddleName," +
					"Birthday,Address,City,StateProvince,Country,PhoneNo1,PhoneNo2," +
					"CellNo,LicenseExpDate,LicenseEndorsements,Height,Weight,Gender," +
					"EyeColor,HairColor,IdMarks,Certifications,Description" +
					" FROM vlfPersonInfo" +
					" WHERE PersonId NOT IN (SELECT DISTINCT vlfDriverAssignment.PersonId FROM vlfFleet,vlfFleetVehicles,vlfVehicleAssignment,vlfDriverAssignment" + 
											" WHERE vlfFleet.OrganizationId=" + organizationId +
											" AND vlfFleet.FleetId=vlfFleetVehicles.FleetId" +
											" AND vlfFleetVehicles.VehicleId=vlfVehicleAssignment.VehicleId" +
											" AND vlfVehicleAssignment.LicensePlate=vlfDriverAssignment.LicensePlate)";
					
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve all unassigned to organization drivers info by organization id=" + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve all unassigned to organization drivers info by organization id=" + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		
        /// <summary>
		/// Retrieves all persons ids
		/// </summary>
		/// <returns> ArrayList [PersonId] </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetAllPersonsIds()
		{
			ArrayList resultList = null;
			try
			{
				DataSet sqlDataSet = null;
				//Prepares SQL statement
				string sql = "SELECT PersonId FROM vlfPersonInfo";
					
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
				if((sqlDataSet != null)&&(sqlDataSet.Tables.Count > 0)&&(sqlDataSet.Tables[0].Rows.Count > 0))
				{
					resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
					//Retrieves info from Table[0].[0][0]
					foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
					{
						resultList.Add(Convert.ToString(currRow[0]).TrimEnd());
					}
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve all persons ids. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve all persons ids. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultList;
		}
		
        /// <summary>
		/// Retrieves all unassigned persons ids
		/// </summary>
		/// <returns> ArrayList [PersonId] </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetAllUnassignedPersonsIds()
		{
			ArrayList resultList = null;
			try
			{
				DataSet sqlDataSet = null;
				//Prepares SQL statement
				string sql = "SELECT PersonId" +
							" FROM vlfPersonInfo" +
							" WHERE PersonId NOT IN (SELECT PersonId FROM vlfUser)" +
							" ORDER BY PersonId";
					
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
				if((sqlDataSet != null)&&(sqlDataSet.Tables.Count > 0)&&(sqlDataSet.Tables[0].Rows.Count > 0))
				{
					resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
					//Retrieves info from Table[0].[0][0]
					foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
					{
						resultList.Add(Convert.ToString(currRow[0]).TrimEnd());
					}
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve all persons ids. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve all persons ids. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultList;
		}
		
        /// <summary>
		/// Delete existing person.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="personId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if person with person id not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeletePersonByPersonId(string personId)
		{
			return DeleteRowsByStrField("PersonId", personId, "person id");		
		}		
		
        /// <summary>
		/// Delete existing person.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if person with person id not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeletePersonByDriverLicense(string driverLicense)
		{
			return DeleteRowsByStrField("DriverLicense", driverLicense, "driver license");		
		}
		
        /// <summary>
		/// Checks if person with same driver license already exists.
		/// </summary>
		/// <returns>bool</returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool IsDriverLicenseExist(string driverLicense)
		{
			bool isExist = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfPersonInfo" + 
					" WHERE DriverLicense='" + driverLicense.Replace("'","''") + "'";
					
				//Executes SQL statement
				int retResult = (int)sqlExec.SQLExecuteScalar(sql);
				if(retResult > 0)
					isExist = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return isExist;
		}
	}
}
