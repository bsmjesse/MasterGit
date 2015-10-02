using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;			// Enums

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfConfiguration table.
	/// </summary>
	public class Configuration : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Configuration(SQLExecuter sqlExec): base ("vlfConfiguration",sqlExec)
		{
		}	
		/// <summary>
		/// Retrieves configuration module types info
		/// </summary>
		/// <param name="typeId"></param>
		/// <param name="configurationModuleState"></param>
		/// <returns>DataSet [ModuleId],[ModuleName],[PSW],[IPAddress],[UserName],[Enabled],[MachineName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetConfigurationModuleTypesInfo(short typeId,VLF.CLS.Def.Enums.ConfigurationModuleState configurationModuleState)
		{
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT DISTINCT vlfConfiguration.ModuleId,vlfConfigurationModules.ModuleName,ISNULL(vlfConfiguration_PSW.KeyValue,'') AS PSW,ISNULL(vlfConfiguration_IP.KeyValue,'') AS IPAddress,ISNULL(vlfConfiguration_Usr.KeyValue,'') AS UserName,Enabled,ISNULL(vlfConfiguration_MN.KeyValue,'') AS MachineName"+
							" FROM   vlfConfiguration INNER JOIN vlfConfigurationModules ON vlfConfiguration.ModuleId = vlfConfigurationModules.ModuleId"+
							" LEFT JOIN vlfConfiguration vlfConfiguration_PSW ON vlfConfiguration.ModuleId = vlfConfiguration_PSW.ModuleId AND vlfConfiguration.CfgGroupId =vlfConfiguration_PSW.CfgGroupId AND vlfConfiguration_PSW.KeyName = 'WMI Password'"+
							" LEFT JOIN vlfConfiguration vlfConfiguration_IP ON vlfConfiguration.ModuleId = vlfConfiguration_IP.ModuleId AND vlfConfiguration.CfgGroupId =vlfConfiguration_IP.CfgGroupId AND vlfConfiguration_IP.KeyName = 'Machine Name'"+
							" LEFT JOIN vlfConfiguration vlfConfiguration_Usr ON vlfConfiguration.ModuleId = vlfConfiguration_Usr.ModuleId AND vlfConfiguration.CfgGroupId =vlfConfiguration_Usr.CfgGroupId AND vlfConfiguration_Usr.KeyName = 'WMI User Name'"+
							" LEFT JOIN vlfConfiguration vlfConfiguration_MN ON vlfConfiguration.ModuleId = vlfConfiguration_MN.ModuleId AND vlfConfiguration.CfgGroupId =vlfConfiguration_MN.CfgGroupId AND vlfConfiguration_MN.KeyName = 'Machine Name'"+
							" WHERE vlfConfiguration.CfgGroupId=1 ";   
							
				if(typeId != VLF.CLS.Def.Const.unassignedShortValue)
					sql += " AND vlfConfigurationModules.TypeId = " + typeId;

				if(configurationModuleState == VLF.CLS.Def.Enums.ConfigurationModuleState.Disabled)
					sql += " AND vlfConfigurationModules.Enabled = " + 0;
				else if(configurationModuleState == VLF.CLS.Def.Enums.ConfigurationModuleState.Enabled)
					sql += " AND vlfConfigurationModules.Enabled = " + 1;
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve configuration module types.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve configuration module types. " + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves configuration module type id
		/// </summary>
		/// <param name="moduleTypeName"></param>
		/// <returns>module id</returns>
		/// <remarks>If module does not exist, return VLF.CLS.Def.Const.unassignedShortValue,
		/// otherwise return module id</remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short GetConfigurationModuleTypeId(string moduleTypeName)
		{
			short moduleId = VLF.CLS.Def.Const.unassignedShortValue;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT ModuleId FROM vlfConfigurationModules"+
							" WHERE ModuleName='" + moduleTypeName + "'";
				// 2. Executes SQL statement
				moduleId = (short)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve configuration module type id by name " + moduleTypeName + ".", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve configuration module type id by name " + moduleTypeName + "." + objException.Message);
			}
			return moduleId;
		}


        /// <summary>
        /// Retrieves configuration module type id
        /// </summary>
        /// <param name="moduleTypeName"></param>
        /// <returns>module id</returns>
        /// <remarks>If module does not exist, return VLF.CLS.Def.Const.unassignedShortValue,
        /// otherwise return module id</remarks>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        //public short GetSLSIdByBoxID(int boxID)
        //{
        //    short slsId = VLF.CLS.Def.Const.unassignedShortValue;
        //    try
        //    {
        //        // 1. Prepares SQL statement
        //        string sql = "SELECT     dbo.SLSOrganizationAssignment.slsId FROM dbo.SLSOrganizationAssignment INNER JOIN "+
        //              " dbo.vlfBox ON dbo.SLSOrganizationAssignment.OrganizationId = dbo.vlfBox.BoxId=" + boxID;
        //        // 2. Executes SQL statement
        //        slsId = (short)sqlExec.SQLExecuteScalar(sql);
        //    }
        //    catch (SqlException objException)
        //    {
        //        Util.ProcessDbException("Unable to retieve SLS id by boxID: " + boxID + ".", objException);
        //    }
        //    catch (DASDbConnectionClosed exCnn)
        //    {
        //        throw new DASDbConnectionClosed(exCnn.Message);
        //    }
        //    catch (Exception objException)
        //    {
        //        throw new DASException("Unable to retieve SLS id by boxID " + boxID + "." + objException.Message);
        //    }
        //    return slsId;
        //}


		/// <summary>
		/// Retrieves configuration value
		/// </summary>
		/// <param name="moduleId"></param>
		/// <param name="cfgGroupId"></param>
		/// <param name="keyName"></param>
		/// <returns>string [KeyValue]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetConfigurationValue(short moduleId,short cfgGroupId,string keyName)
		{
			string retResult = "";
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT KeyValue FROM vlfConfiguration" +
						" WHERE ModuleId=" + moduleId +
						" AND CfgGroupId=" + cfgGroupId + 
						" AND KeyName='" + keyName + "'";
				// 2. Executes SQL statement
				object obj = sqlExec.SQLExecuteScalar(sql);
				if(obj != null)
					retResult = Convert.ToString(obj);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve configuration value.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve configuration value. " + objException.Message);
			}
			// 3. Return result
			return retResult;
		}
		/// <summary>
		/// Retrieves module configuration
		/// </summary>
		/// <param name="moduleId"></param>
		/// <returns>DataSet [CfgGroupId],[CfgGroupName],[KeyName],[KeyValue]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetModuleConfiguration(short moduleId)
		{
			DataSet dsResult = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT vlfConfiguration.CfgGroupId,CfgGroupName,KeyName,KeyValue"+
							" FROM vlfConfiguration INNER JOIN vlfConfigurationGroups ON vlfConfiguration.CfgGroupId = vlfConfigurationGroups.CfgGroupId" +
							" WHERE ModuleId=" + moduleId;
				// 2. Executes SQL statement
				dsResult = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve configuration value.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve configuration value. " + objException.Message);
			}
			// 3. Return result
			return dsResult;
		}
		/// <summary>
		/// Retrieves module configuration structure
		/// </summary>
		/// <param name="typeId"></param>
		/// <returns>DataSet [CfgGroupId],[KeyName],[KeyValue],[CfgGroupName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetModuleConfigurationStructure(short typeId)
		{
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT vlfConfigurationModuleTypeDefaults.CfgGroupId,KeyName,KeyValue,CfgGroupName"+
							" FROM vlfConfigurationModuleTypeDefaults INNER JOIN vlfConfigurationGroups ON dbo.vlfConfigurationModuleTypeDefaults.CfgGroupId = dbo.vlfConfigurationGroups.CfgGroupId";
					if(typeId != VLF.CLS.Def.Const.unassignedShortValue)
						sql += " WHERE TypeId=" + typeId;
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve default configuration for module type " + typeId + ".", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve default configuration for module type " + typeId + "." + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves configuration module types
		/// </summary>
		/// <returns>DataSet [TypeId],[TypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetConfigurationModuleTypesInfo()
		{
			DataSet dsResult = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT * FROM vlfConfigurationModuleTypes";
				// 2. Executes SQL statement
				dsResult = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve configuration groups info.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve configuration groups info. " + objException.Message);
			}
			// 3. Return result
			return dsResult;
		}
		/// <summary>
		/// Retrieves computer modules information
		/// </summary>
		/// <param name="moduleTypeId"></param>
		/// <param name="computerIp"></param>
		/// <returns>DataSet [ModuleId],[ModuleName],[Enabled],[TypeId],[TypeName]</returns>
		/// <remarks>If moduleTypeId==VLF.CLS.Def.Const.unassignedIntValue, 
		/// then doesn't include moduleTypeId into the filter </remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetComputerModulesInfo(short moduleTypeId,string computerIp)
		{
			DataSet dsResult = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT DISTINCT vlfConfigurationModules.ModuleId,ModuleName,Enabled,vlfConfigurationModules.TypeId,TypeName"+
							" FROM vlfConfigurationModules INNER JOIN vlfConfiguration ON vlfConfigurationModules.ModuleId=dbo.vlfConfiguration.ModuleId"+
							" INNER JOIN vlfConfigurationModuleTypes ON vlfConfigurationModules.TypeId=vlfConfigurationModuleTypes.TypeId"+
							" WHERE KeyName ='Machine Name'"+
							" AND KeyValue='" + computerIp + "'";
				if(moduleTypeId != VLF.CLS.Def.Const.unassignedIntValue)
					sql += " AND vlfConfigurationModules.TypeId=" + moduleTypeId;
				
				sql +=" ORDER BY ModuleName";
				// 2. Executes SQL statement
				dsResult = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve computer modules information.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve computer modules information. " + objException.Message);
			}
			// 3. Return result
			return dsResult;
		}
		/// <summary>
		/// Add new module.
		/// </summary>
		/// <param name="typeId"></param>
		/// <param name="moduleName"></param>
		/// <param name="enabled"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short AddModule(short typeId,string moduleName,bool enabled)
		{
			short nextFreeModuleId = GetMaxModuleId();
			try
			{
				// 1. Set SQL command
				string sql = "INSERT INTO vlfConfigurationModules(ModuleId,TypeId,ModuleName,Enabled) VALUES ( @ModuleId,@TypeId,@ModuleName,@Enabled)";
				// 2. Add parameters to SQL statement
				sqlExec.ClearCommandParameters();
				sqlExec.AddCommandParam("@ModuleId",SqlDbType.SmallInt,++nextFreeModuleId);
				sqlExec.AddCommandParam("@TypeId",SqlDbType.SmallInt,typeId);
				sqlExec.AddCommandParam("@ModuleName",SqlDbType.VarChar,moduleName);
				sqlExec.AddCommandParam("@Enabled",SqlDbType.SmallInt,Convert.ToInt16(enabled));
				
				
				if(sqlExec.RequiredTransaction())
				{
					// 3. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 4. Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new module " + moduleName;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new module " + moduleName;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return nextFreeModuleId;
		}	
		/// <summary>
		/// Update module status.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <param name="enable"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if module does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateModuleStatus(short moduleId,bool enable)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "UPDATE vlfConfigurationModules SET Enabled=" + Convert.ToInt16(enable) + 
					" WHERE ModuleId=" + moduleId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to update module " + moduleId + " status.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to update module " + moduleId + " status." + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				throw new DASAppResultNotFoundException("Unable to update module " + moduleId + " status.");
			}
		}	
		/// <summary>
		/// Add new module.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <param name="cfgGroupId"></param>
		/// <param name="keyName"></param>
		/// <param name="keyValue"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if module already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddModuleSettings(short moduleId,short cfgGroupId,string keyName,string keyValue)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Set SQL command
				string sql = "INSERT INTO vlfConfiguration(ModuleId,CfgGroupId,KeyName,KeyValue) VALUES ( @ModuleId,@CfgGroupId,@KeyName,@KeyValue)";
				// 2. Add parameters to SQL statement
				sqlExec.ClearCommandParameters();
				sqlExec.AddCommandParam("@ModuleId",SqlDbType.SmallInt,moduleId);
				sqlExec.AddCommandParam("@CfgGroupId",SqlDbType.SmallInt,cfgGroupId);
				sqlExec.AddCommandParam("@KeyName",SqlDbType.VarChar,keyName);
				sqlExec.AddCommandParam("@KeyValue",SqlDbType.VarChar,keyValue);
				
				if(sqlExec.RequiredTransaction())
				{
					// 3. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new module settings.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new module settings.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new module settings.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This module already exists.");
			}
		}	
		/// <summary>
		/// Retrieves module type groups.
		/// </summary>
		/// <param name="typeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>DataSet [CfgGroupId],[CfgGroupName]</returns>
		public DataSet GetModuleTypeGroups(short typeId)
		{
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT DISTINCT vlfConfigurationModuleTypeDefaults.CfgGroupId,CfgGroupName"+
					" FROM vlfConfigurationModuleTypeDefaults INNER JOIN vlfConfigurationGroups ON dbo.vlfConfigurationModuleTypeDefaults.CfgGroupId = dbo.vlfConfigurationGroups.CfgGroupId";
				if(typeId != VLF.CLS.Def.Const.unassignedShortValue)
					sql += " WHERE TypeId=" + typeId;
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve default configuration for module type " + typeId + ".", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve default configuration for module type " + typeId + "." + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}	
		/// <summary>
		/// Update module settings.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <param name="cfgGroupId"></param>
		/// <param name="keyName"></param>
		/// <param name="keyValue"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if module does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateModuleSettings(short moduleId,short cfgGroupId,string keyName,string keyValue)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "UPDATE vlfConfiguration SET KeyValue='" + keyValue + "'" +
							" WHERE ModuleId=" + moduleId +
							" AND CfgGroupId=" + cfgGroupId + 
							" AND KeyName='" + keyName + "'";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to update module " + moduleId + " settings.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to update module " + moduleId + " settings." + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				throw new DASAppResultNotFoundException("Unable to update module " + moduleId + " settings.");
			}
		}	
		/// <summary>
		/// Delete module.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteModule(short moduleId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM vlfConfigurationModules WHERE ModuleId=" + moduleId;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete module " + moduleId;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete module " + moduleId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Delete module settings.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteModuleSettings(short moduleId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM vlfConfiguration WHERE ModuleId=" + moduleId;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete module " + moduleId + " settings.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete module " + moduleId + " settings.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retrieves max module id
		/// </summary>
		/// <returns>max module id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		private short GetMaxModuleId()
		{
			short moduleId = VLF.CLS.Def.Const.unassignedShortValue;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT MAX(ModuleId) FROM vlfConfigurationModules";
				
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				
				// 3. Executes SQL statement
				moduleId = (short)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve max module id.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve max module id." + objException.Message);
			}
			return moduleId;
		}
		/// <summary>
		/// Set DayLight Savings.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetDayLightSaving(bool dayLightSaving)
		{
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfConfiguration SET KeyValue=KeyValue-1 WHERE KeyName like 'Time Shift'";
			if(dayLightSaving)
				sql = "UPDATE vlfConfiguration SET KeyValue=KeyValue+1 WHERE KeyName like 'Time Shift'";
				
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update dayLightSaving=" + dayLightSaving.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update dayLightSaving=" + dayLightSaving.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}



        public DataSet GetRules()
        {
            DataSet sqlDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "select RuleId,RuleName, RuleType from vlfRules ";
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve rules", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve rules. " + objException.Message);
            }
            // 3. Return result
            return sqlDataSet;
        }

	}
}
