<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DriverSearch.ascx.cs" Inherits="SentinelFM.UserControl_DriverSearch" %>

<%if(loadResources) { %>
<script type="text/javascript" src="//code.jquery.com/ui/1.8.3/jquery-ui.js"></script>
<link rel="stylesheet" href="//code.jquery.com/ui/1.8.3/themes/smoothness/jquery-ui.css" />
<style type="text/css">
    .bsmforminput
    {
        width:<%=width%>;
        height:<%=height%>;
        border: 1px solid #809db9;
        border-bottom-right-radius: 0 !important;
        border-bottom-left-radius: 0 !important;
        border-top-right-radius: 0 !important;
        border-top-left-radius: 0 !important;
        padding: 3px !important;
        font-family: verdana !important;
        font-size: 12px !important;
    }
    .bsmforminput:focus, .bsmforminput:hover {
        border: 1px solid #ddbb77;
    }
    .ui-autocomplete {z-index: 9999 !important;}
    .ui-autocomplete-loading {
        background: white url('http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.2/themes/smoothness/images/ui-anim_basic_16x16.gif') right center no-repeat;
    }
    .ui-widget {
        font-size: 12px !important;
    }
    .ui-state-hover {
        background: none repeat scroll 0 0 #0a246a !important;
        color: White !important;
    }
    .ui-corner-all {
        border-radius: 0 !important;
    }

</style>
<script type="text/javascript">
    var RootUrl = "<%=RootUrl %>";
</script>
<%} %>
<script type="text/javascript">    
    var hasMoreResult = hasMoreResult || false;

    $(document).ready(function () {
        try {

            $('#<%=Input_DriverName %>').autocomplete({
                source: function (request, response) {
                    searchLat = 0;
                    searchLon = 0;
                    var layernames = "";
                    hasMoreResult = false;
                    $.ajax({
                        url: RootUrl + "UserControl/DriverSearchData.aspx?input=" + request.term + "&language=en&action=autocomplete&geoserver=&layernames=" + layernames,                        
                        dataType: "json",
                        type: "GET",
                        success: function (data) {
                            //alert(data.predictions);
                            /****/
                            if (data.moreResult && data.moreResult[0].hasMoreResults == "1")
                                hasMoreResult = true;

                            response($.map(data.predictions, function (item) {
                                return {
                                    label: item.DriverDescription,
                                    value: item.DriverName.replace("   "," "),
                                    reference: item.DriverId
                                };
                            }));                            
                            /***/
                        },
                        error: function (error) {
                            //alert(error);
                            return "";
                        }
                    });

                },
                focus: function (event, ui) {
                    if(ui.item.value != "")
                        $(this).val(ui.item.value);
                    else
                        event.preventDefault();
                },
                minLength: 2,
                select: function (event, ui) {
                    if (ui.item.reference != "") {
                        $('#<%=Input_DriverID %>').val(ui.item.reference);
                        if ("<%=onDriverSelect%>" != "")
                            eval("<%=onDriverSelect%>(" + ui.item.reference + ")");
                    }
                    else
                        event.preventDefault();
                    
                },
                open: function (a,b) {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                    if (hasMoreResult)
                        $('.ui-autocomplete').append('<li style="margin-top:10px;padding-left:0.4em;float:left;clear:left;font-style:italic;width:100%;"> more results are found but not displayed, please continue to refine search.</li>');
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                }
            });
            
        }
        catch (ex) { }
    });

    
</script>

<div id="<%=ClientID %>" style="display:-moz-inline-stack;display:inline-block;zoom:1;*display:inline;">
<input type="text" placeholder="<%=placeholder %>" name="<%=Input_DriverName %>" id="<%=Input_DriverName %>" class="bsmforminput ui-autocomplete-input" autocomplete="off" />
<input type="hidden" id="<%=Input_DriverID %>" name="<%=Input_DriverID %>" />
</div>
