<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="Report">
		<html>
			<style>
			</style>
			<body style="font-family:Arial; font-size:smaller;">
				<!-- Report Captions -->
				<table cellpadding="10">
					<tr>
						<td>
							<!--h1>BSM WIRELESS</h1-->
							<img src="bsm_logo.png" style="width:350px;height:40px"  alt="BSM WIRELESS" title="BSM WIRELESS INC. LOGO"/>
						</td>
						<!--td style="width:60px"></td-->
						<td style="text-align: right; width: 430px;">
							<h2>Install and Service Information</h2>
						</td>
					</tr>
				</table>
				<table cellpadding="10">
					<tr>
						<td style="width:440px; vertical-align: top;">
							<!-- Customer Table -->
							<table cellspacing="0" style="font-family:Arial; font-size:11pt;">
								<caption align="center">
									<b>Customer Information</b>
								</caption>
								<tr style="height:22px">
									<td style="width:120px; text-align:right; padding-right: 5px;">
										Customer
									</td>
									<td style="width:310px; border: outset 1px Gray">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Customer/Name"/>
									</td>
								</tr>
								<tr style="height:22px">
									<td style="text-align:right; padding-right: 5px;">
										Address
									</td>
									<td style="border: outset 1px Gray">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Customer/Address"/>
									</td>
								</tr>
								<tr style="height:22px">
									<td style="text-align:right; padding-right: 5px;">
										Contact Name
									</td>
									<td style="border: outset 1px Gray">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Customer/ContactName"/>
									</td>
								</tr>
								<tr style="height:22px">
									<td style="text-align:right; padding-right: 5px;">
										Contact Number
									</td>
									<td style="border: outset 1px Gray">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Customer/ContactNumber" />
									</td>
								</tr>
							</table>
							<br />
							<!-- Wiring Table -->
							<div style="border: outset 1px Gray; padding: 5px 10px 10px 10px">
								<table cellspacing="0" border="1" style="font-family:Arial; font-size:11pt;">
									<caption align="center">
										<b>Wiring Connections</b>
									</caption>
									<tr>
										<th style="width:30px">Pin</th>
										<th style="width:50px">Sensor</th>
										<th style="width:100px">Color</th>
										<th style="width:120px">Connection</th>
										<th style="width:30px">W</th>
										<th style="width:50px">State</th>
									</tr>
									<xsl:for-each select="Sensors/Sensor">
										<tr>
											<td>
												<xsl:value-of select="@pin" />
											</td>
											<td>
												<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
												<xsl:value-of select="@sensor" />
											</td>
											<td>
												<xsl:value-of select="@color" />
											</td>
											<td>
												<xsl:value-of select="@name" />
											</td>
											<td style="text-align: center;">
												<xsl:choose>
													<xsl:when test="@wired = True">x</xsl:when>
													<xsl:otherwise>
														<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
													</xsl:otherwise>
												</xsl:choose>
											</td>
											<td>
												<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
												<xsl:value-of select="@state" />
											</td>
										</tr>
									</xsl:for-each>
								</table>
							</div>
						</td>
						<td style="vertical-align: top;">
							<!-- Installer Table -->
							<div style="border: outset 1px Gray; padding: 5px 10px 15px 5px; margin-bottom: 10px">
								<table cellspacing="0" style="font-family:Arial; font-size:11pt;">
									<caption style="text-align:center; padding-bottom: 5px">
										<b>Installer Information</b>
									</caption>
									<tr style="height:35px;">
										<td style="text-align: right; padding-right: 5px;">Company Name</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Installer/Company" />
										</td>
									</tr>
									<tr style="height:35px">
										<td style="width:130px; text-align: right; padding-right: 5px;">Installer Name</td>
										<td style="width:190px; border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Installer/Name"/>
										</td>
									</tr>
									<tr style="height:35px">
										<td style="text-align: right; padding-right: 5px;">Installed Date</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Vehicle/DateTime" />
										</td>
									</tr>
									<tr style="height:35px">
										<td style="text-align: right; padding-right: 5px;">Installer Phone</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Installer/Phone"/>
										</td>
									</tr>
									<tr style="height:35px">
										<td style="text-align: right; padding-right: 5px;">Debug Saved?</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										</td>
									</tr>
									<tr style="height:35px">
										<td style="text-align: right; padding-right: 5px;">Tested How?</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										</td>
									</tr>
									<tr style="height:35px">
										<td style="text-align: right; padding-right: 5px;">Installer Signoff</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										</td>
									</tr>
								</table>
							</div>
							<br />
							<!-- Box Location Table -->
							<div style="border: outset 1px Gray; padding: 5px 10px 15px 5px">
								<table cellspacing="0" style="font-family:Arial; font-size:11pt;">
									<caption style="text-align:center; padding-bottom: 5px">
										<b>Box Install Location</b>
									</caption>
									<tr style="height:35px;">
										<td style="width:130px; text-align: right; padding-right: 5px;">
											Box Location
										</td>
										<td style="width:190px; border: outset 1px Gray;">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Location/Box"/>
										</td>
									</tr>
									<tr style="height:35px;">
										<td style="text-align: right; padding-right: 5px;">
											GPS Antenna Loc.
										</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Location/GPSAntenna"/>
										</td>
									</tr>
									<tr style="height:35px;">
										<td style="text-align: right; padding-right: 5px;">
											Cell Antenna Loc.
										</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Location/CellAntenna"/>
										</td>
									</tr>
									<tr style="height:35px;">
										<td style="text-align: right; padding-right: 5px;">
											Ign. Power Source
										</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Location/Ignition"/>
										</td>
									</tr>
									<tr style="height:35px;">
										<td style="text-align: right; padding-right: 5px;">
											Batt. Power Source
										</td>
										<td style="border: outset 1px Gray">
											<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
											<xsl:value-of select="Location/Battery"/>
										</td>
									</tr>
								</table>
							</div>
						</td>
					</tr>
				</table>
				<table cellpadding="10">
					<tr>
						<td>
							<!-- Vehicle Information Table -->
							<table cellspacing="0" border="1" style="font-family:Arial; font-size:11pt;">
								<caption style="text-align: left; padding-left: 10px">
									<b>Vehicle Install Information</b>
								</caption>
								<tr>
									<td style="width:110px; padding-left:5">
										VIN
									</td>
									<td style="width:180px">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Vehicle/VIN" />
									</td>
								</tr>
								<tr>
									<td style="padding-left:5">
										License Plate
									</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Vehicle/LicensePlate"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left:5">
										Year Make
									</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Vehicle/YearMake"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left:5">
										Model
									</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Vehicle/Model"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left:5">
										Name
									</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Vehicle/Name" />
									</td>
								</tr>
							</table>
						</td>
						<td style="vertical-align: top">
							<!-- Box Table -->
							<table cellspacing="0" border="1" style="font-family:Arial; font-size:11pt;">
								<caption style="text-align: left; padding-left: 10px">
									<b>Box</b>
								</caption>
								<tr>
									<td style="width: 120px; padding-left:5">
										Box ID
									</td>
									<td style="width: 350px">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/BoxID"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left:5">
										Serial Number
									</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/BoxSerialNumber"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left:5">Firmware</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/BoxFirmware"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left:5">Device Type</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:choose>
											<xsl:when test="Debug/DeviceType = 0">SFM1000</xsl:when>
											<xsl:when test="Debug/DeviceType = 1">SFM5000</xsl:when>
										</xsl:choose>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
				<!-- Options Table -->
				<table border="1" cellspacing="0" style="margin-left: 10px; font-family: Arial; font-size: 11pt; width: 810px">
					<caption style="text-align: left; padding-left: 10px">
						<b>Install Details</b>
					</caption>
					<tr>
						<td style="width: 260px; padding-left: 5px">
							Install Type:
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
							<xsl:choose>
								<xsl:when test="Installation/Type = 0">New</xsl:when>
								<xsl:when test="Installation/Type = 1">Uninstall</xsl:when>
								<xsl:when test="Installation/Type = 2">Modify</xsl:when>
								<xsl:when test="Installation/Type = 3">Reinstall</xsl:when>
								<xsl:when test="Installation/Type = 4">
									Swap - <xsl:value-of select="Installation/OldBoxId"/>
								</xsl:when>
							</xsl:choose>
						</td>
						<td style="width: 260px; padding-left: 5px">
							Vehicle Type:
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
							<xsl:choose>
								<xsl:when test="VehicleType/Type = 0">Tractor</xsl:when>
								<xsl:when test="VehicleType/Type = 1">Trailer</xsl:when>
								<xsl:when test="VehicleType/Type = 2">Chassis</xsl:when>
								<xsl:when test="VehicleType/Type = 3">Passenger</xsl:when>
								<xsl:when test="VehicleType/Type = 4">
									Other - <xsl:value-of select="VehicleType/Other"/>
								</xsl:when>
							</xsl:choose>
						</td>
						<td style=" padding-left: 5px">
							Fleet Pulse:
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
							<xsl:value-of select="Options/FleetPulse"/>
						</td>
					</tr>
					<tr>
						<td style="padding-left:5" colspan="3">
							Options:
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
							<xsl:for-each select="Options">
								<xsl:if test="Option0 = 1">Basic  </xsl:if>
								<xsl:if test="Option1 = 1">MDT  </xsl:if>
								<xsl:if test="Option3 = 1">Security  </xsl:if>
								<xsl:if test="Option4 = 1">J1708/OBDII  </xsl:if>
								<xsl:if test="Option5 = 1">Satellite  </xsl:if>
								<xsl:if test="Option2 = 1">
									Other - <xsl:value-of select="Other" />
								</xsl:if>
							</xsl:for-each>
						</td>
					</tr>
				</table>
				<br />
				<table style="margin-left: 10px">
					<tr>
						<th style="text-align: left; padding-left: 10px">Installer Notes</th>
						<th style="text-align: left; padding-left: 10px">Wireless Information</th>
						<th style="text-align: left; padding-left: 10px">GPS Information</th>
					</tr>
					<tr>
						<td style="border: outset 1px gray; width:195px; font-size: 9pt; vertical-align: top; padding: 5px 5px 5px 5px">
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
							<xsl:value-of select="Notes"/>
						</td>
						<td style="vertical-align:top; width: 310px">
							<!-- Wireless Table -->
							<table cellspacing="0" border="1" style="font-family:Arial; font-size:11pt;">
								<tr>
									<td style="width: 120px; padding-left: 5px">
										Cell Link Status
									</td>
									<td style="width: 170px">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/CellLink"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">Signal Strength</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/CellSignal"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">Registration</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/CellRegistration"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">Phone Number</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/CellPhone"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">IP Address</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/CellIP"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">Sec. Modem</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/SecondaryModem"/>
									</td>
								</tr>
							</table>
						</td>
						<td style="vertical-align:top;">
							<!-- GPS Table -->
							<table cellspacing="0" border="1" style="font-family:Arial; font-size:11pt;">
								<tr>
									<td style="width: 120px; padding-left: 5px">
										Position Fix
									</td>
									<td style="width: 150px">
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/GPSFix"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">
										Sat. In View
									</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/GPSSatInView"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">Sat. Used</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/GPSSatUsed"/>
									</td>
								</tr>
								<tr>
									<td style="padding-left: 5px">Antenna Status</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/GPSAntenna"/>
									</td>
								</tr>
								<tr style="height:21px">
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</td>
								</tr>
								<tr>
									<td  style="padding-left: 5px">Test Result</td>
									<td>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:value-of select="Debug/TestResult"/>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
				<br />
				<!-- Notes -->
				<!--div style="width: 820px; height: 80px; border: outset 1px gray; padding: 5px 5px 5px 5px">
					<p />Notes:
					<span>
						<xsl:value-of select="Notes"/>
					</span>
				<br />
				</div-->
				<!-- Office use Table -->
				<table cellpadding="0" cellspacing="0"  border="1" style="margin-left: 10px; background-color:LightGrey; font-family:Arial; font-size:11pt;">
					<tr>
						<td style="width:140px; padding-left:5px">
							<b>Office use only</b>
						</td>
						<td style="width:100px; text-align: right; padding-right:5px">Received:</td>
						<td style="width:300px;">
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						</td>
						<td style="width:110px; text-align: right; padding-right:5px">Total $</td>
						<td style="width:135px;">
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						</td>
					</tr>
					<tr>
						<td style="font-size: smaller; padding-left:5px">
							Debug install sheet
						</td>
						<td style="text-align: right; padding-right:5px">Processed:</td>
						<td>
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						</td>
						<td style="text-align: right; padding-right:5px">Order Number:</td>
						<td>
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						</td>
					</tr>
					<tr>
						<td style="font-size: 8pt;">
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
							<script type="text/javascript">document.write(Date());</script>
						</td>
						<td style="text-align: right; padding-right:5px">Initial:</td>
						<td>
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						</td>
						<td style="text-align: right; padding-right:5px">Sheet Number:</td>
						<td>
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						</td>
					</tr>
				</table>
				<br />
				<!-- Footer Table -->
				<table style="margin-left: 10px; border:outset 1px gray; width:810px; height: 30px; vertical-align: middle; font-family:Arial; font-size:11pt;">
					<tr>
						<td style="text-align: left; padding-left: 5px">
							Fax or e-mail this form to 1-425-732-8520 or customercare@bsmwireless.com
						</td>
						<td style="text-align: right; padding-right: 5px">
							BSM Toll-Free 1-866-768-4771
						</td>
					</tr>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>