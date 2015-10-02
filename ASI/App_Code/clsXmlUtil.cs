using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using System.Collections.Generic;


namespace VLF.ASI
{
   /// <summary>
   /// Summary description for clsXmlUtil
   /// </summary>
   public class XmlUtil
   {
      private XmlDocument xDoc;

      /// <summary>
      /// Create xml
      /// </summary>
      public XmlUtil()
      {
         this.xDoc = new XmlDocument();
         XmlDeclaration xmlDecl = xDoc.CreateXmlDeclaration("1.0", Encoding.UTF8.EncodingName, "");
         this.xDoc.AppendChild(xmlDecl);
      }

      /// <summary>
      /// Load xml
      /// </summary>
      /// <param name="xmlString"></param>
      public XmlUtil(string xmlString)
      {
         this.xDoc = new XmlDocument();
         if (!String.IsNullOrEmpty(xmlString))
         {
            xDoc.LoadXml(xmlString);
            if (xDoc.DocumentElement == null)
               throw new XmlException("XmlUtil::Root Element is Empty.");
         }
         else
            throw new ArgumentException("XmlUtil::XML String is Empty.");
      }

      public void CreateRoot(string rootName)
      {
         if (this.Root == null)
         {
            XmlElement root = this.xDoc.CreateElement(rootName);
            xDoc.AppendChild(root);
         }
      }

      /// <summary>
      /// Text contents of a node
      /// </summary>
      /// <param name="xPath"></param>
      /// <returns></returns>
      public string GetNodeValue(string xPath)
      {
         if (String.IsNullOrEmpty(xPath))
            throw new ArgumentException("XmlUtil::GetNodeValue::XPath Parameter is Empty.");
         if (this.Root == null)
            throw new XmlException("XmlUtil::GetNodeValue::Root Element is Empty.");
         XmlNode node = this.Root.SelectSingleNode(xPath);
         if (node == null)
            throw new XmlException(String.Format("XmlUtil::GetNodeValue::Node <{0}> Not Found.", xPath));
         return node.InnerText;
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">String value</param>
      public void CreateNode(string nodeName, string nodeValue)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         newNode.InnerText = nodeValue;
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">Integer value</param>
      public void CreateNode(string nodeName, int nodeValue)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         newNode.InnerText = nodeValue.ToString();
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">Integer value</param>
      public void CreateNode(string nodeName, bool nodeValue)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         newNode.InnerText = nodeValue.ToString();
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">Long value</param>
      public void CreateNode(string nodeName, long nodeValue)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         newNode.InnerText = nodeValue.ToString();
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">Short value</param>
      public void CreateNode(string nodeName, short nodeValue)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         newNode.InnerText = nodeValue.ToString();
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">DateTime value</param>
      public void CreateNode(string nodeName, DateTime nodeValue)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         newNode.InnerText = nodeValue.ToString();
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">DateTime value</param>
      /// <param name="nodeValue">DateTime Format</param>
      /// <param name="utcFormat">True if Utc conversion required</param>
      public void CreateNode(string nodeName, DateTime nodeValue, string dateFormat, bool utcFormat)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         if (utcFormat)
            newNode.InnerText = nodeValue.ToUniversalTime().ToString(dateFormat);
         else
            newNode.InnerText = nodeValue.ToString(dateFormat);
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Add a new node to the xml
      /// </summary>
      /// <param name="nodeName">Node name</param>
      /// <param name="nodeValue">Char value</param>
      public void CreateNode(string nodeName, char nodeValue)
      {
         XmlNode newNode = this.xDoc.CreateNode("element", nodeName, "");
         newNode.InnerText = nodeValue.ToString();
         this.Root.AppendChild(newNode);
      }

      /// <summary>
      /// Root node of the xml document
      /// </summary>
      public XmlElement Root
      {
         get { return this.xDoc.DocumentElement; }
      }

      /// <summary>
      /// String representation of xml doc
      /// </summary>
      public string Xml
      {
         get { return this.xDoc.InnerXml; }
      }



      //This will returns the set of included namespaces for the serializer.
      public static XmlSerializerNamespaces GetNamespaces()
      {

          XmlSerializerNamespaces ns;
          ns = new XmlSerializerNamespaces();
          return ns;

      }

      //Returns the target namespace for the serializer.
      public static string TargetNamespace
      {
          get { return ""; }
      }

      //Creates an object from an XML string.
      public static object FromXml(string Xml, System.Type ObjType)
      {
          XmlSerializer ser;
          ser = new XmlSerializer(ObjType);
          StringReader stringReader;
          stringReader = new StringReader(Xml);
          XmlTextReader xmlReader;
          xmlReader = new XmlTextReader(stringReader);
          object obj;
          obj = ser.Deserialize(xmlReader);
          xmlReader.Close();
          stringReader.Close();
          return obj;

      }

      //Serializes the <i>Obj</i> to an XML string.
      public static string ToXml(object Obj, System.Type ObjType)
      {

          XmlSerializer ser;
          ser = new XmlSerializer(ObjType, TargetNamespace);
          MemoryStream memStream;
          memStream = new MemoryStream();
          XmlTextWriter xmlWriter;
          //xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
          xmlWriter = new XmlTextWriter(memStream, Encoding.ASCII);
          xmlWriter.Namespaces = true;
          ser.Serialize(xmlWriter, Obj, GetNamespaces());
          xmlWriter.Close();
          memStream.Close();
          string xml;
          //xml = Encoding.UTF8.GetString(memStream.GetBuffer());
          xml = Encoding.ASCII.GetString(memStream.GetBuffer());
          xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
          xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
          return xml;

      }

       /// <summary>
       /// Used to replace the GetXml method in DataSet, since the GetXml not include the null column
       /// </summary>
       /// <param name="dsDataset"></param>
       /// <returns></returns>
       public static string GetXmlIncludingNull(DataSet dsDataset)
       {

           DataSet dsDatasetAux = dsDataset.Copy();

           List<DataColumn> aColumnsToReplace = new List<DataColumn>();

           foreach (DataTable dtTable in dsDatasetAux.Tables)
           {
               if (dtTable.Rows.Count > 0)
               {
                   foreach (DataColumn oColumn in dtTable.Columns)
                   {
                       //check if none of the the rows has a value for the column   
                       if (dtTable.Select(string.Format("{0} is not null", oColumn.ColumnName)).Length == 0)
                       {
                           if ((!object.ReferenceEquals(oColumn.DataType, typeof(string))))
                           {
                               aColumnsToReplace.Add(oColumn);
                           }
                           else
                           {
                               dtTable.Rows[0][oColumn] = string.Empty;
                           }
                       }
                   }
                   foreach (DataColumn oColumn in aColumnsToReplace)
                   {
                       dtTable.Columns.Remove(oColumn);
                       dtTable.Columns.Add(oColumn.ColumnName, typeof(string)).DefaultValue = string.Empty;
                       //setting the value for the column in at least one row is enough for GetXML to include it   
                       dtTable.Rows[0][oColumn.ColumnName] = string.Empty;
                   }
               }
           }
           dsDatasetAux.AcceptChanges();

           return dsDatasetAux.GetXml();
       }

   }
}