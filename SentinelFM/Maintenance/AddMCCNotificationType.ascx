<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddMCCNotificationType.ascx.cs" Inherits="Maintenance_AddMCCNotificationType" %>
<table>
   <tr>
      <td  style="width:120px">
         <asp:Label id="lblOperationType" Font-Bold="True" runat="server" CssClass="formtext" Text = "Operation Type" meta:resourcekey="ddlAddMccNotificationlblOperationType" ></asp:Label>
      </td>
      <td colspan="5">
         <asp:DropDownList ID="ddlAddMccNotificationOperationTypes" runat="server" CssClass="formtext" DataTextField="description" DataValueField="id"   meta:resourcekey="ddlAddMccNotificationOperationTypes" Width="180px" />                                               
      </td>
   </tr>


   <tr>
      <td>
         <asp:Label id="lblNotification1" Font-Bold="True" runat="server" CssClass="formtext" Text = "Notification1" meta:resourcekey="AddMccNotificationNotification1" ></asp:Label>
      </td>
      <td>
           <telerik:RadNumericTextBox  IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" CssClass="formtext"
            ID="txtNotification1" Width="50px"
            meta:resourcekey="AddMccNotificationtxtNotification1" Culture="en-CA" >
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
            <NumberFormat DecimalDigits="0"></NumberFormat> 
        </telerik:RadNumericTextBox>
      </td>

      <td>
         <asp:Label id="lblNotification2" Font-Bold="True" runat="server" CssClass="formtext" Text = "Notification2" meta:resourcekey="AddMccNotificationNotification2" ></asp:Label>
      </td>
      <td>
           <telerik:RadNumericTextBox  IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" CssClass="formtext"
            ID="txtNotification2" Width="50px"
            meta:resourcekey="AddMccNotificationtxtNotification2" Culture="en-CA" >
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
            <NumberFormat DecimalDigits="0"></NumberFormat> 
        </telerik:RadNumericTextBox>
      </td>

      <td>
         <asp:Label id="lblNotification3" Font-Bold="True" runat="server" CssClass="formtext" Text = "Notification3" meta:resourcekey="AddMccNotificationNotification3" ></asp:Label>
      </td>
      <td>
           <telerik:RadNumericTextBox  IncrementSettings-InterceptArrowKeys="true"
            IncrementSettings-InterceptMouseWheel="true" runat="server" CssClass="formtext"
            ID="txtNotification3" Width="50px"
            meta:resourcekey="AddMccNotificationtxtNotification3" Culture="en-CA" >
            <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
            <NumberFormat DecimalDigits="0"></NumberFormat> 
        </telerik:RadNumericTextBox>
      </td>

   </tr>
   <tr valign="top">
      <td>
         <asp:Label id="lblDescription" Font-Bold="True" runat="server" CssClass="formtext" Text = "Description" meta:resourcekey="AddMccNotificationlblDescription" ></asp:Label>
      </td>
      <td colspan="5">
         <asp:TextBox id="txtDescription" runat="server" CssClass="formtext" 
              meta:resourcekey="AddMccNotificationlblDescription" MaxLength="50" Width="450px" ></asp:TextBox>
         <span style="color: Red">*</span><br />
          <asp:RequiredFieldValidator ID="valReqtxtDescription" runat="server" ValidationGroup="valMccNotificationAdd"
                                                                            ControlToValidate="txtDescription" meta:resourcekey="valReqAddMccNotificationtxtDescription"
                                                                            Text="Please enter description" Display="Dynamic"></asp:RequiredFieldValidator>
      </td>
   </tr>

   <tr>
      <td colspan="6" >
       <nobr>
      <asp:Button ID="btnSave"  CssClass="combutton" runat ="server" Text = "Save" meta:resourcekey="btnSaveAddMccNotificationResource1" Width="80px" ValidationGroup="valMccNotificationAdd" />
      &nbsp;
      <asp:Button ID="btnCancel" CssClass="combutton" runat ="server" Text = "Cancel" meta:resourcekey="BtnCancelAddMccNotificationResource1" Width="80px"  />
      </nobr>
      </td>
   </tr>
</table>