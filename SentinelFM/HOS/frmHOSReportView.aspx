<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSReportView.aspx.cs" Inherits="SentinelFM.HOS_frmHOSReportView" %>

<%@ Register assembly="ISNet.WebUI.WebGrid" namespace="ISNet.WebUI.WebGrid" tagprefix="ISWebGrid" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DAILY RECORD of DUTY STATUS</title>
   
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
        }
    </style>
   
</head>
<body leftmargin="2" topmargin="2" rightmargin="0" bottommargin="2">
    <form id="form1" runat="server">
    <table class="formtext" style="width:95%">
    
        <tr>
            <td style="width:100%" style="width:95%" >
            <fieldset>
                <table  class="formtext" width="100%">
                    <tr>
                        <td align="center"><b>
                <asp:Label ID="lblHOSReport" runat="server" Font-Size="Larger" 
                    Text="DAILY RECORD of DUTY STATUS"></asp:Label></b>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="Label1" runat="server" Text="(ONE CALENDAR DAY - 24 HOURS)"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align=center  >
                            <table class="formtext" >
                                <tr>
                                    <td align=center  >
                                    <br>
                <asp:Label ID="lblDriverCaption" runat="server" Text="Driver:"></asp:Label><b>
                <asp:Label ID="lblDriver" runat="server"></asp:Label></b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblDateCaption" runat="server" Text="Date:"></asp:Label><b>
                <asp:Label ID="lblDate" runat="server"></asp:Label></b>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
                </fieldset> 
            </td>
        </tr>
         <tr>
            <td>
                <asp:datagrid id="dgHOS" runat="server" class="formtext" 
                    AutoGenerateColumns="False" Visible="False">
                    <Columns>
                        <asp:BoundColumn DataField="ServiceDate" HeaderText="Date"></asp:BoundColumn>
                        <asp:BoundColumn DataField="StateTypeName" HeaderText="State"></asp:BoundColumn>
                        <asp:BoundColumn DataField="HOSDescription" HeaderText="Description">
                        </asp:BoundColumn>
                    </Columns>
                </asp:datagrid>
            </td>
        </tr>
        
        <tr>
            <td>
            <hr />
                                                        <asp:Image ID="imgGraph" runat="server" meta:resourcekey="imgGraphResource1" />
                                                        <hr />
            </td>
        </tr>
       
       
       <tr>
            <td>
                
                <asp:Label ID="lblRemarks" runat="server" Text="Remarks:"></asp:Label>
                
            </td>
        </tr>
       
       
       <tr>
            <td>
                
                &nbsp;</td>
        </tr>
    </table>
    </form>
</body>
</html>
