<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSForms.aspx.cs" Inherits="frmHOSForms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HOS Dynamic Form</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    <asp:Panel ID="pnlForms" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnNewForm" runat="server" Text="New Form" OnClick="btnNewForm_Click"
                                        Width="100px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblFormError" runat="server" ForeColor="Red"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DataGrid ID="dvForms" runat="server" PageSize="9999" AllowPaging="false" DataKeyField="FormId"
                                        AutoGenerateColumns="False" CellPadding="3" BorderColor="Black" BorderStyle="Ridge"
                                        GridLines="Both" BorderWidth="2px" BackColor="White" CellSpacing="1" OnItemCommand="dvForms_ItemCommand">
                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                        <HeaderStyle ForeColor="Black" BackColor="#C6C3C6"></HeaderStyle>
                                        <Columns>
                                            <asp:TemplateColumn HeaderText='Form Name'>
                                                <ItemStyle Width="300px" />
                                                <HeaderStyle Width="300px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblMsg" Text='<%# DataBinder.Eval(Container.DataItem,"FormName") %>'
                                                        runat="server" Width="400">
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtFleetName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"FormName") %>'
                                                        Width="200px" meta:resourcekey="txtFleetNameResource1" MaxLength = "100" ></asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn>
                                                <ItemStyle Width="90px" />
                                                <HeaderStyle Width="90px" />
                                                <ItemTemplate>
                                                    <asp:Button ID="btnEdit" runat="server" CommandName="EditForm" Text="Edit" />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn>
                                                <ItemStyle Width="90px" />
                                                <HeaderStyle Width="90px" />
                                                <ItemTemplate>
                                                    <asp:Button ID="btnAssignment" runat="server" CommandName="FormAssignment" Text="Assignment" />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>

                                            <asp:TemplateColumn>
                                                <ItemStyle Width="90px" />
                                                <HeaderStyle Width="90px" />
                                                <ItemTemplate>
                                                    <asp:Button ID="btnDelete" runat="server" OnClientClick="return confirm('Are you sure you want to delete?')"
                                                        CommandName="DeleteForm" Text="Delete" />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                        </Columns>
                                        <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                            Mode="NumericPages"></PagerStyle>
                                    </asp:DataGrid>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlNewForm" runat="server" Visible="false">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnSaveForm" runat="server" Text="Save" Width="100px" OnClick="btnSaveForm_Click" />
                                    <asp:Button ID="btnBackFrom" runat="server" Text="Back" OnClick="btnBackFrom_Click"
                                        Width="100px" />
                                    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblFormName"> Form Name: </asp:Label>
                                    <asp:TextBox runat="server" ID="txtFormName" Width="356px" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <font color="blue">Note: 1: Please add element if you select dropdown list, Combo, Radio
                                        Button.<br>
                                        &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2: ID should be keep unique;
                                    <br />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 3: Please donot input &#39;@@@@&#39;<br />&nbsp;&nbsp;
                                    </font>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnNewLine" runat="server" Text="Add Item" OnClick="btnNewLine_Click"
                                        Width="100px" />
                                </td>
                            </tr>
                            <tr>
                                <asp:DataGrid ID="dvForm" runat="server" PageSize="9999" AllowPaging="false" AutoGenerateColumns="False"
                                    CellPadding="3" BorderColor="Black" BorderStyle="Ridge" GridLines="Both" BorderWidth="2px"
                                    BackColor="White" CellSpacing="1" OnItemCommand="dvForm_ItemCommand" OnItemDataBound="dvForm_ItemDataBound">
                                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray" VerticalAlign="Top">
                                    </SelectedItemStyle>
                                    <ItemStyle CssClass="gridtext" BackColor="Beige" VerticalAlign="Top"></ItemStyle>
                                    <AlternatingItemStyle CssClass="gridtext" BackColor="Beige" VerticalAlign="Top">
                                    </AlternatingItemStyle>
                                    <HeaderStyle ForeColor="Black" BackColor="#C6C3C6"></HeaderStyle>
                                    <Columns>
                                        <asp:TemplateColumn HeaderText='ID'>
                                            <ItemStyle Width="30px" />
                                            <HeaderStyle Width="30px" />
                                            <ItemTemplate>
                                                <asp:Label ID="txtID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ID") %>'
                                                    Width="30px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText='Label'>
                                            <ItemStyle Width="300px" />
                                            <HeaderStyle Width="300px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Label") %>'
                                                    Width="200px" MaxLength = "100" ></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText='Type'>
                                            <ItemStyle Width="150px" />
                                            <HeaderStyle Width="150px" />
                                            <ItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="lstType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstType_IndexSelected">
                                                                <asp:ListItem Text="Single Line TextBox" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Multiple Lines TextBox" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="Date Picker" Value="2"></asp:ListItem>
                                                                <asp:ListItem Text="Time Picker" Value="3"></asp:ListItem>
                                                                <asp:ListItem Text="Label" Value="4"></asp:ListItem>
                                                                <asp:ListItem Text="CheckBox" Value="5"></asp:ListItem>
                                                                <asp:ListItem Text="Dropdown List" Value="6"></asp:ListItem>
                                                                <asp:ListItem Text="Combo Box" Value="7"></asp:ListItem>
                                                                <asp:ListItem Text="Radio Button" Value="8"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText='Element for dropdown, combo, Radio Box'>
                                            <ItemTemplate>
                                                <table>
                                                    <tr valign="top">
                                                        <td>
                                                            <asp:Button ID="btnAddElement" Visible="false" Text="Add Element" CommandName="AddElement"
                                                                runat="server" />
                                                        </td>
                                                        <td>
                                                            <asp:DataGrid ID="btnPlaceHolder" runat="server" AutoGenerateColumns="false" ShowHeader="false"
                                                                BorderWidth="0px" GridLines="None">
                                                                <Columns>
                                                                    <asp:TemplateColumn>
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtElement" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Element") %>' MaxLength="100"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateColumn>
                                                                </Columns>
                                                            </asp:DataGrid>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn>
                                            <ItemStyle Width="90px" />
                                            <HeaderStyle Width="90px" />
                                            <ItemTemplate>
                                                <asp:Button ID="btnDelete" runat="server" OnClientClick="return confirm('Are you sure you want to delete?')"
                                                    CommandName="DeleteCurrentForm" Text="Delete" />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText='Change order'>
                                            <ItemStyle Width="50px" />
                                            <HeaderStyle Width="50px" />
                                            <ItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:ImageButton runat="server" ID="btnUp" CommandName="UpControl" ImageUrl="~/images/UpArrow.gif" />
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton runat="server" ID="btnDown" CommandName="DownControl" ImageUrl="~/images/DownArrow.gif" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                                    <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                        Mode="NumericPages"></PagerStyle>
                                </asp:DataGrid>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlAssignment" runat="server" Visible="false">
                        <table>
                            <tr>
                              <td>
                                                                     <asp:Button ID="btnBackForm" runat="server" Text="Back" OnClick="btnBackFrom_Click"
                                        Width="100px" />

                              </td>
                            </tr>
                            <tr>
                              <td>
                                 <asp:Label ID ="lblImportant" runat="server" ForeColor="Red" Text="Important:Please test the form definition in MDT before assign to client." >
                                   
                                 </asp:Label>
                              </td>
                            </tr>
                            <tr>
                                <td>
                                   <asp:Label ID = "lblAssignmentError" runat="server" ForeColor="Red" ></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="ddlForm" runat="server"  Font-Bold="true">
                                    </asp:Label>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <table id="tblHistoryOptions" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label runat="server" ID="lblUnassignedvehicles" CssClass="RegularText" EnableViewState="False"
                                                    meta:resourcekey="lblUnassignedvehiclesResource1" Text="Unassigned organizations: "></asp:Label>
                                            </td>
                                            <td>
                                            </td>
                                            <td align="left">
                                                <asp:Label runat="server" ID="lblAssignedvehicles" CssClass="RegularText" EnableViewState="False"
                                                    meta:resourcekey="lblAssignedvehiclesResource1" Text="Assigned organizations: "></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:ListBox ID="lboUnassigned" runat="server" CssClass="formtext" Width="250px"
                                                    meta:resourcekey="lboUnassignedResource1" Height="250px" DataTextField="Company"
                                                    DataValueField="OrganizationId" SelectionMode="Multiple"></asp:ListBox>
                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="cmdAssign" runat="server" CommandName="39" CssClass="combutton" Text="Assign"
                                                                meta:resourcekey="cmdAssignResource1" Width="100px"  OnClick="cmdAssign_Click" />
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="cmdAssignAll" runat="server" CommandName="39" CssClass="combutton"
                                                                Text="Assign All" meta:resourcekey="cmdAssignAllResource1" Width="100px" Enabled="False"
                                                                 Visible="False" />
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="cmdUnAssign" runat="server" CommandName="39" CssClass="combutton"
                                                                Text="UnAssign" meta:resourcekey="cmdUnAssignResource1" Width="100px" 
                                                                OnClick="cmdUnAssign_Click" />
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="cmdUnAssignAll" runat="server" CommandName="39" CssClass="combutton"
                                                                Text="UnAssign All" meta:resourcekey="cmdUnAssignAllResource1" Width="100px"
                                                                Enabled="False"  Visible="False" />
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td>
                                                <asp:ListBox ID="lboassigned" runat="server" CssClass="formtext" Width="250px" meta:resourcekey="lboUnassignedResource1"
                                                    Height="250px" DataTextField="Company" DataValueField="OrganizationId" SelectionMode="Multiple">
                                                </asp:ListBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
