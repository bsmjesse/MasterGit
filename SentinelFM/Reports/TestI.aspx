<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestI.aspx.cs" Inherits="Reports_TestI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
                <script type="text/javascript">
                    function DeleteScheduleReportsFile(ctl) {
                        if (confirm('Are you sure') == false) return
                        var id = " 2910 ";
                        var postData = "";
                        var ReportID = "-1";
                        try {
                            id = $telerik.$.trim(id);
                            postData = "{'ReportId':'" + ReportID + "','RowId':'" + id + "', 'PageName':'<%=  Page.GetType().Name %>'}";
                        }
                        catch (error) { }
                        return false;
                    }
                </script>
        </telerik:RadCodeBlock>
        <table>
          <tr>
            <td>
              All SentinelFM.SecurityManager.SecurityManager methods
            </td>
          </tr>
          <tr>
            <td>
        <asp:ListBox ID="lst" runat ="server" Width = 500 Height = 400 ></asp:ListBox>
            </td>
          </tr>

        </table>
    </div>
    </form>
</body>
</html>
