/***************************************************************************************************
 * BROWSER POLYFILLS
 */

// Polyfills for ES6 features
import 'core-js/es6/promise'; // Polyfill for ES6 Promise, ensures compatibility with older browsers.
import 'core-js/es6/symbol'; // Polyfill for ES6 Symbol, used for unique object property keys.
import 'core-js/es6/map'; // Polyfill for ES6 Map, a key-value data structure.
import 'core-js/es6/set'; // Polyfill for ES6 Set, a collection of unique values.
import 'core-js/es7/array'; // Polyfill for ES7 Array methods like includes().
import 'intl'; // Polyfill for Internationalization API, required for older browsers.
import 'intl/locale-data/jsonp/en'; // Adds locale data for 'en' to the Internationalization API polyfill.
import 'classlist.js'; // Polyfill for Element.classList, required for IE10 and IE11.
import 'whatwg-fetch'; // Polyfill for Fetch API, used for making HTTP requests in older browsers.
import 'intersection-observer'; // Polyfill for Intersection Observer API, used for lazy loading and other viewport-based logic.
import 'resize-observer-polyfill'; // Polyfill for Resize Observer API, used to observe changes to element sizes.

// Polyfills for Web Animations API
import 'web-animations-js'; // Polyfill for Web Animations API, required for animations in IE/Edge.

/***************************************************************************************************
 * Zone JS is required by Angular itself.
 */
import 'zone.js'; // Required by Angular for change detection and asynchronous operations.
