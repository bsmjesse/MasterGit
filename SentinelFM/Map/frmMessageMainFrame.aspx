<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMessageMainFrame.aspx.cs" Inherits="Map_frmMessageMainFrame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
	<frameset border="0" frameSpacing="0" borderColor="#0066cc" frameBorder="0" cols="*,0">
	      <frame name="frmMessageRotator" borderColor="#0066cc" scrolling="no" src="frmmessagerotating.aspx" frameBorder="0" noresize>
	      <frame name="frmMessageRefresh" borderColor="#0066cc" scrolling="no" src="frmMessagesRefresh.aspx" frameBorder="0" noresize>
			
</frameset>
</html>
