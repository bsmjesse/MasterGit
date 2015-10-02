using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

public partial class frmLoginFaileds : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack) 
            dgData_Fill();
    }
    protected void cmdView_Click(object sender, EventArgs e)
    {
        dgData_Fill();
    }

    private void dgData_Fill()
    {
        this.lblLogins.Visible = true ;
        SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
        string xml = "";
        //sec.GetFailedLogins(ref xml);
        //if (xml != "")
        //{
        //    StringReader strrXML = new StringReader(xml);
        //    DataSet ds = new DataSet();
        //    ds.ReadXml(strrXML);
        //    this.dgData.DataSource = ds;
        //    this.dgData.DataBind();
        //    this.lblLogins.Visible = false;
        //}
        //else
        //{
        //    this.dgData.DataSource = null;
        //    this.dgData.DataBind();
        //}

    }
    protected void dgData_SelectedIndexChanged(object sender, EventArgs e)
    {
        string Ip=dgData.SelectedItem.Cells[1].Text;
        SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
        //sec.ClearFailedLoginsByIP(Ip);
        dgData_Fill();
    }
}
