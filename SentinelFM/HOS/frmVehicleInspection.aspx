<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleInspection.aspx.cs" Inherits="HOS_frmVehicleInspection" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register Src="HOSViewTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">      
        .RegularText
        {}
       
        .style1
        {
            width: 15%;          
            vertical-align:top;
            margin-left:auto;            
        }
        .style2
        {
            width: 85%;         
            vertical-align: top;
            margin-left:auto;
        }
      
         .iframeStyle
        {
            width: 100%;         
            vertical-align: top;
            margin-left:auto;
            height:1000px;
        }
        .style3
        {
            width: 380px;
        }
        .buttonStyle
        {
            border: 1px;
        }
    </style>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/Telerik_AddIn.js"></script>
    </head>
    
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
        <div id="divList">
               <select id ="inspectionImageList" size="50" style="width:100%" onclick="showImage(this.value)">
               </select>
        </div>
        <div id="divImage"><img style="width:100%; height:100%" id="imgInspection" /></div>
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" AsyncPostBackTimeout = "3600">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>

    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1"
    />
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel2" runat="server" Skin="Hay"   meta:resourcekey="LoadingPanel1Resource2"
     IsSticky="false" BackgroundPosition="Top"  Height="3000"    />
 
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"  OnAjaxRequest="RadAjaxManager1_AjaxRequest" 
         EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls   >
                    <telerik:AjaxUpdatedControl ControlID="pnl" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="pnl">
                <UpdatedControls   >
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="LogSheetsGrid">
               <UpdatedControls>
                  <telerik:AjaxUpdatedControl ControlID="lblMessage"  />
                  <telerik:AjaxUpdatedControl ControlID="iframe"  />
                  <telerik:AjaxUpdatedControl ControlID="LogSheetsGrid"  LoadingPanelID= "LoadingPanel2"/>
              </UpdatedControls>
           </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="btnErrorLog">
               <UpdatedControls>
                  <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
              </UpdatedControls>
           </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="dgErrorLog">
               <UpdatedControls>
                  <telerik:AjaxUpdatedControl ControlID="dgErrorLog" LoadingPanelID="LoadingPanel1" />
              </UpdatedControls>
           </telerik:AjaxSetting>

        </AjaxSettings>
    </telerik:RadAjaxManager>
        
        <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="100%"  >
            <tr align="left" >
                <td>
                    <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="100%"  >
                        <tr>
                            <td>
                                <uc2:hostabs ID="HosTabs1" runat="server" SelectedControl="cmdVehicleInspection" />
                            </td>
                        </tr>
                        <tr>
                        <td valign="top">
                          <table id="Table3" border="0" cellpadding="3" cellspacing="3" class="frame" style="border-color: #009933;" width="100%" height="800px" >
                          <tr>
                          <td align="center" valign="top" ><div style="text-align: left; height:100%" id="ImageFilesPanel"></div>

      </td>
      </tr>
      </table>
      </td>
      </tr>
      </table>
      </td>
      </tr>
      </table>
      <asp:HiddenField ID="hidDiv" runat="server" />
      <asp:HiddenField ID="hidFilter" runat="server" />
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">


        </script>
    </telerik:RadCodeBlock>
    </form>
        <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />   
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>    
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>
    <script type="text/javascript">
        var ImageFolder = "";
        var uploadFordersWindow = null;
        var store = null;
        var tree = null;
        var foldersJson = null;

        Ext.create('Ext.Panel', {
            renderTo: 'ImageFilesPanel',
            type: 'hbox',
            width: '100%',
            layout: {
                align: 'stretch',
                pack: 'center',
                type: 'vbox'
            },
            items: [
                {
                    xtype: 'container',
                    width: '100%',
                    layout: {
                        align: 'stretch',
                        pack: 'center',
                        type: 'hbox'
                    },
                    items: [
                        {
                            xtype: "textfield",
                            fieldLabel: "<b>Inspection Image Folder</b>",
                            name: "InspectionImageFolder",
                            flex: 6,
                            value: ImageFolder,
                            labelWidth: 180,
                            readOnly: true
                        },
                        { 
                            xtype: 'button',
                            flex: 1,
                            text: '<b>Change Image Folder</b>',
                            handler: function () {
                                selectUploadFolder();
                            }
                        }/*,
                        {
                            xtype: 'button',
                            flex: 1,
                            text: '<b>Upload Image File</b>'
                        }*/
                    ]
                },
                {
                    xtype: 'container',
                    height: '100%',
                    layout: {
                        align: 'stretch',
                        pack: 'center',
                        type: 'hbox'
                    },
                    items: [
                        {
                            xtype: 'panel',
                            flex: 1,
                            contentEl: 'divList'
                        }
                        , {
                            xtype: 'panel',
                            flex: 3,
                            contentEl: 'divImage'
                        }
                    ]
                }
            ]
        });

        function getUploadFolderTree() {

            var path = $("[name=InspectionImageFolder]")[0].value;

            Ext.Ajax.request({
                url: 'frmVehicleInspection.aspx/GetUploadFolderTree',
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                success: function (conn, response, options, eOpts) {
                    var result = Ext.decode(conn.responseText).d;
                    displayUploadFolderTree(result);
                },
                failure: function (conn, response, options, eOpts) {
                    $("divImage").val = "";
                }
            });
        }

        function displayUploadFolderTree(result) {
            uploadFordersWindow = new Ext.form.Panel({
                width: 400,
                height: 380,
                frame: true,
                title: 'Inspection Image Upload Folders',
                draggable: true,
                floating: true,
                closable: true,
                modal:true,
                layout: {
                    align: 'center',
                    pack: 'center',
                    type: 'vbox'
                }
            });

            createFolderTree(uploadFordersWindow, result);

            uploadFordersWindow.show();
        }

        function createFolderTree(c, result) {
            store = Ext.create('Ext.data.TreeStore', {
                root: {
                    text: '/',
                    id: 'src',
                    value: '',
                    expanded: true,
                    children: result
                },
                folderSort: true,
                sorters: [{
                    property: 'text',
                    direction: 'ASC'
                }]
            });

            tree = Ext.create('Ext.tree.Panel', {
                itemId: 'navTree',
                store: store,
                viewConfig: {
                    plugins: {
                        ptype: 'treeviewdragdrop'
                    }
                },
                //renderTo: 'tree-div',
                height: 350,
                width: 450,
                useArrows: true,
                dockedItems: [{
                    xtype: 'toolbar',
                    items: [{
                        text: 'Expand All',
                        handler: function () {
                            tree.getEl().mask('Expanding tree...');
                            var toolbar = this.up('toolbar');
                            toolbar.disable();

                            tree.expandAll(function () {
                                tree.getEl().unmask();
                                toolbar.enable();
                            });
                        }
                    }, {
                        text: 'Collapse All',
                        handler: function () {
                            var toolbar = this.up('toolbar');
                            toolbar.disable();

                            tree.collapseAll(function () {
                                toolbar.enable();
                            });
                        }
                    }, {
                        text: 'Select Folder',
                        handler: function () {
                            if (tree.getSelectionModel().hasSelection()) {
                                var folder = "";
                                if (tree.selModel.selected.items[0].raw) {
                                    folder = tree.selModel.selected.items[0].raw.value;
                                }
                                var select = $("#selImageFolder")[0];

                                var txt = $("[name=InspectionImageFolder]")[0];

                                txt.value = folder;
                                getImageList();

                                uploadFordersWindow.close();

                            } else {
                                Ext.MessageBox.alert('Please select a folder');
                            }


                        }
                    }]
                }]
            });

            c.add(tree);

        }

        function getUploadFolderList() {

            var path = $("[name=InspectionImageFolder]")[0].value;

            Ext.Ajax.request({
                url: 'frmVehicleInspection.aspx/GetUploadFolderList',
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                success: function (conn, response, options, eOpts) {
                    var result = Ext.decode(conn.responseText).d;
                    displayUploadFolderList(result);
                },
                failure: function (conn, response, options, eOpts) {
                    $("divImage").val = "";
                }
            });

        }


        function displayUploadFolderList(list) {
            uploadFordersWindow = new Ext.form.Panel({
                width: 400,
                height: 400,
                title: 'Inspection Image Upload Folders',
                draggable: true,
                floating: true,
                closable: true,
                layout:{
                    align: 'center',
                    pack: 'center',
                    type: 'vbox'
                },
                items: [
                    {
                        xtype: 'component',
                        flex: 5,
                        fieldLabel:'Inspection Image Upload Folders:',
                        listeners: {
                            render: function(c) {
                                var select = document.createElement("select");
                                select.size = 20;
                                select.style.width = "300px";
                                select.id = "selImageFolder";
                                var option = document.createElement("option");
                                option.text = '';
                                option.value = "";
                                select.add(option);

                                for (var i = 0; i < list.length; i++) {
                                    option = document.createElement("option");
                                    option.text = list[i];
                                    option.value = list[i];
                                    select.add(option);
                                }
                                c.el.dom.appendChild(select);
                            }
                        }
                    },
                    {
                        xtype : 'button',
                        flex: 1,
                        text: '<b>Select Folder</b>',
                        style: 'border:1px',
                        handler: function () {
                            var select = $("#selImageFolder")[0];

                            var folder = "";
                            for (var i = 0; i < select.length; i++) {
                                if (select[i].selected == "1") {
                                    folder = select[i].value;
                                    break;
                                }
                            }

                            var txt = $("[name=InspectionImageFolder]")[0];
                            txt.value = folder;
                            getImageList();

                            uploadFordersWindow.close();
                        }
                    }
                ]
            });

            uploadFordersWindow.show();
        }

        function getImageList() {

            var path = $("[name=InspectionImageFolder]")[0].value;

            Ext.Ajax.request({
                url: 'frmVehicleInspection.aspx/GetImageList',
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                params: { path: Ext.JSON.encode(path) },
                success: function (conn, response, options, eOpts) {
                    var result = Ext.decode(conn.responseText).d;
                    displayImageList(result);
                },
                failure: function (conn, response, options, eOpts) {
                    $("divImage").val = "";
                }
            });

        }

        function displayImageList(list) {
            var listCtrl = $("#inspectionImageList");
            listCtrl.empty();
            for (i = 0; i < list.length; i++) {
                listCtrl.append(
                    $('<option></option>').val(list[i]).html(list[i])
                );
            }

            if (listCtrl[0].length > 0) {
                listCtrl[0][0].selected = true;
                showImage(listCtrl[0][0].value);
            } else {
                $("#imgInspection")[0].src = "";
                if (inspectionWindow != null && !inspectionWindow.closed) {
                    inspectionWindow.close();
                }
            }
        }

        function showImage(fileName) {
            var folder = $("[name=InspectionImageFolder]")[0].value;
            Ext.Ajax.request({
                url: 'frmVehicleInspection.aspx/GetImage',
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                params: { folder: Ext.JSON.encode(folder), fileName: Ext.JSON.encode(fileName) },
                success: function (conn, response, options, eOpts) {
                    $("#imgInspection")[0].src = "data:image/jpeg;base64," + unescape(encodeURIComponent(Ext.JSON.decode(conn.responseText).d));
                    showInspectionForm(fileName);
                },
                failure: function (conn, response, options, eOpts) {
                    $("#imgInspection")[0].src = "";
                }
            });

        }

        var inspectionWindow = null;

        $(window).unload(function () {

            if (inspectionWindow != null && !inspectionWindow.closed) {
                  inspectionWindow.close();
            }

        });



        function showInspectionForm(fileName) {
            var folder = $("[name=InspectionImageFolder]")[0].value;

            if (!inspectionWindow || inspectionWindow.closed) {

                inspectionWindow = window.open(encodeURI("frmDVIRInput.aspx?folder=" + folder + "&fileName=" + fileName), "newwin", "height=800, width=900,toolbar=no,scrollbars=no,menubar=no");

            } else {
                if (inspectionWindow.document.title == fileName) {
                    inspectionWindow.focus();
                    return;
                } else if(inspectionWindow.initialForm){
                    inspectionWindow.initialForm(folder, fileName);
                }
            }

        }

        function selectUploadFolder() {
            //var list = getUploadFolderList();
            getUploadFolderTree();
        }

        Ext.onReady(function () {
            getImageList();

        });

        window.removeFileFromList = function (fileName) {
            fileName = decodeURI(fileName);
            var listCtrl = $("#inspectionImageList")[0];

            var opt = null;
            for (var i = 0; i < listCtrl.length; i++) {
                opt = listCtrl[i];
                if (opt.value == fileName) {
                    listCtrl.remove(i);
                    break;
                }
            }
            getImageList();
        }

     </script>

</body>
</html>

