<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmLogViewer.aspx.cs" Inherits="SentinelFM.Admin.frmLogViewer" %>
<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
    TagPrefix="ISWebInput" %>

<%@ Register assembly="ISNet.WebUI.WebGrid" namespace="ISNet.WebUI.WebGrid" tagprefix="ISWebGrid" %>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Log Viewer</title>
    <link rel="stylesheet" type="text/css" href="GlobalStyle.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="height:130px" >
       
                    <fieldset>
                        <table class="formtext">
                            <tr>
                                <td style="width: 3px; height: 44px;" >
                                    From:</td>
                                <td style="height: 44px" >
                                    <iswebinput:webinput id="txtFrom" runat="server" height="17px" width="182px"
                                        wrap="Off"><HIGHLIGHT Type="Phrase" IsEnabled="True" /><DATETIMEEDITOR accessKey="Space" IsEnabled="True" /></iswebinput:webinput></td>
                                <td style="width: 3px; height: 44px" >
                                    <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Height="22px"
                                        meta:resourcekey="cboHoursFromResource1" Width="60px">
                                    </asp:DropDownList></td>
                                <td style="width: 3px; height: 44px" >
                                    <asp:DropDownList ID="cboMinutesFrom" runat="server" CssClass="RegularText" 
                                        >
                                        <asp:ListItem  Text="00" Value="00"></asp:ListItem>
                                        <asp:ListItem  Text="15" Value="15"></asp:ListItem>
                                        <asp:ListItem  Text="30" Value="30"></asp:ListItem>
                                        <asp:ListItem  Text="45" Value="45"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 3px; height: 44px">
                                    
                                </td>
                                <td style="width: 3px; height: 44px">
                                    To:</td>
                                <td style="width: 3px; height: 44px">
                                    <ISWebInput:WebInput ID="txtTo" runat="server" Height="17px" Width="182px"
                                        >
                                        <HighLight IsEnabled="True" Type="Phrase" />
                                        <DateTimeEditor AccessKey="Space" IsEnabled="True">
                                        </DateTimeEditor>
                                    </ISWebInput:WebInput>
                                </td>
                                <td style="width: 3px; height: 44px">
                                    <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Height="22px"
                                        meta:resourcekey="cboHoursToResource1" Width="59px">
                                    </asp:DropDownList></td>
                                <td style="width: 3px; height: 44px">
                                    <asp:DropDownList ID="cboMinutesTo" runat="server" CssClass="RegularText" 
                                        >
                                        <asp:ListItem  Text="00" Value="00"></asp:ListItem>
                                        <asp:ListItem  Text="15" Value="15"></asp:ListItem>
                                        <asp:ListItem  Text="30" Value="30"></asp:ListItem>
                                        <asp:ListItem Text="45" Value="45"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 3px; height: 21px;" >
                                    Module:</td>
                                <td style="height: 21px" colspan="6"  class="formtext" >
                                    <asp:CheckBoxList ID="chkModules" runat="server" RepeatDirection="Horizontal" 
                                        CssClass="formtext">
                                        <asp:ListItem Value="12">SLS</asp:ListItem>
                                        <asp:ListItem Value="1">ACM</asp:ListItem>
                                        <asp:ListItem Value="45">Integration Service</asp:ListItem>
                                        <asp:ListItem Value="3,9,10,16,17,18,20,22,24,27,28,30,31,33,34,36,37,38,39,43,40,41,42,43,44,46,47,48,50,51,54" 
                                            Selected="True">DCL</asp:ListItem>
                                        <asp:ListItem Value="101,102,103" 
                                            >ASI</asp:ListItem>
                                    </asp:CheckBoxList></td>
                                <td style="width: 3px; height: 21px">
                                </td>
                                <td style="width: 3px; height: 21px">
                                    </td>
                            </tr>
                            <tr>
                                <td style="width: 3px" >
                                    Filter:</td>
                                <td colspan="3" >
                                    <asp:TextBox ID="txtFilter" runat="server" Width="100%"></asp:TextBox></td>
                                <td style="width: 3px">
                                </td>
                                <td style="width: 3px">
                                    Top:</td>
                                <td align="left" colspan="3">
                                    <asp:TextBox ID="txtTop" runat="server" Width="61%"></asp:TextBox>
                                    <asp:Button ID="cmdView" runat="server" CssClass="combutton" Text="View" 
                                        onclick="cmdView_Click" /></td>
                            </tr>
                            <tr>
                                <td style="width: 3px" >
                                    &nbsp;</td>
                                <td >
                                    &nbsp;</td>
                                <td style="width: 3px" >
                                    &nbsp;</td>
                                <td style="width: 3px" >
                                    &nbsp;</td>
                                <td style="width: 3px">
                                    &nbsp;</td>
                                <td style="width: 3px">
                                    &nbsp;</td>
                                <td align="left" colspan="3">
                                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    
                    
                    </fieldset> 
                      </div>
                      <div>
                  <ISWebGrid:WebGrid ID="dgLogger" runat="server" Height="400px"   Width="100%" 
                        UseDefaultStyle="True" OnInitializeDataSource="dgLogger_InitializeDataSource" 
                        ><RootTable>
                     <Columns>
                        
                         <ISWebGrid:WebGridColumn Caption="Module" DataMember="ModuleName" Name="ModuleName" Width="8%" 
                           >
                        </ISWebGrid:WebGridColumn>
                        
                        <ISWebGrid:WebGridColumn Caption="Message" DataMember="Message" Name="Message" Width="92%" 
                           >
                        </ISWebGrid:WebGridColumn>
                        
                     </Columns>
                  </RootTable>
                       <LayoutSettings AutoWidth=true AutoHeight=true  AutoColMinWidth=50   ShowRefreshButton=False  RowChangedAction=OnTheFlyPostback 
                      AllowSorting="Yes"  RowHeightDefault=25px
                      PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" />
                
                  </ISWebGrid:WebGrid>
                 </div>
           
    
  
    </form>
</body>
</html>
