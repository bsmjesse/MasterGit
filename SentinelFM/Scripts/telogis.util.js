
// utility functions
// origin: telogis demo scripts; Telogis\GeoBase\GeoStream\server\scripts\tutorial.util.js

(function () {

    // eventRegistry [Array]:
    // the internal list of all registered event handlers. Each entry in $eventRegistry is itself an array of
    // objects detailing the element, name and callback of events all bound to a single element.

    var eventRegistry = [];

    // Util [Namespace]:
    // a collection of common functions that are defined in a separate module in order to increase the clarity of the 
    // tutorials.
    
    window.Util = {

        // addEvent [Function]:
        // binds an event handler function to a certain DOM element so that it is triggered by the desired event.
        // This uses reflection to defer the event handling to either 'addEventListener' or 'attachEvent', and if neither
        // are available, the 'onevent' property of the bound element is used.
        // Arguments:
        //
        // - elem [Element]:      the DOM element to which the event is bound, or its string ID.
        // - name [String]:       the name of the event, without the 'on' prefix.
        // - callback [Function]: the function to be executed when the event is triggered.

        addEvent: function (elem, name, callback) {
        
            if (typeof (elem) === 'string' || (elem instanceof String))
                elem = document.getElementById (elem);
        
            if (elem.addEventListener) elem.addEventListener (name, callback, false);
            else if (elem.attachEvent) elem.attachEvent ('on' + name, callback);
            else                       elem ['on' + name] = callback;
            
            if (typeof (elem.__eventRegistryIndex) !== 'number') {
            
                elem.__eventRegistryIndex = eventRegistry.length;
                eventRegistry.push ([]);
            }
            
            eventRegistry [elem.__eventRegistryIndex].push ({elem: elem, name: name, callback: callback});
        },
        
        // flushEvents [Function]:
        // removes all event handlers bound by addEvent().
        
        flushEvents: function () {
    
            for (var i = eventRegistry.length - 1; i >= 0; i--) {

                var record = eventRegistry [i];
                if (record && record.elem)
                    Util.removeEvent (record.elem);
            }
        },
        
        // getCookie [Function]:
        // reads the value of a cooke with the given name.
        // Arguments:
        //
        // - name [String]: the name of the cookie to get the value of.
        //
        // Return [String]: the value of the named cookie.
        
        getCookie: function (name) {
        
            var value = null;
            var index = document.cookie.indexOf (name + '=');
            if (index >= 0) {
            
                value = document.cookie.substring (index + name.length + 1);
                var endIndex = value.indexOf (';');
                if (endIndex >= 0)
                    value = value.substring (0, endIndex);
            }
            
            return value;
        },

        // nothingEvent [Function]:
        // acts to stop any further actions being taken on a DOM event. Returning this from a custom event handler
        // will stop default events (such as the appearance of a context menu) occurring.
        // Arguments:
        //
        // - e [Event]: the parameters for the event.
        //
        // Return [Boolean]: true, to signify that the event has completed.
                
        nothingEvent: function (e) {

            if (e) {

                if (e.preventDefault) e.preventDefault ();
                e.returnValue = false;
            }
            return false;
        },
        
        // removeEvent [Function]:
        // removes an event handler previously bound by a call to addEvent() from a given DOM element. Depending on the number
        // of parameters passed, all events may be removed from $elem, or all events of a certain type, or only a 
        // single callback.
        // Arguments:
        //
        // - elem [Element]:      the DOM element to remove events from, or its string ID.
        // - name [String]:       the name of the event to remove callbacks for. If not supplied, callbacks for all event
        //                        types are removed.
        // - callback [Function]: the exact event callback function to remove. If not supplied, all callbacks for the given
        //                        event type are removed.
        
        removeEvent: function (elem, name, callback) {
        
            if (typeof (elem) === 'string' || (elem instanceof String))
                elem = document.getElementById (elem);
        
            var entry = eventRegistry [elem.__eventRegistryIndex];
            if (entry) {
            
                for (var i = entry.length - 1; i >= 0; i--) {
                
                    var item = entry [i];
                    
                    if (name     && name     !== item.name)     continue;
                    if (callback && callback !== item.callback) continue;
                    
                    if (item.elem.removeEventListener)       item.elem.removeEventListener (item.name, item.callback, false);
                    if (item.name.substring (0, 2) !== 'on') item.name = 'on' + item.name;
                    if (item.elem.detachEvent)               item.elem.detachEvent (item.name, item.callback);
                    if (item.elem [item.name])               item.elem [item.name] = null;
                    
                    entry.splice (i, 1);
                }
                
                if (entry.length === 0) {
                
                    eventRegistry.splice (elem.__eventRegistryIndex, 1);
                    elem.__eventRegistryIndex = null;
                }
            }
        },
        
        // setCookie [Function]:
        // sets the value of the cookie with the specified name.
        // Arguments:
        //
        // - name [String]:   the name of the cookie to set.
        // - value [String]:  the value to set the cookie to.
        // - expiry [Number]: the optional time, in milliseconds, until the cookie should expire.
        
        setCookie: function (name, value, expiry) {
        
            var cookie = name + '=' + value;
            if (expiry) {
            
                var date = new Date ();
                date.setTime (date.getTime () + expiry);
                cookie += '; expires=' + date;
            }
            
            cookie += '; path=/';
            document.cookie = cookie;
        },
        
        // terminateEvent [Function]:
        // stops an event from bubbling any further up the DOM tree.
        // Arguments:
        // 
        // - e [Event]: the event to prevent further propagation of.

        terminateEvent: function (e) {
        
            if (e.stopPropagation) e.stopPropagation ();
            else                   e.cancelBubble = true;
        }
    };

    // register an event that will cause all registered events to be cleaned up properly when the page unloads.

    if (typeof window !== 'undefined') Util.addEvent (window, 'unload', Util.flushEvents);

})();


// contextual menu
// origin: telogis demo scripts; Telogis\GeoBase\GeoStream\server\scripts\tutorial.contextmenu.js

(function () {
    
    var shallowCopy = function (to, from) {
    
        for (var key in from) {
        
            if (from.hasOwnProperty (key)) {
            
                to [key] = from [key];
            }
        }
    };
   
    var CONTAINER_STYLE = {
    
        "backgroundColor":   "#ffffff",
        "border":            "1px solid #000000",
        "width":             "200px",
        "listStyleType":     "none",
        "listStylePosition": "outside"
    };
    
    var ENABLED_STYLE = {
    
        "backgroundColor": "#ffffff",
        "borderWidth":     "0px 0px 0px 0px",
        "color":           "#000000",
        "cursor":          "pointer",        
        "fontFamily":      "sans-serif",
        "fontSize":        "10px",        
        "fontWeight":      "bold",        
        "margin":          "0px 0px 0px 0px",
        "padding":         "3px 0px 3px 0px",
        "textIndent":      "10px",
        "width":           "100%"        
    };
    
    var DISABLED_STYLE = {};
    shallowCopy (DISABLED_STYLE, ENABLED_STYLE);
    DISABLED_STYLE ["color"]  = "#c0c0c0";
    DISABLED_STYLE ["cursor"] = "default";
    
    var HOVER_STYLE = {};
    shallowCopy (HOVER_STYLE, ENABLED_STYLE);
    HOVER_STYLE ["backgroundColor"] = "#d0e0f0";
    
    var SEPARATOR_STYLE = {
    
        "backgroundColor": "#e0e0e0",
        "fontSize":        "0px",
        "height":          "1px",
        "margin":          "5px 0px 5px 0px"
    };

    // ContextMenu [Class]:
    // a class for handling drop-down context menus.

    window.ContextMenu = function () {
        
        this.entries             = {};
        this.separatorCount      = 0;
        
        this.elem                = document.createElement ("div");
        this.elem.style.display  = "none";
        this.elem.style.position = "absolute";        
        this.elem.style.zIndex   = 256;
        this.elem.style.padding  = "0px 0px 0px 0px";
        this.elem.style.margin   = "0px 0px 0px 0px";
        this._setStyle (this.elem, CONTAINER_STYLE);

        document.body.appendChild (this.elem);
    };
    
    // ContextMenu._setStyle [Function]:
    // copies a set of CSS settings from a predefined style to an element.
    // Arguments:
    //
    // - elem [Element]: the element to set the style of.
    // - style [Object]: the associative array of JavaScript/CSS style properties and their corresponding values to
    //                   copy across to the given element.
    
    ContextMenu.prototype._setStyle = function (elem, style) {
        
        for (var i in style)        
            if (style.hasOwnProperty (i))            
                elem.style [i] = style [i];
    };
        
	// ContextMenu._setupItem [Function]:
	// performs common initialisations for a newly created menu entry or separator.
	// Arguments:
	//
	// - id [String]: the name / identifier of the new item.

	ContextMenu.prototype._setupItem = function (id) {
		
		var entry = this.entries[id];
		
		entry.elem             = document.createElement("div");
		entry.elem.parentEntry = entry;
		entry.parentMenu       = this;
		
		Util.addEvent(entry.elem, 'click', function() {
			if (entry.clickCallback) {
				entry.clickCallback();
			}
		});
		
		Util.addEvent(entry.elem, 'mouseover', function() {
			if (entry.mouseOverCallback) {
				entry.mouseOverCallback();
			}
		});
		
		Util.addEvent(entry.elem, 'mouseout', function() {
			if (entry.mouseOutCallback) {
				entry.mouseOutCallback();
			}
		});
		
		this._update(id);
		this.elem.appendChild(entry.elem);
	};
    
	// ContextMenu._update [Function]:
	// updates any changes in the menu entries since the last render.
	// Arguments:
	//
	// - id [String]: the ID of the entry to update.

	ContextMenu.prototype._update = function (id) {

		var self = this;
		if (typeof(Util) !== "object") {
		
			alert('The module "tutorial.contextmenu.js" requires prior inclusion of "tutorial.util.js".');
			return;
		}
		
		var entry  = this.entries [id];
		if (entry.label) {
			
			var styler = function(elem, style) {
				return function() {
				
					if (elem.parentEntry.enabled) {
						self._setStyle(elem, style);
					}
				};
			};
			
			this._setStyle(entry.elem, entry.enabled ? ENABLED_STYLE : DISABLED_STYLE);
			
			entry.clickCallback     = entry.enabled ? this._wrapCallback (entry.elem) : null;
			entry.mouseOverCallback = styler(this.entries [id].elem, HOVER_STYLE);
			entry.mouseOutCallback  = styler(this.entries [id].elem, ENABLED_STYLE);
		}
		else {
		
			this._setStyle(this.entries [id].elem, SEPARATOR_STYLE);
		}
	};
    
    // ContextMenu._wrapCallback [Function]:
    // hides the ContextMenu and calls the associated callback when an item is selected.
    
    ContextMenu.prototype._wrapCallback = function (elem) {
        return function () {
        
            elem.parentEntry.parentMenu.hide ();
            elem.parentEntry.callback.apply (window);
        }
    };
    
    // ContextMenu.appendEntry [Function]:
    // adds a new entry to the ContextMenu.
    // Arguments:
    //
    // - label [String]:      the label to display on the menu entry.
    // - enabled [Boolean]:   whether the label's associated callback can be called.
    // - callback [Function]: the function associated with the label.
        
    ContextMenu.prototype.appendEntry = function (label, enabled, callback) {
    
        this.entries [label] = new ContextMenu.Entry (label, enabled, callback);
        this._setupItem (label);
        this.entries [label].elem.innerHTML = label;
    };

    // ContextMenu.appendSeparator [Function]:
    // adds a separator entry to the ContextMenu.
    // Arguments:
    //
    // - id [String]: an optional parameter to set an identifier for the separator if future manipulations are desired.
    
    ContextMenu.prototype.appendSeparator = function (id) {
    
        if (!id) id = "separator_" + this.separatorCount++;
        this.entries [id] = new ContextMenu.Entry ();
        this._setupItem (id);
    };
    
    // ContextMenu.hide [Function]:
    // hides the ContextMenu.
        
    ContextMenu.prototype.hide = function () {
        
        this.elem.style.display = "none";
    };
    
    // ContextMenu.show [Function]:
    // shows the ContextMenu, setting up the callbacks to execute with given parameters.
    // Arguments:
    // 
    // - e [Event]: the event resulting in the menu display.

    ContextMenu.prototype.show = function (e) {
        
        for (var i in this.entries) {if (this.entries.hasOwnProperty (i)) {this._update (i);}}
        
        var scrollTop           = document.body.scrollTop  ? document.body.scrollTop  : document.documentElement.scrollTop;
        var scrollLeft          = document.body.scrollLeft ? document.body.scrollLeft : document.documentElement.scrollLeft;
        this.elem.style.left    = e.clientX + (scrollLeft) + "px";
        this.elem.style.top     = e.clientY + (scrollTop)  + "px";
        this.elem.style.display = "block";
    };
    
    // ContextMenu.Entry [Class]:
    // represents a single entry for a ContextMenu.
    // Arguments:
    //
    // - label [String]:      the label to display on the Entry.
    // - enabled [Boolean]:   whether the label's associated callback may be called.
    // - callback [Function]: the function associated with the label.
    
    ContextMenu.Entry = function (label, enabled, callback) {
        
        if (label) {

            this.label    = label;
            this.enabled  = enabled;
            this.callback = callback;
        }
        
        this.elem       = null;
        this.parentMenu = null;
    };
})();
