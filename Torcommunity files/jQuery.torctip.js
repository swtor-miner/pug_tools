// Shawn Khameneh
// ExtraordinaryThoughts.com

(function (jQuery) {
    jQuery("head").append("<link href='http://torcommunity.com/tooltips.css' type='text/css' rel='stylesheet' />");
    jQuery("body").append("<div id='torctip' />");

    //scan the page and add hover links
    jQuery("a[href^='http://torcommunity.com/db/']").each(function (value) {
        var linkId = value.href.substring(27);
        console.log(linkId);
        //
    });

    // Mouse position
    var currentMousePos = { x: -1, y: -1 };
    jQuery(document).mousemove(function (event) {
        currentMousePos.x = event.pageX;
        currentMousePos.y = event.pageY;
    });

    var methods = {
        init: function (parameters) {
            var defaults = {
                showIcon: false,
                detailed: true,
                link: 'http://torcommunity.com/db/',
                container: "#torctip"
            };

            return this.each(function (e) {
                var element = e;
                var settings = jQuery.extend({}, defaults, parameters);

                this.html = settings.html;
                this.container = settings.container;

                element.init = function () {
                    jQuery(e).mouseover(function () {
                        jQuery(this.container).load(link);

                        // move and deflect the tooltip off the window boundary
                        var _container = jQuery(this.container);
                        var left = currentMousePos.x;
                        var top = currentMousePos.y - jQuery(_container).height() - 32;

                        if (top < 0) top = currentMousePos.y + 32;
                        if (left + 314 > jQuery(document).width()) left = left - jQuery(_container).width() - 32;

                        jQuery(_container).css("left", left).css("top", top);

                        jQuery(this.container).show();
                    });

                    jQuery(e).mouseout(function () {
                        jQuery(this.container).hide();
                    });
                };

                element.beforeShow = function (e) {
                    // try call
                    try { parameters.beforeShow.call(this); }
                    catch (e) { }
                };

                element.init();

            });
        },
        destroy: function (parameters) {
            return jQuery(this).each(function () {
                var $this = jQuery(this);

                $this.removeData('torctip');
            });
        }
    };

    jQuery.fn.torctip = function () {
        var method = arguments[0];

        if (methods[method]) {
            method = methods[method];
            arguments = Array.prototype.slice.call(arguments, 1);
        } else if (typeof (method) == 'object' || !method) {
            method = methods.init;
        } else {
            jQuery.error('Method ' + method + ' does not exist on jQuery.torctip');
            return this;
        }

        return method.apply(this, arguments);

    }


})(jQuery);