<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeFile="frmDashboardMessages.aspx.cs" Inherits="SentinelFM.Dashboard_frmDashboardMessages" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>




<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    
     <script language="javascript">
			<!--
			 setTimeout('location.reload(true)',120000)
			-->
    </script>
    
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    
</head>
<body leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    <form id="form1" runat="server">
      <div>
       <fieldset  style="margin: 0px 0px 5px 0px; padding: 0px 0px 0px 0px">
                          
                            <table cellpadding=1 cellspacing=1 >
                            <tr>
                                <td>
                                    <asp:Label CssClass=formtext  ID="lblLastUpdatedCaption" runat="server" Text="Updated:"></asp:Label>
                                    <asp:Label CssClass=formtext ID="lblLastUpdated" runat="server" Text=""></asp:Label>
                                </td>
                                <td>&nbsp;&nbsp;&nbsp;</td>
                                <td>
                                    <asp:LinkButton CssClass=formtext Font-Bold=true  ID="cmdReadMessage" runat="server" 
                                        onclick="cmdReadMessage_Click" >Mark as read</asp:LinkButton>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:Label ID="lblMessage" CssClass="formtext" ForeColor=Red runat="server" Text=""  ></asp:Label>
                                      </td>
                          </tr>
                          </table> 
                            
          <iswebgrid:webgrid id="dgMessages" runat="server" Height="90%"   Width="100%" usedefaultstyle="True"
               OnInitializeDataSource="dgMessages_InitializeDataSource">
              <RootTable DataKeyField="MsgId">
                  <Columns>
                     <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                            Caption="chkBoxShow" ColumnType="CheckBox" DataMember="chkBoxShow" EditType="NoEdit"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                         </ISWebGrid:WebGridColumn>
                         
                      <ISWebGrid:WebGridColumn Caption="Time" DataMember="MsgDate" Name="MsgDate"
                          Width="100px">
                      </ISWebGrid:WebGridColumn>
                      <ISWebGrid:WebGridColumn Caption="Vehicle" DataMember="Description" Name="Description"
                          Width="100px">
                      </ISWebGrid:WebGridColumn>
                      
                      <ISWebGrid:WebGridColumn Caption="Message" DataMember="MsgBody" Name="MsgBody"
                          Width="100px">
                      </ISWebGrid:WebGridColumn>
                  </Columns>
              </RootTable>
              <LayoutSettings AutoHeight=true   AutoFitColumns=true        >
            </LayoutSettings>
          </iswebgrid:webgrid></div>

    </form>
</body>
</html>
