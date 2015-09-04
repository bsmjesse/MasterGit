//var map=null;
var xOffset=8;
var yOffset=12;
var geotype="Rectangle";
var geopoints=null;
var geoband=null;
var drawlayer=null;
var drawing=false;
var key=16;
var submit=false;
var spin=null;
//alert("Loading Vex Draw");

function GetDrawMap(maptype) {

    try {
        if (maptype!="GeoZone")
            geotype=maptype;       
//        var cpin=new VELatLong(45.95115,-97.20703);
//        map=new VEMap('vex');
//        map.EnableShapeDisplayThreshold(false);
//        map.LoadMap(cpin,3,true,false,VEMapMode.Mode2D,true,null);
//        map.AttachEvent("onclick",MapClickDraw);
//        if (geotype!="Landmark") {
//            map.AttachEvent("onkeyup", MapKeyUp);
//            map.AttachEvent("onkeydown", MapKeyDown);
//        }
          AddDrawControlBox();
          landlayer.Show();
          geolayer.Show();
    } catch(err) { alert("GetMap:"+err.description);}     
}

function MapKeyUp(mapevent) {
    if (mapevent.keyCode==key&&drawing) 
        EndDraw();
}

function MapKeyDown(mapevent) {
    if (mapevent.keyCode==key&&!drawing) 
        StartDraw();
}

function SetCursor(cross) {
    try {
        var cur=document.getElementById("vex");
        if (cross) 
            cur.childNodes[0].style.cursor = "crosshair"; 
        else
            cur.childNodes[0].style.cursor = "hand"; 
        cur.setActive();
    } catch(err) { alert("SetCursor:"+err.description);}
}

function MapClickDraw(mapevent) {  
    try{
        if(!drawing) return; 
        if(drawlayer==null) {
            drawlayer=new VEShapeLayer();
            drawlayer.SetTitle("Draw Layer");
            map.AddShapeLayer(drawlayer); 
        }
        var bandpoints=new Array();
        if(geopoints==null) {
            geopoints=new Array();
            map.AttachEvent("onmousemove",MapMouseMove);
        }
        if (geopoints.length>=10) {
            alert("Geozones can contain a maximum of 10 points. \r\nPlease simplify your point set");
            return;
        }
        var xy=map.PixelToLatLong(new VEPixel(mapevent.mapX+xOffset,mapevent.mapY+yOffset));
        geopoints.push(xy);   
        if (geotype=="Landmark") {
            var xyz=map.PixelToLatLong(new VEPixel(mapevent.mapX+xOffset+5,mapevent.mapY+yOffset+5));
            var cpin=new VEShape(VEShapeType.Pushpin,new VELatLong(xyz.Latitude,xyz.Longitude));
            var cicon=new VECustomIconSpecification();
            cicon.ForeColor=new VEColor(255,0,0,1.0);
            cicon.TextContent=geopoints.length;
            cicon.TextFont="Arial";
            cicon.TextSize=9;
            cicon.TextOffset=new VEPixel(20,0);
            cicon.Image="../veicons/Landmark.ico";
            cpin.SetCustomIcon(cicon);
            drawlayer.AddShape(cpin);  
            map.DetachEvent("onmousemove",MapMouseMove);
            EndDraw();
            return;       
        }else{
            if(geopoints.length==1) {
                bandpoints.push(xy);
                bandpoints.push(xy);
                geoband=new VEShape(VEShapeType.Polyline,bandpoints);
                geoband.SetLineColor(new VEColor(255,0,0,1));
                geoband.SetLineWidth(1);
                geoband.HideIcon();
                drawlayer.AddShape(geoband);          
            }
             if (geotype=="Rectangle") {
                if(geopoints.length==1) {
                    bandpoints.push(xy);
                    bandpoints.push(xy);
                    bandpoints.push(xy);
                    bandpoints.push(xy);
                    bandpoints.push(xy);
                    geoband.SetPoints(bandpoints);
                    var xyz=map.PixelToLatLong(new VEPixel(mapevent.mapX+xOffset+5,mapevent.mapY+yOffset+5));
                    spin=new VEShape(VEShapeType.Pushpin,new VELatLong(xyz.Latitude,xyz.Longitude));
                    var cicon=new VECustomIconSpecification();
                    cicon.ForeColor=new VEColor(255,0,0,1.0);
                    cicon.TextContent=geopoints.length;
                    cicon.TextFont="Arial";
                    cicon.TextSize=9;
                    cicon.TextOffset=new VEPixel(20,0);
                    cicon.Image="../veicons/StartGeoZone.ico";
                    spin.SetCustomIcon(cicon);
                    drawlayer.AddShape(spin);                          
                } else if(geopoints.length>=2) { drawlayer.DeleteShape(spin); EndDraw(); }
 
            }else if (geotype=="Polygon") { //Poly
                if (geopoints.length==1) {
                    var xyz=map.PixelToLatLong(new VEPixel(mapevent.mapX+xOffset+5,mapevent.mapY+yOffset+5));
                    spin=new VEShape(VEShapeType.Pushpin,new VELatLong(xyz.Latitude,xyz.Longitude));
                    var cicon=new VECustomIconSpecification();
                    cicon.ForeColor=new VEColor(255,0,0,1.0);
                    cicon.TextContent=geopoints.length;
                    cicon.TextFont="Arial";
                    cicon.TextSize=9;
                    cicon.TextOffset=new VEPixel(20,0);
                    cicon.Image="../veicons/StartGeoZone.ico";
                    spin.SetCustomIcon(cicon);
                    drawlayer.AddShape(spin);                  
                } else if(geopoints.length>1&&geopoints.length<10) {
                    drawlayer.DeleteShape(spin);
                    geoband.SetPoints(geopoints);
                    bandpoints=geoband.GetPoints();
                    bandpoints.push(bandpoints[0]);
                    geoband.SetPoints(bandpoints);
                 }
            }
       }      
        if(geopoints.length>=2) {  
            map.DetachEvent("onmousemove",MapMouseMove);
        }    
        document.getElementById("mbpointcount").innerHTML=geopoints.length;    
    } catch(err) { alert("click:"+mapevent.error); }
}

function MapMouseMove(mapevent) {  
    try{
        var bandpoints=null;
        var xy=map.PixelToLatLong(new VEPixel(mapevent.mapX+xOffset,mapevent.mapY+yOffset));
        if (geotype=="Rectangle") {
            if (geopoints.length>=2) return;       
            bandpoints=geoband.GetPoints();
            bandpoints[1]=new VELatLong(bandpoints[1].Latitude,xy.Longitude);
            bandpoints[2]=new VELatLong(xy.Latitude,xy.Longitude);
            bandpoints[3]=new VELatLong(xy.Latitude,bandpoints[3].Longitude);
            if(bandpoints!=null)
                geoband.SetPoints(bandpoints);
        } 
    } catch(err) { map.DetachEvent("onmousemove",MapMouseMove); } //alert("mousemove:"+mapevent.error); }
}


function AddDrawControlBox()
{
     try { 
        var vis="visible";     
        var evis="visible";    
        if (geotype=="Landmark"||edit==false)
            vis="hidden";
        if (edit==false)
            evis="hidden";
        var el=document.createElement("div");
        el.id="drawcontrol";
        el.className="mapdrawcontrolbox";
        el.style.left="11px";
        el.style.top="435px";     
        el.innerHTML=maptype+"<br>";
        var elg=document.createElement("div");
        elg.id="geocontrol";
        elg.style.visibility=vis;
        elg.className="mapgeocontrolbox";
        var s4=document.createElement('span');
        s4.innerHTML="Geozone Type:&nbsp;";
        elg.appendChild(s4);

       try{
        var rdo1 = document.createElement('<input type="radio" name="mbradio" id="mbrect" value="Rectangle" checked/>');
       }catch(err){
        rdo1 = document.createElement('input');
        rdo1.setAttribute('type','radio');
        rdo1.setAttribute('id','mbrect');
        rdo1.setAttribute('name','mbradio');
        rdo1.setAttribute('checked','checked');
        rdo2.setAttribute('value','Rectangle');
        }
        try {
            rdo1.addEventListener("click",RadioCheck,false);
        } catch(err) { rdo1.attachEvent('onclick',RadioCheck);}
        elg.appendChild(rdo1);       
        elg.appendChild(document.createTextNode('Rectangle'));
        //el.appendChild(document.createElement('br'));
       try{
        var rdo2 = document.createElement('<input type="radio" id="mbpoly" value="Polygon" name="mbradio"/>');
       }catch(err){
        rdo2 = document.createElement('input');
        rdo2.setAttribute('type','radio');
        rdo2.setAttribute('id','mbpoly');
        rdo2.setAttribute('name','mbradio');
        rdo2.setAttribute('value','Polygon');
       }
        try {
            rdo2.addEventListener("click",RadioCheck,false);
        } catch(err) { rdo2.attachEvent('onclick',RadioCheck);}
        elg.appendChild(rdo2);
        elg.appendChild(document.createTextNode('Polygon'));
 
        elg.appendChild(document.createElement('br'));

        elg.appendChild(document.createElement('br'));
        var s1=document.createElement('span');
        s1.innerHTML="Points:&nbsp;";
        var s2=document.createElement('span');
        s2.setAttribute('id','mbpointcount');
        s2.innerHTML="0";

        elg.appendChild(s1);
        elg.appendChild(s2);
        elg.appendChild(document.createElement('br'));
        elg.appendChild(document.createElement('br'));
        
        el.appendChild(elg);

        var cbsdb=document.createElement("input");
        cbsdb.id="mbgeostart";
        cbsdb.type="button";
        cbsdb.style.visibility=evis;
        cbsdb.value="Start Drawing";
        try {
            cbsdb.addEventListener("click",StartDraw,false);
        } catch(err) { cbsdb.attachEvent('onclick',StartDraw);}
        el.appendChild(cbsdb);

        //el.appendChild(document.createElement('br'));
        
        var cbedb=document.createElement("input");
        cbedb.id="mbgeoend";
        cbedb.type="button";
        cbedb.setAttribute('disabled','disabled'); 
        cbedb.style.visibility=evis;
        cbedb.value="Finish Drawing";
        try {
            cbedb.addEventListener("click",EndDraw,false);
        } catch(err) { cbedb.attachEvent('onclick',EndDraw);}
        el.appendChild(cbedb);        
        
        //el.appendChild(document.createElement('br'));
        
      
        

                 
        var cbcdb=document.createElement("input");
        cbcdb.id="mbgeoclear";
        cbcdb.type="button";
        cbcdb.style.visibility=evis;
        cbcdb.value="Clear";
        try {
            cbcdb.addEventListener("click",ClearDraw,false);
        } catch(err) { cbcdb.attachEvent('onclick',ClearDraw);}
        el.appendChild(cbcdb);
                                 
        var cbsdb=document.createElement("input");
        cbsdb.id="mbgeosave";
        cbsdb.type="button";
        cbsdb.style.visibility=evis;
        cbsdb.setAttribute('disabled','disabled'); 
        cbsdb.value="Save";
        try {
            cbsdb.addEventListener("click",SaveDraw,false);
        } catch(err) { cbsdb.attachEvent('onclick',SaveDraw);}
        el.appendChild(cbsdb);

        var cbcdb=document.createElement("input");
        cbcdb.id="mbgeoclose";
        cbcdb.type="button";
        cbcdb.value="Close";
        try {
            cbcdb.addEventListener("click",CloseDraw,false);
        } catch(err) { cbcdb.attachEvent('onclick',CloseDraw);}
        el.appendChild(cbcdb);

        map.AddControl(el);   

    } catch(err) { alert("AddDrawControlBox:"+err.description);}
}

function StartDraw()
{
    drawing=true;
    try {
        document.getElementById("mbgeostart").setAttribute('disabled','disabled');        
        document.getElementById("mbgeoend").setAttribute('disabled','');                
        document.getElementById("mbgeosave").setAttribute('disabled','disabled');        
        if (drawlayer!=null)
            drawlayer.DeleteAllShapes();
        geopoints=null;
        map.vemapcontrol.EnableGeoCommunity(true);
        SetCursor(true);
        map.AttachEvent("onclick",MapClickDraw);
    } catch(err) { } //alert("StartDraw:"+err.description);}
}

function EndDraw()
{
    drawing=false;
    try {    
        document.getElementById("mbgeoend").setAttribute('disabled','disabled');  
        map.vemapcontrol.EnableGeoCommunity(false);
        SetCursor(false);
        map.DetachEvent("onmousemove",MapMouseMove);
        if (geopoints!=null) {
            if((geotype=="Rectangle"&&geopoints.length==2)||(geotype=="Polygon"&&geopoints.length>2)||(geotype=="Landmark"&&geopoints.length==1))
                document.getElementById("mbgeosave").setAttribute('disabled',''); 
        }
        map.DetachEvent("onclick",MapClickDraw);
    } catch(err) { } //alert("EndDraw:"+err.description);}
}


function ClearDraw()
{
    try {    
        document.getElementById("mbgeostart").setAttribute('disabled','');        
        document.getElementById("mbgeoend").setAttribute('disabled','disabled');        
        document.getElementById("mbpointcount").innerHTML=0;    
        geopoints=null;
        drawlayer.DeleteAllShapes();
        EndDraw();
    } catch(err) { } //alert("ClearDraw:"+err.description);}
}

function SaveDraw()
{
    try {   
        document.getElementById("mbgeosave").setAttribute('disabled','disabled');            
        var values="";
        for(var i=0;i<geopoints.length;i++) {
            values=values+geopoints[i].Latitude+",";
            values=values+geopoints[i].Longitude+",";          
        }
        values=values.substr(0,values.length-1);
        document.getElementById("Points").value=values;   
        //submit=true;  
        document.getElementById("mbgeostart").style.visibility="hidden";        
        document.getElementById("mbgeoend").style.visibility="hidden";        
        document.getElementById("mbgeoclear").style.visibility="hidden";  
	alert("1");
        document.MapForm.submit();
	alert("2");
      
    } catch(err) { } // alert("SaveDraw:"+err.description);}
}

function CloseDraw()
{
    try {    
//        if (submit)
//           document.MapForm.submit();
        window.close();
    } catch(err) { } // alert("SaveDraw:"+err.description);}
}

function RadioCheck()
{
    var rb=document.getElementsByName("mbradio");
    for(var i=0;i<rb.length;i++) {
        if (rb[i].checked) {
            SetGeozoneType(rb[i].value);
            return;
        }
    }
}

function SetGeozoneType(value)
{
    geotype=value;
    ClearDraw();
}