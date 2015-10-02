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
    /// Provides interfaces to vlfMdtType table.
    /// </summary>
    public class MdtType : TblOneIntPrimaryKey
    {
        const string tablename = "vlfMdtType";

        /*
        MdtTypeId	int	
        Manufacturer	nvarchar(50)	
        Model	nvarchar(50)	        
         * * */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public MdtType(SQLExecuter sqlExec) : base("vlfMdtType", sqlExec) { }


        /// <summary>
        /// Add new Mdt record
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="model"></param>
        public void AddMdtType(string manufacturer, string model)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("INSERT INTO {0} ( Manufacturer, Model) VALUES ( @Manufacturer, @Model )", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Manufacturer", SqlDbType.VarChar, manufacturer);
                sqlExec.AddCommandParam("@Model", SqlDbType.VarChar, model);

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
                string prefixMsg = string.Format("Unable to add new MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This MdtType already exists.", prefixMsg));
            }
        }

        /// <summary>
        /// Remove existing MdtType record by manufacturer & model
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="model"></param>
        public void RemoveMdtType(string manufacturer, string model)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("DELETE FROM {0} WHERE Manufacturer = @Manufacturer AND Model = @Model", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Manufacturer", SqlDbType.VarChar, manufacturer);
                sqlExec.AddCommandParam("@Model", SqlDbType.VarChar, model);

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
                string prefixMsg = string.Format("Unable to remove MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by Manufacturer & Model not found.", prefixMsg));
            }

        }

        /// <summary>
        /// Remove existing MdtType record by id
        /// </summary>
        /// <param name="identifier"></param>
        public void RemoveMdtType(int identifier)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("DELETE FROM {0} WHERE MdtTypeId = @MdtTypeId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@MdtTypeId", SqlDbType.VarChar, identifier);

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
                string prefixMsg = string.Format("Unable to remove '{0}' MdtType .", identifier);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' MdtType.", identifier);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' MdtType", identifier);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by MdtTypeId not found.", prefixMsg));
            }

        }



        /// <summary>
        /// Update MdtType
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="manufacturer"></param>
        /// <param name="model"></param>
        public void UpdateMdt(int identifier, string manufacturer, string model)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("UPDATE {0} SET Manufacturer = @Manufacturer, Model = @Model WHERE MdtTypeId = @MdtTypeId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Manufacturer", SqlDbType.VarChar, manufacturer);
                sqlExec.AddCommandParam("@Model", SqlDbType.VarChar, model);
                sqlExec.AddCommandParam("@MdtTypeId", SqlDbType.Int, identifier);
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
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to update '{0}' MdtType.", identifier);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by MdtTypeId not found.", prefixMsg));
            }
        }

        public MdtTypeClass GetMdtType(int identifier)
        {
            MdtTypeClass mdtType = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT * FROM {0} WHERE MdtTypeId = @MdtTypeId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@MdtTypeId", SqlDbType.Int, identifier);

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
                            mdtType = new MdtTypeClass();
                            mdtType.Id = Convert.ToInt32(dr["MdtTypeId"]);
                            mdtType.Manufacturer = Convert.ToString(dr["Manufacturer"]);
                            mdtType.Model = Convert.ToString(dr["Model"]);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve '{0}' MdtType.", identifier);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve '{0}' MdtType.", identifier);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return mdtType;
        }

        public MdtTypeClass GetMdtType(string manufacturer, string model)
        {
            MdtTypeClass mdtType = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT * FROM {0} WHERE Manufacturer = @Manufacturer AND Model = @Model", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Manufacturer", SqlDbType.VarChar, manufacturer);
                sqlExec.AddCommandParam("@Model", SqlDbType.VarChar, model);

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
                        if (dr!=null)
                        {
                            mdtType = new MdtTypeClass();
                            mdtType.Id = Convert.ToInt32(dr["MdtTypeId"]);
                            mdtType.Manufacturer = Convert.ToString(dr["Manufacturer"]);
                            mdtType.Model = Convert.ToString(dr["Model"]);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve MdtType from manufacturer '{0}' with model '{1}'.", manufacturer, model);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }

            return mdtType;
        }

    }


    public class MdtTypeClass
    {
        /// <summary>
        /// MdtTypeId
        /// </summary>
        public int Id;

        /// <summary>
        /// Manufacturer
        /// </summary>
        public string Manufacturer;

        /// <summary>
        /// Model
        /// </summary>
        public string Model;

    }



}
