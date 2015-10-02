<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHelp.aspx.cs" Inherits="SentinelFM.Help_frmHelp" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<TITLE></TITLE>
		<META NAME="GENERATOR" Content="Microsoft Visual Studio 7.0"/>
		<LINK href="../GlobalStyle.css" type="text/css" rel="stylesheet/">
	</HEAD>
	<BODY>
	<div style="border-right: gray 2px outset; border-top: gray 2px outset;
            z-index: 101; left: 12px; border-left: gray 2px outset; border-bottom: gray 2px outset;
            position: absolute; top: 4px; height: 97%; width: 98%;  background-color: #fffff0">
            
		<TABLE id="Table1" 	height="100%" bgColor="#fffff0" cellSpacing="0" cellPadding="0" width="100%" border="0">
			<TR bgColor="#fffff0">
				<TD width="158" height="18"></TD>
				<TD align="left" height="18"><STRONG><FONT color="#000066"> &nbsp;&nbsp;&nbsp;&nbsp;</FONT></STRONG>
				</TD>
			</TR>
			<TR>
				<TD width="158" align="center"><asp:HyperLink ID="lnkGettingStartedGuideImg" NavigateUrl="<%$ Resources:GettingStartedGuide %>" Target=_blank   runat="server" meta:resourcekey="lnkGettingStartedGuideImgResource1"><IMG src="../images/downloads.gif" border="0"/></asp:HyperLink></TD>
				<TD>&nbsp;
						<TABLE id="Table2" height="97" cellSpacing="0" cellPadding="0" width="300" border="0">
							<TR>
								<TD width="20"></TD>
								<TD>
                           
                           <asp:HyperLink ID="lnkGettingStartedGuide"   NavigateUrl="<%$ Resources:GettingStartedGuide %>" Target=_blank   runat="server" meta:resourcekey="lnkGettingStartedGuideResource1">Getting Started Guide</asp:HyperLink></TD>
							</TR>
							<TR>
								<TD></TD>
								<TD>
									<P class="formtext">
                              <asp:Label ID="lblGettingStartedGuide" runat="server" Text="This getting started guide contains basic information about the application interface. It also contains information about the Help itself." meta:resourcekey="lblGettingStartedGuideResource1"></asp:Label>&nbsp;</P>
								</TD>
							</TR>
						</TABLE>
					
				</TD>
			</TR>
			<TR>
				<TD height="10" width="158"></TD>
				<TD height="10"></TD>
			</TR>
			<TR>
				<TD align="center" width="158"><asp:HyperLink
                           ID="lnkUserGuideImg" runat="server"  NavigateUrl="<%$ Resources:UserGuide %>" Target="_blank" meta:resourcekey="lnkUserGuideImgResource1"><IMG src="../images/reference.gif" border="0"></asp:HyperLink>
				</TD>
				<TD>&nbsp;
					<TABLE id="Table4" height="97" cellSpacing="0" cellPadding="0" width="300" border="0">
						<TR>
							<TD width="20"></TD>
							<TD><asp:HyperLink
                           ID="lnkUserGuide" runat="server" NavigateUrl="<%$ Resources:UserGuide %>" Target="_blank" meta:resourcekey="lnkUserGuideResource1">User Guide</asp:HyperLink></TD>
						</TR>
						<TR>
							<TD></TD>
							<TD>
								<P class="formtext">
                           &nbsp;<asp:Label ID="lblUserGuide" runat="server" Text="The User Guide helps you use the application interface on a day-to-day basis." meta:resourcekey="lblUserGuideResource1"></asp:Label></P>
							</TD>
						</TR>
					</TABLE>
				</TD>
			</TR>
			<TR>
				<TD height="10" width="158"></TD>
				<TD height="10"></TD>
			</TR>
			<TR>
				<TD align="center" width="158"><asp:HyperLink ID="lnkConfigurationGuideImg" runat="server" NavigateUrl="<%$ Resources:ConfigurationGuide %>" Target="_blank" meta:resourcekey="lnkConfigurationGuideImgResource1"><IMG src="../images/faqs.gif" border="0"></asp:HyperLink></TD>
				<TD>&nbsp;
					<TABLE id="Table3" height="97" cellSpacing="0" cellPadding="0" width="300" border="0">
						<TR>
							<TD width="20"></TD>
							<TD><asp:HyperLink ID="lnkConfigurationGuide" runat="server" NavigateUrl="<%$ Resources:ConfigurationGuide %>" Target="_blank" meta:resourcekey="lnkConfigurationGuideResource1">Configuration Guide</asp:HyperLink></TD>
						</TR>
						<TR>
							<TD></TD>
							<TD>
								<P class="formtext">
                           <asp:Label ID="lblConfigurationGuide" runat="server" Text="The Configuration Guide will help you set up the application interface. Configure the interface according to the preferences of the person who will be using it the most, generally the dispatcher." meta:resourcekey="lblConfigurationGuideResource1"></asp:Label>&nbsp;</P>
							</TD>
						</TR>
					</TABLE>
				</TD>
			</TR>
			<TR>
				<TD width="158" height="100"></TD>
				<TD align="right" height="100"><a href="http://www.adobe.com/products/acrobat/readermain.html" target="top"></a></TD>
			</TR>
		</TABLE>
		</div>
	</BODY>

</HTML>

