<%@ WebService Language="C#" Class="EmailSystem" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Globalization;
using System.Configuration;
using VLF3.DataAccess;
//using VLF3.Common.SDA.Implementation;
using Telerik.Web.UI;
using System.Data; 
using System.Data.SqlClient;       
using System.Text;
using System.Linq; 
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using VLF.CLS;

[WebService(Namespace = "http://www.sentinelfm.com")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class EmailSystem  : System.Web.Services.WebService {

    //private Message ms;
    //private DataTable dt;
    //const string DELIMITER = "|";
  
    //private bool CheckSession(int UserId) {
    //    if (0 == UserId) return false;
    //    return true;
    //}

    //[WebMethod(EnableSession = true), Description("Check if Forms option is enable")]
    //public bool IsEnableForms(int UserId)
    //{
    //    try
    //    {
    //        bool resultFlag;
    //        int result = MessageDAO.GetInstance(System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString).EnableForms(Convert.ToInt64(UserId), out resultFlag);
    //        if (result == 0) return resultFlag;
    //        return false;            
    //    }
    //    catch (Exception ex)
    //    {
            
    //        return false;
    //    }
    //}
    
    //private void Init_InboundInformationFiltered(int UserId,string dateFromYear,string dateFromMonth,string dateFromDay,string dateToYear,string dateToMonth,string dateToDay,string timeHourFrom,string timeMinuteFrom,string timeHourTo,string timeMinuteTo)
    //{
    //    ms = new Message(System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString);
    //    dt = ms.BriefInboundByOrganizationIdAndDateRange(UserId, new DateTime(Convert.ToInt32(dateFromYear), Convert.ToInt32(dateFromMonth), Convert.ToInt32(dateFromDay), Convert.ToInt32(timeHourFrom), Convert.ToInt32(timeMinuteFrom), 1),
    //                                                                     new DateTime(Convert.ToInt32(dateToYear), Convert.ToInt32(dateToMonth), Convert.ToInt32(dateToDay), Convert.ToInt32(timeHourTo), Convert.ToInt32(timeMinuteTo), 1));
    //    if (null == dt)
    //    {
    //        string errMess = ms.GetLastError();
    //    }
    //}
  
    //private void Init_OutbounInformationFiltered(int UserId, string dateFromYear, string dateFromMonth, string dateFromDay, string dateToYear, string dateToMonth, string dateToDay, string timeHourFrom, string timeMinuteFrom, string timeHourTo, string timeMinuteTo)
    //{
    //    ms = new Message(System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString);
    //    dt = ms.BriefOutboundByOrganizationIdAndDateRange(UserId, new DateTime(Convert.ToInt32(dateFromYear), Convert.ToInt32(dateFromMonth), Convert.ToInt32(dateFromDay), Convert.ToInt32(timeHourFrom), Convert.ToInt32(timeMinuteFrom), 1),
    //                                                                     new DateTime(Convert.ToInt32(dateToYear), Convert.ToInt32(dateToMonth), Convert.ToInt32(dateToDay), Convert.ToInt32(timeHourTo), Convert.ToInt32(timeMinuteTo), 1));
    //    if (null == dt)
    //    {
    //        string errMess = ms.GetLastError();
    //    }
    //}

    //[WebMethod(EnableSession = true), Description("Get messages for Inbox filtered by Date and Time")]
    //public IEnumerable<MessageRow> GetRowsListFiltered_Inbox(string sortField, GridSortOrder sortOrder, int UserId,string dateFromYear,string dateFromMonth,string dateFromDay,string dateToYear,string dateToMonth,string dateToDay,string timeHourFrom,string timeMinuteFrom,string timeHourTo,string timeMinuteTo)
    //{
    //    try
    //    {
    //        if (!CheckSession(UserId)) throw new Exception();
    //        Init_InboundInformationFiltered(UserId, dateFromYear, dateFromMonth, dateFromDay, dateToYear, dateToMonth, dateToDay, timeHourFrom, timeMinuteFrom, timeHourTo, timeMinuteTo);
    //        if (null != dt)
    //        {
    //            if (sortOrder == GridSortOrder.Descending)
    //            {
    //                dt.DefaultView.Sort = sortField + " desc";
    //            }
    //            else
    //            {
    //                dt.DefaultView.Sort = sortField + " asc";
    //            }

    //            DataTable t_dt = dt.DefaultView.ToTable();
    //            var messagesrows = from row in t_dt.AsEnumerable()
    //                               select new MessageRow
    //                               {
    //                                   MessageId = row[0].ToString(),
    //                                   MessageReferenceId = row[1].ToString(),
    //                                   MessageFormatTypeId = row[2].ToString(),
    //                                   ReceivedTimestamp = row[3].ToString(),
    //                                   Subject = row[4].ToString(),
    //                                   FieldValues = row[5].ToString(),
    //                                   Sender = row[6].ToString(),
    //                                   SenderId = row[10].ToString(),
    //                                   Recipient = row[7].ToString(),
    //                                   IsRead = row[8].ToString(),
    //                                   MessageStatus = row[9].ToString()
    //                               };
    //            return messagesrows;
    //        }
    //        else
    //        {
    //            IEnumerable<MessageRow> empty = System.Linq.Enumerable.Empty<MessageRow>();
    //            return empty;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
         
    //        return null;
    //    }
    //}
  
    //[WebMethod(EnableSession = true), Description("Gets messages for Outbox")]
    //public IEnumerable<MessageRow> GetRowsListFiltered_Outbox(string sortField, GridSortOrder sortOrder, int UserId,string dateFromYear,string dateFromMonth,string dateFromDay,string dateToYear,string dateToMonth,string dateToDay,string timeHourFrom,string timeMinuteFrom,string timeHourTo,string timeMinuteTo)
    //{
    //    try
    //    {
    //        if (!CheckSession(UserId)) throw new Exception();
    //        Init_OutbounInformationFiltered(UserId, dateFromYear, dateFromMonth, dateFromDay, dateToYear, dateToMonth, dateToDay, timeHourFrom, timeMinuteFrom, timeHourTo, timeMinuteTo);
    //        if (null != dt)
    //        {
    //            if (sortOrder == GridSortOrder.Descending)
    //            {
    //                dt.DefaultView.Sort = sortField + " desc";
    //            }
    //            else
    //            {
    //                dt.DefaultView.Sort = sortField + " asc";
    //            }

    //            DataTable t_dt = dt.DefaultView.ToTable();
    //            var messagesrows = from row in t_dt.AsEnumerable()
    //                               select new MessageRow
    //                               {
    //                                   MessageId = row[0].ToString(),
    //                                   MessageReferenceId = row[1].ToString(),
    //                                   MessageFormatTypeId = row[2].ToString(),
    //                                   ReceivedTimestamp = row[3].ToString(),
    //                                   Subject = row[4].ToString(),
    //                                   FieldValues = row[5].ToString(),
    //                                   Sender = row[6].ToString(),
    //                                   Recipient = row[7].ToString(),
    //                                   IsRead = row[8].ToString(),
    //                                   MessageStatus = row[9].ToString()
    //                               };
    //            return messagesrows;
    //        }
    //        else
    //        {
    //            IEnumerable<MessageRow> empty = System.Linq.Enumerable.Empty<MessageRow>();
    //            return empty;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
           
    //        return null;
    //    }
    //}

    //[WebMethod(EnableSession = true), Description("Get Group List")]
    //public string GetGroupList(string userId)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    try
    //    {
    //        DataTable dt = new DataTable();
    //        sb.AppendFormat("{1}{0}{2}{0}", DELIMITER, -1, "Please select recipient group...");
    //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
    //        {
    //            using (SqlDataAdapter da = new SqlDataAdapter(string.Format("SELECT    vlfFleet.FleetId, vlfFleet.FleetName, vlfFleetUsers.UserId FROM  dbo.vlfFleet WITH (nolock) INNER JOIN dbo.vlfFleetUsers ON dbo.vlfFleet.FleetId = dbo.vlfFleetUsers.FleetId WHERE vlfFleetUsers.UserId = {0} ORDER BY dbo.vlfFleet.FleetName", userId), con))
    //            {
    //                con.Open();
    //                int result = da.Fill(dt);
    //                con.Close();
    //            }
    //        }
    //        if (dt == null) return sb.ToString();
    //        for (int a = 0; a < dt.Rows.Count; a++)
    //        {
    //            sb.AppendFormat("{1}{0}{2}{0}", DELIMITER, dt.Rows[a]["FleetId"].ToString(), dt.Rows[a]["FleetName"].ToString());
    //        }
    //    }
    //    catch (Exception ex)
    //    {
            
    //    }
    //    return sb.ToString();
    //}

    //[WebMethod(EnableSession = true), Description("Get Recipients List")]
    //public string GetRecipientList(string userId, string groupId)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    try
    //    {
    //        DataTable dt = new DataTable();
    //        sb.AppendFormat("{1}{0}{2}{0}", DELIMITER, -1, "Please select recipient group...");
    //        sb.AppendFormat("{1}{0}{2}{0}", DELIMITER, 0, "All recipients");

    //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
    //        {
    //            using (SqlDataAdapter da = new SqlDataAdapter(string.Format("SELECT vlfVehicleAssignment.BoxId, vlfVehicleInfo.Description FROM vlfFleetVehicles INNER JOIN vlfVehicleInfo ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId LEFT OUTER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId WHERE (vlfFleetVehicles.FleetId = {0}) ORDER BY vlfVehicleInfo.Description", groupId), con))
    //            {
    //                con.Open();
    //                int result = da.Fill(dt);
    //                con.Close();
    //            }
    //        }
    //        if (dt == null) return sb.ToString();

    //        for (int a = 0; a < dt.Rows.Count; a++)
    //        {
    //            sb.AppendFormat("{1}{0}{2}{0}", DELIMITER, dt.Rows[a]["BoxId"].ToString(), dt.Rows[a]["Description"].ToString());
    //        }
    //    }
    //    catch(Exception ex) {
            
    //    }
    //    return sb.ToString();
    //}

    //[WebMethod(EnableSession = true), Description("Send Message")]
    //public void SendMessage(bool MessageWithFormat, string MessageFormatId, string Sender, string Group, string Recepient, string  Subject, string Message)
    //{
    //    try
    //    {
    //        if (MessageWithFormat)
    //        {
    //            string[] values = Regex.Split(Message, "\r\n");

    //            //string[] values = Regex.Split(Message, "\r\n");
    //            //string[] val2 = new string[values.Length - 1];
    //            //Array.Copy(values, 0, val2, 0, val2.Length);


    //            MessageDAO.GetInstance(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString).Insert(Convert.ToInt64(MessageFormatId), Convert.ToInt64(Sender), Convert.ToInt64(Recepient), values);
    //        }
    //        else
    //        {
    //            string[] subject = new string[1];
    //            subject[0] = Subject;
    //            string[] lines = Regex.Split(Message, "\r\n");
    //            string[] values = new string[subject.Length + lines.Length];
                
    //    subject.CopyTo(values, 0);
    //            lines.CopyTo(values, subject.Length);

    //           // string[] val2 = new string[values.Length - 1];
    //           // Array.Copy(values, 0, val2, 0, val2.Length);

    //            MessageDAO.GetInstance(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString).Insert(Convert.ToInt64(MessageFormatId), Convert.ToInt64(Sender), Convert.ToInt64(Recepient), values);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
            
    //    }
    //    // throw new ArgumentException("A start-up parameter is required.");
    //}

    //[WebMethod(EnableSession = true), Description("Get Message")]
    //public string MessageView(string UserId, string MessageId)
    //{
    //    try
    //    {
    //        string view = string.Empty;
    //        MessageDAO.GetInstance(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString).View(Convert.ToInt64(UserId), Convert.ToInt64(MessageId), out view);
    //        return view;
    //    }
    //    catch (Exception ex)
    //    {
            
    //        return null;
    //    }
    //}

    //[WebMethod(EnableSession = true), Description("Get Message Format")]
    //public string View(string formatId)
    //{
    //    string view = string.Empty;
    //    try
    //    {
    //        DeviceMessageFormatDAO.GetInstance(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString).View(Convert.ToInt32(formatId), out view);
    //    }
    //    catch(Exception ex) 
    //    {
            
    //    }
    //    return view;
    //}

 
    //// [WebMethod(EnableSession = true), Description("Builds and returns Combo Box Context for Message Formats Combo")]
    ////// public static RadComboBoxData List(RadComboBoxContext context)
    ////public RadComboBoxData GetMessageFormats(RadComboBoxContext context)
    ////{
    ////    DataTable data = null;
    ////    RadComboBoxData comboData = new RadComboBoxData();
    ////    int result1 = DeviceMessageFormatDAO.GetInstance(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString).List(out data);
    ////    if (result1 == 0)
    ////    {
    ////        int itemOffset = context.NumberOfItems;
    ////        int endOffset = Math.Min(itemOffset + 10, data.Rows.Count);
    ////        comboData.EndOfItems = endOffset == data.Rows.Count;

    ////        List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(endOffset - itemOffset);

    ////        for (int i = itemOffset; i < endOffset; i++)
    ////        {
    ////            RadComboBoxItemData itemData = new RadComboBoxItemData();
    ////            itemData.Text = data.Rows[i]["Name"].ToString();
    ////            itemData.Value = data.Rows[i]["DeviceMessageFormatId"].ToString();

    ////            result.Add(itemData);
    ////        }

    ////        comboData.Message = GetStatusMessage(endOffset, data.Rows.Count);

    ////        comboData.Items = result.ToArray();
    ////    }
    ////    return comboData;
    ////}

    //[WebMethod(EnableSession = true), Description("Builds and returns Combo Box Context for Message Formats Combo")]
    //public string GetMessageFormatsList(int UserId)
    //{
    //    if (!CheckSession(UserId)) throw new Exception();
    //    StringBuilder sb = new StringBuilder();
    //    try
    //    {
    //        DataTable data = new DataTable();
    //        int result1 = DeviceMessageFormatDAO.GetInstance(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString).List(out data);
    //        if (result1 == 0)
    //        {
    //            for (int a = 0; a < data.Rows.Count; a++)
    //            {
    //                sb.AppendFormat("{1}{0}{2}{0}", DELIMITER, data.Rows[a]["DeviceMessageFormatId"].ToString(), data.Rows[a]["Name"].ToString());
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
            
    //    }
    //    return sb.ToString();
    //}
   
    //private static string GetStatusMessage(int offset, int total)
    //{
    //    if (total <= 0)
    //        return "No matches";

    //    return String.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", offset, total);
    //}
}


//[WebMethod(EnableSession = true), Description("Gets messages from Inbox")]
//public List<MessageRow> GetRowsList_Inbox()
//{
//    InitMessagesInformation(); 
//    List<MessageRow> messages = new List<MessageRow>();
//    messages = GetMessages("In");
//    return messages;
//} 
//[WebMethod(EnableSession = true), Description("Gets messages from Outbox")]
//public List<MessageRow> GetRowsList_Outbox()
//{
//    List<MessageRow> messages = new List<MessageRow>();
//    messages = GetMessages("Out");
//    return messages;
//}

//Returns List of Messages
//private List<MessageRow> GetMessages(string flag)
//{
//    List<MessageRow> list = new List<MessageRow>();
//    XmlDocument doc = new XmlDocument();
//    try
//    {
//        doc.Load(Server.MapPath("XMLFile.xml"));
//    }
//    catch 
//    {
//        throw;
//    }
//    XmlNodeList masterNode = doc.GetElementsByTagName("Master");

//    foreach (XmlNode node in masterNode)
//    {
//        XmlElement masterElement = (XmlElement)node;
//        if (masterElement.GetElementsByTagName("MsgDirection")[0].InnerText.Equals(flag,StringComparison.InvariantCultureIgnoreCase)) {
//            string _rowId = masterElement.GetElementsByTagName("RowId")[0].InnerText;
//            string _from = masterElement.GetElementsByTagName("From")[0].InnerText;
//            string _to = masterElement.GetElementsByTagName("To")[0].InnerText;
//            string _msgBody = "Flag->"+flag+ " " + masterElement.GetElementsByTagName("MsgBody")[0].InnerText;

//            if (_msgBody.Length > 50)
//            {
//                _msgBody = _msgBody.Substring(0, 50);
//            }

//            string _msgBodyFull = "Flag->" + flag + " " + masterElement.GetElementsByTagName("MsgBody")[0].InnerText;
//            string _msgDateTime = masterElement.GetElementsByTagName("MsgDateTime")[0].InnerText;

//            list.Add(new MessageRow(_rowId, _from, _to, _msgBody, _msgBodyFull, _msgDateTime));
//        }   
//    }
//    return list;
//}
//[WebMethod, Description("Returns List of Messages")]
//public IEnumerable<MessageRow> GetData(string flag, string sortField, GridSortOrder sortOrder)
//{
//    var path = HttpContext.Current.Server.MapPath(@"XMLFile.xml");
//    var xml = System.Xml.Linq.XDocument.Load(path).Descendants("Master");

//    if (sortOrder == GridSortOrder.Descending)
//    {
//        xml = xml.OrderBy(e => e.Element(sortField).Value);
//    }
//    else if (sortOrder == GridSortOrder.None)
//    {
//        xml = xml.OrderByDescending(e => e.Element(sortField).Value);
//    }

//    var messagesrows = from row in xml
//                   where row.Element("MsgDirection").Value.Equals(flag, StringComparison.InvariantCultureIgnoreCase) 
//                   select new MessageRow
//                   {
//                       RowId = row.Element("RowId").Value,
//                       From = row.Element("From").Value,
//                       To = row.Element("To").Value,
//                       MsgBody = row.Element("MsgBody").Value.Length  > 50 ? row.Element("MsgBody").Value.Substring(0, 50) : row.Element("MsgBody").Value,// contact.Element("MsgBody").Value > 50? contact.Element("MsgBody").Value.substring(0,50) : contact.Element("MsgBody").Value
//                       MsgBodyFull = row.Element("MsgBody").Value,
//                       MsgDateTime = row.Element("MsgDateTime").Value
//                   };
//    return messagesrows;                   
//}

//[WebMethod]
//[ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
//public string HelloWorld()
//{
//    return "HELLO";
//    //JavaScriptSerializer js = new JavaScriptSerializer();// Use this when formatting the data as JSON
//    // return js.Serialize("Hello World");        
//}

