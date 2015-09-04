<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMdtOTAForms.aspx.cs" Inherits="SentinelFM.Admin.frmMdtOTAForms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>MDT OTA</title>
    <LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="border-right: gray 4px double;
            border-top: gray 4px double; border-left: gray 4px double; width: 709px; border-bottom: gray 4px double;
            height: 163px">
            <tr>
                <td align="center" style="width: 632px" valign="top">
                    <table id="Table6" border="0" cellpadding="0" cellspacing="0" width="541">
                        <tr>
                            <td align="center" style="height: 153px" valign="middle">
                                <table id="Table1" border="0" cellpadding="0" cellspacing="0" class="formtext" style="width: 288px;
                                    height: 130px" width="288">
                                    <tr>
                                        <td style="height: 12px">
                                        </td>
                                        <td style="height: 12px">
                                        </td>
                                        <td style="height: 12px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="5">
                                        </td>
                                        <td height="5">
                                            <asp:Label ID="lblOrganization" runat="server">Organization:</asp:Label></td>
                                        <td height="5">
                                            <asp:DropDownList ID="cboOrganization" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                DataTextField="OrganizationName" DataValueField="OrganizationId" Enabled="False"
                                                OnSelectedIndexChanged="cboOrganization_SelectedIndexChanged" Width="203px">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 21px">
                                        </td>
                                        <td style="height: 21px">
                                            <asp:Label ID="lblFleets" runat="server">Fleet:</asp:Label></td>
                                        <td style="height: 21px">
                                            <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText"
                                                DataTextField="FleetName" DataValueField="FleetId" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                                Width="203px">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 1px">
                                        </td>
                                        <td style="height: 1px">
                                            <asp:Label ID="lblVehicles" runat="server" Visible="False">Vehicles:</asp:Label></td>
                                        <td style="height: 1px">
                                            <asp:DropDownList ID="cboVehicle" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                DataTextField="Description" DataValueField="VehicleId" 
                                                Width="203px" Visible="False">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td height="10">
                                        </td>
                                        <td height="10">
                                        </td>
                                        <td height="10">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td align="right">
                                            <asp:Button ID="cmdView" runat="server" CssClass="combutton" OnClick="cmdView_Click"
                                                Text="View" /></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" style="width: 632px" valign="top">
                    <table id="tblFW" runat="server" border="0" cellpadding="0" cellspacing="0" style="width: 605px" visible="true">
                        <tr>
                            <td style="width: 533px; height: 546px">
                                <table id="Table3" border="0" cellpadding="0" cellspacing="0" style="width: 606px">
                                    <tr>
                                        <td align="center" colspan="2" height="76" style="width: 587px; height: 76px" valign="middle">
                                            <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="width: 103%;
                                                height: 89px">
                                                <tr>
                                                    <td align="center" colspan="2" valign="middle">
                                                        <table id="Table5" border="0" cellpadding="0" cellspacing="0" style="width: 100%;
                                                            height: 40px" width="468">
                                                            <tr>
                                                                <td align="center" class="formtext" style="width: 100%">
                                                                    &nbsp; &nbsp;&nbsp;
                                                                    <table>
                                                                        <tr>
                                                                            <td style="width: 100px">
                                                                                <asp:Button ID="cmdUnselectAllSensors" runat="server" CssClass="combutton" OnClick="cmdUnselectAllSensors_Click"
                                                                                    Text="Deselect All" Width="92px" /></td>
                                                                            <td style="width: 100px">
                                                                                <asp:Button ID="cmdSetAllSensors" runat="server" CssClass="combutton" OnClick="cmdSetAllSensors_Click"
                                                                                    Text="Select All" Width="92px" /></td>
                                                                            <td style="width: 100px">
                                                                                <asp:Button ID="cmdRefreshBoxFirmware" runat="server" CssClass="combutton" OnClick="cmdRefreshBoxFirmware_Click"
                                                                                    Text="Refresh  Form Status" Width="156px" /></td>
                                                                            <td style="width: 100px">
                                                                                <asp:Button ID="cmdRebootMDT" runat="server" CssClass="combutton" OnClick="cmdRebootMDT_Click"
                                                                                    Text="Reboot MDT" Width="156px" /></td>
                                                                        </tr>
                                                                    </table>
                                                                                <asp:Button ID="cmdGetBoxFirmware" runat="server" CssClass="combutton" OnClick="cmdGetBoxFirmware_Click"
                                                                                    Text="Get Box Firmware" Width="126px" Visible="False" /></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2" style="width: 587px; height: 370px" valign="top">
                                            <asp:DataGrid ID="dgData" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                                DataKeyField="BoxID" ForeColor="#333333" GridLines="None" 
                                                PageSize="9999" OnCancelCommand="dgData_CancelCommand" OnEditCommand="dgData_EditCommand" OnUpdateCommand="dgData_UpdateCommand">
                                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                                                <ItemStyle BackColor="#F7F6F3" Font-Size="11px" ForeColor="#333333" HorizontalAlign="Center"
                                                    Wrap="False" />
                                                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="11px" ForeColor="White"
                                                    HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
                                                <Columns>
                                                    <asp:BoundColumn DataField="BoxId" HeaderText="BoxId" Visible="False"></asp:BoundColumn>
                                                    <asp:TemplateColumn HeaderText="Update">
                                                        <HeaderStyle Width="20px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkBox" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBox") %>'
                                                                Enabled="True" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="BoxId" HeaderText="BoxId" ReadOnly="True"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="MdtDescription" HeaderText="Description " ReadOnly="True"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="SerialNum" HeaderText="Serial Number " ReadOnly="True"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="VehicleDescription" HeaderText="Vehicle " ReadOnly="True"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="LastCommunicated" HeaderText="Date " ReadOnly="True"></asp:BoundColumn>
                                                    
                                                                     
                                                                     
                                                     <asp:TemplateColumn HeaderText="Title">
                                                        <HeaderStyle Width="20px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:Label  ID="lblTitle" runat="server"   Text='<%# DataBinder.Eval(Container.DataItem, "Title")   %>' ></asp:Label>
                                                        </ItemTemplate>
                                                       <EditItemTemplate>
                                                            <asp:TextBox ID="txtTitle" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>' />
                                                       </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                                     
                                                                     
                                                                     
                                                                     
                                                                                                  
                                                    
                                                     <asp:TemplateColumn HeaderText="Clear memory">
                                                        <HeaderStyle Width="20px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkWipe1"  runat="server"   Checked='<%# DataBinder.Eval(Container.DataItem, "chkWipe") %>' />
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="chkWipe" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "chkWipe") %>'
                                                                Enabled="True" />
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    
                                                    
                                                    
                                                    <asp:TemplateColumn HeaderText="Daylight saving">
                                                        <HeaderStyle Width="20px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkDLS1" runat="server"   Checked='<%# DataBinder.Eval(Container.DataItem, "chkDLS") %>' />
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="chkDLS" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "chkDLS") %>'
                                                                Enabled="True" />
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    
                                                    
                                                    
                                                     <asp:TemplateColumn HeaderText="Blank in motion">
                                                        <HeaderStyle Width="20px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkBIM1" runat="server"   Checked='<%# DataBinder.Eval(Container.DataItem, "chkBIM") %>' />
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="chkBIM" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBIM") %>'
                                                                Enabled="True" />
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    
                                                    
                                                    <asp:TemplateColumn HeaderText="Overite MDT Settings">
                                                        <HeaderStyle Width="20px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkOVR1" runat="server"   Checked='<%# DataBinder.Eval(Container.DataItem, "chkOVR") %>' />
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="chkOVR" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "chkOVR") %>'
                                                                Enabled="True" />
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    
                                                    
                                                    <asp:BoundColumn DataField="OtaStatus" HeaderText="Status "
                                                        ReadOnly="True"></asp:BoundColumn>
                                                    
                                                    <asp:EditCommandColumn CancelText="&lt;img src=images/cancel.gif border=0&gt;" EditText="&lt;img src=images/edit.gif border=0&gt;"
                                                        UpdateText="&lt;img src=images/ok.gif border=0&gt;"></asp:EditCommandColumn>
                                                </Columns>
                                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Right" Mode="NumericPages" />
                                            </asp:DataGrid></td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2" height="23" style="width: 587px; height: 23px" valign="middle">
                                            <asp:Button ID="cmdUpdate" runat="server" CssClass="combutton" OnClick="cmdUpdate_Click"
                                                Text="Update" Width="151px" />&nbsp;
                                            </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2" height="40" style="width: 587px" valign="middle">
                                            <asp:Label ID="lblMessage" runat="server" CssClass="regulartext" ForeColor="Red"></asp:Label></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
