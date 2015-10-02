<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditFuelCategory.ascx.cs" Inherits="SentinelFM.Configuration_EditFuelCategory" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<style type="text/css">
    .style1
    {
        width: 455px;
    }
</style>
<table>
  <tr align="left" valign ="top">
     <td>
        <asp:Label id="lblFuelType" runat="Server" Text = "Fuel Type"  
             meta:resourcekey="lblFuelTypeResource1"  Font-Bold="True" ></asp:Label>
     </td>
     <td class="style1">
         <asp:TextBox ID="txtFuelType" runat="server" MaxLength="50" Width="300px" meta:resourcekey="txtFuelTypeResource1"
                ></asp:TextBox>
         <span style="color: Red">*</span><br />
         <asp:RequiredFieldValidator ID="valReqtxtFuelType" runat="server" ValidationGroup="valGPAdd"
                ControlToValidate="txtFuelType" meta:resourcekey="valReqtxtFuelTypeResource1" CssClass="errortext"
                Text="Please enter fuel type." Display="Dynamic"></asp:RequiredFieldValidator>

     </td>
  </tr>
  <tr align="left" valign ="top">
     <td>
        <asp:Label id="lblGHGCategory" runat="Server" Text = "GHG Category"  
             meta:resourcekey="lblGHGCategoryResource1"  Font-Bold="True"></asp:Label>
     </td>
     <td class="style1">
         <asp:TextBox ID="txtGHGCategory" runat="server" MaxLength="50" Width="300px" meta:resourcekey="txtGHGCategoryResource1"
                ></asp:TextBox>
         <span style="color: Red">*</span><br />
         <asp:RequiredFieldValidator ID="valReqtxtGHGCategory" runat="server" ValidationGroup="valGPAdd"
                ControlToValidate="txtGHGCategory" meta:resourcekey="valReqtxtGHGCategoryResource1" CssClass="errortext"
                Text="Please enter GHG category." Display="Dynamic"></asp:RequiredFieldValidator>
     </td>
  </tr>
  <tr valign ="top">
     <td>
        <asp:Label id="lblGHGCategoryDesc" runat="Server" Text = "Description"  
             meta:resourcekey="lblGHGCategoryDescResource1"  Font-Bold="True"></asp:Label>
     </td>
     <td class="style1">
         <asp:TextBox ID="txtGHGCategoryDesc" runat="server" MaxLength="250" 
             Width="421px" meta:resourcekey="txtGHGCategoryDescResource1"
                ></asp:TextBox>
     </td>
  </tr>
  <tr  align="left" valign ="top">
    <td>
        <asp:Label ID="lblCO2Factor" CssClass="formtext" runat="server"  
            Visible ="true" Font-Bold="True" Text = "CO2 Factor"
            meta:resourcekey="lblCO2FactorResource1"></asp:Label>
    </td>
    <td class="style1">
        <telerik:RadNumericTextBox  IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" 
            ID="txtCO2Factor" Width="100px" Value = "0"
            meta:resourcekey="txtCO2FactorResource1" Visible ="true" Culture="en-CA" >
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
             <NumberFormat DecimalDigits="3"></NumberFormat> 
        </telerik:RadNumericTextBox>
    </td>
  </tr>

  <tr>
    <td colspan = "2" >
      <asp:Button ID="btnSave"  CssClass="combutton" runat ="server" Text = "Save" meta:resourcekey="btnSaveResource1" ValidationGroup="valGPAdd" />
      <asp:Button ID="BtnCancel" CssClass="combutton" runat ="server" Text = "Cancel Edit" meta:resourcekey="BtnCancelResource1" />
    </td>
  </tr>
</table>