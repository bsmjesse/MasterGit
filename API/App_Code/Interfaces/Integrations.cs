using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity;
using VLF.DAS.Logic;
using System.Data;
using System.Diagnostics;
using VLF.CLS;

using System.Xml;
using System.IO;
using System.Drawing;

//using Microsoft.Web.Services3;

namespace VLF.ASI.Interfaces
{
    //LogFolder
    /// <summary>
    /// Summary description for BSMCustomService
    /// </summary>
    [WebService(Namespace = "http://www.sentinelfm.com", Description = "Sentinel Web Service Methods Exposer.", Name = "Integrations")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[WebServiceBinding(ConformsTo = WsiProfiles.None)]
    public class Integrations : System.Web.Services.WebService
    {
        string DBConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        string XMLResponse = "<?xml version=\"1.0\" encoding=\"utf-8\"?><StatusDescription>BODY: {0}</StatusDescription>";
        int RecordsProcessed = 0;
        int RecordCount = 0;
        string StatusDetails = String.Empty;
        string InvalidNodes = String.Empty;
        int iResponseHttpStatusCode = 0;

        public Integrations()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        #region Component Designer generated code

        ////Required by the Web Services Designer 
        //private IContainer components = null;

        ///// <summary>
        ///// Required method for Designer support - do not modify
        ///// the contents of this method with the code editor.
        ///// </summary>
        //private void InitializeComponent()
        //{
        //}

        ///// <summary>
        ///// Clean up any resources being used.
        ///// </summary>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && components != null)
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #endregion

        #region LOG

        //private void Log(string strFormat, params object[] objects)
        //{
        //    try
        //    {
        //        Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format(strFormat, objects)));
        //    }
        //    catch { }
        //}

        private void LogException(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error, string.Format(strFormat, objects)));
            }
            catch { }
        }

        #endregion LOG

        #region VALIDATION
        //private bool ValidateUserRequest(int userId, string SID)
        //{
        //    bool ret = false;
        //    try
        //    {
        //        // Authenticate & Authorize
        //        LoginManager.GetInstance().SecurityCheck(userId, SID);

        //        //Authorization
        //        ValidationResult res = SecurityKeyManager.GetInstance().ValidatePasskey(userId, SID);

        //        switch (res)
        //        {
        //            case ValidationResult.Failed:
        //            case ValidationResult.Expired:
        //            case ValidationResult.CallFrequencyExceeded:
        //                ret = false;
        //                break;
        //            default:
        //                ret = true;
        //                break;
        //        }
        //    }
        //    catch
        //    {
        //        throw new Exception("SecurityCheck: Validation failed.");
        //    }

        //    return ret;
        //}
        #endregion VALIDATION

        /// <summary>
        /// Take inputXML and perform ADD/EDIT.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="inputXML"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Takes XML of Hierarchy/Equipment/Employee (predefined format) and returns HTTP Status Code and HTTP Status Description.")]
        public void Post(string token, string xmlString)
        {
            string status = String.Empty;
            string xmlResponseString = String.Empty;
            int AuditLogXmlID = 0;
            int userId = 0;
            DataSet dsIntegrationWebServiceSettings = new DataSet();
            int TransactionsSubmitted = 0;
            int RecordsPerBatch = 0;
            int NumberOfTransactions = 0;
            string Batch_Number = String.Empty;
            string xmlStringUpdated = String.Empty;
            bool bLogXML = true;

            //token = "d@d192!94275b52$a489f272674308&85bb2"; //SFM 2000
            //token = "129f9d$e328023caae@345dd7d22ce4&a12";

            //xmlString = "<XMLdoc><Batch_Number>OH0050568353FE1EE491921ACD5C5F28A8</Batch_Number><Hierarchy><Node><Hierarchy_Nr></Hierarchy_Nr><Action_Flag>M</Action_Flag><Hierarchy_Name>Southeast</Hierarchy_Name><Hierarchy_Level>2</Hierarchy_Level><Parent_Node>SC3000</Parent_Node><Create_Date>2014-09-25 08:49:19</Create_Date><Update_Date>2014-09-25 08:49:19</Update_Date></Node><Node><Hierarchy_Nr>SE3002</Hierarchy_Nr><Action_Flag>M</Action_Flag><Hierarchy_Name>Southeast2</Hierarchy_Name><Hierarchy_Level></Hierarchy_Level><Parent_Node>SC3000</Parent_Node><Create_Date>2014-09-25 08:49:19</Create_Date><Update_Date>2014-09-25 08:49:19</Update_Date></Node></Hierarchy></XMLdoc>";

            if (String.IsNullOrEmpty(token.Trim()))
            {
                status = "Invalid user. ";
                StatusDetails = status;
                WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                return;
            }
            else if (String.IsNullOrEmpty(xmlString.Trim()))
            {
                status = "XML string is empty. ";
                StatusDetails = status;
                WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                return;
            }

            token = HttpUtility.HtmlDecode(token.Trim());
            xmlStringUpdated = HttpUtility.HtmlDecode(xmlString.Trim());

            try
            {
                using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                {
                    userId = iDBIntegrations.GetUserByHashPassword(token);
                }

                if (userId == 0)
                {
                    status = "Invalid user. ";
                    StatusDetails = status;
                    WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                    return;
                }

                xmlStringUpdated = xmlStringUpdated.Replace("&apos;", "'");

                xmlStringUpdated = xmlStringUpdated.Replace("<First_Nm>", "<First_Nm><![CDATA[").Replace("</First_Nm>", "]]></First_Nm>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Last_Nm>", "<Last_Nm><![CDATA[").Replace("</Last_Nm>", "]]></Last_Nm>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Email_Address>", "<Email_Address><![CDATA[").Replace("</Email_Address>", "]]></Email_Address>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Position_DS>", "<Position_DS><![CDATA[").Replace("</Position_DS>", "]]></Position_DS>");

                xmlStringUpdated = xmlStringUpdated.Replace("<Hierarchy_Nr>", "<Hierarchy_Nr><![CDATA[").Replace("</Hierarchy_Nr>", "]]></Hierarchy_Nr>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Project_Nr>", "<Project_Nr><![CDATA[").Replace("</Project_Nr>", "]]></Project_Nr>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Owning_Dist>", "<Owning_Dist><![CDATA[").Replace("</Owning_Dist>", "]]></Owning_Dist>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Record_Source_ID>", "<Record_Source_ID><![CDATA[").Replace("</Record_Source_ID>", "]]></Record_Source_ID>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Hierarchy_Name>", "<Hierarchy_Name><![CDATA[").Replace("</Hierarchy_Name>", "]]></Hierarchy_Name>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Project_Equipment_Superintendent>", "<Project_Equipment_Superintendent><![CDATA[").Replace("</Project_Equipment_Superintendent>", "]]></Project_Equipment_Superintendent>");

                xmlStringUpdated = xmlStringUpdated.Replace("<Equipment_Ds>", "<Equipment_Ds><![CDATA[").Replace("</Equipment_Ds>", "]]></Equipment_Ds>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Make>", "<Make><![CDATA[").Replace("</Make>", "]]></Make>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Model>", "<Model><![CDATA[").Replace("</Model>", "]]></Model>");
                xmlStringUpdated = xmlStringUpdated.Replace("<Short_Desc>", "<Short_Desc><![CDATA[").Replace("</Short_Desc>", "]]></Short_Desc>");

                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlStringUpdated);
                if (xDoc.DocumentElement == null)
                {
                    status = status + "Root element is empty. ";
                    StatusDetails = status;
                    WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                    return;
                }

                if (String.IsNullOrEmpty(status))
                {
                    using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                    {
                        dsIntegrationWebServiceSettings = iDBIntegrations.GetIntegrationWebServiceSettings(userId);
                    }

                    if (dsIntegrationWebServiceSettings != null)
                    {
                        DataTable dtIntegrationWebServiceSettings = dsIntegrationWebServiceSettings.Tables[0];
                        if (dtIntegrationWebServiceSettings.Rows.Count > 0)
                        {
                            if (dtIntegrationWebServiceSettings.Rows[0]["TransactionsSubmitted"] != DBNull.Value)
                                TransactionsSubmitted = Convert.ToInt32(dtIntegrationWebServiceSettings.Rows[0]["TransactionsSubmitted"]);
                            if (dtIntegrationWebServiceSettings.Rows[0]["RecordsPerBatch"] != DBNull.Value)
                                RecordsPerBatch = Convert.ToInt32(dtIntegrationWebServiceSettings.Rows[0]["RecordsPerBatch"]);
                            if (dtIntegrationWebServiceSettings.Rows[0]["NumberOfTransactions"] != DBNull.Value)
                                NumberOfTransactions = Convert.ToInt32(dtIntegrationWebServiceSettings.Rows[0]["NumberOfTransactions"]);
                        }
                    }

                    if (TransactionsSubmitted >= NumberOfTransactions)
                    {
                        status = status + "Daily transaction limit is reached. ";
                        StatusDetails = status;
                        WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                        return;
                    }
                }

                if (String.IsNullOrEmpty(status))
                {
                    //Batch_Number
                    XmlNode Batch_NumberNode = xDoc.SelectSingleNode("/XMLdoc/Batch_Number");
                    if (Batch_NumberNode != null)
                    {
                        Batch_Number = Batch_NumberNode.InnerText;
                    }

                    XmlNode EquipmentListNode = xDoc.SelectSingleNode("/XMLdoc/EquipmentList");
                    if (EquipmentListNode != null)
                    {
                        XmlNodeList Equipment = EquipmentListNode.SelectNodes("Equipment");
                        RecordCount = Equipment.Count;
                        if (RecordCount > RecordsPerBatch)
                        {
                            status = status + String.Format("XML must have {0} or less Equipment nodes. ", RecordsPerBatch.ToString());
                            StatusDetails = status;
                            bLogXML = false;
                            WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                            return;
                        }

                        status = status + ProcessEquipment(EquipmentListNode, userId);
                    }

                    XmlNode EmployeeListNode = xDoc.SelectSingleNode("/XMLdoc/EmployeeList");
                    if (EmployeeListNode != null)
                    {
                        XmlNodeList Employee = EmployeeListNode.SelectNodes("Employee");
                        RecordCount = Employee.Count;
                        if (RecordCount > RecordsPerBatch)
                        {
                            status = status + String.Format("XML must have {0} or less Employee nodes. ", RecordsPerBatch.ToString());
                            StatusDetails = status;
                            bLogXML = false;
                            WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                            return;
                        }

                        status = status + ProcessEmployees(EmployeeListNode, userId);
                    }

                    XmlNode HierarchyNode = xDoc.SelectSingleNode("/XMLdoc/Hierarchy");
                    if (HierarchyNode != null)
                    {
                        XmlNodeList Node = HierarchyNode.SelectNodes("Node");
                        RecordCount = Node.Count;
                        if (RecordCount > RecordsPerBatch)
                        {
                            status = status + String.Format("XML must have {0} or less Hierarchy nodes. ", RecordsPerBatch.ToString());
                            StatusDetails = status;
                            bLogXML = false;
                            WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                            return;
                        }

                        status = status + ProcessHierarchy(HierarchyNode, userId);
                    }

                    if (RecordCount == 0)
                    {
                        status = status + "Xml document is not well formed. ";
                        StatusDetails = status;
                        WebServiceResponse(status, (int)System.Net.HttpStatusCode.PreconditionFailed);
                        return;
                    }

                    if (RecordCount > 1)
                        iResponseHttpStatusCode = (int)System.Net.HttpStatusCode.NotAcceptable;
                    else
                        iResponseHttpStatusCode = (int)System.Net.HttpStatusCode.ExpectationFailed;

                    if (!String.IsNullOrEmpty(status))
                    {
                        WebServiceResponse(status, iResponseHttpStatusCode);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("<< Integrations - Post: uId={0}, EXC={1})", userId, ex.Message);
                status = status + "Error to process XML. ";
                StatusDetails = status + ex.Message + ". ";
            }
            finally
            {
                if (String.IsNullOrEmpty(status))
                    status = "Success";

                if (bLogXML == false)
                    xmlString = String.Empty;

                //log xml
                using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                {
                    AuditLogXmlID = iDBIntegrations.AddAuditLogXml(userId, xmlString, status, Batch_Number, RecordCount, RecordsProcessed, StatusDetails);
                }

                if (status == "Success")
                    WebServiceResponse(status, (int)System.Net.HttpStatusCode.Accepted);
                else
                    WebServiceResponse(status, iResponseHttpStatusCode);
            }

            return;
        }

        private void WebServiceResponse(string status, int ResponseHttpStatusCode)
        {
            string StatusDescription = String.Empty;

            //status = "Project_Nr must be provided for Serial_Number 1FAHP2EW8BG153121 (node 1). Project_Nr must be provided for Serial_Number 1FAHP2EWXBG153122 (node 2). Project_Nr must be provided for Serial_Number 1FAHP2EW1BG153123 (node 3). Project_Nr must be provided for Serial_Number 1FAFP23175G121506 (node 4). Project_Nr must be provided for Serial_Number 1FAFP23155G129751 (node 5). Project_Nr must be provided for Serial_Number 1FAFP53U73G219458 (node 6). Project_Nr must be provided for Serial_Number 1FAFP231X5G176967 (node 7). Project_Nr must be provided for Serial_Number 1FAHP24197G146725 (node 8). Project_Nr must be provided for Serial_Number 1FAHP24177F118678 (node 9). Project_Nr must be provided for Serial_Number 1FAFP24147G110531 (node 10). Project_Nr must be provided for Serial_Number 1FAFP24127G110530 (node 11). Project_Nr must be provided for Serial_Number 1FAFP24197G109133 (node 12). Project_Nr must be provided for Serial_Number 1FAFP24167G110529 (node 13). Project_Nr must be provided for Serial_Number 1FAHP24157G137908 (node 14). Project_Nr must be provided for Serial_Number 1FAHP24177G152264 (node 15). Project_Nr must be provided for Serial_Number 1FTRF122X6KD20160 (node 16). Project_Nr must be provided for Serial_Number 1FTRF12236KD20159 (node 17). Project_Nr must be provided for Serial_Number 1FTRF12216KD20158 (node 18). Project_Nr must be provided for Serial_Number 1FTRF12286KD20156 (node 19). Project_Nr must be provided for Serial_Number 1FTRF12266KD20155 (node 20). Project_Nr must be provided for Serial_Number 1FTRF12246KD20154 (node 21). Project_Nr must be provided for Serial_Number 1FTRF12226KD20153 (node 22). Project_Nr must be provided for Serial_Number 1FTVF125X7NA67401 (node 23). Project_Nr must be provided for Serial_Number 1FTVF12587NA67400 (node 24). Project_Nr must be provided for Serial_Number 1FTVF12557NA67399 (node 25). Project_Nr must be provided for Serial_Number 1FTVF12537NA67398 (node 26). Project_Nr must be provided for Serial_Number 1FTVF12526NB57110 (node 27). Project_Nr must be provided for Serial_Number 1FTVF12566NB57109 (node 28). Project_Nr must be provided for Serial_Number 1FTVF12546NB57108 (node 29). Project_Nr must be provided for Serial_Number 1FTRF12265KD83173 (node 30). Project_Nr must be provided for Serial_Number 1FTVF12557NA67371 (node 31). Project_Nr must be provided for Serial_Number 1FTVF12517NA67366 (node 32). Project_Nr must be provided for Serial_Number 1FTRF12298KE27378 (node 33). Project_Nr must be provided for Serial_Number 1FTRF12278KE27377 (node 34). Project_Nr must be provided for Serial_Number 1FTRF12298KE27381 (node 35). Project_Nr must be provided for Serial_Number 1FTRF12278KE27380 (node 36). Project_Nr must be provided for Serial_Number 1FTRF12208KE27379 (node 37). Project_Nr must be provided for Serial_Number 1FTRF12258KE27376 (node 38). Project_Nr must be provided for Serial_Number 1FTPF1CT3CKD53897 (node 39). Project_Nr must be provided for Serial_Number 1FTPF1CT2CKD53907 (node 40). Project_Nr must be provided for Serial_Number 1FTPF1CT4CKD53908 (node 41). Project_Nr must be provided for Serial_Number 1FTPF1CT6CKD53909 (node 42). Project_Nr must be provided for Serial_Number 1FTPF1CT2CKD53910 (node 43). Project_Nr must be provided for Serial_Number 1FTPF1CT4CKD53911 (node 44). Project_Nr must be provided for Serial_Number 1FTPF1CT6CKD53912 (node 45). Project_Nr must be provided for Serial_Number 1FTPF1CT1CKD53915 (node 46). Project_Nr must be provided for Serial_Number 1FTPF1CT7CKD53918 (node 47). Project_Nr must be provided for Serial_Number 1FTPF1CV6AKE27442 (node 48). Project_Nr must be provided for Serial_Number 1FTPF1CV7AKE27417 (node 49). Project_Nr must be provided for Serial_Number 1FTPF1CV0AKE27422 (node 50). Project_Nr must be provided for Serial_Number 1FTPF1CV9AKE27421 (node 51). Project_Nr must be provided for Serial_Number 1FTPF1CV7AKE27420 (node 52). Project_Nr must be provided for Serial_Number 1FTPF1CV0AKE27419 (node 53). Project_Nr must be provided for Serial_Number 1FTPF1CV9AKE27418 (node 54). Project_Nr must be provided for Serial_Number 1FTPF1CVXAKE27444 (node 55). Project_Nr must be provided for Serial_Number 1FTPF1CV8AKE27443 (node 56). Project_Nr must be provided for Serial_Number 1FTPF1CV4AKE27441 (node 57). Project_Nr must be provided for Serial_Number 1FTPF1CV2AKE27440 (node 58). Project_Nr must be provided for Serial_Number 1FTPF1CV6AKE27439 (node 59). Project_Nr must be provided for Serial_Number 1FTPF1CV4AKE27438 (node 60). Project_Nr must be provided for Serial_Number 1FTPF1CV2AKE27437 (node 61). Project_Nr must be provided for Serial_Number 1FTRF12245KD83124 (node 62). Project_Nr must be provided for Serial_Number 1FTRF12W05KB39036 (node 63). Project_Nr must be provided for Serial_Number 1FTRF12W95KB39035 (node 64). Project_Nr must be provided for Serial_Number 2FTPF17L02CA81628 (node 65). Project_Nr must be provided for Serial_Number 1FTRF172X2KD42601 (node 66). Project_Nr must be provided for Serial_Number 1FTRF17282KD42600 (node 67). Project_Nr must be provided for Serial_Number 2FTPF17L22CA81629 (node 68). Project_Nr must be provided for Serial_Number 2FTRF17WX3CA84959 (node 69). Project_Nr must be provided for Serial_Number 2FTPF17L22CA81632 (node 70). Project_Nr must be provided for Serial_Number 2FTPF17L92CA81630 (node 71). Project_Nr must be provided for Serial_Number 2FTRF17223CA56523 (node 72). Project_Nr must be provided for Serial_Number 1FTNF20L72ED21498 (node 73). Project_Nr must be provided for Serial_Number 1FTNE24L71HA99023 (node 74). Project_Nr must be provided for Serial_Number 1HTWCAZR9CJ620761 (node 75). Project_Nr must be provided for Serial_Number 1HTWGAZR37J405936 (node 76). Project_Nr must be provided for Serial_Number 1FDAF56P66EC81829 (node 77). Project_Nr must be provided for Serial_Number 1FDAF56R99EB05878 (node 78). Project_Nr must be provided for Serial_Number 1FDAF56F1YEC86902 (node 79). Project_Nr must be provided for Serial_Number 3FRNF65R36V362342 (node 80). Project_Nr must be provided for Serial_Number 1FVHCYDC07DY17327 (node 81). Project_Nr must be provided for Serial_Number 1FVHCYBS49DAD1851 (node 82). Project_Nr must be provided for Serial_Number 1FDRF3GT5BEA03131 (node 85). Project_Nr must be provided for Serial_Number 1FDAF56F12ED49374 (node 86). Project_Nr must be provided for Serial_Number 3FDNF65401MA47199 (node 87). Project_Nr must be provided for Serial_Number 1FDWE3FL4ADA05853 (node 88). Project_Nr must be provided for Serial_Number 1FBSS31S91HA98943 (node 89). Project_Nr must be provided for Serial_Number W098676221PS17435 (node 90). Project_Nr must be provided for Serial_Number W098676221PS17436 (node 91). Project_Nr must be provided for Serial_Number W098676221PS17431 (node 92). Project_Nr must be provided for Serial_Number W098676221PS17432 (node 93). Project_Nr must be provided for Serial_Number W098676221PS17434 (node 94). Project_Nr must be provided for Serial_Number W098676221PS17433 (node 95). Project_Nr must be provided for Serial_Number W098756252PS17532 (node 96). Project_Nr must be provided for Serial_Number W0987566232PS17531 (node 97). Project_Nr must be provided for Serial_Number W098670001PS17472 (node 98). Project_Nr must be provided for Serial_Number W098670001PS17471 (node 99). Project_Nr must be provided for Serial_Number 38957-C (node 100). Project_Nr must be provided for Serial_Number 8068 (node 101). Project_Nr must be provided for Serial_Number 1UYFS245XLA337605 (node 102). Project_Nr must be provided for Serial_Number 1H2P04822RW071701 (node 103). Project_Nr must be provided for Serial_Number 1H2P0482SW017502 (node 104). Project_Nr must be provided for Serial_Number 1TTF48200T1049728 (node 105). Project_Nr must be provided for Serial_Number 1H2P04524RW051602 (node 106). Project_Nr must be provided for Serial_Number 1TTF4820951046664 (node 107). Equipment Serial_Number must be provided (node 108). Project_Nr must be provided for Serial_Number 1TTF4820951046664 (node 108). Equipment_Yr must be provided for Serial_Number 1TTF4820951046664 (node 108). Equipment Serial_Number must be provided (node 109). Project_Nr must be provided for Serial_Number 1TTF4820951046664 (node 109). Equipment_Yr must be provided for Serial_Number 1TTF4820951046664 (node 109). Project_Nr must be provided for Serial_Number 123FAKEVIN (node 110). Equipment_Yr must be provided for Serial_Number 123FAKEVIN (node 110). Project_Nr must be provided for Serial_Number 1FTVF125X8KE32160 (node 111). Equipment_Yr must be provided for Serial_Number 1FTVF125X8KE32160 (node 111). Equipment Serial_Number must be provided (node 112). Project_Nr must be provided for Serial_Number 1FTVF125X8KE32160 (node 112). Equipment_Yr must be provided for Serial_Number 1FTVF125X8KE32160 (node 112). Project_Nr must be provided for Serial_Number 6HK00681 (node 113). Project_Nr must be provided for Serial_Number 6HK00681 (node 114). Equipment_Yr must be provided for Serial_Number 6HK00681 (node 114). Project_Nr must be provided for Serial_Number BDA00933 (node 115). Project_Nr must be provided for Serial_Number BDA00933 (node 116). Project_Nr must be provided for Serial_Number B1M02165 (node 117). Equipment_Yr must be provided for Serial_Number B1M02165 (node 117). Project_Nr must be provided for Serial_Number B1M02166 (node 118). Equipment_Yr must be provided for Serial_Number B1M02166 (node 118). Project_Nr must be provided for Serial_Number B1M02167 (node 119). Equipment_Yr must be provided for Serial_Number B1M02167 (node 119). Equipment Serial_Number must be provided (node 120). Equipment_Yr must be provided for Serial_Number B1M02167 (node 120). Equipment Serial_Number must be provided (node 121). Project_Nr must be provided for Serial_Number B1M02167 (node 121). Equipment_Yr must be provided for Serial_Number B1M02167 (node 121). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000001 (node 122). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000001 (node 122). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000002 (node 123). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000002 (node 123). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000003 (node 124). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000006 (node 127). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000008 (node 129). Project_Nr must be provided for Serial_Number TESTKRANTHI (node 132). Equipment_Yr must be provided for Serial_Number TESTKRANTHI (node 132). Project_Nr must be provided for Serial_Number TESTKRANTHI (node 133). Equipment_Yr must be provided for Serial_Number TESTKRANTHI (node 133). Project_Nr must be provided for Serial_Number TESTKRANTHI (node 134). Equipment_Yr must be provided for Serial_Number TESTKRANTHI (node 134). Equipment Serial_Number must be provided (node 135). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000011 (node 136). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000015 (node 139). Equipment_Yr must be provided for Serial_Number TESTKRANTHI (node 140). Equipment_Yr must be provided for Serial_Number TESTKRANTHI (node 141). Equipment Serial_Number must be provided (node 142). Equipment_Yr must be provided for Serial_Number TESTKRANTHI (node 142). Equipment Serial_Number must be provided (node 144). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 144). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 144). Equipment Serial_Number must be provided (node 145). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 145). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 145). Equipment Serial_Number must be provided (node 146). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 146). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 146). Equipment Serial_Number must be provided (node 147). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 147). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 147). Equipment Serial_Number must be provided (node 148). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 148). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 148). Equipment Serial_Number must be provided (node 149). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 149). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 149). Equipment Serial_Number must be provided (node 150). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 150). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 150). Equipment Serial_Number must be provided (node 151). Project_Nr must be provided for Serial_Number GRETCHENSTEST0000018 (node 151). Equipment_Yr must be provided for Serial_Number GRETCHENSTEST0000018 (node 151). Error to process XML. Specified argument was out of the range of valid values.  Parameter name: value. ";
            //InvalidNodes = "1, 2, 3, 8, 9, ";

            if (!String.IsNullOrEmpty(InvalidNodes))
                status = String.Format("Processed {0} records from {1}. Invalid nodes {2}. ", RecordsProcessed.ToString(), RecordCount.ToString(), InvalidNodes.Substring(0, InvalidNodes.Length - 2)) + status;
            else
                status = String.Format("Processed {0} records from {1}. ", RecordsProcessed.ToString(), RecordCount.ToString()) + status;

            if (status.Length > 500)
                StatusDescription = status.Substring(0, 450) + "... See HTML body for full response status.";
            else
                StatusDescription = status;

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/xml";
            HttpContext.Current.Response.StatusCode = ResponseHttpStatusCode;
            HttpContext.Current.Response.StatusDescription = StatusDescription;
            HttpContext.Current.Response.AddHeader("Content-Length", String.Format(XMLResponse, status).Length.ToString());
            HttpContext.Current.Response.Write(String.Format(XMLResponse, status));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Close();
        }

        private string ProcessEquipment(XmlNode MainNode, int UserId)
        {
            string status = String.Empty;
            string Serial_Number = String.Empty;
            string Equipment_Id = String.Empty;
            string SAP_Equipment_Nr = String.Empty;
            string Legacy_Equipment_Nr = String.Empty;
            string Object_Type = String.Empty;
            string Project_Nr = String.Empty;
            string Equipment_Ds = String.Empty;
            string Make = String.Empty;
            string Model = String.Empty;
            string Equipment_Yr = String.Empty;
            double Fuel_Capacity = 0;
            double Avg_Fuel_Burn_Rate = 0;
            string Create_Date = String.Empty;
            string Update_Date = String.Empty;
            string SOLD_DT = String.Empty;
            string EQP_ACQ_DT = String.Empty;
            string EQP_Retire_DT = String.Empty;
            string EQP_Category = String.Empty;
            int EQP_Weight = 0;
            string EQP_Weight_Unit = String.Empty;
            string DOT_Nr = String.Empty;
            string Action_Flag = String.Empty;
            string Object_Prefix = String.Empty;
            string Owning_Dist = String.Empty;
            int VehicleId = 0;
            double Total_Ctr_Reading = 0;
            string Total_Ctr_Reading_UOM = String.Empty;
            string Short_Desc = String.Empty;
            int ctr = 0;
            bool bError = false;

            XmlNodeList Equipment = MainNode.SelectNodes("Equipment");

            foreach (XmlNode item in Equipment)
            {
                try
                {
                    bError = false;
                    ctr = ctr + 1;

                    if (item["Serial_Number"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Serial_Number"].InnerText.Trim()))
                            Serial_Number = item["Serial_Number"].InnerText;
                        else
                        {
                            status = status + String.Format("Equipment Serial_Number must be provided (node {0}). ", ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    }
                    else
                    {
                        status = status + String.Format("Equipment Serial_Number must be provided (node {0}). ", ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Equipment_Id"] != null)
                        Equipment_Id = item["Equipment_Id"].InnerText;
                    else
                        Equipment_Id = String.Empty;

                    if (item["SAP_Equipment_Nr"] != null)
                        SAP_Equipment_Nr = item["SAP_Equipment_Nr"].InnerText;
                    else
                        SAP_Equipment_Nr = String.Empty;

                    if (item["Legacy_Equipment_Nr"] != null)
                        Legacy_Equipment_Nr = item["Legacy_Equipment_Nr"].InnerText;
                    else
                        Legacy_Equipment_Nr = String.Empty;

                    if (item["Object_Type"] != null)
                        Object_Type = item["Object_Type"].InnerText;
                    else
                        Object_Type = String.Empty;

                    if (item["Project_Nr"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Project_Nr"].InnerText.Trim()))
                            Project_Nr = item["Project_Nr"].InnerText.Trim();
                        else
                        {
                            status = status + String.Format("Project_Nr must be provided for Serial_Number {0} (node {1}). ", Serial_Number, ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    }
                    else
                    {
                        status = status + String.Format("Project_Nr must be provided for Serial_Number {0} (node {1}). ", Serial_Number, ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Equipment_Ds"] != null)
                        Equipment_Ds = item["Equipment_Ds"].InnerText;
                    else
                        Equipment_Ds = String.Empty;

                    if (item["Make"] != null)
                        Make = item["Make"].InnerText;
                    else
                        Make = String.Empty;

                    if (item["Model"] != null)
                        Model = item["Model"].InnerText;
                    else
                        Model = String.Empty;

                    if (item["Equipment_Yr"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Equipment_Yr"].InnerText))
                            Equipment_Yr = item["Equipment_Yr"].InnerText;
                        else
                        {
                            status = status + String.Format("Equipment_Yr must be provided for Serial_Number {0} (node {1}). ", Serial_Number, ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    }
                    else
                    {
                        status = status + String.Format("Equipment_Yr must be provided for Serial_Number {0} (node {1}). ", Serial_Number, ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Fuel_Capacity"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Fuel_Capacity"].InnerText.Trim()))
                            Fuel_Capacity = Convert.ToDouble(item["Fuel_Capacity"].InnerText.Trim());
                    }

                    if (item["Avg_Fuel_Burn_Rate"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Avg_Fuel_Burn_Rate"].InnerText.Trim()))
                            Avg_Fuel_Burn_Rate = Convert.ToDouble(item["Avg_Fuel_Burn_Rate"].InnerText.Trim());
                    }

                    if (item["Create_Date"] != null)
                        Create_Date = item["Create_Date"].InnerText;
                    else
                        Create_Date = String.Empty;

                    if (item["Update_Date"] != null)
                        Update_Date = item["Update_Date"].InnerText;
                    else
                        Update_Date = String.Empty;

                    if (item["SOLD_DT"] != null)
                        SOLD_DT = item["SOLD_DT"].InnerText;
                    else
                        SOLD_DT = String.Empty;

                    if (item["EQP_ACQ_DT"] != null)
                        EQP_ACQ_DT = item["EQP_ACQ_DT"].InnerText;
                    else
                        EQP_ACQ_DT = String.Empty;

                    if (item["EQP_Retire_DT"] != null)
                        EQP_Retire_DT = item["EQP_Retire_DT"].InnerText;
                    else
                        EQP_Retire_DT = String.Empty;

                    if (item["EQP_Category"] != null)
                        EQP_Category = item["EQP_Category"].InnerText;
                    else
                        EQP_Category = String.Empty;

                    if (item["EQP_Weight"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["EQP_Weight"].InnerText.Trim()))
                            EQP_Weight = Convert.ToInt32(Convert.ToDouble(item["EQP_Weight"].InnerText.Trim()));
                    }

                    if (item["EQP_Weight_Unit"] != null)
                        EQP_Weight_Unit = item["EQP_Weight_Unit"].InnerText;
                    else
                        EQP_Weight_Unit = String.Empty;

                    if (item["DOT_Nr"] != null)
                        DOT_Nr = item["DOT_Nr"].InnerText;
                    else
                        DOT_Nr = String.Empty;

                    if (item["Action_Flag"] != null)
                        Action_Flag = item["Action_Flag"].InnerText;
                    else
                        Action_Flag = String.Empty;

                    if (item["Object_Prefix"] != null)
                        Object_Prefix = item["Object_Prefix"].InnerText;
                    else
                        Object_Prefix = String.Empty;

                    if (item["Owning_Dist"] != null)
                        Owning_Dist = item["Owning_Dist"].InnerText;
                    else
                        Owning_Dist = String.Empty;

                    if (item["Total_Ctr_Reading"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Total_Ctr_Reading"].InnerText.Trim()))
                            Total_Ctr_Reading = Convert.ToDouble(item["Total_Ctr_Reading"].InnerText.Trim());
                    }

                    if (item["Total_Ctr_Reading_UOM"] != null)
                        Total_Ctr_Reading_UOM = item["Total_Ctr_Reading_UOM"].InnerText;
                    else
                        Total_Ctr_Reading_UOM = String.Empty;

                    if (item["Short_Desc"] != null)
                        Short_Desc = item["Short_Desc"].InnerText;
                    else
                        Short_Desc = String.Empty;

                    if (bError == false)
                    {
                        using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                        {
                            VehicleId = iDBIntegrations.UpdateVehicleFromWebService(Action_Flag, Serial_Number, Convert.ToInt16(Equipment_Yr), Equipment_Ds, EQP_Weight, Fuel_Capacity,
                                        Avg_Fuel_Burn_Rate, EQP_Weight_Unit, Create_Date, Update_Date, Equipment_Id, SAP_Equipment_Nr, Legacy_Equipment_Nr,
                                        Object_Type, DOT_Nr, EQP_Category, EQP_ACQ_DT, EQP_Retire_DT, SOLD_DT, Make, Model, Project_Nr, Object_Prefix, Owning_Dist,
                                        Total_Ctr_Reading, Total_Ctr_Reading_UOM, Short_Desc, UserId);
                            if (VehicleId == 0)
                            {
                                status = status + String.Format("Database error to process Equipment XML for Serial_Number {0} (node {1}). ", Serial_Number, ctr.ToString());
                                StatusDetails = status;
                                InvalidNodes = InvalidNodes + ctr.ToString() + ", ";
                            }
                            else
                                RecordsProcessed = RecordsProcessed + 1;
                        }
                    }
                    else
                    {
                        InvalidNodes = InvalidNodes + ctr.ToString() + ", ";
                    }
                }
                catch (Exception ex)
                {
                    LogException("<< Integrations - ProcessEquipment: Serial_Number={0}, EXC={1})", Serial_Number, ex.Message);
                    status = status + String.Format("Error to process Equipment XML Serial_Number={0} (node {1}). ", Serial_Number, ctr.ToString());
                    StatusDetails = status + ex.Message + ". ";
                }
            }

            return status;
        }

        private string ProcessEmployees(XmlNode MainNode, int UserId)
        {
            string status = String.Empty;
            string Employee_Nr = String.Empty;
            string First_Nm = String.Empty;
            string Last_Nm = String.Empty;
            string Email_Address = String.Empty;
            string Create_Date = String.Empty;
            string Update_Date = String.Empty;
            string Employee_Type = String.Empty;
            string Termination_DT = String.Empty;
            string Position_DS = String.Empty;
            string Action_Flag = String.Empty;
            int DriverId = 0;
            int ctr = 0;
            bool bError = false;

            XmlNodeList Employee = MainNode.SelectNodes("Employee");

            foreach (XmlNode item in Employee)
            {
                try
                {
                    bError = false;
                    ctr = ctr + 1;

                    if (item["Employee_Nr"] != null)
                        if (!String.IsNullOrEmpty(item["Employee_Nr"].InnerText.Trim()))
                            Employee_Nr = item["Employee_Nr"].InnerText;
                        else
                        {
                            status = status + String.Format("Employee_Nr must be provided (node {0}). ", ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }

                    else
                    {
                        status = status + String.Format("Employee_Nr must be provided (node {0}). ", ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["First_Nm"] != null)
                        if (!String.IsNullOrEmpty(item["First_Nm"].InnerText.Trim()))
                            First_Nm = item["First_Nm"].InnerText;
                        else
                        {
                            status = status + String.Format("First_Nm must be provided for Employee_Nr {0} (node {1}). ", Employee_Nr, ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    else
                    {
                        status = status + String.Format("First_Nm must be provided for Employee_Nr {0} (node {1}). ", Employee_Nr, ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Last_Nm"] != null)
                        if (!String.IsNullOrEmpty(item["Last_Nm"].InnerText.Trim()))
                            Last_Nm = item["Last_Nm"].InnerText;
                        else
                        {
                            status = status + String.Format("Last_Nm must be provided for Employee_Nr {0} (node {1}). ", Employee_Nr, ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    else
                    {
                        status = status + String.Format("Last_Nm must be provided for Employee_Nr {0} (node {1}). ", Employee_Nr, ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Email_Address"] != null)
                        Email_Address = item["Email_Address"].InnerText;
                    else
                        Email_Address = String.Empty;

                    if (item["Create_Date"] != null)
                        Create_Date = item["Create_Date"].InnerText;
                    else
                        Create_Date = String.Empty;

                    if (item["Update_Date"] != null)
                        Update_Date = item["Update_Date"].InnerText;
                    else
                        Update_Date = String.Empty;

                    if (item["Employee_Type"] != null)
                        Employee_Type = item["Employee_Type"].InnerText;
                    else
                        Employee_Type = String.Empty;

                    if (item["Termination_DT"] != null)
                        Termination_DT = item["Termination_DT"].InnerText;
                    else
                        Termination_DT = String.Empty;

                    if (item["Position_DS"] != null)
                        Position_DS = item["Position_DS"].InnerText;
                    else
                        Position_DS = String.Empty;

                    if (item["Action_Flag"] != null)
                        Action_Flag = item["Action_Flag"].InnerText;
                    else
                        Action_Flag = String.Empty;

                    if (bError == false)
                    {
                        using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                        {
                            DriverId = iDBIntegrations.UpdateDriverFromWebService(Action_Flag, First_Nm, Last_Nm, Email_Address, Employee_Nr, Termination_DT, Position_DS,
                                        Create_Date, Update_Date, Employee_Type, UserId);
                            if (DriverId == 0)
                            {
                                status = status + String.Format("Database error to process Employee XML for Employee_Nr {0} (node {1}). ", Employee_Nr, ctr.ToString());
                                StatusDetails = status;
                                InvalidNodes = InvalidNodes + ctr.ToString() + ", ";
                            }
                            else
                                RecordsProcessed = RecordsProcessed + 1;
                        }
                    }
                    else
                    {
                        InvalidNodes = InvalidNodes + ctr.ToString() + ", ";
                    }
                }
                catch (Exception ex)
                {
                    LogException("<< Integrations - ProcessEmployees: Employee_Nr={0}, EXC={1})", Employee_Nr, ex.Message);
                    status = status + String.Format("Error to process Employee XML Employee_Nr={0} (node {1}). ", Employee_Nr, ctr.ToString());
                    StatusDetails = status + ex.Message + ". ";
                }
            }

            return status;
        }

        private string ProcessHierarchy(XmlNode MainNode, int UserId)
        {
            string status = String.Empty;
            string Hierarchy_Nr = String.Empty;
            string Record_Source_ID = String.Empty;
            string Hierarchy_Name = String.Empty;
            int Hierarchy_Level = 0;
            string Parent_Node = String.Empty;
            string Create_Date = String.Empty;
            string Update_Date = String.Empty;
            //string Project_Geography = String.Empty;
            double Project_Latitude = 0.0;
            double Project_Longitude = 0.0;
            string Phone_Nr = String.Empty;
            string Project_Close_DT = String.Empty;
            string Project_Equipment_Superintendent = String.Empty;
            string Project_Equipment_Superintendent_ID = String.Empty;
            string Action_Flag = String.Empty;
            int HierarchyID = 0;
            int ctr = 0;
            bool bError = false;

            XmlNodeList Node = MainNode.SelectNodes("Node");

            foreach (XmlNode item in Node)
            {
                try
                {
                    bError = false;
                    ctr = ctr + 1;

                    if (item["Hierarchy_Nr"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Hierarchy_Nr"].InnerText))
                            Hierarchy_Nr = item["Hierarchy_Nr"].InnerText;
                        else
                        {
                            status = status + String.Format("Hierarchy_Nr must be provided (node {0}). ", ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    }
                    else
                    {
                        status = status + String.Format("Hierarchy_Nr must be provided (node {0}). ", ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Record_Source_ID"] != null)
                    {
                        Record_Source_ID = item["Record_Source_ID"].InnerText;
                    }
                    else
                        Record_Source_ID = String.Empty;

                    if (item["Hierarchy_Name"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Hierarchy_Name"].InnerText))
                            Hierarchy_Name = item["Hierarchy_Name"].InnerText;
                        else
                        {
                            status = status + String.Format("Hierarchy_Name must be provided for Hierarchy_Nr {0} (node {1}). ", Hierarchy_Nr, ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    }
                    else
                    {
                        status = status + String.Format("Hierarchy_Name must be provided for Hierarchy_Nr {0} (node {1}). ", Hierarchy_Nr, ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Hierarchy_Level"] != null)
                    {
                        if (!String.IsNullOrEmpty(item["Hierarchy_Level"].InnerText.Trim()))
                            Hierarchy_Level = Convert.ToInt32(item["Hierarchy_Level"].InnerText.Trim());
                        else
                        {
                            status = status + String.Format("Hierarchy_Level must be provided for Hierarchy_Nr {0} (node {1}). ", Hierarchy_Nr, ctr.ToString());
                            StatusDetails = status;
                            bError = true;
                        }
                    }
                    else
                    {
                        status = status + String.Format("Hierarchy_Level must be provided for Hierarchy_Nr {0} (node {1}). ", Hierarchy_Nr, ctr.ToString());
                        StatusDetails = status;
                        bError = true;
                    }

                    if (item["Parent_Node"] != null)
                        Parent_Node = item["Parent_Node"].InnerText;
                    else
                        Parent_Node = String.Empty;

                    if (item["Create_Date"] != null)
                        Create_Date = item["Create_Date"].InnerText;
                    else
                        Create_Date = String.Empty;

                    if (item["Update_Date"] != null)
                        Update_Date = item["Update_Date"].InnerText;
                    else
                        Update_Date = String.Empty;

                    if (item["Latitude"] != null)
                        Project_Latitude = Convert.ToDouble(item["Latitude"].InnerText);
                    else
                        Project_Latitude = 0.0;

                    if (item["Longitude"] != null)
                        Project_Longitude = Convert.ToDouble(item["Longitude"].InnerText);
                    else
                        Project_Longitude = 0.0;

                    //if (item["Project_Geography"] != null)
                    //{
                    //    Project_Geography = item["Project_Geography"].InnerText;
                    //    if (!String.IsNullOrEmpty(Project_Geography))
                    //    {
                    //        Project_Geography = Project_Geography.Replace("POINT(", "").Replace(" , ", ",").Replace(")", "");
                    //        string [] LongLat = Project_Geography.Split(',');
                    //        if (LongLat.Length == 2)
                    //        {
                    //            Project_Longitude = Convert.ToDouble(LongLat[0]);
                    //            Project_Latitude = Convert.ToDouble(LongLat[1]);
                    //        }
                    //    }
                    //}
                    //else
                    //    Project_Geography = String.Empty;

                    if (item["Phone_Nr"] != null)
                        Phone_Nr = item["Phone_Nr"].InnerText;
                    else
                        Phone_Nr = String.Empty;

                    if (item["Project_Close_DT"] != null)
                        Project_Close_DT = item["Project_Close_DT"].InnerText;
                    else
                        Project_Close_DT = String.Empty;

                    if (item["Project_Equipment_Superintendent"] != null)
                        Project_Equipment_Superintendent = item["Project_Equipment_Superintendent"].InnerText;
                    else
                        Project_Equipment_Superintendent = String.Empty;

                    if (item["Project_Equipment_Superintendent_ID"] != null)
                        Project_Equipment_Superintendent_ID = item["Project_Equipment_Superintendent_ID"].InnerText;
                    else
                        Project_Equipment_Superintendent_ID = String.Empty;

                    if (item["Action_Flag"] != null)
                        Action_Flag = item["Action_Flag"].InnerText;
                    else
                        Action_Flag = String.Empty;

                    if (bError == false)
                    {
                        using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                        {
                            HierarchyID = iDBIntegrations.UpdateHierarchyFromWebService(Action_Flag, Hierarchy_Nr, Hierarchy_Name, Hierarchy_Level, Parent_Node, Record_Source_ID, Phone_Nr,
                                            Project_Equipment_Superintendent, Project_Equipment_Superintendent_ID, Create_Date, Update_Date, Project_Longitude, Project_Latitude, UserId);
                            if (HierarchyID == 0)
                            {
                                status = status + String.Format("Database error to process Hierarchy XML for Hierarchy_Nr {0} (node {1}). ", Hierarchy_Nr, ctr.ToString());
                                StatusDetails = status;
                                InvalidNodes = InvalidNodes + ctr.ToString() + ", ";
                            }
                            else
                                RecordsProcessed = RecordsProcessed + 1;
                        }
                    }
                    else
                    {
                        InvalidNodes = InvalidNodes + ctr.ToString() + ", ";
                    }
                }
                catch (Exception ex)
                {
                    LogException("<< Integrations - ProcessHierarchy: Hierarchy_Nr={0}, EXC={1})", Hierarchy_Nr, ex.Message);
                    status = status + String.Format("Error to process Hierarchy XML Hierarchy_Nr={0} (node {1}). ", Hierarchy_Nr, ctr.ToString());
                    StatusDetails = status + ex.Message + ". ";
                }
            }

            return status;
        }

        //[WebMethod(Description = "Returnes XML of Hierarchy/Equipment/Employee.")]
        //public string Get(string token, string inputXML/*, ref string outputXML*/)
        //{
        //    int retVal = 1;
        //    string outputXML;
        //    outputXML = "Failed.";

        //    //Log("<< InputXMLPayload: {0}", inputXML);
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        //int userId = SecurityKeyBypassManager.GetUserIdFromPasswordStr(token);
        //        //WriteEquipment(userId, inputXML);
        //        //status = "<![CDATA[" + inputXML + "]]>";

        //        try
        //        {
        //            //DataSet iDataSet = new DataSet();
        //            //System.IO.StringReader strXML = new System.IO.StringReader(inputXML);
        //            //iDataSet.ReadXml(strXML);
        //            //status = "Total Record(s): " + iDataSet.Tables[0].Rows.Count;

        //            HttpContext.Current.Response.Clear();
        //            HttpContext.Current.Response.ContentType = "text/xml";
        //            HttpContext.Current.Response.AddHeader("Content-Length", inputXML.Length.ToString());
        //            HttpContext.Current.Response.Write(inputXML);
        //            HttpContext.Current.Response.Flush();
        //            HttpContext.Current.Response.Close();
        //        }
        //        catch { outputXML = "Failed"; }

        //        retVal = 0;
        //    }
        //    return outputXML;
        //}

        [WebMethod(Description = "Returns requested image.")]
        public byte[] GetImageFile(string token, string fileName)
        {
            string status = String.Empty;
            int userId = 0;
            byte[] ImageFile = new byte[] { 0 };

            token = "d@d192!94275b52$a489f272674308&85bb2";

            if (String.IsNullOrEmpty(token.Trim()))
            {
                status = "Invalid user";
                return ImageFile;
            }

            try
            {
                using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                {
                    userId = iDBIntegrations.GetUserByHashPassword(token);
                }

                if (userId == 0)
                {
                    status = "Invalid user";
                    return ImageFile;
                }

                if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/image.jpg") + fileName))
                {
                    ImageFile = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/image.jpg") + fileName);
                }
            }
            catch (Exception ex)
            {
                LogException("<< Integrations - GetImageFile: uId={0}, EXC={1})", userId, ex.Message);

                status = "Error to get image file. " + ex.Message;
            }
            finally
            {
                if (String.IsNullOrEmpty(status))
                {
                    status = "Success";
                }
            }

            return ImageFile;
        }

        [WebMethod(Description = "Downloads requested image.")]
        public byte[] DownloadImageFile(string token, string fileName)
        {
            string status = String.Empty;
            int userId = 0;
            byte[] ImageFile = new byte[] { 0 };

            token = "d@d192!94275b52$a489f272674308&85bb2";

            if (String.IsNullOrEmpty(token.Trim()))
            {
                status = "Invalid user";
                return ImageFile;
            }

            try
            {
                using (DAS.Logic.Integrations iDBIntegrations = new DAS.Logic.Integrations(DBConnectionString))
                {
                    userId = iDBIntegrations.GetUserByHashPassword(token);
                }

                if (userId == 0)
                {
                    status = "Invalid user";
                    return ImageFile;
                }

                //if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/image.jpg") + fileName))
                //{
                //    ImageFile = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/image.jpg") + fileName);
                //}

                using (FileStream fs = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/image.jpg"), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    ImageFile = new byte[fs.Length];
                    fs.Read(ImageFile, 0, (int)fs.Length);
                }
            }
            catch (Exception ex)
            {
                LogException("<< Integrations - DownloadImageFile: uId={0}, EXC={1})", userId, ex.Message);

                status = "Error to get image file. " + ex.Message;
            }
            finally
            {
                if (String.IsNullOrEmpty(status))
                {
                    status = "Success";
                }
            }

            return ImageFile;
        }

        //[WebMethod]
        //public void SubmitImageMethod(byte[] fileData)
        //{
        //    if (fileData != null)
        //    {
        //        MemoryStream stream = new MemoryStream(fileData);
        //        Bitmap image = new Bitmap(stream);

        //    }
        //}

    }
}