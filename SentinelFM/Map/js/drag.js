function Drag() {
	this.obj = null
	this.array = new Array()
	this.dropTargets = new Array()
	this.active = false
	this.offsetX = 0
	this.offsetY = 0
	this.zIndex = 0
	this.resort = true
	this.add = DragAdd
	this.addTargets = DragAddTargets
	this.checkTargets = DragCheckTargets
	this.targetHit == null
	this.remove = DragRemove
	this.setGrab = DragSetGrab
	this.mouseDown = DragMouseDown
	this.mouseMove = DragMouseMove
	this.mouseUp = DragMouseUp
	this.onDragStart = new Function()
	this.onDragMove = new Function()
	this.onDragEnd = new Function()
	this.onDragDrop = new Function()
}
function DragAdd() {
	for (var i=0; i<arguments.length; i++) {
		var l = this.array.length
		this.array[l] = arguments[i]
		this.array[l].dragGrab = new Array(0,this.array[l].w,this.array[l].h,0)
		this.zIndex += 1
	}
}
function DragAddTargets() {
	for (var i=0; i<arguments.length; i++) {
		var l = this.dropTargets.length
		this.dropTargets[l] = arguments[i]
		this.dropTargets[l].dragGrab = new Array(0,this.dropTargets[l].w,this.dropTargets[l].h,0)
	}
}
function DragSetGrab(dynlayer,top,right,bottom,left) { 
	dynlayer.dragGrab = new Array(top,right,bottom,left)
}
function DragRemove() {
	for (var i=0; i<arguments.length; i++) {
		for (var j=0; j<this.array.length; j++) {
			if (this.array[j]==arguments[i]) {
				for (var k=j;k<=this.array.length-2;k++) this.array[k] = this.array[k+1]
				this.array[this.array.length-1] = null
				this.array.length -= 1
				break
			}
		}
	}
}
function DragMouseDown(x,y) {
	for (var i=this.array.length-1;i>=0;i--) {
		var lyr = this.array[i]
		if (checkWithinLayer(x,y,lyr)) {
			this.obj = this.array[i]
			this.offsetX = x-this.obj.x
			this.offsetY = y-this.obj.y
			this.active = true
			break
		}
	}
	if (!this.active) return false
	else {
		if (this.resort) {
			this.obj.css.zIndex = this.zIndex++
			for (var j=i;j<=this.array.length-2;j++) this.array[j] = this.array[j+1]
			this.array[this.array.length-1] = this.obj
		}
		this.onDragStart(x,y)
		return true
	}
}
function DragMouseMove(x,y) {
	if (!this.active) return false
	else {
		this.obj.moveTo(x-this.offsetX,y-this.offsetY)
		this.onDragMove(x,y)
		return true
	}
}
function DragMouseUp(x,y) {
	if (!this.active) return false
	else {
		this.active = false
		if (this.checkTargets()) this.onDragDrop()
		this.onDragEnd(x,y)
		return true
	}
}
function DragCheckTargets() {
	for (i in this.dropTargets) {
		var lyr = this.dropTargets[i]
		if (checkWithinLayer(lyr.x,lyr.y,this.obj) ||
		checkWithinLayer(lyr.x+lyr.w,lyr.y,this.obj) ||
		checkWithinLayer(lyr.x,lyr.y+lyr.h,this.obj) ||
		checkWithinLayer(lyr.x+lyr.w,lyr.y+lyr.h,this.obj) ||
		checkWithinLayer(this.obj.x,this.obj.y,lyr) ||
		checkWithinLayer(this.obj.x+this.obj.w,this.obj.y,lyr) ||
		checkWithinLayer(this.obj.x,this.obj.y+this.obj.h,lyr) ||
		checkWithinLayer(this.obj.x+this.obj.w,this.obj.y+this.obj.h,lyr)) {
			this.targetHit = lyr
			return true
		}
	}
	return false
}

function checkWithin(x,y,left,right,top,bottom) {
	if (x>=left && x<right && y>=top && y<bottom) return true
	else return false
}
function checkWithinLayer(x,y,lyr) {
	if (checkWithin(x,y,lyr.x+lyr.dragGrab[3],lyr.x+lyr.dragGrab[1],lyr.y+lyr.dragGrab[0],lyr.y+lyr.dragGrab[2])) return true
	else return false
}

// automatically define the default "drag" Drag Object
drag = new Drag()
