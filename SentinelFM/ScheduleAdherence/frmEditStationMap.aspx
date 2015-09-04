<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEditStationMap.aspx.cs" Inherits="ScheduleAdherence_frmEditStationMap" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../GlobalStyle.css" />
    <script type="text/javascript">
        function MoreOption() {
            var table_main = document.getElementById("table_main");
            var table_options = document.getElementById("table_moreOptions");
            table_main.style.display = "none";
            table_options.style.display = "inline";
        }
        function MainOption() {
            var table_main = document.getElementById("table_main");
            var table_options = document.getElementById("table_moreOptions");
            table_main.style.display = "inline";
            table_options.style.display = "none";
        }
        function ConfirmDeleteion() {
            var msg;
            if (document.getElementById("hdType").value == "1")
                msg = "Are you sure you want to delete this station?";
            else
                msg = "Are you sure you want to delete this depot?";

            if (confirm(msg))
                return true;
            else
                return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField id="hdAction" runat="server" />
    <asp:HiddenField ID="hdStationId" runat="server" />
    <asp:HiddenField ID="hdLandmarkId" runat="server" />
    <asp:HiddenField ID="hdType" runat="server" />
    <asp:HiddenField ID="hdLon" runat="server" />
    <asp:HiddenField ID="hdLat" runat="server" />
    <asp:HiddenField ID="hdCurName" runat="server" />
<h6 style="margin:0 0 10px 10px;border-bottom:1px solid #cccccc" id="h6_title" runat="server"></h6>
<table id="table_main" width="100%" class="formtext">
    <tr style="height:20px">
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext" ></asp:ValidationSummary>
        </td>
    </tr>
    <tr style="height:20px">
        <td style="width:100px">
            <asp:Label ID="lb_name" runat="server"></asp:Label>:
            <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName"
                ErrorMessage="Please enter a Name" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td>
            <asp:Label ID="lbName" runat="server" CssClass="formtext" />
            <asp:textbox id="txtName" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr style="height:20px">
        <td><asp:Label ID="lb_number" runat="server"></asp:Label>:</td>
        <td>
            <asp:Label ID="lbNumber" runat="server" CssClass="formtext" />
            <asp:textbox id="txtNumber" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr style="height:20px">
        <td>Radius(m):</td>
        <td>
            <asp:Label ID="lbRadius" runat="server" CssClass="formtext" />
            <asp:textbox id="txtRadius" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr style="height:20px">
        <td>Contact:</td>
        <td>
            <asp:Label ID="lbContact" runat="server" CssClass="formtext" />
            <asp:textbox id="txtContact" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr style="height:20px">
        <td>
            Phone:
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                ControlToValidate="txtPhone" CssClass="formtext" 
                ErrorMessage="Invalid Phone Number:" 
            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$">*</asp:RegularExpressionValidator>
        </td>
        <td>
            <asp:Label ID="lbPhone" runat="server" CssClass="formtext" />
            <asp:textbox id="txtPhone" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <a href='javascript:void(0)' onclick='MoreOption();'>More Options</a>
        </td>
    </tr>
    </table>
    <table id="table_moreOptions" style="display:none" width="100%" class="formtext">
    <tr style="height:20px">
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary2" runat="server" CssClass="errortext" ></asp:ValidationSummary>
        </td>
    </tr>
    <tr style="height:20px">
        <td style="width:100px">Fax:
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                ControlToValidate="txtFax" CssClass="formtext" 
                ErrorMessage="Invalid Fax Number:"
            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$">*</asp:RegularExpressionValidator>
        </td>
        <td>
            <asp:Label ID="lbFax" runat="server" CssClass="formtext" />
            <asp:textbox id="txtFax" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr style="height:20px">
        <td>Email:
            <asp:RegularExpressionValidator ID="rev_txtEmail" runat="server" 
                ControlToValidate="txtEmail" CssClass="formtext" 
                ErrorMessage="Invalid Email:"
            ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$">*</asp:RegularExpressionValidator>
        </td>
        <td>
            <asp:Label ID="lbEmail" runat="server" CssClass="formtext" />
            <asp:textbox id="txtEmail" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr style="height:20px">
        <td>Address:</td>
        <td>
            <asp:Label ID="lbAddress" runat="server" CssClass="formtext" />
            <asp:textbox id="txtAddress" runat="server" CssClass="formtext" ></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>Description:</td>
        <td>
            <asp:Label ID="lbDescription" runat="server" CssClass="formtext" />
            <asp:textbox id="txtDescription" runat="server"  CssClass="formtext" TextMode="MultiLine"></asp:textbox>
        </td>
    </tr>
    <tr style="height:20px">
        <td>Time Zone:</td>
        <td>
            <asp:Label ID="lbTimezone" runat="server" CssClass="formtext" />
        <asp:DropDownList ID="ddl_timezone" runat="server">
            <asp:ListItem Text="GMT-12" Value="-12" />
            <asp:ListItem Text="GMT-11" Value="-11" />
            <asp:ListItem Text="GMT-10" Value="-10" />
            <asp:ListItem Text="GMT-9" Value="-9" />
            <asp:ListItem Text="GMT-8" Value="-8" />
            <asp:ListItem Text="GMT-7" Value="-7" />
            <asp:ListItem Text="GMT-6" Value="-6" />
            <asp:ListItem Text="GMT-5" Value="-5" />
            <asp:ListItem Text="GMT-4" Value="-4" />
            <asp:ListItem Text="GMT-3" Value="-3" />
            <asp:ListItem Text="GMT-2" Value="-2" />
            <asp:ListItem Text="GMT-1" Value="-1" />
            <asp:ListItem Text="GMT" Value="0" />
            <asp:ListItem Text="GMT+1" Value="1" />
            <asp:ListItem Text="GMT+2" Value="2" />
            <asp:ListItem Text="GMT+3" Value="3" />
            <asp:ListItem Text="GMT+4" Value="4" />
            <asp:ListItem Text="GMT+5" Value="5" />
            <asp:ListItem Text="GMT+6" Value="6" />
            <asp:ListItem Text="GMT+7" Value="7" />
            <asp:ListItem Text="GMT+8" Value="8" />
            <asp:ListItem Text="GMT+9" Value="9" />
            <asp:ListItem Text="GMT+10" Value="10" />
            <asp:ListItem Text="GMT+11" Value="11" />
            <asp:ListItem Text="GMT+12" Value="12" />
            <asp:ListItem Text="GMT+13" Value="13" />
        </asp:DropDownList>
        </td>
    </tr>
    <tr style="height:20px">
        <td colspan="2">
            <asp:Label ID="lbDayLight" runat="server" CssClass="formtext" />
            <asp:CheckBox ID="ck_dayLight" runat="server" Text="Automatically adjust for daylight savings time" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <a href='javascript:void(0)' onclick='MainOption();'>Back</a>
        </td>
    </tr>
</table>
<div style="width:100%;">
    <asp:button id="cmdEdit" runat="server" Width="50px" CssClass="combutton" Text="Edit" onclick="cmdEdit_Click" />
    <asp:button id="cmdSave" runat="server" Width="50px" CssClass="combutton" Text="Save" onclick="cmdSave_Click" />
    <asp:button id="cmdDelete" runat="server" Width="50px" CausesValidation="false" CssClass="combutton" Text="Delete" 
        OnClientClick="return ConfirmDeleteion();" onclick="cmdDelete_Click" />
    <asp:button id="cmdCancel" runat="server" Width="50px" CausesValidation="false" CssClass="combutton" Text="Cancel" onclick="cmdCancel_Click" />
</div>
    </form>
</body>
</html>
