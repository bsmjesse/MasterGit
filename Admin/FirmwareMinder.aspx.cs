using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM.Admin
{
public partial class FirmwareMinder : System.Web.UI.Page
{
    protected SentinelFMSession sn = null;
    string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
    static DataTable resultsTable;
    static DataTable filteredTable;
    bool Initialized;
    static int counter = 0;
    static string FirmwareVersionKey = "Firmware Version";

    protected void Page_Load(object sender, EventArgs e)
    {


			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}

        // ddlOrg.Attributes.Add("onchange", "$('#bFetch').attr('disabled', ''); return false;");
        bFetch.Attributes.Add("onclick", "showLoading();");
        if (Page.IsPostBack)
        {


        }
        else
        {
            LoadOrgs();
        }
    }




    void LoadOrgs()
    {
        try
        {
            DataTable dt = new DataTable();
            string sql = "SELECT OrganizationId, OrganizationName FROM vlfOrganization ORDER BY OrganizationName ASC";
            using (SqlConnection scon = new SqlConnection(constr))
            {
                scon.Open();
                using (SqlCommand scom = new SqlCommand(sql, scon))
                {
                    scom.CommandType = System.Data.CommandType.Text;
                    scom.Parameters.Clear();
                    scom.CommandTimeout = 300;
                    using (SqlDataAdapter sda = new SqlDataAdapter(scom))
                    {
                        sda.Fill(dt);
                    }
                }
                scon.Close();
            }
                string selected = string.Empty;
                ArrayList list = new ArrayList();
                list.Add(new IntOptions(-1, "Please select organization..."));
                foreach (DataRow row in dt.Rows)
                {
                    int id = Convert.ToInt32(row["OrganizationId"]);
                    string name = (string)row["OrganizationName"];
                    list.Add(new IntOptions(id, name));
                    if (id == sn.User.OrganizationId)
                    {
                        selected = name;
                    }
                }
                ddlOrg.DataTextField = "Display";
                ddlOrg.DataValueField = "Value";
                ddlOrg.DataSource = list;
                ddlOrg.DataBind();

                if (!string.IsNullOrEmpty(selected))
                {
                    int index = ddlOrg.FindItemIndexByText(selected);
                    ddlOrg.SelectedIndex = index;
                    bFetch.Enabled = true;
                }
        }
        catch (Exception exc)
        {
            //MessageBox.Show(this, exc.Message);
        }
        Initialized = true;
    }


    void Fetch()
    {
        try
        {
            int orgid = Convert.ToInt32(ddlOrg.SelectedItem.Value);
            resultsTable = new DataTable();
            string sql = "GetLatestFirmwareByOrganization";
            using (SqlConnection scon = new SqlConnection(constr))
            {
                scon.Open();
                using (SqlCommand scom = new SqlCommand(sql, scon))
                {
                    scom.CommandType = System.Data.CommandType.StoredProcedure;
                    scom.Parameters.Clear();
                    scom.CommandTimeout = 300;
                    scom.Parameters.AddWithValue("@OrgID", orgid);
                    using (SqlDataAdapter sda = new SqlDataAdapter(scom))
                    {
                        sda.Fill(resultsTable);
                    }
                }
                scon.Close();
            }
            Filter();
        }
        catch (Exception exc)
        {
        }
    }


    //void Filter()
    //{
    //    try
    //    {
    //        int count = 0;
    //        int filtered = 0;
    //        this.lReportingUnits.Text = (resultsTable.Rows.Count - 1).ToString();
    //        string searchType = ddlFilter.SelectedItem.Text;

    //        filteredTable = resultsTable.Clone();
    //        filteredTable.Columns.RemoveAt(2);
    //        filteredTable.Columns.RemoveAt(3);
    //        //filteredTable.Columns.RemoveAt(4);
    //        filteredTable.Columns.Add(new DataColumn("FirmwareVersion", typeof(string)));
    //        filteredTable.Columns.Add(new DataColumn("UpperBoardVersion", typeof(string)));
    //        filteredTable.Columns.Add(new DataColumn("Custom Prop", typeof(string)));
    //        filteredTable.Columns[2].ColumnName = "Description";
    //        foreach (DataRow row in resultsTable.Rows)
    //        {
    //            count = Convert.ToInt32(row.ItemArray[4]);
    //            if (Convert.ToInt32(row.ItemArray[0]) == 0)
    //            {
    //                break;
    //            }

    //            string fw = string.Empty;
    //            string up = string.Empty;
    //            DataRow newRow = filteredTable.NewRow();
    //            newRow.ItemArray = row.ItemArray;
    //            string cp = (string)row.ItemArray[3];
    //            string[] sp = cp.Split(';');
    //            string[] spx = sp[3].Split('=');
    //            int index = spx[1].IndexOf('(');
    //            if (index > -1)
    //            {
    //                fw = spx[1].Substring(0, index);
    //                int index2 = spx[1].IndexOf(')') + 1;
    //                if (index2 > -1)
    //                {
    //                    int index3 = spx[1].IndexOf('(', index2);
    //                    if (index3 > -1)
    //                    {
    //                        up = spx[1].Substring(index2, index3 - index2);
    //                    }
    //                }
    //            }
    //            newRow[3] = fw;
    //            newRow[4] = up;
    //            newRow[5] = spx[1];

    //                         int match = (this.rbAX.Checked) ?  2  : (this.rbMB.Checked) ? 0 : 1;
    //                bool add = false;
    //                switch (searchType)
    //                {
    //                    case "No Filter":
    //                        filteredTable.Rows.Add(newRow);
    //                        filtered++;
    //                        break;
    //                    case "Include Exact Matches":
    //                        //<asp:ListItem Text="Include Exact Matches" />
    //                        switch (match)
    //                        {
    //                            case 0:
    //                                add = fw.Equals(txtFW.Text);
    //                                break;
    //                            case 1:
    //                                add = up.Equals(txtUB.Text);
    //                                break;
    //                            case 2:
    //                                add = fw.Equals(txtFW.Text) && up.Equals(txtUB.Text);
    //                                break;
    //                        }
    //                        //if (match.Equals(txtFW.Text))
    //                        //{
    //                        //    filteredTable.Rows.Add(newRow);
    //                        //    filtered++;
    //                        //}
    //                        break;
    //                    case "Include Partial Matches":
    //                        //<asp:ListItem Text="Include Partial Matches" />
    //                        //if (match.Contains(txtFW.Text))
    //                        //{
    //                        //    filteredTable.Rows.Add(newRow);
    //                        //    filtered++;
    //                        //}
    //                        switch (match)
    //                        {
    //                            case 0:
    //                                add = fw.Contains(txtFW.Text);
    //                                break;
    //                            case 1:
    //                                add = up.Contains(txtUB.Text);
    //                                break;
    //                            case 2:
    //                                add = fw.Contains(txtFW.Text) && up.Contains(txtUB.Text);
    //                                break;
    //                        }
    //                        break;
    //                    case "Exclude Exact Matches":
    //                        //<asp:ListItem Text="Exclude Exact Matches" />
    //                        //if (!match.Equals(txtFW.Text))
    //                        //{
    //                        //    filteredTable.Rows.Add(newRow);
    //                        //    filtered++;
    //                        //}
    //                        switch (match)
    //                        {
    //                            case 0:
    //                                add = !fw.Equals(txtFW.Text);
    //                                break;
    //                            case 1:
    //                                add = !up.Equals(txtUB.Text);
    //                                break;
    //                            case 2:
    //                                add = !fw.Equals(txtFW.Text) &&  !up.Equals(txtUB.Text);
    //                                break;
    //                        }
    //                        break;
    //                    case "Exclude Partial Matches":
    //                        //<asp:ListItem Text="Exclude Partial Matches" />
    //                        switch (match)
    //                        {
    //                            case 0:
    //                                add = !fw.Contains(txtFW.Text);
    //                                break;
    //                            case 1:
    //                                add = !up.Contains(txtUB.Text);
    //                                break;
    //                            case 2:
    //                                add = !fw.Contains(txtFW.Text) && !up.Contains(txtUB.Text);
    //                                break;
    //                        }
    //                        //if (!match.Contains(txtFW.Text))
    //                        //{
    //                        //    filteredTable.Rows.Add(newRow);
    //                        //    filtered++;
    //                        //}
    //                        break;
    //                }
    //                if (add)
    //                {
    //                    filteredTable.Rows.Add(newRow);
    //                    filtered++;
    //                }


    //        }
    //        GridView.DataSource = filteredTable;
    //        GridView.DataBind();
    //        //tsTotalBoxes.Text = count.ToString();
    //        //tsFilterCount.Text = filtered.ToString();
    //        this.lTotalUnits.Text = count.ToString();
    //        this.lFilteredUnits.Text = filtered.ToString();
    //    }
    //    catch (Exception exc)
    //    {

    //    }
    //}


    private Dictionary<string, string> ParseText(string inputText, char fieldSeparator, char valueSeparator)
    {
        Dictionary<string, string> rvDict = new Dictionary<string, string>();

        string[] fields = inputText.Split(fieldSeparator);
        string[] keyValue = null;

        foreach (string oneField in fields)
        {
            if (string.IsNullOrEmpty(oneField.Trim()) == false)
            {
                keyValue = oneField.Split(valueSeparator);
                if (keyValue.Length >= 2)
                {
                    rvDict.Add(keyValue[0], keyValue[1]);
                }
                else if (keyValue.Length == 1)
                {
                    rvDict.Add(keyValue[0], string.Empty);
                }
            }
        }

        return rvDict;
    }

    void Filter()
    {
        Dictionary<string, string> resultDict = null;

        try
        {
            int count = 0;
            int filtered = 0;
            this.lReportingUnits.Text = (resultsTable.Rows.Count - 1).ToString();
            string searchType = ddlFilter.SelectedItem.Text;

            filteredTable = resultsTable.Clone();
            filteredTable.Columns.RemoveAt(2);
            filteredTable.Columns.RemoveAt(3);
            filteredTable.Columns.Add(new DataColumn("FirmwareVersion", typeof(string)));
            filteredTable.Columns.Add(new DataColumn("UpperBoardVersion", typeof(string)));
            filteredTable.Columns.Add(new DataColumn("Custom Prop", typeof(string)));
            filteredTable.Columns[2].ColumnName = "Description";
            foreach (DataRow row in resultsTable.Rows)
            {
                count = Convert.ToInt32(row.ItemArray[4]);
                if (Convert.ToInt32(row.ItemArray[0]) == 0)
                {
                    break;
                }

                string fw = string.Empty;
                string up = string.Empty;
                DataRow newRow = filteredTable.NewRow();
                newRow.ItemArray = row.ItemArray;
                string cp = (string)row.ItemArray[3];

                resultDict = ParseText(cp, ';', '=');
                //string[] sp = cp.Split(';');
                //string[] spx = sp[3].Split('=');
                string firmwareVersionValue = resultDict[FirmwareVersionKey];

                int index = firmwareVersionValue.IndexOf('(');
                if (index > -1)
                {
                    fw = firmwareVersionValue.Substring(0, index);
                    int index2 = firmwareVersionValue.IndexOf(')') + 1;
                    if (index2 > -1)
                    {
                        int index3 = firmwareVersionValue.IndexOf('(', index2);
                        if (index3 > -1)
                        {
                            up = firmwareVersionValue.Substring(index2, index3 - index2);
                        }
                    }
                }
                newRow[3] = fw;
                newRow[4] = up;
                newRow[5] = firmwareVersionValue;

                int match = (this.rbAX.Checked) ? 2 : (this.rbMB.Checked) ? 0 : 1;
                bool add = false;
                switch (searchType)
                {
                    case "No Filter":
                        filteredTable.Rows.Add(newRow);
                        filtered++;
                        break;
                    case "Include Exact Matches":
                        switch (match)
                        {
                            case 0:
                                add = fw.Equals(txtFW.Text);
                                break;
                            case 1:
                                add = up.Equals(txtUB.Text);
                                break;
                            case 2:
                                add = fw.Equals(txtFW.Text) && up.Equals(txtUB.Text);
                                break;
                        }
                        break;
                    case "Include Partial Matches":
                        switch (match)
                        {
                            case 0:
                                add = fw.Contains(txtFW.Text);
                                break;
                            case 1:
                                add = up.Contains(txtUB.Text);
                                break;
                            case 2:
                                add = fw.Contains(txtFW.Text) && up.Contains(txtUB.Text);
                                break;
                        }
                        break;
                    case "Exclude Exact Matches":
                        switch (match)
                        {
                            case 0:
                                add = !fw.Equals(txtFW.Text);
                                break;
                            case 1:
                                add = !up.Equals(txtUB.Text);
                                break;
                            case 2:
                                add = !fw.Equals(txtFW.Text) && !up.Equals(txtUB.Text);
                                break;
                        }
                        break;
                    case "Exclude Partial Matches":
                        switch (match)
                        {
                            case 0:
                                add = !fw.Contains(txtFW.Text);
                                break;
                            case 1:
                                add = !up.Contains(txtUB.Text);
                                break;
                            case 2:
                                add = !fw.Contains(txtFW.Text) && !up.Contains(txtUB.Text);
                                break;
                        }
                        break;
                }
                if (add)
                {
                    filteredTable.Rows.Add(newRow);
                    filtered++;
                }


            }
            GridView.DataSource = filteredTable;
            GridView.DataBind();
            this.lTotalUnits.Text = count.ToString();
            this.lFilteredUnits.Text = filtered.ToString();
        }
        catch (Exception exc)
        {

        }
    }



    class IntOptions
    {
        int _Value;
        public int Value { get { return _Value; } set { _Value = value; } }

        string _Display = string.Empty;
        public String Display { get { return _Display; } set { _Display = value; } }

        public IntOptions() : this(0, string.Empty) { }

        public IntOptions(int value, string display)
        {
            this._Display = display;
            this._Value = value;
        }

        public override string ToString()
        {
            return this._Display;
        }


    }


    protected void bFetch_Click(object sender, EventArgs e)
    {
        Fetch();
    }
    protected void bFilter_Click(object sender, EventArgs e)
    {
        Filter();
    }
    protected void ddlOrg_SelectedIndexChanged(object sender, EventArgs e)
    {
    }



    protected void GridView_SortCommand(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
    {
        Filter();
    }
    protected void GridView_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
    {

        Filter();
    }
    protected void GridView_PageSizeChanged(object sender, Telerik.Web.UI.GridPageSizeChangedEventArgs e)
    {
        Filter();

    }
    protected void GridView_DataBound(object sender, EventArgs e)
    {

    }
}
}