function initializeMain(map)
{
    mapscreen = new BsmVE(map); 
    mapscreen.frameResize();
}

function mapEndZoom(mapevent) {
    try {
        var level = mapevent.zoomLevel;           
        mapscreen.setZoomLevel(level);
        setDisplay();
    } 
    catch (e) {
        alert("mapEndZoom error:" + e.description + "\r\n   mapevent error:" + mapevent.error);
    }
}

function mapResize(mapevent) {
    setDisplay();
}

function mapClicked(mapevent) {
    if (mapevent.leftMouseButton)
        mapscreen.zoomToObject(mapevent.elementID)
    else if (mapevent.rightMouseButton) 
        mapscreen.showDetail(mapevent.elementID, mapevent.shiftKey);
}

function mapStartPan(mapevent) {
    mapscreen.clearDragLayer();
}

function mapEndPan(mapevent) {
    setDisplay();
}

function mapError(mapevent) {
    
}
function mapTokenError(mapevent) {
    
}
function mapTokenExpire(mapevent) {
    
}
         
function mapClickDraw(mapevent) {
        mapscreen.draw(mapevent.mapX, mapevent.mapY);
}
    
function mapKeyUp(mapevent) {
    if (mapevent.keyCode == mapscreen.key && mapscreen.drawing) 
        mapscreen.endDraw();
}

function mapKeyDown(mapevent) {
    if (mapevent.keyCode == mapscreen.key && !mapscreen.drawing) 
        mapscreen.startDraw();
}
      
function mapMouseMove(mapevent) {
        mapscreen.setMousePoint(mapevent.mapX, mapevent.mapY);
}
             
function setCursor(cross){
    try {
        var cur = document.getElementById("vex");
        if (cross) 
            cur.childNodes[0].style.cursor = "crosshair";
        else 
            cur.childNodes[0].style.cursor = "hand";
        //cur.setActive();
    } 
    catch (e) {
        //alert("setCursor error:" + e.description);
    }
}       
                       
function mapStartZoom(mapevent) {
    mapscreen.clearDragLayer();
}

function setDisplay() {
    mapscreen.isInBounds();
    //parent.window.frames["framefooter"].enablePOI(mapscreen.isInBounds());
    mapscreen.drawBounds();
}

function  getGeozonesProxy() {
        GetGeoZonesFromService(uid, sid, org, 
                            mapscreen.bounds.TopLeftLatLong.Latitude, 
                            mapscreen.bounds.TopLeftLatLong.Longitude, 
                            mapscreen.bounds.BottomRightLatLong.Latitude, 
                            mapscreen.bounds.BottomRightLatLong.Longitude);
}
                
function  getLandmarksProxy() {
       GetLandmarksFromService(uid, sid, org, 
                            mapscreen.bounds.TopLeftLatLong.Latitude, 
                            mapscreen.bounds.TopLeftLatLong.Longitude, 
                            mapscreen.bounds.BottomRightLatLong.Latitude, 
                            mapscreen.bounds.BottomRightLatLong.Longitude);
 }

  
function searchResults(layer, resultsArray, places, hasMore, veErrorMessage) {
    if (veErrorMessage != null)
        alert(veErrorMessage);
    else 
        mapscreen.displayResults(layer, resultsArray, places, hasMore, veErrorMessage);
    parent.window.frames["framefooter"].cancelWait();
}      