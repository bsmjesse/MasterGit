using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

public partial class frmHOSForms : System.Web.UI.Page
{
    clsHOSManager clsManager = new clsHOSManager();
    protected SentinelFMSession sn = null;
    DataTable currentFormDt = null;
    int elementIndex = 6;
    Boolean editFirst = false;
    protected void Page_Load(object sender, EventArgs e)
    {
            sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}

            if (!Page.IsPostBack)
            {
                //dvForms.EmptyDataText = "No Records Found";
                BindHOSForms();
            }

            if (pnlNewForm.Visible)
            {
                GenerateCurrentFormDt();
            }
    }

    private void BindHOSForms()
    {
        DataSet ds = clsManager.GetFormDefinition(); ;
        if (ds.Tables[0].Rows.Count == 0)
        {
            ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
            dvForms.DataSource = ds;
            dvForms.DataBind();
            int columncount = dvForms.Items[0].Cells.Count;
            dvForms.Items[0].Cells.Clear();
            dvForms.Items[0].Cells.Add(new TableCell());
            dvForms.Items[0].Cells[0].ColumnSpan = columncount;
            dvForms.Items[0].Cells[0].Text = "No Records Found";
        }
        else
        {
            //ddlForm.Items.Clear();
            //ddlForm.Items.Add("Please select a from");
            //foreach (DataRow dr in ds.Tables[0].Rows) ddlForm.Items.Add(dr["FormName"].ToString());
            dvForms.DataSource = ds;
            dvForms.DataBind();
        }
    }
    protected void btnNewForm_Click(object sender, EventArgs e)
    {
        ViewState["currentFormId"] = -1;
        pnlNewForm.Visible = true;
        pnlForms.Visible = false;
        pnlAssignment.Visible = false;
        currentFormDt = CreateFormDataTable();
        currentFormDt.Rows.Add(currentFormDt.NewRow());
        currentFormDt.Rows[currentFormDt.Rows.Count - 1]["ID"] = "I_" + findMaxRowsId();
        dvForm.DataSource = currentFormDt;
        dvForm.DataBind();
    }



    private DataTable CreateFormDataTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID");
        dt.Columns.Add("Label");
        dt.Columns.Add("Type");
        dt.Columns.Add("Content");
        return dt;
    }
    protected void btnNewLine_Click(object sender, EventArgs e)
    {
        currentFormDt.Rows.Add(currentFormDt.NewRow());
        currentFormDt.Rows[currentFormDt.Rows.Count - 1]["ID"] = "I_" + findMaxRowsId();
        dvForm.DataSource = currentFormDt;
        dvForm.DataBind();

    }

    private string findMaxRowsId()
    {
        int rowCount = 0;
        foreach (DataRow dr in currentFormDt.Rows)
        {
            try
            {
                int curID = int.Parse(dr["ID"].ToString().Replace("I_", ""));
                if (rowCount < curID) rowCount = curID;
            }
            catch (Exception ex) { }
        }
        return string.Format("{0:D2}", rowCount + 1);
    }

    protected void btnBackFrom_Click(object sender, EventArgs e)
    {
        pnlNewForm.Visible = false;
        pnlForms.Visible = true;
        pnlAssignment.Visible = false;
        BindHOSForms();
    }
    protected void dvForm_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        if (e.CommandName == "AddElement")
        {
            DataGrid plh = (DataGrid)e.Item.FindControl("btnPlaceHolder");
            DropDownList lstType = (DropDownList)e.Item.FindControl("lstType");
            if (lstType.SelectedIndex < elementIndex) return;
            DataTable dt = new DataTable();
            dt.Columns.Add("Element");
            foreach (DataGridItem dgi in plh.Items)
            { 
                DataRow dr = dt.NewRow();
                dr["Element"] = ((TextBox)dgi.Cells[0].Controls[1]).Text;
                dt.Rows.Add(dr);

            }
            dt.Rows.Add(dt.NewRow());
            plh.DataSource = dt;
            plh.DataBind();
        }

        if (e.CommandName == "UpControl")
        {
            int index = e.Item.ItemIndex ;
            DataRow selectedRow = currentFormDt.Rows[index];
            DataRow newRow = currentFormDt.NewRow();
            newRow.ItemArray = selectedRow.ItemArray; // copy data
            currentFormDt.Rows.Remove(selectedRow);
            currentFormDt.Rows.InsertAt(newRow, index - 1);
            dvForm.DataSource = currentFormDt;
            dvForm.DataBind();

        }

        if (e.CommandName == "DownControl")
        {
            int index = e.Item.ItemIndex ;
            DataRow selectedRow = currentFormDt.Rows[index];
            DataRow newRow = currentFormDt.NewRow();
            newRow.ItemArray = selectedRow.ItemArray; // copy data
            currentFormDt.Rows.Remove(selectedRow);
            currentFormDt.Rows.InsertAt(newRow, index + 1);
            dvForm.DataSource = currentFormDt;
            dvForm.DataBind();

        }

        if (e.CommandName == "DeleteCurrentForm")
        { 
            currentFormDt.Rows.RemoveAt(e.Item.ItemIndex);
            dvForm.DataSource = currentFormDt;
            dvForm.DataBind();
        }
    }
    protected void lstType_IndexSelected(object sender, EventArgs e)
    {
        if (((DropDownList)sender).SelectedIndex >= elementIndex)
        {
            DataGridItem dgi = ((DataGridItem)((DropDownList)sender).Parent.Parent);
            DataGrid plh = (DataGrid)dgi.FindControl("btnPlaceHolder");
            Button btnAddElement = (Button)dgi.FindControl("btnAddElement");
            plh.Visible = true;
            btnAddElement.Visible = true;
        }
        else
        {
            DataGridItem dgi = ((DataGridItem)((DropDownList)sender).Parent.Parent);
            DataGrid plh = (DataGrid)dgi.FindControl("btnPlaceHolder");
            Button btnAddElement = (Button)dgi.FindControl("btnAddElement");
            plh.Visible = false;
            btnAddElement.Visible = false;
        }
    }

    private void GenerateCurrentFormDt()
    { 
        currentFormDt = CreateFormDataTable();
        foreach(DataGridItem dgt in dvForm.Items)
        {
            TextBox txtLabel = (TextBox)dgt.FindControl("txtLabel");
            DropDownList lstType = (DropDownList)dgt.FindControl("lstType");
            DataGrid plh = (DataGrid)dgt.FindControl("btnPlaceHolder");
            Label txtID = (Label)dgt.FindControl("txtID");
            StringBuilder content = new StringBuilder();
            foreach (DataGridItem dgi in plh.Items)
            {
                if (content.Length == 0)
                   content.Append(((TextBox)dgi.Cells[0].Controls[1]).Text );
                else
                    content.Append("@@@@" + ((TextBox)dgi.Cells[0].Controls[1]).Text.Replace("@@@@", ""));
            }
            DataRow dr = currentFormDt.NewRow();
            dr["ID"] = txtID.Text.ToString().Trim();
            dr["Label"] = txtLabel.Text;
            dr["Type"] = lstType.SelectedValue;
            dr["Content"] = content.ToString();
            currentFormDt.Rows.Add(dr);
        }
    }
    protected void dvForm_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        Label txtID = (Label)e.Item.FindControl("txtID");
        TextBox txtLabel = (TextBox)e.Item.FindControl("txtLabel");
        DropDownList lstType = (DropDownList)e.Item.FindControl("lstType");
        DataGrid plh = (DataGrid)e.Item.FindControl("btnPlaceHolder");
        Button btnAddElement = (Button)e.Item.FindControl("btnAddElement");
        ImageButton btnUp = (ImageButton)e.Item.FindControl("btnUp");
        ImageButton btnDown = (ImageButton)e.Item.FindControl("btnDown");
        if (e.Item.DataItem is DataRowView)
        {
            if (editFirst ) txtID.Enabled = false;
            if (e.Item.ItemIndex == 0)
            {
                btnUp.Visible = false;
            }
            if (e.Item.ItemIndex >= currentFormDt.Rows.Count - 1)
            {
                btnDown.Visible = false;
            }

            DataRowView drv = (DataRowView)e.Item.DataItem;

            txtID.Text = drv["ID"].ToString();
            txtLabel.Text = drv["Label"].ToString();

            int index = 0;
            int.TryParse(drv["Type"].ToString(), out index);
            lstType.SelectedValue = index.ToString(); ;
            String[] strs = Regex.Split(drv["Content"].ToString(), "@@@@");
            DataTable dt = new DataTable();
            dt.Columns.Add("Element");
            foreach (string dgi in strs)
            {
                if (dgi.Trim() != "")
                {
                    DataRow dr = dt.NewRow();
                    dr["Element"] = dgi;
                    dt.Rows.Add(dr);
                }

            }
            if (index >= elementIndex) btnAddElement.Visible = true;
            plh.DataSource = dt;
            plh.DataBind();
        }

    }
    protected void btnSaveForm_Click(object sender, EventArgs e)
    {
        if (txtFormName.Text.Trim() == "") {lblError.Text = "Please input form name.";return;}
        int count = 0;
        string idStr = "";
        foreach (DataRow dr in currentFormDt.Rows)
        { 
            int index = 0;
            int.TryParse(dr["Type"].ToString(), out index);
            string curID = "," + dr["ID"].ToString() + ",";
            if (idStr.Contains(curID))
            {
                lblError.Text = "Please remove duplicate ID."; return;
            }
            idStr = idStr + "," + dr["ID"].ToString() + ",";
            if (dr["Label"].ToString().Trim() == "" ||
               (index >= elementIndex && dr["Content"].ToString().Trim() == ""))
                continue;
            if (dr["ID"].ToString() == "")
            {
                lblError.Text = "Please input ID."; return;
            }

            count = count + 1;
        }

        if (count == 0) { lblError.Text = "Please complete each item."; return; }
        string xformcontent = GenerateXfrom();
        clsManager.AddOrUpdateFormDefinition((int)ViewState["currentFormId"], txtFormName.Text.Trim().Replace("@@@@", ""), xformcontent, sn.UserID);
        btnBackFrom_Click(null, null);
    }

    private String GenerateXfrom()
    {
        StringBuilder formStr = new StringBuilder();
        StringBuilder modelStr = new StringBuilder();
        foreach (DataRow dr in currentFormDt.Rows)
        {
            int index = 0;
            int.TryParse(dr["Type"].ToString(), out index);
            string label = XmlConvert.EncodeName(dr["Label"].ToString());
            string content = dr["Content"].ToString().Trim();
            if (label.Trim() == "" ||
               (index >= elementIndex && content == ""))
                continue;
            String[] strs = Regex.Split(content, "@@@@");
            string id = dr["ID"].ToString();
            string str = "";
            switch (index)
	        {
                case 0:
                    str = string.Format("<xf:input ref=\"{0}\"><xf:label>{1}</xf:label></xf:input>", id, label); 
                    formStr.Append(str);

                    str = string.Format("<{0} xsi:type=\"xsd:string\" />", id);
                    modelStr.Append(str);

                    break;
                case 1:
                    str = string.Format("<xf:textarea ref=\"{0}\"><xf:label>{1}</xf:label></xf:textarea>", id, label);
                    formStr.Append(str);

                    str = string.Format("<{0} xsi:type=\"xsd:string\" />", id);
                    modelStr.Append(str);
                    break;
                case 2:
                    str = string.Format("<xf:input ref=\"{0}\"><xf:label>{1}</xf:label></xf:input>", id, label);
                    formStr.Append(str);

                    str = string.Format("<{0} xsi:type=\"xsd:date\" />", id);
                    modelStr.Append(str);
                    break;
                case 3:
                    str = string.Format("<xf:input ref=\"{0}\"><xf:label>{1}</xf:label></xf:input>", id, label);
                    formStr.Append(str);

                    str = string.Format("<{0} xsi:type=\"xsd:time\" />", id);
                    modelStr.Append(str);
                    break;
                case 4:
                    str = string.Format("<xf:message ref=\"{0}\" >{1}</xf:message>", id, label);
                    formStr.Append(str);
                    str = string.Format("<{0} xsi:type=\"xsd:string\" />", id);
                    formStr.Append(str);
                    break;
                case 5:
                    str = string.Format("<xf:input ref=\"{0}\"><xf:label>{1}</xf:label></xf:input>", id, label);
                    formStr.Append(str);

                    str = string.Format("<{0} xsi:type=\"xsd:boolean\" />", id);
                    modelStr.Append(str);
                    break;
                case 6:
                    str = string.Format("<xf:select1 ref=\"{0}\"><xf:label>{1}</xf:label>", id, label);
                    formStr.Append(str);
                    foreach (string dgi in strs)
                    {
                        str = string.Format("<xf:item><xf:label>{0}</xf:label></xf:item>", XmlConvert.EncodeName(dgi));
                        formStr.Append(str);
                    }
                    formStr.Append("</xf:select1>");
                    str = string.Format("<{0} xsi:type=\"xsd:string\" />", id);
                    modelStr.Append(str);
                    break;
                case 7:
                    str = string.Format("<xf:select1 ref=\"{0}\" appearance=\"partial\" ><xf:label>{1}</xf:label>", id, label);
                    formStr.Append(str);
                    foreach (string dgi in strs)
                    {
                        str = string.Format("<xf:item><xf:label>{0}</xf:label></xf:item>", XmlConvert.EncodeName(dgi));
                        formStr.Append(str);
                    }

                    formStr.Append("</xf:select1>");
                    str = string.Format("<{0} xsi:type=\"xsd:string\" />", id);
                    modelStr.Append(str);
                    break; 
                case 8:
                    str = string.Format("<xf:select1 ref=\"{0}\" appearance=\"full\"><xf:label>{1}</xf:label>", id, label);
                    formStr.Append(str);
                    foreach (string dgi in strs)
                    {
                        str = string.Format("<xf:item><xf:label>{0}</xf:label></xf:item>", XmlConvert.EncodeName(dgi));
                        formStr.Append(str);
                    }

                    formStr.Append("</xf:select1>");
                    str = string.Format("<{0} xsi:type=\"xsd:string\" />", id);
                    modelStr.Append(str);
                    break;
	        }
        }

        StringBuilder sp = new StringBuilder();
        if (formStr.Length > 0)
        {
            sp.Append(@"<html xmlns='http://www.w3.org/1999/xhtml' xmlns:xf='http://www.w3.org/2002/xforms' xmlns:ev='http://www.w3.org/2001/xml-events'  xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>");
            sp.Append("<head>");
            sp.Append(string.Format("<title>{0}</title>", txtFormName.Text.Trim()));
            sp.Append(string.Format("<xf:model><xf:instance>{0}</xf:instance></xf:model>", modelStr.ToString()));
            sp.Append(string.Format("</head><body>{0}</body></html>", formStr.ToString()));
        }
        return sp.ToString();
    }
    protected void dvForms_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        ViewState["currentFormId"] = int.Parse(dvForms.DataKeys[e.Item.ItemIndex].ToString());
        if (e.CommandName == "DeleteForm")
        {
            if (clsManager.DeleteFormDefinition((int)ViewState["currentFormId"]) == -1)
            {
                lblFormError.Text = "Failed to delete because the form has been assigned to organization(s).";
            }
            btnBackFrom_Click(null, null);
        }

        if (e.CommandName == "FormAssignment")
        {
            pnlAssignment.Visible = true;
            pnlForms.Visible = false;
            pnlNewForm.Visible = false;

            ddlForm.Text ="Form Name: "  + ((Label)e.Item.FindControl("lblMsg")).Text;

            BindAssignment();
        }

        if (e.CommandName == "EditForm")
        {
            pnlNewForm.Visible = true;
            pnlForms.Visible = false;
            pnlAssignment.Visible = false;
            DataTable dt = clsManager.GetFormDefinitionByFormId((int)ViewState["currentFormId"]);
            txtFormName.Text = dt.Rows[0]["FormName"].ToString();

            string definition = dt.Rows[0]["Definition"].ToString();
            Dictionary<String, String> dictIDs = new Dictionary<string, string>();
            Dictionary<String, String> dictItems = new Dictionary<string, string>();
            Boolean startData = false;
            string element =  "";
            string id = "";
            string label = "";
            string appearance = "";
            string itemContent = "";
            string controlType = "";
            currentFormDt = CreateFormDataTable();
            using (XmlReader reader = XmlReader.Create(new StringReader(definition)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "xf:instance")
                            {
                                startData = true;
                            }
                            if (startData)
                            {
                                element = reader.Name;

                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "xsi:type")
                                        dictIDs.Add(element, reader.Value.Replace("xsd:", ""));
                                }
                            }

                            controlType = reader.Name;
                            if (controlType == "xf:input" || controlType == "xf:textarea"
                                || controlType == "xf:select1" || controlType == "xf:message"
                                )
                            {
                                appearance = "";
                                id = "";
                                label = "";
                                itemContent = "";

                                while (reader.HasAttributes && reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "ref")
                                        id = reader.Value;
                                    if (reader.Name == "appearance")
                                        appearance = reader.Value;
                                }
                                if (controlType != "xf:message")
                                {
                                    while (reader.Read())
                                    {
                                        if (reader.NodeType == XmlNodeType.Element)
                                        {
                                            if (reader.Name == "xf:label")
                                            {
                                                label = reader.ReadString();
                                            }
                                            if (controlType != "xf:select1") break;
                                            while (reader.Read())
                                            {
                                                if (reader.Name == "xf:label")
                                                {
                                                    if (itemContent == "")
                                                        itemContent = XmlConvert.DecodeName(reader.ReadString());
                                                    else itemContent = itemContent + "@@@@" + XmlConvert.DecodeName(reader.ReadString());
                                                }
                                                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "xf:select1") break;
                                            }
                                            break;
                                        }
                                    }
                                }
                                else label = reader.ReadString();

                                DataRow dr = currentFormDt.NewRow();
                                dr["ID"] = id;
                                dr["Label"] = XmlConvert.DecodeName(label);
                                dr["Content"] = itemContent;
                                if (controlType == "xf:textarea")
                                {
                                    if (dictIDs[id] == "string") dr["Type"] = 1;
                                }
                                if (controlType == "xf:message")
                                {
                                     dr["Type"] = 4;
                                }

                                if (controlType == "xf:input")
                                {
                                    if (dictIDs[id] == "string" ) dr["Type"] = 0;
                                    if (dictIDs[id] == "date") dr["Type"] = 2;
                                    if (dictIDs[id] == "time") dr["Type"] = 3;
                                    if (dictIDs[id] == "boolean") dr["Type"] = 5;
                                }
                                if (controlType == "xf:select1")
                                {
                                    if (appearance == "") dr["Type"] = 6;
                                    if (appearance == "partial") dr["Type"] = 7;
                                    if (appearance == "full") dr["Type"] = 8;
                                }
                                currentFormDt.Rows.Add(dr);
                            }

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "xf:instance")
                            {
                                startData = false;
                            }
                            break;
                    }
                }
            }
            editFirst = true;
            dvForm.DataSource = currentFormDt;
            dvForm.DataBind();
        }
    }

    private void BindAssignment()
    {
        DataTable assignDt = clsManager.GetFormAssignment((int)ViewState["currentFormId"],
            null, null, true);

        DataTable unAssignDt = clsManager.GetFormAssignment((int)ViewState["currentFormId"],
            null, null, false);

        lboUnassigned.DataSource = unAssignDt;
        lboUnassigned.DataBind();

        lboassigned.DataSource = assignDt;
        lboassigned.DataBind();
        lblAssignmentError.Visible = false;
    }

    protected void cmdAssign_Click(object sender, EventArgs e)
    {
        if (lboUnassigned.Items.Count == 0) return;
        if (lboUnassigned.GetSelectedIndices().Length == 0)
        {
            lblAssignmentError.Text = "Please select a organization.";
            lblAssignmentError.Visible = true;
            return;
        }

        foreach (int index in lboUnassigned.GetSelectedIndices())
        clsManager.AddFormAssignment((int)ViewState["currentFormId"], int.Parse(lboUnassigned.Items[index].Value), sn.UserID);
        BindAssignment();

    }
    protected void cmdUnAssign_Click(object sender, EventArgs e)
    {
        if (lboassigned.Items.Count == 0) return;
        if (lboassigned.GetSelectedIndices().Length == 0)
        {
            lblAssignmentError.Text = "Please select a organization.";
            lblAssignmentError.Visible = true;
            return;
        }
        foreach (int index in lboassigned.GetSelectedIndices())
            clsManager.DeleteFormAssignment((int)ViewState["currentFormId"], int.Parse(lboassigned.Items[index].Value), sn.UserID);

        BindAssignment();
    }
}
