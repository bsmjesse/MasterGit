<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HosTabs.ascx.cs" Inherits="HOS_HosTabs" %>
<table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td style="width: 180px">
            <asp:Button ID="cmdVehicleDrivers" runat="server" Text="Vehicle-Drivers Assignment" 
            CausesValidation="False" CssClass="confbutton"
            meta:resourcekey="cmdVehicleDriversResource" 
            PostBackUrl="~/HOS/frmHOSAssignDrivers.aspx"
            width = "180px"
            />
        </td>
        <td style="width: 180px">
            <asp:Button ID="cmdEmailAddress" runat="server" Text="Email Address" CssClass="confbutton" CausesValidation="False" visible = "true"
                 meta:resourcekey="cmdEmailAddressResource" PostBackUrl="~/HOS/frmEmailAddress.aspx" 
                 width = "180px"
                 >
            </asp:Button>
        </td>
        <td style="width: 180px">
            <asp:Button ID="cmdMDTVersion" runat="server" Text="MDT Version" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmMDTVersion.aspx" meta:resourcekey="cmdMDTVersionResource"
                width = "180px"
                >
            </asp:Button>
        </td>
        <td >
            <asp:Button ID="cmdQuestion" runat="server" Text="Questions" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmQuestion.aspx" meta:resourcekey="cmdQuestionResource"
                width = "100px"
                >
            </asp:Button>
        </td>
        <td >
            <asp:Button ID="cmdQuestionSet" runat="server" Text="Question Form" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmDynamicInspections.aspx" meta:resourcekey="cmdQuestionSetResource"
                width = "100px"
                >
            </asp:Button>
        </td>
        <td >
            <asp:Button ID="cmdQuestionSetAssignment" runat="server" Text="Form Assignment" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmInspectionsAssignment.aspx" meta:resourcekey="cmdQuestionSetResource"
                width = "120px"
                >
            </asp:Button>
        </td>
        <td >
            <asp:Button ID="cmdQRCode" runat="server" Text="Form and QR Code" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmPrintQrCode.aspx" meta:resourcekey="cmdQRCodeResource"
                width = "130px"
                >
            </asp:Button>
        </td>

        <td >
            <asp:Button ID="cmdCycleAssignemt" runat="server" Text="Cycle Assignment" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmCycleAssignment.aspx" meta:resourcekey="cmdQRCodeResource"
                width = "130px"
                >
            </asp:Button>
        </td>

        <td style="width: 200px">
            <asp:Button ID="cmdPPCID" runat="server" Text="Create(Pair) Box & MDT" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmPPCID.aspx" meta:resourcekey="cmdPPCIDResource"
                width = "200px" CommandName="91"
                >
            </asp:Button>
        </td>
        <td style="width: 60px">
            <asp:Button ID="cmdSetting" runat="server" Text="Settings" CssClass="confbutton"
                CausesValidation="False" PostBackUrl="~/HOS/frmHOSSetting.aspx" meta:resourcekey="cmdcmdSettingResource"
                width = "60px" CommandName="91"
                >
            </asp:Button>
        </td>

    </tr>
</table>
