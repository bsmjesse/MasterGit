using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
public partial class HOS_HosTabs : System.Web.UI.UserControl
{
    public string SelectedControl = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SelectedControl))
        {
            if (FindControl(SelectedControl) is Button)
            {
                ((Button)FindControl(SelectedControl)).CssClass = "selectedbutton";
                ((Button)FindControl(SelectedControl)).OnClientClick = "javascript:return false;";
                SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
                if (sn != null)
                {
                    cmdPPCID.Visible = sn.User.ControlEnable(sn, 91);
                    cmdQuestion.Visible = sn.User.ControlEnable(sn, 98);
                    cmdQuestionSet.Visible = sn.User.ControlEnable(sn, 99);
                    cmdQuestionSetAssignment.Visible = sn.User.ControlEnable(sn, 100);
                    cmdQRCode.Visible = sn.User.ControlEnable(sn, 101);
                    cmdSetting.Visible = sn.User.ControlEnable(sn, 111);
                    cmdCycleAssignemt.Visible = sn.User.ControlEnable(sn, 112);
                }
            }
        }
    }
}