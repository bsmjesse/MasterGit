
var vMap = Class.create({
    sImageName: "rubberBandLayer",
    oImageInfo: {},
    oSelectInfo: {},
    bDown: false,
    bMustSetValues: false,
    mouseX: 0,
    mouseY: 0,
    parentObject: null,
    redirect: "",


    getMouse: function(e) {
        this.mouseX = e.clientX + $(document.body).scrollLeft;
        this.mouseY = e.clientY + $(document.body).scrollTop;
    },

    showSelect: function() {
        Element.setStyle($("selectLayer"), { visibility: "visible" });
    },

    hideSelect: function() {
        Element.setStyle($("selectLayer"), { visibility: "hidden" });
    },

    redrawSelect: function() {
        var top = (this.oSelectInfo.y1 < this.oSelectInfo.y2 ? this.oSelectInfo.y1 : this.oSelectInfo.y2) - (document.all ? 0 : 1);
        var left = (this.oSelectInfo.x1 < this.oSelectInfo.x2 ? this.oSelectInfo.x1 : this.oSelectInfo.x2) - (document.all ? 0 : 1);
        var width = (Math.abs(this.oSelectInfo.x2 - this.oSelectInfo.x1) - 2); if (width < 0) width = 0;
        var height = (Math.abs(this.oSelectInfo.y2 - this.oSelectInfo.y1) - 2); if (height < 0) height = 0;
        Element.setStyle($("selectLayer"), {
            top: top,
            left: left,
            width: width,
            height: height
        });
    },

    clear: function() {
        $('imagex').value = 0;
        $('imagey').value = 0;
        $('imageW').value = 0;
        $('imageH').value = 0;
    },

    move: function(e) {
        if (this.bDown) {
            if (this.bMustSetValues) {
                this.oSelectInfo.x1 = this.mouseX;
                this.oSelectInfo.y1 = this.mouseY;
                this.bMustSetValues = false;
            }
            this.getMouse(e);
            if (this.mouseX < this.oImageInfo.x) this.mouseX = this.oImageInfo.x;
            if (this.mouseX >= (this.oImageInfo.x + this.oImageInfo.w)) this.mouseX = (this.oImageInfo.x + this.oImageInfo.w);
            if (this.mouseY < this.oImageInfo.y) mouseY = this.oImageInfo.y;
            if (this.mouseY > (this.oImageInfo.y + this.oImageInfo.h)) mouseY = (this.oImageInfo.y + this.oImageInfo.h);
            this.oSelectInfo.x2 = this.mouseX;
            this.oSelectInfo.y2 = this.mouseY;
            $('imagex').value = (this.oSelectInfo.x1 < this.oSelectInfo.x2 ? this.oSelectInfo.x1 : this.oSelectInfo.x2) - this.oImageInfo.x;
            $('imagey').value = (this.oSelectInfo.y1 < this.oSelectInfo.y2 ? this.oSelectInfo.y1 : this.oSelectInfo.y2) - this.oImageInfo.y;
            $('imageW').value = Math.abs(this.oSelectInfo.x2 - this.oSelectInfo.x1);
            $('imageH').value = Math.abs(this.oSelectInfo.y2 - this.oSelectInfo.y1);
            this.redrawSelect();
        }
        else
            Event.stopObserving($(this.sImageName), "mousemove");

        return false;
    },

    up: function(e) {
	this.bDown = false;
	if (e.button == 2) return false;
        Event.stopObserving($(this.sImageName));
        var oTmp = { x: this.mouseX, y: this.mouseY };
        this.getMouse(e);
        $('imageEndx').value = parseFloat($('imagex').getValue()) + parseFloat($('imageW').getValue());
        $('imageEndy').value = parseFloat($('imagey').getValue()) + parseFloat($('imageH').getValue());
        //parent.window.location.href = "frmVehicleMap.aspx?CoordInX=" + $('imagex').getValue() + "&CoordInY=" + $('imagey').getValue() + "&CoordEndX=" + $('imageEndx').getValue() + "&CoordEndY=" + $('imageEndy').getValue();
        if (redirect="frmbigmap")
            window.location.href = this.redirect + ".aspx?CoordInX=" + $('imagex').getValue() + "&CoordInY=" + $('imagey').getValue() + "&CoordEndX=" + $('imageEndx').getValue() + "&CoordEndY=" + $('imageEndy').getValue();
          else
            this.parentObject.location.href = this.redirect + ".aspx?CoordInX=" + $('imagex').getValue() + "&CoordInY=" + $('imagey').getValue() + "&CoordEndX=" + $('imageEndx').getValue() + "&CoordEndY=" + $('imageEndy').getValue();
            
        
        return true;
    },

    down: function(e) {
        if (e.button == 2) return false;      
        this.bMustSetValues = true;
        this.bDown = true;
        this.clear();
        this.getMouse(e);
        $('imagex').value = this.mouseX;
        $('imagey').value = this.mouseY;
        this.showSelect();
        this.getMouse(e);
        if (this.mouseX < this.oImageInfo.x) this.mouseX = this.oImageInfo.x;
        if (this.mouseX > (this.oImageInfo.x + this.oImageInfo.w)) this.mouseX = (this.oImageInfo.x + this.oImageInfo.w);
        if (this.mouseY < this.oImageInfo.y) this.mouseY = oImageInfo.y;
        if (this.mouseY > (this.oImageInfo.y + this.oImageInfo.h)) this.mouseY = (this.oImageInfo.y + this.oImageInfo.h);
        Event.observe($(this.sImageName), "mousemove", this.move.bindAsEventListener(this));
        return false;
    },

    initialize: function(parent, redirect) {

        Element.setStyle($('rubberBandLayer'), { width: imWidth + "px" });
        this.parentObject = parent;
        this.redirect = redirect;
        this.oImageInfo = { x: -1, y: -1, w: -1, h: -1 };
        this.oSelectInfo = { x1: -1, y1: -1, x2: -1, y2: -1 };
        var oPosition = Element.positionedOffset($(this.sImageName));
        var oDim = Element.getDimensions($(this.sImageName));
        this.oImageInfo.x = oPosition.left;
        this.oImageInfo.y = oPosition.top;
        this.oImageInfo.w = oDim.width;
        this.oImageInfo.h = oDim.height;
        Event.observe($(this.sImageName), "mousedown", this.down.bindAsEventListener(this));
        Event.observe($(this.sImageName), "mouseup", this.up.bindAsEventListener(this));
        //Event.observe($(this.sImageName), "mouseout", this.up.bindAsEventListener(this));

        this.clear();
    },

    check: function() { return true; }

});








