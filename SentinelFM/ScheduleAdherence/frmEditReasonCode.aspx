<%@ Page Title="" Language="C#" MasterPageFile="~/ScheduleAdherence/SAMasterPage.master" AutoEventWireup="true" CodeFile="frmEditReasonCode.aspx.cs" Inherits="ScheduleAdherence_frmEditReasonCode" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField id="hdAction" runat="server" />
    <asp:HiddenField ID="hdReasonCodeId" runat="server" />
<table width="100%" class="formtext">
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext" ></asp:ValidationSummary>
        </td>
    </tr>
    <tr style="height:20px">
        <td style="width:100px">
            Reason Code:
            <asp:RequiredFieldValidator ID="valReasonCode" runat="server" ControlToValidate="txtReasonCode"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtReasonCode" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr>
        <td>Description:
            <asp:RequiredFieldValidator ID="valDescription" runat="server" ControlToValidate="txtDescription"
                ErrorMessage="Please enter a description" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtDescription" runat="server"  CssClass="formtext"></asp:textbox></td>
    </tr>
    <tr style="height:50px">
        <td colspan="2" align="center">
            <asp:button id="cmdSave" runat="server" Width="50px" CssClass="combutton" Text="Save" onclick="cmdSave_Click" />
            <asp:button id="cmdCancel" runat="server" Width="50px" CausesValidation="false" CssClass="combutton" Text="Cancel" onclick="cmdCancel_Click" />
        </td>
    </tr>
</table>
</asp:Content>

