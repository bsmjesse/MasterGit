using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Xml.Serialization;

using VLF.CLS;
using VLF.CLS.Def;
// using VLF.DAS.Logic;
using VLF.DAS.DB;

namespace VLF.DAS.DB
{
    /** \class  MessageToTable
        *  \brief  this class get a number of messages from queue and move them into a SQL table
        *          the whole process is wrapped in a transaction
        *  \comment if SLS crashes when the array of session is in memory that's not a problem !!
        *  \comment SLS selects messages based on priority and handles them first !!
        *  \idea    or write into a file in XML format and from time to time insert directly in vlfMsgIn
        */
    public class MessageToTable
    {
        private string _pattern;
        private string _connectionString;
        private string _tableDestination;
        private int _batchSize;
        private bool _bOut = false;
        private Thread _dumper = null;
        private long _operationsCnt = 0L;
        ArrayList _mappingCollection = null;
        private string _currentDir;
        string _PersistantDataStorePath = @"C:\Sentinelfm\bin\services\";
        string _PersistantDataStoreFilenameFormat = "{0}.dat";
        string _PersistantDataStoreFilename = "unknown.dat";

        Sequencer2 _Sequencer2;

        //      System.Messaging.Cursor _msgCursor = null;
        /**
         *  a user is adding in queue and in _table
         *  a timer thread is saving the datatable and remove elements from MessageQueue after the bulk insert
         *  if the message is not removed in 
         */
        private DataTable _table;

        public MessageToTable(string connectionString,
                              string tableDestination,
                              int batchSize)
        {
            Util.BTrace(Util.INF0, ">> MessageToTable -> conn={0} table={1} batchsize={2}",
                                    connectionString, tableDestination, batchSize);
            _connectionString = connectionString;
            _tableDestination = tableDestination;
            _batchSize = batchSize;
            _currentDir = Directory.GetCurrentDirectory() + "\\";
            _Sequencer2 = new Sequencer2(tableDestination, false);
        }

        public void Init(int dclId)
        {
            //LoadFileName(dclId);


            _pattern = string.Format("I{0}_*", dclId);

            Util.BTrace(Util.INF0, ">> MessageToTable -> Init {0}", _pattern);

            #region Requests table definition
            _table = new DataTable("Queue2VlfMsgIn");

            DataColumn dateTimeReceived = new DataColumn();
            dateTimeReceived.DataType = System.Type.GetType("System.Int64");
            dateTimeReceived.ColumnName = "DateTimeReceived";
            _table.Columns.Add(dateTimeReceived);

            DataColumn boxId = new DataColumn();
            boxId.DataType = System.Type.GetType("System.Int32");
            boxId.ColumnName = "BoxId";
            _table.Columns.Add(boxId);


            _table.Columns.Add("DclId", typeof(short));
            _table.Columns.Add("CommModeId", typeof(short));
            _table.Columns.Add("BoxMsgInTypeId", typeof(short));
            _table.Columns.Add("BoxProtocolTypeId", typeof(short));
            _table.Columns.Add("OriginDateTime", typeof(DateTime));
            _table.Columns.Add("CommInfo1", typeof(string));
            _table.Columns.Add("CommInfo2", typeof(string));

            _table.Columns.Add("ValidGps", typeof(sbyte));
            _table.Columns.Add("Latitude", typeof(double));
            _table.Columns.Add("Longitude", typeof(double));
            _table.Columns.Add("Speed", typeof(short));
            _table.Columns.Add("Heading", typeof(short));
            _table.Columns.Add("SensorMask", typeof(long));
            _table.Columns.Add("CustomProp", typeof(string));
            _table.Columns.Add("BlobData", typeof(byte[]));

            _table.Columns.Add("BlobDataSize", typeof(int));
            _table.Columns.Add("SequenceNum", typeof(int));
            _table.Columns.Add("IsArmed", typeof(sbyte));
            _table.Columns.Add("Priority", typeof(sbyte));
            _table.Columns.Add("FileName", typeof(string));    ///< this is the file name with the XML file containing the packet

            /*
                     DataColumn[] keys = new DataColumn[2];
                     keys[0] = dateTimeReceived;
                     keys[1] = boxId;
                     _table.PrimaryKey = keys;
            */
            // create a custom view on the table
            _table.DefaultView.Sort = "Priority DESC";

            #endregion

            ArrayList _mappingCollection = new ArrayList();

            foreach (DataColumn dc in _table.Columns)
            {
                if ("FileName".Equals(dc.ColumnName))
                    continue;
                _mappingCollection.Add(new SqlBulkCopyColumnMapping(dc.ColumnName, dc.ColumnName));
            }

            // here you have to load all the CMFIn(s) remained unprocessed because the DCL was down
            ReadFromFiles();

            _dumper = new Thread(new ThreadStart(OnTimer));
            _dumper.Start();

            Util.BTrace(Util.INF0, "<< MessageToTable -> Init {0}", _pattern);
        }

        public void Close()
        {
            _bOut = true;
            _dumper.Join(5000);
            if (null != _table)
            {
                lock (_table)
                {
                    Util.BTrace(Util.INF0, "MessageTable.Close -> messages {0}",
                          _table.Rows != null ? _table.Rows.Count : 0);
                    _table.Clear();
                }
            }

        }

        /**
         *  \comment  filter all files in the format pp_BOXID and put them in the table
         *            if the priority is 0 or 1, then discard the file
         */
        private void ReadFromFiles()
        {
            string[] fileNames = Directory.GetFiles(".", _pattern);
            if (null != fileNames)
            {
                for (int i = 0; i < fileNames.Length; ++i)
                    Send(FromXml(fileNames[i]));
            }

        }

        private CMFIn FromXml(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CMFIn));
            TextReader reader = new StreamReader(fileName);
            CMFIn cmf = (CMFIn)serializer.Deserialize(reader);
            reader.Close();
            return cmf;
        }

        /**
         *  \brief  save the object in XML format and return the name of the file
         */
        private string SaveToFile(CMFIn cmf)
        {
            if (null != cmf)
            {
                string filePath = ("pp_" + cmf.boxID + "_" + cmf.originatedDateTime + UtilRand.RandomInt32()).ToString().Replace('/', '_').Replace(':', '_');
                FileStream fs = new FileStream(filePath, FileMode.Append);

                XmlSerializer serializer = new XmlSerializer(typeof(CMFIn));
                TextWriter textwriter = new StringWriter();
                serializer.Serialize(textwriter, cmf);

                StreamWriter sw = new StreamWriter(fs);
                sw.Write(textwriter.ToString()); // .ToXml());
                sw.Flush();
                sw.Close();
                fs.Close();
                return filePath;
            }
            else
                throw new InvalidDataException("null cmf");
        }



        /** 
         *  \brief     the entry point for the UDP receiver to send back the ACKs
         */
        public bool Send(CMFIn cmfIn)
        {
            DataRow dr = null;
            try
            {
                // Convert received message to object that you think was sent
                lock (_table)
                {
                    dr = _table.NewRow();
                    //            Util.BTrace(Util.INF0, "CMFIN = {0}", cmfIn.ToString());
                    dr["DateTimeReceived"] = cmfIn.receivedDateTime.Ticks.ToString();
                    dr["BoxId"] = cmfIn.boxID.ToString();

                    dr["DclId"] = cmfIn.dclID.ToString();
                    dr["CommModeId"] = cmfIn.commMode.ToString();
                    dr["BoxMsgInTypeId"] = cmfIn.messageTypeID.ToString();
                    dr["BoxProtocolTypeId"] = cmfIn.protocolTypeID.ToString();


                    //this call to sequencer provides a unique origindatetime value based on last origindatetime value provided by box
                    cmfIn.originatedDateTime = _Sequencer2.SetSequenceData(cmfIn);
                    dr["OriginDateTime"] = cmfIn.originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"); // very important                    

                    dr["CommInfo1"] = cmfIn.commInfo1;
                    dr["CommInfo2"] = cmfIn.commInfo2;

                    dr["ValidGps"] = cmfIn.validGPS.ToString();
                    dr["Latitude"] = cmfIn.latitude.ToString();
                    dr["Longitude"] = cmfIn.longitude.ToString();
                    dr["Speed"] = cmfIn.speed.ToString();
                    dr["Heading"] = cmfIn.heading.ToString();
                    dr["SensorMask"] = cmfIn.sensorMask.ToString();
                    dr["CustomProp"] = cmfIn.customProperties;
                    dr["BlobData"] = cmfIn.blobData;

                    dr["BlobDataSize"] = cmfIn.blobSize;
                    dr["SequenceNum"] = cmfIn.sequenceNum.ToString();
                    dr["IsArmed"] = ((sbyte)cmfIn.isArmed).ToString();
                    dr["Priority"] = cmfIn.priority.ToString();
                    dr["FileName"] = SaveToFile(cmfIn);


                    _table.Rows.Add(dr);
                    _table.AcceptChanges();

                    //Util.BTrace(Util.INF0, "MessageToTable.Send -> SUCCESSFUL WRITE for boxID={0}", cmfIn.boxID.ToString());

                    return true;
                }
            }
            catch (Exception exc)
            {
                Util.BTrace(Util.INF0, "MessageToTable.Send -> EXC={0}", exc.Message);
                if (null != dr)
                    File.Delete(dr["FileName"].ToString());
            }

            return false;
        }

        public bool Send(CMFIn[] cmfIns)
        {
            DataRow dr = null;
            try
            {
                if (null != cmfIns && cmfIns.Length > 0)
                {
                    // Convert received message to object that you think was sent
                    lock (_table)
                    {
                        foreach (CMFIn cmfIn in cmfIns)
                        {
                            dr = _table.NewRow();
                            //            Util.BTrace(Util.INF0, "CMFIN = {0}", cmfIn.ToString());
                            dr["DateTimeReceived"] = cmfIn.receivedDateTime.Ticks.ToString();
                            dr["BoxId"] = cmfIn.boxID.ToString();

                            dr["DclId"] = cmfIn.dclID.ToString();
                            dr["CommModeId"] = cmfIn.commMode.ToString();
                            dr["BoxMsgInTypeId"] = cmfIn.messageTypeID.ToString();
                            dr["BoxProtocolTypeId"] = cmfIn.protocolTypeID.ToString();

                            //this call to sequencer provides a unique origindatetime value based on last origindatetime value provided by box
                            cmfIn.originatedDateTime = _Sequencer2.SetSequenceData(cmfIn);
                            dr["OriginDateTime"] = cmfIn.originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"); // very important                                 

                            dr["CommInfo1"] = cmfIn.commInfo1;
                            dr["CommInfo2"] = cmfIn.commInfo2;

                            dr["ValidGps"] = cmfIn.validGPS.ToString();
                            dr["Latitude"] = cmfIn.latitude.ToString();
                            dr["Longitude"] = cmfIn.longitude.ToString();
                            dr["Speed"] = cmfIn.speed.ToString();
                            dr["Heading"] = cmfIn.heading.ToString();
                            dr["SensorMask"] = cmfIn.sensorMask.ToString();
                            dr["CustomProp"] = cmfIn.customProperties;
                            dr["BlobData"] = cmfIn.blobData;

                            dr["BlobDataSize"] = cmfIn.blobSize;
                            dr["SequenceNum"] = cmfIn.sequenceNum.ToString();
                            dr["IsArmed"] = ((sbyte)cmfIn.isArmed).ToString();
                            dr["Priority"] = cmfIn.priority.ToString();
                            dr["FileName"] = SaveToFile(cmfIn);


                            _table.Rows.Add(dr);
                            _table.AcceptChanges();

                            //Util.BTrace(Util.INF0, "MessageToTable.Send -> SUCCESSFUL WRITE for boxID={0}", cmfIn.boxID.ToString());

                        }

                    }
                }

                return true;


            }
            catch (Exception exc)
            {
                Util.BTrace(Util.INF0, "MessageToTable.Send -> EXC={0}", exc.Message);
                if (null != dr)
                    File.Delete(dr["FileName"].ToString());
            }

            return false;
        }


        /** \fn        CopyDataToDestination
         *  \brief     this writes directly in vlfMsgInHst
         *             and then it goes in 
         */
        private void CopyDataToDestination(string connectionString,
                                           DataTable table,
                                           string[] columnsName,          // only a selection of the columns
                                           int notifyAfter)
        {
            //         long times = Interlocked.Increment(ref _operationsCnt);

            //         Util.BTrace(Util.INF0, ">> MessageToTable.CopyDataToDestination -> rows={0}", table.Rows.Count);

            if (null != table && null != table.Rows && table.Rows.Count > 0)
            {

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                {
                    bulkCopy.BatchSize = table.Rows.Count; //  _batchSize;
                    bulkCopy.BulkCopyTimeout = 5;
#if TESTING
               Util.BTrace(Util.INF0, "-- CopyDataToDestination -> rows={0}", bulkCopy.BatchSize);
#endif
                    //               bulkCopy.ColumnMappings.Add( = _mappingCollection.ToArray(typeof(SqlBulkCopyColumnMapping));

                    if (null != columnsName)
                        foreach (string columnName in columnsName)
                        {
                            SqlBulkCopyColumnMapping mapping = new SqlBulkCopyColumnMapping(columnName, columnName);
                            bulkCopy.ColumnMappings.Add(mapping);
                        }
                    else
                    {
                        // add all columns from DataTable
                        foreach (DataColumn dc in table.Columns)
                        {
                            if ("FileName".Equals(dc.ColumnName))
                                continue;
                            SqlBulkCopyColumnMapping mapping = new SqlBulkCopyColumnMapping(dc.ColumnName, dc.ColumnName);
                            bulkCopy.ColumnMappings.Add(mapping);
                        }

                    }

                    bulkCopy.DestinationTableName = _tableDestination;
                    /*
                                   bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(bulkCopy_SqlRowsCopied);
                                   bulkCopy.NotifyAfter = notifyAfter;    ///< this is the 
                    */
                    try
                    {
                        lock (table)
                        {
                            // order table by priority
                            DataTable newDT = null;
                            try
                            {

                                newDT = table.DefaultView.ToTable();
                                Util.BTrace(Util.INF0, "-- MessageToTable.CopyDataToDestination -> Row count:{0}", newDT.Rows.Count);
                            }
                            catch (Exception exc)
                            {
                                Util.BTrace(Util.INF0, "-- MessageToTable.CopyDataToDestination (ToTable) -> EXC {0}", exc.Message);
                            }

                            if (newDT == null || newDT.Rows.Count == 0)
                            {
                                Util.BTrace(Util.INF0, "-- MessageToTable.CopyDataToDestination (newDT is null or has no rows) ");
                                return;
                            }

                            try
                            {
                                bulkCopy.WriteToServer(newDT);
                            }
                            catch (Exception exc)
                            {
                                Util.BTrace(Util.ERR0, "-- BulkCopy Failed. Start insert one by one.");

                                using (SqlConnection scon = new SqlConnection(connectionString))
                                {
                                   // string sql = string.Format("WriteTo{0}", _tableDestination);
                                    string sql = "vlfMsgIn_Insert";
                                    using (SqlCommand scom = new SqlCommand(sql, scon))
                                    {
                                        scom.CommandType = CommandType.StoredProcedure;



                                        foreach (DataRow row in newDT.Rows)
                                        {
                                            try
                                            {
                                                scom.Parameters.Clear();
                                                scom.Parameters.Add("@DateTimeReceived", row["DateTimeReceived"]);
                                                scom.Parameters.Add("@BoxId", row["BoxId"]);
                                                scom.Parameters.Add("@DclId", row["DclId"]);
                                                scom.Parameters.Add("@CommModeId", row["CommModeId"]);
                                                scom.Parameters.Add("@BoxMsgInTypeId", row["BoxMsgInTypeId"]);
                                                scom.Parameters.Add("@BoxProtocolTypeId", row["BoxProtocolTypeId"]);
                                                scom.Parameters.Add("@OriginDateTime", row["OriginDateTime"]);
                                                scom.Parameters.Add("@CommInfo1", row["CommInfo1"]);
                                                scom.Parameters.Add("@CommInfo2", row["CommInfo2"]);
                                                scom.Parameters.Add("@ValidGps", row["ValidGps"]);
                                                scom.Parameters.Add("@Latitude", row["Latitude"]);
                                                scom.Parameters.Add("@Longitude", row["Longitude"]);
                                                scom.Parameters.Add("@Speed", row["Speed"]);
                                                scom.Parameters.Add("@Heading", row["Heading"]);
                                                scom.Parameters.Add("@SensorMask", row["SensorMask"]);
                                                scom.Parameters.Add("@CustomProp", row["CustomProp"]);
                                                scom.Parameters.Add("@BlobData", row["BlobData"]);
                                                scom.Parameters.Add("@BlobDataSize", row["BlobDataSize"]);
                                                scom.Parameters.Add("@SequenceNum", row["SequenceNum"]);
                                                scom.Parameters.Add("@Priority", row["Priority"]);

                                                if (scom.Connection.State == ConnectionState.Closed)
                                                    scom.Connection.Open();

                                                int rows = scom.ExecuteNonQuery();
                                                if (rows == 0)
                                                    Util.BTrace(Util.ERR0, "-- MessageToTable.CopyDataToDestination single row insert failed.-> {0}:{1}", row["BoxId"].ToString(), row["SequenceNum"].ToString());


                                            }
                                            catch (Exception exc1)
                                            {
                                                Util.BTrace(Util.ERR0, "-- MessageToTable.CopyDataToDestination single row insert failed.-> {0}:{1} EXC {2}", row["BoxId"].ToString(), row["SequenceNum"].ToString(), exc1.Message);
                                            }
                                            finally
                                            {

                                            }
                                        }
                                    }

                                }
                            }


                            // or delete all persistent files
                            foreach (DataRow dr in table.Rows)
                            {
                                try
                                {
                                    File.Delete(dr["FileName"].ToString());
                                }
                                catch (Exception exc)
                                {
                                    Util.BTrace(Util.INF0, "-- MessageToTable.CopyDataToDestination (DELETE) -> EXC {0}", exc.Message);
                                }

                            }

                            table.Clear();
                        }
                    }
                    catch (Exception exc)
                    {
                        Util.BTrace(Util.ERR0, "-- MessageToTable.CopyDataToDestination -> EXC {0}", exc.Message);
                    }
                }
            }

            //         Util.BTrace(Util.INF0, "<< MessageToTable.CopyDataToDestination -> rows={0}", table.Rows.Count);

        }

        void bulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            //         Util.BTrace(Util.INF1, "bulkCopy_SqlRowsCopied -> {0} Rows have been copied.", e.RowsCopied.ToString());
            Util.BTrace(Util.INF0, "bulkCopy_SqlRowsCopied -> {0} Rows have been copied.", e.RowsCopied.ToString());
        }

        public int GetTableSize
        {
            get
            {
                if (null != _table)
                {
                    lock (_table)
                    {
                        if (null != _table.Rows)
                            return _table.Rows.Count;
                    }
                }

                return (-1);
            }
        }


        /**
         *  \comment   on timer you call bulk_insert and then go through the queue and delete 
         *             elements you added to the database
         *             loops for maximum X elements in the queue
         */
        private void OnTimer()
        {
            byte cnt = 0;
            while (!_bOut)
            {
                if (GetTableSize > _batchSize || cnt > 6)
                {
                    CopyDataToDestination(_connectionString, _table, null, 10);
                    cnt = 0;
                }
                ++cnt;
                Thread.Sleep(100);
            }
        }

        private void LoadFileName(int dclId)
        {
            Util.BTrace(Util.INF0, "MessageToTable.LoadFileName -> ENTER");
            try
            {
                string sql = "GetDclNameById";
                using (SqlConnection scon = new SqlConnection(_connectionString))
                {
                    using (SqlCommand scom = new SqlCommand(sql, scon))
                    {
                        scom.CommandType = CommandType.StoredProcedure;
                        scom.Parameters.Clear();
                        scom.Parameters.Add("@DCLID", dclId);
                        scom.Prepare();
                        scom.Connection.Open();
                        _PersistantDataStoreFilename = string.Format(_PersistantDataStoreFilenameFormat, (string)scom.ExecuteScalar());
                        if (scom.Connection.State == ConnectionState.Open)
                            scom.Connection.Close();
                    }
                }
                Util.BTrace(Util.INF0, "MessageToTable.LoadFileName -> PersistantDataStoreFilename:[{0}]", _PersistantDataStoreFilename);
            }
            catch (Exception exc)
            {
                Util.BTrace(Util.ERR0, "-- MessageToTable.LoadFileName -> EXC {0}", exc.Message);

            }
            finally
            {
                Util.BTrace(Util.INF0, "MessageToTable.LoadFileName -> EXIT");
            }
        }

    }
}