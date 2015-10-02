<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditFactors.ascx.cs" Inherits="SentinelFM.Configuration_Equipment_EditFactors" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table>
  <tr align="left">
     <td>
        <asp:Label id="lblDescription" runat="Server" Text = "Description"  
             meta:resourcekey="lblDescriptionResource1"  Font-Bold="True" ></asp:Label>
     </td>
     <td>
         <asp:TextBox ID="txtDescription" runat="server" MaxLength="50" Width="300px" meta:resourcekey="txtDescriptionResource1"
                ></asp:TextBox>
         <span style="color: Red">*</span><br />
         <asp:RequiredFieldValidator ID="valReqtxtDescription" runat="server" ValidationGroup="valGPAdd"
                ControlToValidate="txtDescription" meta:resourcekey="valReqtxtDescriptionResource1"
                Text="Please enter description" Display="Dynamic"></asp:RequiredFieldValidator>

     </td>
  </tr>
  <tr align="left">
     <td>
        <asp:Label id="lblTypeName" runat="Server" Text = "Media Type"  
             meta:resourcekey="lblTypeNameResource1"  Font-Bold="True"></asp:Label>
     </td>
     <td>
        <telerik:RadComboBox ID="combTypeName" runat="server" 
             DataTextField="TypeName" DataValueField="MediaTypeId"
            Width="200px" meta:resourcekey="combTypeNameResource1" AutoPostBack="True" 
             onselectedindexchanged="combTypeName_SelectedIndexChanged">
        </telerik:RadComboBox>
        <span style="color: Red">*</span><br />
        <asp:RequiredFieldValidator ID="valReqcombTypeName" runat="server" ValidationGroup="valGPAdd"
            ControlToValidate="combTypeName" meta:resourcekey="valReqcombTypeNameResource1"
            Text="Please select a media type" Display="Dynamic"></asp:RequiredFieldValidator>
     </td>
  </tr>
  <tr>
     <td>
        <asp:Label id="lblMeasureUnit" runat="Server" Text = "Units of measurement"  
             meta:resourcekey="lblMeasureUnitResource1"  Font-Bold="True"></asp:Label>
     </td>
     <td>
        <telerik:RadComboBox ID="combMeasureUnit" runat="server" 
             DataTextField="UnitOfMeasureAcr" DataValueField="BaseUnit"
            Width="200px" meta:resourcekey="combMeasureUnit"  
             >
        </telerik:RadComboBox>
        <span style="color: Red">*</span>
     </td>
  </tr>
  <tr  align="left">
    <td>
        <asp:Label ID="lblFactorName1" CssClass="formtext" runat="server"  
            Visible ="False" Font-Bold="True"
            meta:resourcekey="lblFactorName1Resource1"></asp:Label>
    </td>
    <td>
        <telerik:RadNumericTextBox  IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" 
            ID="txtFactor1" Width="100px"
            meta:resourcekey="txtFactor1Resource1" Visible ="False" Culture="en-CA" >
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
             <NumberFormat DecimalDigits="3"></NumberFormat> 
        </telerik:RadNumericTextBox>
    </td>
  </tr>
  <tr  align="left">
    <td>
        <asp:Label ID="lblFactorName2" CssClass="formtext" runat="server" 
            Visible ="False" Font-Bold="True"
            meta:resourcekey="lblFactorName2Resource1"></asp:Label>
    </td>
    <td>
        <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" 
            ID="txtFactor2" Width="100px"
            meta:resourcekey="txtFactor2Resource1" Visible ="False" Culture="en-CA">
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
             <NumberFormat DecimalDigits="3"></NumberFormat> 

        </telerik:RadNumericTextBox>
    </td>
  </tr>
  <tr  align="left">
    <td>
        <asp:Label ID="lblFactorName3" CssClass="formtext" runat="server" 
            Visible ="False" Font-Bold="True"
            meta:resourcekey="lblFactorName3Resource1"></asp:Label>
    </td>
    <td>
        <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" 
            ID="txtFactor3" Width="100px"
            meta:resourcekey="txtFactor3Resource1" Visible ="False" Culture="en-CA">
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
             <NumberFormat DecimalDigits="3"></NumberFormat> 

        </telerik:RadNumericTextBox>
    </td>
  </tr>
  <tr  align="left">
    <td>
        <asp:Label ID="lblFactorName4" CssClass="formtext" runat="server" 
            Visible ="False" Font-Bold="True"
            meta:resourcekey="lblFactorName4Resource1"></asp:Label>
    </td>
    <td>
        <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" 
            ID="txtFactor4" Width="100px"
            meta:resourcekey="txtFactor4Resource1" Visible ="False" Culture="en-CA">
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
             <NumberFormat DecimalDigits="3"></NumberFormat> 

        </telerik:RadNumericTextBox>
    </td>
  </tr>
  <tr  align="left">
    <td>
        <asp:Label ID="lblFactorName5" CssClass="formtext" runat="server" 
            Visible ="False" Font-Bold="True"
            meta:resourcekey="lblFactorName5Resource1"></asp:Label>
    </td>
    <td>
        <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" 
            ID="txtFactor5" Width="100px"
            meta:resourcekey="txtFactor5Resource1" Visible ="False" Culture="en-CA">
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