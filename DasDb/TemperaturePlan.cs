using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to vlfTemperaturePlan table.
   /// </summary>
   public class TemperaturePlan : TblTwoPrimaryKeys
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public TemperaturePlan(SQLExecuter sqlExec) : base("vlfTemperaturePlan", sqlExec)
      {
      }
/*
      /// <summary>
      /// Add new driver information
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="productName"></param>
      /// <param name="upper"></param>
      /// <param name="lower"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver license or person alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <returns>Rows affected</returns>
      public int AddProduct(int organizationId, string productName, float upper, float lower)
      {
         // 1. Prepares SQL statement
         int newProductId = (int)base.GetMaxValueByOrganization("ProductID", organizationId) + 1;

         string sql = "INSERT " + this.tableName +
            "(OrganizationID, ProductID, ProductName, Upper, Lower) VALUES(@orgId, @productId, @name, @upper, @lower)";

         sqlExec.ClearCommandParameters();

         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);
         sqlExec.AddCommandParam("@productId", SqlDbType.Int, newProductId);
         sqlExec.AddCommandParam("@name", SqlDbType.VarChar, productName, 50);
         sqlExec.AddCommandParam("@upper", SqlDbType.Float, upper);
         sqlExec.AddCommandParam("@lower", SqlDbType.Float, lower);

         if (sqlExec.RequiredTransaction())
         {
            // 4. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         // 5. Executes SQL statement
         return sqlExec.SQLExecuteNonQuery(sql);
      }

      /// <summary>
      /// Update driver information.
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="productId"></param>
      /// <param name="name"></param>
      /// <param name="upper"></param>
      /// <param name="lower"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if person does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <returns>Rows affected</returns>
      public int UpdateProduct(int organizationId, int productId, string name, float upper, float lower)
      {
         // 1. Prepares SQL statement
         string sql = "UPDATE " + this.tableName + 
            " SET ProductName = @name, Upper = @upper, Lower = @lower WHERE OrganizationID=@orgId AND ProductID = @productId";

         sqlExec.ClearCommandParameters();

         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);
         sqlExec.AddCommandParam("@productId", SqlDbType.Int, productId);
         sqlExec.AddCommandParam("@name", SqlDbType.VarChar, name, 50);
         sqlExec.AddCommandParam("@upper", SqlDbType.Float, upper);
         sqlExec.AddCommandParam("@lower", SqlDbType.Float, lower);

         //Executes SQL statement
         return sqlExec.SQLExecuteNonQuery(sql);
      }
*/
   }
}
