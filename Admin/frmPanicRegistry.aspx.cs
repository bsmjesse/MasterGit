using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient ;

using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SentinelFM;
using System.IO;


namespace SentinelFM.Admin
{
    public partial class frmPanicRegistry : System.Web.UI.Page
    {
       
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
           

                sn = (SentinelFMSession)Session["SentinelFMSession"];
                if (!Page.IsPostBack)
                {
                    cboOrganization_Fill();
                }
        }

        private void cboOrganization_Fill()
        {

            StringReader strrXML = null;
            DataSet ds = new DataSet();
            objUtil = new clsUtility(sn);
            string xml = "";

            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            this.cboOrganization.DataSource = ds;
            this.cboOrganization.DataBind();



        }

        protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string sql = "";
            if (Convert.ToInt32(cboOrganization.SelectedItem.Value) != 0)
            {
                sql = "SELECT dbo.vlfBox.BoxId, dbo.vlfVehicleInfo.Description, dbo.PANICREGISTRY.DeviceId, dbo.PANICREGISTRY.SensorId," +
                      " dbo.PANICREGISTRY.RegistryDate, dbo.PANICREGISTRY.AccountId, dbo.vlfBoxCommInfo.CommAddressTypeId, dbo.vlfBoxCommInfo.CommAddressValue " +
                      " FROM         dbo.vlfBox INNER JOIN " +
                      " dbo.vlfBoxCommInfo ON dbo.vlfBox.BoxId = dbo.vlfBoxCommInfo.BoxId INNER JOIN " +
                      " dbo.PANICREGISTRY ON dbo.vlfBox.BoxId = dbo.PANICREGISTRY.BoxId INNER JOIN " +
                      " dbo.vlfVehicleAssignment ON dbo.vlfBox.BoxId = dbo.vlfVehicleAssignment.BoxId INNER JOIN " +
                      " dbo.vlfVehicleInfo ON dbo.vlfVehicleAssignment.VehicleId = dbo.vlfVehicleInfo.VehicleId " +
                       " WHERE     (dbo.vlfBoxCommInfo.CommAddressTypeId = 12) " + " and  dbo.vlfVehicleInfo.OrganizationId=" + cboOrganization.SelectedItem.Value +
                       " ORDER BY dbo.vlfBox.BoxId";

                DataSet ds = SQLExecuteDataset(sql);
                dgCommDiag.DataSource = ds;
                dgCommDiag.DataBind(); 
            }

            
        }


     

           /// <summary>
        /// Sends the CommandText to the Connection and builds a DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet SQLExecuteDataset(string sql)
        {

            SqlConnection sqlconnection=new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(sql, sqlconnection);


            command.CommandText = sql;
            
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dataSet);
            sqlconnection.Close(); 
            return dataSet;
        }

    }
   }
