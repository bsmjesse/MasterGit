using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.CLS.Def;			// for Enums
using System.Collections;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to vlfImages table.
    /// </summary>
    public class Images : TblOneIntPrimaryKey
    {
        #region Public Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Images(SQLExecuter sqlExec)
            : base("vlfImages", sqlExec)
        {
        }
        /// <summary>
        /// Add new Image.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="dclId"></param>
        /// <param name="boxProtocolTypeId"></param>
        /// <param name="imageSize"></param>
        /// <param name="imageData"></param>
        /// <param name="description"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddImage(int boxId, short dclId, short boxProtocolTypeId,
                            int imageSize, byte[] imageData, string description)
        {
            int rowsAffected = 0;
            // Retrieves max index for next row.
            int nextImageIndex = (int)GetMaxRecordIndex("ImageIndex");
            string prefixMsg = "Unable to add new image.";
            try
            {
                //Prepares SQL statement
                string sql = "INSERT INTO " + tableName
                    + " (ImageIndex,DateTime,ImageSize,ImageData,Description) VALUES (@ImageIndex,@DateTime,@ImageSize,@BlobData,@Description)";

                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                // 2. Set SQL command
                sqlExec.ClearCommandParameters();
                // 3. Add parameters to SQL statement
                sqlExec.AddCommandParam("@ImageIndex", SqlDbType.BigInt, ++nextImageIndex);
                sqlExec.AddCommandParam("@DateTime", SqlDbType.DateTime, DateTime.Now);
                sqlExec.AddCommandParam("@ImageSize", SqlDbType.Int, imageSize);
                sqlExec.AddCommandParam("@BlobData", SqlDbType.Binary, imageData, imageSize);
                if (description == null)
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);

                // 4. attach current command SQL to transaction
                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 5. Executes SQL statement (Add new image)
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

                // 6. Add new row into MsgInHst table
                string customProp = VLF.CLS.Util.MakePair(Const.keyImageIndex, nextImageIndex.ToString());
                AddToHistory(DateTime.Now, DateTime.Now, boxId, dclId,
                            Convert.ToInt16(Enums.MessageType.PictureDownloadComplete),
                            boxProtocolTypeId, customProp, imageSize);

                // 7. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 7. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 7. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This image already exists.");
            }
        }

        /// <summary>
        /// Deletes information by box id
        /// </summary>
        /// <param name="imageIndex"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteImageByImageIndex(int imageIndex)
        {
            return DeleteRowsByIntField("ImageIndex", imageIndex, "image index");
        }

        /// <summary>
        /// Returns only first image. 
        /// </summary>
        /// <param name="imageIndex"></param>
        /// <returns>DataSet [ImageIndex],[DateTime},[ImageSize],[ImageData],[Description]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetImageInfoByImageIndex(int imageIndex)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "Unable to retrieve image=" + imageIndex + ". ";
            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM " + tableName + " WHERE ImageIndex=" + imageIndex;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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

            return resultDataSet;
        }
        /// <summary>
        /// Deletes information by license plate
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteImagesByLicensePlate(string licensePlate)
        {
            int rowsAffected = 0;
            // Retrieves all image indexes by license plate from the history table
            ArrayList imageIndex = GetImageIndexByLicensePlate(licensePlate);
            if (imageIndex != null)
            {
                System.Collections.IEnumerator ittr = imageIndex.GetEnumerator();
                // Retrieves nest result by image index
                while (ittr.MoveNext())
                {
                    rowsAffected += DeleteImageByImageIndex(Convert.ToInt32(ittr.Current));
                }
            }
            return rowsAffected;
        }

        /// <summary>
        /// Returns only first image. 
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns>DataSet [ImageIndex],[DateTime},[ImageSize],[ImageData],[Description]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetImagesInfoByLicensePlate(string licensePlate)
        {
            DataSet resultDataSet = null;
            // Retrieves all image indexes by license plate
            ArrayList imageIndex = GetImageIndexByLicensePlate(licensePlate);
            if (imageIndex != null)
            {
                resultDataSet = new DataSet();
                bool isFirst = true;
                DataSet imageTmpDataSet = null;
                System.Collections.IEnumerator ittr = imageIndex.GetEnumerator();
                while (ittr.MoveNext())
                {
                    // Retrieves next result by image index
                    imageTmpDataSet = GetImageInfoByImageIndex(Convert.ToInt32(ittr.Current));
                    if (isFirst)
                    {
                        isFirst = false;
                        // copy metadata to result dataset (only once).
                        resultDataSet = imageTmpDataSet.Clone();
                    }
                    resultDataSet.Tables[0].LoadDataRow(imageTmpDataSet.Tables[0].Rows[0].ItemArray, false);
                }
            }
            return resultDataSet;
        }
        /// <summary>
        /// Update description  by image index.
        /// </summary>
        /// <param name="imageIndex"></param>
        /// <param name="description"></param>
        /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void SetDescription(int imageIndex, string description)
        {
            int rowsAffected = 0;
            string prefixMsg = "Unable to set new descrition by image index='" + imageIndex + "'. ";
            try
            {
                //Prepares SQL statement
                string sql = "UPDATE " + tableName +
                    " SET Description='" + description.Replace("'", "''") + "'" +
                    " WHERE ImageIndex=" + imageIndex;
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
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

            //Throws exception in case of wrong result
            if (rowsAffected == 0)
            {
                throw new DASAppResultNotFoundException(prefixMsg + " Wrong image index='" + imageIndex + "'.");
            }
        }
        #endregion

        #region Protected Interfaces
        /// <summary>
        /// Adds msg to history
        /// </summary>
        /// <remarks>
        /// Backup Msg to the history.
        /// Deletes exist message.
        /// </remarks>
        /// <param name="originDateTime"></param>
        /// <param name="dateTimeReceived"></param>
        /// <param name="boxId"></param>
        /// <param name="dclId"></param>
        /// <param name="boxMsgInTypeId"></param>
        /// <param name="boxProtocolTypeId"></param>
        /// <param name="customProp"></param>
        /// <param name="blobDataSize"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected void AddToHistory(DateTime originDateTime, DateTime dateTimeReceived,
                                    int boxId, short dclId, short boxMsgInTypeId,
                                    short boxProtocolTypeId,
                                    string customProp, int blobDataSize)
        {
            int rowsAffected = 0;
            // Construct SQL for migration to the data target. 
            string sql = "INSERT INTO vlfMsgInHst ( " +
                    " OriginDateTime" +
                    ",DateTimeReceived" +
                    ",BoxId" +
                    ",DclId" +
                    ",BoxMsgInTypeId" +
                    ",BoxProtocolTypeId" +
                    ",CustomProp" +
                    ",BlobDataSize" +
                    ") VALUES ( " +
                    "@OriginDateTime, " +
                    "@DateTimeReceived, " +
                    "@BoxId, " +
                    "@DclId, " +
                    "@BoxMsgInTypeId, " +
                    "@BoxProtocolTypeId, " +
                    "@CustomProp, " +
                    "@BlobDataSize ) ";

            // Set SQL command
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, originDateTime);
            sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, dateTimeReceived);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, dclId);
            sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, boxMsgInTypeId);
            sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, boxProtocolTypeId);

            if (customProp == null)
                sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, System.DBNull.Value);
            else
                sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, customProp);
            sqlExec.AddCommandParam("@BlobDataSize", SqlDbType.Int, blobDataSize);
            // attach current command SQL to transaction
            if (sqlExec.RequiredTransaction())
            {
                // 3. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException(" The message with datetime '" + originDateTime.ToString() + "' already exists.");
            }
        }
        /// <summary>
        /// Retrieves array of active images by license plate
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns>In case of empty result, returns null.</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected ArrayList GetImageIndexByLicensePlate(string licensePlate)
        {
            ArrayList imageIndexArr = null;
            DataSet resultDataSet = null;
            string prefixMsg = "Unable to retrieve image index by license plate '" + licensePlate + "'. ";
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT DISTINCT CustomProp FROM vlfMsgInHst with (nolock) ,vlfBoxVehicleAssignment" +
                    " WHERE (vlfBoxVehicleAssignment.LicensePlate='" + licensePlate.Replace("'", "''") + "')" +
                    " AND (vlfBoxVehicleAssignment.BoxId=vlfMsgInHst.BoxId)" +
                    " AND (vlfMsgInHst.BoxProtocolTypeId=" + Convert.ToInt16(Enums.ProtocolTypes.ASI) + ")" +
                    " AND (vlfMsgInHst.BoxMsgInTypeId=" + Convert.ToInt16(Enums.MessageType.PictureDownloadComplete) + ")";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);

                // 3. Retrieves result into array list.
                if (resultDataSet.Tables[0].Rows.Count > 0)
                {
                    imageIndexArr = new ArrayList();
                    int imageIndex = 0;
                    foreach (DataRow currRow in resultDataSet.Tables[0].Rows)
                    {
                        imageIndex = Convert.ToInt32(VLF.CLS.Util.PairFindValue(Const.keyImageIndex, Convert.ToString(currRow[0]).TrimEnd()));
                        if (IsImageExist(imageIndex))
                        {
                            imageIndexArr.Add(imageIndex);
                        }
                    }
                }
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
            return imageIndexArr;
        }

        /// <summary>
        /// Checks if image exist
        /// </summary>
        /// <param name="imageIndex"></param>
        /// <returns> True if image index exists, otherwise false</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected bool IsImageExist(int imageIndex)
        {
            bool isExist = false;
            string prefixMsg = "Unable to find image index=" + imageIndex + ". ";
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT COUNT(*) FROM " + tableName +
                    " WHERE (ImageIndex=" + imageIndex + ")";
                // 2. Executes SQL statement
                int retResult = (int)sqlExec.SQLExecuteScalar(sql);
                if (retResult == 1)
                {
                    isExist = true;
                }
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
            return isExist;
        }

        #endregion
    }
}
