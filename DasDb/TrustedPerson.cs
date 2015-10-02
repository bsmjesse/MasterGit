using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// vlfTrustedPersonInfo table structure
	/// </summary>
	public struct TrustedPersonInfo
	{
		public  int		userId;
		public	string	firstName;		// char 50
		public	string	lastName;		// char 50
		public	string	contactInfo;	// char 300
	}
	
	/// <summary>
	/// Provides interface to vlfTrustedPerson table.
	/// </summary>
	public class TrustedPerson : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public TrustedPerson(SQLExecuter sqlExec) : base ("vlfTrustedPerson", sqlExec)
		{
		}
		/// <summary>
		/// Add new TrustedPerson.
		/// </summary>
		/// <param name="driverLicense"></param>
		/// <param name="userInfo"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public void AddTrustedPerson(TrustedPersonInfo userInfo,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}
		/// <summary>
		/// Update trusted person information.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="userInfo"></param>
		/// <param name="driverLicense"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public void UpdateInfo(TrustedPersonInfo userInfo,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		/// <summary>
		/// Deletes existing user.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if user with driver license does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public int DeleteTrustedPersonByDriverLicense(string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}
		/// <summary>
		/// Delete existing user.
		/// </summary>
        /// <returns>Rows affected</returns>
		/// <param name="userId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if user with user id not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteTrustedPersonsByUserId(int userId)
        {
            return DeleteRowsByIntField("UserId", userId, "user id");
        }

        /// <summary>
        /// Delete trusted person by person id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Rows affected</returns>
        public int DeleteTrustedPersonsByPersonId(string personId)
        {
            return DeleteRowsByStrField("PersonId", personId, "person id");
        }

        /// <summary>
		/// Retrieves trusted persons info
		/// </summary>
		/// <returns>DataSet of all trusted persons related to specific user id</returns>
		/// <param name="userId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public DataSet GetTrustedPersonsInfoByUserId(int userId)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		/// <summary>
		/// Retrieves trusted person info
		/// </summary>
		/// <returns>DataSet with trusted person iformation</returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public DataSet GetTrustedPersonInfoByDriverLicense(string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		/// <summary>
		/// Returns user id by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public int GetUserIdByDriverLicense(string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		/// <summary>
		/// Returns first name by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public string GetFirstNameByDriverLicense(string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		/// <summary>
		/// Returns last name by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public string GetLastNameByDriverLicense(string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		/// <summary>
		/// Returns contact info by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public string GetContactInfoByDriverLicense(string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		/// <summary>
		/// Returns driver license by user id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>ArrayList of all drivers license related to this user id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public ArrayList GetDriverLicensesByUserId(int userId)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}		
		
		/// <summary>
		/// Updates user id by driver license
		/// </summary>
		/// <param name="userId"></param> 
		/// <param name="driverLicense"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public void SetUserIdByDriverLicense(int userId,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}
		/// <summary>
		/// Updates first name by driver license
		/// </summary>
		/// <param name="firstName"></param> 
		/// <param name="driverLicense"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public void SetFirstNameByDriverLicense(string firstName,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}
		/// <summary>
		/// Updates last name by driver license
		/// </summary>
		/// <param name="lastName"></param> 
		/// <param name="driverLicense"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public void SetLastNameByDriverLicense(string lastName,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
		}
		/// <summary>
		/// Updates contact info by driver license
		/// </summary>
		/// <param name="contactInfo"></param> 
		/// <param name="driverLicense"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		public void SetContactInfoByDriverLicense(string contactInfo,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
			//SetFieldByDriverLicense("ContactInfo",contactInfo,driverLicense);
		}
		/// <summary>
		/// Retrieves user info
		/// </summary>
		/// <param name="searchFieldName"></param> 
		/// <param name="resultFieldName"></param> 
		/// <param name="searchFieldValue"></param> 
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		protected object GetUserInfoFieldBy(string searchFieldName,string resultFieldName,int searchFieldValue)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
			/*object resultFieldValue = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + resultFieldName + " FROM " + tableName + 
					" WHERE " + searchFieldName + "=" + searchFieldValue;
				//Executes SQL statement
				resultFieldValue = sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultFieldValue;
			*/
		}
		/// <summary>
		/// Retrieves user info
		/// </summary>
		/// <param name="searchFieldName"></param> 
		/// <param name="resultFieldName"></param> 
		/// <param name="searchFieldValue"></param> 
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		protected object GetUserInfoFieldBy(string searchFieldName,string resultFieldName,string searchFieldValue)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
			/*object resultFieldValue = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + resultFieldName + " FROM " + tableName + 
					" WHERE (" + searchFieldName + "='" + searchFieldValue + "')";
				//Executes SQL statement
				//sqlDataSet = sqlExec.SQLExecuteDataset(sql);
				resultFieldValue = sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultFieldValue;
			*/
		}
		/// <summary>
		/// Retrieves user info
		/// </summary>
		/// <param name="searchFieldName"></param> 
		/// <param name="searchFieldValue"></param> 
		/// <returns>DataSet</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		protected DataSet GetUserInfoBy(string searchFieldName,int searchFieldValue)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
			/*DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM " + tableName + 
					" WHERE " + searchFieldName + "=" + searchFieldValue;
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
			*/
		}
		/// <summary>
		/// Retrieves user info
		/// </summary>
		/// <param name="searchFieldName"></param> 
		/// <param name="searchFieldValue"></param> 
		/// <returns>DataSet</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		protected DataSet GetUserInfoBy(string searchFieldName,string searchFieldValue)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
			/*DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM " + tableName + 
					" WHERE (" + searchFieldName + "='" + searchFieldValue + "')";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
			*/
		}
		/// <summary>
		/// Updates field by driver license
		/// </summary>
		/// <param name="updateFieldName"></param>
		/// <param name="updateFieldValue"></param>
		/// <param name="driverLicense"></param>
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		protected void SetFieldByDriverLicense(string updateFieldName,string updateFieldValue,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
			/*int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET " + updateFieldName + "='" + updateFieldValue + "'" +
					" WHERE DriverLicense='" + driverLicense + "'";
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set " + updateFieldName + "=" + updateFieldValue + " by DriverLicense=" + driverLicense + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set " + updateFieldName + "=" + updateFieldValue + " by DriverLicense=" + driverLicense + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set " + updateFieldName + "=" + updateFieldValue + " by DriverLicense=" + driverLicense + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong user driver license='" + driverLicense  + "'.");
			}
			*/
		}
		/// <summary>
		/// Updates field by driver license
		/// </summary>
		/// <param name="updateFieldName"></param>
		/// <param name="updateFieldValue"></param>
		/// <param name="driverLicense"></param>
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <remarks>Not implemented</remarks>
		protected void SetFieldByDriverLicense(string updateFieldName,int updateFieldValue,string driverLicense)
		{
			throw new DASAppViolatedIntegrityConstraintsException("Not implemented");
			/*int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET " + updateFieldName + "=" + updateFieldValue + 
					" WHERE DriverLicense='" + driverLicense + "'";
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set " + updateFieldName + "=" + updateFieldValue + " by DriverLicense=" + driverLicense + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set " + updateFieldName + "=" + updateFieldValue + " by DriverLicense=" + driverLicense + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set " + updateFieldName + "=" + updateFieldValue + " by DriverLicense=" + driverLicense + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong user driver license='" + driverLicense  + "'.");
			}
			*/
		}
	}
}
