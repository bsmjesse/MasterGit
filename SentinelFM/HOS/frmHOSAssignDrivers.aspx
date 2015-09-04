<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSAssignDrivers.aspx.cs" Inherits="HOS_frmHOSAssignDrivers" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register src="../Configuration/Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>

<%@ Register src="HosTabs.ascx" tagname="HosTabs" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <div style="text-align:center">

        <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300" >
            <tr align="left">
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" SelectedControl="btnHOS"  />
                </td>
            </tr>
            <tr align="left">
                <td  >
                    <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td><uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdVehicleDrivers" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width:990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;" class="tableDoubleBorder" >
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0" 
                                                                                                style="width: 950px; height: 495px">
                                                                                                <tr>
                                                                                                    <td align="center" style="width: 100%;" valign="top">
                                                                                                                                                  <table >
            <tr align="center">
               <td colspan="2">
               <B>HOS Vehicle-Drivers Assignment</B>
               <br />
               </td>
            </tr>
            <tr align="center">
               <td align="right">
                    <asp:Label runat="server" ID="lblDriver" CssClass="RegularText" EnableViewState="False"  meta:resourcekey="lblDriverResource1" Text="Driver: "></asp:Label>
               </td>
               <td align="left">
                    <asp:DropDownList ID="ddlDrivers" runat="server" CssClass="RegularText" EnableViewState="true"
                                                            DataTextField="FullName" 
                        DataValueField="DriverId" Width="358px" AutoPostBack="True" 
                        onselectedindexchanged="ddlDrivers_SelectedIndexChanged" >               
                    </asp:DropDownList>
               </td>
            </tr>
            <tr>
              <td colspan="2" align="center">
                  <asp:Label ID= "lblError" runat="server" CssClass="errortext"></asp:Label>
              </td>
            </tr>
            <tr>
              <td colspan="2">
                   <table id="tblHistoryOptions" cellspacing="0" cellpadding="0" border="0" >
                      <tr>
                          <td align="left">
                              <asp:Label runat="server" ID="lblUnassignedvehicles" CssClass="RegularText" EnableViewState="False"  meta:resourcekey="lblUnassignedvehiclesResource1" Text="Unassigned vehicles: "></asp:Label>
                          </td>
                          <td>
                          </td>
                          <td align="left">
                             <asp:Label runat="server" ID="lblAssignedvehicles" CssClass="RegularText" EnableViewState="False"  meta:resourcekey="lblAssignedvehiclesResource1" Text="Assigned vehicles: "></asp:Label>
                          </td>
                      </tr>
                      <tr>
                          <td align="left">
                                <asp:ListBox ID="lboUnassigned" runat="server" CssClass="formtext" 
                                    Width="250px" meta:resourcekey="lboUnassignedResource1" Height="250px" 
                                  DataTextField="Description" DataValueField="BoxId" SelectionMode="Multiple" 
                                  >
                                </asp:ListBox>

                          </td>
                          <td>
                               <table>
                                   <tr>
                                      <td>
                                            <asp:Button ID="cmdAssign" runat="server" CommandName="39" CssClass="combutton"
                                                                              Text="Assign"  
                                                meta:resourcekey="cmdAssignResource1" Width="100px" Enabled="False" 
                                                onclick="cmdAssign_Click" />
                                                <br />
                                                <br />
                                      </td>
                                   </tr>
                                   <tr>
                                      <td>
                                            <asp:Button ID="cmdAssignAll" runat="server" CommandName="39" CssClass="combutton"
                                                                              Text="Assign All"  
                                                meta:resourcekey="cmdAssignAllResource1" Width="100px" Enabled="False" 
                                                onclick="cmdAssignAll_Click" />
                                                <br /><br />
                                      </td>
                                   </tr>
                                   <tr>
                                      <td>
                                            <asp:Button ID="cmdUnAssign" runat="server" CommandName="39" CssClass="combutton"
                                                                              Text="UnAssign"  
                                                meta:resourcekey="cmdUnAssignResource1" Width="100px" Enabled="False" 
                                                onclick="cmdUnAssign_Click" />
                                                <br /><br />
                                      </td>
                                   </tr>
                                   <tr>
                                      <td>
                                            <asp:Button ID="cmdUnAssignAll" runat="server" CommandName="39" CssClass="combutton"
                                                                              Text="UnAssign All"  
                                                meta:resourcekey="cmdUnAssignAllResource1" Width="100px" Enabled="False" 
                                                onclick="cmdUnAssignAll_Click" />
                                                <br /><br />
                                      </td>
                                   </tr>

                               </table>
                          </td>
                          <td>
                                <asp:ListBox ID="lboassigned" runat="server" CssClass="formtext" Width="250px" 
                                    meta:resourcekey="lboUnassignedResource1" Height="250px"
                                DataTextField="Description" DataValueField="BoxId" SelectionMode="Multiple" 
                                 >
                                </asp:ListBox>

                          </td>
                      </tr>
                   </table>             
              </td>
            </tr>
        </table>
                                                                                                         
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
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
