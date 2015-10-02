using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for OleDbDataReader
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to vlfBoxHwType table.
   /// </summary>
   public class BoxHwType : TblOneIntPrimaryKey
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public BoxHwType(SQLExecuter sqlExec)
         : base("vlfBoxHwType", sqlExec)
      {
      }

      /// <summary>
      /// Add new box hardware type.
      /// </summary>
      /// <param name="boxHwTypeName"></param>
      /// <param name="maxSensorsNum"></param>
      /// <param name="maxOutputsNum"></param>
      /// <returns>int new hardware type id</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if hardware type name already exist</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddRecord(string boxHwTypeName, short maxSensorsNum, short maxOutputsNum)
      {
         int nextRowId = -1, rowsAffected = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to add new box hardware type with BoxHwTypeName=" + boxHwTypeName + " and MaxSensorsNum=" + maxSensorsNum + ".";
            Object res = GetMaxValue("BoxHwTypeId");
            if (res != null)
               nextRowId = Convert.ToInt16(res);
            //Prepare SQL statement
            string sql = string.Format("INSERT INTO " + tableName +
                  " (BoxHwTypeId,BoxHwTypeName,MaxSensorsNum,MaxOutputsNum)" +
                  " VALUES ( {0},'{1}',{2},{3} )",
                  ++nextRowId, boxHwTypeName.Replace("'", "''"), maxSensorsNum, maxOutputsNum);
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
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " The box hardware type already exists.");
         }
         return nextRowId;
      }

      /// <summary>
      /// Deletes existing box hardware type by name.
      /// </summary>
      /// <param name="boxHwTypeName"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if hardware type name does not exist</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteRecord(string boxHwTypeName)
      {
         return DeleteRowsByStrField("BoxHwTypeName", boxHwTypeName, "hardware type");
      }

      /// <summary>
      /// Deletes existing box hardware type by id.
      /// </summary>
      /// <param name="boxHwTypeId"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if hardware type name does not exist</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteRecord(short boxHwTypeId)
      {
         return DeleteRowsByIntField("BoxHwTypeId", boxHwTypeId, "hardware type");
      }

      /// <summary>
      /// Retrieves record count of "vlfBoxHwType" table
      /// </summary>
      /// <returns>int total records</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public Int64 RecordCount
      {
         get
         {
            return GetRecordCount("BoxHwTypeId");
         }
      }

      /// <summary>
      /// Retrieves max record index from "vlfBoxHwType" table
      /// </summary>
      /// <returns>int last record id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public Int64 MaxRecordIndex
      {
         get
         {
            return GetMaxRecordIndex("BoxHwTypeId");
         }
      }

      /// <summary>
      /// Retrieves box hardware type name by id from "vlfBoxHwType" table
      /// </summary>
      /// <param name="boxHwTypeId"></param>
      /// <returns>string hardware name by hardware id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public string GetNameById(short boxHwTypeId)
      {
         string name = Convert.ToString(GetFieldObjValueByRowId("BoxHwTypeId", boxHwTypeId, "BoxHwTypeName", "hardware type"));
         return name.TrimEnd();
      }

      /// <summary>
      /// Retrieves box hardware type id by name from "vlfBoxHwType" table
      /// </summary>
      /// <param name="boxHwTypeName"></param>
      /// <returns>short box Hw type Id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public short GetIdByName(string boxHwTypeName)
      {
         short boxHwTypeId = VLF.CLS.Def.Const.unassignedIntValue;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT BoxHwTypeId FROM " + tableName +
                     " WHERE BoxHwTypeName='" + boxHwTypeName.Replace("'", "''") + "'";
            //Executes SQL statement
            boxHwTypeId = (short)sqlExec.SQLExecuteScalar(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve box Hw type Id by name=" + boxHwTypeName + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve box Hw type Id by name=" + boxHwTypeName + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return boxHwTypeId;
      }

      /// <summary>
      /// Retrieves max sensors number by id from "vlfBoxHwType" table
      /// </summary>
      /// <param name="boxHwTypeId"></param>
      /// <returns>SByte max sensors number by hardware id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public short GetMaxSensorsNumById(short boxHwTypeId)
      {
         return Convert.ToInt16(GetFieldObjValueByRowId("BoxHwTypeId", boxHwTypeId, "MaxSensorsNum", "hardware type"));
      }

      /// <summary>
      /// Retrieves max outputs number by id from "vlfBoxHwType" table
      /// </summary>
      /// <param name="boxHwTypeId"></param>
      /// <returns>SByte max outputs number by hardware id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public short GetMaxOutputsNumById(short boxHwTypeId)
      {
         return Convert.ToInt16(GetFieldObjValueByRowId("BoxHwTypeId", boxHwTypeId, "MaxOutputsNum", "hardware type"));
      }
   }
}
