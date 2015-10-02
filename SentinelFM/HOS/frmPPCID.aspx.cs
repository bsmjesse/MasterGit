using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace SentinelFM
{
    public partial class HOS_frmPPCID : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (sn.User.OrganizationId <= 0) RedirectToLogin();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //lblMessage.Text = Ex.Message;
                RedirectToLogin();
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (sn == null || sn.User.OrganizationId == 0 || sn.UserID == 0)
            {
                lblMessage.Text = "Session time out.";
                return;
            }

            if (txtPPcid.Text.Trim() == "")
            {
                lblMessage.Text = "Please input boxid and MDt ppcid(serial #).";
                return;
            }
            else
            {
                try
                {
                    clsHOSManager clsHos = new clsHOSManager();
                    string ppcidseed = clsHos.CreateHOSCompany(sn.User.OrganizationId);
                    string sourceStr = txtPPcid.Text.Trim();
                    StringReader lines = new StringReader(sourceStr);
                    string line;
                    string newSource = "";
                    bool noSpace = false;
                    while ((line = lines.ReadLine()) != null)
                    {
                        Boolean hasNew = false;
                        string[] strs = line.Trim().Split(' ');
                        if (strs.Length >= 2)
                        {
                            int boxid = 0;
                            string ppcid = strs[strs.Length - 1];
                            int.TryParse(strs[0], out boxid);
                            if (boxid > 0 && ppcid.Length > 0)
                            {
                                string newppcid = "";
                                int index = 0;
                                if (ppcid.IndexOf("-") < 0) //the serial number is not ppcid then create ppcid
                                {
                                    try
                                    {
                                        if (ppcid.Length <= 3)
                                            newppcid = ppcid + "-" + ppcidseed;
                                        else newppcid = ppcid.Substring(0, 3) + ppcidseed;
                                        index = 3;
                                        newppcid = newppcid + "-" + ppcid.Substring(3, 4);
                                        index = 7;
                                        newppcid = newppcid + "-" + ppcid.Substring(7, 3);
                                        index = 10;
                                        newppcid = newppcid + "-" + ppcid.Substring(10);
                                    }
                                    catch (Exception ex) {
                                        if (ppcid.Length > index)
                                        {
                                            newppcid = newppcid + "-" + ppcid.Substring(index);
                                        }
                                    }
                                    if (newppcid == "") newppcid = ppcid + "-" + ppcidseed;
                                }
                                else newppcid = ppcid;
                                string boxidStr = boxid.ToString();
                                if  (txtDesc.Text.Trim() == "")
                                    txtDesc.Text = boxidStr.PadRight(10, ' ') + newppcid;
                                else txtDesc.Text = txtDesc.Text + System.Environment.NewLine + boxidStr.PadRight(10, ' ') + newppcid;
                                clsHos.UpdateVehicleAssignment(boxid, newppcid, sn.User.OrganizationId);

                                hasNew = true;
                            }

                        }
                        else noSpace = true;
                        if (!hasNew)
                        {
                            if (newSource == "")
                                newSource = line;
                            else newSource = newSource + System.Environment.NewLine + line;
                        }
                    }
                    txtPPcid.Text = newSource;
                    if (noSpace)
                        lblMessage.Text = "There is no space between boxid and mdt Serial #(ppcid)";
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                       Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    lblMessage.Text = "Failed to submit";
                }
                
            }
        }
    }
}