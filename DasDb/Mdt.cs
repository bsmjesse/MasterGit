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
    /// Provides interfaces to vlfMdt table.
    /// </summary>
    public class Mdt : TblOneIntPrimaryKey
    {
        const string tablename = "vlfMdt";

        /*
        MdtId	int	
        FirmwareVersion	nvarchar(6)	
        SerialNum	nvarchar(32)	
        Description	nvarchar(100)	
        MdtType	int	         
         * * */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Mdt(SQLExecuter sqlExec) : base("vlfMdt", sqlExec) { }


        /// <summary>
        /// Add new Mdt record
        /// </summary>
        /// <param name="firmwareVersion"></param>
        /// <param name="serialNum"></param>
        /// <param name="description"></param>
        /// <param name="mdtType"></param>
        public void AddMdt(string firmwareVersion, string serialNum, string description, int mdtType)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("INSERT INTO {0} ( FirmwareVersion, SerialNum, Description, MdtType) VALUES ( @FirmwareVersion, @SerialNum, @Description, @MdtType )", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FirmwareVersion", SqlDbType.VarChar, firmwareVersion);
                sqlExec.AddCommandParam("@SerialNum", SqlDbType.VarChar, serialNum);
                sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
                sqlExec.AddCommandParam("@MdtType", SqlDbType.Int, mdtType);

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
                string prefixMsg = string.Format("Unable to add new Mdt with serial number '{0}'.", serialNum);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new Mdt with serial number '{0}'.", serialNum);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new Mdt with serial number '{0}'.", serialNum);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This Mdt already exists.", prefixMsg));
            }
        }

        /// <summary>
        /// Remove existing Mdt record by serial number and type
        /// </summary>
        /// <param name="serialNum"></param>
        public void RemoveMdt(string serialNum, int mdtType)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("DELETE FROM {0} WHERE SerialNum = @SerialNum AND MdtType = @MdtType", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@SerialNum", SqlDbType.VarChar, serialNum);
                sqlExec.AddCommandParam("@MdtType", SqlDbType.Int, mdtType);

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
                string prefixMsg = string.Format("Unable to remove Mdt with type '{0}' serial number '{1}'.", mdtType, serialNum);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove Mdt with type '{0}' serial number '{1}'.", mdtType, serialNum);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove Mdt with type '{0}' serial number '{1}'.", mdtType, serialNum);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by SerialNum not found.", prefixMsg));
            }

        }

        /// <summary>
        /// Remove existing Mdt record by id
        /// </summary>
        /// <param name="identifier"></param>
        public void RemoveMdt(int identifier)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("DELETE FROM {0} WHERE MdtId = @MdtId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@MdtId", SqlDbType.VarChar, identifier);

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
                string prefixMsg = string.Format("Unable to remove '{0}' Mdt .", identifier);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' Mdt.", identifier);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' Mdt", identifier);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by MdtId not found.", prefixMsg));
            }

        }



        /// <summary>
        /// Update Mdt
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="firmwareVersion"></param>
        /// <param name="serialNum"></param>
        /// <param name="description"></param>
        /// <param name="mdtType"></param>
        public void UpdateMdt(int identifier, string firmwareVersion, string serialNum, string description, int mdtType)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("UPDATE {0} SET Firmware = @Firmware, SerialNum = @SerialNum, Description = @Description, MdtType = @MdtType WHERE MdtId = @MdtId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@MdtId", SqlDbType.Int, identifier);
                sqlExec.AddCommandParam("@FirmwareVersion", SqlDbType.VarChar, firmwareVersion);
                sqlExec.AddCommandParam("@SerialNum", SqlDbType.VarChar, serialNum);
                sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, description);
                sqlExec.AddCommandParam("@MdtType", SqlDbType.Int, mdtType);

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
                string prefixMsg = string.Format("Unable to update '{0}' Mdt.", identifier);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to update '{0}' Mdt.", identifier);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to update '{0}' Mdt.", identifier);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by MdtId not found.", prefixMsg));
            }
        }


        public MdtClass GetMdtType(int identifier)
        {
            MdtClass mdt = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT * FROM {0} WHERE MdtId = @MdtId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@MdtId", SqlDbType.Int, identifier);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement

                using (DataSet ds = sqlExec.SQLExecuteDataset(sql))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        if (dr != null)
                        {
                            mdt = new MdtClass();
                            mdt.Id = Convert.ToInt32(dr["MdtId"]);
                            mdt.FirmwareVersion = Convert.ToString(dr["FirmwareVersion"]);
                            mdt.SerialNumber = Convert.ToString(dr["SerialNum"]);
                            mdt.Description = Convert.ToString(dr["Description"]);
                            mdt.MdtType = Convert.ToInt32(dr["MdtType"]);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to update '{0}' MdtType.", identifier);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to update '{0}' MdtType.", identifier);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }

            return mdt;
        }


        public MdtClass GetMdtType(string serialNum, int mdtType)
        {
            MdtClass mdt = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT * FROM {0} WHERE SerialNum = @SerialNum AND MdtType = @MdtType", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@SerialNum", SqlDbType.VarChar, serialNum);
                sqlExec.AddCommandParam("@MdtType", SqlDbType.Int, mdtType);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement

                using (DataSet ds = sqlExec.SQLExecuteDataset(sql))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        if (dr != null)
                        {
                            mdt = new MdtClass();
                            mdt.Id = Convert.ToInt32(dr["MdtId"]);
                            mdt.FirmwareVersion = Convert.ToString(dr["FirmwareVersion"]);
                            mdt.SerialNumber = Convert.ToString(dr["SerialNum"]);
                            mdt.Description = Convert.ToString(dr["Description"]);
                            mdt.MdtType = Convert.ToInt32(dr["MdtType"]);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve Mdt with type '{0}' serial number '{1}'.", mdtType, serialNum);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve Mdt with type '{0}' serial number '{1}'.", mdtType, serialNum);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return mdt;
        }
    }


    public class MdtClass
    {
        /// <summary>
        /// MdtId
        /// </summary>
        public int Id;

        /// <summary>
        /// FirmwareVersion
        /// </summary>
        public string FirmwareVersion;

        /// <summary>
        /// SerialNumber
        /// </summary>
        public string SerialNumber;

        /// <summary>
        /// Description
        /// </summary>
        public string Description;

        /// <summary>
        /// MdtType
        /// </summary>
        public int MdtType;

    }



}
