<%@ Page language="c#" Inherits="SentinelFM.Admin.frmDiagnostic" CodeFile="frmDiagnostic.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Box Diagnostic</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form method="post" runat="server">
			<TABLE id="tblDiagnostic" style="Z-INDEX: 101; LEFT: 178px; WIDTH: 359px; POSITION: absolute; TOP: 150px; HEIGHT: 271px" cellSpacing="0" cellPadding="0" width="359" border="0">
				<TR>
					<TD class="heading" style="HEIGHT: 20px">Box Diagnostic (within&nbsp;
						<asp:DropDownList id="cboHours" runat="server" CssClass="RegularText">
							<asp:ListItem Value="12">12 hours</asp:ListItem>
							<asp:ListItem Value="24">24 hours</asp:ListItem>
							<asp:ListItem Value="36">36 hours</asp:ListItem>
							<asp:ListItem Value="48">48 hours</asp:ListItem>
						</asp:DropDownList>) by:</TD>
				</TR>
				<TR>
					<TD class="formtext" height="30"></TD>
				</TR>
				<TR>
					<TD>
						<TABLE id="Table1" style="BORDER-RIGHT: #009933 thin outset; BORDER-TOP: #009933 thin outset; BORDER-LEFT: #009933 thin outset; WIDTH: 357px; BORDER-BOTTOM: #009933 thin outset; HEIGHT: 91px" cellSpacing="0" cellPadding="0" width="357" border="0">
							<TR>
								<TD class="formtext" style="WIDTH: 5px; HEIGHT: 8px"></TD>
								<TD class="formtext" style="WIDTH: 127px; HEIGHT: 8px"></TD>
								<TD class="formtext" style="WIDTH: 126px; HEIGHT: 10px"></TD>
							</TR>
							<TR>
								<TD class="formtext" style="WIDTH: 5px; HEIGHT: 8px"></TD>
								<TD class="formtext" style="WIDTH: 127px; HEIGHT: 8px">
									<asp:CheckBox id="chkNotValidGPS" runat="server" CssClass="formtext" Text="Not Valid GPS more than" Width="139px"></asp:CheckBox></TD>
								<TD class="formtext" style="WIDTH: 126px; HEIGHT: 8px">
									<asp:DropDownList id="cboPercent" runat="server" CssClass="RegularText">
										<asp:ListItem Value="25">25 %</asp:ListItem>
										<asp:ListItem Value="50">50 %</asp:ListItem>
										<asp:ListItem Value="75">75 %</asp:ListItem>
										<asp:ListItem Value="100">100 %</asp:ListItem>
									</asp:DropDownList></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 5px; HEIGHT: 19px"></TD>
								<TD style="WIDTH: 127px; HEIGHT: 19px"></TD>
								<TD style="WIDTH: 126px; HEIGHT: 19px"></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 5px; HEIGHT: 1px"></TD>
								<TD style="WIDTH: 127px; HEIGHT: 1px">
									<asp:CheckBox id="chkWithoutIP" runat="server" CssClass="formtext" Text="No communications" Width="183px"></asp:CheckBox></TD>
								<TD style="WIDTH: 126px; HEIGHT: 1px"></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 5px"></TD>
								<TD style="WIDTH: 127px"></TD>
								<TD style="WIDTH: 126px; HEIGHT: 10px"></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 5px"></TD>
								<TD style="WIDTH: 127px">
									<asp:CheckBox id="chkReportedFrequency" runat="server" CssClass="formtext" Text="Reported more than" Width="176px"></asp:CheckBox></TD>
								<TD style="WIDTH: 126px" class="formtext">
									<asp:TextBox id="txtReportFrq" runat="server" CssClass="formtext" Width="58px">0</asp:TextBox>&nbsp;messages.
									<asp:RangeValidator id="RangeValidator1" runat="server" ErrorMessage="Please enter a correct  number." Type="Integer" ControlToValidate="txtReportFrq" MinimumValue="0" MaximumValue="9999">*</asp:RangeValidator></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 5px"></TD>
								<TD style="WIDTH: 127px"></TD>
								<TD style="WIDTH: 126px; HEIGHT: 10px"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD height="30" align="middle">
						<asp:Label id="lblMessage" runat="server" CssClass="errortext"></asp:Label>
						<asp:ValidationSummary id="ValidationSummary1" runat="server"></asp:ValidationSummary></TD>
				</TR>
				<TR>
					<TD align="left" height="40">
					</TD>
				</TR>
				<TR>
					<TD align="middle">
						<INPUT class="combutton" id="cmdClose" onclick="window.close()" type="button" value="Close" name="cmdClose">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:button id="cmdView" onmouseover="javascript:this.style.backgroundColor='#FFFFFF'; &#13;&#10;&#9;&#9;&#9;&#9;&#9;&#9;javascript:this.style.color='#FFCC00';&#13;&#10;&#9;&#9;&#9;&#9;&#9;&#9;" tabIndex="4" runat="server" CssClass="combutton" Text="View" Width="94px" onclick="cmdView_Click"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
