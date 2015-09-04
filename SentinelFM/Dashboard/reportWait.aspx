<%@ Page Language="C#" AutoEventWireup="true" CodeFile="reportWait.aspx.cs" Inherits="DashBoard_reportWait" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>


<script type="text/javascript">

        function OpenMyWin() {
            window.open(<%=rpt %>, '');
            window.close();
        } 

</script> 


</head>
<body class="configTabBackground" onload="OpenMyWin()"> 

    <form id="Form1" method="post" runat="server">
        <table class="table"  style='z-index: 101;left: 217px; width: 401px;position: absolute; top: 207px; height: 128px' width="401" border="0" cellpadding="0"
            cellspacing="0">
            <tr>
                <td align="center" valign="middle">
                    <table width="398" bgcolor="#ffffff" border="0" cellpadding="0" cellspacing="0" style="width: 398px;
                        height: 110px">
                        <tr>
                            <td align="center" valign="middle">
                                <font face='Arial, Verdana' size="4" color="gray"><b>
                                    <p>
                                        
                                        <asp:Label ID="lblPleaseWaitMessage" runat="server" >Please wait</asp:Label>&nbsp;</p>
                                    <p>
                                        <asp:Label ID="lblPreparingMessage" runat="server" >Preparing the Report</asp:Label>&nbsp;...</p>
                                    <p>
                                        &nbsp;</p>
                                </b></font>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div>
        </div>
    </form>
</body>
</html>