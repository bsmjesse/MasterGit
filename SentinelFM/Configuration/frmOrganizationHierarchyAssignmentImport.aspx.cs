using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VLF.DAS.DB;
using VLF.PATCH.Logic;
using System.Data.OleDb;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;

namespace SentinelFM
{

    public partial class Configuration_frmOrganizationHierarchyAssignmentImport : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        string strMessage1;
        string strMessage2;
        string strMessage3;
        string strMessage4; 

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Server.ScriptTimeout = 90000;

            strMessage1 = GetLocalResourceObject("strMessage1").ToString();
            strMessage2 = GetLocalResourceObject("strMessage2").ToString();
            strMessage3 = GetLocalResourceObject("strMessage3").ToString();
            strMessage4 = GetLocalResourceObject("strMessage4").ToString();

            bool DriverAssignment = false;
            if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 951)
                DriverAssignment = true;

            if (DriverAssignment)
            {
                trDriverAssignment.Visible = true;
            }

            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
        }
        
        protected void cmdSaveUploadOrganizationHierarchyAssignment_Click(object sender, EventArgs e)
        {
            if (this.fileOrganizationHierarchyAssignment.FileName.Trim() == String.Empty)
            {
                lblMessage.ForeColor = Color.Red;
                //lblMessage.Text = "Please select a file";
                lblMessage.Text = strMessage1;
                return;
            }

            string s = string.Empty;

            try
            {
                if (sn.User.OrganizationId == 999988)   //TDSB
                {
                    /*
                     * Assignment by Vehicle
                     * File format: Excel
                     * 
                     */
                    string fileName = "";
                    if (this.fileOrganizationHierarchyAssignment.FileName.Trim() == String.Empty)
                    {
                        lblMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "Please select a file";
                        lblMessage.Text = strMessage1;
                        return;
                    }

                    if (sn.User.OrganizationId <= 0)
                    {
                        lblMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "You don't have access or your session is time out.";
                        lblMessage.Text = strMessage2;
                        return;
                    }

                    try
                    {
                        fileName = Server.MapPath("~/App_Data/") + this.fileOrganizationHierarchyAssignment.FileName;
                        this.fileOrganizationHierarchyAssignment.PostedFile.SaveAs(fileName);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " :: " + Ex.StackTrace));
                        //base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resErrorUploadingFile").ToString(), Color.Red);
                        lblMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "Failed to upload file.";
                        lblMessage.Text = strMessage3;
                    }

                    /*
                    string connString = String.Format(
                       "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";",
                       fileName);

                    DataSet dsOHData = new DataSet("OrganizationHierarchyAssignment");
                    DataTable dtOHData = dsOHData.Tables.Add("vlfoha");

                    // get data into dataset
                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Report1$]", conn);
                        da.Fill(dtOHData);
                    }
                     */

                    DataTable dtOHData = ReadDataFromExcelUsingNPOI(fileName);

                    DataSet dsOrganizationHierarchy = new DataSet("OrganizationHierarchyAssignment");
                    dsOrganizationHierarchy.ReadXmlSchema(Server.MapPath("~/Datasets/OrganizationHierarchyAssignment.xsd"));

                    bool DriverAssignment = false;
                    //if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 951)
                    //    DriverAssignment = true;

                    dsOrganizationHierarchy.Tables[0].Columns.Add("Equipment", typeof(String));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("TeamLeaderName", typeof(String));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("ListName", typeof(String));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("LicensePlate", typeof(String));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("Weight", typeof(Int32));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("ConstructYear", typeof(Int16));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("FuelType", typeof(String));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("Manufacturer", typeof(String));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("ModelNumber", typeof(String));
                    dsOrganizationHierarchy.Tables[0].Columns.Add("VehicleType", typeof(String));

                    Int32 vehicleweight = 0;
                    Int16 constructYear = 1900;

                    foreach (DataRow row in dtOHData.Rows)
                    {
                        if (row["Org Unit"].ToString().Trim() != string.Empty && row["Fleet Object No"].ToString().Trim() != string.Empty)
                        {
                            DataRow newOHRow = dsOrganizationHierarchy.Tables[0].NewRow();

                            Int16.TryParse(row["ConstructYear"].ToString().Trim(), out constructYear);
                            Int32.TryParse(row["Gross Weight"].ToString().Trim(), out vehicleweight);

                            newOHRow["OrganizationId"] = sn.User.OrganizationId.ToString();
                            newOHRow["NodeCode"] = row["Org Unit"].ToString().Trim();
                            newOHRow["ObjectTableName"] = "vlfVehicleInfo";
                            newOHRow["ObjectID"] = row["Fleet Object No"].ToString().Trim();
                            newOHRow["Equipment"] = row["Equipment"].ToString().Trim();
                            newOHRow["TeamLeaderName"] = row["Team Leader Name"].ToString().Trim();
                            newOHRow["ListName"] = row["List name"].ToString().Trim();
                            newOHRow["LicensePlate"] = row["License plate"].ToString().Trim();
                            newOHRow["Weight"] = vehicleweight;
                            newOHRow["ConstructYear"] = constructYear;
                            newOHRow["FuelType"] = row["Primary fuel"].ToString().Trim();
                            newOHRow["Manufacturer"] = row["Manufacturer"].ToString().Trim();
                            newOHRow["ModelNumber"] = row["Model number"].ToString().Trim();
                            newOHRow["VehicleType"] = row["Vehicle Type"].ToString().Trim();
                            

                            dsOrganizationHierarchy.Tables[0].Rows.Add(newOHRow);
                        }

                        //dsDrivers.Tables[0].ImportRow(row);
                    }

                    DateTime effectiveFrom;

                    if (this.txtStartAssgnDate.Text.Trim() == "")
                        effectiveFrom = DateTime.UtcNow;
                    else
                    {
                        effectiveFrom = Convert.ToDateTime(txtStartAssgnDate.Text + " " +
                            (this.txtStartAssgnTime.Text.Trim() == string.Empty ? string.Empty : (this.txtStartAssgnTime.Text + " " + ddlStartAM.SelectedItem.Text))).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
                    }

                    VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                    poh.SaveOrganizationHierarchyAssignmentByVehicleDescription(sn.User.OrganizationId, dsOrganizationHierarchy.Tables[0], "vlfVehicleInfo", effectiveFrom, DriverAssignment, true, sn.UserID);


                    lblMessage.ForeColor = Color.Green;
                    //lblMessage.Text = "Sucessfully imported the data.";
                    lblMessage.Text = strMessage4;
                }
                else if (sn.User.OrganizationId == 951 || true)
                {
                    /*
                     * Assignment by Vehicle
                     * File format: Excel
                     * 
                     */
                    string fileName = "";
                    if (this.fileOrganizationHierarchyAssignment.FileName.Trim() == String.Empty)
                    {
                        lblMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "Please select a file";
                        lblMessage.Text = strMessage1;
                        return;
                    }

                    if (sn.User.OrganizationId <= 0)
                    {
                        lblMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "You don't have access or your session is time out.";
                        lblMessage.Text = strMessage2;
                        return;
                    }

                    try
                    {
                        fileName = Server.MapPath("~/App_Data/") + this.fileOrganizationHierarchyAssignment.FileName;
                        this.fileOrganizationHierarchyAssignment.PostedFile.SaveAs(fileName);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " :: " + Ex.StackTrace));
                        //base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resErrorUploadingFile").ToString(), Color.Red);
                        lblMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "Failed to upload file.";
                        lblMessage.Text = strMessage3;
                    }

                    /*
                    string connString = String.Format(
                       "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";",
                       fileName);

                    DataSet dsOHData = new DataSet("OrganizationHierarchyAssignment");
                    DataTable dtOHData = dsOHData.Tables.Add("vlfoha");

                    // get data into dataset
                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Report1$]", conn);
                        da.Fill(dtOHData);
                    }
                     */

                    DataTable dtOHData = ReadDataFromExcelUsingNPOI(fileName);

                    DataSet dsOrganizationHierarchy = new DataSet("OrganizationHierarchyAssignment");
                    dsOrganizationHierarchy.ReadXmlSchema(Server.MapPath("~/Datasets/OrganizationHierarchyAssignment.xsd"));

                    dsOrganizationHierarchy.Tables[0].Columns.Add("Weight", typeof(Int32));

                    bool DriverAssignment = false;
                    //if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 951)
                    //    DriverAssignment = true;

                    string GVWR = string.Empty;
                    string[] _gvwr;
                    int weight = -1;

                    dsOrganizationHierarchy.Tables[0].Columns.Add("TeamLeaderName", typeof(String));

                    foreach (DataRow row in dtOHData.Rows)
                    {
                        if (row["Cost Center"].ToString().Trim() != string.Empty && row["Vehicle"].ToString().Trim() != string.Empty)
                        {
                            weight = -1;
                            DataRow newOHRow = dsOrganizationHierarchy.Tables[0].NewRow();

                            newOHRow["OrganizationId"] = sn.User.OrganizationId.ToString();
                            newOHRow["NodeCode"] = row["Cost Center"].ToString().Trim();
                            newOHRow["ObjectTableName"] = "vlfVehicleInfo";
                            newOHRow["ObjectID"] = row["Vehicle"].ToString().Trim();

                            if (dtOHData.Columns.Contains("Cost Center Manager") && row["Cost Center Manager"] != null)
                                newOHRow["TeamLeaderName"] = row["Cost Center Manager"].ToString().Trim();


                            if (DriverAssignment)
                            {
                                newOHRow["DriverFirstName"] = row["Driver First Name"].ToString().Trim();
                                newOHRow["DriverLastName"] = row["Driver Last Name"].ToString().Trim();
                            }

                            if (sn.User.OrganizationId == 951)  // UP will update vehicle's weight information.
                            {
                                GVWR = row["GVWR"].ToString().Trim();
                                if (clsUtility.IsNumeric(GVWR))
                                {
                                    int.TryParse(GVWR, out weight);
                                }
                                else
                                {
                                    _gvwr = GVWR.Split(':');
                                    if (_gvwr.Length > 1)
                                    {
                                        string[] w = _gvwr[1].Split(new string[] { " lb" }, StringSplitOptions.None);
                                        if (w.Length > 1)
                                        {
                                            string[] _a = w[0].Split('-');
                                            string _fin = string.Empty;
                                            if (_a.Length > 1)
                                                _fin = _a[1];
                                            else
                                                _fin = _a[0];

                                            _fin = _fin.Trim().Replace(",", "");
                                            int.TryParse(_fin, out weight);
                                        }
                                    }
                                }
                            }

                            newOHRow["Weight"] = weight;

                            dsOrganizationHierarchy.Tables[0].Rows.Add(newOHRow);
                        }

                        //dsDrivers.Tables[0].ImportRow(row);
                    }

                    DateTime effectiveFrom;

                    if (this.txtStartAssgnDate.Text.Trim() == "")
                        effectiveFrom = DateTime.UtcNow;
                    else
                    {
                        effectiveFrom = Convert.ToDateTime(txtStartAssgnDate.Text + " " +
                            (this.txtStartAssgnTime.Text.Trim() == string.Empty ? string.Empty : (this.txtStartAssgnTime.Text + " " + ddlStartAM.SelectedItem.Text))).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
                    }

                    VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                    poh.SaveOrganizationHierarchyAssignmentByVehicleDescription(sn.User.OrganizationId, dsOrganizationHierarchy.Tables[0], "vlfVehicleInfo", effectiveFrom, DriverAssignment);


                    lblMessage.ForeColor = Color.Green;
                    //lblMessage.Text = "Sucessfully imported the data.";
                    lblMessage.Text = strMessage4;
                }
                else
                {
                    /*
                     * Assignment by driver
                     * File Format: Text
                     * 
                     */
                    StreamReader reader = new StreamReader(this.fileOrganizationHierarchyAssignment.FileContent);
                    DataTable dtOHAssignment = new DataTable("ohassignment");
                    dtOHAssignment.Columns.Add("FirstName", typeof(string));
                    dtOHAssignment.Columns.Add("LastName", typeof(string));
                    dtOHAssignment.Columns.Add("NodeCode", typeof(string));
                    do
                    {
                        string textLine = reader.ReadLine();
                        if (textLine.Length != 483)
                        {
                            lblMessage.ForeColor = Color.Red;
                            lblMessage.Text = "File format is wrong.";
                            reader.Close();
                            return;

                        }
                        string firstname = textLine.Substring(7, 30).Trim();
                        string lastname = textLine.Substring(38, 30).Trim();
                        string nodecode = textLine.Substring(314, 5).Trim();

                        // if(nodecode != "")
                        //     s = s + firstname + ", " + lastname + ": " + nodecode + "<br />";
                        DataRow newOHRow = dtOHAssignment.NewRow();

                        newOHRow["FirstName"] = firstname.Trim();
                        newOHRow["LastName"] = lastname.Trim();
                        newOHRow["NodeCode"] = nodecode.Trim();

                        dtOHAssignment.Rows.Add(newOHRow);

                    }
                    while (reader.Peek() != -1);
                    reader.Close();

                    DateTime effectiveFrom;

                    if (this.txtStartAssgnDate.Text.Trim() == "")
                        effectiveFrom = DateTime.UtcNow;
                    else
                    {
                        effectiveFrom = Convert.ToDateTime(txtStartAssgnDate.Text + " " +
                            (this.txtStartAssgnTime.Text.Trim() == string.Empty ? string.Empty : (this.txtStartAssgnTime.Text + " " + ddlStartAM.SelectedItem.Text))).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
                    }

                    VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                    string m = poh.BatchOrganizationHierarchyAssignment(sn.User.OrganizationId, dtOHAssignment, "vlfDriver", effectiveFrom);
                    if (m != string.Empty)
                    {
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Text = m;
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " :: " + Ex.StackTrace));
                lblMessage.ForeColor = Color.Red;
                //lblMessage.Text = "Failed to import the data.";
                lblMessage.Text = strMessage3 + ": " + Ex.Message + " :: " + Ex.StackTrace;
                return;
            }
            //base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resErrorUploadingFile").ToString(), Color.Red);
            lblMessage.ForeColor = Color.Green;
            //lblMessage.Text = "Sucessfully imported the data.<br />";
            lblMessage.Text = strMessage4;
        }

        /// <summary>
        /// Driver Assignment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdSaveUploadOrganizationHierarchyDriverAssignment_Click(object sender, EventArgs e)
        {
            if (this.driverFileOrganizationHierarchyAssignment.FileName.Trim() == String.Empty)
            {
                lblDriverMessage.ForeColor = Color.Red;
                //lblMessage.Text = "Please select a file";
                lblDriverMessage.Text = strMessage1;
                return;
            }

            string s = string.Empty;

            try
            {
                
                    string fileName = "";                    

                    if (sn.User.OrganizationId <= 0)
                    {
                        lblDriverMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "You don't have access or your session is time out.";
                        lblDriverMessage.Text = strMessage2;
                        return;
                    }

                    try
                    {
                        fileName = Server.MapPath("~/App_Data/") + this.driverFileOrganizationHierarchyAssignment.FileName;
                        this.driverFileOrganizationHierarchyAssignment.PostedFile.SaveAs(fileName);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " :: " + Ex.StackTrace));
                        //base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resErrorUploadingFile").ToString(), Color.Red);
                        lblDriverMessage.ForeColor = Color.Red;
                        //lblMessage.Text = "Failed to upload file.";
                        lblDriverMessage.Text = strMessage3 + ": " + Ex.Message + " :: " + Ex.StackTrace;
                        return;
                    }

                /*

                    string connString = String.Format(
                       "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";",
                       fileName);

                    DataSet dsOHData = new DataSet("OrganizationHierarchyDriverAssignment");
                    DataTable dtOHData = dsOHData.Tables.Add("vlfoha");

                    // get data into dataset
                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        try
                        {
                            OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Report1$]", conn);
                            da.Fill(dtOHData);
                        }
                        catch
                        {
                            OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [UP Inventory$]", conn);
                            da.Fill(dtOHData);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                 * 
                 */
                    DataTable dtOHData = ReadDataFromExcelUsingNPOI(fileName);

                    //UPFuelTransaction20150109.xls

                    string pattern = @"^[a-zA-Z]+(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})*.*";
                    
                    Match match = Regex.Match(this.driverFileOrganizationHierarchyAssignment.FileName, pattern);

                    string transactionDateFromFileName = "";
                    if (match.Groups["Year"].Value != "" && match.Groups["Month"].Value != "" && match.Groups["Day"].Value != "")
                    {
                        transactionDateFromFileName = match.Groups["Year"].Value + "-" + match.Groups["Month"].Value + "-" + match.Groups["Day"].Value;
                    }
                    
                    DataSet dsOrganizationHierarchyDriverAssignment = new DataSet("OrganizationHierarchyDriverAssignment");
                    dsOrganizationHierarchyDriverAssignment.ReadXmlSchema(Server.MapPath("~/Datasets/OrganizationHierarchyDriverAssignment.xsd"));
                
                    bool sorttable = false;

                    foreach (DataRow row in dtOHData.Rows)
                    {
                        if (row["Vehicle Number"].ToString().Trim() != string.Empty && (row["First Name"].ToString().Trim() != string.Empty || row["Last Name"].ToString().Trim() != string.Empty))
                        {
                            DataRow newOHRow = dsOrganizationHierarchyDriverAssignment.Tables[0].NewRow();

                            newOHRow["OrganizationId"] = sn.User.OrganizationId.ToString();
                            newOHRow["VehicleNumber"] = row["Vehicle Number"].ToString().Trim();
                            newOHRow["VIN"] = row["VIN"].ToString().Trim();
                            newOHRow["FirstName"] = row["First Name"].ToString().Trim();
                            newOHRow["LastName"] = row["Last Name"].ToString().Trim();
                            //newOHRow["Odometer"] = row["Odometer"].ToString().Trim(); // we don't use Odometer currently
                            if (row.Table.Columns.Contains("Transaction Date"))
                            {
                                newOHRow["TransactionDate"] = row["Transaction Date"].ToString().Trim();
                                sorttable = true;
                            }
                            else if (transactionDateFromFileName != "")
                            {
                                newOHRow["TransactionDate"] = transactionDateFromFileName;
                            }
                            else
                                newOHRow["TransactionDate"] = DateTime.Now.ToUniversalTime().ToString();

                            dsOrganizationHierarchyDriverAssignment.Tables[0].Rows.Add(newOHRow);
                        }

                        //dsDrivers.Tables[0].ImportRow(row);
                    }

                    //dsOrganizationHierarchyDriverAssignment.Tables[0].Columns.Add("EffectiveDateTime");

                    //foreach (DataRow dr in dsOrganizationHierarchyDriverAssignment.Tables[0].Rows)
                    //{
                    //    dr["EffectiveDateTime"] = DateTime.ParseExact(dr["TransactionDate"].ToString(), "M/d/yyyy h:mm:ss tt",
                    //                   System.Globalization.CultureInfo.InvariantCulture);
                    //}

                    DataView dv = dsOrganizationHierarchyDriverAssignment.Tables[0].DefaultView;
                    dv.Sort = "TransactionDate ASC";
                    DataTable sortedDT = dv.ToTable();

                    VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                    if(sorttable)
                        poh.SaveOrganizationHierarchyDriverAssignmentByVehicleDescriptionDriverName(sn.User.OrganizationId, sortedDT);
                    else
                        poh.SaveOrganizationHierarchyDriverAssignmentByVehicleDescriptionDriverName(sn.User.OrganizationId, dsOrganizationHierarchyDriverAssignment.Tables[0]);


                    lblDriverMessage.ForeColor = Color.Green;
                    //lblMessage.Text = "Sucessfully imported the data.";
                    lblDriverMessage.Text = strMessage4;
                
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " :: " + Ex.StackTrace));
                lblDriverMessage.ForeColor = Color.Red;
                //lblMessage.Text = "Failed to import the data.";
                lblDriverMessage.Text = strMessage3 + ": " + Ex.Message + " :: " + Ex.StackTrace;
                return;
            }
            //base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resErrorUploadingFile").ToString(), Color.Red);
            lblDriverMessage.ForeColor = Color.Green;
            //lblMessage.Text = "Sucessfully imported the data.<br />";
            lblDriverMessage.Text = strMessage4;
        }

        public DataTable ReadDataFromExcelUsingNPOI(string filePath)
        {
            HSSFWorkbook hssfworkbook;

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();
            //for (int j = 0; j < 5; j++)
            //{
            //    dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
            //}

            if (rows.MoveNext())
            {
                HSSFRow row0 = (HSSFRow)rows.Current;
                for (int i = 0; i < row0.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row0.GetCell(i);

                    dt.Columns.Add(cell.ToString());
                    //dr[i] = cell.ToString();
                    
                }
            }

            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row.GetCell(i);

                    dr[i] = (cell == null) ? "" : cell.ToString();
                    
                }
                dt.Rows.Add(dr);
            }
            return dt;
        } 

}
}