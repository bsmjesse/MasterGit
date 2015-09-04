using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.DAS.DB;

namespace VLF.DAS.Logic
{
   /// <summary>
   /// Organization Products Interface
   /// </summary>
   public partial class Organization
   {
      /// <summary>
      /// Add a new product to organization
      /// </summary>
      /// <param name="orgId"></param>
      /// <param name="productName"></param>
      /// <param name="upper"></param>
      /// <param name="lower"></param>
      public void AddProduct(int orgId, string productName, float upper, float lower)
      {
         int rowsAffected = 0, newProductId = 0;

         try
         {
            object value = product.GetMaxValueByOrganization("ProductID", orgId);
            if (value == null || value == System.DBNull.Value)
               newProductId = 1;
            else
            {
               try
               {
                  newProductId = Convert.ToInt32(value) + 1;
               }
               catch (InvalidCastException ex)
               {
                  newProductId = 1;
               }
            }
            string sql = "(OrganizationID, ProductID, ProductName, Upper, Lower) VALUES(@orgId, @productId, @name, @upper, @lower)";

            SqlParameter[] paramArray = new SqlParameter[5];
            paramArray[0] = new SqlParameter("@orgId", orgId);
            paramArray[1] = new SqlParameter("@productId", newProductId);
            paramArray[2] = new SqlParameter("@name", productName);
            paramArray[3] = new SqlParameter("@upper", upper);
            paramArray[4] = new SqlParameter("@lower", lower);

            rowsAffected = product.AddRow(sql, paramArray);
            if (rowsAffected == 0)
               throw new DASAppDataAlreadyExistsException("Cannot add a new product. This product already exists.");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to add a new product ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(objException.Message);
         }
      }

      /// <summary>
      /// Update existing product by Id
      /// </summary>
      /// <param name="orgId"></param>
      /// <param name="productId"></param>
      /// <param name="productName"></param>
      /// <param name="upper"></param>
      /// <param name="lower"></param>
      public void UpdateProductById(int orgId, int productId, string productName, float upper, float lower)
      {
         int rowsAffected = 0;
         try
         {
            string sql = "SET ProductName = @name, Upper = @upper, Lower = @lower WHERE OrganizationID=@orgId AND ProductID = @prodId";
            SqlParameter[] paramArray = new SqlParameter[5];
            paramArray[0] = new SqlParameter("@orgId", orgId);
            paramArray[1] = new SqlParameter("@prodId", productId);
            paramArray[2] = new SqlParameter("@name", productName);
            paramArray[3] = new SqlParameter("@upper", upper);
            paramArray[4] = new SqlParameter("@lower", lower);

            rowsAffected = product.UpdateRow(sql, paramArray);

            //rowsAffected = product.UpdateProduct(orgId, productId, productName, upper, lower);
            if (rowsAffected == 0)
               throw new DASAppDataAlreadyExistsException("Cannot update a product.");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to update a product ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(objException.Message);
         }
      }

      /// <summary>
      /// Update existing product by name
      /// </summary>
      /// <param name="orgId"></param>
      /// <param name="productName"></param>
      /// <param name="newName"></param>
      /// <param name="upper"></param>
      /// <param name="lower"></param>
      public void UpdateProductByName(int orgId, string productName, string newName, float upper, float lower)
      {
         int rowsAffected = 0;
         try
         {
            string sql = "SET ProductName = @newname, Upper = @upper, Lower = @lower WHERE OrganizationID = @orgId AND ProductName = @name";
            SqlParameter[] paramArray = new SqlParameter[5];
            paramArray[0] = new SqlParameter("@orgId", orgId);
            paramArray[1] = new SqlParameter("@name", productName);
            paramArray[2] = new SqlParameter("@newname", newName);
            paramArray[3] = new SqlParameter("@upper", upper);
            paramArray[4] = new SqlParameter("@lower", lower);

            rowsAffected = product.UpdateRow(sql, paramArray);
            if (rowsAffected == 0)
               throw new DASAppDataAlreadyExistsException("Cannot update a product.");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to update a product ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(objException.Message);
         }
      }

      /// <summary>
      /// Delete organization product by Id
      /// </summary>
      /// <param name="orgId"></param>
      /// <param name="productId"></param>
      public void DeleteProduct(int orgId, int productId)
      {
         int rowsAffected = 0;
         rowsAffected = product.DeleteRowsByFields("OrganizationID", orgId, "ProductID", productId, String.Empty);
         if (rowsAffected == 0)
            throw new DASDbException("Cannot delete organization product: " + productId.ToString());
      }

      /// <summary>
      /// Delete all organization products
      /// </summary>
      /// <param name="orgId"></param>
      public void DeleteAllProducts(int orgId)
      {
         int rowsAffected = 0;
         product.DeleteRowsByIntField("OrganizationID", orgId, String.Empty);
         if (rowsAffected == 0)
            throw new DASDbException("Cannot delete products for organization: " + orgId.ToString());
      }

      /// <summary>
      /// Get organization product by Id
      /// </summary>
      /// <param name="orgId"></param>
      /// <param name="productId"></param>
      public DataSet GetProduct(int orgId, int productId)
      {
         return product.GetRowsByPrimaryKey("OrganizationID", orgId, "ProductID", productId);
      }

      /// <summary>
      /// Get organization product by name
      /// </summary>
      /// <param name="orgId"></param>
      /// <param name="productName"></param>
      public DataSet GetProduct(int orgId, string productName)
      {
         string filter = "WHERE OrganizationID = @orgId AND ProductName = @name";
         SqlParameter[] paramArray = new SqlParameter[2];
         paramArray[0] = new SqlParameter("@orgId", orgId);
         paramArray[1] = new SqlParameter("@name", productName);

         return product.GetRowsByFilter(filter, paramArray);
      }

      /// <summary>
      /// Get all products for organization
      /// </summary>
      /// <param name="orgId"></param>
      public DataSet GetAllProducts(int orgId)
      {
         return product.GetRowsByIntField("OrganizationID", orgId, String.Empty);
      }
   }
}
