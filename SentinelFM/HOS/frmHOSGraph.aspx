<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSGraph.aspx.cs" Inherits="SentinelFM.frmHOSGraph" %>

<%@ Register Assembly="ISNet.WebUI.ISDataSource, Version=1.0.1500.1, Culture=neutral, PublicKeyToken=c4184ef0d326354b"
    Namespace="ISNet.WebUI.DataSource" TagPrefix="ISDataSource" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
           <asp:XmlDataSource ID="XmlSource" runat="server" XPath="HOSInfo/HoursOfServiceId" >           
                <Data><HOSInfo><HoursOfServiceId ServiceDate='10/15/2008 10:00 AM' StateTypeName='Test' HOSexception='false' /></HOSInfo></Data>
            </asp:XmlDataSource>
            <asp:GridView ID="GridView1" runat="server" DataSourceID="XmlSource" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="StateTypeName" />
                </Columns>
            </asp:GridView>
            <ISWebGrid:WebGrid ID="WebGrid1" runat="server" DataSourceID="XmlSource" Height="250px"
                UseDefaultStyle="True" Width="500px">
                <RootTable >
                    <Columns>
                         <ISWebGrid:WebGridColumn Caption="Date" DataMember="ServiceDate" DataType="System.DateTime"
                                                                            Name="ServiceDate" Width="300px">
                                                                        </ISWebGrid:WebGridColumn>
                                                                        <ISWebGrid:WebGridColumn Caption="State" DataMember="StateTypeName" Name="StateTypeName"
                                                                            Width="100px">
                                                                        </ISWebGrid:WebGridColumn>
                                                                        <ISWebGrid:WebGridColumn Caption="HOSexception" DataMember="HOSexception" Name="HOSexception"
                                                                            Visible="False">
                                                                        </ISWebGrid:WebGridColumn>
                    </Columns>
                </RootTable>
                <ChartSettings ChartType="Line">
                </ChartSettings>
                <LayoutSettings InitialView="PivotChartView">
                </LayoutSettings>
                
                 <ChartDataCollection>
            <ISWebGrid:ChartPivotDataConfig DataMember="StateTypeName"  />
        </ChartDataCollection>
        <ChartCategoryCollection>
            <ISWebGrid:ChartPivotFilterConfig DataMember="StateTypeName" />
        </ChartCategoryCollection>
       
        
            </ISWebGrid:WebGrid>
            
           
           
            <ISWebGrid:WebGrid ID="dgData" runat="server" Height="250px" UseDefaultStyle="True"
                                                                Width="506px" AllowPivotCharting="True" DataSourceID="XmlSource" >
                                                                <RootTable Caption="HOSinfo">
                                                                    <Columns>
                                                                        <ISWebGrid:WebGridColumn Caption="Date" DataMember="ServiceDate" DataType="System.DateTime"
                                                                            Name="ServiceDate" Width="300px">
                                                                        </ISWebGrid:WebGridColumn>
                                                                        <ISWebGrid:WebGridColumn Caption="State" DataMember="StateTypeName" Name="StateTypeName"
                                                                            Width="100px">
                                                                        </ISWebGrid:WebGridColumn>
                                                                        <ISWebGrid:WebGridColumn Caption="HOSexception" DataMember="HOSexception" Name="HOSexception"
                                                                            Visible="False">
                                                                        </ISWebGrid:WebGridColumn>
                                                                        
                                                                    </Columns>
                                                                </RootTable>
                                                                                <ChartDataCollection>
                                                                                    <ISWebGrid:ChartPivotDataConfig DataMember="State" />
        </ChartDataCollection>
        <ChartSeriesCollection>
            <ISWebGrid:ChartPivotFilterConfig DataMember="State" DataType="System.Int32" FilterCriterias="" />
        </ChartSeriesCollection>
        <ChartCategoryCollection>
            <ISWebGrid:ChartPivotFilterConfig DataMember="ServiceDate" DataType="System.DateTime"
                DateInterval="Quarters" FilterCriterias="" PivotType="ByMonth" />
        </ChartCategoryCollection>
        <ChartInteractiveUI>
            <ChartTypeRibbon>
                <SmoothLineType Enabled="False" />
                <DoughnutType Enabled="False" />
                <AreaType Enabled="False" />
                <PieType Enabled="False" />
                <LineType Enabled="False" />                
                <ColumnType Enabled="False" />
                <StepLineType Enabled="False" />
            </ChartTypeRibbon>            
        </ChartInteractiveUI>
        <ChartSettings ChartType="Bar">            
            <VisualEffectSettings EnableJittering="True" JitteringSteps="3" />
            <LightModelSettings EnableLighting="True" LightModel="ISLights" GlobalAmbientLight="White">
            </LightModelSettings>
            <HeaderSettings Text="HOS">
            </HeaderSettings>
        </ChartSettings> 
        

                                                                
                                                            </ISWebGrid:WebGrid>
                                                            
          </div>
    </form>
</body>
</html>
