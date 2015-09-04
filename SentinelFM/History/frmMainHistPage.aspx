<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMainHistPage.aspx.cs" Inherits="History_frmMainHistPage" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop" TagPrefix="ISWebDesktop" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
       <table width="100%" style="height:100%">
          <tr>
             <td style="width: 90%">
               <iframe src="frmHistMapSoluteMap.aspx"></iframe> 
             </td>
             <td style="width: 10%">
               <iframe src="frmhistorycrt.aspx" ></iframe>
             </td>
          </tr>
          <tr>
             <td colspan="2">
               <form id="frmHistory" name="frmHistory" >
                  <iframe src="frmHistDataGridExtended.aspx" height="500" width="100%"></iframe> 
               </form>
             </td>
          </tr>
       </table>   
    </form>
</body>
</html>
