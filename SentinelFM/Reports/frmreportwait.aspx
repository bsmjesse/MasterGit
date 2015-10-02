<%@ Page Language="c#" Inherits="SentinelFM.Reports.frmReportWait" CodeFile="frmReportWait.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Processing...</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    
</head>
<body class="configTabBackground">
    <form method="post" runat="server">
        <table class="table"  style='z-index: 101;left: 317px; width: 401px;position: absolute; top: 207px; height: 128px' width="401" border="0" cellpadding="0"
            cellspacing="0">
            <tr>
                <td align="center" valign="middle">
                    <table width="398" bgcolor="#ffffff" border="0" cellpadding="0" cellspacing="0" style="width: 398px;
                        height: 110px">
                        <tr>
                            <td align="center" valign="middle">
                                <font face='Arial, Verdana' size="4" color="gray"><b>
                                    <p>
                                        <br>
                                        <asp:Label ID="lblPleaseWaitMessage" runat="server" meta:resourcekey="lblPleaseWaitMessageResource1">Please wait</asp:Label>&nbsp;</p>
                                    <p>
                                        <asp:Label ID="lblPreparingMessage" runat="server" meta:resourcekey="lblPreparingMessageResource1">Preparing the Report</asp:Label>&nbsp;...</p>
                                    <p>
                                        &nbsp;</p>
                                </b></font>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div>
        </div>
    </form>
</body>
</html>
