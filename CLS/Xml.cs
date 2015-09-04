using System;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Collections;

namespace VLF.CLS
{
	public class XmlConfig : IDisposable
	{
		public XmlDocument xmlReader;

		public XmlConfig(string xmlName)
		{
			try
			{
				xmlReader = new XmlDocument();
				xmlReader.Load(xmlName);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(CLS.Util.TraceFormat( "ERRO:: Constructor XmlConfig( " + xmlName + " )." + ex.Message));
			}
		}

		public XmlConfig(string xmlName, bool isFile)
		{
			try
			{
				xmlReader = new XmlDocument();
				if( isFile )
					xmlReader.Load(xmlName);
				else
					xmlReader.LoadXml(xmlName);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(CLS.Util.TraceFormat( "ERRO:: Constructor XmlConfig( " + xmlName + ", " + isFile + " )." + ex.Message));
			}
		}

		public void Dispose()
		{
			xmlReader.RemoveAll();
			xmlReader = null;
		}	

		/// <summary>
		// method for retrieving attributes from node
		// example
		// SortedList attrList = xml.XmlReadElement("system/service");
		/// </summary>
		public SortedList XmlReadAttributes(string element)
		{
			int index = 1;
			SortedList nodes = new SortedList();
			XmlNodeList nodeList = xmlReader.SelectNodes(element);
			foreach (XmlNode nodeName in nodeList)
			{
				XmlAttributeCollection attr = nodeName.Attributes;
				for (int i=0; i < attr.Count; i++)
				{
					nodes.Add(attr.Item(i).Name + Convert.ToString(index++), attr.Item(i).Value);
				}         
			}
			return nodes;
		}

      public IList XmlReadAllElements(string node)
      {
         ArrayList arr = new ArrayList();
         XmlNodeList nodeList = xmlReader.SelectNodes(node);
         foreach (XmlNode nodeName in nodeList)
         {
            arr.Add(nodeName.InnerText);
         }

         return (IList)arr;
      }

		/// <summary>
		// method for retrieving values from node
		// examples 
		// int	diagMask = Convert.ToInt16(xml.XmlReadElement("system/service[@name = \"IPDCL\"]/library[@name = \"IPDCS\"]")["diag_mask"]);
		// SortedList paramValue = xml.XmlReadElement("system/service[@name = \"IPDCL\"]/library[@name = \"IPDCS\"]/devices/device[@name = \"UDP Server\"]");
		/// </summary>
		public IList XmlReadElement(string node)
		{
			XmlNodeReader nodeReader = XmlGetNodeReader(node);
			return SearchXml(nodeReader).GetValueList();
		}

		/// <summary>
		// method for retrieving values from node
		// examples 
		// int	diagMask = Convert.ToInt16(xml.XmlReadElement("system/service[@name = \"IPDCL\"]/library[@name = \"IPDCS\"]")["diag_mask"]);
		// SortedList paramValue = xml.XmlReadElement("system/service[@name = \"IPDCL\"]/library[@name = \"IPDCS\"]/devices/device[@name = \"UDP Server\"]");
		/// </summary>
		public SortedList XmlReadElementToSortedList(string node)
		{
			try
			{
				XmlNodeReader nodeReader = XmlGetNodeReader(node);
				return SearchXml(nodeReader);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(CLS.Util.TraceFormat( "ERRO:: XmlReadElementToSortedList ( " + node + " )." + ex.Message));
			}
			return null;
		}

		public XmlNodeReader XmlGetNodeReader(string service)
		{
			return new XmlNodeReader (xmlReader.SelectSingleNode(service));
		}

		private SortedList SearchXml (XmlNodeReader reader)
		{
			SortedList nodes = new SortedList();
			string element = "";
			int index = -1;
			int order = 0;

			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						element = reader.Name;
						while(reader.MoveToNextAttribute())
						{
							if( reader.Name == "count" )
							{
								index=Convert.ToInt16(reader.Value);
								order = index;
							}
						}
						break;
					case XmlNodeType.Text:
						if(index != -1)
						{
							nodes.Add(element + Convert.ToString(index - --order), reader.Value);
							if(order == 0)
								index = -1;
						}
						else
						{
							nodes.Add(element, reader.Value);
						}
						break;
				}
			}
			return nodes;
		}

		public static IList GetValueListFromDB(string keyValue, string keyName)
		{
			IList list = null;
			if( keyValue.StartsWith("<") )
			{
				try
				{
					XmlConfig xmlReader = new XmlConfig( keyValue, false );
					list = xmlReader.XmlReadElement(keyName);
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(CLS.Util.TraceFormat( "ERRO:: GetValueListFromDB( " + keyValue + ", " + keyName + " )." + ex.Message));
				}
			}
			else
			{
				try
				{
					// check if value is name or number
					if( keyValue.IndexOf('.') > 0 )
						System.Net.IPAddress.Parse(keyValue);
					else
						Convert.ToInt32(keyValue);
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(CLS.Util.TraceFormat( "WARN:: Exception in GetValueListFromDB( " + keyValue + ", " + keyName + " ). " + ex.Message + " Using value '" + keyValue + "'."));
				}
				list = new ArrayList();
				list.Add(keyValue);
			}
			return list;
		}
		/// <summary>
		/// update node's attribute with new value
		/// </summary>
		/// <param name="top">node to search from it to the bottom</param>
		/// <param name="nodeName">node to change will have this name</param>
		/// <param name="ValueToSet">this value will be set as an attribute to found node</param>
		public static void setStringName(XmlNode top, string nodeName, string ValueToSet)
		{
			foreach ( XmlAttribute att in top.Attributes)
			{
				if(nodeName.Equals(att.Name))
				{
					if(ValueToSet!=null)
						att.Value = ValueToSet;
					return;
				}
			}
			return;
		}

		/// <summary>
		/// update node's value with new value
		/// </summary>
		/// <param name="top">node to search from it to the bottom</param>
		/// <param name="nodeName">node to change will have this name</param>
		/// <param name="ValueToSet">this value will be set to found node</param>
		public static void setStringValue(XmlNode top, string nodeName, string ValueToSet)
		{
			foreach ( XmlNode n in top.ChildNodes)
			{
				if(nodeName.Equals(n.Name))
				{
					if(ValueToSet!=null && !ValueToSet.Equals(""))
						n.InnerText = ValueToSet;
					else 
						n.InnerText = "";
					return;
				}
			}
			return;
		}

		/// <summary>
		/// returns node's value of specific name
		/// </summary>
		/// <param name="top">node to search</param>
		/// <param name="nodeName">search criteria</param>
		/// <returns>node's value</returns>
		public static string getStringValue(XmlNode top, string nodeName)
		{
			string ret = "";
			foreach ( XmlNode n in top.ChildNodes)
			{
				if(nodeName.Equals(n.Name))
				{
					ret = n.InnerText;
					return ret;
				}
			}
			return ret;
		}

		/// <summary>
		/// return attribute's value of specific name
		/// </summary>
		/// <param name="top">node to search</param>
		/// <param name="nodeName">search criteria</param>
		/// <returns>attribute's value</returns>
		public static string getStringName(XmlNode top, string nodeName)
		{
			string ret = "";
			foreach ( XmlAttribute att in top.Attributes)
			{
				if(nodeName.Equals(att.Name))
				{
					ret = att.Value;
					return ret;
				}
			}
			return ret;
		}

		public ArrayList getNodeNames(string node)
		{
			ArrayList al = new ArrayList();
			int i = 0;
			XmlNode dclNode=null;
			while(true)
			{
				dclNode = xmlReader.GetElementsByTagName(node)[i];
				if(dclNode==null) break;
				XmlAttributeCollection attr = dclNode.Attributes;
				al.Add(attr.Item(0).Value);
				i++;
			}
			return al;
		}

      public static string ReadValue(string cfgString, string nodeName)
      {
         try
         {
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(cfgString)))
            {
               while (xmlReader.Read())
               {
                  //keep reading until you see my element
                  if (xmlReader.Name.Equals(nodeName) && (xmlReader.NodeType == XmlNodeType.Element))
                     return xmlReader.ReadElementContentAsString();
               }
            }
         }
         catch (Exception e)
         {
            System.Diagnostics.Debug.WriteLine(e.Message);
         }

         return (null);
      }
/*
      /// <summary>
      ///      int, bool, float, long, 
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="cfgString"></param>
      /// <param name="nodeName"></param>
      /// <returns></returns>
      public static T ReadValue<T>(string cfgString, string nodeName) where T:struct
      {
         T? val = null;
         try
         {
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(cfgString)))
            {
               while (xmlReader.Read())
               {
                  //keep reading until you see my element
                  if (xmlReader.Name.Equals(nodeName) && (xmlReader.NodeType == XmlNodeType.Element))
                  {
                     if (typeof(T) == typeof(int))
                        return xmlReader.ReadElementContentAsInt;
                     else if (typeof(T) == typeof(bool))
                           return xmlReader.ReadElementContentAsBoolean();
                     else if (typeof(T) == typeof(float))
                           return xmlReader.ReadElementContentAsFloat;
                     else if (typeof(T) == typeof(double))
                           return xmlReader.ReadElementContentAsDouble;
                     else if (typeof(T) == typeof(long))
                          return xmlReader.ReadElementContentAsLong;
                     else if (typeof(T) == typeof(DateTime))
                          return xmlReader.ReadElementContentAsDateTime;
                     else 
                        return xmlReader.ReadElementContentAsObject;       
                  }
               }
            }
         }
         catch (Exception e)
         {
            System.Diagnostics.Debug.WriteLine(e.Message);
         }

         return val;
      }
*/
		public static string TakeNodeFromXML( string xml, string node )
		{
			string retNode = "";
			int idxFirst = xml.IndexOf( node );
			if( idxFirst == -1 ) return retNode;
			idxFirst--; // < before node
			int idxLast = xml.LastIndexOf( node );
			if( idxLast == -1 ) return retNode;
			idxLast+=node.Length+1; // > after node like <pings>
			retNode = xml.Substring(idxFirst, idxLast - idxFirst);
			return retNode;
		}

      /// <summary>
      /// Get Node Attributes
      /// example: <node value="test" id="1" name="item" text="item 1 test" />
      /// </summary>
      /// <param name="xPath">xpath expresssion</param>
      /// <returns>SortedList</returns>
      public SortedList GetNodeAttributes(string nodeXPath)
      {
         SortedList list = new SortedList();
         XmlAttributeCollection attr = xmlReader.SelectSingleNode(nodeXPath).Attributes;

         for (int i = 0; i < attr.Count; i++)
         {
            list.Add(attr.Item(i).Name, attr.Item(i).Value);
         }
         return list;
      }

      /// <summary>
      /// Get Attribute Value
      /// </summary>
      /// <param name="xPath">Node xpath expression</param>
      /// <param name="attributeName">Node attribute name</param>
      /// <returns></returns>
      public string GetAttributeValue(string nodeXPath, string attributeName)
      {
         string result = "";
         XmlNode node = xmlReader.SelectSingleNode(nodeXPath);
         if (node != null)
            result = node.Attributes.GetNamedItem(attributeName).Value;
         return result;
      }
	}
}
