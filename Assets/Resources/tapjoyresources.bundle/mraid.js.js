/*  Copyright (c) 2011 The MRAID.org project authors. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

(function() {

  var mraidview = window.mraidview = {};
  var isCustomClose = false;

  mraidview.canProcessBridgeCalls = false;

  /****************************************************/
  /********** PROPERTIES OF THE MRAID BRIDGE **********/
  /****************************************************/

  /** Expand Properties */
  var expandProperties = {
    useBackground: false,
    backgroundColor: '#ffffff',
    backgroundOpacity: 1.0,
    lockOrientation: false,
    width: null,
    height: null,
    useCustomClose: false,
    isModal: true
  };

  /** Mraid Resize Properties */
  var resizeProperties = {
    width: null,
    height: null,
    customClosePosition: null,
    offsetX: null,
    offsetY: null,
    allowOffscreen: null
  };
	
	/** Mraid Orientation Properties */
  var orientationProperties = {
    allowOrientationChange: true,
    forceOrientation: "none"
  };

  /** The set of listeners for MRAID Native Bridge Events */
  var listeners = {};

  /** A Queue of Calls to the Native SDK that still need execution */
  var nativeCallQueue = [];

  /** Identifies if a native call is currently in progress */
  var nativeCallInFlight = false;


  /**********************************************/
  /********** OBJECTIVE-C ENTRY POINTS **********/
  /**********************************************/

  /**
   * Called by the Objective-C SDK when an asset has been fully cached.
   *
   * @returns string, "OK"
   */
  mraidview.fireAssetReadyEvent = function(alias, URL) {
    var handlers = listeners["assetReady"];
    if (handlers != null) {
      for (var i = 0; i < handlers.length; i++) {
        handlers[i](alias, URL);
      }
    }

    return "OK";
  };

  /**
   * Called by the Objective-C SDK when an asset has been removed from the
   * cache at the request of the creative.
   *
   * @returns string, "OK"
   */
  mraidview.fireAssetRemovedEvent = function(alias) {
    var handlers = listeners["assetRemoved"];
    if (handlers != null) {
      for (var i = 0; i < handlers.length; i++) {
        handlers[i](alias);
      }
    }

    return "OK";
  };

  /**
   * Called by the Objective-C SDK when an asset has been automatically
   * removed from the cache for reasons outside the control of the creative.
   *
   * @returns string, "OK"
   */
  mraidview.fireAssetRetiredEvent = function(alias) {
    var handlers = listeners["assetRetired"];
    if (handlers != null) {
      for (var i = 0; i < handlers.length; i++) {
        handlers[i](alias);
      }
    }

    return "OK";
  };

  /**
   * Called by the Objective-C SDK when various state properties have changed.
   *
   * @returns string, "OK"
   */
  mraidview.fireChangeEvent = function(properties) {
    var handlers = listeners["change"];
    if (handlers != null) {
      for (var i = 0; i < handlers.length; i++) {
        handlers[i](properties);
      }
    }

    return "OK";
  };

  /**
   * Called by the Objective-C SDK when an error has occured.
   *
   * @returns string, "OK"
   */
  mraidview.fireErrorEvent = function(message, action) {
    var handlers = listeners["error"];
    if (handlers != null) {
      for (var i = 0; i < handlers.length; i++) {
        handlers[i](message, action);
      }
    }

    return "OK";
  };

  /**
   * Called by the Objective-C SDK when the user shakes the device.
   *
   * @returns string, "OK"
   */
  mraidview.fireShakeEvent = function() {
    var handlers = listeners["shake"];
    if (handlers != null) {
      for (var i = 0; i < handlers.length; i++) {
        handlers[i]();
      }
    }

    return "OK";
  };

  /**
   * nativeCallComplete notifies the abstraction layer that a native call has
   * been completed..
   *
   * NOTE: This function is called by the native code and is not intended to be
   *       used by anything else.
   *
   * @returns string, "OK"
   */
  mraidview.nativeCallComplete = function(cmd) {

    // anything left to do?
    if (nativeCallQueue.length == 0) {
      nativeCallInFlight = false;
      return;
    }

    // still have something to do
    var bridgeCall = nativeCallQueue.pop();
    window.location = bridgeCall;

    return "OK";
  };

  mraidview.showAlert = function(message) {
    alert(message);
  };

  /*********************************************/
  /********** INTERNALLY USED METHODS **********/
  /*********************************************/

  /**
   *
   */
  mraidview.zeroPad = function(number) {
    var text = "";
    if (number < 10) {
      text += "0";
    }
    text += number;
    return text;
  }

  /**
   *
   */
  mraidview.executeNativeCall = function(command) {
    // build iOS command
    var bridgeCall = "mraid://" + command;
    var value;
    var firstArg = true;
    for (var i = 1; i < arguments.length; i += 2) {
      value = arguments[i + 1];
      if (value == null) {
        // no value, ignore the property
        continue;
      }

      // add the correct separator to the name/value pairs
      if (firstArg) {
        bridgeCall += "?";
        firstArg = false;
      } else {
        bridgeCall += "&";
      }
      bridgeCall += arguments[i] + "=" + escape(value);
    }

    // add call to queue
    if (!mraidview.canProcessBridgeCalls) {
      return;
    } else if (nativeCallInFlight) {
      // call pending, queue up request
      nativeCallQueue.push(bridgeCall);
    } else {
      // no call currently in process, execute it directly
      nativeCallInFlight = true;
      window.location = bridgeCall;
    }
  };

  /***************************************************************************/
  /********** LEVEL 0 (not part of spec, but required by public API **********/
  /***************************************************************************/

  /**
   *
   */
  mraidview.activate = function(event) {
    this.executeNativeCall("service", "name", event, "enabled", "Y");
  };

  /**
   *
   */
  mraidview.addEventListener = function(event, listener) {
    var handlers = listeners[event];
    if (handlers == null) {
      // no handlers defined yet, set it up
      listeners[event] = [];
      handlers = listeners[event];
    }

    // see if the listener is already present
    for (var handler in handlers) {
      if (listener == handler) {
        // listener already present, nothing to do
        return;
      }
    }

    // not present yet, go ahead and add it
    handlers.push(listener);
  };

  /**
   *
   */
  mraidview.deactivate = function(event) {
    this.executeNativeCall("service", "name", event, "enabled", "N");
  };

  /**
   *
   */
  mraidview.removeEventListener = function(event, listener) {
    var handlers = listeners[event];
    if (handlers != null) {
      handlers.remove(listener);
    }
  };

  /*****************************/
  /********** LEVEL 1 **********/
  /*****************************/

  /**
   *
   */
  mraidview.close = function() {
    this.executeNativeCall("close");
  };

  /**
   *
   */
  mraidview.expand = function(dimensions, URL) {
    try {
      var cmd = "this.executeNativeCall( 'expand'";
      if (URL != null) {
        cmd += ", 'url', '" + URL + "'";
      }
      if ((typeof dimensions.x != "undefined") && (dimensions.x != null)) {
        cmd += ", 'x', '" + dimensions.x + "'";
      }
      if ((typeof dimensions.y != "undefined") && (dimensions.y != null)) {
        cmd += ", 'y', '" + dimensions.y + "'";
      }
      if ((typeof dimensions.width != "undefined") && (dimensions.width != null)) {
        cmd += ", 'w', '" + dimensions.width + "'";
      }
      if ((typeof dimensions.height != "undefined") && (dimensions.height != null)) {
        cmd += ", 'h', '" + dimensions.height + "'";
      }
      if ((typeof expandProperties.useBackground != "undefined") && (expandProperties.useBackground != null)) {
        cmd += ", 'useBG', '" + (expandProperties.useBackground ? "Y" : "N") + "'";
      }
      if ((typeof expandProperties.backgroundColor != "undefined") && (expandProperties.backgroundColor != null)) {
        cmd += ", 'bgColor', '" + expandProperties.backgroundColor + "'";
      }
      if ((typeof expandProperties.backgroundOpacity != "undefined") && (expandProperties.backgroundOpacity != null)) {
        cmd += ", 'bgOpacity', " + expandProperties.backgroundOpacity;
      }
      if ((typeof expandProperties.lockOrientation != "undefined") && (expandProperties.lockOrientation != null)) {
        cmd += ", 'lkOrientation', '" + (expandProperties.lockOrientation ? "Y" : "N") + "'";
      }
      if ((typeof expandProperties.useCustomClose != "undefined") && (expandProperties.useCustomClose != null)) {
        cmd += ", 'useCustomClose', '" + (expandProperties.useCustomClose ? "Y" : "N") + "'";
      } else {
        cmd += ", 'useCustomClose', '" + "N" + "'";
      }

      cmd += " );";
      eval(cmd);
    } catch (e) {
      alert("executeNativeExpand: " + e + ", cmd = " + cmd);
    }
  };

  /**
   *
   */
  mraidview.hide = function() {
    this.executeNativeCall("hide");
  };

  /**
   *
   */
  mraidview.useCustomClose = function(boolean) {
    isCustomClose = boolean;
    var isCustomClose = "N";
    if (boolean) {
      isCustomClose = "Y";
    }
    expandProperties.useCustomClose = boolean;
    this.executeNativeCall("allowCustomClose", "isCustomClose", isCustomClose);
		
		if(mraid.getPlacementType() == "interstitial" || mraid.getState() == "expanded")
			document.getElementById('button').style.visibility = boolean ? 'hidden' : 'visible';
  };

  /**
   *
   */
  mraidview.open = function(URL, controls) {
    // the navigation parameter is an array, break it into its parts
    var back = false;
    var forward = false;
    var refresh = false;
    if (controls == null) {
      back = true;
      forward = true;
      refresh = true;
    } else {
      for (var i = 0; i < controls.length; i++) {
        if ((controls[i] == "none") && (i > 0)) {
          // error
          self.fireErrorEvent("none must be the only navigation element present.", "open");
          return;
        } else if (controls[i] == "all") {
          if (i > 0) {
            // error
            self.fireErrorEvent("none must be the only navigation element present.", "open");
            return;
          }

          // ok
          back = true;
          forward = true;
          refresh = true;
        } else if (controls[i] == "back") {
          back = true;
        } else if (controls[i] == "forward") {
          forward = true;
        } else if (controls[i] == "refresh") {
          refresh = true;
        }
      }
    }

    this.executeNativeCall("open", "url", URL, "back", (back ? "Y" : "N"), "forward", (forward ? "Y" : "N"), "refresh", (refresh ? "Y" : "N"));
  };

  /**
   *
   */
  mraidview.openMap = function(URL, fullscreen) {

    this.executeNativeCall("openMap", "url", URL, "fullscreen", fullscreen);
  };

  /**
   *
   */
  mraidview.resize = function() {
    var allowOffscreen = "N";
    if (resizeProperties.allowOffscreen) {
      allowOffscreen = "Y";
    }
    this.executeNativeCall("resize", "w", resizeProperties.width, "h", resizeProperties.height, "customClosePosition", resizeProperties.customClosePosition, "offsetX", resizeProperties.offsetX, "offsetY", resizeProperties.offsetY, "allowOffscreen", resizeProperties.allowOffscreen);
  };

  mraidview.getExpandProperties = function() {
    return expandProperties;
  }

  mraidview.getResizeProperties = function() {
    return resizeProperties;
  }

  mraidview.getOrientationProperties = function() {
    return orientationProperties;
  }

  /**
   *
   */
  mraidview.setExpandProperties = function(properties) {
    expandProperties = properties;
  };

  mraidview.setResizeProperties = function(properties) {
    resizeProperties = properties;
  };

  mraidview.setOrientationProperties = function(properties) {
    orientationProperties = {
      allowOrientationChange: properties.allowOrientationChange == true ? true : false,
      forceOrientation: ["portrait", "landscape"].indexOf(properties.forceOrientation) == -1 ? "none" : properties.forceOrientation
    };
		
		var args = [ "setorientationproperties" ];
    args.push( "allowOrientationChange", orientationProperties.allowOrientationChange );
    args.push( "forceOrientation", orientationProperties.forceOrientation );

    this.executeNativeCall.apply(this, args);
  };

  /**
   *
   */
  mraidview.show = function() {
    this.executeNativeCall("show");
  };

  /**
   *
   */
  mraidview.playAudio = function(URL, properties) {

    var cmd = "this.executeNativeCall( 'playAudio'";

    cmd += ", 'url', '" + URL + "'";

    if (properties != null) {

      if ((typeof properties.autoplay != "undefined") && (properties.autoplay != null)) {
        cmd += ", 'autoplay', 'Y'";
      } else {
        cmd += ", 'autoplay', 'N'";
      }

      if ((typeof properties.controls != "undefined") && (properties.controls != null)) {
        cmd += ", 'controls', 'Y'";
      } else {
        cmd += ", 'controls', 'N'";
      }

      if ((typeof properties.loop != "undefined") && (properties.loop != null)) {
        cmd += ", 'loop', 'Y'";
      } else {
        cmd += ", 'loop', 'N'";
      }

      if ((typeof properties.position != "undefined") && (properties.position != null)) {
        cmd += ", 'position', 'Y'";
      } else {
        cmd += ", 'position', 'N'";
      }

      //TODO check valid values...
      if ((typeof properties.startStyle != "undefined") && (properties.startStyle != null)) {
        cmd += ", 'startStyle', '" + properties.startStyle + "'";
      } else {
        cmd += ", 'startStyle', 'normal'";
      }

      if ((typeof properties.stopStyle != "undefined") && (properties.stopStyle != null)) {
        cmd += ", 'stopStyle', '" + properties.stopStyle + "'";
      } else {
        cmd += ", 'stopStyle', 'normal'";
      }

    }

    cmd += " );";

    eval(cmd);
  };

  /**
   *
   */
  mraidview.playVideo = function(URL, properties) {
    var cmd = "this.executeNativeCall( 'playVideo'";

    cmd += ", 'url', '" + URL + "'";

    if (properties != null) {

      if ((typeof properties.audio != "undefined") && (properties.audio != null)) {
        cmd += ", 'audioMuted', 'Y'";
      } else {
        cmd += ", 'audioMuted', 'N'";
      }

      if ((typeof properties.autoplay != "undefined") && (properties.autoplay != null)) {
        cmd += ", 'autoplay', 'Y'";
      } else {
        cmd += ", 'autoplay', 'N'";
      }

      if ((typeof properties.controls != "undefined") && (properties.controls != null)) {
        cmd += ", 'controls', 'Y'";
      } else {
        cmd += ", 'controls', 'N'";
      }

      if ((typeof properties.loop != "undefined") && (properties.loop != null)) {
        cmd += ", 'loop', 'Y'";
      } else {
        cmd += ", 'loop', 'N'";
      }

      if ((typeof properties.position != "undefined") && (properties.position != null)) {
        cmd += ", 'position_top', '" + properties.position.top + "'";
        cmd += ", 'position_left', '" + properties.position.left + "'";

        if ((typeof properties.width != "undefined") && (properties.width != null)) {
          cmd += ", 'position_width', '" + properties.width + "'";
        } else {
          //TODO ERROR
        }

        if ((typeof properties.height != "undefined") && (properties.height != null)) {
          cmd += ", 'position_height', '" + properties.height + "'";
        } else {
          //TODO ERROR
        }
      }

      if ((typeof properties.startStyle != "undefined") && (properties.startStyle != null)) {
        cmd += ", 'startStyle', '" + properties.startStyle + "'";
      } else {
        cmd += ", 'startStyle', 'normal'";
      }

      if ((typeof properties.stopStyle != "undefined") && (properties.stopStyle != null)) {
        cmd += ", 'stopStyle', '" + properties.stopStyle + "'";
      } else {
        cmd += ", 'stopStyle', 'normal'";
      }
    }

    cmd += " );";

    eval(cmd);
  };

  /*****************************/
  /********** LEVEL 2 **********/
  /*****************************/

  /**
   *
   */
  mraidview.createEvent = function(date, title, body) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hours = date.getHours();
    var minutes = date.getMinutes();


    var dateString = year + this.zeroPad(month) + this.zeroPad(day) + this.zeroPad(hours) + this.zeroPad(minutes);
    this.executeNativeCall("calendar", "date", dateString, "title", title, "body", body);
  };

  /**
   *
   */
  mraidview.createCalendarEvent = function(eventEntry) {
    var id = eventEntry.id;
    var body = eventEntry.description;
    var location = eventEntry.location;
    var title = eventEntry.summary;
    var StringDate = eventEntry.start;
    var StringEnd = eventEntry.end;
    var status = eventEntry.status;
    var transparency = eventEntry.transparency;
    var recurrence = eventEntry.recurrence;
    var reminder = eventEntry.reminder;

    // parse start date
    var date = new Date(Date.parse(StringDate));
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var dateString = year + this.zeroPad(month) + this.zeroPad(day) + this.zeroPad(hours) + this.zeroPad(minutes);

    // parse end date
    var endDate = new Date(Date.parse(StringEnd));
    var eyear = endDate.getFullYear();
    var emonth = endDate.getMonth() + 1;
    var eday = endDate.getDate();
    var ehours = endDate.getHours();
    var eminutes = endDate.getMinutes();
    var endDateString = eyear + this.zeroPad(emonth) + this.zeroPad(eday) + this.zeroPad(ehours) + this.zeroPad(eminutes);


    this.executeNativeCall("calendar", "id", id, "title", title, "location", location, "body", body, "date", dateString, "end", endDateString, "status", status, "transparency", transparency, "recurrence", recurrence, "reminder", reminder);
  };

  /**
   *
   */
  mraidview.makeCall = function(phoneNumber) {
    this.executeNativeCall("phone", "number", phoneNumber);
  };

  /**
   *
   */
  mraidview.sendMail = function(recipient, subject, body) {
    this.executeNativeCall("email", "to", recipient, "subject", subject, "body", body, "html", "N");
  };

  /**
   *
   */
  mraidview.sendSMS = function(recipient, body) {
    this.executeNativeCall("sms", "to", recipient, "body", body);
  };

  /**
   *
   */
  mraidview.setShakeProperties = function(properties) {};

  /*****************************/
  /********** LEVEL 3 **********/
  /*****************************/

  /**
   *
   */
  mraidview.addAsset = function(URL, alias) {
    this.executeNativeCall("addasset", "uri", url, "alias", alias);
  };

  /**
   *
   */
  mraidview.request = function(URI, display) {
    this.executeNativeCall("request", "uri", uri, "display", display);
  };

  /**
   *
   */
  mraidview.removeAsset = function(alias) {
    this.executeNativeCall("removeasset", "alias", alias);
  };
})();

/**
 *
 */
mraidview.storePicture = function(URL) {
  this.executeNativeCall("storePicture", "url", URL);
};

/*  Copyright (c) 2011 The MRAID.org project authors. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

(function() {
  var mraid = window.mraid = {};

  // CONSTANTS
  var STATES = mraid.STATES = {
    LOADING: 'loading',
    DEFAULT: 'default',
    RESIZED: 'resized',
    EXPANDED: 'expanded',
    HIDDEN: 'hidden'
  };

  var EVENTS = mraid.EVENTS = {
    ASSETREADY: 'assetReady',
    ASSETREMOVED: 'assetRemoved',
    ASSETRETIRED: 'assetRetired',
    ERROR: 'error',
    INFO: 'info',
    READY: 'ready',
    HEADINGCHANGE: 'headingChange',
    KEYBOARDCHANGE: 'keyboardChange',
    LOCATIONCHANGE: 'locationChange',
    NETWORKCHANGE: 'networkChange',
    ORIENTATIONCHANGE: 'orientationChange',
    RESPONSE: 'response',
    SCREENCHANGE: 'screenChange',
    SHAKE: 'shake',
    SIZECHANGE: 'sizeChange',
    STATECHANGE: 'stateChange',
    TILTCHANGE: 'tiltChange',
    VIEWABLECHANGE: 'viewableChange'
  };

  var CONTROLS = mraid.CONTROLS = {
    BACK: 'back',
    FORWARD: 'forward',
    REFRESH: 'refresh',
    ALL: 'all'
  };

  var FEATURES = mraid.FEATURES = {
    LEVEL1: 'level-1',
    LEVEL2: 'level-2',
    LEVEL3: 'level-3',
    SCREEN: 'screen',
    ORIENTATION: 'orientation',
    HEADING: 'heading',
    LOCATION: 'location',
    SHAKE: 'shake',
    TILT: 'tilt',
    NETWORK: 'network',
    SMS: 'sms',
    PHONE: 'phone',
    EMAIL: 'email',
    CALENDAR: 'calendar',
    CAMERA: 'camera',
    AUDIO: 'audio',
    VIDEO: 'video',
    MAP: 'map',
    STOREPICTURE: 'storePicture'
  };

  var NETWORK = mraid.NETWORK = {
    OFFLINE: 'offline',
    WIFI: 'wifi',
    CELL: 'cell',
    UNKNOWN: 'unknown'
  };

  // PRIVATE PROPERTIES (sdk controlled)
  var MRAID_VERSION = "2.0";

  var customClose = false;

  var state = STATES.LOADING;

  var placementType = '';

  var size = {
    width: 0,
    height: 0
  };

  var defaultPosition = {
    x: 0,
    y: 0,
    width: 0,
    height: 0
  };

  var currentPosition = {
    x: 0,
    y: 0,
    width: 0,
    height: 0
  };

  var maxSize = {
    width: 0,
    height: 0
  };

  var supports = {
    'level-1': true,
    'level-2': true,
    'level-3': true,
    'screen': true,
    'orientation': true,
    'heading': true,
    'location': true,
    'shake': true,
    'tilt': true,
    'network': true,
    'sms': true,
    'phone': true,
    'email': true,
    'calendar': true,
    'camera': true,
    'audio': true,
    'video': true,
    'map': true,
    'storePicture': true
  };

  var heading = -1;

  var keyboardState = false;

  var location = null;

  var network = NETWORK.UNKNOWN;

  var orientation = -1;

  var screenSize = null;

  var shakeProperties = null;

  var tilt = null;

  var assets = {};

  var cacheRemaining = -1;

  var viewable = false;

  // PRIVATE PROPERTIES (internal)
  var intervalID = null;
  var readyTimeout = 10000;
  var readyDuration = 0;

  function trim(str) {
    return str.replace(/^\s+|\s+$/g, "");
  }

  var dimensionValidators = {
    x: function(value) {
      return !isNaN(value) && value >= 0;
    },
    y: function(value) {
      return !isNaN(value) && value >= 0;
    },
    width: function(value) {
      return !isNaN(value) && value >= 0
    },
    height: function(value) {
      return !isNaN(value) && value >= 0
    }
  };

  var expandPropertyValidators = {
    useBackground: function(value) {
      return (value === true || value === false);
    },
    backgroundColor: function(value) {
      return (typeof value == 'string' && value.substr(0, 1) == '#' && !isNaN(parseInt(value.substr(1), 16)));
    },
    backgroundOpacity: function(value) {
      return !isNaN(value) && value >= 0 && value <= 1;
    },
    lockOrientation: function(value) {
      return (value === true || value === false);
    }
  };

  var shakePropertyValidators = {
    intensity: function(value) {
      return !isNaN(value);
    },
    interval: function(value) {
      return !isNaN(value);
    }
  };

  var changeHandlers = {
    state: function(val) {
      if (state == STATES.LOADING) {
        mraidview.canProcessBridgeCalls = true;
        broadcastEvent(EVENTS.INFO, 'controller initialized, attempting callback');
      }
      broadcastEvent(EVENTS.INFO, 'setting state to ' + stringify(val));
      state = val;
      broadcastEvent(EVENTS.STATECHANGE, state);
    },
    placementType: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting placementType to ' + stringify(val));
      placementType = val;
    },
    size: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting size to ' + stringify(val));
      size = val;
      broadcastEvent(EVENTS.SIZECHANGE, size.width, size.height);
    },
    defaultPosition: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting default position to ' + stringify(val));
      defaultPosition = val;
    },
    currentPosition: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting current position to ' + stringify(val));
      currentPosition = val;
    },
    maxSize: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting maxSize to ' + stringify(val));
      maxSize = val;
    },
    expandProperties: function(val) {
      broadcastEvent(EVENTS.INFO, 'merging expandProperties with ' + stringify(val));
      for (var i in val) {
        expandProperties[i] = val[i];
      }
    },
    supports: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting supports to ' + stringify(val));
      supports = {};
      for (var key in FEATURES) {
        supports[FEATURES[key]] = contains(FEATURES[key], val);
      }
    },
    heading: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting heading to ' + stringify(val));
      heading = val;
      broadcastEvent(EVENTS.HEADINGCHANGE, heading);
    },
    keyboardState: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting keyboardState to ' + stringify(val));
      keyboardState = val;
      broadcastEvent(EVENTS.KEYBOARDCHANGE, keyboardState);
    },
    location: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting location to ' + stringify(val));
      location = val;
      broadcastEvent(EVENTS.LOCATIONCHANGE, location.lat, location.lon, location.acc);
    },
    network: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting network to ' + stringify(val));
      network = val;
      broadcastEvent(EVENTS.NETWORKCHANGE, (network != NETWORK.OFFLINE && network != NETWORK.UNKNOWN), network);
    },
    orientation: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting orientation to ' + stringify(val));
      orientation = val;
      broadcastEvent(EVENTS.ORIENTATIONCHANGE, orientation);
    },
    screenSize: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting screenSize to ' + stringify(val));
      screenSize = val;
      broadcastEvent(EVENTS.SCREENCHANGE, screenSize.width, screenSize.height);
    },
    shakeProperties: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting shakeProperties to ' + stringify(val));
      shakeProperties = val;
    },
    tilt: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting tilt to ' + stringify(val));
      tilt = val;
      broadcastEvent(EVENTS.TILTCHANGE, tilt.x, tilt.y, tilt.z);
    },
    cacheRemaining: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting cacheRemaining to ' + stringify(val));
      cacheRemaining = val;
    },
    viewable: function(val) {
      broadcastEvent(EVENTS.INFO, 'setting viewable to ' + stringify(val));
      viewable = val;
      broadcastEvent(EVENTS.VIEWABLECHANGE, viewable);
    }
  };

  var listeners = {};

  var EventListeners = function(event) {
    this.event = event;
    this.count = 0;
    var listeners = {};

    this.add = function(func) {
      var id = String(func);
      if (!listeners[id]) {
        listeners[id] = func;
        this.count++;
        if (this.count == 1) mraidview.activate(event);
      }
    };
    this.remove = function(func) {
      var id = String(func);
      if (listeners[id]) {
        listeners[id] = null;
        delete listeners[id];
        this.count--;
        if (this.count == 0) mraidview.deactivate(event);
        return true;
      } else {
        return false;
      }
    };
    this.removeAll = function() {
      for (var id in listeners) this.remove(listeners[id]);
    };
    this.broadcast = function(args) {
      for (var id in listeners) listeners[id].apply({}, args);
    };
    this.toString = function() {
      var out = [event, ':'];
      for (var id in listeners) out.push('|', id, '|');
      return out.join('');
    };
  };

  // PRIVATE METHODS ////////////////////////////////////////////////////////////
  mraidview.addEventListener('change', function(properties) {
    for (var property in properties) {
      var handler = changeHandlers[property];
      handler(properties[property]);
    }
  });

  mraidview.addEventListener('shake', function() {
    broadcastEvent(EVENTS.SHAKE);
  });

  mraidview.addEventListener('error', function(message, action) {
    broadcastEvent(EVENTS.ERROR, message, action);
  });

  mraidview.addEventListener('response', function(uri, response) {
    broadcastEvent(EVENTS.RESPONSE, uri, response);
  });

  mraidview.addEventListener('assetReady', function(alias, URL) {
    assets[alias] = URL;
    broadcastEvent(EVENTS.ASSETREADY, alias);
  });

  mraidview.addEventListener('assetRemoved', function(alias) {
    assets[alias] = null;
    delete assets[alias];
    broadcastEvent(EVENTS.ASSETREMOVED, alias);
  });

  mraidview.addEventListener('assetRetired', function(alias) {
    assets[alias] = null;
    delete assets[alias];
    broadcastEvent(EVENTS.ASSETRETIRED, alias);
  });

  var clone = function(obj) {
    var f = function() {};
    f.prototype = obj;
    return new f();
  };

  var stringify = function(obj) {
    if (typeof obj == 'object') {
      if (obj.push) {
        var out = [];
        for (var p in obj) {
          out.push(obj[p]);
        }
        return '[' + out.join(',') + ']';
      } else {
        var out = [];
        for (var p in obj) {
          out.push('\'' + p + '\':' + obj[p]);
        }
        return '{' + out.join(',') + '}';
      }
    } else {
      return String(obj);
    }
  };

  var valid = function(obj, validators, action, full) {
    if (full) {
      if (obj === undefined) {
        broadcastEvent(EVENTS.ERROR, 'Required object missing.', action);
        return false;
      } else {
        for (var i in validators) {
          if (obj[i] === undefined) {
            broadcastEvent(EVENTS.ERROR, 'Object missing required property ' + i, action);
            return false;
          }
        }
      }
    }
    for (var i in obj) {
      if (!validators[i]) {
        broadcastEvent(EVENTS.ERROR, 'Invalid property specified - ' + i + '.', action);
        return false;
      } else if (!validators[i](obj[i])) {
        broadcastEvent(EVENTS.ERROR, 'Value of property ' + i + ' is not valid type.', action);
        return false;
      }
    }
    return true;
  };

  var contains = function(value, array) {
    for (var i in array) if (array[i] == value) return true;
    return false;
  };

  var broadcastEvent = function() {
    var args = new Array(arguments.length);
    for (var i = 0; i < arguments.length; i++) args[i] = arguments[i];
    var event = args.shift();
    if (listeners[event]) listeners[event].broadcast(args);
  }

  insertCss = function() {
    var script = document.createElement('style');
    script.type = 'text/css';
    script.innerHTML = '.button { -webkit-appearance:none;position:fixed;top:1px;right:1px;z-index: 99999999999;border: 2px solid #fff;background: #111;padding: 1px 5px;-webkit-border-radius: 24px;-moz-border-radius: 24px;border-radius: 24px;-webkitbox-shadow: rgba(0,0,0,1) 1px 1px 1px;-moz-box-shadow: rgba(0,0,0,1) 1px 1px 1px;box-shadow: rgba(0,0,0,1) 1px 1px 1px;text-shadow: rgba(0,0,0,.4) 0 1px 0;color: white;font-size: 18px;font-family: Georgia, serif;text-decoration: none;vertical-align: middle;} .button:active {border-color: #ccc;}';

    document.getElementsByTagName('body')[0].appendChild(script);
    var closeBtn = document.createElement('input');
    closeBtn.type = "button";
    closeBtn.value = "\u2716";
    closeBtn.name = "button";
    closeBtn.id = "button";
    closeBtn.className = "button";
    closeBtn.addEventListener('click', function() { mraid.close(); });

    if (placementType != "interstitial" || customClose) {
      closeBtn.style.opacity = '0.0';
      closeBtn.style.visibility = 'hidden';
    } 
		
    document.getElementsByTagName('body')[0].appendChild(closeBtn);

		mraidview.useCustomClose(customClose);
  }

  // MRAID LEVEL 1
  mraid.addEventListener = function(event, listener) {
    if (!event || !listener) {
      broadcastEvent(EVENTS.ERROR, 'Both event and listener are required.', 'addEventListener');
    } else if (!contains(event, EVENTS)) {
      broadcastEvent(EVENTS.ERROR, 'Unknown event: ' + event, 'addEventListener');
    } else {
      if (!listeners[event]) listeners[event] = new EventListeners(event);
      listeners[event].add(listener);
    }
  };

  mraid.close = function() {
    mraidview.close();
  };

  mraid.expand = function(URL) {
    var dimensions = getDimensions();
    mraid.expandvalidator(dimensions, URL);
  };

  // private helper method
  getDimensions = function() {
    var mraidwidth = mraid.getExpandProperties().width;
    var mraidheight = mraid.getExpandProperties().height;
    if ((mraidwidth == null) || (mraidheight == null)) {
      mraidwidth = mraid.getMaxSize().width;
      mraidheight = mraid.getMaxSize().height;
    }
    return {
      "x": mraid.getDefaultPosition().x,
      "y": mraid.getDefaultPosition().y,
      "width": mraidwidth,
      "height": mraidheight
    };
  };

  // private helper method
  mraid.expandvalidator = function(dimensions, URL) {
    broadcastEvent(EVENTS.INFO, 'expanding to ' + stringify(dimensions));
    if (valid(dimensions, dimensionValidators, 'expand', true)) {
      mraidview.expand(dimensions, URL);
    }
  };

  mraid.getExpandProperties = function() {
    var properties = clone(mraidview.getExpandProperties());

    return {
      useBackground: properties.useBackground,
      backgroundColor: properties.backgroundColor,
      backgroundOpacity: properties.backgroundOpacity,
      lockOrientation: properties.lockOrientation,
      width: properties.width,
      height: properties.height,
      useCustomClose: properties.useCustomClose,
      isModal: properties.isModal
    };
  };

  mraid.getPlacementType = function() {
    return placementType;
  };

  mraid.getState = function() {
    return state;
  };

  mraid.getVersion = function() {
    return MRAID_VERSION;
  };

  mraid.isViewable = function() {
    return viewable;
  };

  mraid.open = function(URL, controls) {
    if (!URL) {
      broadcastEvent(EVENTS.ERROR, 'URL is required.', 'open');
    } else {
      mraidview.open(URL, controls);
    }
  };

  mraid.removeEventListener = function(event, listener) {
    if (!event) {
      broadcastEvent(EVENTS.ERROR, 'Must specify an event.', 'removeEventListener');
    } else {
      if (listener && (!listeners[event] || !listeners[event].remove(listener))) {
        broadcastEvent(EVENTS.ERROR, 'Listener not currently registered for event', 'removeEventListener');
        return;
      } else if (listeners[event]) {
        listeners[event].removeAll();
      }

      if (listeners[event] && listeners[event].count == 0) {
        listeners[event] = null;
        delete listeners[event];
      }
    }
  };

  mraid.resize = function() {
    var width = mraid.getResizeProperties().width;
    var height = mraid.getResizeProperties().height;
    if (width == null || height == null || isNaN(width) || isNaN(height) || width < 0 || height < 0) {
      broadcastEvent(EVENTS.ERROR, 'Requested size must be numeric values between 0 and maxSize.', 'resize');
    } else if (width > maxSize.width || height > maxSize.height) {
      broadcastEvent(EVENTS.ERROR, 'Request (' + width + ' x ' + height + ') exceeds maximum allowable size of (' + maxSize.width + ' x ' + maxSize.height + ')', 'resize');
    } else if (width == size.width && height == size.height) {
      broadcastEvent(EVENTS.ERROR, 'Requested size equals current size.', 'resize');
    } else {
      mraidview.resize();
    }
  };

  mraid.setExpandProperties = function(properties) {
    mraidview.setExpandProperties(properties);
  };

  mraid.useCustomClose = function(boolean) {
    customClose = boolean;
    mraidview.useCustomClose(boolean);
  };

  // MRIAD LEVEL 1 extension for ORMMA LEVEL 1 standards
  mraid.signalReady = function() {
    broadcastEvent(EVENTS.READY);
  };

  mraid.getCurrentPosition = function() {
    return clone(currentPosition);
  }

  mraid.getDefaultPosition = function() {
    return clone(defaultPosition);
  };

  mraid.getSize = function() {
    return clone(size);
  };

  mraid.hide = function() {
    if (state == STATES.HIDDEN) {
      broadcastEvent(EVENTS.ERROR, 'Ad is currently hidden.', 'hide');
    } else {
      mraidview.hide();
    }
  };

  mraid.openMap = function(POI, fullscreen) {
    if (!POI) {
      broadcastEvent(EVENTS.ERROR, 'POI is required.', 'openMap');
    } else {
      mraidview.openMap(POI, fullscreen);
    }
  };

  mraid.show = function() {
    if (state != STATES.HIDDEN) {
      broadcastEvent(EVENTS.ERROR, 'Ad is currently visible.', 'show');
    } else {
      mraidview.show();
    }
  };

  mraid.playAudio = function(URL, properties) {
    if (!supports[FEATURES.AUDIO]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'playAudio');
    } else if (!URL || typeof URL != 'string') {
      broadcastEvent(EVENTS.ERROR, 'Request must specify a URL', 'playAudio');
    } else {
      mraidview.playAudio(URL, properties);
    }
  };

  // MRAID LEVEL 2 MRAID that is in ORMMA LEVEL 1
  mraid.getMaxSize = function() {
    return clone(maxSize);
  };

  mraid.getResizeProperties = function() {
    return clone(mraidview.getResizeProperties());
  };

  mraid.setResizeProperties = function(properties) {
    mraidview.setResizeProperties(properties);
  };

  mraid.playVideo = function(URL, properties) {
    if (properties == null) {
      properties = {
        audio: false,
        automatically: true,
        controls: true,
        loop: false
      };
    }

    if (!supports[FEATURES.VIDEO]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'playVideo');
    } else if (!URL || typeof URL != 'string') {
      broadcastEvent(EVENTS.ERROR, 'Request must specify a URL', 'playVideo');
    } else {
      mraidview.playVideo(URL, properties);
    }
  };

  // MRAID LEVEL 2
  mraid.setOrientationProperties = function(properties) {
    mraidview.setOrientationProperties(properties);
  };

  mraid.getOrientationProperties = function() {
    return mraidview.getOrientationProperties();
  };

  mraid.createCalendarEvent = function(eventEntry) {
    mraidview.createCalendarEvent(eventEntry);
  };

  mraid.getScreenSize = function() {
    if (!supports[FEATURES.SCREEN]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getScreenSize');
    } else {
      return (null == screenSize) ? null : clone(screenSize);
    }
  };

  mraid.storePicture = function(URL) {
    mraidview.storePicture(URL);
  };

  mraid.supports = function(feature) {
    if (supports[feature]) {
      return true;
    } else {
      return false;
    }
  };

  // MRIAD level 2 extension for ormma level 2 standards ///////////////////////////////////////////
  mraid.createEvent = function(date, title, body) {
    title = trim(title);
    body = trim(body);
    if (!supports[FEATURES.CALENDAR]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'createEvent');
    } else if (!date || typeof date != 'object' || !date.getDate) {
      broadcastEvent(EVENTS.ERROR, 'Valid date required.', 'createEvent');
    } else if (!title || typeof title != 'string' || title.length == 0) {
      broadcastEvent(EVENTS.ERROR, 'Valid title required.', 'createEvent');
    } else if (!body || typeof body != 'string' || body.length == 0) {
      broadcastEvent(EVENTS.ERROR, 'Valid body required.', 'createEvent');
    } else {
      mraidview.createEvent(date, title, body);
    }
  };

  mraid.getHeading = function() {
    if (!supports[FEATURES.HEADING]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getHeading');
    }
    return heading;
  };

  mraid.getKeyboardState = function() {
    if (!supports[FEATURES.LEVEL2]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getKeyboardState');
    }
    return keyboardState;
  }

  mraid.getLocation = function() {
    if (!supports[FEATURES.LOCATION]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getLocation');
    }
    return (null == location) ? null : clone(location);
  };

  mraid.getNetwork = function() {
    if (!supports[FEATURES.NETWORK]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getNetwork');
    }
    return network;
  };

  mraid.getOrientation = function() {
    if (!supports[FEATURES.ORIENTATION]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getOrientation');
    }
    return orientation;
  };

  mraid.getShakeProperties = function() {
    if (!supports[FEATURES.SHAKE]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getShakeProperties');
    } else {
      return (null == shakeProperties) ? null : clone(shakeProperties);
    }
  };

  mraid.getTilt = function() {
    if (!supports[FEATURES.TILT]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getTilt');
    } else {
      return (null == tilt) ? null : clone(tilt);
    }
  };

  mraid.makeCall = function(number) {
    if (!supports[FEATURES.PHONE]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'makeCall');
    } else if (!number || typeof number != 'string') {
      broadcastEvent(EVENTS.ERROR, 'Request must provide a number to call.', 'makeCall');
    } else {
      mraidview.makeCall(number);
    }
  };

  mraid.sendMail = function(recipient, subject, body) {
    if (!supports[FEATURES.EMAIL]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'sendMail');
    } else if (!recipient || typeof recipient != 'string') {
      broadcastEvent(EVENTS.ERROR, 'Request must specify a recipient.', 'sendMail');
    } else {
      mraidview.sendMail(recipient, subject, body);
    }
  };

  mraid.sendSMS = function(recipient, body) {
    if (!supports[FEATURES.SMS]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'sendSMS');
    } else if (!recipient || typeof recipient != 'string') {
      broadcastEvent(EVENTS.ERROR, 'Request must specify a recipient.', 'sendSMS');
    } else {
      mraidview.sendSMS(recipient, body);
    }
  };

  mraid.setShakeProperties = function(properties) {
    if (!supports[FEATURES.SHAKE]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'setShakeProperties');
    } else if (valid(properties, shakePropertyValidators, 'setShakeProperties')) {
      mraidview.setShakeProperties(properties);
    }
  };

  // Ormma LEVEL 3 ////////////////////////////////////////////////////////////////////
  mraid.addAsset = function(URL, alias) {
    if (!URL || !alias || typeof URL != 'string' || typeof alias != 'string') {
      broadcastEvent(EVENTS.ERROR, 'URL and alias are required.', 'addAsset');
    } else if (supports[FEATURES.LEVEL3]) {
      mraidview.addAsset(URL, alias);
    } else if (URL.indexOf('mraid://') == 0) {
      broadcastEvent(EVENTS.ERROR, 'Native device assets not supported by this client.', 'addAsset');
    } else {
      assets[alias] = URL;
      broadcastEvent(EVENTS.ASSETREADY, alias);
    }
  };

  mraid.addAssets = function(assets) {
    for (var alias in assets) {
      mraid.addAsset(assets[alias], alias);
    }
  };

  mraid.getAssetURL = function(alias) {
    if (!assets[alias]) {
      broadcastEvent(EVENTS.ERROR, 'Alias unknown.', 'getAssetURL');
    }
    return assets[alias];
  };

  mraid.getCacheRemaining = function() {
    if (!supports[FEATURES.LEVEL3]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'getCacheRemaining');
    }
    return cacheRemaining;
  };

  mraid.request = function(uri, display) {
    if (!supports[FEATURES.LEVEL3]) {
      broadcastEvent(EVENTS.ERROR, 'Method not supported by this client.', 'request');
    } else if (!uri || typeof uri != 'string') {
      broadcastEvent(EVENTS.ERROR, 'URI is required.', 'request');
    } else {
      mraidview.request(uri, display);
    }
  };

  mraid.removeAllAssets = function() {
    for (var alias in assets) {
      mraid.removeAsset(alias);
    }
  };

  mraid.removeAsset = function(alias) {
    if (!alias || typeof alias != 'string') {
      broadcastEvent(EVENTS.ERROR, 'Alias is required.', 'removeAsset');
    } else if (!assets[alias]) {
      broadcastEvent(EVENTS.ERROR, 'Alias unknown.', 'removeAsset');
    } else if (supports[FEATURES.LEVEL3]) {
      mraidview.removeAsset(alias);
    } else {
      assets[alias] = null;
      delete assets[alias];
      broadcastEvent(EVENTS.ASSETREMOVED, alias);
    }
  };
})();

// ORMMA api
(function() {
  var ormma = window.ormma = {};

  ormma.signalReady = function() {
    mraid.signalReady();
  };

  ormma.addEventListener = function(event, listener) {
    mraid.addEventListener(event, listener);
  };

  ormma.close = function() {
    mraid.close();
  };

  ormma.expand = function(dimensions, URL) {
    if (dimensions == null) {
      dimensions = getDimensions();
    }
    mraid.expandvalidator(dimensions, URL);
  };

  ormma.getViewable = function() {
    return mraid.getViewable();
  }

  ormma.getDefaultPosition = function() {
    return mraid.getDefaultPosition();
  };

  ormma.getExpandProperties = function() {
    return nmraid.getExpandProperties();
  };

  ormma.getMaxSize = function() {
    return mraid.getMaxsize();
  };

  ormma.getSize = function() {
    return mraid.getSize();
  };

  ormma.getState = function() {
    return mraid.getState();
  };

  ormma.hide = function() {
    mraid.hide();
  };

  ormma.open = function(URL, controls) {
    mraid.open(URL, controls);
  };

  ormma.openMap = function(POI, fullscreen) {
    mraid.openMap(POI, fullscreen);
  };

  ormma.removeEventListener = function(event, listener) {
    mraid.removeEventListener(event, listener);
  };

  ormma.resize = function(width, height) {
    var properties = {
      width: width,
      height: height,
      customClosePosition: "top-right",
      offsetX: mraid.getCurrentPosition().x,
      offsetY: mraid.getCurrentPosition().y,
      allowOffscreen: true
    }
    mraid.setResizeProperties(properties);
    mraid.resize();
  };

  ormma.setExpandProperties = function(properties) {
    ormmaSetExpandProperties(properties);
  };

  // private method
  ormmaSetExpandProperties = function(properties) {
    if (valid(properties, expandPropertyValidators, 'setExpandProperties')) {
      mraidview.setExpandProperties(properties);
    }
  };

  ormma.show = function() {
    mraid.show();
  };

  ormma.playAudio = function(URL, properties) {
    mraid.playAudio(URL, properties);
  };


  ormma.playVideo = function(URL, properties) {
    mraid.playVideo(URL, properties);
  };

  // LEVEL 2 ////////////////////////////////////////////////////////////////////
  ormma.createEvent = function(date, title, body) {
    mraid.createEvent(date, title, body);
  };

  ormma.getHeading = function() {
    return mraid.getHeading();
  };

  ormma.getKeyboardState = function() {
    return mraid.getKeyboardState();
  }

  ormma.getLocation = function() {
    return mraid.getKeyboardState();
  };

  ormma.getNetwork = function() {
    return mraid.getNetwork();
  };

  ormma.getOrientation = function() {
    return mraid.getOrientation();
  };

  ormma.getScreenSize = function() {
    return mraid.getScreenSize();
  };

  ormma.getPlacementType = function() {
    return mraid.getPlacementType();
  };

  ormma.getShakeProperties = function() {
    return mraid.getScreenSize();
  };

  ormma.getTilt = function() {
    return mraid.getTilt();
  };

  ormma.makeCall = function(number) {
    mraid.makeCall(number);
  };

  ormma.sendMail = function(recipient, subject, body) {
    mraid.sendMail(recipient, subject, body);
  };

  ormma.sendSMS = function(recipient, body) {
    mraid.sendSMS(recipient, body);
  };

  ormma.setShakeProperties = function(properties) {
    mraid.setShakeProperties(properties);
  };

  ormma.storePicture = function(URL) {
    mraid.storePicture(url);
  };

  ormma.supports = function(feature) {
    mraid.supports(feature);
  };

  // LEVEL 3 ////////////////////////////////////////////////////////////////////
  ormma.addAsset = function(URL, alias) {
    mraid.addAsset(URL, alias);
  };

  ormma.addAssets = function(assets) {
    mraid.addAssets(assets);
  };

  ormma.getAssetURL = function(alias) {
    return mraid.getAssetURL(alias);
  };

  ormma.getCacheRemaining = function() {
    return mraid.getCacheRemaining();
  };

  ormma.request = function(uri, display) {
    mraid.request(uri, display);
  };

  ormma.removeAllAssets = function() {
    mraid.removeAllAssets();
  };

  ormma.removeAsset = function(alias) {
    mraid.removeAsset(alias);
  };

  ormma.isViewable = function() {
    return mraid.isViewable();
  };

  var clone = function(obj) {
    var f = function() {};
    f.prototype = obj;
    return new f();
  };

  console.log = function(log) {
    mraidview.executeNativeCall("log", "log", log);
  }

})();