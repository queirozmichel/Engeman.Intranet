/**
 * Core script to handle plugins
 */

var Plugins = function() {

	"use strict";

	/**
	 * $.browser for jQuery 1.9
	 */
	var initBrowserDetection = function() {
		$.browser={};(function(){$.browser.msie=false;
		$.browser.version=0;if(navigator.userAgent.match(/MSIE ([0-9]+)\./)){
		$.browser.msie=true;$.browser.version=RegExp.$1;}})();
	}

	/**************************
	 * Tooltips               *
	 **************************/
	var initTooltips = function() {
		// Set default options

		// TODO: $.extend does not work since BS3!

		// This fixes issue #5865
		// (When using tooltips and popovers with the Bootstrap input groups,
		// you'll have to set the container option to avoid unwanted side effects.)
		$.extend(true, $.fn.tooltip.defaults, {
			container: 'body'
		});

		// Use e.g. "#container" as container (instead of "body")
		// if you're experience errors when using Ajax
		$('.bs-tooltip').tooltip({
			container: 'body'
		});
		$('.bs-focus-tooltip').tooltip({
			trigger: 'focus',
			container: 'body'
		});
	}

	/**************************
	 * Popovers               *
	 **************************/
	var initPopovers = function() {
		$('.bs-popover').popover();
	}

	/**************************
	 * Noty                   *
	 **************************/
	var initNoty = function() {
		if ($.noty) {
			// Set default options
			$.extend(true, $.noty.defaults, {
				type: 'alert',
				timeout: false,
				maxVisible: 5,
				animation: {
					open: {
						height:'toggle'
					},
					close: {
						height:'toggle'
					},
					easing: 'swing',
					speed: 200
				}
			});
		}
	}

	/**************************
	 * Easy Pie Chart         *
	 **************************/
	var initCircularCharts = function() {
		if ($.easyPieChart) {
			// Set default options
			$.extend(true, $.easyPieChart.defaultOptions, {
				lineCap: 'butt',
				animate: 500,
				barColor: App.getLayoutColorCode('blue')
			});

			// Initialize defaults
			$('.circular-chart').easyPieChart({
				size: 110,
				lineWidth: 10
			});
		}
	}

	/**************************
	 * Flot Defaults          *
	 **************************/
	var defaultPlotOptions = {
		colors: [App.getLayoutColorCode('blue'), App.getLayoutColorCode('red'), App.getLayoutColorCode('green'), App.getLayoutColorCode('purple'), App.getLayoutColorCode('grey'), App.getLayoutColorCode('yellow')],
		legend: {
			show: true,
			labelBoxBorderColor: "", // border color for the little label boxes
			backgroundOpacity: 0.95 // set to 0 to avoid background
		},
		series: {
			points: {
				show: false,
				radius: 3,
				lineWidth: 2, // in pixels
				fill: true,
				fillColor: "#ffffff",
				symbol: "circle" // or callback
			},
			lines: {
				// we don't put in show: false so we can see
				// whether lines were actively disabled
				show: true,
				lineWidth: 2, // in pixels
				fill: false,
				fillColor: { colors: [ { opacity: 0.4 }, { opacity: 0.1 } ] },
			},
			bars: {
				lineWidth: 1, // in pixels
				barWidth: 1, // in units of the x axis
				fill: true,
				fillColor: { colors: [ { opacity: 0.7 }, { opacity: 1 } ] },
				align: "left", // or "center"
				horizontal: false
			},
			pie: {
				show: false,
				radius: 1,
				label: {
					show: false,
					radius: 2/3,
					formatter: function(label, series){
						return '<div style="font-size:8pt;text-align:center;padding:2px;color:white;text-shadow: 0 1px 0 rgba(0, 0, 0, 0.6);">'+label+'<br/>'+Math.round(series.percent)+'%</div>';
					},
					threshold: 0.1
				}
			},
			shadowSize: 0
		},
		grid: {
			show: true,
			borderColor: "#efefef", // set if different from the grid color
			tickColor: "rgba(0,0,0,0.06)", // color for the ticks, e.g. "rgba(0,0,0,0.15)"
			labelMargin: 10, // in pixels
			axisMargin: 8, // in pixels
			borderWidth: 0, // in pixels
			minBorderMargin: 10, // in pixels, null means taken from points radius
			mouseActiveRadius: 5 // how far the mouse can be away to activate an item
		},
		tooltipOpts: {
			defaultTheme: false
		},
		selection: {
			color: App.getLayoutColorCode('blue')
		}
	};

	var defaultPlotWidgetOptions = {
		colors: ['#ffffff'],
		legend: {
			show: false,
			backgroundOpacity: 0
		},
		series: {
			points: {
			}
		},
		grid: {
			tickColor: 'rgba(255, 255, 255, 0.1)',
			color: '#ffffff',
		},
		shadowSize: 1
	};

	/**************************
	 * Circle Dial (Knob)     *
	 **************************/
	var initKnob = function() {
		if ($.fn.knob) {
			$(".knob").knob();

			// All elements, which has no color specified, apply default color
			$('.knob').each(function () {
				if (typeof $(this).attr('data-fgColor') == 'undefined') {
					$(this).trigger('configure', {
						'fgColor': App.getLayoutColorCode('blue'),
						'inputColor': App.getLayoutColorCode('blue')
					});
				}
			});
		}
	}

	/**************************
	 * ColorPicker            *
	 **************************/
	var initColorPicker = function() {
		if ($.fn.colorpicker) {
			$('.bs-colorpicker').colorpicker();
		}
	}

	/**************************
	 * Template               *
	 **************************/
	var initTemplate = function() {
		if ($.fn.template) {
			// Set default options
			$.extend(true, $.fn.template.defaults, {

			});
		}
	}

	return {

		// main function to initiate all plugins
		init: function () {
			initBrowserDetection(); // $.browser for jQuery 1.9
			initTooltips(); // Bootstrap tooltips
			initPopovers(); // Bootstrap popovers
			initNoty(); // Notifications
			initCircularCharts(); // Easy Pie Chart
			initKnob(); // Circle Dial
			initColorPicker(); // Bootstrap ColorPicker
			initTemplate(); // Template
		},

		getFlotDefaults: function() {
			return defaultPlotOptions;
		},

		getFlotWidgetDefaults: function() {
			return $.extend(true, {}, Plugins.getFlotDefaults(), defaultPlotWidgetOptions);
		},

	};

}();