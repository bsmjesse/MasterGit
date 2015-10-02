$(document).bind('mobileinit', function() {
     $.fn.animationComplete = function(callback) {
       if ($.support.cssTransitions) {
         var superfy= "WebKitTransitionEvent" in window ? "webkitAnimationEnd" : "animationend";
         return $(this).one(superfy, callback);
       } else {

         setTimeout(callback, 0);
         return $(this);
       }
     };

   })