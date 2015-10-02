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
using VLF.DAS.DB;
using VLF.PATCH.Logic;
using System.Data.OleDb;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;

namespace SentinelFM
{
    public partial class Configuration_frmOrganizationHierarchyImport : SentinelFMBasePage
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
            
            if (!Page.IsPostBack)
            {
                
            }
        }

        protected void cmdSaveUploadOrganizationHierarchy_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            string fileName = "";

            try
            {

                if (this.fileOrganizationHierarchy.FileName.Trim() == String.Empty)
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
                    fileName = Server.MapPath("~/App_Data/") + this.fileOrganizationHierarchy.FileName;
                    this.fileOrganizationHierarchy.PostedFile.SaveAs(fileName);
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

                //string connString = String.Format(
                //   "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";",
                //   fileName);

                //DataSet dsOHData = new DataSet("OrganizationHierarchy");
                //DataTable dtOHData = dsOHData.Tables.Add("vlfoh");

                ////dtDrivers.Columns.Add("OrganizationId", typeof(int), sn.User.OrganizationId.ToString());

                //// get data into dataset
                //using (OleDbConnection conn = new OleDbConnection(connString))
                //{
                //    OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Pyramid$]", conn);
                //    da.Fill(dtOHData);
                //}

                DataTable dtOHData = ReadDataFromExcelUsingNPOI(fileName);

                /*xmlData = dsOH.GetXml();

                DataSet dsData = new DataSet();
                dsData.ReadXml(new StringReader(xmlData));*/

                DataSet dsOrganizationHierarchy = new DataSet("OrganizationHierarchy");
                dsOrganizationHierarchy.ReadXmlSchema(Server.MapPath("~/Datasets/OrganizationHierarchy.xsd"));

                foreach (DataRow row in dtOHData.Rows)
                {
                    string nodecode = string.Empty;
                    string nodename = string.Empty;

                    //if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999988 || sn.User.OrganizationId == 1000076 || sn.User.OrganizationId == 999722)
                    if (sn.User.OrganizationId != 951 )
                    {
                        nodecode = row["Organisation unit id"].ToString().Trim();
                        nodename = row["Organisation unit name"].ToString().Trim();
                    }
                    else
                    {
                        nodecode = row["COST_CENTER"].ToString().Trim();
                        nodename = row["COST_CENTER_NAME"].ToString().Trim();
                    }

                    if (nodename.Length >= 50)
                        nodename = nodename.Substring(0, 49);

                    if (nodecode != string.Empty && nodename != string.Empty)
                    {

                        DataRow newOHRow = dsOrganizationHierarchy.Tables[0].NewRow();

                        newOHRow["OrganizationId"] = sn.User.OrganizationId.ToString();
                        newOHRow["NodeCode"] = nodecode;
                        newOHRow["Nodename"] = nodename;
                        //newOHRow["IsParent"] = row["IS_PARENT"].ToString().Trim() == "Y" ? 1 : 0;
                        newOHRow["IsParent"] = 0;

                        string parentNodecode = string.Empty;
                        //if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999988 || sn.User.OrganizationId == 1000076 || sn.User.OrganizationId == 999722)
                        if (sn.User.OrganizationId != 951)
                        {
                            parentNodecode = row["Parent unit id"].ToString().Trim();                            
                        }
                        else
                        {
                            string path = row["PATH"].ToString().Trim();
                            string[] p = path.Split('/');
                            if (p.Length > 1)
                                parentNodecode = p[p.Length - 2] == "" ? row["COST_CENTER"].ToString().Trim() : p[p.Length - 2];
                            else
                                parentNodecode = row["COST_CENTER"].ToString().Trim();
                        }

                        newOHRow["ParentNodeCode"] = parentNodecode;
                        
                        newOHRow["Description"] = "";

                        dsOrganizationHierarchy.Tables[0].Rows.Add(newOHRow);
                    }
                }

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                poh.BatchAddOrganizationHierarchy(sn.User.OrganizationId, dsOrganizationHierarchy.Tables[0]);


                lblMessage.ForeColor = Color.Green;
                //lblMessage.Text = "This feature is disabled for now.";
                lblMessage.Text = strMessage4;


            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = Ex.Message + " :: " + Ex.StackTrace;
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = Ex.Message + " :: " + Ex.StackTrace;
                //RedirectToLogin();
            }



            //lblMessage.Text = "OrganizationHierarchy clicked";

        }

        public DataTable ReadDataFromExcelUsingNPOI(string filePath)
        {
            HSSFWorkbook hssfworkbook;

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            //HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheet("Pyramid");
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