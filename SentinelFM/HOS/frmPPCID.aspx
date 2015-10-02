<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmPPCID.aspx.cs" Inherits="SentinelFM.HOS_frmPPCID"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" EnableTheming="true" %>
<%@ Register src="../Configuration/Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>

<%@ Register src="HosTabs.ascx" tagname="HosTabs" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div >
      <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300" >
            <tr align="left">
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" SelectedControl="btnHOS"  />
                </td>
            </tr>
            <tr align="left">
                <td  >
                    <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td><uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdPPCID" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width:990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;" class="tableDoubleBorder" >
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0" 
                                                                                                style="width: 950px; height: 495px">
                                                                                                <tr valign="top">
                                                                                                    <td>
                                                                                                    
																		<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="657" align="center" border="0">
																			<TR>
																				<TD>
																					<TABLE id="Table9" class=table WIDTH="960px" HEIGHT="500px"
																						border="0">
																						<TR>
																							<TD class="configTabBackground" valign=top  >
																								        <table>
            <tr>
                <td>
                    This page implement two functions.
                    <p>
                        <b>1. Create pccid and pair with box</b><br />
                        &nbsp;&nbsp;&nbsp;&nbsp;Past box id and mdt Serial # to the Source text box.<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;(Box id and mdt Serial # are separated by space, example:63985 765A717290042<br />
                        <b>2. Pair box with existent PPCID</b><br />
                        &nbsp;&nbsp;&nbsp;&nbsp;Past box id and mdt PPCID to the Source text box.<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;(Box id and PPCID are separated by space, example:63985 765SMF0-A717-290-042)<br />
                    </p>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblSource" runat="server" Text="Source" Font-Bold ="true"></asp:Label><br />
                                <asp:TextBox ID="txtPPcid" TextMode="MultiLine" BorderStyle="Inset" Height="444px"
                                    Width="350px" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblDesc" runat="server" Text="Paired box and PPCID" Font-Bold ="true"></asp:Label><br />
                                <asp:TextBox ID="txtDesc" TextMode="MultiLine" BorderStyle="Inset" Height="444px"
                                    Width="400px" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="20px" Width="100%"
                        Text="" meta:resourcekey="lblMessageResource1"></asp:Label>
                </td>
            </tr>
        </table>
																							</TD>
																						</TR>
																					</TABLE>
																				</TD>
																			</TR>
																		</TABLE>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
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
