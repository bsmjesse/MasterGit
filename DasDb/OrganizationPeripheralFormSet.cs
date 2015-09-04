using System;
using System.Collections.Generic;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using System.Text;

namespace VLF.DAS.DB
{

    /// <summary>
    /// Provides interfaces to vlfMdtOTA table.
    /// </summary>
    public class OrganizationPeripheralFormset : TblOneIntPrimaryKey
    {


        /*
            OrganizationPeripheralFormsetId	int	
            OrganizationId	int	
            PeripheralTypeId	int	
            Description	nvarchar(255)	
        */

        const string tablename = "vlfOrganizationPeripheralFormset";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public OrganizationPeripheralFormset(SQLExecuter sqlExec) : base(tablename, sqlExec) { }

        /// <summary>
        /// Add new OrganizationPeripheralFormset  record
        /// </summary>
        /// <param name="item"></param>
        public void AddOrganizationPeripheralFormset(VLF.CLS.Def.Structures.OrganizationPeripheralFormset item)
        {
            AddOrganizationPeripheralFormset(
                item.organizationId,
                item.peripheralTypeId,
                item.description,
                item.title,
                item.gmtOffset,
                item.autoDaylightSavings,
                item.blankInMotion);

            }
        /// <summary>
            /// Add new OrganizationPeripheralFormset  record
            /// </summary>
        /// <param name="orgId"></param>
        /// <param name="peripheralTypeId"></param>
        /// <param name="description"></param>
        public void AddOrganizationPeripheralFormset(uint orgId, ushort peripheralTypeId, string description, string homescreenTitle, short gmtOffset, bool autoDaylightsavingsAdjust, bool blankInMotion)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("INSERT INTO {0} (OrganizationId, PeripheralTypeId, Description, HomeScreenTitle, GmtOffset, AutoDaylightSavings, BlankInMotion) VALUES ( @OrganizationId, @PeripheralTypeId, @Description, @HomeScreenTitle, @GmtOffset, @AutoDaylightSavings, @BlankInMotion)", tableName);
                //INSERT INTO vlfOrganizationPeripheralFormset (OrganizationPeripheralFormsetId, OrganizationId, PeripheralTypeId, Description) VALUES 

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralTypeId);
                sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, description);
                sqlExec.AddCommandParam("@HomeScreenTitle", SqlDbType.NVarChar, homescreenTitle);
                sqlExec.AddCommandParam("@GmtOffset", SqlDbType.Int, gmtOffset);
                sqlExec.AddCommandParam("@AutoDaylightSavings", SqlDbType.Bit, autoDaylightsavingsAdjust ? 1 : 0);
                sqlExec.AddCommandParam("@BlankInMotion", SqlDbType.Bit, blankInMotion ? 1 : 0);
                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' OrganizationPeripheralFormset.", orgId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' OrganizationPeripheralFormset.", orgId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' OrganizationPeripheralFormset.", orgId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This formset already exists.", prefixMsg));
            }
        }


        /// <summary>
        /// update OrganizationPeripheralFormset  record
        /// </summary>
        /// <param name="item"></param>
        public void UpdateOrganizationPeripheralFormset(VLF.CLS.Def.Structures.OrganizationPeripheralFormset item)
        {
            UpdateOrganizationPeripheralFormset(
                item.organizationPeripheralFormsetId,
                item.organizationId,
                item.peripheralTypeId,
                item.description,
                item.title,
                item.gmtOffset,
                item.autoDaylightSavings,
                item.blankInMotion);

        }
        
        
        /// <summary>
        /// Update OrganizationPeripheralFormset  record
        /// </summary>
        /// <param name="organizationPeripheralFormsetId"></param>
        /// <param name="orgId"></param>
        /// <param name="peripheralTypeId"></param>
        /// <param name="description"></param>
        /// <param name="homescreenTitle"></param>
        /// <param name="gmtOffset"></param>
        /// <param name="autoDaylightsavingsAdjust"></param>
        /// <param name="blankInMotion"></param>
        public void UpdateOrganizationPeripheralFormset(uint organizationPeripheralFormsetId, uint orgId, ushort peripheralTypeId, string description, string homescreenTitle, short gmtOffset, bool autoDaylightsavingsAdjust, bool blankInMotion)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("UPDATE {0} SET OrganizationId = @OrganizationId, PeripheralTypeId = @PeripheralTypeId, Description = @Description, HomeScreenTitle = @HomeScreenTitle, GmtOffset = @GmtOffset, AutoDaylightSavings = @AutoDaylightSavings, BlankInMotion = @BlankInMotion WHERE (OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId)", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralTypeId);
                sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, description);
                sqlExec.AddCommandParam("@HomeScreenTitle", SqlDbType.NVarChar, homescreenTitle);
                sqlExec.AddCommandParam("@GmtOffset", SqlDbType.Int, gmtOffset);
                sqlExec.AddCommandParam("@AutoDaylightSavings", SqlDbType.Bit, autoDaylightsavingsAdjust ? 1 : 0);
                sqlExec.AddCommandParam("@BlankInMotion", SqlDbType.Bit, blankInMotion ? 1 : 0);
                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to update '{0}' OrganizationPeripheralFormset.", organizationPeripheralFormsetId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to update '{0}' OrganizationPeripheralFormset.", organizationPeripheralFormsetId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to update '{0}' OrganizationPeripheralFormset.", organizationPeripheralFormsetId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This formset already exists.", prefixMsg));
            }
        }


        /// <summary>
        /// Remove Organization PeripheralFormset
        /// </summary>
        /// <param name="organizationPeripheralFormsetId"></param>
        public void RemoveOrganizationPeripheralFormset(uint organizationPeripheralFormsetId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = string.Format("DELETE FROM vlfPeripheralFormset WHERE (OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to remove form linkages for OrganizationPeripheralFormset '{0}' .", organizationPeripheralFormsetId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove form linkages for OrganizationPeripheralFormset '{0}' .", organizationPeripheralFormsetId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }



            try
            {
                string sql = string.Format("DELETE FROM {0} WHERE (OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' OrganizationPeripheralFormset.", organizationPeripheralFormsetId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' OrganizationPeripheralFormset.", organizationPeripheralFormsetId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
        }


        /// <summary>
        /// Delete existing OrganizationPeripheralFormset .
        /// Throws exception in case of wrong result (see TblOneIntPrimaryKey class).
        /// </summary>
        /// <param name="orgId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int RemoveAllOrganizationPeripheralFormsets(uint orgId)
        {
            return DeleteRowsByIntField("OrganizationId", orgId, "org");
        }

        /// <summary>
        /// Returns existing Peripheral Formset Definitions by peripheral type
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="peripheralType"></param>
        /// <returns>DataSet</returns>
        public DataSet GetOrganizationPeripheralFormsetsByPeripheralType(uint organization, ushort peripheralType)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT OrganizationPeripheralFormsetId, OrganizationId, PeripheralTypeId, Description, HomeScreenTitle, GmtOffset, AutoDaylightSavings, BlankInMotion FROM {0} WHERE (PeripheralTypeId = @PeripheralTypeId AND OrganizationId = @OrganizationId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralType);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organization);

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve PeripheralFormsets by organization=" + organization + " and peripheralType Id=" + peripheralType + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve PeripheralFormsets by organization=" + organization + " and peripheralType Id=" + peripheralType + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return sqlDataSet;

        }


        /// <summary>
        /// Returns existing Peripheral Formset Definitions by peripheral type
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="peripheralType"></param>
        /// <returns>DataSet</returns>
        public List<VLF.CLS.Def.Structures.OrganizationPeripheralFormset> GetListOfOrganizationPeripheralFormsetsByPeripheralType(uint organization, ushort peripheralType)
        {
            List<VLF.CLS.Def.Structures.OrganizationPeripheralFormset> items = new List<VLF.CLS.Def.Structures.OrganizationPeripheralFormset>();
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT OrganizationPeripheralFormsetId, OrganizationId, PeripheralTypeId, Description, HomeScreenTitle, GmtOffset, AutoDaylightSavings, BlankInMotion FROM {0} WHERE (PeripheralTypeId = @PeripheralTypeId AND OrganizationId = @OrganizationId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralType);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organization);

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                {
                    foreach (DataRow dr in sqlDataSet.Tables[0].Rows)
                    {
                        VLF.CLS.Def.Structures.OrganizationPeripheralFormset item = new VLF.CLS.Def.Structures.OrganizationPeripheralFormset();
                        item.organizationPeripheralFormsetId = Convert.ToUInt32(dr["OrganizationPeripheralFormsetId"]);
                        item.organizationId = Convert.ToUInt32(dr["OrganizationId"]);
                        item.peripheralTypeId = Convert.ToUInt16(dr["PeripheralTypeId"]);
                        item.description = Convert.ToString(dr["Description"]);
                        item.title = Convert.ToString(dr["HomeScreenTitle"]);
                        item.gmtOffset = Convert.ToInt16(dr["GmtOffset"]);
                        item.autoDaylightSavings = Convert.ToBoolean(dr["AutoDaylightSavings"]);
                        item.blankInMotion = Convert.ToBoolean(dr["BlankInMotion"]);
                        items.Add(item);
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve PeripheralFormsets by organization=" + organization + " and peripheralType Id=" + peripheralType + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve PeripheralFormsets by organization=" + organization + " and peripheralType Id=" + peripheralType + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return items;

        }

        /// <summary>
        /// Returns existing Peripheral Formset Definitions by id
        /// </summary>
        /// <param name="organizationPeripheralFormsetId"></param>
        /// <returns>DataSet</returns>
        public VLF.CLS.Def.Structures.OrganizationPeripheralFormset GetOrganizationPeripheralFormset(uint organizationPeripheralFormsetId)
        {
            VLF.CLS.Def.Structures.OrganizationPeripheralFormset item = new VLF.CLS.Def.Structures.OrganizationPeripheralFormset();
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT OrganizationPeripheralFormsetId, OrganizationId, PeripheralTypeId, Description, HomeScreenTitle, GmtOffset, AutoDaylightSavings, BlankInMotion FROM {0} WHERE (OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                {
                    foreach (DataRow dr in sqlDataSet.Tables[0].Rows)
                    {
                        item.organizationPeripheralFormsetId = Convert.ToUInt32(dr["OrganizationPeripheralFormsetId"]);
                        item.organizationId = Convert.ToUInt32(dr["OrganizationId"]);
                        item.peripheralTypeId = Convert.ToUInt16(dr["PeripheralTypeId"]);
                        item.description = Convert.ToString(dr["Description"]);
                        item.title = Convert.ToString(dr["HomeScreenTitle"]);
                        item.gmtOffset = Convert.ToInt16(dr["GmtOffset"]);
                        item.autoDaylightSavings = Convert.ToBoolean(dr["AutoDaylightSavings"]);
                        item.blankInMotion = Convert.ToBoolean(dr["BlankInMotion"]);
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve PeripheralFormset by id=" + organizationPeripheralFormsetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve PeripheralFormset by id=" + organizationPeripheralFormsetId + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return item;

        }

        /// <summary>
        /// Add PeripheralForm To OrganizationPeripheralFormset
        /// </summary>
        /// <param name="organizationPeripheralFormsetId"></param>
        /// <param name="peripheralFormId"></param>
        /// <param name="index"></param>
        public void AddPeripheralFormToOrganizationPeripheralFormset(uint organizationPeripheralFormsetId, uint peripheralFormId, byte index)
        {
            int rowsAffected = 0;
            try
            {

                // 1. Set SQL command
                string sql = "UPDATE vlfPeripheralFormset SET FormIndex = FormIndex + 1 WHERE OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId AND FormIndex >= @FormIndex";
                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);
                sqlExec.AddCommandParam("@FormIndex", SqlDbType.TinyInt, index);
                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

                rowsAffected = 0;

                // 1. Set SQL command
                sql = "INSERT INTO vlfPeripheralFormset (OrganizationPeripheralFormsetId, PeripheralFormId, FormIndex) VALUES ( @OrganizationPeripheralFormsetId, @PeripheralFormId, @FormIndex)";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);
                sqlExec.AddCommandParam("@PeripheralFormId", SqlDbType.Int, peripheralFormId);
                sqlExec.AddCommandParam("@FormIndex", SqlDbType.TinyInt, index);
                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to add new peripheralFormId '{0}' link for OrganizationPeripheralFormsetId '{1}'. ", peripheralFormId, organizationPeripheralFormsetId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new peripheralFormId '{0}' link for OrganizationPeripheralFormsetId '{1}'. ", peripheralFormId, organizationPeripheralFormsetId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new peripheralFormId '{0}' link for OrganizationPeripheralFormsetId '{1}'. ", peripheralFormId, organizationPeripheralFormsetId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This formset already exists.", prefixMsg));
            }
        }

        /// <summary>
        /// Remove PeripheralForm from OrganizationPeripheralFormset
        /// </summary>
        /// <param name="organizationPeripheralFormsetId"></param>
        /// <param name="peripheralFormId"></param>
        /// <param name="index"></param>
        public void RemovePeripheralFormFromOrganizationPeripheralFormset(uint organizationPeripheralFormsetId, uint peripheralFormId, byte index)
        {
            try
            {
                string sql = @"DELETE FROM vlfPeripheralFormset WHERE (OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId AND PeripheralFormId = @PeripheralFormId)";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);
                sqlExec.AddCommandParam("@PeripheralFormId", SqlDbType.Int, peripheralFormId);

                int rowsaffected = sqlExec.SQLExecuteNonQuery(sql);
                if (rowsaffected > 0)
                {
                    sql = "UPDATE vlfPeripheralFormset SET FormIndex = FormIndex - 1 WHERE OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId AND FormIndex > @FormIndex";
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);
                    sqlExec.AddCommandParam("@FormIndex", SqlDbType.TinyInt, index);
                    rowsaffected = sqlExec.SQLExecuteNonQuery(sql);
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to remove Peripheral Form link for organizationPeripheralFormsetId=" + organizationPeripheralFormsetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to remove Peripheral Form link for organizationPeripheralFormsetId=" + organizationPeripheralFormsetId + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }

        }
    }




}
