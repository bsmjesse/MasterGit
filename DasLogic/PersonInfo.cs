using System;
using VLF.ERR;
using VLF.DAS.DB;
using System.Data;			// for DataSet
using System.Collections;
using System.Text;	// for ArrayList

namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to personal information functionality in database
	/// </summary>
	public class PersonInfo : Das
	{
		private VLF.DAS.DB.PersonInfo personInfo = null;

		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public PersonInfo(string connectionString) : base (connectionString)
		{
			personInfo = new VLF.DAS.DB.PersonInfo(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region Public Interfaces

		/// <summary>
		/// Add new person information.
		/// </summary>
		/// <param name="info"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver license or person alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddPerson(ref VLF.CLS.Def.Structures.PersonInfoStruct info)
		{
			personInfo.AddPerson(ref info);
		}

		/// <summary>
		/// Update person information.
		/// </summary>
		/// <param name="info"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if person does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int UpdateInfo(VLF.CLS.Def.Structures.PersonInfoStruct info)
		{
			return personInfo.UpdateInfo(info);
		}

		/// <summary>
		/// Delete existing person.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="personId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if person with person id does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeletePersonByPersonId(string personId)
		{
            int rowsAffected = 0;
            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 1. Delete all driver assignments
                DriverAssignment drvAss = new DriverAssignment(sqlExec);
                rowsAffected += drvAss.DeleteDriverAssignments(personId);

                // 2. Delete all the history of driver assignments
                DriverAssignmentHst drvAssHst = new DriverAssignmentHst(sqlExec);
                rowsAffected += drvAssHst.DeleteDriverAssignments(personId);

                // 3. Delete all trusted person info
                TrustedPerson tr = new TrustedPerson(sqlExec);
                rowsAffected += tr.DeleteTrustedPersonsByPersonId(personId);

                // 4. Delete the person info
                rowsAffected += personInfo.DeletePersonByPersonId(personId);

                sqlExec.CommitTransaction();
            }
            catch(Exception exc)
            {
                sqlExec.RollbackTransaction();
                rowsAffected = 0;
                throw new Exception("Cannot delete the person ID " + personId, exc);
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
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetPersonInfoByDriverLicense(string driverLicense)
		{
			DataSet dsResult = personInfo.GetPersonInfoByDriverLicense(driverLicense);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "PersonInfo" ;
				}
				dsResult.DataSetName = "Driver";
			}
			return dsResult;
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
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetAllPersonsInfo()
		{
			DataSet dsResult = personInfo.GetAllRecords();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllPersonsInfo" ;
				}
				dsResult.DataSetName = "Driver";
			}
			return dsResult;
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
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetAllPersonsInfoByOrganizationId(int organizationId)
		{
			DataSet dsResult = personInfo.GetAllPersonsInfoByOrganizationId(organizationId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllPersonsInfo" ;
				}
				dsResult.DataSetName = "Driver";
			}
			return dsResult;
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
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetAllUnassignedPersonsInfoByOrganizationId(int organizationId)
		{
			DataSet dsResult = personInfo.GetAllUnassignedPersonsInfoByOrganizationId(organizationId);
			if(dsResult != null)
			{
				if(dsResult.Tables.Count > 0)
				{
               dsResult.Tables[0].TableName = "AllPersonsInfo";
				}
				dsResult.DataSetName = "Driver";
			}
			return dsResult;
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
		/// <param name="personId"></param> 
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetPersonInfoByPersonId(string personId)
		{
			DataSet dsResult = personInfo.GetPersonInfoByPersonId(personId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "PersonInfo" ;
				}
				dsResult.DataSetName = "Driver";
			}
			return dsResult;
		}

		/// <summary>
		/// Retrieves all persons ids
		/// </summary>
		/// <returns> ArrayList [PersonId] </returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public ArrayList GetAllPersonsIds()
		{
			return personInfo.GetAllPersonsIds();
		}

		/// <summary>
		/// Retrieves all unassigned persons ids
		/// </summary>
		/// <returns> ArrayList [PersonId] </returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public ArrayList GetAllUnassignedPersonsIds()
		{
			return personInfo.GetAllUnassignedPersonsIds();
		}

      /// <summary>
      /// Retrieves all unassigned persons
      /// </summary>
      /// <returns> DataSet [PersonId, FirstName, LastName] </returns>
      /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
      public DataSet GetAllUnassignedPersons()
      {
         return personInfo.GetRowsBySql("SELECT PersonId, FirstName, LastName FROM vlfPersonInfo WHERE PersonId NOT IN (SELECT PersonId FROM vlfUser) ORDER BY PersonId");
      }

      /// <summary>
      /// Get all persons for organization
      /// </summary>
      /// <param name="orgId">Organization Id</param>
      /// <returns>Person info DataSet</returns>
      public DataSet GetAllPersonsInfo(object orgId)
      {
         StringBuilder sql = new StringBuilder();
         sql.AppendLine("SELECT vlfPersonInfo.PersonId, FirstName, LastName FROM vlfPersonInfo");
         sql.AppendLine("INNER JOIN vlfUser ON vlfUser.PersonId = vlfPersonInfo.PersonId");
         sql.AppendLine("WHERE vlfUser.OrganizationId = @orgId");
         return personInfo.GetRowsBySql(sql.ToString(), new System.Data.SqlClient.SqlParameter("@orgId", orgId));
      }

      #endregion
   }
}
