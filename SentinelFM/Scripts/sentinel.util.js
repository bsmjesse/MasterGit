
/*
    Summary:
        Utility functions.

    Dependencies:
        * jQuery
        * console support (optional)
*/

// ************************************************************************************************
// Debug console support

// if console is not defined, e.g., Firebug console is not enabled or incompatible browser
// setup a "fake" console to prevent the browser from failing when executing code that uses the console
(function () {
    if (!window.console /*|| !console.firebug*/) {
        var methods = [
     "log", "debug", "info", "warn", "error", "assert",
     "dir", "dirxml", "group", "groupEnd", "time", "timeEnd",
     "count", "trace", "profile", "profileEnd"
  ];
        window.console = {};
        for (var i = 0; i < methods.length; i++) {
            window.console[methods[i]] = function () { };
        }
    }
})();


// ************************************************************************************************
// Drop down menu
// Copyright 2006-2007 javascript-array.com

var timeout    = 500;
var closetimer = 0;
var ddmenuitem = 0;

function jsddm_open() {  
    jsddm_canceltimer();
    jsddm_close();
    ddmenuitem = $(this).find('ul').css('visibility', 'visible');
}

function jsddm_close() {
    if (ddmenuitem) 
        ddmenuitem.css('visibility', 'hidden');
}

function jsddm_timer() {
    closetimer = window.setTimeout(jsddm_close, timeout);
}

function jsddm_canceltimer() {
    if (closetimer) {  
        window.clearTimeout(closetimer);
        closetimer = null;
    }
}

$(document).ready(function () {
    $('#jsddm > li').bind('mouseover', jsddm_open);
    $('#jsddm > li').bind('mouseout', jsddm_timer);
});


// ************************************************************************************************
// Event dispatcher

EventDispatcher:EventDispatcher = function () {
    return {
        _listeners: {},

        register: function (eventName, target, callback) {
            if (!this._listeners[eventName]) {
                this._listeners[eventName] = [];
            }
            for (var index in this._listeners[eventName]) {
                if (this._listeners[eventName][index]['target'] == target) {
                    return; // target already registered for eventName
                }
            }
            this._listeners[eventName].push({
                target: target,
                callback: callback
            });
        },

        unregister: function (eventName, target) {
            if (!this._listeners[eventName]) {
                return;
            }
            for (var index in this._listeners[eventName]) {
                if (this._listeners[eventName][target]) {
                    this._listeners[eventName].splice(index, 1);
                    return;
                }
            }
        },

        unregisterTarget: function (target) {
            for (var eventIndex in this._listeners) {
                for (var targetIndex in this._listeners[eventIndex]) {
                    if (this._listeners[eventIndex][target]) {
                        this._listeners[eventIndex].splice(targetIndex, 1);
                        break;
                    }
                }
            }
        },

        raiseEvent: function (eventName, args) {
            if (!this._listeners[eventName]) {
                return;
            }
            for (var index in this._listeners[eventName]) {
                var target = this._listeners[eventName][index]['target'];
                var callback = this._listeners[eventName][index]['callback'];
                try {
                    callback(args);
                } catch (e) {
                    console.error("Exception raised when invoking event [", eventName, "] on target [", target, "]", e);
                }
            }
        }


    };
};
