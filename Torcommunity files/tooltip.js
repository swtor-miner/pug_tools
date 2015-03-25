// Only do anything if jQuery isn't defined
if (typeof jQuery == 'undefined') {

	if (typeof $ == 'function') {
		// warning, global var
		thisPageUsingOtherJSLibrary = true;
	}
	
	function getScript(url, success) {
	
		var script     = document.createElement('script');
		     script.src = url;
		
		var head = document.getElementsByTagName('head')[0],
		done = false;
		
		// Attach handlers for all browsers
		script.onload = script.onreadystatechange = function() {
		
			if (!done && (!this.readyState || this.readyState == 'loaded' || this.readyState == 'complete')) {
			
			done = true;
				
				// callback function provided as param
				success();
				
				script.onload = script.onreadystatechange = null;
				head.removeChild(script);
				
			};
		
		};
		
		head.appendChild(script);
	
	};
	
	getScript('https://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js', function() {
	
		if (typeof jQuery=='undefined') {
		
			// Super failsafe - still somehow failed...
		
		} else {
		
			// jQuery loaded! Make sure to use .noConflict just in case
			fancyCode();
			
			if (thisPageUsingOtherJSLibrary) {

				initTooltips();

			} else {

				// Use .noConflict(), then run your jQuery Code

			}
		
		}
	
	});
	
} else { // jQuery was already loaded
	
	initTooltips();
};

function initTooltips() { //Load our script now that jquery is confirmed as loaded
	jQuery.getScript("http://torcommunity.com/db/jQuery.torctip.js");
}