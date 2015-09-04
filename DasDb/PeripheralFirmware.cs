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
    /// Provides interfaces to vlfPeripheralFirmware table.
    /// </summary>
    public class PeripheralFirmware : TblOneIntPrimaryKey
    {


        /*
                PeripheralFirmwareId	int	
                PeripheralTypeId	smallint	
                Major	tinyint	
                Minor	tinyint	
                Description	nvarchar(50)	
                BIN	image	      
         */

        const string tablename = "vlfPeripheralFirmware";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public PeripheralFirmware(SQLExecuter sqlExec) : base(tablename, sqlExec) { }




        /// <summary>
        /// Add new PeripheralFirmware  record
        /// </summary>
        /// <param name="peripheralTypeId"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="description"></param>
        /// <param name="bin"></param>
        public void AddPeripheralFirmware(Int16 peripheralTypeId, byte major, byte minor, string description, byte[] bin)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("INSERT INTO {0} ( PeripheralTypeId, Major, Minor, Description, BIN) VALUES (@PeripheralTypeId, @Major, @Minor, @Description, @BIN)", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.Int, peripheralTypeId);
                sqlExec.AddCommandParam("@Major", SqlDbType.TinyInt, major);
                sqlExec.AddCommandParam("@Minor", SqlDbType.TinyInt, minor);
                sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, description);
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
                string prefixMsg = string.Format("Unable to add new PeripheralFirmware for PeripheralType `{0}`.", peripheralTypeId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new PeripheralFirmware for PeripheralType `{0}`.", peripheralTypeId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new PeripheralFirmware for PeripheralType `{0}`.", peripheralTypeId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This firmware already exists.", prefixMsg));
            }
        }


        /// <summary>
        /// Delete existing PeripheralFirmware by peripheral type .
        /// </summary>
        /// <param name="peripheralTypeId"></param>
        /// <returns></returns>
        public int RemoveAllPeripheralFormwareByPeripheralTypeId(short peripheralTypeId)
        {
            return DeleteRowsByIntField("PeripheralTypeId", peripheralTypeId, "peripheral type");
        }

        /// <summary>
        /// Returns existing Peripheral Firmwares  by peripheral type
        /// </summary>
        /// <param name="peripheralType"></param>
        /// <returns>DataSet</returns>
        public DataSet GetPeripheralFirmwaresByPeripheralType(short peripheralType)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT PeripheralFirmwareId, PeripheralTypeId, Major, Minor, Description FROM {0} WHERE (PeripheralTypeId = @PeripheralTypeId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.SmallInt, peripheralType);

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral firmwares by peripheralType Id=" + peripheralType + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral firmwares by peripheralType Id=" + peripheralType + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return sqlDataSet;

        }

        /// <summary>
        /// Returns  Peripheral Firmware binary image  by peripheralFirmwareId
        /// </summary>
        /// <param name="peripheralFirmwareId"></param>
        /// <returns>DataSet</returns>
        public byte[] GetPeripheralFirmwaresBinary(int peripheralFirmwareId)
        {
            byte[] payload = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT BIN FROM {0} WHERE (PeripheralFirmwareId = @PeripheralFirmwareId)", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralFirmwareId", SqlDbType.Int, peripheralFirmwareId);

                //Executes SQL statement
                payload = (byte[])sqlExec.SQLExecuteScalar(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral firmware binary image by Peripheral Firmware Id=" + peripheralFirmwareId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Peripheral firmware binary image by Peripheral Firmware Id=" + peripheralFirmwareId + ". ";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return payload;

        }



        /// <summary>
        /// Returns  Peripheral Firmware binary image  by peripheralTypeId
        /// </summary>
        /// <param name="peripheralTypeId"></param>
        /// <returns>byte[]</returns>
        public byte[] GetLatestPeripheralFirmwareBinaryByPeripheralTypeId(short peripheralTypeId)
        {
            byte[] payload = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT TOP 1 BIN FROM {0} WHERE (PeripheralTypeId = @PeripheralTypeId) ORDER BY Major DESC, Minor DESC", tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PeripheralTypeId", SqlDbType.SmallInt, peripheralTypeId);

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
