<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddMCCMaintenance.ascx.cs" Inherits="Maintenance_AddMCCMaintenance" %>
<table>
   <tr>
      <td  style="width:120px">
         <asp:Label id="lblOperationType" Font-Bold="True" runat="server" CssClass="formtext" Text = "Operation Type" meta:resourcekey="ddlMCCMaintenancelblOperationType" ></asp:Label>
      </td>
      <td>
         <asp:DropDownList ID="ddlMCCMaintenanceOperationTypes" runat="server" 
              CssClass="formtext" DataTextField="description" DataValueField="id"   
              meta:resourcekey="ddlMCCMaintenanceOperationTypes" Width="180px" 
              AutoPostBack="True" 
              
              onselectedindexchanged="ddlMCCMaintenanceOperationTypes_SelectedIndexChanged"  />  
      </td>
   </tr>

   <tr>
      <td>
         <asp:Label id="lblNotificationType" Font-Bold="True" runat="server" CssClass="formtext" Text = "Notification Type" meta:resourcekey="ddlMCCMaintenancelblNotificationType" ></asp:Label>
      </td>
      <td>
         <asp:DropDownList ID="ddlNotificationType" runat="server" CssClass="formtext" AppendDataBoundItems="True"
              DataTextField="description" DataValueField="NotificationID"   
              meta:resourcekey="ddlMCCMaintenancelNotificationType" Width="420px" 
               > 

         </asp:DropDownList>
      </td>
   </tr>
   <tr>
      <td>
         <asp:Label id="lblFrequencyID" Font-Bold="True" runat="server" CssClass="formtext" Text = "Frequency ID" meta:resourcekey="MCCMaintenancelblFrequencyID" ></asp:Label>
      </td>
      <td>
           <asp:DropDownList ID="ddlFrequencyID" runat="server" Width = "150px" 
               DataTextField="FrequencyName" DataValueField="FrequencyID" CssClass="formtext" 
               meta:resourcekey="ddlFrequencyIDResource1" ></asp:DropDownList>

      </td>
   </tr>

   <tr>
      <td>
         <asp:Label id="lblInterval" Font-Bold="True" runat="server" CssClass="formtext" Text = "Interval" meta:resourcekey="MCCMaintenancelblInterval" ></asp:Label>
      </td>
      <td>
            <table>
            <tr>
                <td>
                       <telerik:RadNumericTextBox  IncrementSettings-InterceptArrowKeys="true"
                    IncrementSettings-InterceptMouseWheel="true" runat="server" CssClass="formtext"
                    ID="txtInterval" Width="100px" MinValue=1
                    meta:resourcekey="MCCMaintenancetxtlInterval" Culture="en-CA" >
                    <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
                    <NumberFormat DecimalDigits="0"></NumberFormat> 
                   </telerik:RadNumericTextBox>
                   <asp:DropDownList ID= "ddlInterval" runat="server" Width="150px" Visible="False" 
                           DataTextField="Name" DataValueField="TimespanId" CssClass="formtext" 
                           AutoPostBack="True" 
                           onselectedindexchanged="ddlInterval_SelectedIndexChanged" 
                           meta:resourcekey="ddlIntervalResource1" ></asp:DropDownList>
               </td>
               <td>
                  <asp:CheckBox ID="chkFixedInterval" runat="server" Text ="Fixed Interval" 
                       meta:resourcekey="chkFixedIntervalResource1"  />
               </td>
               <td>
                   <asp:Panel ID="pnlDate" runat="server" Visible="False" 
                       meta:resourcekey="pnlDateResource1" >
                       <table>
                       <tr>
                        <td>
                           <asp:CheckBox ID="chkFixedDate" runat="server" Text ="Fixed Service Date" 
                                oncheckedchanged="chkFixedDate_CheckedChanged" AutoPostBack="True" 
                                meta:resourcekey="chkFixedDateResource1" />
                        </td>
                        <td>
                           <asp:Panel ID="pnlMonthDay" runat="server" Visible="False" 
                                meta:resourcekey="pnlMonthDayResource1" >
                              <table>
                                <tr>
                                    <td>
                                       <asp:Label ID="lblMonth" runat="server" Text="Month:" 
                                            meta:resourcekey="lblMonthResource1"  ></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlMonth" runat="server" CssClass="formtext" 
                                            meta:resourcekey="ddlMonthResource1"  >
                                           <asp:ListItem Text="Jan" Value ="1" Selected="True" 
                                                meta:resourcekey="ListItemResource1" ></asp:ListItem>
                                           <asp:ListItem Text="Feb" Value ="2" meta:resourcekey="ListItemResource2" ></asp:ListItem>
                                           <asp:ListItem Text="Mar" Value ="3" meta:resourcekey="ListItemResource3" ></asp:ListItem>
                                           <asp:ListItem Text="Apr" Value ="4" meta:resourcekey="ListItemResource4" ></asp:ListItem>
                                           <asp:ListItem Text="May" Value ="5" meta:resourcekey="ListItemResource5" ></asp:ListItem>
                                           <asp:ListItem Text="Jun" Value ="6" meta:resourcekey="ListItemResource6" ></asp:ListItem>
                                           <asp:ListItem Text="Jul" Value ="7" meta:resourcekey="ListItemResource7" ></asp:ListItem>
                                           <asp:ListItem Text="Aug" Value ="8" meta:resourcekey="ListItemResource8" ></asp:ListItem>
                                           <asp:ListItem Text="Sep" Value ="9" meta:resourcekey="ListItemResource9" ></asp:ListItem>
                                           <asp:ListItem Text="Oct" Value ="10" meta:resourcekey="ListItemResource10" ></asp:ListItem>
                                           <asp:ListItem Text="Nov" Value ="11" meta:resourcekey="ListItemResource11" ></asp:ListItem>
                                           <asp:ListItem Text="Dec" Value ="12" meta:resourcekey="ListItemResource12" ></asp:ListItem>
                                           <asp:ListItem Text="None" Value ="None" meta:resourcekey="ListItemResource13" ></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblDay" runat="server" Text="Day:" 
                                            meta:resourcekey="lblDayResource1" ></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlDay" runat="server" CssClass="formtext" 
                                            meta:resourcekey="ddlDayResource1"  >
                                           <asp:ListItem Text="1" Value ="1" Selected="True" 
                                                meta:resourcekey="ListItemResource14" ></asp:ListItem>
                                           <asp:ListItem Text="2" Value ="2" meta:resourcekey="ListItemResource15" ></asp:ListItem>
                                           <asp:ListItem Text="3" Value ="3" meta:resourcekey="ListItemResource16" ></asp:ListItem>
                                           <asp:ListItem Text="4" Value ="4" meta:resourcekey="ListItemResource17" ></asp:ListItem>
                                           <asp:ListItem Text="5" Value ="5" meta:resourcekey="ListItemResource18" ></asp:ListItem>
                                           <asp:ListItem Text="6" Value ="6" meta:resourcekey="ListItemResource19" ></asp:ListItem>
                                           <asp:ListItem Text="7" Value ="7" meta:resourcekey="ListItemResource20" ></asp:ListItem>
                                           <asp:ListItem Text="8" Value ="8" meta:resourcekey="ListItemResource21" ></asp:ListItem>
                                           <asp:ListItem Text="9" Value ="9" meta:resourcekey="ListItemResource22" ></asp:ListItem>
                                           <asp:ListItem Text="10" Value ="10" meta:resourcekey="ListItemResource23" ></asp:ListItem>
                                           <asp:ListItem Text="11" Value ="11" meta:resourcekey="ListItemResource24" ></asp:ListItem>
                                           <asp:ListItem Text="12" Value ="12" meta:resourcekey="ListItemResource25" ></asp:ListItem>
                                           <asp:ListItem Text="13" Value ="13" meta:resourcekey="ListItemResource26" ></asp:ListItem>
                                           <asp:ListItem Text="14" Value ="14" meta:resourcekey="ListItemResource27" ></asp:ListItem>
                                           <asp:ListItem Text="15" Value ="15" meta:resourcekey="ListItemResource28" ></asp:ListItem>
                                           <asp:ListItem Text="16" Value ="16" meta:resourcekey="ListItemResource29" ></asp:ListItem>
                                           <asp:ListItem Text="17" Value ="17" meta:resourcekey="ListItemResource30" ></asp:ListItem>
                                           <asp:ListItem Text="18" Value ="18" meta:resourcekey="ListItemResource31" ></asp:ListItem>
                                           <asp:ListItem Text="19" Value ="19" meta:resourcekey="ListItemResource32" ></asp:ListItem>
                                           <asp:ListItem Text="20" Value ="20" meta:resourcekey="ListItemResource33" ></asp:ListItem>
                                           <asp:ListItem Text="21" Value ="21" meta:resourcekey="ListItemResource34" ></asp:ListItem>
                                           <asp:ListItem Text="22" Value ="22" meta:resourcekey="ListItemResource35" ></asp:ListItem>
                                           <asp:ListItem Text="23" Value ="23" meta:resourcekey="ListItemResource36" ></asp:ListItem>
                                           <asp:ListItem Text="24" Value ="24" meta:resourcekey="ListItemResource37" ></asp:ListItem>
                                           <asp:ListItem Text="25" Value ="25" meta:resourcekey="ListItemResource38" ></asp:ListItem>
                                           <asp:ListItem Text="26" Value ="26" meta:resourcekey="ListItemResource39" ></asp:ListItem>
                                           <asp:ListItem Text="27" Value ="27" meta:resourcekey="ListItemResource40" ></asp:ListItem>
                                           <asp:ListItem Text="28" Value ="28" meta:resourcekey="ListItemResource41" ></asp:ListItem>
                                           <asp:ListItem Text="29" Value ="29" meta:resourcekey="ListItemResource42" ></asp:ListItem>
                                           <asp:ListItem Text="30" Value ="30" meta:resourcekey="ListItemResource43" ></asp:ListItem>
                                           <asp:ListItem Text="31" Value ="31" meta:resourcekey="ListItemResource44" ></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>

                                </tr>
                              </table>
                           </asp:Panel>
                        </td>
                    </tr>
                   </table>
                   </asp:Panel>
               </td>
           </tr>
           </table>
      </td>
   </tr>
   <tr valign="top">
      <td>
         <asp:Label id="lblDescription" Font-Bold="True" runat="server" CssClass="formtext" Text = "Description" meta:resourcekey="MCCMaintenancelblDescription" ></asp:Label>
      </td>
      <td>
         <asp:TextBox id="txtDescription" runat="server" CssClass="formtext" 
              meta:resourcekey="MCCMaintenancelblDescription" MaxLength="50" Width="450px" ></asp:TextBox>
         <span style="color: Red">*</span><br />
          <asp:RequiredFieldValidator ID="valReqtxtDescription" runat="server" ValidationGroup="valMccMaintenanceAdd"
                                                                            ControlToValidate="txtDescription" meta:resourcekey="valReqAddMcccMaintenancetxtDescription"
                                                                            Text="Please enter description" Display="Dynamic"></asp:RequiredFieldValidator>

      </td>
   </tr>

   <tr>
      <td colspan="2" >
       <nobr>
      <asp:Button ID="btnSave"  CssClass="combutton" runat ="server" Text = "Save" meta:resourcekey="btnSaveMCCMaintenanceResource1" Width="80px"  ValidationGroup="valMccMaintenanceAdd"  />
      &nbsp;
      <asp:Button ID="btnCancel" CssClass="combutton" runat ="server" Text = "Cancel" meta:resourcekey="BtnCancelMCCMaintenanceResource1" Width="80px"  />
      <asp:Button ID="btnSaveTmp"  runat ="server"  Width="0px" Height="0px" 
              style="display:none; border:0px;" meta:resourcekey="btnSaveTmpResource1"  />
      </nobr>
      </td>
   </tr>
</table>
