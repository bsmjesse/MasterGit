<%@ Page Language="c#" Inherits="SentinelFM.frmServicesUpdates" CodeFile="frmServicesUpdates.aspx.cs" Culture="en-US" meta:resourcekey="PageResource2" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmServicesUpdates</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form method="post" runat="server">
        <!-- END CURVED INFORMATION AREA -->
        <!-- periphial content here -->
        <!-- END CURVED INFORMATION AREA -->
        <!-- periphial content here -->
        &nbsp;&nbsp;&nbsp;
        <table id="Table2" style="z-index: 102; left: 5px;  position: absolute;
            top: 0px; height: 600px" cellspacing="0" cellpadding="0"  border="0">
            <tr>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 23px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 18px" height="18">
                </td>
                <td class="formtext">
                    <asp:Label ID="lblLastLoginTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLastLoginTitleResource2" Text="Last Login:"></asp:Label>
                    <asp:Label ID="lblLastLogin" runat="server" CssClass="formtext"
                        meta:resourcekey="lblLastLoginResource2"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 23px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 10px">
                    &nbsp;
                </td>
                <td style=" height: 10px" CssClass="formtext">
                   
                   <table>
                    <tr>
                        <td><asp:Label ID="lblVersionName" runat="server" CssClass="formtext" Font-Bold="False" ForeColor="Black"
                                   Text="Version:" meta:resourcekey="lblVersionNameResource1"></asp:Label></td>
                        <td><asp:Label ID="lblVersion" runat="server" Text="4.13.3" CssClass="formtext" ></asp:Label></td>
                    </tr>
                    <tr>
                        <td> <asp:Label ID="lblRelease" runat="server" CssClass="formtext" Font-Bold="False" ForeColor="Black"
                                   Text="Release date:" meta:resourcekey="lblReleaseResource1"></asp:Label></td>
                        <td> <asp:Label ID="lblReleaseDate" runat="server" CssClass="formtext" Font-Bold="False" ForeColor="Black"
                                   Text="March 13,2014"></asp:Label></td>
                    </tr>
                   </table>
                     
                      </td>
            </tr>
          
            <tr>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 23px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 7px">
                </td>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 219px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 7px">
                   &nbsp;<asp:Label ID="lblWhatNew" runat="server" ForeColor="Black" meta:resourcekey="lblWhatNewResource1"
                      Text="What's New"></asp:Label></td>
            </tr>
            <tr>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 23px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 3px" height="3">
                </td>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 100%;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 3px" height="3">
                    <table id="Table3" cellspacing="0" cellpadding="0" width="100%"
                        border="0">
                        <tr>
                            <td style="border-top: white 1px solid; font-weight: bold; font-size: 11px; font-family: verdana;
                                border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none">
                               &nbsp;<asp:Label ID="lblSystemUpdates" runat="server" Font-Bold="True" meta:resourcekey="lblSystemUpdatesResource1"
                                  Text="System Updates:"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DataGrid ID="dgSystem" runat="server" Width="95%" ShowHeader="False" BorderWidth="0px"
                                    BorderStyle="Ridge" AutoGenerateColumns="False" CellPadding="3" BackColor="White"
                                    GridLines="None" CellSpacing="1" PageSize="2" meta:resourcekey="dgSystemResource2">
                                    <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                    <ItemStyle Font-Size="11px" Font-Names="verdana" ForeColor="Black" CssClass="formtext"
                                        BackColor="White"></ItemStyle>
                                    <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                    <Columns>
                                        <asp:BoundColumn DataField="Msg" HeaderText="System Updates:"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Severity" HeaderText="Severity"></asp:BoundColumn>
                                    </Columns>
                                </asp:DataGrid></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 23px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 10px">
                </td>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 219px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 10px">
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 23px;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 10px">
                </td>
                <td style="font-weight: bold; font-size: 15px; vertical-align: baseline; width: 100%;
                    color: gray; font-family: Arial, Helvetica, sans-serif; height: 10px">
                    <table id="Table4" style="height: 140px" cellspacing="0" cellpadding="0"
                        width="100%" border="0">
                        <tr>
                            <td style="border-top: white 1px solid; font-weight: bold; font-size: 11px; font-family: verdana;
                                border-right-style: none; border-left-style: none; height: 21px; border-bottom-style: none">
                               &nbsp;<asp:Label ID="lblNewFeatures" runat="server" meta:resourcekey="lblNewFeaturesResource1"
                                  Text="New Features:"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DataGrid ID="dgFeatures" runat="server" Width="95%" ShowHeader="False" BorderWidth="0px"
                                    BorderStyle="Ridge" AutoGenerateColumns="False" CellPadding="3" GridLines="None"
                                    CellSpacing="1" BorderColor="Black" meta:resourcekey="dgFeaturesResource2">
                                    <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                    <SelectedItemStyle Font-Bold="True" ForeColor="Black" BackColor="SlateGray"></SelectedItemStyle>
                                    <ItemStyle CssClass="formtext" Font-Size="11px" Font-Names="verdana" ForeColor="Black"
                                        BackColor="White"></ItemStyle>
                                    <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                    <Columns>
                                        <asp:BoundColumn DataField="Msg" HeaderText="New Features: Version 2.2"></asp:BoundColumn>
                                    </Columns>
                                </asp:DataGrid>
                                

                                  
                                <asp:TextBox ID="lblCustomMsg" runat="server" 
                                    CssClass="formtext"                                                                                                              
                                    MaxLength="1000" Rows=14 TextMode="MultiLine" Width="100%" Visible="false" 
                                    ReadOnly="true" BorderWidth="1" />

                                </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
