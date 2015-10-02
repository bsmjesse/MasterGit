using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;
using System.Security.Permissions;

/// <summary>
/// Product used for a reefer sensors setup
/// </summary>
public class Product
{
   public string ProductId;
   public string ProductName;
   public string UpperLimit;
   public string LowerLimit;
   private char separator = '\t';
   private string value;

   public Product(string id, string name, string upper, string lower)
   {
      ProductId = id;
      ProductName = name;
      UpperLimit = upper;
      LowerLimit = lower;
      value = String.Format("{0}{1}{2}{3}{4}{5}{6}",
         id, separator, name, separator, upper, separator, lower);
   }

   public Product(string delimitedString)
   {
      string[] fields = delimitedString.Split(separator);
      if (fields.Length > 0)
         ProductId = fields[0];
      if (fields.Length > 1)
         ProductName = fields[1];
      if (fields.Length > 2)
         UpperLimit = fields[2];
      if (fields.Length > 3)
         LowerLimit = fields[3];
      value = delimitedString;
   }

   /// <summary>
   /// Text for a list box
   /// </summary>
   public string Text
   {
      get { return ProductName; }
   }

   /// <summary>
   /// Serialization value for a list box
   /// </summary>
   public string Value
   {
      get { return value; }
   }
}
