<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMaintenaceClose.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenaceClose" %>

<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls" TagPrefix="ISWebInput" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Maintenance</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
</head>
<body>
    <form id="form1" runat="server">
      <div>
      </div>
      <table cellpadding="3" cellspacing="3" style="border-right: gray 4px double; border-top: gray 4px double; border-left: gray 4px double; border-bottom: gray 4px double">
          <tr>
              <td>
                  <asp:RadioButtonList ID="optMaintenance" runat="server" AutoPostBack="True" CssClass="formtext" OnSelectedIndexChanged="optMaintenance_SelectedIndexChanged" BorderStyle="Solid" BorderWidth="1px">
                        <asp:ListItem Selected="True" Value="0" Text="Automatically" />
                        <asp:ListItem Value="1" Text="Starting with current value"/>
                        <asp:ListItem Value="2" Text="Next Due Value"/>
                    </asp:RadioButtonList>
                    <asp:Label ID="lblVehicleId" runat="server" Visible="False"/>
                    <asp:Label ID="lblServiceId" runat="server" Visible="False"/>
              </td>
          </tr>
          <tr>
              <td> 
                  <table>
                      <tr>
                          <td style="width: 254px" colspan="2">
                              <asp:Label ID="lblOPerationType" runat="server" CssClass="formtext" Text="Type:"></asp:Label><asp:Label ID="lblMaintenanceType" runat="server" CssClass="formtext"/>
                          </td>
                      </tr>
                      <tr>
                          <td>
                              <asp:Label ID="lblNextDueValue" runat="server" CssClass="formtext" Text="Next Due Value:"/>
                          </td>
                          <td>
                              <asp:TextBox ID="txtMaintenaceValue" runat="server" CssClass="formtext" Visible="False" Text="0"/>
                              <ISWebInput:WebInput ID="txtFromDate" runat="server" Height="17px" Visible="False" Width="118px" Wrap="Off">
                                  <DateTimeEditor AccessKey="Space" IsEnabled="True"/>
                                  <HighLight IsEnabled="True" Type="Phrase"/>
                              </ISWebInput:WebInput>   
                         </td>
                      </tr>
                  </table>
              </td>
          </tr>
          <tr>
              <td style="width: 100px; height: 21px">
                  <asp:Label ID="lblComments" runat="server" CssClass="formtext" Text="Comments:"/>
              </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 21px">
                    <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" CssClass="formtext" Width="243%"/>
                </td>
            </tr>
            <tr>
                <td  align="left" style="height: 15px">
                    <asp:Label ID="lblMessage" runat="server" CssClass="formtext" Font-Bold="True" ForeColor="Green"/>
                </td>
            </tr>
            <tr>
                <td align="left" style="height: 51px" >
                    <table id="TableServiceDetailsSaveCancel" runat="server">
                        <tr id="Tr2" runat="server">
                            <td id="Td5" runat="server">
                                &nbsp;
                                <asp:Button ID="btnSave" runat="server" CssClass="combutton" Text="Save" OnClick="btnSave_Click"/>
                            </td>
                            <td id="Td6" runat="server" style="width: 20px"></td>
                            <td id="Td7" runat="server">
                                &nbsp;
                                <asp:Button ID="btnCancel" runat="server" CausesValidation="False" OnClientClick="window.close()" CssClass="combutton" Text="Cancel"/>
                            </td>
                        </tr>
                    </table>
                    &nbsp;
                    <asp:Label ID="lblOperationTypeId" runat="server" Visible="False"/>
                    <asp:Label ID="lblDueValue" runat="server" Visible="False"/>
                    <asp:Label ID="lblServiceInterval" runat="server" Visible="False"/>
                    <asp:Label ID="lblCurrentValue" runat="server" Visible="False"/>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>