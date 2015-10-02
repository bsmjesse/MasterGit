<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddEmail.ascx.cs" Inherits="Configuration_CustomerReport_AddEmail" %>
<table width="100%">
    <tr>
        <td colspan="2">
            <asp:Label ID="lblEmail" runat="server" Text="Email: (Please seperate multiple emails by semicolon)"
                meta:resourcekey="lblEmailResource1"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox ID="txtEmails" runat="server" Width="700px" Height="60px" TextMode="MultiLine" MaxLength="4010"></asp:TextBox>
            <asp:CustomValidator ID="cvtxtEmails" runat="server" ControlToValidate="txtEmails"
                ValidationGroup="AddEmail" Display="Dynamic" Text="*" ErrorMessage="Invalid emaill address or emails are not seperated by semicolon."
                ClientValidationFunction="CustomValidateEmail" CssClass="errortext"></asp:CustomValidator>
            <asp:RequiredFieldValidator ID="rvtxtEmails"  runat="server" ControlToValidate="txtEmails"
 ValidationGroup="AddEmail" Display="Dynamic" Text="*" ErrorMessage="Email is required."
                 CssClass="errortext"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td colspan="2">
                <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px"
                                            meta:resourcekey="cboVehicleResource1" MaxHeight="200px" DataTextField="FleetName"
                                            DataValueField="FleetId" EmptyMessage="Select Fleet(s)" AllowCustomText="true"
                                            Skin="Hay">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkFleet" Checked="false" />
                                                <asp:Label runat="server" ID="lblFleet" Text='<%# Eval("FleetName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </telerik:RadComboBox>
            <asp:CustomValidator ID="cvcboFleet" runat="server"   ControlToValidate="txtEmails" Text="*" 
                ValidationGroup="AddEmail" Display="Dynamic"  ErrorMessage="Select a Fleet."
                ClientValidationFunction="CustomValidateFleet" CssClass="errortext"></asp:CustomValidator>

        </td>
    </tr>
    <tr>
        <td colspan="2">
            <nobr>
          <asp:Button ID="btnSave"  CssClass="combutton" runat ="server" Text = "Save" meta:resourcekey="btnSaveEmailResource1" Width="80px"  ValidationGroup="AddEmail"  />
           &nbsp;
          <asp:Button ID="btnCancel" CssClass="combutton" runat ="server" Text = "Cancel" meta:resourcekey="BtnCancelEmailResource1" Width="80px"  />
          </nobr>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="left">
                <table>
                    <tr>
                        <td align="left">
                            <!--for chrome browser -->
                            <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="AddEmail" DisplayMode="BulletList" />
                        </td>
                    </tr>
                </table>
        </td>
    </tr>
</table>

