<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEditStation.aspx.cs" Inherits="ScheduleAdherence_frmEditStation" MasterPageFile="~/ScheduleAdherence/SAMasterPage.master"%>
<%@ MasterType  virtualPath="~/ScheduleAdherence/SAMasterPage.master"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField id="hdAction" runat="server" />
    <asp:HiddenField ID="hdStationId" runat="server" />
<table width="100%" class="formtext">
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext" ></asp:ValidationSummary>
        </td>
    </tr>
    <tr style="height:20px">
        <td style="width:100px">
            Station Name:
            <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName"
                ErrorMessage="Please enter a Name" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtName" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>Station Number:</td>
        <td><asp:textbox id="txtNumber" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>Type:</td>
        <td>
            <asp:DropDownList ID="ddlType" runat="server">
                <asp:ListItem Text="Station" Value="1"></asp:ListItem>
                <asp:ListItem Text="Depot" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr style="height:20px">
        <td>Landmark:</td>
        <td><asp:DropDownList ID="ddlLandmark" DataTextField="LandmarkName" DataValueField="LandmarkId" runat="server"></asp:DropDownList></td>
    </tr>
    <tr style="height:20px">
        <td>Contact:</td>
        <td><asp:textbox id="txtContact" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>
            Phone:
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                ControlToValidate="txtPhone" CssClass="formtext" 
                ErrorMessage="Invalid Phone Number:" 
            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$">*</asp:RegularExpressionValidator>
        </td>
        <td><asp:textbox id="txtPhone" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>Fax:
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                ControlToValidate="txtFax" CssClass="formtext" 
                ErrorMessage="Invalid Fax Number:"
            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$">*</asp:RegularExpressionValidator>
        </td>
        <td><asp:textbox id="txtFax" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>Email:
            <asp:RegularExpressionValidator ID="rev_txtEmail" runat="server" 
                ControlToValidate="txtEmail" CssClass="formtext" 
                ErrorMessage="Invalid Email:"
            ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$">*</asp:RegularExpressionValidator>
        </td>
        <td><asp:textbox id="txtEmail" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>Address:</td>
        <td><asp:textbox id="txtAddress" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr>
        <td>Description:</td>
        <td><asp:textbox id="txtDescription" runat="server"  CssClass="formtext" TextMode="MultiLine"></asp:textbox></td>
    </tr>
    <tr style="height:50px">
        <td colspan="2" align="center">
            <asp:button id="cmdSave" runat="server" Width="50px" CssClass="combutton" Text="Save" onclick="cmdSave_Click" />
            <asp:button id="cmdCancel" runat="server" Width="50px" CausesValidation="false" CssClass="combutton" Text="Cancel" onclick="cmdCancel_Click" />
        </td>
    </tr>
</table>

</asp:Content>