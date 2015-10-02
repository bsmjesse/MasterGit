<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMaptest.aspx.cs" Inherits="UserControl_frmMaptest" %>

<%@ Register Src="MapNavigation.ascx" TagName="MapNavigation" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body topmargin=0 leftmargin=0>
    <form id="form1" runat="server" >
      <table cellpadding=0 cellspacing=0  >
          <tr>
             <td   style="background-color: whitesmoke; border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; border-bottom: gray 1px solid;">
       
                <uc1:MapNavigation ID="MapNavigation1" runat="server" OnLoad="MapNavigation1_Load" />
             </td>
          </tr>
          <tr>
             <td> 
                <img src="../images/GeoMicroStartPageMap.png" /></td>
          </tr>
       </table>
    </form>
</body>
</html>
