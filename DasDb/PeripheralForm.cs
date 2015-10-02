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
    /// Provides interfaces to vlfPeripheralForm table.
    /// </summary>
    public class PeripheralForm : TblOneIntPrimaryKey
    {

        /*
            PeripheralFormId	int	
            PeripheralTypeId	int	
            Description	nvarchar(120)	
            Modality	tinyint	
            Version	tinyint	
            IsThirdParty	bit	
            BIN	image	    
         */

        const string tablename = "vlfPeripheralForm";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public PeripheralForm(SQLExecuter sqlExec) : base(tablename, sqlExec) { }




        /// <summary>
        /// Add new PeripheralForm  record
        /// </summary>
        /// <param name="peripheralTypeId"></param>
        /// <param name="description"></param>
        /// <param name="modality"></param>
        /// <param name="version"></param>
        /// <param name="isThirdParty"></param>
        /// <param name="bin"></param>
        public void AddPeripheralForm(ushort peripheralTypeId, string description, ushort identifier,  byte modality, byte version, bool isThirdParty, byte[] bin)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("INSERT INTO {0} ( PeripheralTypeId, Description, Identifier, Modality, Version, IsThirdParty, BIN) VALUES (@PeripheralTypeId, @Description, @Identifier, @Modality, @Version, @IsThirdParty, @BIN)", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralTypeId);
                sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
                sqlExec.AddCommandParam("@Identifier", SqlDbType.Int, identifier);
                sqlExec.AddCommandParam("@Modality", SqlDbType.TinyInt, modality);
                sqlExec.AddCommandParam("@Version", SqlDbType.TinyInt, version);
                sqlExec.AddCommandParam("@IsThirdParty", SqlDbType.Bit, isThirdParty ? 1 : 0);
                sqlExec.AddCommandParam("@BIN", SqlDbType.Image, bin);
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
                string prefixMsg = string.Format("Unable to add new PeripheralForm for PeripheralType `{0}`.", peripheralTypeId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new PeripheralForm for PeripheralType `{0}`.", peripheralTypeId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new PeripheralForm for PeripheralType `{0}`.", peripheralTypeId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This firmware already exists.", prefixMsg));
            }
        }




        /// <summary>
        /// Returns existing Peripheral Forms  by peripheral type
        /// </summary>
        /// <param name="peripheralType"></param>
        /// <returns>DataSet</returns>
        public DataSet GetPeripheralFormsByPeripheralType(ushort peripheralType)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT PeripheralFormId, PeripheralTypeId, Description, Identifier, Modality, Version, IsThirdParty FROM {0} WHERE (PeripheralTypeId = @PeripheralTypeId) ORDER BY Identifier", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralType);

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral forms by peripheralType Id=" + peripheralType + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral forms by peripheralType Id=" + peripheralType + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return sqlDataSet;

        }


        /// <summary>
        /// Returns existing Peripheral Forms  by organization Peripheral Formset Id
        /// </summary>
        /// <param name="organizationPeripheralFormsetId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetPeripheralFormsByPeripheralFormsetId(uint organizationPeripheralFormsetId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = @"SELECT vlfPeripheralForm.Description, vlfPeripheralForm.Identifier, vlfPeripheralForm.Modality, vlfPeripheralForm.Version, vlfPeripheralForm.IsThirdParty, vlfPeripheralForm.BIN, vlfPeripheralForm.PeripheralFormId, vlfPeripheralForm.PeripheralTypeId FROM vlfPeripheralForm RIGHT OUTER JOIN vlfPeripheralFormset ON vlfPeripheralForm.PeripheralFormId = vlfPeripheralFormset.PeripheralFormId WHERE (vlfPeripheralFormset.OrganizationPeripheralFormsetId = @OrganizationPeripheralFormsetId) ORDER BY vlfPeripheralFormset.FormIndex";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationPeripheralFormsetId", SqlDbType.Int, organizationPeripheralFormsetId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral Forms by organizationPeripheralFormsetId=" + organizationPeripheralFormsetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral Forms by organizationPeripheralFormsetId=" + organizationPeripheralFormsetId + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return sqlDataSet;

        }

        
        /// <summary>
        /// Returns  Peripheral Firmware binary image  by PeripheralFormId
        /// </summary>
        /// <param name="PeripheralFormId"></param>
        /// <returns>DataSet</returns>
        public byte[] GetPeripheralFormsBinary(uint PeripheralFormId)
        {
            byte[] payload = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT BIN FROM {0} WHERE (PeripheralFormId = @PeripheralFormId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralFormId", SqlDbType.Int, PeripheralFormId);

                //Executes SQL statement
                payload = (byte[])sqlExec.SQLExecuteScalar(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral form binary image by PeripheralFormId=" + PeripheralFormId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral form binary image by PeripheralFormId=" + PeripheralFormId + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return payload;

        }



        /// <summary>
        /// Returns  Peripheral Firmware binary image  by peripheralTypeId
        /// </summary>
        /// <param name="peripheralTypeId"></param>
        /// <returns>byte[]</returns>
        public byte[] GetLatestPeripheralFormBinaryByPeripheralTypeId(ushort peripheralTypeId)
        {
            byte[] payload = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT TOP 1 BIN FROM {0} WHERE (PeripheralTypeId = @PeripheralTypeId) ORDER BY Major DESC, Minor DESC", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralTypeId);

                //Executes SQL statement
                payload = (byte[])sqlExec.SQLExecuteScalar(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve latest Peripheral firmware binary image by PeripheralType Id=" + peripheralTypeId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve latest Peripheral firmware binary image by PeripheralType Id=" + peripheralTypeId + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return payload;

        }
    }



}
