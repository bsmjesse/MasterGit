<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportError.aspx.cs" Inherits="SentinelFM.Reports_frmReportError" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <title>Untitled Page</title>
</head>
<body>
   <form id="form1" runat="server">
      <div id='cache'>
         <table style="z-index: 101; left: 200px; position: absolute; top: 200px; width:400px; background-color:#000000;"
            border="0" cellpadding="2" cellspacing="0">
            <tr>
               <td align="center" valign="middle">
                  <table width="100%" border="0" cellpadding="0" cellspacing="0" style="background-color:#FFFFFF; font-family:Arial, Verdana; font-size:large; font-weight:bold;">
                     <tr>
                        <td align="center" valign="middle">
                           <!--font face='Arial, Verdana' size="4"><b-->
                              <br /><%=sn.MessageText%>
                              <br />
                              <br />
                           <!--/b></font-->
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
