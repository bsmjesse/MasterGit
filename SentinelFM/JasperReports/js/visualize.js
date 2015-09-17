




/**
 * @author: Igor Nesterenko, Sergey Prilukin
 * @version $Id: visualize.jsp 51369 2014-11-12 13:59:41Z sergey.prilukin $
 */










    
    
        
    










    
    









    



    
    
        
        var __jrsConfigs__ = {
            userLocale: "en_US",
            avaliableLocales: ["de", "en", "es", "fr", "it", "ja", "ro", "zh_TW", "zh_CN"]
        };
        /** vim: et:ts=4:sw=4:sts=4
 * @license RequireJS 2.1.10 Copyright (c) 2010-2014, The Dojo Foundation All Rights Reserved.
 * Available via the MIT or new BSD license.
 * see: http://github.com/jrburke/requirejs for details
 */
//Not using strict: uneven strict support in browsers, #392, and causes
//problems with requirejs.exec()/transpiler plugins that may not be strict.
/*jslint regexp: true, nomen: true, sloppy: true */
/*global window, navigator, document, importScripts, setTimeout, opera */

var requirejs, require, define;
(function (global) {
    var req, s, head, baseElement, dataMain, src,
        interactiveScript, currentlyAddingScript, mainScript, subPath,
        version = '2.1.10',
        commentRegExp = /(\/\*([\s\S]*?)\*\/|([^:]|^)\/\/(.*)$)/mg,
        cjsRequireRegExp = /[^.]\s*require\s*\(\s*["']([^'"\s]+)["']\s*\)/g,
        jsSuffixRegExp = /\.js$/,
        currDirRegExp = /^\.\//,
        op = Object.prototype,
        ostring = op.toString,
        hasOwn = op.hasOwnProperty,
        ap = Array.prototype,
        apsp = ap.splice,
        isBrowser = !!(typeof window !== 'undefined' && typeof navigator !== 'undefined' && window.document),
        isWebWorker = !isBrowser && typeof importScripts !== 'undefined',
        //PS3 indicates loaded and complete, but need to wait for complete
        //specifically. Sequence is 'loading', 'loaded', execution,
        // then 'complete'. The UA check is unfortunate, but not sure how
        //to feature test w/o causing perf issues.
        readyRegExp = isBrowser && navigator.platform === 'PLAYSTATION 3' ?
                      /^complete$/ : /^(complete|loaded)$/,
        defContextName = '_',
        //Oh the tragedy, detecting opera. See the usage of isOpera for reason.
        isOpera = typeof opera !== 'undefined' && opera.toString() === '[object Opera]',
        contexts = {},
        cfg = {},
        globalDefQueue = [],
        useInteractive = false;

    function isFunction(it) {
        return ostring.call(it) === '[object Function]';
    }

    function isArray(it) {
        return ostring.call(it) === '[object Array]';
    }

    /**
     * Helper function for iterating over an array. If the func returns
     * a true value, it will break out of the loop.
     */
    function each(ary, func) {
        if (ary) {
            var i;
            for (i = 0; i < ary.length; i += 1) {
                if (ary[i] && func(ary[i], i, ary)) {
                    break;
                }
            }
        }
    }

    /**
     * Helper function for iterating over an array backwards. If the func
     * returns a true value, it will break out of the loop.
     */
    function eachReverse(ary, func) {
        if (ary) {
            var i;
            for (i = ary.length - 1; i > -1; i -= 1) {
                if (ary[i] && func(ary[i], i, ary)) {
                    break;
                }
            }
        }
    }

    function hasProp(obj, prop) {
        return hasOwn.call(obj, prop);
    }

    function getOwn(obj, prop) {
        return hasProp(obj, prop) && obj[prop];
    }

    /**
     * Cycles over properties in an object and calls a function for each
     * property value. If the function returns a truthy value, then the
     * iteration is stopped.
     */
    function eachProp(obj, func) {
        var prop;
        for (prop in obj) {
            if (hasProp(obj, prop)) {
                if (func(obj[prop], prop)) {
                    break;
                }
            }
        }
    }

    /**
     * Simple function to mix in properties from source into target,
     * but only if target does not already have a property of the same name.
     */
    function mixin(target, source, force, deepStringMixin) {
        if (source) {
            eachProp(source, function (value, prop) {
                if (force || !hasProp(target, prop)) {
                    if (deepStringMixin && typeof value === 'object' && value &&
                        !isArray(value) && !isFunction(value) &&
                        !(value instanceof RegExp)) {

                        if (!target[prop]) {
                            target[prop] = {};
                        }
                        mixin(target[prop], value, force, deepStringMixin);
                    } else {
                        target[prop] = value;
                    }
                }
            });
        }
        return target;
    }

    //Similar to Function.prototype.bind, but the 'this' object is specified
    //first, since it is easier to read/figure out what 'this' will be.
    function bind(obj, fn) {
        return function () {
            return fn.apply(obj, arguments);
        };
    }

    function scripts() {
        return document.getElementsByTagName('script');
    }

    function defaultOnError(err) {
        throw err;
    }

    //Allow getting a global that expressed in
    //dot notation, like 'a.b.c'.
    function getGlobal(value) {
        if (!value) {
            return value;
        }
        var g = global;
        each(value.split('.'), function (part) {
            g = g[part];
        });
        return g;
    }

    /**
     * Constructs an error with a pointer to an URL with more information.
     * @param {String} id the error ID that maps to an ID on a web page.
     * @param {String} message human readable error.
     * @param {Error} [err] the original error, if there is one.
     *
     * @returns {Error}
     */
    function makeError(id, msg, err, requireModules) {
        var e = new Error(msg + '\nhttp://requirejs.org/docs/errors.html#' + id);
        e.requireType = id;
        e.requireModules = requireModules;
        if (err) {
            e.originalError = err;
        }
        return e;
    }

    if (typeof define !== 'undefined') {
        //If a define is already in play via another AMD loader,
        //do not overwrite.
        return;
    }

    if (typeof requirejs !== 'undefined') {
        if (isFunction(requirejs)) {
            //Do not overwrite and existing requirejs instance.
            return;
        }
        cfg = requirejs;
        requirejs = undefined;
    }

    //Allow for a require config object
    if (typeof require !== 'undefined' && !isFunction(require)) {
        //assume it is a config object.
        cfg = require;
        require = undefined;
    }

    function newContext(contextName) {
        var inCheckLoaded, Module, context, handlers,
            checkLoadedTimeoutId,
            config = {
                //Defaults. Do not set a default for map
                //config to speed up normalize(), which
                //will run faster if there is no default.
                waitSeconds: 7,
                baseUrl: './',
                paths: {},
                bundles: {},
                pkgs: {},
                shim: {},
                config: {}
            },
            registry = {},
            //registry of just enabled modules, to speed
            //cycle breaking code when lots of modules
            //are registered, but not activated.
            enabledRegistry = {},
            undefEvents = {},
            defQueue = [],
            defined = {},
            urlFetched = {},
            bundlesMap = {},
            requireCounter = 1,
            unnormalizedCounter = 1;

        /**
         * Trims the . and .. from an array of path segments.
         * It will keep a leading path segment if a .. will become
         * the first path segment, to help with module name lookups,
         * which act like paths, but can be remapped. But the end result,
         * all paths that use this function should look normalized.
         * NOTE: this method MODIFIES the input array.
         * @param {Array} ary the array of path segments.
         */
        function trimDots(ary) {
            var i, part, length = ary.length;
            for (i = 0; i < length; i++) {
                part = ary[i];
                if (part === '.') {
                    ary.splice(i, 1);
                    i -= 1;
                } else if (part === '..') {
                    if (i === 1 && (ary[2] === '..' || ary[0] === '..')) {
                        //End of the line. Keep at least one non-dot
                        //path segment at the front so it can be mapped
                        //correctly to disk. Otherwise, there is likely
                        //no path mapping for a path starting with '..'.
                        //This can still fail, but catches the most reasonable
                        //uses of ..
                        break;
                    } else if (i > 0) {
                        ary.splice(i - 1, 2);
                        i -= 2;
                    }
                }
            }
        }

        /**
         * Given a relative module name, like ./something, normalize it to
         * a real name that can be mapped to a path.
         * @param {String} name the relative name
         * @param {String} baseName a real name that the name arg is relative
         * to.
         * @param {Boolean} applyMap apply the map config to the value. Should
         * only be done if this normalization is for a dependency ID.
         * @returns {String} normalized name
         */
        function normalize(name, baseName, applyMap) {
            var pkgMain, mapValue, nameParts, i, j, nameSegment, lastIndex,
                foundMap, foundI, foundStarMap, starI,
                baseParts = baseName && baseName.split('/'),
                normalizedBaseParts = baseParts,
                map = config.map,
                starMap = map && map['*'];

            //Adjust any relative paths.
            if (name && name.charAt(0) === '.') {
                //If have a base name, try to normalize against it,
                //otherwise, assume it is a top-level require that will
                //be relative to baseUrl in the end.
                if (baseName) {
                    //Convert baseName to array, and lop off the last part,
                    //so that . matches that 'directory' and not name of the baseName's
                    //module. For instance, baseName of 'one/two/three', maps to
                    //'one/two/three.js', but we want the directory, 'one/two' for
                    //this normalization.
                    normalizedBaseParts = baseParts.slice(0, baseParts.length - 1);
                    name = name.split('/');
                    lastIndex = name.length - 1;

                    // If wanting node ID compatibility, strip .js from end
                    // of IDs. Have to do this here, and not in nameToUrl
                    // because node allows either .js or non .js to map
                    // to same file.
                    if (config.nodeIdCompat && jsSuffixRegExp.test(name[lastIndex])) {
                        name[lastIndex] = name[lastIndex].replace(jsSuffixRegExp, '');
                    }

                    name = normalizedBaseParts.concat(name);
                    trimDots(name);
                    name = name.join('/');
                } else if (name.indexOf('./') === 0) {
                    // No baseName, so this is ID is resolved relative
                    // to baseUrl, pull off the leading dot.
                    name = name.substring(2);
                }
            }

            //Apply map config if available.
            if (applyMap && map && (baseParts || starMap)) {
                nameParts = name.split('/');

                outerLoop: for (i = nameParts.length; i > 0; i -= 1) {
                    nameSegment = nameParts.slice(0, i).join('/');

                    if (baseParts) {
                        //Find the longest baseName segment match in the config.
                        //So, do joins on the biggest to smallest lengths of baseParts.
                        for (j = baseParts.length; j > 0; j -= 1) {
                            mapValue = getOwn(map, baseParts.slice(0, j).join('/'));

                            //baseName segment has config, find if it has one for
                            //this name.
                            if (mapValue) {
                                mapValue = getOwn(mapValue, nameSegment);
                                if (mapValue) {
                                    //Match, update name to the new value.
                                    foundMap = mapValue;
                                    foundI = i;
                                    break outerLoop;
                                }
                            }
                        }
                    }

                    //Check for a star map match, but just hold on to it,
                    //if there is a shorter segment match later in a matching
                    //config, then favor over this star map.
                    if (!foundStarMap && starMap && getOwn(starMap, nameSegment)) {
                        foundStarMap = getOwn(starMap, nameSegment);
                        starI = i;
                    }
                }

                if (!foundMap && foundStarMap) {
                    foundMap = foundStarMap;
                    foundI = starI;
                }

                if (foundMap) {
                    nameParts.splice(0, foundI, foundMap);
                    name = nameParts.join('/');
                }
            }

            // If the name points to a package's name, use
            // the package main instead.
            pkgMain = getOwn(config.pkgs, name);

            return pkgMain ? pkgMain : name;
        }

        function removeScript(name) {
            if (isBrowser) {
                each(scripts(), function (scriptNode) {
                    if (scriptNode.getAttribute('data-requiremodule') === name &&
                            scriptNode.getAttribute('data-requirecontext') === context.contextName) {
                        scriptNode.parentNode.removeChild(scriptNode);
                        return true;
                    }
                });
            }
        }

        function hasPathFallback(id) {
            var pathConfig = getOwn(config.paths, id);
            if (pathConfig && isArray(pathConfig) && pathConfig.length > 1) {
                //Pop off the first array value, since it failed, and
                //retry
                pathConfig.shift();
                context.require.undef(id);
                context.require([id]);
                return true;
            }
        }

        //Turns a plugin!resource to [plugin, resource]
        //with the plugin being undefined if the name
        //did not have a plugin prefix.
        function splitPrefix(name) {
            var prefix,
                index = name ? name.indexOf('!') : -1;
            if (index > -1) {
                prefix = name.substring(0, index);
                name = name.substring(index + 1, name.length);
            }
            return [prefix, name];
        }

        /**
         * Creates a module mapping that includes plugin prefix, module
         * name, and path. If parentModuleMap is provided it will
         * also normalize the name via require.normalize()
         *
         * @param {String} name the module name
         * @param {String} [parentModuleMap] parent module map
         * for the module name, used to resolve relative names.
         * @param {Boolean} isNormalized: is the ID already normalized.
         * This is true if this call is done for a define() module ID.
         * @param {Boolean} applyMap: apply the map config to the ID.
         * Should only be true if this map is for a dependency.
         *
         * @returns {Object}
         */
        function makeModuleMap(name, parentModuleMap, isNormalized, applyMap) {
            var url, pluginModule, suffix, nameParts,
                prefix = null,
                parentName = parentModuleMap ? parentModuleMap.name : null,
                originalName = name,
                isDefine = true,
                normalizedName = '';

            //If no name, then it means it is a require call, generate an
            //internal name.
            if (!name) {
                isDefine = false;
                name = '_@r' + (requireCounter += 1);
            }

            nameParts = splitPrefix(name);
            prefix = nameParts[0];
            name = nameParts[1];

            if (prefix) {
                prefix = normalize(prefix, parentName, applyMap);
                pluginModule = getOwn(defined, prefix);
            }

            //Account for relative paths if there is a base name.
            if (name) {
                if (prefix) {
                    if (pluginModule && pluginModule.normalize) {
                        //Plugin is loaded, use its normalize method.
                        normalizedName = pluginModule.normalize(name, function (name) {
                            return normalize(name, parentName, applyMap);
                        });
                    } else {
                        normalizedName = normalize(name, parentName, applyMap);
                    }
                } else {
                    //A regular module.
                    normalizedName = normalize(name, parentName, applyMap);

                    //Normalized name may be a plugin ID due to map config
                    //application in normalize. The map config values must
                    //already be normalized, so do not need to redo that part.
                    nameParts = splitPrefix(normalizedName);
                    prefix = nameParts[0];
                    normalizedName = nameParts[1];
                    isNormalized = true;

                    url = context.nameToUrl(normalizedName);
                }
            }

            //If the id is a plugin id that cannot be determined if it needs
            //normalization, stamp it with a unique ID so two matching relative
            //ids that may conflict can be separate.
            suffix = prefix && !pluginModule && !isNormalized ?
                     '_unnormalized' + (unnormalizedCounter += 1) :
                     '';

            return {
                prefix: prefix,
                name: normalizedName,
                parentMap: parentModuleMap,
                unnormalized: !!suffix,
                url: url,
                originalName: originalName,
                isDefine: isDefine,
                id: (prefix ?
                        prefix + '!' + normalizedName :
                        normalizedName) + suffix
            };
        }

        function getModule(depMap) {
            var id = depMap.id,
                mod = getOwn(registry, id);

            if (!mod) {
                mod = registry[id] = new context.Module(depMap);
            }

            return mod;
        }

        function on(depMap, name, fn) {
            var id = depMap.id,
                mod = getOwn(registry, id);

            if (hasProp(defined, id) &&
                    (!mod || mod.defineEmitComplete)) {
                if (name === 'defined') {
                    fn(defined[id]);
                }
            } else {
                mod = getModule(depMap);
                if (mod.error && name === 'error') {
                    fn(mod.error);
                } else {
                    mod.on(name, fn);
                }
            }
        }

        function onError(err, errback) {
            var ids = err.requireModules,
                notified = false;

            if (errback) {
                errback(err);
            } else {
                each(ids, function (id) {
                    var mod = getOwn(registry, id);
                    if (mod) {
                        //Set error on module, so it skips timeout checks.
                        mod.error = err;
                        if (mod.events.error) {
                            notified = true;
                            mod.emit('error', err);
                        }
                    }
                });

                if (!notified) {
                    req.onError(err);
                }
            }
        }

        /**
         * Internal method to transfer globalQueue items to this context's
         * defQueue.
         */
        function takeGlobalQueue() {
            //Push all the globalDefQueue items into the context's defQueue
            if (globalDefQueue.length) {
                //Array splice in the values since the context code has a
                //local var ref to defQueue, so cannot just reassign the one
                //on context.
                apsp.apply(defQueue,
                           [defQueue.length, 0].concat(globalDefQueue));
                globalDefQueue = [];
            }
        }

        handlers = {
            'require': function (mod) {
                if (mod.require) {
                    return mod.require;
                } else {
                    return (mod.require = context.makeRequire(mod.map));
                }
            },
            'exports': function (mod) {
                mod.usingExports = true;
                if (mod.map.isDefine) {
                    if (mod.exports) {
                        return mod.exports;
                    } else {
                        return (mod.exports = defined[mod.map.id] = {});
                    }
                }
            },
            'module': function (mod) {
                if (mod.module) {
                    return mod.module;
                } else {
                    return (mod.module = {
                        id: mod.map.id,
                        uri: mod.map.url,
                        config: function () {
                            return  getOwn(config.config, mod.map.id) || {};
                        },
                        exports: handlers.exports(mod)
                    });
                }
            }
        };

        function cleanRegistry(id) {
            //Clean up machinery used for waiting modules.
            delete registry[id];
            delete enabledRegistry[id];
        }

        function breakCycle(mod, traced, processed) {
            var id = mod.map.id;

            if (mod.error) {
                mod.emit('error', mod.error);
            } else {
                traced[id] = true;
                each(mod.depMaps, function (depMap, i) {
                    var depId = depMap.id,
                        dep = getOwn(registry, depId);

                    //Only force things that have not completed
                    //being defined, so still in the registry,
                    //and only if it has not been matched up
                    //in the module already.
                    if (dep && !mod.depMatched[i] && !processed[depId]) {
                        if (getOwn(traced, depId)) {
                            mod.defineDep(i, defined[depId]);
                            mod.check(); //pass false?
                        } else {
                            breakCycle(dep, traced, processed);
                        }
                    }
                });
                processed[id] = true;
            }
        }

        function checkLoaded() {
            var err, usingPathFallback,
                waitInterval = config.waitSeconds * 1000,
                //It is possible to disable the wait interval by using waitSeconds of 0.
                expired = waitInterval && (context.startTime + waitInterval) < new Date().getTime(),
                noLoads = [],
                reqCalls = [],
                stillLoading = false,
                needCycleCheck = true;

            //Do not bother if this call was a result of a cycle break.
            if (inCheckLoaded) {
                return;
            }

            inCheckLoaded = true;

            //Figure out the state of all the modules.
            eachProp(enabledRegistry, function (mod) {
                var map = mod.map,
                    modId = map.id;

                //Skip things that are not enabled or in error state.
                if (!mod.enabled) {
                    return;
                }

                if (!map.isDefine) {
                    reqCalls.push(mod);
                }

                if (!mod.error) {
                    //If the module should be executed, and it has not
                    //been inited and time is up, remember it.
                    if (!mod.inited && expired) {
                        if (hasPathFallback(modId)) {
                            usingPathFallback = true;
                            stillLoading = true;
                        } else {
                            noLoads.push(modId);
                            removeScript(modId);
                        }
                    } else if (!mod.inited && mod.fetched && map.isDefine) {
                        stillLoading = true;
                        if (!map.prefix) {
                            //No reason to keep looking for unfinished
                            //loading. If the only stillLoading is a
                            //plugin resource though, keep going,
                            //because it may be that a plugin resource
                            //is waiting on a non-plugin cycle.
                            return (needCycleCheck = false);
                        }
                    }
                }
            });

            if (expired && noLoads.length) {
                //If wait time expired, throw error of unloaded modules.
                err = makeError('timeout', 'Load timeout for modules: ' + noLoads, null, noLoads);
                err.contextName = context.contextName;
                return onError(err);
            }

            //Not expired, check for a cycle.
            if (needCycleCheck) {
                each(reqCalls, function (mod) {
                    breakCycle(mod, {}, {});
                });
            }

            //If still waiting on loads, and the waiting load is something
            //other than a plugin resource, or there are still outstanding
            //scripts, then just try back later.
            if ((!expired || usingPathFallback) && stillLoading) {
                //Something is still waiting to load. Wait for it, but only
                //if a timeout is not already in effect.
                if ((isBrowser || isWebWorker) && !checkLoadedTimeoutId) {
                    checkLoadedTimeoutId = setTimeout(function () {
                        checkLoadedTimeoutId = 0;
                        checkLoaded();
                    }, 50);
                }
            }

            inCheckLoaded = false;
        }

        Module = function (map) {
            this.events = getOwn(undefEvents, map.id) || {};
            this.map = map;
            this.shim = getOwn(config.shim, map.id);
            this.depExports = [];
            this.depMaps = [];
            this.depMatched = [];
            this.pluginMaps = {};
            this.depCount = 0;

            /* this.exports this.factory
               this.depMaps = [],
               this.enabled, this.fetched
            */
        };

        Module.prototype = {
            init: function (depMaps, factory, errback, options) {
                options = options || {};

                //Do not do more inits if already done. Can happen if there
                //are multiple define calls for the same module. That is not
                //a normal, common case, but it is also not unexpected.
                if (this.inited) {
                    return;
                }

                this.factory = factory;

                if (errback) {
                    //Register for errors on this module.
                    this.on('error', errback);
                } else if (this.events.error) {
                    //If no errback already, but there are error listeners
                    //on this module, set up an errback to pass to the deps.
                    errback = bind(this, function (err) {
                        this.emit('error', err);
                    });
                }

                //Do a copy of the dependency array, so that
                //source inputs are not modified. For example
                //"shim" deps are passed in here directly, and
                //doing a direct modification of the depMaps array
                //would affect that config.
                this.depMaps = depMaps && depMaps.slice(0);

                this.errback = errback;

                //Indicate this module has be initialized
                this.inited = true;

                this.ignore = options.ignore;

                //Could have option to init this module in enabled mode,
                //or could have been previously marked as enabled. However,
                //the dependencies are not known until init is called. So
                //if enabled previously, now trigger dependencies as enabled.
                if (options.enabled || this.enabled) {
                    //Enable this module and dependencies.
                    //Will call this.check()
                    this.enable();
                } else {
                    this.check();
                }
            },

            defineDep: function (i, depExports) {
                //Because of cycles, defined callback for a given
                //export can be called more than once.
                if (!this.depMatched[i]) {
                    this.depMatched[i] = true;
                    this.depCount -= 1;
                    this.depExports[i] = depExports;
                }
            },

            fetch: function () {
                if (this.fetched) {
                    return;
                }
                this.fetched = true;

                context.startTime = (new Date()).getTime();

                var map = this.map;

                //If the manager is for a plugin managed resource,
                //ask the plugin to load it now.
                if (this.shim) {
                    context.makeRequire(this.map, {
                        enableBuildCallback: true
                    })(this.shim.deps || [], bind(this, function () {
                        return map.prefix ? this.callPlugin() : this.load();
                    }));
                } else {
                    //Regular dependency.
                    return map.prefix ? this.callPlugin() : this.load();
                }
            },

            load: function () {
                var url = this.map.url;

                //Regular dependency.
                if (!urlFetched[url]) {
                    urlFetched[url] = true;
                    context.load(this.map.id, url);
                }
            },

            /**
             * Checks if the module is ready to define itself, and if so,
             * define it.
             */
            check: function () {
                if (!this.enabled || this.enabling) {
                    return;
                }

                var err, cjsModule,
                    id = this.map.id,
                    depExports = this.depExports,
                    exports = this.exports,
                    factory = this.factory;

                if (!this.inited) {
                    this.fetch();
                } else if (this.error) {
                    this.emit('error', this.error);
                } else if (!this.defining) {
                    //The factory could trigger another require call
                    //that would result in checking this module to
                    //define itself again. If already in the process
                    //of doing that, skip this work.
                    this.defining = true;

                    if (this.depCount < 1 && !this.defined) {
                        if (isFunction(factory)) {
                            //If there is an error listener, favor passing
                            //to that instead of throwing an error. However,
                            //only do it for define()'d  modules. require
                            //errbacks should not be called for failures in
                            //their callbacks (#699). However if a global
                            //onError is set, use that.
                            if ((this.events.error && this.map.isDefine) ||
                                req.onError !== defaultOnError) {
                                try {
                                    exports = context.execCb(id, factory, depExports, exports);
                                } catch (e) {
                                    err = e;
                                }
                            } else {
                                exports = context.execCb(id, factory, depExports, exports);
                            }

                            // Favor return value over exports. If node/cjs in play,
                            // then will not have a return value anyway. Favor
                            // module.exports assignment over exports object.
                            if (this.map.isDefine && exports === undefined) {
                                cjsModule = this.module;
                                if (cjsModule) {
                                    exports = cjsModule.exports;
                                } else if (this.usingExports) {
                                    //exports already set the defined value.
                                    exports = this.exports;
                                }
                            }

                            if (err) {
                                err.requireMap = this.map;
                                err.requireModules = this.map.isDefine ? [this.map.id] : null;
                                err.requireType = this.map.isDefine ? 'define' : 'require';
                                return onError((this.error = err));
                            }

                        } else {
                            //Just a literal value
                            exports = factory;
                        }

                        this.exports = exports;

                        if (this.map.isDefine && !this.ignore) {
                            defined[id] = exports;

                            if (req.onResourceLoad) {
                                req.onResourceLoad(context, this.map, this.depMaps);
                            }
                        }

                        //Clean up
                        cleanRegistry(id);

                        this.defined = true;
                    }

                    //Finished the define stage. Allow calling check again
                    //to allow define notifications below in the case of a
                    //cycle.
                    this.defining = false;

                    if (this.defined && !this.defineEmitted) {
                        this.defineEmitted = true;
                        this.emit('defined', this.exports);
                        this.defineEmitComplete = true;
                    }

                }
            },

            callPlugin: function () {
                var map = this.map,
                    id = map.id,
                    //Map already normalized the prefix.
                    pluginMap = makeModuleMap(map.prefix);

                //Mark this as a dependency for this plugin, so it
                //can be traced for cycles.
                this.depMaps.push(pluginMap);

                on(pluginMap, 'defined', bind(this, function (plugin) {
                    var load, normalizedMap, normalizedMod,
                        bundleId = getOwn(bundlesMap, this.map.id),
                        name = this.map.name,
                        parentName = this.map.parentMap ? this.map.parentMap.name : null,
                        localRequire = context.makeRequire(map.parentMap, {
                            enableBuildCallback: true
                        });

                    //If current map is not normalized, wait for that
                    //normalized name to load instead of continuing.
                    if (this.map.unnormalized) {
                        //Normalize the ID if the plugin allows it.
                        if (plugin.normalize) {
                            name = plugin.normalize(name, function (name) {
                                return normalize(name, parentName, true);
                            }) || '';
                        }

                        //prefix and name should already be normalized, no need
                        //for applying map config again either.
                        normalizedMap = makeModuleMap(map.prefix + '!' + name,
                                                      this.map.parentMap);
                        on(normalizedMap,
                            'defined', bind(this, function (value) {
                                this.init([], function () { return value; }, null, {
                                    enabled: true,
                                    ignore: true
                                });
                            }));

                        normalizedMod = getOwn(registry, normalizedMap.id);
                        if (normalizedMod) {
                            //Mark this as a dependency for this plugin, so it
                            //can be traced for cycles.
                            this.depMaps.push(normalizedMap);

                            if (this.events.error) {
                                normalizedMod.on('error', bind(this, function (err) {
                                    this.emit('error', err);
                                }));
                            }
                            normalizedMod.enable();
                        }

                        return;
                    }

                    //If a paths config, then just load that file instead to
                    //resolve the plugin, as it is built into that paths layer.
                    if (bundleId) {
                        this.map.url = context.nameToUrl(bundleId);
                        this.load();
                        return;
                    }

                    load = bind(this, function (value) {
                        this.init([], function () { return value; }, null, {
                            enabled: true
                        });
                    });

                    load.error = bind(this, function (err) {
                        this.inited = true;
                        this.error = err;
                        err.requireModules = [id];

                        //Remove temp unnormalized modules for this module,
                        //since they will never be resolved otherwise now.
                        eachProp(registry, function (mod) {
                            if (mod.map.id.indexOf(id + '_unnormalized') === 0) {
                                cleanRegistry(mod.map.id);
                            }
                        });

                        onError(err);
                    });

                    //Allow plugins to load other code without having to know the
                    //context or how to 'complete' the load.
                    load.fromText = bind(this, function (text, textAlt) {
                        /*jslint evil: true */
                        var moduleName = map.name,
                            moduleMap = makeModuleMap(moduleName),
                            hasInteractive = useInteractive;

                        //As of 2.1.0, support just passing the text, to reinforce
                        //fromText only being called once per resource. Still
                        //support old style of passing moduleName but discard
                        //that moduleName in favor of the internal ref.
                        if (textAlt) {
                            text = textAlt;
                        }

                        //Turn off interactive script matching for IE for any define
                        //calls in the text, then turn it back on at the end.
                        if (hasInteractive) {
                            useInteractive = false;
                        }

                        //Prime the system by creating a module instance for
                        //it.
                        getModule(moduleMap);

                        //Transfer any config to this other module.
                        if (hasProp(config.config, id)) {
                            config.config[moduleName] = config.config[id];
                        }

                        try {
                            req.exec(text);
                        } catch (e) {
                            return onError(makeError('fromtexteval',
                                             'fromText eval for ' + id +
                                            ' failed: ' + e,
                                             e,
                                             [id]));
                        }

                        if (hasInteractive) {
                            useInteractive = true;
                        }

                        //Mark this as a dependency for the plugin
                        //resource
                        this.depMaps.push(moduleMap);

                        //Support anonymous modules.
                        context.completeLoad(moduleName);

                        //Bind the value of that module to the value for this
                        //resource ID.
                        localRequire([moduleName], load);
                    });

                    //Use parentName here since the plugin's name is not reliable,
                    //could be some weird string with no path that actually wants to
                    //reference the parentName's path.
                    plugin.load(map.name, localRequire, load, config);
                }));

                context.enable(pluginMap, this);
                this.pluginMaps[pluginMap.id] = pluginMap;
            },

            enable: function () {
                enabledRegistry[this.map.id] = this;
                this.enabled = true;

                //Set flag mentioning that the module is enabling,
                //so that immediate calls to the defined callbacks
                //for dependencies do not trigger inadvertent load
                //with the depCount still being zero.
                this.enabling = true;

                //Enable each dependency
                each(this.depMaps, bind(this, function (depMap, i) {
                    var id, mod, handler;

                    if (typeof depMap === 'string') {
                        //Dependency needs to be converted to a depMap
                        //and wired up to this module.
                        depMap = makeModuleMap(depMap,
                                               (this.map.isDefine ? this.map : this.map.parentMap),
                                               false,
                                               !this.skipMap);
                        this.depMaps[i] = depMap;

                        handler = getOwn(handlers, depMap.id);

                        if (handler) {
                            this.depExports[i] = handler(this);
                            return;
                        }

                        this.depCount += 1;

                        on(depMap, 'defined', bind(this, function (depExports) {
                            this.defineDep(i, depExports);
                            this.check();
                        }));

                        if (this.errback) {
                            on(depMap, 'error', bind(this, this.errback));
                        }
                    }

                    id = depMap.id;
                    mod = registry[id];

                    //Skip special modules like 'require', 'exports', 'module'
                    //Also, don't call enable if it is already enabled,
                    //important in circular dependency cases.
                    if (!hasProp(handlers, id) && mod && !mod.enabled) {
                        context.enable(depMap, this);
                    }
                }));

                //Enable each plugin that is used in
                //a dependency
                eachProp(this.pluginMaps, bind(this, function (pluginMap) {
                    var mod = getOwn(registry, pluginMap.id);
                    if (mod && !mod.enabled) {
                        context.enable(pluginMap, this);
                    }
                }));

                this.enabling = false;

                this.check();
            },

            on: function (name, cb) {
                var cbs = this.events[name];
                if (!cbs) {
                    cbs = this.events[name] = [];
                }
                cbs.push(cb);
            },

            emit: function (name, evt) {
                each(this.events[name], function (cb) {
                    cb(evt);
                });
                if (name === 'error') {
                    //Now that the error handler was triggered, remove
                    //the listeners, since this broken Module instance
                    //can stay around for a while in the registry.
                    delete this.events[name];
                }
            }
        };

        function callGetModule(args) {
            //Skip modules already defined.
            if (!hasProp(defined, args[0])) {
                getModule(makeModuleMap(args[0], null, true)).init(args[1], args[2]);
            }
        }

        function removeListener(node, func, name, ieName) {
            //Favor detachEvent because of IE9
            //issue, see attachEvent/addEventListener comment elsewhere
            //in this file.
            if (node.detachEvent && !isOpera) {
                //Probably IE. If not it will throw an error, which will be
                //useful to know.
                if (ieName) {
                    node.detachEvent(ieName, func);
                }
            } else {
                node.removeEventListener(name, func, false);
            }
        }

        /**
         * Given an event from a script node, get the requirejs info from it,
         * and then removes the event listeners on the node.
         * @param {Event} evt
         * @returns {Object}
         */
        function getScriptData(evt) {
            //Using currentTarget instead of target for Firefox 2.0's sake. Not
            //all old browsers will be supported, but this one was easy enough
            //to support and still makes sense.
            var node = evt.currentTarget || evt.srcElement;

            //Remove the listeners once here.
            removeListener(node, context.onScriptLoad, 'load', 'onreadystatechange');
            removeListener(node, context.onScriptError, 'error');

            return {
                node: node,
                id: node && node.getAttribute('data-requiremodule')
            };
        }

        function intakeDefines() {
            var args;

            //Any defined modules in the global queue, intake them now.
            takeGlobalQueue();

            //Make sure any remaining defQueue items get properly processed.
            while (defQueue.length) {
                args = defQueue.shift();
                if (args[0] === null) {
                    return onError(makeError('mismatch', 'Mismatched anonymous define() module: ' + args[args.length - 1]));
                } else {
                    //args are id, deps, factory. Should be normalized by the
                    //define() function.
                    callGetModule(args);
                }
            }
        }

        context = {
            config: config,
            contextName: contextName,
            registry: registry,
            defined: defined,
            urlFetched: urlFetched,
            defQueue: defQueue,
            Module: Module,
            makeModuleMap: makeModuleMap,
            nextTick: req.nextTick,
            onError: onError,

            /**
             * Set a configuration for the context.
             * @param {Object} cfg config object to integrate.
             */
            configure: function (cfg) {
                //Make sure the baseUrl ends in a slash.
                if (cfg.baseUrl) {
                    if (cfg.baseUrl.charAt(cfg.baseUrl.length - 1) !== '/') {
                        cfg.baseUrl += '/';
                    }
                }

                //Save off the paths since they require special processing,
                //they are additive.
                var shim = config.shim,
                    objs = {
                        paths: true,
                        bundles: true,
                        config: true,
                        map: true
                    };

                eachProp(cfg, function (value, prop) {
                    if (objs[prop]) {
                        if (!config[prop]) {
                            config[prop] = {};
                        }
                        mixin(config[prop], value, true, true);
                    } else {
                        config[prop] = value;
                    }
                });

                //Reverse map the bundles
                if (cfg.bundles) {
                    eachProp(cfg.bundles, function (value, prop) {
                        each(value, function (v) {
                            if (v !== prop) {
                                bundlesMap[v] = prop;
                            }
                        });
                    });
                }

                //Merge shim
                if (cfg.shim) {
                    eachProp(cfg.shim, function (value, id) {
                        //Normalize the structure
                        if (isArray(value)) {
                            value = {
                                deps: value
                            };
                        }
                        if ((value.exports || value.init) && !value.exportsFn) {
                            value.exportsFn = context.makeShimExports(value);
                        }
                        shim[id] = value;
                    });
                    config.shim = shim;
                }

                //Adjust packages if necessary.
                if (cfg.packages) {
                    each(cfg.packages, function (pkgObj) {
                        var location, name;

                        pkgObj = typeof pkgObj === 'string' ? { name: pkgObj } : pkgObj;

                        name = pkgObj.name;
                        location = pkgObj.location;
                        if (location) {
                            config.paths[name] = pkgObj.location;
                        }

                        //Save pointer to main module ID for pkg name.
                        //Remove leading dot in main, so main paths are normalized,
                        //and remove any trailing .js, since different package
                        //envs have different conventions: some use a module name,
                        //some use a file name.
                        config.pkgs[name] = pkgObj.name + '/' + (pkgObj.main || 'main')
                                     .replace(currDirRegExp, '')
                                     .replace(jsSuffixRegExp, '');
                    });
                }

                //If there are any "waiting to execute" modules in the registry,
                //update the maps for them, since their info, like URLs to load,
                //may have changed.
                eachProp(registry, function (mod, id) {
                    //If module already has init called, since it is too
                    //late to modify them, and ignore unnormalized ones
                    //since they are transient.
                    if (!mod.inited && !mod.map.unnormalized) {
                        mod.map = makeModuleMap(id);
                    }
                });

                //If a deps array or a config callback is specified, then call
                //require with those args. This is useful when require is defined as a
                //config object before require.js is loaded.
                if (cfg.deps || cfg.callback) {
                    context.require(cfg.deps || [], cfg.callback);
                }
            },

            makeShimExports: function (value) {
                function fn() {
                    var ret;
                    if (value.init) {
                        ret = value.init.apply(global, arguments);
                    }
                    return ret || (value.exports && getGlobal(value.exports));
                }
                return fn;
            },

            makeRequire: function (relMap, options) {
                options = options || {};

                function localRequire(deps, callback, errback) {
                    var id, map, requireMod;

                    if (options.enableBuildCallback && callback && isFunction(callback)) {
                        callback.__requireJsBuild = true;
                    }

                    if (typeof deps === 'string') {
                        if (isFunction(callback)) {
                            //Invalid call
                            return onError(makeError('requireargs', 'Invalid require call'), errback);
                        }

                        //If require|exports|module are requested, get the
                        //value for them from the special handlers. Caveat:
                        //this only works while module is being defined.
                        if (relMap && hasProp(handlers, deps)) {
                            return handlers[deps](registry[relMap.id]);
                        }

                        //Synchronous access to one module. If require.get is
                        //available (as in the Node adapter), prefer that.
                        if (req.get) {
                            return req.get(context, deps, relMap, localRequire);
                        }

                        //Normalize module name, if it contains . or ..
                        map = makeModuleMap(deps, relMap, false, true);
                        id = map.id;

                        if (!hasProp(defined, id)) {
                            return onError(makeError('notloaded', 'Module name "' +
                                        id +
                                        '" has not been loaded yet for context: ' +
                                        contextName +
                                        (relMap ? '' : '. Use require([])')));
                        }
                        return defined[id];
                    }

                    //Grab defines waiting in the global queue.
                    intakeDefines();

                    //Mark all the dependencies as needing to be loaded.
                    context.nextTick(function () {
                        //Some defines could have been added since the
                        //require call, collect them.
                        intakeDefines();

                        requireMod = getModule(makeModuleMap(null, relMap));

                        //Store if map config should be applied to this require
                        //call for dependencies.
                        requireMod.skipMap = options.skipMap;

                        requireMod.init(deps, callback, errback, {
                            enabled: true
                        });

                        checkLoaded();
                    });

                    return localRequire;
                }

                mixin(localRequire, {
                    isBrowser: isBrowser,

                    /**
                     * Converts a module name + .extension into an URL path.
                     * *Requires* the use of a module name. It does not support using
                     * plain URLs like nameToUrl.
                     */
                    toUrl: function (moduleNamePlusExt) {
                        var ext,
                            index = moduleNamePlusExt.lastIndexOf('.'),
                            segment = moduleNamePlusExt.split('/')[0],
                            isRelative = segment === '.' || segment === '..';

                        //Have a file extension alias, and it is not the
                        //dots from a relative path.
                        if (index !== -1 && (!isRelative || index > 1)) {
                            ext = moduleNamePlusExt.substring(index, moduleNamePlusExt.length);
                            moduleNamePlusExt = moduleNamePlusExt.substring(0, index);
                        }

                        return context.nameToUrl(normalize(moduleNamePlusExt,
                                                relMap && relMap.id, true), ext,  true);
                    },

                    defined: function (id) {
                        return hasProp(defined, makeModuleMap(id, relMap, false, true).id);
                    },

                    specified: function (id) {
                        id = makeModuleMap(id, relMap, false, true).id;
                        return hasProp(defined, id) || hasProp(registry, id);
                    }
                });

                //Only allow undef on top level require calls
                if (!relMap) {
                    localRequire.undef = function (id) {
                        //Bind any waiting define() calls to this context,
                        //fix for #408
                        takeGlobalQueue();

                        var map = makeModuleMap(id, relMap, true),
                            mod = getOwn(registry, id);

                        removeScript(id);

                        delete defined[id];
                        delete urlFetched[map.url];
                        delete undefEvents[id];

                        //Clean queued defines too. Go backwards
                        //in array so that the splices do not
                        //mess up the iteration.
                        eachReverse(defQueue, function(args, i) {
                            if(args[0] === id) {
                                defQueue.splice(i, 1);
                            }
                        });

                        if (mod) {
                            //Hold on to listeners in case the
                            //module will be attempted to be reloaded
                            //using a different config.
                            if (mod.events.defined) {
                                undefEvents[id] = mod.events;
                            }

                            cleanRegistry(id);
                        }
                    };
                }

                return localRequire;
            },

            /**
             * Called to enable a module if it is still in the registry
             * awaiting enablement. A second arg, parent, the parent module,
             * is passed in for context, when this method is overriden by
             * the optimizer. Not shown here to keep code compact.
             */
            enable: function (depMap) {
                var mod = getOwn(registry, depMap.id);
                if (mod) {
                    getModule(depMap).enable();
                }
            },

            /**
             * Internal method used by environment adapters to complete a load event.
             * A load event could be a script load or just a load pass from a synchronous
             * load call.
             * @param {String} moduleName the name of the module to potentially complete.
             */
            completeLoad: function (moduleName) {
                var found, args, mod,
                    shim = getOwn(config.shim, moduleName) || {},
                    shExports = shim.exports;

                takeGlobalQueue();

                while (defQueue.length) {
                    args = defQueue.shift();
                    if (args[0] === null) {
                        args[0] = moduleName;
                        //If already found an anonymous module and bound it
                        //to this name, then this is some other anon module
                        //waiting for its completeLoad to fire.
                        if (found) {
                            break;
                        }
                        found = true;
                    } else if (args[0] === moduleName) {
                        //Found matching define call for this script!
                        found = true;
                    }

                    callGetModule(args);
                }

                //Do this after the cycle of callGetModule in case the result
                //of those calls/init calls changes the registry.
                mod = getOwn(registry, moduleName);

                if (!found && !hasProp(defined, moduleName) && mod && !mod.inited) {
                    if (config.enforceDefine && (!shExports || !getGlobal(shExports))) {
                        if (hasPathFallback(moduleName)) {
                            return;
                        } else {
                            return onError(makeError('nodefine',
                                             'No define call for ' + moduleName,
                                             null,
                                             [moduleName]));
                        }
                    } else {
                        //A script that does not call define(), so just simulate
                        //the call for it.
                        callGetModule([moduleName, (shim.deps || []), shim.exportsFn]);
                    }
                }

                checkLoaded();
            },

            /**
             * Converts a module name to a file path. Supports cases where
             * moduleName may actually be just an URL.
             * Note that it **does not** call normalize on the moduleName,
             * it is assumed to have already been normalized. This is an
             * internal API, not a public one. Use toUrl for the public API.
             */
            nameToUrl: function (moduleName, ext, skipExt) {
                var paths, syms, i, parentModule, url,
                    parentPath, bundleId,
                    pkgMain = getOwn(config.pkgs, moduleName);

                if (pkgMain) {
                    moduleName = pkgMain;
                }

                bundleId = getOwn(bundlesMap, moduleName);

                if (bundleId) {
                    return context.nameToUrl(bundleId, ext, skipExt);
                }

                //If a colon is in the URL, it indicates a protocol is used and it is just
                //an URL to a file, or if it starts with a slash, contains a query arg (i.e. ?)
                //or ends with .js, then assume the user meant to use an url and not a module id.
                //The slash is important for protocol-less URLs as well as full paths.
                if (req.jsExtRegExp.test(moduleName)) {
                    //Just a plain path, not module name lookup, so just return it.
                    //Add extension if it is included. This is a bit wonky, only non-.js things pass
                    //an extension, this method probably needs to be reworked.
                    url = moduleName + (ext || '');
                } else {
                    //A module that needs to be converted to a path.
                    paths = config.paths;

                    syms = moduleName.split('/');
                    //For each module name segment, see if there is a path
                    //registered for it. Start with most specific name
                    //and work up from it.
                    for (i = syms.length; i > 0; i -= 1) {
                        parentModule = syms.slice(0, i).join('/');

                        parentPath = getOwn(paths, parentModule);
                        if (parentPath) {
                            //If an array, it means there are a few choices,
                            //Choose the one that is desired
                            if (isArray(parentPath)) {
                                parentPath = parentPath[0];
                            }
                            syms.splice(0, i, parentPath);
                            break;
                        }
                    }

                    //Join the path parts together, then figure out if baseUrl is needed.
                    url = syms.join('/');
                    url += (ext || (/^data\:|\?/.test(url) || skipExt ? '' : '.js'));
                    url = (url.charAt(0) === '/' || url.match(/^[\w\+\.\-]+:/) ? '' : config.baseUrl) + url;
                }

                return config.urlArgs ? url +
                                        ((url.indexOf('?') === -1 ? '?' : '&') +
                                         config.urlArgs) : url;
            },

            //Delegates to req.load. Broken out as a separate function to
            //allow overriding in the optimizer.
            load: function (id, url) {
                req.load(context, id, url);
            },

            /**
             * Executes a module callback function. Broken out as a separate function
             * solely to allow the build system to sequence the files in the built
             * layer in the right sequence.
             *
             * @private
             */
            execCb: function (name, callback, args, exports) {
                return callback.apply(exports, args);
            },

            /**
             * callback for script loads, used to check status of loading.
             *
             * @param {Event} evt the event from the browser for the script
             * that was loaded.
             */
            onScriptLoad: function (evt) {
                //Using currentTarget instead of target for Firefox 2.0's sake. Not
                //all old browsers will be supported, but this one was easy enough
                //to support and still makes sense.
                if (evt.type === 'load' ||
                        (readyRegExp.test((evt.currentTarget || evt.srcElement).readyState))) {
                    //Reset interactive script so a script node is not held onto for
                    //to long.
                    interactiveScript = null;

                    //Pull out the name of the module and the context.
                    var data = getScriptData(evt);
                    context.completeLoad(data.id);
                }
            },

            /**
             * Callback for script errors.
             */
            onScriptError: function (evt) {
                var data = getScriptData(evt);
                if (!hasPathFallback(data.id)) {
                    return onError(makeError('scripterror', 'Script error for: ' + data.id, evt, [data.id]));
                }
            }
        };

        context.require = context.makeRequire();
        return context;
    }

    /**
     * Main entry point.
     *
     * If the only argument to require is a string, then the module that
     * is represented by that string is fetched for the appropriate context.
     *
     * If the first argument is an array, then it will be treated as an array
     * of dependency string names to fetch. An optional function callback can
     * be specified to execute when all of those dependencies are available.
     *
     * Make a local req variable to help Caja compliance (it assumes things
     * on a require that are not standardized), and to give a short
     * name for minification/local scope use.
     */
    req = requirejs = function (deps, callback, errback, optional) {

        //Find the right context, use default
        var context, config,
            contextName = defContextName;

        // Determine if have config object in the call.
        if (!isArray(deps) && typeof deps !== 'string') {
            // deps is a config object
            config = deps;
            if (isArray(callback)) {
                // Adjust args if there are dependencies
                deps = callback;
                callback = errback;
                errback = optional;
            } else {
                deps = [];
            }
        }

        if (config && config.context) {
            contextName = config.context;
        }

        context = getOwn(contexts, contextName);
        if (!context) {
            context = contexts[contextName] = req.s.newContext(contextName);
        }

        if (config) {
            context.configure(config);
        }

        return context.require(deps, callback, errback);
    };

    /**
     * Support require.config() to make it easier to cooperate with other
     * AMD loaders on globally agreed names.
     */
    req.config = function (config) {
        return req(config);
    };

    /**
     * Execute something after the current tick
     * of the event loop. Override for other envs
     * that have a better solution than setTimeout.
     * @param  {Function} fn function to execute later.
     */
    req.nextTick = typeof setTimeout !== 'undefined' ? function (fn) {
        setTimeout(fn, 4);
    } : function (fn) { fn(); };

    /**
     * Export require as a global, but only if it does not already exist.
     */
    if (!require) {
        require = req;
    }

    req.version = version;

    //Used to filter out dependencies that are already paths.
    req.jsExtRegExp = /^\/|:|\?|\.js$/;
    req.isBrowser = isBrowser;
    s = req.s = {
        contexts: contexts,
        newContext: newContext
    };

    //Create default context.
    req({});

    //Exports some context-sensitive methods on global require.
    each([
        'toUrl',
        'undef',
        'defined',
        'specified'
    ], function (prop) {
        //Reference from contexts instead of early binding to default context,
        //so that during builds, the latest instance of the default context
        //with its config gets used.
        req[prop] = function () {
            var ctx = contexts[defContextName];
            return ctx.require[prop].apply(ctx, arguments);
        };
    });

    if (isBrowser) {
        head = s.head = document.getElementsByTagName('head')[0];
        //If BASE tag is in play, using appendChild is a problem for IE6.
        //When that browser dies, this can be removed. Details in this jQuery bug:
        //http://dev.jquery.com/ticket/2709
        baseElement = document.getElementsByTagName('base')[0];
        if (baseElement) {
            head = s.head = baseElement.parentNode;
        }
    }

    /**
     * Any errors that require explicitly generates will be passed to this
     * function. Intercept/override it if you want custom error handling.
     * @param {Error} err the error object.
     */
    req.onError = defaultOnError;

    /**
     * Creates the node for the load command. Only used in browser envs.
     */
    req.createNode = function (config, moduleName, url) {
        var node = config.xhtml ?
                document.createElementNS('http://www.w3.org/1999/xhtml', 'html:script') :
                document.createElement('script');
        node.type = config.scriptType || 'text/javascript';
        node.charset = 'utf-8';
        node.async = true;
        return node;
    };

    /**
     * Does the request to load a module for the browser case.
     * Make this a separate function to allow other environments
     * to override it.
     *
     * @param {Object} context the require context to find state.
     * @param {String} moduleName the name of the module.
     * @param {Object} url the URL to the module.
     */
    req.load = function (context, moduleName, url) {
        var config = (context && context.config) || {},
            node;
        if (isBrowser) {
            //In the browser so use a script tag
            node = req.createNode(config, moduleName, url);

            node.setAttribute('data-requirecontext', context.contextName);
            node.setAttribute('data-requiremodule', moduleName);

            //Set up load listener. Test attachEvent first because IE9 has
            //a subtle issue in its addEventListener and script onload firings
            //that do not match the behavior of all other browsers with
            //addEventListener support, which fire the onload event for a
            //script right after the script execution. See:
            //https://connect.microsoft.com/IE/feedback/details/648057/script-onload-event-is-not-fired-immediately-after-script-execution
            //UNFORTUNATELY Opera implements attachEvent but does not follow the script
            //script execution mode.
            if (node.attachEvent &&
                    //Check if node.attachEvent is artificially added by custom script or
                    //natively supported by browser
                    //read https://github.com/jrburke/requirejs/issues/187
                    //if we can NOT find [native code] then it must NOT natively supported.
                    //in IE8, node.attachEvent does not have toString()
                    //Note the test for "[native code" with no closing brace, see:
                    //https://github.com/jrburke/requirejs/issues/273
                    !(node.attachEvent.toString && node.attachEvent.toString().indexOf('[native code') < 0) &&
                    !isOpera) {
                //Probably IE. IE (at least 6-8) do not fire
                //script onload right after executing the script, so
                //we cannot tie the anonymous define call to a name.
                //However, IE reports the script as being in 'interactive'
                //readyState at the time of the define call.
                useInteractive = true;

                node.attachEvent('onreadystatechange', context.onScriptLoad);
                //It would be great to add an error handler here to catch
                //404s in IE9+. However, onreadystatechange will fire before
                //the error handler, so that does not help. If addEventListener
                //is used, then IE will fire error before load, but we cannot
                //use that pathway given the connect.microsoft.com issue
                //mentioned above about not doing the 'script execute,
                //then fire the script load event listener before execute
                //next script' that other browsers do.
                //Best hope: IE10 fixes the issues,
                //and then destroys all installs of IE 6-9.
                //node.attachEvent('onerror', context.onScriptError);
            } else {
                node.addEventListener('load', context.onScriptLoad, false);
                node.addEventListener('error', context.onScriptError, false);
            }
            node.src = url;

            //For some cache cases in IE 6-8, the script executes before the end
            //of the appendChild execution, so to tie an anonymous define
            //call to the module name (which is stored on the node), hold on
            //to a reference to this node, but clear after the DOM insertion.
            currentlyAddingScript = node;
            if (baseElement) {
                head.insertBefore(node, baseElement);
            } else {
                head.appendChild(node);
            }
            currentlyAddingScript = null;

            return node;
        } else if (isWebWorker) {
            try {
                //In a web worker, use importScripts. This is not a very
                //efficient use of importScripts, importScripts will block until
                //its script is downloaded and evaluated. However, if web workers
                //are in play, the expectation that a build has been done so that
                //only one script needs to be loaded anyway. This may need to be
                //reevaluated if other use cases become common.
                importScripts(url);

                //Account for anonymous modules
                context.completeLoad(moduleName);
            } catch (e) {
                context.onError(makeError('importscripts',
                                'importScripts failed for ' +
                                    moduleName + ' at ' + url,
                                e,
                                [moduleName]));
            }
        }
    };

    function getInteractiveScript() {
        if (interactiveScript && interactiveScript.readyState === 'interactive') {
            return interactiveScript;
        }

        eachReverse(scripts(), function (script) {
            if (script.readyState === 'interactive') {
                return (interactiveScript = script);
            }
        });
        return interactiveScript;
    }

    //Look for a data-main script attribute, which could also adjust the baseUrl.
    if (isBrowser && !cfg.skipDataMain) {
        //Figure out baseUrl. Get it from the script tag with require.js in it.
        eachReverse(scripts(), function (script) {
            //Set the 'head' where we can append children by
            //using the script's parent.
            if (!head) {
                head = script.parentNode;
            }

            //Look for a data-main attribute to set main script for the page
            //to load. If it is there, the path to data main becomes the
            //baseUrl, if it is not already set.
            dataMain = script.getAttribute('data-main');
            if (dataMain) {
                //Preserve dataMain in case it is a path (i.e. contains '?')
                mainScript = dataMain;

                //Set final baseUrl if there is not already an explicit one.
                if (!cfg.baseUrl) {
                    //Pull off the directory of data-main for use as the
                    //baseUrl.
                    src = mainScript.split('/');
                    mainScript = src.pop();
                    subPath = src.length ? src.join('/')  + '/' : './';

                    cfg.baseUrl = subPath;
                }

                //Strip off any trailing .js since mainScript is now
                //like a module name.
                mainScript = mainScript.replace(jsSuffixRegExp, '');

                 //If mainScript is still a path, fall back to dataMain
                if (req.jsExtRegExp.test(mainScript)) {
                    mainScript = dataMain;
                }

                //Put the data-main script in the files to load.
                cfg.deps = cfg.deps ? cfg.deps.concat(mainScript) : [mainScript];

                return true;
            }
        });
    }

    /**
     * The function that handles definitions of modules. Differs from
     * require() in that a string for the module should be the first argument,
     * and the function to execute after dependencies are loaded should
     * return a value to define the module corresponding to the first argument's
     * name.
     */
    define = function (name, deps, callback) {
        var node, context;

        //Allow for anonymous modules
        if (typeof name !== 'string') {
            //Adjust args appropriately
            callback = deps;
            deps = name;
            name = null;
        }

        //This module may not have dependencies
        if (!isArray(deps)) {
            callback = deps;
            deps = null;
        }

        //If no name, and callback is a function, then figure out if it a
        //CommonJS thing with dependencies.
        if (!deps && isFunction(callback)) {
            deps = [];
            //Remove comments from the callback string,
            //look for require calls, and pull them into the dependencies,
            //but only if there are function args.
            if (callback.length) {
                callback
                    .toString()
                    .replace(commentRegExp, '')
                    .replace(cjsRequireRegExp, function (match, dep) {
                        deps.push(dep);
                    });

                //May be a CommonJS thing even without require calls, but still
                //could use exports, and module. Avoid doing exports and module
                //work though if it just needs require.
                //REQUIRES the function to expect the CommonJS variables in the
                //order listed below.
                deps = (callback.length === 1 ? ['require'] : ['require', 'exports', 'module']).concat(deps);
            }
        }

        //If in IE 6-8 and hit an anonymous define() call, do the interactive
        //work.
        if (useInteractive) {
            node = currentlyAddingScript || getInteractiveScript();
            if (node) {
                if (!name) {
                    name = node.getAttribute('data-requiremodule');
                }
                context = contexts[node.getAttribute('data-requirecontext')];
            }
        }

        //Always save off evaluating the def call until the script onload handler.
        //This allows multiple modules to be in a file without prematurely
        //tracing dependencies, and allows for anonymous module support,
        //where the module name is not known until the script onload event
        //occurs. If no context, use the global queue, and get it processed
        //in the onscript load callback.
        (context ? context.defQueue : globalDefQueue).push([name, deps, callback]);
    };

    define.amd = {
        jQuery: true
    };


    /**
     * Executes the text. Normally just uses eval, but can be modified
     * to use a better, environment-specific call. Only used for transpiling
     * loader plugins, not for plain JS modules.
     * @param {String} text the text to execute/evaluate.
     */
    req.exec = function (text) {
        /*jslint evil: true */
        return eval(text);
    };

    //Set up with config info.
    req(cfg);
}(this));

        
        define("common/loader/core/enum/links",{links:[{title:"JRS Settings",rel:"settings",href:"/login.html",method:"GET",mediaType:"text/html"},{title:"RequireJS configuration for JRS",name:"jrs",rel:"requirejs",href:"/{scripts}/require.config.js",method:"GET",mediaType:"application/javascript"},{title:"Xdm provider",rel:"xdm",href:"/xdm.html",method:"GET",mediaType:"text/html"}]}),define("common/loader/core/enum/relations",{XDM:"xdm",REQUIREJS:"requirejs",LOGIN:"login",SETTINGS:"settings"}),function(){function e(e,t,n){for(var r=(n||0)-1,o=e?e.length:0;++r<o;)if(e[r]===t)return r;return-1}function t(e,t){for(var n=e.criteria,r=t.criteria,o=-1,i=n.length;++o<i;){var a=n[o],s=r[o];if(a!==s){if(a>s||"undefined"==typeof a)return 1;if(s>a||"undefined"==typeof s)return-1}}return e.index-t.index}function n(e){return"\\"+Dn[e]}function r(){return dn.pop()||[]}function o(e){e.length=0,dn.length<vn&&dn.push(e)}function i(e,t,n){t||(t=0),"undefined"==typeof n&&(n=e?e.length:0);for(var r=-1,o=n-t||0,i=Array(0>o?0:o);++r<o;)i[r]=e[t+r];return i}function a(e){return e instanceof a?e:new s(e)}function s(e,t){this.__chain__=!!t,this.__wrapped__=e}function u(e){function t(){if(r){var e=i(r);Vn.apply(e,arguments)}if(this instanceof t){var a=c(n.prototype),s=n.apply(a,e||arguments);return I(s)?s:a}return n.apply(o,e||arguments)}var n=e[0],r=e[2],o=e[4];return t}function l(e,t,n,a,s){if(n){var u=n(e);if("undefined"!=typeof u)return u}var c=I(e);if(!c)return e;var f=$n.call(e);if(!Ln[f])return e;var p=or[f];switch(f){case En:case kn:return new p(+e);case Sn:case An:return new p(e);case jn:return u=p(e.source,yn.exec(e)),u.lastIndex=e.lastIndex,u}var d=ar(e);if(t){var h=!a;a||(a=r()),s||(s=r());for(var g=a.length;g--;)if(a[g]==e)return s[g];u=d?p(e.length):{}}else u=d?i(e):S({},e);return d&&(Jn.call(e,"index")&&(u.index=e.index),Jn.call(e,"input")&&(u.input=e.input)),t?(a.push(e),s.push(u),(d?ot:hr)(e,function(e,r){u[r]=l(e,t,n,a,s)}),h&&(o(a),o(s)),u):u}function c(e){return I(e)?Qn(e):{}}function f(e,t,n){if("function"!=typeof e)return Yt;if("undefined"==typeof t||!("prototype"in e))return e;switch(n){case 1:return function(n){return e.call(t,n)};case 2:return function(n,r){return e.call(t,n,r)};case 3:return function(n,r,o){return e.call(t,n,r,o)};case 4:return function(n,r,o,i){return e.call(t,n,r,o,i)}}return Ft(e,t)}function p(e){function t(){var e=l?s:this;if(o){var m=i(o);Vn.apply(m,arguments)}if((a||d)&&(m||(m=i(arguments)),a&&Vn.apply(m,a),d&&m.length<u))return r|=16,p([n,h?r:-4&r,m,null,s,u]);if(m||(m=arguments),f&&(n=e[g]),this instanceof t){e=c(n.prototype);var v=n.apply(e,m);return I(v)?v:e}return n.apply(e,m)}var n=e[0],r=e[1],o=e[2],a=e[3],s=e[4],u=e[5],l=1&r,f=2&r,d=4&r,h=8&r,g=n;return t}function d(e,t){for(var n=-1,r=T(),o=e?e.length:0,i=[];++n<o;){var a=e[n];r(t,a)<0&&i.push(a)}return i}function h(e,t,n,r){for(var o=(r||0)-1,i=e?e.length:0,a=[];++o<i;){var s=e[o];if(s&&"object"==typeof s&&"number"==typeof s.length&&(ar(s)||_(s))){t||(s=h(s,t,n));var u=-1,l=s.length,c=a.length;for(a.length+=l;++u<l;)a[c++]=s[u]}else n||a.push(s)}return a}function g(e,t,n,r){if(e===t)return 0!==e||1/e==1/t;var o=typeof e,i=typeof t;if(!(e!==e||e&&On[o]||t&&On[i]))return!1;if(null==e||null==t)return e===t;var s=$n.call(e),u=$n.call(t);if(s!=u)return!1;switch(s){case En:case kn:return+e==+t;case Sn:return e!=+e?t!=+t:0==e?1/e==1/t:e==+t;case jn:case An:return e==String(t)}var l=s==Cn;if(!l){var c=e instanceof a,f=t instanceof a;if(c||f)return g(c?e.__wrapped__:e,f?t.__wrapped__:t,n,r);if(s!=Nn)return!1;var p=e.constructor,d=t.constructor;if(p!=d&&!(B(p)&&p instanceof p&&B(d)&&d instanceof d)&&"constructor"in e&&"constructor"in t)return!1}n||(n=[]),r||(r=[]);for(var h=n.length;h--;)if(n[h]==e)return r[h]==t;var m=!0,v=0;if(n.push(e),r.push(t),l){if(v=t.length,m=v==e.length)for(;v--&&(m=g(e[v],t[v],n,r)););}else dr(t,function(t,o,i){return Jn.call(i,o)?(v++,!(m=Jn.call(e,o)&&g(e[o],t,n,r))&&gn):void 0}),m&&dr(e,function(e,t,n){return Jn.call(n,t)?!(m=--v>-1)&&gn:void 0});return n.pop(),r.pop(),m}function m(e,t,n,r,o){(ar(t)?ot:hr)(t,function(t,i){var a,s,u=t,l=e[i];if(t&&((s=ar(t))||gr(t))){for(var c=r.length;c--;)if(a=r[c]==t){l=o[c];break}if(!a){var f;n&&(u=n(l,t),(f="undefined"!=typeof u)&&(l=u)),f||(l=s?ar(l)?l:[]:gr(l)?l:{}),r.push(t),o.push(l),f||m(l,t,n,r,o)}}else n&&(u=n(l,t),"undefined"==typeof u&&(u=t)),"undefined"!=typeof u&&(l=u);e[i]=l})}function v(e,t){return e+Un(rr()*(t-e+1))}function y(e,t,n){for(var r=-1,o=T(),i=e?e.length:0,a=[],s=n?[]:a;++r<i;){var u=e[r],l=n?n(u,r,e):u;(t?!r||s[s.length-1]!==l:o(s,l)<0)&&(n&&s.push(l),a.push(u))}return a}function b(e){return function(t,n,r){var o={};n=Gt(n,r,3);var i=-1,a=t?t.length:0;if("number"==typeof a)for(;++i<a;){var s=t[i];e(o,s,n(s,i,t),t)}else hr(t,function(t,r,i){e(o,t,n(t,r,i),i)});return o}}function x(e,t,n,r,o,i){var a=2&t,s=16&t,l=32&t;if(!a&&!B(e))throw new TypeError;s&&!n.length&&(t&=-17,s=n=!1),l&&!r.length&&(t&=-33,l=r=!1);var c=1==t||17===t?u:p;return c([e,t,n,r,o,i])}function w(e){return lr[e]}function T(){var t=(t=a.indexOf)===Et?e:t;return t}function C(e){return"function"==typeof e&&Wn.test(e)}function E(e){var t,n;return e&&$n.call(e)==Nn&&(t=e.constructor,!B(t)||t instanceof t)?(dr(e,function(e,t){n=t}),"undefined"==typeof n||Jn.call(e,n)):gn}function k(e){return cr[e]}function _(e){return e&&"object"==typeof e&&"number"==typeof e.length&&$n.call(e)==Tn||!1}function S(e){if(!e)return e;for(var t=1,n=arguments.length;n>t;t++){var r=arguments[t];if(r)for(var o in r)e[o]=r[o]}return e}function N(e,t,n,r){return"boolean"!=typeof t&&null!=t&&(r=n,n=t,t=!1),l(e,t,"function"==typeof n&&f(n,r,1))}function j(e,t,n){return l(e,!0,"function"==typeof t&&f(t,n,1))}function A(e){if(!e)return e;for(var t=1,n=arguments.length;n>t;t++){var r=arguments[t];if(r)for(var o in r)"undefined"==typeof e[o]&&(e[o]=r[o])}return e}function L(e){var t=[];return dr(e,function(e,n){B(e)&&t.push(n)}),t.sort()}function O(e,t){return e?Jn.call(e,t):!1}function D(e){for(var t=-1,n=ur(e),r=n.length,o={};++t<r;){var i=n[t];o[e[i]]=i}return o}function R(e){return e===!0||e===!1||e&&"object"==typeof e&&$n.call(e)==En||!1}function M(e){return e&&"object"==typeof e&&$n.call(e)==kn||!1}function H(e){return e&&1===e.nodeType||!1}function q(e){if(!e)return!0;if(ar(e)||X(e))return!e.length;for(var t in e)if(Jn.call(e,t))return!1;return!0}function F(e,t){return g(e,t)}function P(e){return Kn(e)&&!Zn(parseFloat(e))}function B(e){return"function"==typeof e}function I(e){return!(!e||!On[typeof e])}function $(e){return z(e)&&e!=+e}function W(e){return null===e}function z(e){return"number"==typeof e||e&&"object"==typeof e&&$n.call(e)==Sn||!1}function U(e){return e&&On[typeof e]&&$n.call(e)==jn||!1}function X(e){return"string"==typeof e||e&&"object"==typeof e&&$n.call(e)==An||!1}function J(e){return"undefined"==typeof e}function V(e){var t=arguments,n=2;if(!I(e))return e;if("number"!=typeof t[2]&&(n=t.length),n>3&&"function"==typeof t[n-2])var a=f(t[--n-1],t[n--],2);else n>2&&"function"==typeof t[n-1]&&(a=t[--n]);for(var s=i(arguments,1,n),u=-1,l=r(),c=r();++u<n;)m(e,s[u],a,l,c);return o(l),o(c),e}function G(e){var t=[];dr(e,function(e,n){t.push(n)}),t=d(t,h(arguments,!0,!1,1));for(var n=-1,r=t.length,o={};++n<r;){var i=t[n];o[i]=e[i]}return o}function Q(e){for(var t=-1,n=ur(e),r=n.length,o=Array(r);++t<r;){var i=n[t];o[t]=[i,e[i]]}return o}function Y(e){for(var t=-1,n=h(arguments,!0,!1,1),r=n.length,o={};++t<r;){var i=n[t];i in e&&(o[i]=e[i])}return o}function K(e){for(var t=-1,n=ur(e),r=n.length,o=Array(r);++t<r;)o[t]=e[n[t]];return o}function Z(e,t){var n=T(),r=e?e.length:0,o=!1;return r&&"number"==typeof r?o=n(e,t)>-1:hr(e,function(e){return(o=e===t)&&gn}),o}function et(e,t,n){var r=!0;t=Gt(t,n,3);var o=-1,i=e?e.length:0;if("number"==typeof i)for(;++o<i&&(r=!!t(e[o],o,e)););else hr(e,function(e,n,o){return!(r=!!t(e,n,o))&&gn});return r}function tt(e,t,n){var r=[];t=Gt(t,n,3);var o=-1,i=e?e.length:0;if("number"==typeof i)for(;++o<i;){var a=e[o];t(a,o,e)&&r.push(a)}else hr(e,function(e,n,o){t(e,n,o)&&r.push(e)});return r}function nt(e,t,n){t=Gt(t,n,3);var r=-1,o=e?e.length:0;if("number"!=typeof o){var i;return hr(e,function(e,n,r){return t(e,n,r)?(i=e,gn):void 0}),i}for(;++r<o;){var a=e[r];if(t(a,r,e))return a}}function rt(e,t){return bt(e,t,!0)}function ot(e,t,n){var r=-1,o=e?e.length:0;if(t=t&&"undefined"==typeof n?t:f(t,n,3),"number"==typeof o)for(;++r<o&&t(e[r],r,e)!==gn;);else hr(e,t)}function it(e,t){var n=e?e.length:0;if("number"==typeof n)for(;n--&&t(e[n],n,e)!==!1;);else{var r=ur(e);n=r.length,hr(e,function(e,o,i){return o=r?r[--n]:--n,t(i[o],o,i)===!1&&gn})}}function at(e,t){var n=i(arguments,2),r=-1,o="function"==typeof t,a=e?e.length:0,s=Array("number"==typeof a?a:0);return ot(e,function(e){s[++r]=(o?t:e[t]).apply(e,n)}),s}function st(e,t,n){var r=-1,o=e?e.length:0;if(t=Gt(t,n,3),"number"==typeof o)for(var i=Array(o);++r<o;)i[r]=t(e[r],r,e);else i=[],hr(e,function(e,n,o){i[++r]=t(e,n,o)});return i}function ut(e,t,n){var r=-1/0,o=r;"function"!=typeof t&&n&&n[t]===e&&(t=null);var i=-1,a=e?e.length:0;if(null==t&&"number"==typeof a)for(;++i<a;){var s=e[i];s>o&&(o=s)}else t=Gt(t,n,3),ot(e,function(e,n,i){var a=t(e,n,i);a>r&&(r=a,o=e)});return o}function lt(e,t,n){var r=1/0,o=r;"function"!=typeof t&&n&&n[t]===e&&(t=null);var i=-1,a=e?e.length:0;if(null==t&&"number"==typeof a)for(;++i<a;){var s=e[i];o>s&&(o=s)}else t=Gt(t,n,3),ot(e,function(e,n,i){var a=t(e,n,i);r>a&&(r=a,o=e)});return o}function ct(e,t,n,r){if(!e)return n;var o=arguments.length<3;t=Gt(t,r,4);var i=-1,a=e.length;if("number"==typeof a)for(o&&(n=e[++i]);++i<a;)n=t(n,e[i],i,e);else hr(e,function(e,r,i){n=o?(o=!1,e):t(n,e,r,i)});return n}function ft(e,t,n,r){var o=arguments.length<3;return t=Gt(t,r,4),it(e,function(e,r,i){n=o?(o=!1,e):t(n,e,r,i)}),n}function pt(e,t,n){return t=Gt(t,n,3),tt(e,function(e,n,r){return!t(e,n,r)})}function dt(e,t,n){if(e&&"number"!=typeof e.length&&(e=K(e)),null==t||n)return e?e[v(0,e.length-1)]:pn;var r=ht(e);return r.length=nr(tr(0,t),r.length),r}function ht(e){var t=-1,n=e?e.length:0,r=Array("number"==typeof n?n:0);return ot(e,function(e){var n=v(0,++t);r[t]=r[n],r[n]=e}),r}function gt(e){var t=e?e.length:0;return"number"==typeof t?t:ur(e).length}function mt(e,t,n){var r;t=Gt(t,n,3);var o=-1,i=e?e.length:0;if("number"==typeof i)for(;++o<i&&!(r=t(e[o],o,e)););else hr(e,function(e,n,o){return(r=t(e,n,o))&&gn});return!!r}function vt(e,n,r){var o=-1,i=e?e.length:0,a=Array("number"==typeof i?i:0);for(n=Gt(n,r,3),ot(e,function(e,t,r){a[++o]={criteria:[n(e,t,r)],index:o,value:e}}),i=a.length,a.sort(t);i--;)a[i]=a[i].value;return a}function yt(e){return ar(e)?i(e):e&&"number"==typeof e.length?st(e):K(e)}function bt(e,t,n){return n&&q(t)?pn:(n?nt:tt)(e,t)}function xt(e){for(var t=-1,n=e?e.length:0,r=[];++t<n;){var o=e[t];o&&r.push(o)}return r}function wt(e){return d(e,h(arguments,!0,!0,1))}function Tt(e,t,n){var r=0,o=e?e.length:0;if("number"!=typeof t&&null!=t){var a=-1;for(t=Gt(t,n,3);++a<o&&t(e[a],a,e);)r++}else if(r=t,null==r||n)return e?e[0]:pn;return i(e,0,nr(tr(0,r),o))}function Ct(e,t){return h(e,t)}function Et(t,n,r){if("number"==typeof r){var o=t?t.length:0;r=0>r?tr(0,o+r):r||0}else if(r){var i=Lt(t,n);return t[i]===n?i:-1}return e(t,n,r)}function kt(e,t,n){var r=0,o=e?e.length:0;if("number"!=typeof t&&null!=t){var a=o;for(t=Gt(t,n,3);a--&&t(e[a],a,e);)r++}else r=null==t||n?1:t||r;return i(e,0,nr(tr(0,o-r),o))}function _t(){for(var e=[],t=-1,n=arguments.length;++t<n;){var r=arguments[t];(ar(r)||_(r))&&e.push(r)}var o=e[0],i=-1,a=T(),s=o?o.length:0,u=[];e:for(;++i<s;)if(r=o[i],a(u,r)<0){for(var t=n;--t;)if(a(e[t],r)<0)continue e;u.push(r)}return u}function St(e,t,n){var r=0,o=e?e.length:0;if("number"!=typeof t&&null!=t){var a=o;for(t=Gt(t,n,3);a--&&t(e[a],a,e);)r++}else if(r=t,null==r||n)return e?e[o-1]:pn;return i(e,tr(0,o-r))}function Nt(e,t,n){var r=e?e.length:0;for("number"==typeof n&&(r=(0>n?tr(0,r+n):nr(n,r-1))+1);r--;)if(e[r]===t)return r;return-1}function jt(e,t,n){e=+e||0,n=+n||1,null==t&&(t=e,e=0);for(var r=-1,o=tr(0,zn((t-e)/n)),i=Array(o);++r<o;)i[r]=e,e+=n;return i}function At(e,t,n){if("number"!=typeof t&&null!=t){var r=0,o=-1,a=e?e.length:0;for(t=Gt(t,n,3);++o<a&&t(e[o],o,e);)r++}else r=null==t||n?1:tr(0,t);return i(e,r)}function Lt(e,t,n,r){var o=0,i=e?e.length:o;for(n=n?Gt(n,r,1):Yt,t=n(t);i>o;){var a=o+i>>>1;n(e[a])<t?o=a+1:i=a}return o}function Ot(){return y(h(arguments,!0,!0))}function Dt(e,t,n,r){return"boolean"!=typeof t&&null!=t&&(r=n,n="function"!=typeof t&&r&&r[t]===e?null:t,t=!1),null!=n&&(n=Gt(n,r,3)),y(e,t,n)}function Rt(e){return d(e,i(arguments,1))}function Mt(){for(var e=-1,t=ut(br(arguments,"length")),n=Array(0>t?0:t);++e<t;)n[e]=br(arguments,e);return n}function Ht(e,t){var n=-1,r=e?e.length:0,o={};for(t||!r||ar(e[0])||(t=[]);++n<r;){var i=e[n];t?o[i]=t[n]:i&&(o[i[0]]=i[1])}return o}function qt(e,t){if(!B(t))throw new TypeError;return function(){return--e<1?t.apply(this,arguments):void 0}}function Ft(e,t){return arguments.length>2?x(e,17,i(arguments,2),null,t):x(e,1,null,null,t)}function Pt(e){for(var t=arguments.length>1?h(arguments,!0,!1,1):L(e),n=-1,r=t.length;++n<r;){var o=t[n];e[o]=x(e[o],1,null,null,e)}return e}function Bt(){for(var e=arguments,t=e.length;t--;)if(!B(e[t]))throw new TypeError;return function(){for(var t=arguments,n=e.length;n--;)t=[e[n].apply(this,t)];return t[0]}}function It(e,t,n){var r,o,i,a,s,u,l,c=0,f=!1,p=!0;if(!B(e))throw new TypeError;if(t=tr(0,t)||0,n===!0){var d=!0;p=!1}else I(n)&&(d=n.leading,f="maxWait"in n&&(tr(t,n.maxWait)||0),p="trailing"in n?n.trailing:p);var h=function(){var n=t-(xr()-a);if(0>=n){o&&clearTimeout(o);var f=l;o=u=l=pn,f&&(c=xr(),i=e.apply(s,r),u||o||(r=s=null))}else u=setTimeout(h,n)},g=function(){u&&clearTimeout(u),o=u=l=pn,(p||f!==t)&&(c=xr(),i=e.apply(s,r),u||o||(r=s=null))};return function(){if(r=arguments,a=xr(),s=this,l=p&&(u||!d),f===!1)var n=d&&!u;else{o||d||(c=a);var m=f-(a-c),v=0>=m;v?(o&&(o=clearTimeout(o)),c=a,i=e.apply(s,r)):o||(o=setTimeout(g,m))}return v&&u?u=clearTimeout(u):u||t===f||(u=setTimeout(h,t)),n&&(v=!0,i=e.apply(s,r)),!v||u||o||(r=s=null),i}}function $t(e){if(!B(e))throw new TypeError;var t=i(arguments,1);return setTimeout(function(){e.apply(pn,t)},1)}function Wt(e,t){if(!B(e))throw new TypeError;var n=i(arguments,2);return setTimeout(function(){e.apply(pn,n)},t)}function zt(e,t){var n={};return function(){var r=t?t.apply(this,arguments):mn+arguments[0];return Jn.call(n,r)?n[r]:n[r]=e.apply(this,arguments)}}function Ut(e){var t,n;if(!B(e))throw new TypeError;return function(){return t?n:(t=!0,n=e.apply(this,arguments),e=null,n)}}function Xt(e){return x(e,16,i(arguments,1))}function Jt(e,t,n){var r=!0,o=!0;if(!B(e))throw new TypeError;return n===!1?r=!1:I(n)&&(r="leading"in n?n.leading:r,o="trailing"in n?n.trailing:o),n={},n.leading=r,n.maxWait=t,n.trailing=o,It(e,t,n)}function Vt(e,t){return x(t,16,[e])}function Gt(e,t,n){var r=typeof e;if(null==e||"function"==r)return f(e,t,n);if("object"!=r)return en(e);var o=ur(e);return function(t){for(var n=o.length,r=!1;n--&&(r=t[o[n]]===e[o[n]]););return r}}function Qt(e){return null==e?"":String(e).replace(pr,w)}function Yt(e){return e}function Kt(e){ot(L(e),function(t){var n=a[t]=e[t];a.prototype[t]=function(){var e=[this.__wrapped__];Vn.apply(e,arguments);var t=n.apply(a,e);return this.__chain__?new s(t,!0):t}})}function Zt(){return Rn._=In,this}function en(e){return function(t){return t[e]}}function tn(e,t){return null==e&&null==t&&(t=1),e=+e||0,null==t?(t=e,e=0):t=+t||0,e+Un(rr()*(t-e+1))}function nn(e,t){if(e){var n=e[t];return B(n)?e[t]():n}}function rn(e,t,r){var o=a,i=o.templateSettings;e=String(e||""),r=A({},r,i);var s=0,u="__p += '",l=r.variable,c=RegExp((r.escape||xn).source+"|"+(r.interpolate||xn).source+"|"+(r.evaluate||xn).source+"|$","g");e.replace(c,function(t,r,o,i,a){return u+=e.slice(s,a).replace(wn,n),r&&(u+="' +\n_.escape("+r+") +\n'"),i&&(u+="';\n"+i+";\n__p += '"),o&&(u+="' +\n((__t = ("+o+")) == null ? '' : __t) +\n'"),s=a+t.length,t}),u+="';\n",l||(l="obj",u="with ("+l+" || {}) {\n"+u+"\n}\n"),u="function("+l+") {\nvar __t, __p = '', __j = Array.prototype.join;\nfunction print() { __p += __j.call(arguments, '') }\n"+u+"return __p\n}";try{var f=Function("_","return "+u)(o)}catch(p){throw p.source=u,p}return t?f(t):(f.source=u,f)}function on(e,t,n){e=(e=+e)>-1?e:0;var r=-1,o=Array(e);for(t=f(t,n,1);++r<e;)o[r]=t(r);return o}function an(e){return null==e?"":String(e).replace(fr,k)}function sn(e){var t=++hn+"";return e?e+t:t}function un(e){return e=new s(e),e.__chain__=!0,e}function ln(e,t){return t(e),e}function cn(){return this.__chain__=!0,this}function fn(){return this.__wrapped__}var pn,dn=[],hn=0,gn={},mn=+new Date+"",vn=40,yn=/\w*$/,bn=/<%=([\s\S]+?)%>/g,xn=/($^)/,wn=/['\n\r\t\u2028\u2029\\]/g,Tn="[object Arguments]",Cn="[object Array]",En="[object Boolean]",kn="[object Date]",_n="[object Function]",Sn="[object Number]",Nn="[object Object]",jn="[object RegExp]",An="[object String]",Ln={};Ln[_n]=!1,Ln[Tn]=Ln[Cn]=Ln[En]=Ln[kn]=Ln[Sn]=Ln[Nn]=Ln[jn]=Ln[An]=!0;var On={"boolean":!1,"function":!0,object:!0,number:!1,string:!1,undefined:!1},Dn={"\\":"\\","'":"'","\n":"n","\r":"r","	":"t","\u2028":"u2028","\u2029":"u2029"},Rn=On[typeof window]&&window||this,Mn=On[typeof exports]&&exports&&!exports.nodeType&&exports,Hn=On[typeof module]&&module&&!module.nodeType&&module,qn=Hn&&Hn.exports===Mn&&Mn,Fn=On[typeof global]&&global;!Fn||Fn.global!==Fn&&Fn.window!==Fn||(Rn=Fn);var Pn=[],Bn=Object.prototype,In=Rn._,$n=Bn.toString,Wn=RegExp("^"+String($n).replace(/[.*+?^${}()|[\]\\]/g,"\\$&").replace(/toString| for [^\]]+/g,".*?")+"$"),zn=Math.ceil,Un=Math.floor,Xn=C(Xn=Object.getPrototypeOf)&&Xn,Jn=Bn.hasOwnProperty,Vn=Pn.push,Gn=Bn.propertyIsEnumerable,Qn=C(Qn=Object.create)&&Qn,Yn=C(Yn=Array.isArray)&&Yn,Kn=Rn.isFinite,Zn=Rn.isNaN,er=C(er=Object.keys)&&er,tr=Math.max,nr=Math.min,rr=Math.random,or={};or[Cn]=Array,or[En]=Boolean,or[kn]=Date,or[_n]=Function,or[Nn]=Object,or[Sn]=Number,or[jn]=RegExp,or[An]=String,s.prototype=a.prototype;var ir={};!function(){var e={0:1,length:1};ir.spliceObjects=(Pn.splice.call(e,0,1),!e[0])}(1),a.templateSettings={escape:/<%-([\s\S]+?)%>/g,evaluate:/<%([\s\S]+?)%>/g,interpolate:bn,variable:""},Qn||(c=function(){function e(){}return function(t){if(I(t)){e.prototype=t;var n=new e;e.prototype=null}return n||Rn.Object()}}()),_(arguments)||(_=function(e){return e&&"object"==typeof e&&"number"==typeof e.length&&Jn.call(e,"callee")&&!Gn.call(e,"callee")||!1});var ar=Yn||function(e){return e&&"object"==typeof e&&"number"==typeof e.length&&$n.call(e)==Cn||!1},sr=function(e){var t,n=e,r=[];if(!n)return r;if(!On[typeof e])return r;for(t in n)Jn.call(n,t)&&r.push(t);return r},ur=er?function(e){return I(e)?er(e):[]}:sr,lr={"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#x27;"},cr=D(lr),fr=RegExp("("+ur(cr).join("|")+")","g"),pr=RegExp("["+ur(lr).join("")+"]","g"),dr=function(e,t){var n,r=e,o=r;if(!r)return o;if(!On[typeof r])return o;for(n in r)if(t(r[n],n,e)===gn)return o;return o},hr=function(e,t){var n,r=e,o=r;if(!r)return o;if(!On[typeof r])return o;for(n in r)if(Jn.call(r,n)&&t(r[n],n,e)===gn)return o;return o};B(/x/)&&(B=function(e){return"function"==typeof e&&$n.call(e)==_n});var gr=Xn?function(e){if(!e||$n.call(e)!=Nn)return!1;var t=e.valueOf,n=C(t)&&(n=Xn(t))&&Xn(n);return n?e==n||Xn(e)==n:E(e)}:E,mr=b(function(e,t,n){Jn.call(e,n)?e[n]++:e[n]=1}),vr=b(function(e,t,n){(Jn.call(e,n)?e[n]:e[n]=[]).push(t)}),yr=b(function(e,t,n){e[n]=t}),br=st,xr=C(xr=Date.now)&&xr||function(){return(new Date).getTime()};a.after=qt,a.bind=Ft,a.bindAll=Pt,a.chain=un,a.compact=xt,a.compose=Bt,a.countBy=mr,a.debounce=It,a.defaults=A,a.defer=$t,a.delay=Wt,a.difference=wt,a.filter=tt,a.flatten=Ct,a.forEach=ot,a.functions=L,a.groupBy=vr,a.indexBy=yr,a.initial=kt,a.intersection=_t,a.invert=D,a.invoke=at,a.keys=ur,a.map=st,a.max=ut,a.memoize=zt,a.merge=V,a.min=lt,a.omit=G,a.once=Ut,a.pairs=Q,a.partial=Xt,a.pick=Y,a.pluck=br,a.range=jt,a.reject=pt,a.rest=At,a.shuffle=ht,a.sortBy=vt,a.tap=ln,a.throttle=Jt,a.times=on,a.toArray=yt,a.union=Ot,a.uniq=Dt,a.values=K,a.where=bt,a.without=Rt,a.wrap=Vt,a.zip=Mt,a.collect=st,a.drop=At,a.each=ot,a.extend=S,a.methods=L,a.object=Ht,a.select=tt,a.tail=At,a.unique=Dt,a.clone=N,a.cloneDeep=j,a.contains=Z,a.escape=Qt,a.every=et,a.find=nt,a.has=O,a.identity=Yt,a.indexOf=Et,a.isArguments=_,a.isArray=ar,a.isBoolean=R,a.isDate=M,a.isElement=H,a.isEmpty=q,a.isEqual=F,a.isFinite=P,a.isFunction=B,a.isNaN=$,a.isNull=W,a.isNumber=z,a.isObject=I,a.isRegExp=U,a.isString=X,a.isUndefined=J,a.lastIndexOf=Nt,a.mixin=Kt,a.noConflict=Zt,a.random=tn,a.reduce=ct,a.reduceRight=ft,a.result=nn,a.size=gt,a.some=mt,a.sortedIndex=Lt,a.template=rn,a.unescape=an,a.uniqueId=sn,a.all=et,a.any=mt,a.detect=nt,a.findWhere=rt,a.foldl=ct,a.foldr=ft,a.include=Z,a.inject=ct,a.first=Tt,a.last=St,a.sample=dt,a.take=Tt,a.head=Tt,Kt(a),a.VERSION="2.4.1",a.prototype.chain=cn,a.prototype.value=fn,ot(["pop","push","reverse","shift","sort","splice","unshift"],function(e){var t=Pn[e];a.prototype[e]=function(){var e=this.__wrapped__;return t.apply(e,arguments),ir.spliceObjects||0!==e.length||delete e[0],this}}),ot(["concat","join","slice"],function(e){var t=Pn[e];a.prototype[e]=function(){var e=this.__wrapped__,n=t.apply(e,arguments);return this.__chain__&&(n=new s(n),n.__chain__=!0),n}}),"function"==typeof define&&"object"==typeof define.amd&&define.amd?(Rn._=a,define("lodash.custom",[],function(){return a})):Mn&&Hn?qn?(Hn.exports=a)._=a:Mn._=a:Rn._=a}.call(this),define("common/util/xssUtil",["require"],function(){var e="a,abbr,acronym,address,area,article,aside,b,bdi,bdo,big,blockquote,body,br,caption,center,center,cite,code,col,colgroup,dd,details,dfn,div,dl,dt,em,fieldset,font,footer,form,h1,h2,h3,h4,h5,h6,head,header,hr,html,i,iframe,img,input,label,legend,li,main,map,mark,menu,menuitem,meta,nav,ol,option,p,pre,section,select,small,span,strike,strong,sub,summary,sup,table,tbody,td,textarea,th,thead,title,tr,u,ul,!--",t={"(":"&#40;",")":"&#41;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;"},n=function(e){return null==e?"":e.replace(/[-\/\\^$*+?.()|[\]{}]/g,"\\$&")},r=function(e){var t=[];for(var r in e)Object.prototype.hasOwnProperty.call(e,r)&&t.push(n(r));return t},o=function(){var e,n={"&#111;":"o","&#110;":"n","&amp;":"&"};for(e in t)Object.prototype.hasOwnProperty.call(t,e)&&(n[t[e]]=e);return n}(),i="(?:"+r(t).join("|")+")",a=RegExp(i),s=RegExp(i,"g"),u="(?:\\)|\\()",l=RegExp(u),c=RegExp(u,"g"),f="\\b='(.*?)'",p=RegExp(f),d=RegExp(f,"g"),h='\\b="(.*?)"',g=RegExp(h),m=RegExp(h,"g"),v="<(?!/|"+e.replace(/,/g,"|")+")",y=RegExp(v,"i"),b=RegExp(v,"ig"),x="</(?!"+e.replace(/,/g,"|")+")",w=RegExp(x,"i"),T=RegExp(x,"ig"),C="\\bjavascript:",E=RegExp(C,"i"),k=RegExp(C,"ig"),_="\\bon(\\w+?)=",S=RegExp(_,"i"),N=RegExp(_,"ig"),j=function(e,n){if(e=null==e?"":e,n=n||{},!("string"==typeof e||e instanceof String))return e;if(n.softHTMLEscape){if(e=l.test(e)?e.replace(c,function(e){return t[e]}):e,e=p.test(e)?e.replace(d,"=`$1`").replace(/'/g,"&#39;").replace(/\b=`(.*?)`/g,"='$1'"):e.replace(/'/g,"&#39;"),e=g.test(e)?e.replace(m,"=`$1`").replace(/"/g,"&quot;").replace(/\b=`(.*?)`/g,'="$1"'):e.replace(/"/g,"&quot;"),n.whiteList&&n.whiteList instanceof Array&&n.whiteList.length>0){var r="<(?!/|"+n.whiteList.join("|")+")",o=RegExp(r,"i"),i=RegExp(r,"ig");e=o.test(e)?e.replace(i,"&lt;"):e;var u="</(?!"+n.whiteList.join("|")+")",f=RegExp(u,"i"),h=RegExp(u,"ig");e=f.test(e)?e.replace(h,"&lt;/"):e}else e=y.test(e)?e.replace(b,"&lt;"):e,e=w.test(e)?e.replace(T,"&lt;/"):e;e=E.test(e)?e.replace(k,""):e,e=S.test(e)?e.replace(N,"&#111;&#110;$1="):e}else e=a.test(e)?e.replace(s,function(e){return t[e]}):e;return e},A="(?:"+r(o).join("|")+")",L=RegExp(A,"i"),O=RegExp(A,"ig"),D=function(e){return e=null==e?"":e,("string"==typeof e||e instanceof String)&&L.test(e)?e.replace(O,function(e){return o[e]}):e};return xssUtil={escape:j,unescape:D,noConflict:function(){}},xssUtil}),!function(e,t){"use strict";var n=t.prototype.trim,r=t.prototype.trimRight,o=t.prototype.trimLeft,i=function(e){return 1*e||0},a=function(e,t){if(1>t)return"";for(var n="";t>0;)1&t&&(n+=e),t>>=1,e+=e;return n},s=[].slice,u=function(e){return null==e?"\\s":e.source?e.source:"["+d.escapeRegExp(e)+"]"},l={lt:"<",gt:">",quot:'"',apos:"'",amp:"&"},c={};for(var f in l)c[l[f]]=f;var p=function(){function e(e){return Object.prototype.toString.call(e).slice(8,-1).toLowerCase()}var n=a,r=function(){return r.cache.hasOwnProperty(arguments[0])||(r.cache[arguments[0]]=r.parse(arguments[0])),r.format.call(null,r.cache[arguments[0]],arguments)};return r.format=function(r,o){var i,a,s,u,l,c,f,d=1,h=r.length,g="",m=[];for(a=0;h>a;a++)if(g=e(r[a]),"string"===g)m.push(r[a]);else if("array"===g){if(u=r[a],u[2])for(i=o[d],s=0;s<u[2].length;s++){if(!i.hasOwnProperty(u[2][s]))throw new Error(p('[_.sprintf] property "%s" does not exist',u[2][s]));i=i[u[2][s]]}else i=u[1]?o[u[1]]:o[d++];if(/[^s]/.test(u[8])&&"number"!=e(i))throw new Error(p("[_.sprintf] expecting number but found %s",e(i)));switch(u[8]){case"b":i=i.toString(2);break;case"c":i=t.fromCharCode(i);break;case"d":i=parseInt(i,10);break;case"e":i=u[7]?i.toExponential(u[7]):i.toExponential();break;case"f":i=u[7]?parseFloat(i).toFixed(u[7]):parseFloat(i);break;case"o":i=i.toString(8);break;case"s":i=(i=t(i))&&u[7]?i.substring(0,u[7]):i;break;case"u":i=Math.abs(i);break;case"x":i=i.toString(16);break;case"X":i=i.toString(16).toUpperCase()}i=/[def]/.test(u[8])&&u[3]&&i>=0?"+"+i:i,c=u[4]?"0"==u[4]?"0":u[4].charAt(1):" ",f=u[6]-t(i).length,l=u[6]?n(c,f):"",m.push(u[5]?i+l:l+i)}return m.join("")},r.cache={},r.parse=function(e){for(var t=e,n=[],r=[],o=0;t;){if(null!==(n=/^[^\x25]+/.exec(t)))r.push(n[0]);else if(null!==(n=/^\x25{2}/.exec(t)))r.push("%");else{if(null===(n=/^\x25(?:([1-9]\d*)\$|\(([^\)]+)\))?(\+)?(0|'[^$])?(-)?(\d+)?(?:\.(\d+))?([b-fosuxX])/.exec(t)))throw new Error("[_.sprintf] huh?");if(n[2]){o|=1;var i=[],a=n[2],s=[];if(null===(s=/^([a-z_][a-z_\d]*)/i.exec(a)))throw new Error("[_.sprintf] huh?");for(i.push(s[1]);""!==(a=a.substring(s[0].length));)if(null!==(s=/^\.([a-z_][a-z_\d]*)/i.exec(a)))i.push(s[1]);else{if(null===(s=/^\[(\d+)\]/.exec(a)))throw new Error("[_.sprintf] huh?");i.push(s[1])}n[2]=i}else o|=2;if(3===o)throw new Error("[_.sprintf] mixing positional and named placeholders is not (yet) supported");r.push(n)}t=t.substring(n[0].length)}return r},r}(),d={VERSION:"2.3.0",isBlank:function(e){return null==e&&(e=""),/^\s*$/.test(e)},stripTags:function(e){return null==e?"":t(e).replace(/<\/?[^>]+>/g,"")},capitalize:function(e){return e=null==e?"":t(e),e.charAt(0).toUpperCase()+e.slice(1)},chop:function(e,n){return null==e?[]:(e=t(e),n=~~n,n>0?e.match(new RegExp(".{1,"+n+"}","g")):[e])},clean:function(e){return d.strip(e).replace(/\s+/g," ")},count:function(e,n){return null==e||null==n?0:t(e).split(n).length-1},chars:function(e){return null==e?[]:t(e).split("")},swapCase:function(e){return null==e?"":t(e).replace(/\S/g,function(e){return e===e.toUpperCase()?e.toLowerCase():e.toUpperCase()})},escapeHTML:function(e){return null==e?"":t(e).replace(/[&<>"']/g,function(e){return"&"+c[e]+";"})},unescapeHTML:function(e){return null==e?"":t(e).replace(/\&([^;]+);/g,function(e,n){var r;return n in l?l[n]:(r=n.match(/^#x([\da-fA-F]+)$/))?t.fromCharCode(parseInt(r[1],16)):(r=n.match(/^#(\d+)$/))?t.fromCharCode(~~r[1]):e})},escapeRegExp:function(e){return null==e?"":t(e).replace(/([.*+?^=!:${}()|[\]\/\\])/g,"\\$1")},splice:function(e,t,n,r){var o=d.chars(e);return o.splice(~~t,~~n,r),o.join("")},insert:function(e,t,n){return d.splice(e,t,0,n)},include:function(e,n){return""===n?!0:null==e?!1:-1!==t(e).indexOf(n)},join:function(){var e=s.call(arguments),t=e.shift();return null==t&&(t=""),e.join(t)},lines:function(e){return null==e?[]:t(e).split("\n")},reverse:function(e){return d.chars(e).reverse().join("")},startsWith:function(e,n){return""===n?!0:null==e||null==n?!1:(e=t(e),n=t(n),e.length>=n.length&&e.slice(0,n.length)===n)},endsWith:function(e,n){return""===n?!0:null==e||null==n?!1:(e=t(e),n=t(n),e.length>=n.length&&e.slice(e.length-n.length)===n)},succ:function(e){return null==e?"":(e=t(e),e.slice(0,-1)+t.fromCharCode(e.charCodeAt(e.length-1)+1))},titleize:function(e){return null==e?"":t(e).replace(/(?:^|\s)\S/g,function(e){return e.toUpperCase()})},camelize:function(e){return d.trim(e).replace(/[-_\s]+(.)?/g,function(e,t){return t.toUpperCase()})},underscored:function(e){return d.trim(e).replace(/([a-z\d])([A-Z]+)/g,"$1_$2").replace(/[-\s]+/g,"_").toLowerCase()},dasherize:function(e){return d.trim(e).replace(/([A-Z])/g,"-$1").replace(/[-_\s]+/g,"-").toLowerCase()},classify:function(e){return d.titleize(t(e).replace(/_/g," ")).replace(/\s/g,"")},humanize:function(e){return d.capitalize(d.underscored(e).replace(/_id$/,"").replace(/_/g," "))},trim:function(e,r){return null==e?"":!r&&n?n.call(e):(r=u(r),t(e).replace(new RegExp("^"+r+"+|"+r+"+$","g"),""))},ltrim:function(e,n){return null==e?"":!n&&o?o.call(e):(n=u(n),t(e).replace(new RegExp("^"+n+"+"),""))},rtrim:function(e,n){return null==e?"":!n&&r?r.call(e):(n=u(n),t(e).replace(new RegExp(n+"+$"),""))},truncate:function(e,n,r){return null==e?"":(e=t(e),r=r||"...",n=~~n,e.length>n?e.slice(0,n)+r:e)},prune:function(e,n,r){if(null==e)return"";if(e=t(e),n=~~n,r=null!=r?t(r):"...",e.length<=n)return e;var o=function(e){return e.toUpperCase()!==e.toLowerCase()?"A":" "},i=e.slice(0,n+1).replace(/.(?=\W*\w*$)/g,o);return i=i.slice(i.length-2).match(/\w\w/)?i.replace(/\s*\S+$/,""):d.rtrim(i.slice(0,i.length-1)),(i+r).length>e.length?e:e.slice(0,i.length)+r},words:function(e,t){return d.isBlank(e)?[]:d.trim(e,t).split(t||/\s+/)},pad:function(e,n,r,o){e=null==e?"":t(e),n=~~n;var i=0;switch(r?r.length>1&&(r=r.charAt(0)):r=" ",o){case"right":return i=n-e.length,e+a(r,i);case"both":return i=n-e.length,a(r,Math.ceil(i/2))+e+a(r,Math.floor(i/2));default:return i=n-e.length,a(r,i)+e}},lpad:function(e,t,n){return d.pad(e,t,n)},rpad:function(e,t,n){return d.pad(e,t,n,"right")},lrpad:function(e,t,n){return d.pad(e,t,n,"both")},sprintf:p,vsprintf:function(e,t){return t.unshift(e),p.apply(null,t)},toNumber:function(e,n){if(null==e||""==e)return 0;e=t(e);var r=i(i(e).toFixed(~~n));return 0!==r||e.match(/^0+$/)?r:Number.NaN},numberFormat:function(e,t,n,r){if(isNaN(e)||null==e)return"";e=e.toFixed(~~t),r=r||",";var o=e.split("."),i=o[0],a=o[1]?(n||".")+o[1]:"";return i.replace(/(\d)(?=(?:\d{3})+$)/g,"$1"+r)+a},strRight:function(e,n){if(null==e)return"";e=t(e),n=null!=n?t(n):n;var r=n?e.indexOf(n):-1;return~r?e.slice(r+n.length,e.length):e},strRightBack:function(e,n){if(null==e)return"";e=t(e),n=null!=n?t(n):n;var r=n?e.lastIndexOf(n):-1;return~r?e.slice(r+n.length,e.length):e},strLeft:function(e,n){if(null==e)return"";e=t(e),n=null!=n?t(n):n;var r=n?e.indexOf(n):-1;return~r?e.slice(0,r):e},strLeftBack:function(e,t){if(null==e)return"";e+="",t=null!=t?""+t:t;var n=e.lastIndexOf(t);return~n?e.slice(0,n):e},toSentence:function(e,t,n,r){t=t||", ",n=n||" and ";var o=e.slice(),i=o.pop();return e.length>2&&r&&(n=d.rtrim(t)+n),o.length?o.join(t)+n+i:i},toSentenceSerial:function(){var e=s.call(arguments);return e[3]=!0,d.toSentence.apply(d,e)},slugify:function(e){if(null==e)return"";var n="\u0105\xe0\xe1\xe4\xe2\xe3\xe5\xe6\u0107\u0119\xe8\xe9\xeb\xea\xec\xed\xef\xee\u0142\u0144\xf2\xf3\xf6\xf4\xf5\xf8\xf9\xfa\xfc\xfb\xf1\xe7\u017c\u017a",r="aaaaaaaaceeeeeiiiilnoooooouuuunczz",o=new RegExp(u(n),"g");return e=t(e).toLowerCase().replace(o,function(e){var t=n.indexOf(e);return r.charAt(t)||"-"}),d.dasherize(e.replace(/[^\w\s-]/g,""))},surround:function(e,t){return[t,e,t].join("")},quote:function(e){return d.surround(e,'"')},exports:function(){var e={};for(var t in this)this.hasOwnProperty(t)&&!t.match(/^(?:include|contains|reverse)$/)&&(e[t]=this[t]);return e},repeat:function(e,n,r){if(null==e)return"";if(n=~~n,null==r)return a(t(e),n);for(var o=[];n>0;o[--n]=e);return o.join(r)},levenshtein:function(e,n){if(null==e&&null==n)return 0;
if(null==e)return t(n).length;if(null==n)return t(e).length;e=t(e),n=t(n);for(var r,o,i=[],a=0;a<=n.length;a++)for(var s=0;s<=e.length;s++)o=a&&s?e.charAt(s-1)===n.charAt(a-1)?r:Math.min(i[s],i[s-1],r)+1:a+s,r=i[s],i[s]=o;return i.pop()}};d.strip=d.trim,d.lstrip=d.ltrim,d.rstrip=d.rtrim,d.center=d.lrpad,d.rjust=d.lpad,d.ljust=d.rpad,d.contains=d.include,d.q=d.quote,"undefined"!=typeof exports?("undefined"!=typeof module&&module.exports&&(module.exports=d),exports._s=d):"function"==typeof define&&define.amd?define("underscore.string",[],function(){return d}):(e._=e._||{},e._.string=e._.str=d)}(this,String),define("underscore",["require","lodash.custom","common/util/xssUtil","underscore.string"],function(e){"use strict";var t=e("lodash.custom"),n=e("common/util/xssUtil");t.str=e("underscore.string"),t.templateSettings={evaluate:/\{\{([\s\S]+?)\}\}/g,interpolate:/\{\{=([\s\S]+?)\}\}/g,escape:/\{\{-([\s\S]+?)\}\}/g},t.mixin(t.str.exports()),t.xssEscape=n.escape;var r=t.template;return t.template=function(e,n,o){var i=t.templateSettings,a=/($^)/;e=String(e||""),o=t.defaults({},o,i);var s=RegExp((o.escape||a).source+"|$","g");return e=e.replace(s,"{{ print(_.xssEscape($1)); }}"),r.call(t,e,n,o)},t}),function(){function e(t,r){function i(e){if(i[e]!==m)return i[e];var t;if("bug-string-char-index"==e)t="a"!="a"[0];else if("json"==e)t=i("json-stringify")&&i("json-parse");else{var n,o='{"a":[1,true,false,null,"\\u0000\\b\\n\\f\\r\\t"]}';if("json-stringify"==e){var u=r.stringify,c="function"==typeof u&&b;if(c){(n=function(){return 1}).toJSON=n;try{c="0"===u(0)&&"0"===u(new a)&&'""'==u(new s)&&u(y)===m&&u(m)===m&&u()===m&&"1"===u(n)&&"[1]"==u([n])&&"[null]"==u([m])&&"null"==u(null)&&"[null,null,null]"==u([m,y,null])&&u({a:[n,!0,!1,null,"\x00\b\n\f\r	"]})==o&&"1"===u(null,n)&&"[\n 1,\n 2\n]"==u([1,2],null,1)&&'"-271821-04-20T00:00:00.000Z"'==u(new l(-864e13))&&'"+275760-09-13T00:00:00.000Z"'==u(new l(864e13))&&'"-000001-01-01T00:00:00.000Z"'==u(new l(-621987552e5))&&'"1969-12-31T23:59:59.999Z"'==u(new l(-1))}catch(f){c=!1}}t=c}if("json-parse"==e){var p=r.parse;if("function"==typeof p)try{if(0===p("0")&&!p(!1)){n=p(o);var d=5==n.a.length&&1===n.a[0];if(d){try{d=!p('"	"')}catch(f){}if(d)try{d=1!==p("01")}catch(f){}if(d)try{d=1!==p("1.")}catch(f){}}}}catch(f){d=!1}t=d}}return i[e]=!!t}t||(t=o.Object()),r||(r=o.Object());var a=t.Number||o.Number,s=t.String||o.String,u=t.Object||o.Object,l=t.Date||o.Date,c=t.SyntaxError||o.SyntaxError,f=t.TypeError||o.TypeError,p=t.Math||o.Math,d=t.JSON||o.JSON;"object"==typeof d&&d&&(r.stringify=d.stringify,r.parse=d.parse);var h,g,m,v=u.prototype,y=v.toString,b=new l(-0xc782b5b800cec);try{b=-109252==b.getUTCFullYear()&&0===b.getUTCMonth()&&1===b.getUTCDate()&&10==b.getUTCHours()&&37==b.getUTCMinutes()&&6==b.getUTCSeconds()&&708==b.getUTCMilliseconds()}catch(x){}if(!i("json")){var w="[object Function]",T="[object Date]",C="[object Number]",E="[object String]",k="[object Array]",_="[object Boolean]",S=i("bug-string-char-index");if(!b)var N=p.floor,j=[0,31,59,90,120,151,181,212,243,273,304,334],A=function(e,t){return j[t]+365*(e-1970)+N((e-1969+(t=+(t>1)))/4)-N((e-1901+t)/100)+N((e-1601+t)/400)};if((h=v.hasOwnProperty)||(h=function(e){var t,n={};return(n.__proto__=null,n.__proto__={toString:1},n).toString!=y?h=function(e){var t=this.__proto__,n=e in(this.__proto__=null,this);return this.__proto__=t,n}:(t=n.constructor,h=function(e){var n=(this.constructor||t).prototype;return e in this&&!(e in n&&this[e]===n[e])}),n=null,h.call(this,e)}),g=function(e,t){var r,o,i,a=0;(r=function(){this.valueOf=0}).prototype.valueOf=0,o=new r;for(i in o)h.call(o,i)&&a++;return r=o=null,a?g=2==a?function(e,t){var n,r={},o=y.call(e)==w;for(n in e)o&&"prototype"==n||h.call(r,n)||!(r[n]=1)||!h.call(e,n)||t(n)}:function(e,t){var n,r,o=y.call(e)==w;for(n in e)o&&"prototype"==n||!h.call(e,n)||(r="constructor"===n)||t(n);(r||h.call(e,n="constructor"))&&t(n)}:(o=["valueOf","toString","toLocaleString","propertyIsEnumerable","isPrototypeOf","hasOwnProperty","constructor"],g=function(e,t){var r,i,a=y.call(e)==w,s=!a&&"function"!=typeof e.constructor&&n[typeof e.hasOwnProperty]&&e.hasOwnProperty||h;for(r in e)a&&"prototype"==r||!s.call(e,r)||t(r);for(i=o.length;r=o[--i];s.call(e,r)&&t(r));}),g(e,t)},!i("json-stringify")){var L={92:"\\\\",34:'\\"',8:"\\b",12:"\\f",10:"\\n",13:"\\r",9:"\\t"},O="000000",D=function(e,t){return(O+(t||0)).slice(-e)},R="\\u00",M=function(e){for(var t='"',n=0,r=e.length,o=!S||r>10,i=o&&(S?e.split(""):e);r>n;n++){var a=e.charCodeAt(n);switch(a){case 8:case 9:case 10:case 12:case 13:case 34:case 92:t+=L[a];break;default:if(32>a){t+=R+D(2,a.toString(16));break}t+=o?i[n]:e.charAt(n)}}return t+'"'},H=function(e,t,n,r,o,i,a){var s,u,l,c,p,d,v,b,x,w,S,j,L,O,R,q;try{s=t[e]}catch(F){}if("object"==typeof s&&s)if(u=y.call(s),u!=T||h.call(s,"toJSON"))"function"==typeof s.toJSON&&(u!=C&&u!=E&&u!=k||h.call(s,"toJSON"))&&(s=s.toJSON(e));else if(s>-1/0&&1/0>s){if(A){for(p=N(s/864e5),l=N(p/365.2425)+1970-1;A(l+1,0)<=p;l++);for(c=N((p-A(l,0))/30.42);A(l,c+1)<=p;c++);p=1+p-A(l,c),d=(s%864e5+864e5)%864e5,v=N(d/36e5)%24,b=N(d/6e4)%60,x=N(d/1e3)%60,w=d%1e3}else l=s.getUTCFullYear(),c=s.getUTCMonth(),p=s.getUTCDate(),v=s.getUTCHours(),b=s.getUTCMinutes(),x=s.getUTCSeconds(),w=s.getUTCMilliseconds();s=(0>=l||l>=1e4?(0>l?"-":"+")+D(6,0>l?-l:l):D(4,l))+"-"+D(2,c+1)+"-"+D(2,p)+"T"+D(2,v)+":"+D(2,b)+":"+D(2,x)+"."+D(3,w)+"Z"}else s=null;if(n&&(s=n.call(t,e,s)),null===s)return"null";if(u=y.call(s),u==_)return""+s;if(u==C)return s>-1/0&&1/0>s?""+s:"null";if(u==E)return M(""+s);if("object"==typeof s){for(O=a.length;O--;)if(a[O]===s)throw f();if(a.push(s),S=[],R=i,i+=o,u==k){for(L=0,O=s.length;O>L;L++)j=H(L,s,n,r,o,i,a),S.push(j===m?"null":j);q=S.length?o?"[\n"+i+S.join(",\n"+i)+"\n"+R+"]":"["+S.join(",")+"]":"[]"}else g(r||s,function(e){var t=H(e,s,n,r,o,i,a);t!==m&&S.push(M(e)+":"+(o?" ":"")+t)}),q=S.length?o?"{\n"+i+S.join(",\n"+i)+"\n"+R+"}":"{"+S.join(",")+"}":"{}";return a.pop(),q}};r.stringify=function(e,t,r){var o,i,a,s;if(n[typeof t]&&t)if((s=y.call(t))==w)i=t;else if(s==k){a={};for(var u,l=0,c=t.length;c>l;u=t[l++],s=y.call(u),(s==E||s==C)&&(a[u]=1));}if(r)if((s=y.call(r))==C){if((r-=r%1)>0)for(o="",r>10&&(r=10);o.length<r;o+=" ");}else s==E&&(o=r.length<=10?r:r.slice(0,10));return H("",(u={},u[""]=e,u),i,a,o,"",[])}}if(!i("json-parse")){var q,F,P=s.fromCharCode,B={92:"\\",34:'"',47:"/",98:"\b",116:"	",110:"\n",102:"\f",114:"\r"},I=function(){throw q=F=null,c()},$=function(){for(var e,t,n,r,o,i=F,a=i.length;a>q;)switch(o=i.charCodeAt(q)){case 9:case 10:case 13:case 32:q++;break;case 123:case 125:case 91:case 93:case 58:case 44:return e=S?i.charAt(q):i[q],q++,e;case 34:for(e="@",q++;a>q;)if(o=i.charCodeAt(q),32>o)I();else if(92==o)switch(o=i.charCodeAt(++q)){case 92:case 34:case 47:case 98:case 116:case 110:case 102:case 114:e+=B[o],q++;break;case 117:for(t=++q,n=q+4;n>q;q++)o=i.charCodeAt(q),o>=48&&57>=o||o>=97&&102>=o||o>=65&&70>=o||I();e+=P("0x"+i.slice(t,q));break;default:I()}else{if(34==o)break;for(o=i.charCodeAt(q),t=q;o>=32&&92!=o&&34!=o;)o=i.charCodeAt(++q);e+=i.slice(t,q)}if(34==i.charCodeAt(q))return q++,e;I();default:if(t=q,45==o&&(r=!0,o=i.charCodeAt(++q)),o>=48&&57>=o){for(48==o&&(o=i.charCodeAt(q+1),o>=48&&57>=o)&&I(),r=!1;a>q&&(o=i.charCodeAt(q),o>=48&&57>=o);q++);if(46==i.charCodeAt(q)){for(n=++q;a>n&&(o=i.charCodeAt(n),o>=48&&57>=o);n++);n==q&&I(),q=n}if(o=i.charCodeAt(q),101==o||69==o){for(o=i.charCodeAt(++q),(43==o||45==o)&&q++,n=q;a>n&&(o=i.charCodeAt(n),o>=48&&57>=o);n++);n==q&&I(),q=n}return+i.slice(t,q)}if(r&&I(),"true"==i.slice(q,q+4))return q+=4,!0;if("false"==i.slice(q,q+5))return q+=5,!1;if("null"==i.slice(q,q+4))return q+=4,null;I()}return"$"},W=function(e){var t,n;if("$"==e&&I(),"string"==typeof e){if("@"==(S?e.charAt(0):e[0]))return e.slice(1);if("["==e){for(t=[];e=$(),"]"!=e;n||(n=!0))n&&(","==e?(e=$(),"]"==e&&I()):I()),","==e&&I(),t.push(W(e));return t}if("{"==e){for(t={};e=$(),"}"!=e;n||(n=!0))n&&(","==e?(e=$(),"}"==e&&I()):I()),(","==e||"string"!=typeof e||"@"!=(S?e.charAt(0):e[0])||":"!=$())&&I(),t[e.slice(1)]=W($());return t}I()}return e},z=function(e,t,n){var r=U(e,t,n);r===m?delete e[t]:e[t]=r},U=function(e,t,n){var r,o=e[t];if("object"==typeof o&&o)if(y.call(o)==k)for(r=o.length;r--;)z(o,r,n);else g(o,function(e){z(o,e,n)});return n.call(e,t,o)};r.parse=function(e,t){var n,r;return q=0,F=""+e,n=W($()),"$"!=$()&&I(),q=F=null,t&&y.call(t)==w?U((r={},r[""]=n,r),"",t):n}}}return r.runInContext=e,r}var t="function"==typeof define&&define.amd,n={"function":!0,object:!0},r=n[typeof exports]&&exports&&!exports.nodeType&&exports,o=n[typeof window]&&window||this,i=r&&n[typeof module]&&module&&!module.nodeType&&"object"==typeof global&&global;if(!i||i.global!==i&&i.window!==i&&i.self!==i||(o=i),r&&!t)e(o,r);else{var a=o.JSON,s=o.JSON3,u=!1,l=e(o,o.JSON3={noConflict:function(){return u||(u=!0,o.JSON=a,o.JSON3=s,a=s=null),l}});o.JSON={parse:l.parse,stringify:l.stringify}}t&&define("json3",[],function(){return l})}.call(this),function(e,t){"function"==typeof define&&define.amd?define("common/enum/loggingLevels",[],e):(t.logging||(t.logging={}),t.logging.loggingLevels=e())}(function(){return{DEBUG:100,INFO:200,WARN:300,ERROR:400}},this),function(e,t){"function"==typeof define&&define.amd?define("common/logging/Level",["common/enum/loggingLevels"],e):(t.logging||(t.logging={}),t.logging.Level=e(t.logging.loggingLevels))}(function(e){function t(e,t){this.level=e,this.name=t.toUpperCase()}t.prototype.isGreaterOrEqual=function(e){var n=(e instanceof t?e:t.getLevel(e)).level;return this.level>=n},t.prototype.toString=function(){return this.name},t.getLevel=function(e){return t[e.toUpperCase()]};for(var n in e)e.hasOwnProperty(n)&&(t[n]=new t(e[n],n));return t},this),function(e,t){"function"==typeof define&&define.amd?define("common/logging/LogItem",[],e):(t.logging||(t.logging={}),t.logging.LogItem=e())}(function(){"use strict";function e(e){var t,n=e.getHours().toString(),r=e.getMinutes().toString(),o=e.getSeconds().toString(),i=e.getMilliseconds();return 1==n.length&&(n="0"+n),1==r.length&&(r="0"+r),1==o.length&&(o="0"+o),t=n+":"+r+":"+o+"."+i}function t(e){for(var t in e)if(e.hasOwnProperty(t)){if("args"===t)for(var n=0,r=e[t].length;r>n;n++)e[t][n]instanceof Error&&(e[t][n]=e[t][n].message);this[t]=e[t]}}return t.prototype.toArray=function(){var t=[];return t.push(e(this.time)),t.push("["+this.id+"]"),"unknown"!==this.file&&t.push("["+this.file+":"+this.line+"]"),t.push("["+this.level.toString()+"] -"),t=t.concat(this.args)},t},this),function(e,t){"function"==typeof define&&define.amd?define("common/logging/Log",["common/logging/Level","common/logging/LogItem"],e):(t.logging||(t.logging={}),t.logging.Log=e(t.logging.Level,t.logging.LogItem))}(function(e,t){function n(t){return function(){return this._prepareLogItem({level:e.getLevel(t),args:arguments})}}function r(e,t){this._id=e.id,this._callback=t}return r.prototype._prepareLogItem=function(e){e.id=this._id,e.args=Array.prototype.slice.call(e.args,0),e.time=new Date;var n=(new Error).stack;if(n){var r=n.split("\n")[2],o=r.match(/\/(\w+\.\w+):(\d+)/i);o&&(e.file=o[1],e.line=o[2])}return e.file||(e.file="unknown",e.line="0"),e=new t(e),this._callback(e),e},r.prototype.debug=n("debug"),r.prototype.info=n("info"),r.prototype.warn=n("warn"),r.prototype.error=n("error"),r},this),function(e,t){"function"==typeof define&&define.amd?define("common/logging/appender/ConsoleAppender",[],e):(t.logging||(t.logging={}),t.logging.appender||(t.logging.appender={}),t.logging.appender.ConsoleAppender=e())}(function(){function e(){}return e.prototype.console=function(){if("undefined"==typeof console){var e=function(){};return{assert:e,clear:e,count:e,debug:e,dir:e,dirxml:e,error:e,group:e,groupCollapsed:e,groupEnd:e,info:e,log:e,markTimeline:e,profile:e,profileEnd:e,table:e,time:e,timeEnd:e,timeStamp:e,trace:e,warn:e}}return console}(),e.prototype.write=function(e){var t=this.console.log;switch(e.level.toString()){case"DEBUG":t=this.console.debug||this.console.log;break;case"INFO":t=this.console.info||this.console.log;break;case"WARN":t=this.console.warn;break;case"ERROR":t=this.console.error}try{t.apply(this.console,e.toArray())}catch(n){try{Function.prototype.apply.call(t,this.console,e.toArray())}catch(r){}}},e},this),function(e,t,n){"function"==typeof define&&define.amd?define("common/logging/LoggerManager",["underscore","common/logging/Log","common/logging/Level","common/logging/appender/ConsoleAppender"],e):(t.logging||(t.logging={}),t.logging.LoggerManager=e(n,t.logging.Log,t.logging.Level,t.logging.appender.ConsoleAppender))}(function(e,t,n,r){var o={console:r},i=function(e){this.initialize(e||{})};return e.extend(i.prototype,{defaults:function(){return{enabled:!1,level:"error",appenders:{},appenderInstances:{},loggers:{}}},initialize:function(t){this.attributes=e.defaults(t,this.defaults());var n={};e.each(o,function(e,t){n[t]=new e}),this.set("appenderInstances",n)},get:function(e){return this.attributes[e]},set:function(e,t){this.attributes[e]=t},register:function(n){var r={id:"root"};if("string"==typeof n&&""!==n?r.id=n:n&&n.hasOwnProperty("id")&&(r.id=n.id),!this.get("loggers").hasOwnProperty(r.id)){var o=this.get("loggers");o[r.id]=new t(r,e.bind(this._processLogItem,this)),this.set("loggers",o)}return this.get("loggers")[r.id]},disable:function(){this.set("enabled",!1)},enable:function(e){e&&this.set("level",n.getLevel(e)),this.set("enabled",!0)},setLevel:function(e){this.set("level",e)},_processLogItem:function(e){this.get("enabled")&&e.level.isGreaterOrEqual(this.get("level"))&&this._appendLogItem(e)},_appendLogItem:function(e){var t=this.get("appenders"),n=this.get("appenderInstances");for(var r in t)n.hasOwnProperty(t[r])&&n[t[r]].write(e)}}),i},this,this._),define("logger",["require","exports","module","common/logging/LoggerManager"],function(e,t,n){"use strict";var r=e("common/logging/LoggerManager"),o=n.config(),i=new r(o);return i}),function(e){"use strict";define("xdm",["json3","logger"],e)}(function(e,t){function n(e,t){var n=typeof e[t];return"function"==n||!("object"!=n||!e[t])||"unknown"==n}function r(e,t){return!("object"!=typeof e[t]||!e[t])}function o(e){return"[object Array]"===Object.prototype.toString.call(e)}function i(){var e="Shockwave Flash",t="application/x-shockwave-flash";if(!g(navigator.plugins)&&"object"==typeof navigator.plugins[e]){var n=navigator.plugins[e].description;n&&!g(navigator.mimeTypes)&&navigator.mimeTypes[t]&&navigator.mimeTypes[t].enabledPlugin&&(E=n.match(/\d+/g))}if(!E){var r;try{r=new ActiveXObject("ShockwaveFlash.ShockwaveFlash"),E=Array.prototype.slice.call(r.GetVariable("$version").match(/(\d+),(\d+),(\d+),(\d+)/),1),r=null}catch(o){}}if(!E)return!1;var i=parseInt(E[0],10),a=parseInt(E[1],10);return k=i>9&&a>0,!0}function a(){if(!W){W=!0,I("firing dom_onReady");for(var e=0;e<z.length;e++)z[e]();z.length=0}}function s(e,t){return W?void e.call(t):void z.push(function(){e.call(t)})}function u(){var e=parent;if(""!==H)for(var t=0,n=H.split(".");t<n.length;t++)e=e[n[t]];return e.easyXDM}function l(e){return I("Settings namespace to '"+e+"'"),window.easyXDM=F,H=e,H&&(P="easyXDM_"+H.replace(".","_")+"_"),q}function c(e){return e.match(D)[3]}function f(e){return e.match(D)[4]||""}function p(e){var t=e.toLowerCase().match(D),n=t[2],r=t[3],o=t[4]||"";return("http:"==n&&":80"==o||"https:"==n&&":443"==o)&&(o=""),n+"//"+r+o}function d(e){if(e=e.replace(M,"$1/"),!e.match(/^(http||https):\/\//)){var t="/"===e.substring(0,1)?"":location.pathname;"/"!==t.substring(t.length-1)&&(t=t.substring(0,t.lastIndexOf("/")+1)),e=location.protocol+"//"+location.host+t+e}for(;R.test(e);)e=e.replace(R,"");return I("resolved url '"+e+"'"),e}function h(e,t){var n="",r=e.indexOf("#");-1!==r&&(n=e.substring(r),e=e.substring(0,r));var o=[];for(var i in t)t.hasOwnProperty(i)&&o.push(i+"="+encodeURIComponent(t[i]));return e+(B?"#":-1==e.indexOf("?")?"?":"&")+o.join("&")+n}function g(e){return"undefined"==typeof e}function m(e,t,n){var r;for(var o in t)t.hasOwnProperty(o)&&(o in e?(r=t[o],"object"==typeof r?m(e[o],r,n):n||(e[o]=t[o])):e[o]=t[o]);return e}function v(){var e=document.body.appendChild(document.createElement("form")),t=e.appendChild(document.createElement("input"));t.name=P+"TEST"+L,C=t!==e.elements[t.name],document.body.removeChild(e),I("HAS_NAME_PROPERTY_BUG: "+C)}function y(e){I("creating frame: "+e.props.src),g(C)&&v();var t;C?t=document.createElement('<iframe name="'+e.props.name+'"/>'):(t=document.createElement("IFRAME"),t.name=e.props.name),t.id=t.name=e.props.name,delete e.props.name,"string"==typeof e.container&&(e.container=document.getElementById(e.container)),e.container||(m(t.style,{position:"absolute",top:"-2000px",left:"0px"}),e.container=document.body);var n=e.props.src;if(e.props.src="javascript:false",m(t,e.props),t.border=t.frameBorder=0,t.allowTransparency=!0,e.container.appendChild(t),e.onLoad&&_(t,"load",e.onLoad),e.usePost){var r,o=e.container.appendChild(document.createElement("form"));if(o.target=t.name,o.action=n,o.method="POST","object"==typeof e.usePost)for(var i in e.usePost)e.usePost.hasOwnProperty(i)&&(C?r=document.createElement('<input name="'+i+'"/>'):(r=document.createElement("INPUT"),r.name=i),r.value=e.usePost[i],o.appendChild(r));o.submit(),o.parentNode.removeChild(o)}else t.src=n;return e.props.src=n,t}function b(e,t){"string"==typeof e&&(e=[e]);for(var n,r=e.length;r--;)if(n=e[r],n=new RegExp("^"==n.substr(0,1)?n:"^"+n.replace(/(\*)/g,".$1").replace(/\?/g,".")+"$"),n.test(t))return!0;return!1}function x(e){var t,r=e.protocol;if(e.isHost=e.isHost||g(X.xdm_p),B=e.hash||!1,I("preparing transport stack"),e.props||(e.props={}),e.isHost)e.remote=d(e.remote),e.channel=e.channel||"default"+L++,e.secret=Math.random().toString(16).substring(2),g(r)?(r=p(location.href)==p(e.remote)?"4":n(window,"postMessage")||n(document,"postMessage")?"1":e.swf&&n(window,"ActiveXObject")&&i()?"6":"Gecko"===navigator.product&&"frameElement"in window&&-1==navigator.userAgent.indexOf("WebKit")?"5":e.remoteHelper?"2":"0",I("selecting protocol: "+r)):I("using protocol: "+r);else if(I("using parameters from query"),e.channel=X.xdm_c.replace(/["'<>\\]/g,""),e.secret=X.xdm_s,e.remote=X.xdm_e.replace(/["'<>\\]/g,""),r=X.xdm_p,e.acl&&!b(e.acl,e.remote))throw new Error("Access denied for "+e.remote);switch(e.protocol=r,r){case"0":if(m(e,{interval:100,delay:2e3,useResize:!0,useParent:!1,usePolling:!1},!0),e.isHost){if(!e.local){I("looking for image to use as local");for(var o,a=location.protocol+"//"+location.host,s=document.body.getElementsByTagName("img"),u=s.length;u--;)if(o=s[u],o.src.substring(0,a.length)===a){e.local=o.src;break}e.local||(I("no image found, defaulting to using the window"),e.local=window)}var l={xdm_c:e.channel,xdm_p:0};e.local===window?(e.usePolling=!0,e.useParent=!0,e.local=location.protocol+"//"+location.host+location.pathname+location.search,l.xdm_e=e.local,l.xdm_pa=1):l.xdm_e=d(e.local),e.container&&(e.useResize=!1,l.xdm_po=1),e.remote=h(e.remote,l)}else m(e,{channel:X.xdm_c,remote:X.xdm_e,useParent:!g(X.xdm_pa),usePolling:!g(X.xdm_po),useResize:e.useParent?!1:e.useResize});t=[new q.stack.HashTransport(e),new q.stack.ReliableBehavior({}),new q.stack.QueueBehavior({encode:!0,maxLength:4e3-e.remote.length}),new q.stack.VerifyBehavior({initiate:e.isHost})];break;case"1":t=[new q.stack.PostMessageTransport(e)];break;case"2":e.isHost&&(e.remoteHelper=d(e.remoteHelper)),t=[new q.stack.NameTransport(e),new q.stack.QueueBehavior,new q.stack.VerifyBehavior({initiate:e.isHost})];break;case"3":t=[new q.stack.NixTransport(e)];break;case"4":t=[new q.stack.SameOriginTransport(e)];break;case"5":t=[new q.stack.FrameElementTransport(e)];break;case"6":E||i(),t=[new q.stack.FlashTransport(e)]}return t.push(new q.stack.QueueBehavior({lazy:e.lazy,remove:!0})),t}function w(e){for(var t,n={incoming:function(e,t){this.up.incoming(e,t)},outgoing:function(e,t){this.down.outgoing(e,t)},callback:function(e){this.up.callback(e)},init:function(){this.down.init()},destroy:function(){this.down.destroy()}},r=0,o=e.length;o>r;r++)t=e[r],m(t,n,!0),0!==r&&(t.down=e[r-1]),r!==o-1&&(t.up=e[r+1]);return t}function T(e){e.up.down=e.down,e.down.up=e.up,e.up=e.down=null}if("undefined"==typeof document)return{};var C,E,k,_,S,N=window.setTimeout,j=t.register("EasyXDM"),A=this,L=Math.floor(1e4*Math.random()),O=Function.prototype,D=/^((http.?:)\/\/([^:\/\s]+)(:\d+)*)/,R=/[\-\w]+\/\.\.\//,M=/([^:])\/\//g,H="",q={},F=window.easyXDM,P="easyXDM_",B=!1,I=O;if(n(window,"addEventListener"))_=function(e,t,n){I("adding listener "+t),e.addEventListener(t,n,!1)},S=function(e,t,n){I("removing listener "+t),e.removeEventListener(t,n,!1)};else{if(!n(window,"attachEvent"))throw new Error("Browser not supported");_=function(e,t,n){I("adding listener "+t),e.attachEvent("on"+t,n)},S=function(e,t,n){I("removing listener "+t),e.detachEvent("on"+t,n)}}var $,W=!1,z=[];if("readyState"in document?($=document.readyState,W="complete"==$||~navigator.userAgent.indexOf("AppleWebKit/")&&("loaded"==$||"interactive"==$)):W=!!document.body,!W){if(n(window,"addEventListener"))_(document,"DOMContentLoaded",a);else if(_(document,"readystatechange",function(){"complete"==document.readyState&&a()}),document.documentElement.doScroll&&window===top){var U=function(){if(!W){try{document.documentElement.doScroll("left")}catch(e){return void N(U,1)}a()}};U()}_(window,"load",a)}var X=function(e){e=e.substring(1).split("&");for(var t,n={},r=e.length;r--;)t=e[r].split("="),n[t[0]]=decodeURIComponent(t[1]);return n}(/xdm_e=/.test(location.search)?location.search:location.hash),J=function(){var e={},t={a:[1,2,3]},n='{"a":[1,2,3]}';return"undefined"!=typeof JSON&&"function"==typeof JSON.stringify&&JSON.stringify(t).replace(/\s/g,"")===n?JSON:(Object.toJSON&&Object.toJSON(t).replace(/\s/g,"")===n&&(e.stringify=Object.toJSON),"function"==typeof String.prototype.evalJSON&&(t=n.evalJSON(),t.a&&3===t.a.length&&3===t.a[2]&&(e.parse=function(e){return e.evalJSON()})),e.stringify&&e.parse?(J=function(){return e},e):null)};m(q,{version:"2.4.19.1",query:X,stack:{},apply:m,getJSONObject:J,whenReady:s,noConflict:l});var V={_deferred:[],flush:function(){this.trace("... deferred messages ...");for(var e=0,t=this._deferred.length;t>e;e++)this.trace(this._deferred[e]);this._deferred.length=0,this.trace("... end of deferred messages ...")},getTime:function(){var e=new Date,t=e.getHours()+"",n=e.getMinutes()+"",r=e.getSeconds()+"",o=e.getMilliseconds()+"",i="000";return 1==t.length&&(t="0"+t),1==n.length&&(n="0"+n),1==r.length&&(r="0"+r),o=i.substring(o.length)+o,t+":"+n+":"+r+"."+o},log:function(e){j.debug(location.host+(H?":"+H:"")+": "+e)},getTracer:function(e){var n=t.register(e);return function(e){n.debug(e)}}};return V.log("easyXDM present on '"+location.href),q.Debug=V,I=V.getTracer("EasyXDM.{Private}"),q.DomHelper={on:_,un:S,requiresJSON:function(e){r(window,"JSON")?V.log("native JSON found"):(V.log("loading external JSON"),document.write('<script type="text/javascript" src="'+e+'"></script>'))}},function(){var e={};q.Fn={set:function(t,n){this._trace("storing function "+t),e[t]=n},get:function(t,n){if(this._trace("retrieving function "+t),e.hasOwnProperty(t)){var r=e[t];return r||this._trace(t+" not found"),n&&delete e[t],r}}},q.Fn._trace=V.getTracer("easyXDM.Fn")}(),q.Socket=function(e){var t=V.getTracer("easyXDM.Socket");t("constructor");var n=w(x(e).concat([{incoming:function(t,n){e.onMessage(t,n)},callback:function(t){e.onReady&&e.onReady(t)}}])),r=p(e.remote);this.origin=p(e.remote),this.destroy=function(){n.destroy()},this.postMessage=function(e){n.outgoing(e,r)},n.init()},q.Rpc=function(e,t){var n=V.getTracer("easyXDM.Rpc");if(n("constructor"),t.local)for(var r in t.local)if(t.local.hasOwnProperty(r)){var o=t.local[r];"function"==typeof o&&(t.local[r]={method:o})}var i=w(x(e).concat([new q.stack.RpcBehavior(this,t),{callback:function(t){e.onReady&&e.onReady(t)}}]));this.origin=p(e.remote),this.destroy=function(){i.destroy()},i.init()},q.stack.SameOriginTransport=function(e){var t=V.getTracer("easyXDM.stack.SameOriginTransport");t("constructor");var n,r,o,i;return n={outgoing:function(e,t,n){o(e),n&&n()},destroy:function(){t("destroy"),r&&(r.parentNode.removeChild(r),r=null)},onDOMReady:function(){t("init"),i=p(e.remote),e.isHost?(m(e.props,{src:h(e.remote,{xdm_e:location.protocol+"//"+location.host+location.pathname,xdm_c:e.channel,xdm_p:4}),name:P+e.channel+"_provider"}),r=y(e),q.Fn.set(e.channel,function(e){return o=e,N(function(){n.up.callback(!0)},0),function(e){n.up.incoming(e,i)}})):(o=u().Fn.get(e.channel,!0)(function(e){n.up.incoming(e,i)}),N(function(){n.up.callback(!0)},0))},init:function(){s(n.onDOMReady,n)}}},q.stack.FlashTransport=function(e){function t(e){N(function(){r("received message"),o.up.incoming(e,a)},0)}function n(t){r("creating factory with SWF from "+t);var n=e.swf+"?host="+e.isHost,o="easyXDM_swf_"+Math.floor(1e4*Math.random());q.Fn.set("flash_loaded"+t.replace(/[\-.]/g,"_"),function(){q.stack.FlashTransport[t].swf=u=l.firstChild;for(var e=q.stack.FlashTransport[t].queue,n=0;n<e.length;n++)e[n]();e.length=0}),e.swfContainer?l="string"==typeof e.swfContainer?document.getElementById(e.swfContainer):e.swfContainer:(l=document.createElement("div"),m(l.style,k&&e.swfNoThrottle?{height:"20px",width:"20px",position:"fixed",right:0,top:0}:{height:"1px",width:"1px",position:"absolute",overflow:"hidden",right:0,top:0}),document.body.appendChild(l));var i="callback=flash_loaded"+encodeURIComponent(t.replace(/[\-.]/g,"_"))+"&proto="+A.location.protocol+"&domain="+encodeURIComponent(c(A.location.href))+"&port="+encodeURIComponent(f(A.location.href))+"&ns="+encodeURIComponent(H);i+="&log=true",l.innerHTML="<object height='20' width='20' type='application/x-shockwave-flash' id='"+o+"' data='"+n+"'><param name='allowScriptAccess' value='always'></param><param name='wmode' value='transparent'><param name='movie' value='"+n+"'></param><param name='flashvars' value='"+i+"'></param><embed type='application/x-shockwave-flash' FlashVars='"+i+"' allowScriptAccess='always' wmode='transparent' src='"+n+"' height='1' width='1'></embed></object>"}var r=V.getTracer("easyXDM.stack.FlashTransport");r("constructor");var o,i,a,u,l;return o={outgoing:function(t,n,r){u.postMessage(e.channel,t.toString()),r&&r()},destroy:function(){r("destroy");try{u.destroyChannel(e.channel)}catch(t){}u=null,i&&(i.parentNode.removeChild(i),i=null)},onDOMReady:function(){r("init"),a=e.remote,q.Fn.set("flash_"+e.channel+"_init",function(){N(function(){r("firing onReady"),o.up.callback(!0)})}),q.Fn.set("flash_"+e.channel+"_onMessage",t),e.swf=d(e.swf);var s=c(e.swf),l=function(){q.stack.FlashTransport[s].init=!0,u=q.stack.FlashTransport[s].swf,u.createChannel(e.channel,e.secret,p(e.remote),e.isHost),e.isHost&&(k&&e.swfNoThrottle&&m(e.props,{position:"fixed",right:0,top:0,height:"20px",width:"20px"}),m(e.props,{src:h(e.remote,{xdm_e:p(location.href),xdm_c:e.channel,xdm_p:6,xdm_s:e.secret}),name:P+e.channel+"_provider"}),i=y(e))};q.stack.FlashTransport[s]&&q.stack.FlashTransport[s].init?l():q.stack.FlashTransport[s]?q.stack.FlashTransport[s].queue.push(l):(q.stack.FlashTransport[s]={queue:[l]},n(s))},init:function(){s(o.onDOMReady,o)}}},q.stack.PostMessageTransport=function(e){function t(e){if(e.origin)return p(e.origin);if(e.uri)return p(e.uri);if(e.domain)return location.protocol+"//"+e.domain;throw"Unable to retrieve the origin of the event"}function n(n){var i=t(n);r("received message '"+n.data+"' from "+i),i==u&&n.data.substring(0,e.channel.length+1)==e.channel+" "&&o.up.incoming(n.data.substring(e.channel.length+1),i)}var r=V.getTracer("easyXDM.stack.PostMessageTransport");r("constructor");var o,i,a,u;return o={outgoing:function(t,n,r){a.postMessage(e.channel+" "+t,n||u),r&&r()},destroy:function(){r("destroy"),S(window,"message",n),i&&(a=null,i.parentNode.removeChild(i),i=null)},onDOMReady:function(){if(r("init"),u=p(e.remote),e.isHost){var t=function(s){s.data==e.channel+"-ready"&&(r("firing onReady"),a="postMessage"in i.contentWindow?i.contentWindow:i.contentWindow.document,S(window,"message",t),_(window,"message",n),N(function(){o.up.callback(!0)},0))};_(window,"message",t),m(e.props,{src:h(e.remote,{xdm_e:p(location.href),xdm_c:e.channel,xdm_p:1}),name:P+e.channel+"_provider"}),i=y(e)}else _(window,"message",n),a="postMessage"in window.parent?window.parent:window.parent.document,a.postMessage(e.channel+"-ready",u),N(function(){o.up.callback(!0)},0)},init:function(){s(o.onDOMReady,o)}}},q.stack.FrameElementTransport=function(e){var t=V.getTracer("easyXDM.stack.FrameElementTransport");t("constructor");var n,r,o,i;return n={outgoing:function(e,t,n){o.call(this,e),n&&n()},destroy:function(){t("destroy"),r&&(r.parentNode.removeChild(r),r=null)},onDOMReady:function(){t("init"),i=p(e.remote),e.isHost?(m(e.props,{src:h(e.remote,{xdm_e:p(location.href),xdm_c:e.channel,xdm_p:5}),name:P+e.channel+"_provider"}),r=y(e),r.fn=function(e){return delete r.fn,o=e,N(function(){n.up.callback(!0)},0),function(e){n.up.incoming(e,i)}}):(document.referrer&&p(document.referrer)!=X.xdm_e&&(window.top.location=X.xdm_e),o=window.frameElement.fn(function(e){n.up.incoming(e,i)}),n.up.callback(!0))},init:function(){s(n.onDOMReady,n)}}},q.stack.NameTransport=function(e){function t(t){var n=e.remoteHelper+(u?"#_3":"#_2")+e.channel;i("sending message "+t),i("navigating to  '"+n+"'"),l.contentWindow.sendMessage(t,n)}function n(){u?2!==++f&&u||a.up.callback(!0):(t("ready"),i("calling onReady"),a.up.callback(!0))}function r(e){i("received message "+e),a.up.incoming(e,b)}function o(){v&&N(function(){v(!0)},0)}var i=V.getTracer("easyXDM.stack.NameTransport");i("constructor"),e.isHost&&g(e.remoteHelper)&&i("missing remoteHelper");var a,u,l,c,f,v,b,x;return a={outgoing:function(e,n,r){v=r,t(e)},destroy:function(){i("destroy"),l.parentNode.removeChild(l),l=null,u&&(c.parentNode.removeChild(c),c=null)},onDOMReady:function(){i("init"),u=e.isHost,f=0,b=p(e.remote),e.local=d(e.local),u?(q.Fn.set(e.channel,function(t){i("received initial message "+t),u&&"ready"===t&&(q.Fn.set(e.channel,r),n())}),x=h(e.remote,{xdm_e:e.local,xdm_c:e.channel,xdm_p:2}),m(e.props,{src:x+"#"+e.channel,name:P+e.channel+"_provider"}),c=y(e)):(e.remoteHelper=e.remote,q.Fn.set(e.channel,r));var t=function(){var r=l||this;S(r,"load",t),q.Fn.set(e.channel+"_load",o),function i(){"function"==typeof r.contentWindow.sendMessage?n():N(i,50)}()};l=y({props:{src:e.local+"#_4"+e.channel},onLoad:t})},init:function(){s(a.onDOMReady,a)}}},q.stack.HashTransport=function(e){function t(t){if(i("sending message '"+(d+1)+" "+t+"' to "+b),!g)return void i("no caller window");var n=e.remote+"#"+d++ +"_"+t;(u||!v?g.contentWindow:g).location=n}function n(e){f=e,i("received message '"+f+"' from "+b),a.up.incoming(f.substring(f.indexOf("_")+1),b)}function r(){if(h){var e=h.location.href,t="",r=e.indexOf("#");-1!=r&&(t=e.substring(r)),t&&t!=f&&(i("poll: new message"),n(t))}}function o(){i("starting polling"),l=setInterval(r,c)}var i=V.getTracer("easyXDM.stack.HashTransport");i("constructor");var a,u,l,c,f,d,h,g,v,b;return a={outgoing:function(e){t(e)},destroy:function(){window.clearInterval(l),(u||!v)&&g.parentNode.removeChild(g),g=null},onDOMReady:function(){if(u=e.isHost,c=e.interval,f="#"+e.channel,d=0,v=e.useParent,b=p(e.remote),u){if(m(e.props,{src:e.remote,name:P+e.channel+"_provider"}),v)e.onLoad=function(){h=window,o(),a.up.callback(!0)};else{var t=0,n=e.delay/50;!function r(){if(++t>n)throw i("unable to get reference to _listenerWindow, giving up"),new Error("Unable to reference listenerwindow");try{h=g.contentWindow.frames[P+e.channel+"_consumer"]}catch(s){}h?(o(),i("got a reference to _listenerWindow"),a.up.callback(!0)):N(r,50)}()}g=y(e)}else h=window,o(),v?(g=parent,a.up.callback(!0)):(m(e,{props:{src:e.remote+"#"+e.channel+new Date,name:P+e.channel+"_consumer"},onLoad:function(){a.up.callback(!0)}}),g=y(e))},init:function(){s(a.onDOMReady,a)}}},q.stack.ReliableBehavior=function(){var e=V.getTracer("easyXDM.stack.ReliableBehavior");
e("constructor");var t,n,r=0,o=0,i="";return t={incoming:function(a,s){e("incoming: "+a);var u=a.indexOf("_"),l=a.substring(0,u).split(",");a=a.substring(u+1),l[0]==r&&(e("message delivered"),i="",n&&n(!0)),a.length>0&&(e("sending ack, and passing on "+a),t.down.outgoing(l[1]+","+r+"_"+i,s),o!=l[1]&&(o=l[1],t.up.incoming(a,s)))},outgoing:function(e,a,s){i=e,n=s,t.down.outgoing(o+","+ ++r+"_"+e,a)}}},q.stack.QueueBehavior=function(e){function t(){if(e.remove&&0===i.length)return n("removing myself from the stack"),void T(r);if(!a&&0!==i.length&&!o){n("dispatching from queue"),a=!0;var s=i.shift();r.down.outgoing(s.data,s.origin,function(e){a=!1,s.callback&&N(function(){s.callback(e)},0),t()})}}var n=V.getTracer("easyXDM.stack.QueueBehavior");n("constructor");var r,o,i=[],a=!0,s="",u=0,l=!1,c=!1;return r={init:function(){g(e)&&(e={}),e.maxLength&&(u=e.maxLength,c=!0),e.lazy?l=!0:r.down.init()},callback:function(e){a=!1;var n=r.up;t(),n.callback(e)},incoming:function(t,o){if(c){var i=t.indexOf("_"),a=parseInt(t.substring(0,i),10);s+=t.substring(i+1),0===a?(n("received the last fragment"),e.encode&&(s=decodeURIComponent(s)),r.up.incoming(s,o),s=""):n("waiting for more fragments, seq="+t)}else r.up.incoming(t,o)},outgoing:function(o,a,s){e.encode&&(o=encodeURIComponent(o));var f,p=[];if(c){for(;0!==o.length;)f=o.substring(0,u),o=o.substring(f.length),p.push(f);for(;f=p.shift();)n("enqueuing"),i.push({data:p.length+"_"+f,origin:a,callback:0===p.length?s:null})}else i.push({data:o,origin:a,callback:s});l?r.down.init():t()},destroy:function(){n("destroy"),o=!0,r.down.destroy()}}},q.stack.VerifyBehavior=function(e){function t(){n("requesting verification"),o=Math.random().toString(16).substring(2),r.down.outgoing(o)}var n=V.getTracer("easyXDM.stack.VerifyBehavior");n("constructor");var r,o,i;return r={incoming:function(a,s){var u=a.indexOf("_");-1===u?a===o?(n("verified, calling callback"),r.up.callback(!0)):i||(n("returning secret"),i=a,e.initiate||t(),r.down.outgoing(a)):a.substring(0,u)===i&&r.up.incoming(a.substring(u+1),s)},outgoing:function(e,t,n){r.down.outgoing(o+"_"+e,t,n)},callback:function(){e.initiate&&t()}}},q.stack.RpcBehavior=function(e,t){function n(e){e.jsonrpc="2.0",a.down.outgoing(u.stringify(e))}function r(e,t){var r=Array.prototype.slice;return s("creating method "+t),function(){s("executing method "+t);var o,i=arguments.length,a={method:t};return i>0&&"function"==typeof arguments[i-1]?(i>1&&"function"==typeof arguments[i-2]?(o={success:arguments[i-2],error:arguments[i-1]},a.params=r.call(arguments,0,i-2)):(o={success:arguments[i-1]},a.params=r.call(arguments,0,i-1)),c[""+ ++l]=o,a.id=l):a.params=r.call(arguments,0),e.namedParams&&1===a.params.length&&(a.params=a.params[0]),n(a),a.id}}function i(e,t,r,i){if(!r)return s("requested to execute non-existent procedure "+e),void(t&&n({id:t,error:{code:-32601,message:"Procedure not found."}}));s("requested to execute procedure "+e);var a,u;t?(a=function(e){a=O,n({id:t,result:e})},u=function(e,r){u=O;var o={id:t,error:{code:-32099,message:e}};r&&(o.error.data=r),n(o)}):a=u=O,o(i)||(i=[i]);try{var l=r.method.apply(r.scope,i.concat([a,u]));g(l)||a(l)}catch(c){u(c.message)}}var a,s=V.getTracer("easyXDM.stack.RpcBehavior"),u=t.serializer||J(),l=0,c={};return a={incoming:function(e){var r=u.parse(e);if(r.method)s("received request to execute method "+r.method+(r.id?" using callback id "+r.id:"")),r&&r.params&&r.params[0]&&"object"==typeof r.params[0]&&(r.params[0].id=r.id),t.handle?t.handle(r,n):i(r.method,r.id,t.local[r.method],r.params);else{s("received return value destined to callback with id "+r.id);var o=c[r.id];r.error?o.error?o.error(r.error):s("unhandled error returned."):o.success&&o.success(r.result),delete c[r.id]}},init:function(){if(s("init"),t.remote){s("creating stubs");for(var n in t.remote)t.remote.hasOwnProperty(n)&&(e[n]=r(t.remote[n],n))}a.down.init()},destroy:function(){s("destroy");for(var n in t.remote)t.remote.hasOwnProperty(n)&&e.hasOwnProperty(n)&&delete e[n];a.down.destroy()}}},q}),define("common/transport/fakeXhrFactory",["require","underscore"],function(e){var t=e("underscore");return function(e){var n={},r={};return t.extend(e,{getResponseHeader:function(t){var n,r=/^(.*?):[ \t]*([^\r\n]*)$/gm;if(4===e.readyState){if(!this.responseHeaders)for(this.responseHeaders={};n=r.exec(this.responseHeadersString);)this.responseHeaders[n[1].toLowerCase()]=n[2];n=this.responseHeaders[t.toLowerCase()]}return null==n?null:n},getAllResponseHeaders:function(){return 2===e.readyState?this.responseHeadersString:null},setRequestHeader:function(t,o){var i=t.toLowerCase();return e.readyState||(t=n[i]=n[i]||t,r[t]=o),this}})}}),define("common/loader/core/transport/request",["require","jquery","xdm","common/transport/fakeXhrFactory","logger"],function(e){"use strict";function t(){var e=[];return e.push({name:"logEnabled",value:s.get("enabled")}),s.get("level")&&e.push({name:"logLevel",value:s.get("level")}),r.param(e)}var n,r=e("jquery"),o=e("xdm"),i=r.Deferred,a=e("common/transport/fakeXhrFactory"),s=e("logger"),u=function(e){n=new o.Rpc({remote:e+"?"+t(),container:document.body,props:{style:{display:"none"}}},{remote:{request:{},abort:{}}})},l=function(e,t){var r=new i,o=e.error;return t=t||e.success,n?(n.request(e,function(e){r.resolve(e.data,e.status,a(e.xhr))},function(e){r.reject(e.message,e.data)}),t&&r.done(t),o&&r.error(o)):r.rejectWith(new Error("RPC object is not initialized")),r.promise()};return l.rpc=u,l}),define("common/loader/core/util/helper",[],function(){"use strict";return{serverSettings:function(e){var t=e.match(/<script[^>]*>([^<]*)<\/script>/)[1],n=new Function(t+"return __jrsConfigs__;");return n()},loaderConfig:function(e){return new Function("requirejs","return "+e)({config:function(e){return e}})}}}),define("common/loader/core/Root",["require","./enum/links","./enum/relations","underscore","jquery","./transport/request","./util/helper","logger"],function(e){"use strict";function t(e,t){return c.debug(e,t),t}function n(e){return a.find(o,function(t){return t.rel===e.rel&&t.name===e.name})}function r(e,t,n,r){this.isLoggerEnabled=t,this.logLevel=n,this.scripts=r,this.baseUrl=e?e:"",this.xdm().then(function(e){u.rpc(e)})}var o=e("./enum/links").links,i=e("./enum/relations"),a=e("underscore"),s=e("jquery"),u=e("./transport/request"),l=e("./util/helper"),c=e("logger").register("Root");return a.extend(r.prototype,{xdm:function(){var e,t=new s.Deferred,r=n({rel:i.XDM});return r&&(e=this.baseUrl+r.href),t.resolve(e),t.promise()},settings:function(){var e=new s.Deferred,t=n({rel:i.SETTINGS}),r=this;return t?u({url:this.baseUrl+t.href},function(t){var n=l.serverSettings(t);n.contextPath=r.baseUrl,n.isXdm=!0,"undefined"!=typeof __jrsConfigs__?a.extend(__jrsConfigs__,n):window.__jrsConfigs__=n,e.resolve(n)}):e.resolve(new Error("Can't get server settings")),e.promise()},requirejs:function(){var e=new s.Deferred,r=n({rel:i.REQUIREJS,name:"jrs"});if(o){var c,f=this.scripts?this.scripts:"scripts",p=r.href.replace("{scripts}",this.scripts),d=this.baseUrl+"/",h=a.partial(t,"Script loader configs for JRS: "),g=this;c=u({url:this.baseUrl+p,dataType:"text"}).then(l.loaderConfig).then(function(e){var t=e.baseUrl?e.baseUrl:f;return e.baseUrl=d+t,e.config&&e.config.logger&&(e.config.logger.enabled=g.isLoggerEnabled,e.config.logger.level=g.logLevel),e}).then(h),s.when(c).then(function(e){return e},e.reject).then(e.resolve)}else e.reject(new Error("Can't get RequireJS configs"));return e.promise()}}),r}),function(e){"use strict";function t(e,t){var a,s,u,l,c,f,p,d;if(e&&("object"==typeof e?(s=e.url,l=e.logEnabled,c=e.logLevel,f=e.scripts,require.config({config:{logger:{enabled:e.logEnabled,level:e.logLevel,appenders:["console"]}}})):"string"==typeof e&&(s=e)),s||(a=new Error(n.BASE_URL_NOT_FOUND)),t&&"function"!=typeof t&&(a=new Error(n.ILLEGAL_CALLBACK),t=null),t||(t=r),a)t.apply(this,[a]),d=function(e){throw new Error("Can't load "+e)};else{var h;try{h=o(window.location.href)===o(s)}catch(g){h=!1}p=new i(t,a),require(["common/loader/core/Root","logger","xdm"],function(e,t,n){h&&(window.easyXDM=n),u=new e(s,l,c,f);var r=t.register("Jasper");u.settings().done(function(e){r.debug("Server settings: ",e),p.resolveSettings(e)}),u.requirejs().done(function(e){var t=requirejs.config(e);p.resolveConfig(t)})},t),d=function(e,n){p.then(function(r){r.apply(this,[e,n,t])})}}return d}var n={BASE_URL_NOT_FOUND:"Illegal 'conf' argument, it should be url to server or contains 'url' property",ILLEGAL_CALLBACK:"Illegal 'callback' argument, it should be function or undefined"},r=function(e){if(e)throw e},o=function(e){var t=document.createElement("a");return t.href=e,t.origin||t.protocol+"//"+t.host},i=function(e,t){function n(){e&&e.call(this,t,this.require);for(var n=0;n<this._callbacks.length;n++)this._callbacks[n](this.require)}this._callbacks=[],this.then=function(e){this.require?e(this.require):this._callbacks.push(e)},this.resolveSettings=function(e){this.settings=e,this.require&&n.call(this)},this.resolveConfig=function(e){this.require=e,this.settings&&n.call(this)}},a=e.jasper,s=e.jasperjs;t.noConflict=function(n){return e.jasper===t&&(e.jasper=a),n&&e.jasperjs===jasperjs&&(e.jasperjs=s),t},e.jasper=t,e.jasperjs=t,t._errors_=n,t.version="0.0.1a"}(this),define("jasper",function(e){return function(){var t;return t||e.jasper}}(this)),function(e,t){"object"==typeof module&&"object"==typeof module.exports?module.exports=e.document?t(e,!0):function(e){if(!e.document)throw new Error("jQuery requires a window with a document");return t(e)}:t(e)}("undefined"!=typeof window?window:this,function(e,t){function n(e){var t=e.length,n=it.type(e);return"function"===n||it.isWindow(e)?!1:1===e.nodeType&&t?!0:"array"===n||0===t||"number"==typeof t&&t>0&&t-1 in e}function r(e,t,n){if(it.isFunction(t))return it.grep(e,function(e,r){return!!t.call(e,r,e)!==n});if(t.nodeType)return it.grep(e,function(e){return e===t!==n});if("string"==typeof t){if(dt.test(t))return it.filter(t,e,n);t=it.filter(t,e)}return it.grep(e,function(e){return it.inArray(e,t)>=0!==n})}function o(e,t){do e=e[t];while(e&&1!==e.nodeType);return e}function i(e){var t=wt[e]={};return it.each(e.match(xt)||[],function(e,n){t[n]=!0}),t}function a(){gt.addEventListener?(gt.removeEventListener("DOMContentLoaded",s,!1),e.removeEventListener("load",s,!1)):(gt.detachEvent("onreadystatechange",s),e.detachEvent("onload",s))}function s(){(gt.addEventListener||"load"===event.type||"complete"===gt.readyState)&&(a(),it.ready())}function u(e,t,n){if(void 0===n&&1===e.nodeType){var r="data-"+t.replace(_t,"-$1").toLowerCase();if(n=e.getAttribute(r),"string"==typeof n){try{n="true"===n?!0:"false"===n?!1:"null"===n?null:+n+""===n?+n:kt.test(n)?it.parseJSON(n):n}catch(o){}it.data(e,t,n)}else n=void 0}return n}function l(e){var t;for(t in e)if(("data"!==t||!it.isEmptyObject(e[t]))&&"toJSON"!==t)return!1;return!0}function c(e,t,n,r){if(it.acceptData(e)){var o,i,a=it.expando,s=e.nodeType,u=s?it.cache:e,l=s?e[a]:e[a]&&a;if(l&&u[l]&&(r||u[l].data)||void 0!==n||"string"!=typeof t)return l||(l=s?e[a]=V.pop()||it.guid++:a),u[l]||(u[l]=s?{}:{toJSON:it.noop}),("object"==typeof t||"function"==typeof t)&&(r?u[l]=it.extend(u[l],t):u[l].data=it.extend(u[l].data,t)),i=u[l],r||(i.data||(i.data={}),i=i.data),void 0!==n&&(i[it.camelCase(t)]=n),"string"==typeof t?(o=i[t],null==o&&(o=i[it.camelCase(t)])):o=i,o}}function f(e,t,n){if(it.acceptData(e)){var r,o,i=e.nodeType,a=i?it.cache:e,s=i?e[it.expando]:it.expando;if(a[s]){if(t&&(r=n?a[s]:a[s].data)){it.isArray(t)?t=t.concat(it.map(t,it.camelCase)):t in r?t=[t]:(t=it.camelCase(t),t=t in r?[t]:t.split(" ")),o=t.length;for(;o--;)delete r[t[o]];if(n?!l(r):!it.isEmptyObject(r))return}(n||(delete a[s].data,l(a[s])))&&(i?it.cleanData([e],!0):rt.deleteExpando||a!=a.window?delete a[s]:a[s]=null)}}}function p(){return!0}function d(){return!1}function h(){try{return gt.activeElement}catch(e){}}function g(e){var t=qt.split("|"),n=e.createDocumentFragment();if(n.createElement)for(;t.length;)n.createElement(t.pop());return n}function m(e,t){var n,r,o=0,i=typeof e.getElementsByTagName!==Et?e.getElementsByTagName(t||"*"):typeof e.querySelectorAll!==Et?e.querySelectorAll(t||"*"):void 0;if(!i)for(i=[],n=e.childNodes||e;null!=(r=n[o]);o++)!t||it.nodeName(r,t)?i.push(r):it.merge(i,m(r,t));return void 0===t||t&&it.nodeName(e,t)?it.merge([e],i):i}function v(e){Lt.test(e.type)&&(e.defaultChecked=e.checked)}function y(e,t){return it.nodeName(e,"table")&&it.nodeName(11!==t.nodeType?t:t.firstChild,"tr")?e.getElementsByTagName("tbody")[0]||e.appendChild(e.ownerDocument.createElement("tbody")):e}function b(e){return e.type=(null!==it.find.attr(e,"type"))+"/"+e.type,e}function x(e){var t=Vt.exec(e.type);return t?e.type=t[1]:e.removeAttribute("type"),e}function w(e,t){for(var n,r=0;null!=(n=e[r]);r++)it._data(n,"globalEval",!t||it._data(t[r],"globalEval"))}function T(e,t){if(1===t.nodeType&&it.hasData(e)){var n,r,o,i=it._data(e),a=it._data(t,i),s=i.events;if(s){delete a.handle,a.events={};for(n in s)for(r=0,o=s[n].length;o>r;r++)it.event.add(t,n,s[n][r])}a.data&&(a.data=it.extend({},a.data))}}function C(e,t){var n,r,o;if(1===t.nodeType){if(n=t.nodeName.toLowerCase(),!rt.noCloneEvent&&t[it.expando]){o=it._data(t);for(r in o.events)it.removeEvent(t,r,o.handle);t.removeAttribute(it.expando)}"script"===n&&t.text!==e.text?(b(t).text=e.text,x(t)):"object"===n?(t.parentNode&&(t.outerHTML=e.outerHTML),rt.html5Clone&&e.innerHTML&&!it.trim(t.innerHTML)&&(t.innerHTML=e.innerHTML)):"input"===n&&Lt.test(e.type)?(t.defaultChecked=t.checked=e.checked,t.value!==e.value&&(t.value=e.value)):"option"===n?t.defaultSelected=t.selected=e.defaultSelected:("input"===n||"textarea"===n)&&(t.defaultValue=e.defaultValue)}}function E(t,n){var r=it(n.createElement(t)).appendTo(n.body),o=e.getDefaultComputedStyle?e.getDefaultComputedStyle(r[0]).display:it.css(r[0],"display");return r.detach(),o}function k(e){var t=gt,n=en[e];return n||(n=E(e,t),"none"!==n&&n||(Zt=(Zt||it("<iframe frameborder='0' width='0' height='0'/>")).appendTo(t.documentElement),t=(Zt[0].contentWindow||Zt[0].contentDocument).document,t.write(),t.close(),n=E(e,t),Zt.detach()),en[e]=n),n}function _(e,t){return{get:function(){var n=e();if(null!=n)return n?void delete this.get:(this.get=t).apply(this,arguments)}}}function S(e,t){if(t in e)return t;for(var n=t.charAt(0).toUpperCase()+t.slice(1),r=t,o=hn.length;o--;)if(t=hn[o]+n,t in e)return t;return r}function N(e,t){for(var n,r,o,i=[],a=0,s=e.length;s>a;a++)r=e[a],r.style&&(i[a]=it._data(r,"olddisplay"),n=r.style.display,t?(i[a]||"none"!==n||(r.style.display=""),""===r.style.display&&jt(r)&&(i[a]=it._data(r,"olddisplay",k(r.nodeName)))):i[a]||(o=jt(r),(n&&"none"!==n||!o)&&it._data(r,"olddisplay",o?n:it.css(r,"display"))));for(a=0;s>a;a++)r=e[a],r.style&&(t&&"none"!==r.style.display&&""!==r.style.display||(r.style.display=t?i[a]||"":"none"));return e}function j(e,t,n){var r=cn.exec(t);return r?Math.max(0,r[1]-(n||0))+(r[2]||"px"):t}function A(e,t,n,r,o){for(var i=n===(r?"border":"content")?4:"width"===t?1:0,a=0;4>i;i+=2)"margin"===n&&(a+=it.css(e,n+Nt[i],!0,o)),r?("content"===n&&(a-=it.css(e,"padding"+Nt[i],!0,o)),"margin"!==n&&(a-=it.css(e,"border"+Nt[i]+"Width",!0,o))):(a+=it.css(e,"padding"+Nt[i],!0,o),"padding"!==n&&(a+=it.css(e,"border"+Nt[i]+"Width",!0,o)));return a}function L(e,t,n){var r=!0,o="width"===t?e.offsetWidth:e.offsetHeight,i=tn(e),a=rt.boxSizing()&&"border-box"===it.css(e,"boxSizing",!1,i);if(0>=o||null==o){if(o=nn(e,t,i),(0>o||null==o)&&(o=e.style[t]),on.test(o))return o;r=a&&(rt.boxSizingReliable()||o===e.style[t]),o=parseFloat(o)||0}return o+A(e,t,n||(a?"border":"content"),r,i)+"px"}function O(e,t,n,r,o){return new O.prototype.init(e,t,n,r,o)}function D(){return setTimeout(function(){gn=void 0}),gn=it.now()}function R(e,t){var n,r={height:e},o=0;for(t=t?1:0;4>o;o+=2-t)n=Nt[o],r["margin"+n]=r["padding"+n]=e;return t&&(r.opacity=r.width=e),r}function M(e,t,n){for(var r,o=(wn[t]||[]).concat(wn["*"]),i=0,a=o.length;a>i;i++)if(r=o[i].call(n,t,e))return r}function H(e,t,n){var r,o,i,a,s,u,l,c,f=this,p={},d=e.style,h=e.nodeType&&jt(e),g=it._data(e,"fxshow");n.queue||(s=it._queueHooks(e,"fx"),null==s.unqueued&&(s.unqueued=0,u=s.empty.fire,s.empty.fire=function(){s.unqueued||u()}),s.unqueued++,f.always(function(){f.always(function(){s.unqueued--,it.queue(e,"fx").length||s.empty.fire()})})),1===e.nodeType&&("height"in t||"width"in t)&&(n.overflow=[d.overflow,d.overflowX,d.overflowY],l=it.css(e,"display"),c=k(e.nodeName),"none"===l&&(l=c),"inline"===l&&"none"===it.css(e,"float")&&(rt.inlineBlockNeedsLayout&&"inline"!==c?d.zoom=1:d.display="inline-block")),n.overflow&&(d.overflow="hidden",rt.shrinkWrapBlocks()||f.always(function(){d.overflow=n.overflow[0],d.overflowX=n.overflow[1],d.overflowY=n.overflow[2]}));for(r in t)if(o=t[r],vn.exec(o)){if(delete t[r],i=i||"toggle"===o,o===(h?"hide":"show")){if("show"!==o||!g||void 0===g[r])continue;h=!0}p[r]=g&&g[r]||it.style(e,r)}if(!it.isEmptyObject(p)){g?"hidden"in g&&(h=g.hidden):g=it._data(e,"fxshow",{}),i&&(g.hidden=!h),h?it(e).show():f.done(function(){it(e).hide()}),f.done(function(){var t;it._removeData(e,"fxshow");for(t in p)it.style(e,t,p[t])});for(r in p)a=M(h?g[r]:0,r,f),r in g||(g[r]=a.start,h&&(a.end=a.start,a.start="width"===r||"height"===r?1:0))}}function q(e,t){var n,r,o,i,a;for(n in e)if(r=it.camelCase(n),o=t[r],i=e[n],it.isArray(i)&&(o=i[1],i=e[n]=i[0]),n!==r&&(e[r]=i,delete e[n]),a=it.cssHooks[r],a&&"expand"in a){i=a.expand(i),delete e[r];for(n in i)n in e||(e[n]=i[n],t[n]=o)}else t[r]=o}function F(e,t,n){var r,o,i=0,a=xn.length,s=it.Deferred().always(function(){delete u.elem}),u=function(){if(o)return!1;for(var t=gn||D(),n=Math.max(0,l.startTime+l.duration-t),r=n/l.duration||0,i=1-r,a=0,u=l.tweens.length;u>a;a++)l.tweens[a].run(i);return s.notifyWith(e,[l,i,n]),1>i&&u?n:(s.resolveWith(e,[l]),!1)},l=s.promise({elem:e,props:it.extend({},t),opts:it.extend(!0,{specialEasing:{}},n),originalProperties:t,originalOptions:n,startTime:gn||D(),duration:n.duration,tweens:[],createTween:function(t,n){var r=it.Tween(e,l.opts,t,n,l.opts.specialEasing[t]||l.opts.easing);return l.tweens.push(r),r},stop:function(t){var n=0,r=t?l.tweens.length:0;if(o)return this;for(o=!0;r>n;n++)l.tweens[n].run(1);return t?s.resolveWith(e,[l,t]):s.rejectWith(e,[l,t]),this}}),c=l.props;for(q(c,l.opts.specialEasing);a>i;i++)if(r=xn[i].call(l,e,c,l.opts))return r;return it.map(c,M,l),it.isFunction(l.opts.start)&&l.opts.start.call(e,l),it.fx.timer(it.extend(u,{elem:e,anim:l,queue:l.opts.queue})),l.progress(l.opts.progress).done(l.opts.done,l.opts.complete).fail(l.opts.fail).always(l.opts.always)}function P(e){return function(t,n){"string"!=typeof t&&(n=t,t="*");var r,o=0,i=t.toLowerCase().match(xt)||[];if(it.isFunction(n))for(;r=i[o++];)"+"===r.charAt(0)?(r=r.slice(1)||"*",(e[r]=e[r]||[]).unshift(n)):(e[r]=e[r]||[]).push(n)}}function B(e,t,n,r){function o(s){var u;return i[s]=!0,it.each(e[s]||[],function(e,s){var l=s(t,n,r);return"string"!=typeof l||a||i[l]?a?!(u=l):void 0:(t.dataTypes.unshift(l),o(l),!1)}),u}var i={},a=e===Un;return o(t.dataTypes[0])||!i["*"]&&o("*")}function I(e,t){var n,r,o=it.ajaxSettings.flatOptions||{};for(r in t)void 0!==t[r]&&((o[r]?e:n||(n={}))[r]=t[r]);return n&&it.extend(!0,e,n),e}function $(e,t,n){for(var r,o,i,a,s=e.contents,u=e.dataTypes;"*"===u[0];)u.shift(),void 0===o&&(o=e.mimeType||t.getResponseHeader("Content-Type"));if(o)for(a in s)if(s[a]&&s[a].test(o)){u.unshift(a);break}if(u[0]in n)i=u[0];else{for(a in n){if(!u[0]||e.converters[a+" "+u[0]]){i=a;break}r||(r=a)}i=i||r}return i?(i!==u[0]&&u.unshift(i),n[i]):void 0}function W(e,t,n,r){var o,i,a,s,u,l={},c=e.dataTypes.slice();if(c[1])for(a in e.converters)l[a.toLowerCase()]=e.converters[a];for(i=c.shift();i;)if(e.responseFields[i]&&(n[e.responseFields[i]]=t),!u&&r&&e.dataFilter&&(t=e.dataFilter(t,e.dataType)),u=i,i=c.shift())if("*"===i)i=u;else if("*"!==u&&u!==i){if(a=l[u+" "+i]||l["* "+i],!a)for(o in l)if(s=o.split(" "),s[1]===i&&(a=l[u+" "+s[0]]||l["* "+s[0]])){a===!0?a=l[o]:l[o]!==!0&&(i=s[0],c.unshift(s[1]));break}if(a!==!0)if(a&&e["throws"])t=a(t);else try{t=a(t)}catch(f){return{state:"parsererror",error:a?f:"No conversion from "+u+" to "+i}}}return{state:"success",data:t}}function z(e,t,n,r){var o;if(it.isArray(t))it.each(t,function(t,o){n||Gn.test(e)?r(e,o):z(e+"["+("object"==typeof o?t:"")+"]",o,n,r)});else if(n||"object"!==it.type(t))r(e,t);else for(o in t)z(e+"["+o+"]",t[o],n,r)}function U(){try{return new e.XMLHttpRequest}catch(t){}}function X(){try{return new e.ActiveXObject("Microsoft.XMLHTTP")}catch(t){}}function J(e){return it.isWindow(e)?e:9===e.nodeType?e.defaultView||e.parentWindow:!1}var V=[],G=V.slice,Q=V.concat,Y=V.push,K=V.indexOf,Z={},et=Z.toString,tt=Z.hasOwnProperty,nt="".trim,rt={},ot="1.11.0",it=function(e,t){return new it.fn.init(e,t)},at=/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g,st=/^-ms-/,ut=/-([\da-z])/gi,lt=function(e,t){return t.toUpperCase()};it.fn=it.prototype={jquery:ot,constructor:it,selector:"",length:0,toArray:function(){return G.call(this)},get:function(e){return null!=e?0>e?this[e+this.length]:this[e]:G.call(this)},pushStack:function(e){var t=it.merge(this.constructor(),e);return t.prevObject=this,t.context=this.context,t},each:function(e,t){return it.each(this,e,t)},map:function(e){return this.pushStack(it.map(this,function(t,n){return e.call(t,n,t)}))},slice:function(){return this.pushStack(G.apply(this,arguments))},first:function(){return this.eq(0)},last:function(){return this.eq(-1)},eq:function(e){var t=this.length,n=+e+(0>e?t:0);return this.pushStack(n>=0&&t>n?[this[n]]:[])},end:function(){return this.prevObject||this.constructor(null)},push:Y,sort:V.sort,splice:V.splice},it.extend=it.fn.extend=function(){var e,t,n,r,o,i,a=arguments[0]||{},s=1,u=arguments.length,l=!1;for("boolean"==typeof a&&(l=a,a=arguments[s]||{},s++),"object"==typeof a||it.isFunction(a)||(a={}),s===u&&(a=this,s--);u>s;s++)if(null!=(o=arguments[s]))for(r in o)e=a[r],n=o[r],a!==n&&(l&&n&&(it.isPlainObject(n)||(t=it.isArray(n)))?(t?(t=!1,i=e&&it.isArray(e)?e:[]):i=e&&it.isPlainObject(e)?e:{},a[r]=it.extend(l,i,n)):void 0!==n&&(a[r]=n));return a},it.extend({expando:"jQuery"+(ot+Math.random()).replace(/\D/g,""),isReady:!0,error:function(e){throw new Error(e)},noop:function(){},isFunction:function(e){return"function"===it.type(e)},isArray:Array.isArray||function(e){return"array"===it.type(e)},isWindow:function(e){return null!=e&&e==e.window},isNumeric:function(e){return e-parseFloat(e)>=0},isEmptyObject:function(e){var t;for(t in e)return!1;return!0},isPlainObject:function(e){var t;if(!e||"object"!==it.type(e)||e.nodeType||it.isWindow(e))return!1;try{if(e.constructor&&!tt.call(e,"constructor")&&!tt.call(e.constructor.prototype,"isPrototypeOf"))return!1}catch(n){return!1}if(rt.ownLast)for(t in e)return tt.call(e,t);for(t in e);return void 0===t||tt.call(e,t)},type:function(e){return null==e?e+"":"object"==typeof e||"function"==typeof e?Z[et.call(e)]||"object":typeof e},globalEval:function(t){t&&it.trim(t)&&(e.execScript||function(t){e.eval.call(e,t)})(t)},camelCase:function(e){return e.replace(st,"ms-").replace(ut,lt)},nodeName:function(e,t){return e.nodeName&&e.nodeName.toLowerCase()===t.toLowerCase()},each:function(e,t,r){var o,i=0,a=e.length,s=n(e);if(r){if(s)for(;a>i&&(o=t.apply(e[i],r),o!==!1);i++);else for(i in e)if(o=t.apply(e[i],r),o===!1)break}else if(s)for(;a>i&&(o=t.call(e[i],i,e[i]),o!==!1);i++);else for(i in e)if(o=t.call(e[i],i,e[i]),o===!1)break;return e},trim:nt&&!nt.call("\ufeff\xa0")?function(e){return null==e?"":nt.call(e)}:function(e){return null==e?"":(e+"").replace(at,"")},makeArray:function(e,t){var r=t||[];return null!=e&&(n(Object(e))?it.merge(r,"string"==typeof e?[e]:e):Y.call(r,e)),r},inArray:function(e,t,n){var r;if(t){if(K)return K.call(t,e,n);for(r=t.length,n=n?0>n?Math.max(0,r+n):n:0;r>n;n++)if(n in t&&t[n]===e)return n}return-1},merge:function(e,t){for(var n=+t.length,r=0,o=e.length;n>r;)e[o++]=t[r++];if(n!==n)for(;void 0!==t[r];)e[o++]=t[r++];return e.length=o,e},grep:function(e,t,n){for(var r,o=[],i=0,a=e.length,s=!n;a>i;i++)r=!t(e[i],i),r!==s&&o.push(e[i]);return o},map:function(e,t,r){var o,i=0,a=e.length,s=n(e),u=[];if(s)for(;a>i;i++)o=t(e[i],i,r),null!=o&&u.push(o);else for(i in e)o=t(e[i],i,r),null!=o&&u.push(o);return Q.apply([],u)},guid:1,proxy:function(e,t){var n,r,o;return"string"==typeof t&&(o=e[t],t=e,e=o),it.isFunction(e)?(n=G.call(arguments,2),r=function(){return e.apply(t||this,n.concat(G.call(arguments)))},r.guid=e.guid=e.guid||it.guid++,r):void 0},now:function(){return+new Date},support:rt}),it.each("Boolean Number String Function Array Date RegExp Object Error".split(" "),function(e,t){Z["[object "+t+"]"]=t.toLowerCase()});var ct=function(e){function t(e,t,n,r){var o,i,a,s,u,l,f,h,g,m;if((t?t.ownerDocument||t:B)!==O&&L(t),t=t||O,n=n||[],!e||"string"!=typeof e)return n;if(1!==(s=t.nodeType)&&9!==s)return[];if(R&&!r){if(o=yt.exec(e))if(a=o[1]){if(9===s){if(i=t.getElementById(a),!i||!i.parentNode)return n;if(i.id===a)return n.push(i),n}else if(t.ownerDocument&&(i=t.ownerDocument.getElementById(a))&&F(t,i)&&i.id===a)return n.push(i),n}else{if(o[2])return Z.apply(n,t.getElementsByTagName(e)),n;if((a=o[3])&&C.getElementsByClassName&&t.getElementsByClassName)return Z.apply(n,t.getElementsByClassName(a)),n}if(C.qsa&&(!M||!M.test(e))){if(h=f=P,g=t,m=9===s&&e,1===s&&"object"!==t.nodeName.toLowerCase()){for(l=p(e),(f=t.getAttribute("id"))?h=f.replace(xt,"\\$&"):t.setAttribute("id",h),h="[id='"+h+"'] ",u=l.length;u--;)l[u]=h+d(l[u]);g=bt.test(e)&&c(t.parentNode)||t,m=l.join(",")}if(m)try{return Z.apply(n,g.querySelectorAll(m)),n}catch(v){}finally{f||t.removeAttribute("id")}}}return w(e.replace(ut,"$1"),t,n,r)}function n(){function e(n,r){return t.push(n+" ")>E.cacheLength&&delete e[t.shift()],e[n+" "]=r}var t=[];return e}function r(e){return e[P]=!0,e}function o(e){var t=O.createElement("div");try{return!!e(t)}catch(n){return!1}finally{t.parentNode&&t.parentNode.removeChild(t),t=null}}function i(e,t){for(var n=e.split("|"),r=e.length;r--;)E.attrHandle[n[r]]=t}function a(e,t){var n=t&&e,r=n&&1===e.nodeType&&1===t.nodeType&&(~t.sourceIndex||V)-(~e.sourceIndex||V);if(r)return r;if(n)for(;n=n.nextSibling;)if(n===t)return-1;return e?1:-1}function s(e){return function(t){var n=t.nodeName.toLowerCase();return"input"===n&&t.type===e}}function u(e){return function(t){var n=t.nodeName.toLowerCase();return("input"===n||"button"===n)&&t.type===e}}function l(e){return r(function(t){return t=+t,r(function(n,r){for(var o,i=e([],n.length,t),a=i.length;a--;)n[o=i[a]]&&(n[o]=!(r[o]=n[o]))})})}function c(e){return e&&typeof e.getElementsByTagName!==J&&e}function f(){}function p(e,n){var r,o,i,a,s,u,l,c=z[e+" "];if(c)return n?0:c.slice(0);for(s=e,u=[],l=E.preFilter;s;){(!r||(o=lt.exec(s)))&&(o&&(s=s.slice(o[0].length)||s),u.push(i=[])),r=!1,(o=ct.exec(s))&&(r=o.shift(),i.push({value:r,type:o[0].replace(ut," ")}),s=s.slice(r.length));for(a in E.filter)!(o=ht[a].exec(s))||l[a]&&!(o=l[a](o))||(r=o.shift(),i.push({value:r,type:a,matches:o}),s=s.slice(r.length));if(!r)break}return n?s.length:s?t.error(e):z(e,u).slice(0)}function d(e){for(var t=0,n=e.length,r="";n>t;t++)r+=e[t].value;return r}function h(e,t,n){var r=t.dir,o=n&&"parentNode"===r,i=$++;return t.first?function(t,n,i){for(;t=t[r];)if(1===t.nodeType||o)return e(t,n,i)}:function(t,n,a){var s,u,l=[I,i];if(a){for(;t=t[r];)if((1===t.nodeType||o)&&e(t,n,a))return!0}else for(;t=t[r];)if(1===t.nodeType||o){if(u=t[P]||(t[P]={}),(s=u[r])&&s[0]===I&&s[1]===i)return l[2]=s[2];if(u[r]=l,l[2]=e(t,n,a))return!0}}}function g(e){return e.length>1?function(t,n,r){for(var o=e.length;o--;)if(!e[o](t,n,r))return!1;return!0}:e[0]}function m(e,t,n,r,o){for(var i,a=[],s=0,u=e.length,l=null!=t;u>s;s++)(i=e[s])&&(!n||n(i,r,o))&&(a.push(i),l&&t.push(s));return a}function v(e,t,n,o,i,a){return o&&!o[P]&&(o=v(o)),i&&!i[P]&&(i=v(i,a)),r(function(r,a,s,u){var l,c,f,p=[],d=[],h=a.length,g=r||x(t||"*",s.nodeType?[s]:s,[]),v=!e||!r&&t?g:m(g,p,e,s,u),y=n?i||(r?e:h||o)?[]:a:v;if(n&&n(v,y,s,u),o)for(l=m(y,d),o(l,[],s,u),c=l.length;c--;)(f=l[c])&&(y[d[c]]=!(v[d[c]]=f));if(r){if(i||e){if(i){for(l=[],c=y.length;c--;)(f=y[c])&&l.push(v[c]=f);i(null,y=[],l,u)}for(c=y.length;c--;)(f=y[c])&&(l=i?tt.call(r,f):p[c])>-1&&(r[l]=!(a[l]=f))}}else y=m(y===a?y.splice(h,y.length):y),i?i(null,a,y,u):Z.apply(a,y)})}function y(e){for(var t,n,r,o=e.length,i=E.relative[e[0].type],a=i||E.relative[" "],s=i?1:0,u=h(function(e){return e===t},a,!0),l=h(function(e){return tt.call(t,e)>-1},a,!0),c=[function(e,n,r){return!i&&(r||n!==N)||((t=n).nodeType?u(e,n,r):l(e,n,r))}];o>s;s++)if(n=E.relative[e[s].type])c=[h(g(c),n)];else{if(n=E.filter[e[s].type].apply(null,e[s].matches),n[P]){for(r=++s;o>r&&!E.relative[e[r].type];r++);return v(s>1&&g(c),s>1&&d(e.slice(0,s-1).concat({value:" "===e[s-2].type?"*":""})).replace(ut,"$1"),n,r>s&&y(e.slice(s,r)),o>r&&y(e=e.slice(r)),o>r&&d(e))}c.push(n)}return g(c)}function b(e,n){var o=n.length>0,i=e.length>0,a=function(r,a,s,u,l){var c,f,p,d=0,h="0",g=r&&[],v=[],y=N,b=r||i&&E.find.TAG("*",l),x=I+=null==y?1:Math.random()||.1,w=b.length;for(l&&(N=a!==O&&a);h!==w&&null!=(c=b[h]);h++){if(i&&c){for(f=0;p=e[f++];)if(p(c,a,s)){u.push(c);break}l&&(I=x)}o&&((c=!p&&c)&&d--,r&&g.push(c))}if(d+=h,o&&h!==d){for(f=0;p=n[f++];)p(g,v,a,s);if(r){if(d>0)for(;h--;)g[h]||v[h]||(v[h]=Y.call(u));v=m(v)}Z.apply(u,v),l&&!r&&v.length>0&&d+n.length>1&&t.uniqueSort(u)}return l&&(I=x,N=y),g};return o?r(a):a}function x(e,n,r){for(var o=0,i=n.length;i>o;o++)t(e,n[o],r);return r}function w(e,t,n,r){var o,i,a,s,u,l=p(e);if(!r&&1===l.length){if(i=l[0]=l[0].slice(0),i.length>2&&"ID"===(a=i[0]).type&&C.getById&&9===t.nodeType&&R&&E.relative[i[1].type]){if(t=(E.find.ID(a.matches[0].replace(wt,Tt),t)||[])[0],!t)return n;e=e.slice(i.shift().value.length)}for(o=ht.needsContext.test(e)?0:i.length;o--&&(a=i[o],!E.relative[s=a.type]);)if((u=E.find[s])&&(r=u(a.matches[0].replace(wt,Tt),bt.test(i[0].type)&&c(t.parentNode)||t))){if(i.splice(o,1),e=r.length&&d(i),!e)return Z.apply(n,r),n;break}}return S(e,l)(r,t,!R,n,bt.test(e)&&c(t.parentNode)||t),n}var T,C,E,k,_,S,N,j,A,L,O,D,R,M,H,q,F,P="sizzle"+-new Date,B=e.document,I=0,$=0,W=n(),z=n(),U=n(),X=function(e,t){return e===t&&(A=!0),0},J="undefined",V=1<<31,G={}.hasOwnProperty,Q=[],Y=Q.pop,K=Q.push,Z=Q.push,et=Q.slice,tt=Q.indexOf||function(e){for(var t=0,n=this.length;n>t;t++)if(this[t]===e)return t;return-1},nt="checked|selected|async|autofocus|autoplay|controls|defer|disabled|hidden|ismap|loop|multiple|open|readonly|required|scoped",rt="[\\x20\\t\\r\\n\\f]",ot="(?:\\\\.|[\\w-]|[^\\x00-\\xa0])+",it=ot.replace("w","w#"),at="\\["+rt+"*("+ot+")"+rt+"*(?:([*^$|!~]?=)"+rt+"*(?:(['\"])((?:\\\\.|[^\\\\])*?)\\3|("+it+")|)|)"+rt+"*\\]",st=":("+ot+")(?:\\(((['\"])((?:\\\\.|[^\\\\])*?)\\3|((?:\\\\.|[^\\\\()[\\]]|"+at.replace(3,8)+")*)|.*)\\)|)",ut=new RegExp("^"+rt+"+|((?:^|[^\\\\])(?:\\\\.)*)"+rt+"+$","g"),lt=new RegExp("^"+rt+"*,"+rt+"*"),ct=new RegExp("^"+rt+"*([>+~]|"+rt+")"+rt+"*"),ft=new RegExp("="+rt+"*([^\\]'\"]*?)"+rt+"*\\]","g"),pt=new RegExp(st),dt=new RegExp("^"+it+"$"),ht={ID:new RegExp("^#("+ot+")"),CLASS:new RegExp("^\\.("+ot+")"),TAG:new RegExp("^("+ot.replace("w","w*")+")"),ATTR:new RegExp("^"+at),PSEUDO:new RegExp("^"+st),CHILD:new RegExp("^:(only|first|last|nth|nth-last)-(child|of-type)(?:\\("+rt+"*(even|odd|(([+-]|)(\\d*)n|)"+rt+"*(?:([+-]|)"+rt+"*(\\d+)|))"+rt+"*\\)|)","i"),bool:new RegExp("^(?:"+nt+")$","i"),needsContext:new RegExp("^"+rt+"*[>+~]|:(even|odd|eq|gt|lt|nth|first|last)(?:\\("+rt+"*((?:-\\d)?\\d*)"+rt+"*\\)|)(?=[^-]|$)","i")},gt=/^(?:input|select|textarea|button)$/i,mt=/^h\d$/i,vt=/^[^{]+\{\s*\[native \w/,yt=/^(?:#([\w-]+)|(\w+)|\.([\w-]+))$/,bt=/[+~]/,xt=/'|\\/g,wt=new RegExp("\\\\([\\da-f]{1,6}"+rt+"?|("+rt+")|.)","ig"),Tt=function(e,t,n){var r="0x"+t-65536;return r!==r||n?t:0>r?String.fromCharCode(r+65536):String.fromCharCode(r>>10|55296,1023&r|56320)};try{Z.apply(Q=et.call(B.childNodes),B.childNodes),Q[B.childNodes.length].nodeType
}catch(Ct){Z={apply:Q.length?function(e,t){K.apply(e,et.call(t))}:function(e,t){for(var n=e.length,r=0;e[n++]=t[r++];);e.length=n-1}}}C=t.support={},_=t.isXML=function(e){var t=e&&(e.ownerDocument||e).documentElement;return t?"HTML"!==t.nodeName:!1},L=t.setDocument=function(e){var t,n=e?e.ownerDocument||e:B,r=n.defaultView;return n!==O&&9===n.nodeType&&n.documentElement?(O=n,D=n.documentElement,R=!_(n),r&&r!==r.top&&(r.addEventListener?r.addEventListener("unload",function(){L()},!1):r.attachEvent&&r.attachEvent("onunload",function(){L()})),C.attributes=o(function(e){return e.className="i",!e.getAttribute("className")}),C.getElementsByTagName=o(function(e){return e.appendChild(n.createComment("")),!e.getElementsByTagName("*").length}),C.getElementsByClassName=vt.test(n.getElementsByClassName)&&o(function(e){return e.innerHTML="<div class='a'></div><div class='a i'></div>",e.firstChild.className="i",2===e.getElementsByClassName("i").length}),C.getById=o(function(e){return D.appendChild(e).id=P,!n.getElementsByName||!n.getElementsByName(P).length}),C.getById?(E.find.ID=function(e,t){if(typeof t.getElementById!==J&&R){var n=t.getElementById(e);return n&&n.parentNode?[n]:[]}},E.filter.ID=function(e){var t=e.replace(wt,Tt);return function(e){return e.getAttribute("id")===t}}):(delete E.find.ID,E.filter.ID=function(e){var t=e.replace(wt,Tt);return function(e){var n=typeof e.getAttributeNode!==J&&e.getAttributeNode("id");return n&&n.value===t}}),E.find.TAG=C.getElementsByTagName?function(e,t){return typeof t.getElementsByTagName!==J?t.getElementsByTagName(e):void 0}:function(e,t){var n,r=[],o=0,i=t.getElementsByTagName(e);if("*"===e){for(;n=i[o++];)1===n.nodeType&&r.push(n);return r}return i},E.find.CLASS=C.getElementsByClassName&&function(e,t){return typeof t.getElementsByClassName!==J&&R?t.getElementsByClassName(e):void 0},H=[],M=[],(C.qsa=vt.test(n.querySelectorAll))&&(o(function(e){e.innerHTML="<select t=''><option selected=''></option></select>",e.querySelectorAll("[t^='']").length&&M.push("[*^$]="+rt+"*(?:''|\"\")"),e.querySelectorAll("[selected]").length||M.push("\\["+rt+"*(?:value|"+nt+")"),e.querySelectorAll(":checked").length||M.push(":checked")}),o(function(e){var t=n.createElement("input");t.setAttribute("type","hidden"),e.appendChild(t).setAttribute("name","D"),e.querySelectorAll("[name=d]").length&&M.push("name"+rt+"*[*^$|!~]?="),e.querySelectorAll(":enabled").length||M.push(":enabled",":disabled"),e.querySelectorAll("*,:x"),M.push(",.*:")})),(C.matchesSelector=vt.test(q=D.webkitMatchesSelector||D.mozMatchesSelector||D.oMatchesSelector||D.msMatchesSelector))&&o(function(e){C.disconnectedMatch=q.call(e,"div"),q.call(e,"[s!='']:x"),H.push("!=",st)}),M=M.length&&new RegExp(M.join("|")),H=H.length&&new RegExp(H.join("|")),t=vt.test(D.compareDocumentPosition),F=t||vt.test(D.contains)?function(e,t){var n=9===e.nodeType?e.documentElement:e,r=t&&t.parentNode;return e===r||!(!r||1!==r.nodeType||!(n.contains?n.contains(r):e.compareDocumentPosition&&16&e.compareDocumentPosition(r)))}:function(e,t){if(t)for(;t=t.parentNode;)if(t===e)return!0;return!1},X=t?function(e,t){if(e===t)return A=!0,0;var r=!e.compareDocumentPosition-!t.compareDocumentPosition;return r?r:(r=(e.ownerDocument||e)===(t.ownerDocument||t)?e.compareDocumentPosition(t):1,1&r||!C.sortDetached&&t.compareDocumentPosition(e)===r?e===n||e.ownerDocument===B&&F(B,e)?-1:t===n||t.ownerDocument===B&&F(B,t)?1:j?tt.call(j,e)-tt.call(j,t):0:4&r?-1:1)}:function(e,t){if(e===t)return A=!0,0;var r,o=0,i=e.parentNode,s=t.parentNode,u=[e],l=[t];if(!i||!s)return e===n?-1:t===n?1:i?-1:s?1:j?tt.call(j,e)-tt.call(j,t):0;if(i===s)return a(e,t);for(r=e;r=r.parentNode;)u.unshift(r);for(r=t;r=r.parentNode;)l.unshift(r);for(;u[o]===l[o];)o++;return o?a(u[o],l[o]):u[o]===B?-1:l[o]===B?1:0},n):O},t.matches=function(e,n){return t(e,null,null,n)},t.matchesSelector=function(e,n){if((e.ownerDocument||e)!==O&&L(e),n=n.replace(ft,"='$1']"),!(!C.matchesSelector||!R||H&&H.test(n)||M&&M.test(n)))try{var r=q.call(e,n);if(r||C.disconnectedMatch||e.document&&11!==e.document.nodeType)return r}catch(o){}return t(n,O,null,[e]).length>0},t.contains=function(e,t){return(e.ownerDocument||e)!==O&&L(e),F(e,t)},t.attr=function(e,t){(e.ownerDocument||e)!==O&&L(e);var n=E.attrHandle[t.toLowerCase()],r=n&&G.call(E.attrHandle,t.toLowerCase())?n(e,t,!R):void 0;return void 0!==r?r:C.attributes||!R?e.getAttribute(t):(r=e.getAttributeNode(t))&&r.specified?r.value:null},t.error=function(e){throw new Error("Syntax error, unrecognized expression: "+e)},t.uniqueSort=function(e){var t,n=[],r=0,o=0;if(A=!C.detectDuplicates,j=!C.sortStable&&e.slice(0),e.sort(X),A){for(;t=e[o++];)t===e[o]&&(r=n.push(o));for(;r--;)e.splice(n[r],1)}return j=null,e},k=t.getText=function(e){var t,n="",r=0,o=e.nodeType;if(o){if(1===o||9===o||11===o){if("string"==typeof e.textContent)return e.textContent;for(e=e.firstChild;e;e=e.nextSibling)n+=k(e)}else if(3===o||4===o)return e.nodeValue}else for(;t=e[r++];)n+=k(t);return n},E=t.selectors={cacheLength:50,createPseudo:r,match:ht,attrHandle:{},find:{},relative:{">":{dir:"parentNode",first:!0}," ":{dir:"parentNode"},"+":{dir:"previousSibling",first:!0},"~":{dir:"previousSibling"}},preFilter:{ATTR:function(e){return e[1]=e[1].replace(wt,Tt),e[3]=(e[4]||e[5]||"").replace(wt,Tt),"~="===e[2]&&(e[3]=" "+e[3]+" "),e.slice(0,4)},CHILD:function(e){return e[1]=e[1].toLowerCase(),"nth"===e[1].slice(0,3)?(e[3]||t.error(e[0]),e[4]=+(e[4]?e[5]+(e[6]||1):2*("even"===e[3]||"odd"===e[3])),e[5]=+(e[7]+e[8]||"odd"===e[3])):e[3]&&t.error(e[0]),e},PSEUDO:function(e){var t,n=!e[5]&&e[2];return ht.CHILD.test(e[0])?null:(e[3]&&void 0!==e[4]?e[2]=e[4]:n&&pt.test(n)&&(t=p(n,!0))&&(t=n.indexOf(")",n.length-t)-n.length)&&(e[0]=e[0].slice(0,t),e[2]=n.slice(0,t)),e.slice(0,3))}},filter:{TAG:function(e){var t=e.replace(wt,Tt).toLowerCase();return"*"===e?function(){return!0}:function(e){return e.nodeName&&e.nodeName.toLowerCase()===t}},CLASS:function(e){var t=W[e+" "];return t||(t=new RegExp("(^|"+rt+")"+e+"("+rt+"|$)"))&&W(e,function(e){return t.test("string"==typeof e.className&&e.className||typeof e.getAttribute!==J&&e.getAttribute("class")||"")})},ATTR:function(e,n,r){return function(o){var i=t.attr(o,e);return null==i?"!="===n:n?(i+="","="===n?i===r:"!="===n?i!==r:"^="===n?r&&0===i.indexOf(r):"*="===n?r&&i.indexOf(r)>-1:"$="===n?r&&i.slice(-r.length)===r:"~="===n?(" "+i+" ").indexOf(r)>-1:"|="===n?i===r||i.slice(0,r.length+1)===r+"-":!1):!0}},CHILD:function(e,t,n,r,o){var i="nth"!==e.slice(0,3),a="last"!==e.slice(-4),s="of-type"===t;return 1===r&&0===o?function(e){return!!e.parentNode}:function(t,n,u){var l,c,f,p,d,h,g=i!==a?"nextSibling":"previousSibling",m=t.parentNode,v=s&&t.nodeName.toLowerCase(),y=!u&&!s;if(m){if(i){for(;g;){for(f=t;f=f[g];)if(s?f.nodeName.toLowerCase()===v:1===f.nodeType)return!1;h=g="only"===e&&!h&&"nextSibling"}return!0}if(h=[a?m.firstChild:m.lastChild],a&&y){for(c=m[P]||(m[P]={}),l=c[e]||[],d=l[0]===I&&l[1],p=l[0]===I&&l[2],f=d&&m.childNodes[d];f=++d&&f&&f[g]||(p=d=0)||h.pop();)if(1===f.nodeType&&++p&&f===t){c[e]=[I,d,p];break}}else if(y&&(l=(t[P]||(t[P]={}))[e])&&l[0]===I)p=l[1];else for(;(f=++d&&f&&f[g]||(p=d=0)||h.pop())&&((s?f.nodeName.toLowerCase()!==v:1!==f.nodeType)||!++p||(y&&((f[P]||(f[P]={}))[e]=[I,p]),f!==t)););return p-=o,p===r||p%r===0&&p/r>=0}}},PSEUDO:function(e,n){var o,i=E.pseudos[e]||E.setFilters[e.toLowerCase()]||t.error("unsupported pseudo: "+e);return i[P]?i(n):i.length>1?(o=[e,e,"",n],E.setFilters.hasOwnProperty(e.toLowerCase())?r(function(e,t){for(var r,o=i(e,n),a=o.length;a--;)r=tt.call(e,o[a]),e[r]=!(t[r]=o[a])}):function(e){return i(e,0,o)}):i}},pseudos:{not:r(function(e){var t=[],n=[],o=S(e.replace(ut,"$1"));return o[P]?r(function(e,t,n,r){for(var i,a=o(e,null,r,[]),s=e.length;s--;)(i=a[s])&&(e[s]=!(t[s]=i))}):function(e,r,i){return t[0]=e,o(t,null,i,n),!n.pop()}}),has:r(function(e){return function(n){return t(e,n).length>0}}),contains:r(function(e){return function(t){return(t.textContent||t.innerText||k(t)).indexOf(e)>-1}}),lang:r(function(e){return dt.test(e||"")||t.error("unsupported lang: "+e),e=e.replace(wt,Tt).toLowerCase(),function(t){var n;do if(n=R?t.lang:t.getAttribute("xml:lang")||t.getAttribute("lang"))return n=n.toLowerCase(),n===e||0===n.indexOf(e+"-");while((t=t.parentNode)&&1===t.nodeType);return!1}}),target:function(t){var n=e.location&&e.location.hash;return n&&n.slice(1)===t.id},root:function(e){return e===D},focus:function(e){return e===O.activeElement&&(!O.hasFocus||O.hasFocus())&&!!(e.type||e.href||~e.tabIndex)},enabled:function(e){return e.disabled===!1},disabled:function(e){return e.disabled===!0},checked:function(e){var t=e.nodeName.toLowerCase();return"input"===t&&!!e.checked||"option"===t&&!!e.selected},selected:function(e){return e.parentNode&&e.parentNode.selectedIndex,e.selected===!0},empty:function(e){for(e=e.firstChild;e;e=e.nextSibling)if(e.nodeType<6)return!1;return!0},parent:function(e){return!E.pseudos.empty(e)},header:function(e){return mt.test(e.nodeName)},input:function(e){return gt.test(e.nodeName)},button:function(e){var t=e.nodeName.toLowerCase();return"input"===t&&"button"===e.type||"button"===t},text:function(e){var t;return"input"===e.nodeName.toLowerCase()&&"text"===e.type&&(null==(t=e.getAttribute("type"))||"text"===t.toLowerCase())},first:l(function(){return[0]}),last:l(function(e,t){return[t-1]}),eq:l(function(e,t,n){return[0>n?n+t:n]}),even:l(function(e,t){for(var n=0;t>n;n+=2)e.push(n);return e}),odd:l(function(e,t){for(var n=1;t>n;n+=2)e.push(n);return e}),lt:l(function(e,t,n){for(var r=0>n?n+t:n;--r>=0;)e.push(r);return e}),gt:l(function(e,t,n){for(var r=0>n?n+t:n;++r<t;)e.push(r);return e})}},E.pseudos.nth=E.pseudos.eq;for(T in{radio:!0,checkbox:!0,file:!0,password:!0,image:!0})E.pseudos[T]=s(T);for(T in{submit:!0,reset:!0})E.pseudos[T]=u(T);return f.prototype=E.filters=E.pseudos,E.setFilters=new f,S=t.compile=function(e,t){var n,r=[],o=[],i=U[e+" "];if(!i){for(t||(t=p(e)),n=t.length;n--;)i=y(t[n]),i[P]?r.push(i):o.push(i);i=U(e,b(o,r))}return i},C.sortStable=P.split("").sort(X).join("")===P,C.detectDuplicates=!!A,L(),C.sortDetached=o(function(e){return 1&e.compareDocumentPosition(O.createElement("div"))}),o(function(e){return e.innerHTML="<a href='#'></a>","#"===e.firstChild.getAttribute("href")})||i("type|href|height|width",function(e,t,n){return n?void 0:e.getAttribute(t,"type"===t.toLowerCase()?1:2)}),C.attributes&&o(function(e){return e.innerHTML="<input/>",e.firstChild.setAttribute("value",""),""===e.firstChild.getAttribute("value")})||i("value",function(e,t,n){return n||"input"!==e.nodeName.toLowerCase()?void 0:e.defaultValue}),o(function(e){return null==e.getAttribute("disabled")})||i(nt,function(e,t,n){var r;return n?void 0:e[t]===!0?t.toLowerCase():(r=e.getAttributeNode(t))&&r.specified?r.value:null}),t}(e);it.find=ct,it.expr=ct.selectors,it.expr[":"]=it.expr.pseudos,it.unique=ct.uniqueSort,it.text=ct.getText,it.isXMLDoc=ct.isXML,it.contains=ct.contains;var ft=it.expr.match.needsContext,pt=/^<(\w+)\s*\/?>(?:<\/\1>|)$/,dt=/^.[^:#\[\.,]*$/;it.filter=function(e,t,n){var r=t[0];return n&&(e=":not("+e+")"),1===t.length&&1===r.nodeType?it.find.matchesSelector(r,e)?[r]:[]:it.find.matches(e,it.grep(t,function(e){return 1===e.nodeType}))},it.fn.extend({find:function(e){var t,n=[],r=this,o=r.length;if("string"!=typeof e)return this.pushStack(it(e).filter(function(){for(t=0;o>t;t++)if(it.contains(r[t],this))return!0}));for(t=0;o>t;t++)it.find(e,r[t],n);return n=this.pushStack(o>1?it.unique(n):n),n.selector=this.selector?this.selector+" "+e:e,n},filter:function(e){return this.pushStack(r(this,e||[],!1))},not:function(e){return this.pushStack(r(this,e||[],!0))},is:function(e){return!!r(this,"string"==typeof e&&ft.test(e)?it(e):e||[],!1).length}});var ht,gt=e.document,mt=/^(?:\s*(<[\w\W]+>)[^>]*|#([\w-]*))$/,vt=it.fn.init=function(e,t){var n,r;if(!e)return this;if("string"==typeof e){if(n="<"===e.charAt(0)&&">"===e.charAt(e.length-1)&&e.length>=3?[null,e,null]:mt.exec(e),!n||!n[1]&&t)return!t||t.jquery?(t||ht).find(e):this.constructor(t).find(e);if(n[1]){if(t=t instanceof it?t[0]:t,it.merge(this,it.parseHTML(n[1],t&&t.nodeType?t.ownerDocument||t:gt,!0)),pt.test(n[1])&&it.isPlainObject(t))for(n in t)it.isFunction(this[n])?this[n](t[n]):this.attr(n,t[n]);return this}if(r=gt.getElementById(n[2]),r&&r.parentNode){if(r.id!==n[2])return ht.find(e);this.length=1,this[0]=r}return this.context=gt,this.selector=e,this}return e.nodeType?(this.context=this[0]=e,this.length=1,this):it.isFunction(e)?"undefined"!=typeof ht.ready?ht.ready(e):e(it):(void 0!==e.selector&&(this.selector=e.selector,this.context=e.context),it.makeArray(e,this))};vt.prototype=it.fn,ht=it(gt);var yt=/^(?:parents|prev(?:Until|All))/,bt={children:!0,contents:!0,next:!0,prev:!0};it.extend({dir:function(e,t,n){for(var r=[],o=e[t];o&&9!==o.nodeType&&(void 0===n||1!==o.nodeType||!it(o).is(n));)1===o.nodeType&&r.push(o),o=o[t];return r},sibling:function(e,t){for(var n=[];e;e=e.nextSibling)1===e.nodeType&&e!==t&&n.push(e);return n}}),it.fn.extend({has:function(e){var t,n=it(e,this),r=n.length;return this.filter(function(){for(t=0;r>t;t++)if(it.contains(this,n[t]))return!0})},closest:function(e,t){for(var n,r=0,o=this.length,i=[],a=ft.test(e)||"string"!=typeof e?it(e,t||this.context):0;o>r;r++)for(n=this[r];n&&n!==t;n=n.parentNode)if(n.nodeType<11&&(a?a.index(n)>-1:1===n.nodeType&&it.find.matchesSelector(n,e))){i.push(n);break}return this.pushStack(i.length>1?it.unique(i):i)},index:function(e){return e?"string"==typeof e?it.inArray(this[0],it(e)):it.inArray(e.jquery?e[0]:e,this):this[0]&&this[0].parentNode?this.first().prevAll().length:-1},add:function(e,t){return this.pushStack(it.unique(it.merge(this.get(),it(e,t))))},addBack:function(e){return this.add(null==e?this.prevObject:this.prevObject.filter(e))}}),it.each({parent:function(e){var t=e.parentNode;return t&&11!==t.nodeType?t:null},parents:function(e){return it.dir(e,"parentNode")},parentsUntil:function(e,t,n){return it.dir(e,"parentNode",n)},next:function(e){return o(e,"nextSibling")},prev:function(e){return o(e,"previousSibling")},nextAll:function(e){return it.dir(e,"nextSibling")},prevAll:function(e){return it.dir(e,"previousSibling")},nextUntil:function(e,t,n){return it.dir(e,"nextSibling",n)},prevUntil:function(e,t,n){return it.dir(e,"previousSibling",n)},siblings:function(e){return it.sibling((e.parentNode||{}).firstChild,e)},children:function(e){return it.sibling(e.firstChild)},contents:function(e){return it.nodeName(e,"iframe")?e.contentDocument||e.contentWindow.document:it.merge([],e.childNodes)}},function(e,t){it.fn[e]=function(n,r){var o=it.map(this,t,n);return"Until"!==e.slice(-5)&&(r=n),r&&"string"==typeof r&&(o=it.filter(r,o)),this.length>1&&(bt[e]||(o=it.unique(o)),yt.test(e)&&(o=o.reverse())),this.pushStack(o)}});var xt=/\S+/g,wt={};it.Callbacks=function(e){e="string"==typeof e?wt[e]||i(e):it.extend({},e);var t,n,r,o,a,s,u=[],l=!e.once&&[],c=function(i){for(n=e.memory&&i,r=!0,a=s||0,s=0,o=u.length,t=!0;u&&o>a;a++)if(u[a].apply(i[0],i[1])===!1&&e.stopOnFalse){n=!1;break}t=!1,u&&(l?l.length&&c(l.shift()):n?u=[]:f.disable())},f={add:function(){if(u){var r=u.length;!function i(t){it.each(t,function(t,n){var r=it.type(n);"function"===r?e.unique&&f.has(n)||u.push(n):n&&n.length&&"string"!==r&&i(n)})}(arguments),t?o=u.length:n&&(s=r,c(n))}return this},remove:function(){return u&&it.each(arguments,function(e,n){for(var r;(r=it.inArray(n,u,r))>-1;)u.splice(r,1),t&&(o>=r&&o--,a>=r&&a--)}),this},has:function(e){return e?it.inArray(e,u)>-1:!(!u||!u.length)},empty:function(){return u=[],o=0,this},disable:function(){return u=l=n=void 0,this},disabled:function(){return!u},lock:function(){return l=void 0,n||f.disable(),this},locked:function(){return!l},fireWith:function(e,n){return!u||r&&!l||(n=n||[],n=[e,n.slice?n.slice():n],t?l.push(n):c(n)),this},fire:function(){return f.fireWith(this,arguments),this},fired:function(){return!!r}};return f},it.extend({Deferred:function(e){var t=[["resolve","done",it.Callbacks("once memory"),"resolved"],["reject","fail",it.Callbacks("once memory"),"rejected"],["notify","progress",it.Callbacks("memory")]],n="pending",r={state:function(){return n},always:function(){return o.done(arguments).fail(arguments),this},then:function(){var e=arguments;return it.Deferred(function(n){it.each(t,function(t,i){var a=it.isFunction(e[t])&&e[t];o[i[1]](function(){var e=a&&a.apply(this,arguments);e&&it.isFunction(e.promise)?e.promise().done(n.resolve).fail(n.reject).progress(n.notify):n[i[0]+"With"](this===r?n.promise():this,a?[e]:arguments)})}),e=null}).promise()},promise:function(e){return null!=e?it.extend(e,r):r}},o={};return r.pipe=r.then,it.each(t,function(e,i){var a=i[2],s=i[3];r[i[1]]=a.add,s&&a.add(function(){n=s},t[1^e][2].disable,t[2][2].lock),o[i[0]]=function(){return o[i[0]+"With"](this===o?r:this,arguments),this},o[i[0]+"With"]=a.fireWith}),r.promise(o),e&&e.call(o,o),o},when:function(e){var t,n,r,o=0,i=G.call(arguments),a=i.length,s=1!==a||e&&it.isFunction(e.promise)?a:0,u=1===s?e:it.Deferred(),l=function(e,n,r){return function(o){n[e]=this,r[e]=arguments.length>1?G.call(arguments):o,r===t?u.notifyWith(n,r):--s||u.resolveWith(n,r)}};if(a>1)for(t=new Array(a),n=new Array(a),r=new Array(a);a>o;o++)i[o]&&it.isFunction(i[o].promise)?i[o].promise().done(l(o,r,i)).fail(u.reject).progress(l(o,n,t)):--s;return s||u.resolveWith(r,i),u.promise()}});var Tt;it.fn.ready=function(e){return it.ready.promise().done(e),this},it.extend({isReady:!1,readyWait:1,holdReady:function(e){e?it.readyWait++:it.ready(!0)},ready:function(e){if(e===!0?!--it.readyWait:!it.isReady){if(!gt.body)return setTimeout(it.ready);it.isReady=!0,e!==!0&&--it.readyWait>0||(Tt.resolveWith(gt,[it]),it.fn.trigger&&it(gt).trigger("ready").off("ready"))}}}),it.ready.promise=function(t){if(!Tt)if(Tt=it.Deferred(),"complete"===gt.readyState)setTimeout(it.ready);else if(gt.addEventListener)gt.addEventListener("DOMContentLoaded",s,!1),e.addEventListener("load",s,!1);else{gt.attachEvent("onreadystatechange",s),e.attachEvent("onload",s);var n=!1;try{n=null==e.frameElement&&gt.documentElement}catch(r){}n&&n.doScroll&&!function o(){if(!it.isReady){try{n.doScroll("left")}catch(e){return setTimeout(o,50)}a(),it.ready()}}()}return Tt.promise(t)};var Ct,Et="undefined";for(Ct in it(rt))break;rt.ownLast="0"!==Ct,rt.inlineBlockNeedsLayout=!1,it(function(){var e,t,n=gt.getElementsByTagName("body")[0];n&&(e=gt.createElement("div"),e.style.cssText="border:0;width:0;height:0;position:absolute;top:0;left:-9999px;margin-top:1px",t=gt.createElement("div"),n.appendChild(e).appendChild(t),typeof t.style.zoom!==Et&&(t.style.cssText="border:0;margin:0;width:1px;padding:1px;display:inline;zoom:1",(rt.inlineBlockNeedsLayout=3===t.offsetWidth)&&(n.style.zoom=1)),n.removeChild(e),e=t=null)}),function(){var e=gt.createElement("div");if(null==rt.deleteExpando){rt.deleteExpando=!0;try{delete e.test}catch(t){rt.deleteExpando=!1}}e=null}(),it.acceptData=function(e){var t=it.noData[(e.nodeName+" ").toLowerCase()],n=+e.nodeType||1;return 1!==n&&9!==n?!1:!t||t!==!0&&e.getAttribute("classid")===t};var kt=/^(?:\{[\w\W]*\}|\[[\w\W]*\])$/,_t=/([A-Z])/g;it.extend({cache:{},noData:{"applet ":!0,"embed ":!0,"object ":"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"},hasData:function(e){return e=e.nodeType?it.cache[e[it.expando]]:e[it.expando],!!e&&!l(e)},data:function(e,t,n){return c(e,t,n)},removeData:function(e,t){return f(e,t)},_data:function(e,t,n){return c(e,t,n,!0)},_removeData:function(e,t){return f(e,t,!0)}}),it.fn.extend({data:function(e,t){var n,r,o,i=this[0],a=i&&i.attributes;if(void 0===e){if(this.length&&(o=it.data(i),1===i.nodeType&&!it._data(i,"parsedAttrs"))){for(n=a.length;n--;)r=a[n].name,0===r.indexOf("data-")&&(r=it.camelCase(r.slice(5)),u(i,r,o[r]));it._data(i,"parsedAttrs",!0)}return o}return"object"==typeof e?this.each(function(){it.data(this,e)}):arguments.length>1?this.each(function(){it.data(this,e,t)}):i?u(i,e,it.data(i,e)):void 0},removeData:function(e){return this.each(function(){it.removeData(this,e)})}}),it.extend({queue:function(e,t,n){var r;return e?(t=(t||"fx")+"queue",r=it._data(e,t),n&&(!r||it.isArray(n)?r=it._data(e,t,it.makeArray(n)):r.push(n)),r||[]):void 0},dequeue:function(e,t){t=t||"fx";var n=it.queue(e,t),r=n.length,o=n.shift(),i=it._queueHooks(e,t),a=function(){it.dequeue(e,t)};"inprogress"===o&&(o=n.shift(),r--),o&&("fx"===t&&n.unshift("inprogress"),delete i.stop,o.call(e,a,i)),!r&&i&&i.empty.fire()},_queueHooks:function(e,t){var n=t+"queueHooks";return it._data(e,n)||it._data(e,n,{empty:it.Callbacks("once memory").add(function(){it._removeData(e,t+"queue"),it._removeData(e,n)})})}}),it.fn.extend({queue:function(e,t){var n=2;return"string"!=typeof e&&(t=e,e="fx",n--),arguments.length<n?it.queue(this[0],e):void 0===t?this:this.each(function(){var n=it.queue(this,e,t);it._queueHooks(this,e),"fx"===e&&"inprogress"!==n[0]&&it.dequeue(this,e)})},dequeue:function(e){return this.each(function(){it.dequeue(this,e)})},clearQueue:function(e){return this.queue(e||"fx",[])},promise:function(e,t){var n,r=1,o=it.Deferred(),i=this,a=this.length,s=function(){--r||o.resolveWith(i,[i])};for("string"!=typeof e&&(t=e,e=void 0),e=e||"fx";a--;)n=it._data(i[a],e+"queueHooks"),n&&n.empty&&(r++,n.empty.add(s));return s(),o.promise(t)}});var St=/[+-]?(?:\d*\.|)\d+(?:[eE][+-]?\d+|)/.source,Nt=["Top","Right","Bottom","Left"],jt=function(e,t){return e=t||e,"none"===it.css(e,"display")||!it.contains(e.ownerDocument,e)},At=it.access=function(e,t,n,r,o,i,a){var s=0,u=e.length,l=null==n;if("object"===it.type(n)){o=!0;for(s in n)it.access(e,t,s,n[s],!0,i,a)}else if(void 0!==r&&(o=!0,it.isFunction(r)||(a=!0),l&&(a?(t.call(e,r),t=null):(l=t,t=function(e,t,n){return l.call(it(e),n)})),t))for(;u>s;s++)t(e[s],n,a?r:r.call(e[s],s,t(e[s],n)));return o?e:l?t.call(e):u?t(e[0],n):i},Lt=/^(?:checkbox|radio)$/i;!function(){var e=gt.createDocumentFragment(),t=gt.createElement("div"),n=gt.createElement("input");if(t.setAttribute("className","t"),t.innerHTML="  <link/><table></table><a href='/a'>a</a>",rt.leadingWhitespace=3===t.firstChild.nodeType,rt.tbody=!t.getElementsByTagName("tbody").length,rt.htmlSerialize=!!t.getElementsByTagName("link").length,rt.html5Clone="<:nav></:nav>"!==gt.createElement("nav").cloneNode(!0).outerHTML,n.type="checkbox",n.checked=!0,e.appendChild(n),rt.appendChecked=n.checked,t.innerHTML="<textarea>x</textarea>",rt.noCloneChecked=!!t.cloneNode(!0).lastChild.defaultValue,e.appendChild(t),t.innerHTML="<input type='radio' checked='checked' name='t'/>",rt.checkClone=t.cloneNode(!0).cloneNode(!0).lastChild.checked,rt.noCloneEvent=!0,t.attachEvent&&(t.attachEvent("onclick",function(){rt.noCloneEvent=!1}),t.cloneNode(!0).click()),null==rt.deleteExpando){rt.deleteExpando=!0;try{delete t.test}catch(r){rt.deleteExpando=!1}}e=t=n=null}(),function(){var t,n,r=gt.createElement("div");for(t in{submit:!0,change:!0,focusin:!0})n="on"+t,(rt[t+"Bubbles"]=n in e)||(r.setAttribute(n,"t"),rt[t+"Bubbles"]=r.attributes[n].expando===!1);r=null}();var Ot=/^(?:input|select|textarea)$/i,Dt=/^key/,Rt=/^(?:mouse|contextmenu)|click/,Mt=/^(?:focusinfocus|focusoutblur)$/,Ht=/^([^.]*)(?:\.(.+)|)$/;it.event={global:{},add:function(e,t,n,r,o){var i,a,s,u,l,c,f,p,d,h,g,m=it._data(e);if(m){for(n.handler&&(u=n,n=u.handler,o=u.selector),n.guid||(n.guid=it.guid++),(a=m.events)||(a=m.events={}),(c=m.handle)||(c=m.handle=function(e){return typeof it===Et||e&&it.event.triggered===e.type?void 0:it.event.dispatch.apply(c.elem,arguments)},c.elem=e),t=(t||"").match(xt)||[""],s=t.length;s--;)i=Ht.exec(t[s])||[],d=g=i[1],h=(i[2]||"").split(".").sort(),d&&(l=it.event.special[d]||{},d=(o?l.delegateType:l.bindType)||d,l=it.event.special[d]||{},f=it.extend({type:d,origType:g,data:r,handler:n,guid:n.guid,selector:o,needsContext:o&&it.expr.match.needsContext.test(o),namespace:h.join(".")},u),(p=a[d])||(p=a[d]=[],p.delegateCount=0,l.setup&&l.setup.call(e,r,h,c)!==!1||(e.addEventListener?e.addEventListener(d,c,!1):e.attachEvent&&e.attachEvent("on"+d,c))),l.add&&(l.add.call(e,f),f.handler.guid||(f.handler.guid=n.guid)),o?p.splice(p.delegateCount++,0,f):p.push(f),it.event.global[d]=!0);e=null}},remove:function(e,t,n,r,o){var i,a,s,u,l,c,f,p,d,h,g,m=it.hasData(e)&&it._data(e);if(m&&(c=m.events)){for(t=(t||"").match(xt)||[""],l=t.length;l--;)if(s=Ht.exec(t[l])||[],d=g=s[1],h=(s[2]||"").split(".").sort(),d){for(f=it.event.special[d]||{},d=(r?f.delegateType:f.bindType)||d,p=c[d]||[],s=s[2]&&new RegExp("(^|\\.)"+h.join("\\.(?:.*\\.|)")+"(\\.|$)"),u=i=p.length;i--;)a=p[i],!o&&g!==a.origType||n&&n.guid!==a.guid||s&&!s.test(a.namespace)||r&&r!==a.selector&&("**"!==r||!a.selector)||(p.splice(i,1),a.selector&&p.delegateCount--,f.remove&&f.remove.call(e,a));u&&!p.length&&(f.teardown&&f.teardown.call(e,h,m.handle)!==!1||it.removeEvent(e,d,m.handle),delete c[d])}else for(d in c)it.event.remove(e,d+t[l],n,r,!0);it.isEmptyObject(c)&&(delete m.handle,it._removeData(e,"events"))}},trigger:function(t,n,r,o){var i,a,s,u,l,c,f,p=[r||gt],d=tt.call(t,"type")?t.type:t,h=tt.call(t,"namespace")?t.namespace.split("."):[];if(s=c=r=r||gt,3!==r.nodeType&&8!==r.nodeType&&!Mt.test(d+it.event.triggered)&&(d.indexOf(".")>=0&&(h=d.split("."),d=h.shift(),h.sort()),a=d.indexOf(":")<0&&"on"+d,t=t[it.expando]?t:new it.Event(d,"object"==typeof t&&t),t.isTrigger=o?2:3,t.namespace=h.join("."),t.namespace_re=t.namespace?new RegExp("(^|\\.)"+h.join("\\.(?:.*\\.|)")+"(\\.|$)"):null,t.result=void 0,t.target||(t.target=r),n=null==n?[t]:it.makeArray(n,[t]),l=it.event.special[d]||{},o||!l.trigger||l.trigger.apply(r,n)!==!1)){if(!o&&!l.noBubble&&!it.isWindow(r)){for(u=l.delegateType||d,Mt.test(u+d)||(s=s.parentNode);s;s=s.parentNode)p.push(s),c=s;c===(r.ownerDocument||gt)&&p.push(c.defaultView||c.parentWindow||e)}for(f=0;(s=p[f++])&&!t.isPropagationStopped();)t.type=f>1?u:l.bindType||d,i=(it._data(s,"events")||{})[t.type]&&it._data(s,"handle"),i&&i.apply(s,n),i=a&&s[a],i&&i.apply&&it.acceptData(s)&&(t.result=i.apply(s,n),t.result===!1&&t.preventDefault());if(t.type=d,!o&&!t.isDefaultPrevented()&&(!l._default||l._default.apply(p.pop(),n)===!1)&&it.acceptData(r)&&a&&r[d]&&!it.isWindow(r)){c=r[a],c&&(r[a]=null),it.event.triggered=d;try{r[d]()}catch(g){}it.event.triggered=void 0,c&&(r[a]=c)}return t.result}},dispatch:function(e){e=it.event.fix(e);var t,n,r,o,i,a=[],s=G.call(arguments),u=(it._data(this,"events")||{})[e.type]||[],l=it.event.special[e.type]||{};if(s[0]=e,e.delegateTarget=this,!l.preDispatch||l.preDispatch.call(this,e)!==!1){for(a=it.event.handlers.call(this,e,u),t=0;(o=a[t++])&&!e.isPropagationStopped();)for(e.currentTarget=o.elem,i=0;(r=o.handlers[i++])&&!e.isImmediatePropagationStopped();)(!e.namespace_re||e.namespace_re.test(r.namespace))&&(e.handleObj=r,e.data=r.data,n=((it.event.special[r.origType]||{}).handle||r.handler).apply(o.elem,s),void 0!==n&&(e.result=n)===!1&&(e.preventDefault(),e.stopPropagation()));return l.postDispatch&&l.postDispatch.call(this,e),e.result}},handlers:function(e,t){var n,r,o,i,a=[],s=t.delegateCount,u=e.target;if(s&&u.nodeType&&(!e.button||"click"!==e.type))for(;u!=this;u=u.parentNode||this)if(1===u.nodeType&&(u.disabled!==!0||"click"!==e.type)){for(o=[],i=0;s>i;i++)r=t[i],n=r.selector+" ",void 0===o[n]&&(o[n]=r.needsContext?it(n,this).index(u)>=0:it.find(n,this,null,[u]).length),o[n]&&o.push(r);o.length&&a.push({elem:u,handlers:o})}return s<t.length&&a.push({elem:this,handlers:t.slice(s)}),a},fix:function(e){if(e[it.expando])return e;var t,n,r,o=e.type,i=e,a=this.fixHooks[o];for(a||(this.fixHooks[o]=a=Rt.test(o)?this.mouseHooks:Dt.test(o)?this.keyHooks:{}),r=a.props?this.props.concat(a.props):this.props,e=new it.Event(i),t=r.length;t--;)n=r[t],e[n]=i[n];return e.target||(e.target=i.srcElement||gt),3===e.target.nodeType&&(e.target=e.target.parentNode),e.metaKey=!!e.metaKey,a.filter?a.filter(e,i):e},props:"altKey bubbles cancelable ctrlKey currentTarget eventPhase metaKey relatedTarget shiftKey target timeStamp view which".split(" "),fixHooks:{},keyHooks:{props:"char charCode key keyCode".split(" "),filter:function(e,t){return null==e.which&&(e.which=null!=t.charCode?t.charCode:t.keyCode),e}},mouseHooks:{props:"button buttons clientX clientY fromElement offsetX offsetY pageX pageY screenX screenY toElement".split(" "),filter:function(e,t){var n,r,o,i=t.button,a=t.fromElement;return null==e.pageX&&null!=t.clientX&&(r=e.target.ownerDocument||gt,o=r.documentElement,n=r.body,e.pageX=t.clientX+(o&&o.scrollLeft||n&&n.scrollLeft||0)-(o&&o.clientLeft||n&&n.clientLeft||0),e.pageY=t.clientY+(o&&o.scrollTop||n&&n.scrollTop||0)-(o&&o.clientTop||n&&n.clientTop||0)),!e.relatedTarget&&a&&(e.relatedTarget=a===e.target?t.toElement:a),e.which||void 0===i||(e.which=1&i?1:2&i?3:4&i?2:0),e}},special:{load:{noBubble:!0},focus:{trigger:function(){if(this!==h()&&this.focus)try{return this.focus(),!1}catch(e){}},delegateType:"focusin"},blur:{trigger:function(){return this===h()&&this.blur?(this.blur(),!1):void 0},delegateType:"focusout"},click:{trigger:function(){return it.nodeName(this,"input")&&"checkbox"===this.type&&this.click?(this.click(),!1):void 0},_default:function(e){return it.nodeName(e.target,"a")}},beforeunload:{postDispatch:function(e){void 0!==e.result&&(e.originalEvent.returnValue=e.result)}}},simulate:function(e,t,n,r){var o=it.extend(new it.Event,n,{type:e,isSimulated:!0,originalEvent:{}});r?it.event.trigger(o,null,t):it.event.dispatch.call(t,o),o.isDefaultPrevented()&&n.preventDefault()}},it.removeEvent=gt.removeEventListener?function(e,t,n){e.removeEventListener&&e.removeEventListener(t,n,!1)}:function(e,t,n){var r="on"+t;e.detachEvent&&(typeof e[r]===Et&&(e[r]=null),e.detachEvent(r,n))},it.Event=function(e,t){return this instanceof it.Event?(e&&e.type?(this.originalEvent=e,this.type=e.type,this.isDefaultPrevented=e.defaultPrevented||void 0===e.defaultPrevented&&(e.returnValue===!1||e.getPreventDefault&&e.getPreventDefault())?p:d):this.type=e,t&&it.extend(this,t),this.timeStamp=e&&e.timeStamp||it.now(),void(this[it.expando]=!0)):new it.Event(e,t)},it.Event.prototype={isDefaultPrevented:d,isPropagationStopped:d,isImmediatePropagationStopped:d,preventDefault:function(){var e=this.originalEvent;this.isDefaultPrevented=p,e&&(e.preventDefault?e.preventDefault():e.returnValue=!1)},stopPropagation:function(){var e=this.originalEvent;this.isPropagationStopped=p,e&&(e.stopPropagation&&e.stopPropagation(),e.cancelBubble=!0)},stopImmediatePropagation:function(){this.isImmediatePropagationStopped=p,this.stopPropagation()}},it.each({mouseenter:"mouseover",mouseleave:"mouseout"},function(e,t){it.event.special[e]={delegateType:t,bindType:t,handle:function(e){var n,r=this,o=e.relatedTarget,i=e.handleObj;return(!o||o!==r&&!it.contains(r,o))&&(e.type=i.origType,n=i.handler.apply(this,arguments),e.type=t),n}}}),rt.submitBubbles||(it.event.special.submit={setup:function(){return it.nodeName(this,"form")?!1:void it.event.add(this,"click._submit keypress._submit",function(e){var t=e.target,n=it.nodeName(t,"input")||it.nodeName(t,"button")?t.form:void 0;n&&!it._data(n,"submitBubbles")&&(it.event.add(n,"submit._submit",function(e){e._submit_bubble=!0}),it._data(n,"submitBubbles",!0))})},postDispatch:function(e){e._submit_bubble&&(delete e._submit_bubble,this.parentNode&&!e.isTrigger&&it.event.simulate("submit",this.parentNode,e,!0))},teardown:function(){return it.nodeName(this,"form")?!1:void it.event.remove(this,"._submit")}}),rt.changeBubbles||(it.event.special.change={setup:function(){return Ot.test(this.nodeName)?(("checkbox"===this.type||"radio"===this.type)&&(it.event.add(this,"propertychange._change",function(e){"checked"===e.originalEvent.propertyName&&(this._just_changed=!0)}),it.event.add(this,"click._change",function(e){this._just_changed&&!e.isTrigger&&(this._just_changed=!1),it.event.simulate("change",this,e,!0)})),!1):void it.event.add(this,"beforeactivate._change",function(e){var t=e.target;Ot.test(t.nodeName)&&!it._data(t,"changeBubbles")&&(it.event.add(t,"change._change",function(e){!this.parentNode||e.isSimulated||e.isTrigger||it.event.simulate("change",this.parentNode,e,!0)
}),it._data(t,"changeBubbles",!0))})},handle:function(e){var t=e.target;return this!==t||e.isSimulated||e.isTrigger||"radio"!==t.type&&"checkbox"!==t.type?e.handleObj.handler.apply(this,arguments):void 0},teardown:function(){return it.event.remove(this,"._change"),!Ot.test(this.nodeName)}}),rt.focusinBubbles||it.each({focus:"focusin",blur:"focusout"},function(e,t){var n=function(e){it.event.simulate(t,e.target,it.event.fix(e),!0)};it.event.special[t]={setup:function(){var r=this.ownerDocument||this,o=it._data(r,t);o||r.addEventListener(e,n,!0),it._data(r,t,(o||0)+1)},teardown:function(){var r=this.ownerDocument||this,o=it._data(r,t)-1;o?it._data(r,t,o):(r.removeEventListener(e,n,!0),it._removeData(r,t))}}}),it.fn.extend({on:function(e,t,n,r,o){var i,a;if("object"==typeof e){"string"!=typeof t&&(n=n||t,t=void 0);for(i in e)this.on(i,t,n,e[i],o);return this}if(null==n&&null==r?(r=t,n=t=void 0):null==r&&("string"==typeof t?(r=n,n=void 0):(r=n,n=t,t=void 0)),r===!1)r=d;else if(!r)return this;return 1===o&&(a=r,r=function(e){return it().off(e),a.apply(this,arguments)},r.guid=a.guid||(a.guid=it.guid++)),this.each(function(){it.event.add(this,e,r,n,t)})},one:function(e,t,n,r){return this.on(e,t,n,r,1)},off:function(e,t,n){var r,o;if(e&&e.preventDefault&&e.handleObj)return r=e.handleObj,it(e.delegateTarget).off(r.namespace?r.origType+"."+r.namespace:r.origType,r.selector,r.handler),this;if("object"==typeof e){for(o in e)this.off(o,t,e[o]);return this}return(t===!1||"function"==typeof t)&&(n=t,t=void 0),n===!1&&(n=d),this.each(function(){it.event.remove(this,e,n,t)})},trigger:function(e,t){return this.each(function(){it.event.trigger(e,t,this)})},triggerHandler:function(e,t){var n=this[0];return n?it.event.trigger(e,t,n,!0):void 0}});var qt="abbr|article|aside|audio|bdi|canvas|data|datalist|details|figcaption|figure|footer|header|hgroup|mark|meter|nav|output|progress|section|summary|time|video",Ft=/ jQuery\d+="(?:null|\d+)"/g,Pt=new RegExp("<(?:"+qt+")[\\s/>]","i"),Bt=/^\s+/,It=/<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/gi,$t=/<([\w:]+)/,Wt=/<tbody/i,zt=/<|&#?\w+;/,Ut=/<(?:script|style|link)/i,Xt=/checked\s*(?:[^=]|=\s*.checked.)/i,Jt=/^$|\/(?:java|ecma)script/i,Vt=/^true\/(.*)/,Gt=/^\s*<!(?:\[CDATA\[|--)|(?:\]\]|--)>\s*$/g,Qt={option:[1,"<select multiple='multiple'>","</select>"],legend:[1,"<fieldset>","</fieldset>"],area:[1,"<map>","</map>"],param:[1,"<object>","</object>"],thead:[1,"<table>","</table>"],tr:[2,"<table><tbody>","</tbody></table>"],col:[2,"<table><tbody></tbody><colgroup>","</colgroup></table>"],td:[3,"<table><tbody><tr>","</tr></tbody></table>"],_default:rt.htmlSerialize?[0,"",""]:[1,"X<div>","</div>"]},Yt=g(gt),Kt=Yt.appendChild(gt.createElement("div"));Qt.optgroup=Qt.option,Qt.tbody=Qt.tfoot=Qt.colgroup=Qt.caption=Qt.thead,Qt.th=Qt.td,it.extend({clone:function(e,t,n){var r,o,i,a,s,u=it.contains(e.ownerDocument,e);if(rt.html5Clone||it.isXMLDoc(e)||!Pt.test("<"+e.nodeName+">")?i=e.cloneNode(!0):(Kt.innerHTML=e.outerHTML,Kt.removeChild(i=Kt.firstChild)),!(rt.noCloneEvent&&rt.noCloneChecked||1!==e.nodeType&&11!==e.nodeType||it.isXMLDoc(e)))for(r=m(i),s=m(e),a=0;null!=(o=s[a]);++a)r[a]&&C(o,r[a]);if(t)if(n)for(s=s||m(e),r=r||m(i),a=0;null!=(o=s[a]);a++)T(o,r[a]);else T(e,i);return r=m(i,"script"),r.length>0&&w(r,!u&&m(e,"script")),r=s=o=null,i},buildFragment:function(e,t,n,r){for(var o,i,a,s,u,l,c,f=e.length,p=g(t),d=[],h=0;f>h;h++)if(i=e[h],i||0===i)if("object"===it.type(i))it.merge(d,i.nodeType?[i]:i);else if(zt.test(i)){for(s=s||p.appendChild(t.createElement("div")),u=($t.exec(i)||["",""])[1].toLowerCase(),c=Qt[u]||Qt._default,s.innerHTML=c[1]+i.replace(It,"<$1></$2>")+c[2],o=c[0];o--;)s=s.lastChild;if(!rt.leadingWhitespace&&Bt.test(i)&&d.push(t.createTextNode(Bt.exec(i)[0])),!rt.tbody)for(i="table"!==u||Wt.test(i)?"<table>"!==c[1]||Wt.test(i)?0:s:s.firstChild,o=i&&i.childNodes.length;o--;)it.nodeName(l=i.childNodes[o],"tbody")&&!l.childNodes.length&&i.removeChild(l);for(it.merge(d,s.childNodes),s.textContent="";s.firstChild;)s.removeChild(s.firstChild);s=p.lastChild}else d.push(t.createTextNode(i));for(s&&p.removeChild(s),rt.appendChecked||it.grep(m(d,"input"),v),h=0;i=d[h++];)if((!r||-1===it.inArray(i,r))&&(a=it.contains(i.ownerDocument,i),s=m(p.appendChild(i),"script"),a&&w(s),n))for(o=0;i=s[o++];)Jt.test(i.type||"")&&n.push(i);return s=null,p},cleanData:function(e,t){for(var n,r,o,i,a=0,s=it.expando,u=it.cache,l=rt.deleteExpando,c=it.event.special;null!=(n=e[a]);a++)if((t||it.acceptData(n))&&(o=n[s],i=o&&u[o])){if(i.events)for(r in i.events)c[r]?it.event.remove(n,r):it.removeEvent(n,r,i.handle);u[o]&&(delete u[o],l?delete n[s]:typeof n.removeAttribute!==Et?n.removeAttribute(s):n[s]=null,V.push(o))}}}),it.fn.extend({text:function(e){return At(this,function(e){return void 0===e?it.text(this):this.empty().append((this[0]&&this[0].ownerDocument||gt).createTextNode(e))},null,e,arguments.length)},append:function(){return this.domManip(arguments,function(e){if(1===this.nodeType||11===this.nodeType||9===this.nodeType){var t=y(this,e);t.appendChild(e)}})},prepend:function(){return this.domManip(arguments,function(e){if(1===this.nodeType||11===this.nodeType||9===this.nodeType){var t=y(this,e);t.insertBefore(e,t.firstChild)}})},before:function(){return this.domManip(arguments,function(e){this.parentNode&&this.parentNode.insertBefore(e,this)})},after:function(){return this.domManip(arguments,function(e){this.parentNode&&this.parentNode.insertBefore(e,this.nextSibling)})},remove:function(e,t){for(var n,r=e?it.filter(e,this):this,o=0;null!=(n=r[o]);o++)t||1!==n.nodeType||it.cleanData(m(n)),n.parentNode&&(t&&it.contains(n.ownerDocument,n)&&w(m(n,"script")),n.parentNode.removeChild(n));return this},empty:function(){for(var e,t=0;null!=(e=this[t]);t++){for(1===e.nodeType&&it.cleanData(m(e,!1));e.firstChild;)e.removeChild(e.firstChild);e.options&&it.nodeName(e,"select")&&(e.options.length=0)}return this},clone:function(e,t){return e=null==e?!1:e,t=null==t?e:t,this.map(function(){return it.clone(this,e,t)})},html:function(e){return At(this,function(e){var t=this[0]||{},n=0,r=this.length;if(void 0===e)return 1===t.nodeType?t.innerHTML.replace(Ft,""):void 0;if(!("string"!=typeof e||Ut.test(e)||!rt.htmlSerialize&&Pt.test(e)||!rt.leadingWhitespace&&Bt.test(e)||Qt[($t.exec(e)||["",""])[1].toLowerCase()])){e=e.replace(It,"<$1></$2>");try{for(;r>n;n++)t=this[n]||{},1===t.nodeType&&(it.cleanData(m(t,!1)),t.innerHTML=e);t=0}catch(o){}}t&&this.empty().append(e)},null,e,arguments.length)},replaceWith:function(){var e=arguments[0];return this.domManip(arguments,function(t){e=this.parentNode,it.cleanData(m(this)),e&&e.replaceChild(t,this)}),e&&(e.length||e.nodeType)?this:this.remove()},detach:function(e){return this.remove(e,!0)},domManip:function(e,t){e=Q.apply([],e);var n,r,o,i,a,s,u=0,l=this.length,c=this,f=l-1,p=e[0],d=it.isFunction(p);if(d||l>1&&"string"==typeof p&&!rt.checkClone&&Xt.test(p))return this.each(function(n){var r=c.eq(n);d&&(e[0]=p.call(this,n,r.html())),r.domManip(e,t)});if(l&&(s=it.buildFragment(e,this[0].ownerDocument,!1,this),n=s.firstChild,1===s.childNodes.length&&(s=n),n)){for(i=it.map(m(s,"script"),b),o=i.length;l>u;u++)r=s,u!==f&&(r=it.clone(r,!0,!0),o&&it.merge(i,m(r,"script"))),t.call(this[u],r,u);if(o)for(a=i[i.length-1].ownerDocument,it.map(i,x),u=0;o>u;u++)r=i[u],Jt.test(r.type||"")&&!it._data(r,"globalEval")&&it.contains(a,r)&&(r.src?it._evalUrl&&it._evalUrl(r.src):it.globalEval((r.text||r.textContent||r.innerHTML||"").replace(Gt,"")));s=n=null}return this}}),it.each({appendTo:"append",prependTo:"prepend",insertBefore:"before",insertAfter:"after",replaceAll:"replaceWith"},function(e,t){it.fn[e]=function(e){for(var n,r=0,o=[],i=it(e),a=i.length-1;a>=r;r++)n=r===a?this:this.clone(!0),it(i[r])[t](n),Y.apply(o,n.get());return this.pushStack(o)}});var Zt,en={};!function(){var e,t,n=gt.createElement("div"),r="-webkit-box-sizing:content-box;-moz-box-sizing:content-box;box-sizing:content-box;display:block;padding:0;margin:0;border:0";n.innerHTML="  <link/><table></table><a href='/a'>a</a><input type='checkbox'/>",e=n.getElementsByTagName("a")[0],e.style.cssText="float:left;opacity:.5",rt.opacity=/^0.5/.test(e.style.opacity),rt.cssFloat=!!e.style.cssFloat,n.style.backgroundClip="content-box",n.cloneNode(!0).style.backgroundClip="",rt.clearCloneStyle="content-box"===n.style.backgroundClip,e=n=null,rt.shrinkWrapBlocks=function(){var e,n,o,i;if(null==t){if(e=gt.getElementsByTagName("body")[0],!e)return;i="border:0;width:0;height:0;position:absolute;top:0;left:-9999px",n=gt.createElement("div"),o=gt.createElement("div"),e.appendChild(n).appendChild(o),t=!1,typeof o.style.zoom!==Et&&(o.style.cssText=r+";width:1px;padding:1px;zoom:1",o.innerHTML="<div></div>",o.firstChild.style.width="5px",t=3!==o.offsetWidth),e.removeChild(n),e=n=o=null}return t}}();var tn,nn,rn=/^margin/,on=new RegExp("^("+St+")(?!px)[a-z%]+$","i"),an=/^(top|right|bottom|left)$/;e.getComputedStyle?(tn=function(e){return e.ownerDocument.defaultView.getComputedStyle(e,null)},nn=function(e,t,n){var r,o,i,a,s=e.style;return n=n||tn(e),a=n?n.getPropertyValue(t)||n[t]:void 0,n&&(""!==a||it.contains(e.ownerDocument,e)||(a=it.style(e,t)),on.test(a)&&rn.test(t)&&(r=s.width,o=s.minWidth,i=s.maxWidth,s.minWidth=s.maxWidth=s.width=a,a=n.width,s.width=r,s.minWidth=o,s.maxWidth=i)),void 0===a?a:a+""}):gt.documentElement.currentStyle&&(tn=function(e){return e.currentStyle},nn=function(e,t,n){var r,o,i,a,s=e.style;return n=n||tn(e),a=n?n[t]:void 0,null==a&&s&&s[t]&&(a=s[t]),on.test(a)&&!an.test(t)&&(r=s.left,o=e.runtimeStyle,i=o&&o.left,i&&(o.left=e.currentStyle.left),s.left="fontSize"===t?"1em":a,a=s.pixelLeft+"px",s.left=r,i&&(o.left=i)),void 0===a?a:a+""||"auto"}),function(){function t(){var t,n,r=gt.getElementsByTagName("body")[0];r&&(t=gt.createElement("div"),n=gt.createElement("div"),t.style.cssText=l,r.appendChild(t).appendChild(n),n.style.cssText="-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:absolute;display:block;padding:1px;border:1px;width:4px;margin-top:1%;top:1%",it.swap(r,null!=r.style.zoom?{zoom:1}:{},function(){o=4===n.offsetWidth}),i=!0,a=!1,s=!0,e.getComputedStyle&&(a="1%"!==(e.getComputedStyle(n,null)||{}).top,i="4px"===(e.getComputedStyle(n,null)||{width:"4px"}).width),r.removeChild(t),n=r=null)}var n,r,o,i,a,s,u=gt.createElement("div"),l="border:0;width:0;height:0;position:absolute;top:0;left:-9999px",c="-webkit-box-sizing:content-box;-moz-box-sizing:content-box;box-sizing:content-box;display:block;padding:0;margin:0;border:0";u.innerHTML="  <link/><table></table><a href='/a'>a</a><input type='checkbox'/>",n=u.getElementsByTagName("a")[0],n.style.cssText="float:left;opacity:.5",rt.opacity=/^0.5/.test(n.style.opacity),rt.cssFloat=!!n.style.cssFloat,u.style.backgroundClip="content-box",u.cloneNode(!0).style.backgroundClip="",rt.clearCloneStyle="content-box"===u.style.backgroundClip,n=u=null,it.extend(rt,{reliableHiddenOffsets:function(){if(null!=r)return r;var e,t,n,o=gt.createElement("div"),i=gt.getElementsByTagName("body")[0];if(i)return o.setAttribute("className","t"),o.innerHTML="  <link/><table></table><a href='/a'>a</a><input type='checkbox'/>",e=gt.createElement("div"),e.style.cssText=l,i.appendChild(e).appendChild(o),o.innerHTML="<table><tr><td></td><td>t</td></tr></table>",t=o.getElementsByTagName("td"),t[0].style.cssText="padding:0;margin:0;border:0;display:none",n=0===t[0].offsetHeight,t[0].style.display="",t[1].style.display="none",r=n&&0===t[0].offsetHeight,i.removeChild(e),o=i=null,r},boxSizing:function(){return null==o&&t(),o},boxSizingReliable:function(){return null==i&&t(),i},pixelPosition:function(){return null==a&&t(),a},reliableMarginRight:function(){var t,n,r,o;if(null==s&&e.getComputedStyle){if(t=gt.getElementsByTagName("body")[0],!t)return;n=gt.createElement("div"),r=gt.createElement("div"),n.style.cssText=l,t.appendChild(n).appendChild(r),o=r.appendChild(gt.createElement("div")),o.style.cssText=r.style.cssText=c,o.style.marginRight=o.style.width="0",r.style.width="1px",s=!parseFloat((e.getComputedStyle(o,null)||{}).marginRight),t.removeChild(n)}return s}})}(),it.swap=function(e,t,n,r){var o,i,a={};for(i in t)a[i]=e.style[i],e.style[i]=t[i];o=n.apply(e,r||[]);for(i in t)e.style[i]=a[i];return o};var sn=/alpha\([^)]*\)/i,un=/opacity\s*=\s*([^)]*)/,ln=/^(none|table(?!-c[ea]).+)/,cn=new RegExp("^("+St+")(.*)$","i"),fn=new RegExp("^([+-])=("+St+")","i"),pn={position:"absolute",visibility:"hidden",display:"block"},dn={letterSpacing:0,fontWeight:400},hn=["Webkit","O","Moz","ms"];it.extend({cssHooks:{opacity:{get:function(e,t){if(t){var n=nn(e,"opacity");return""===n?"1":n}}}},cssNumber:{columnCount:!0,fillOpacity:!0,fontWeight:!0,lineHeight:!0,opacity:!0,order:!0,orphans:!0,widows:!0,zIndex:!0,zoom:!0},cssProps:{"float":rt.cssFloat?"cssFloat":"styleFloat"},style:function(e,t,n,r){if(e&&3!==e.nodeType&&8!==e.nodeType&&e.style){var o,i,a,s=it.camelCase(t),u=e.style;if(t=it.cssProps[s]||(it.cssProps[s]=S(u,s)),a=it.cssHooks[t]||it.cssHooks[s],void 0===n)return a&&"get"in a&&void 0!==(o=a.get(e,!1,r))?o:u[t];if(i=typeof n,"string"===i&&(o=fn.exec(n))&&(n=(o[1]+1)*o[2]+parseFloat(it.css(e,t)),i="number"),null!=n&&n===n&&("number"!==i||it.cssNumber[s]||(n+="px"),rt.clearCloneStyle||""!==n||0!==t.indexOf("background")||(u[t]="inherit"),!(a&&"set"in a&&void 0===(n=a.set(e,n,r)))))try{u[t]="",u[t]=n}catch(l){}}},css:function(e,t,n,r){var o,i,a,s=it.camelCase(t);return t=it.cssProps[s]||(it.cssProps[s]=S(e.style,s)),a=it.cssHooks[t]||it.cssHooks[s],a&&"get"in a&&(i=a.get(e,!0,n)),void 0===i&&(i=nn(e,t,r)),"normal"===i&&t in dn&&(i=dn[t]),""===n||n?(o=parseFloat(i),n===!0||it.isNumeric(o)?o||0:i):i}}),it.each(["height","width"],function(e,t){it.cssHooks[t]={get:function(e,n,r){return n?0===e.offsetWidth&&ln.test(it.css(e,"display"))?it.swap(e,pn,function(){return L(e,t,r)}):L(e,t,r):void 0},set:function(e,n,r){var o=r&&tn(e);return j(e,n,r?A(e,t,r,rt.boxSizing()&&"border-box"===it.css(e,"boxSizing",!1,o),o):0)}}}),rt.opacity||(it.cssHooks.opacity={get:function(e,t){return un.test((t&&e.currentStyle?e.currentStyle.filter:e.style.filter)||"")?.01*parseFloat(RegExp.$1)+"":t?"1":""},set:function(e,t){var n=e.style,r=e.currentStyle,o=it.isNumeric(t)?"alpha(opacity="+100*t+")":"",i=r&&r.filter||n.filter||"";n.zoom=1,(t>=1||""===t)&&""===it.trim(i.replace(sn,""))&&n.removeAttribute&&(n.removeAttribute("filter"),""===t||r&&!r.filter)||(n.filter=sn.test(i)?i.replace(sn,o):i+" "+o)}}),it.cssHooks.marginRight=_(rt.reliableMarginRight,function(e,t){return t?it.swap(e,{display:"inline-block"},nn,[e,"marginRight"]):void 0}),it.each({margin:"",padding:"",border:"Width"},function(e,t){it.cssHooks[e+t]={expand:function(n){for(var r=0,o={},i="string"==typeof n?n.split(" "):[n];4>r;r++)o[e+Nt[r]+t]=i[r]||i[r-2]||i[0];return o}},rn.test(e)||(it.cssHooks[e+t].set=j)}),it.fn.extend({css:function(e,t){return At(this,function(e,t,n){var r,o,i={},a=0;if(it.isArray(t)){for(r=tn(e),o=t.length;o>a;a++)i[t[a]]=it.css(e,t[a],!1,r);return i}return void 0!==n?it.style(e,t,n):it.css(e,t)},e,t,arguments.length>1)},show:function(){return N(this,!0)},hide:function(){return N(this)},toggle:function(e){return"boolean"==typeof e?e?this.show():this.hide():this.each(function(){jt(this)?it(this).show():it(this).hide()})}}),it.Tween=O,O.prototype={constructor:O,init:function(e,t,n,r,o,i){this.elem=e,this.prop=n,this.easing=o||"swing",this.options=t,this.start=this.now=this.cur(),this.end=r,this.unit=i||(it.cssNumber[n]?"":"px")},cur:function(){var e=O.propHooks[this.prop];return e&&e.get?e.get(this):O.propHooks._default.get(this)},run:function(e){var t,n=O.propHooks[this.prop];return this.pos=t=this.options.duration?it.easing[this.easing](e,this.options.duration*e,0,1,this.options.duration):e,this.now=(this.end-this.start)*t+this.start,this.options.step&&this.options.step.call(this.elem,this.now,this),n&&n.set?n.set(this):O.propHooks._default.set(this),this}},O.prototype.init.prototype=O.prototype,O.propHooks={_default:{get:function(e){var t;return null==e.elem[e.prop]||e.elem.style&&null!=e.elem.style[e.prop]?(t=it.css(e.elem,e.prop,""),t&&"auto"!==t?t:0):e.elem[e.prop]},set:function(e){it.fx.step[e.prop]?it.fx.step[e.prop](e):e.elem.style&&(null!=e.elem.style[it.cssProps[e.prop]]||it.cssHooks[e.prop])?it.style(e.elem,e.prop,e.now+e.unit):e.elem[e.prop]=e.now}}},O.propHooks.scrollTop=O.propHooks.scrollLeft={set:function(e){e.elem.nodeType&&e.elem.parentNode&&(e.elem[e.prop]=e.now)}},it.easing={linear:function(e){return e},swing:function(e){return.5-Math.cos(e*Math.PI)/2}},it.fx=O.prototype.init,it.fx.step={};var gn,mn,vn=/^(?:toggle|show|hide)$/,yn=new RegExp("^(?:([+-])=|)("+St+")([a-z%]*)$","i"),bn=/queueHooks$/,xn=[H],wn={"*":[function(e,t){var n=this.createTween(e,t),r=n.cur(),o=yn.exec(t),i=o&&o[3]||(it.cssNumber[e]?"":"px"),a=(it.cssNumber[e]||"px"!==i&&+r)&&yn.exec(it.css(n.elem,e)),s=1,u=20;if(a&&a[3]!==i){i=i||a[3],o=o||[],a=+r||1;do s=s||".5",a/=s,it.style(n.elem,e,a+i);while(s!==(s=n.cur()/r)&&1!==s&&--u)}return o&&(a=n.start=+a||+r||0,n.unit=i,n.end=o[1]?a+(o[1]+1)*o[2]:+o[2]),n}]};it.Animation=it.extend(F,{tweener:function(e,t){it.isFunction(e)?(t=e,e=["*"]):e=e.split(" ");for(var n,r=0,o=e.length;o>r;r++)n=e[r],wn[n]=wn[n]||[],wn[n].unshift(t)},prefilter:function(e,t){t?xn.unshift(e):xn.push(e)}}),it.speed=function(e,t,n){var r=e&&"object"==typeof e?it.extend({},e):{complete:n||!n&&t||it.isFunction(e)&&e,duration:e,easing:n&&t||t&&!it.isFunction(t)&&t};return r.duration=it.fx.off?0:"number"==typeof r.duration?r.duration:r.duration in it.fx.speeds?it.fx.speeds[r.duration]:it.fx.speeds._default,(null==r.queue||r.queue===!0)&&(r.queue="fx"),r.old=r.complete,r.complete=function(){it.isFunction(r.old)&&r.old.call(this),r.queue&&it.dequeue(this,r.queue)},r},it.fn.extend({fadeTo:function(e,t,n,r){return this.filter(jt).css("opacity",0).show().end().animate({opacity:t},e,n,r)},animate:function(e,t,n,r){var o=it.isEmptyObject(e),i=it.speed(t,n,r),a=function(){var t=F(this,it.extend({},e),i);(o||it._data(this,"finish"))&&t.stop(!0)};return a.finish=a,o||i.queue===!1?this.each(a):this.queue(i.queue,a)},stop:function(e,t,n){var r=function(e){var t=e.stop;delete e.stop,t(n)};return"string"!=typeof e&&(n=t,t=e,e=void 0),t&&e!==!1&&this.queue(e||"fx",[]),this.each(function(){var t=!0,o=null!=e&&e+"queueHooks",i=it.timers,a=it._data(this);if(o)a[o]&&a[o].stop&&r(a[o]);else for(o in a)a[o]&&a[o].stop&&bn.test(o)&&r(a[o]);for(o=i.length;o--;)i[o].elem!==this||null!=e&&i[o].queue!==e||(i[o].anim.stop(n),t=!1,i.splice(o,1));(t||!n)&&it.dequeue(this,e)})},finish:function(e){return e!==!1&&(e=e||"fx"),this.each(function(){var t,n=it._data(this),r=n[e+"queue"],o=n[e+"queueHooks"],i=it.timers,a=r?r.length:0;for(n.finish=!0,it.queue(this,e,[]),o&&o.stop&&o.stop.call(this,!0),t=i.length;t--;)i[t].elem===this&&i[t].queue===e&&(i[t].anim.stop(!0),i.splice(t,1));for(t=0;a>t;t++)r[t]&&r[t].finish&&r[t].finish.call(this);delete n.finish})}}),it.each(["toggle","show","hide"],function(e,t){var n=it.fn[t];it.fn[t]=function(e,r,o){return null==e||"boolean"==typeof e?n.apply(this,arguments):this.animate(R(t,!0),e,r,o)}}),it.each({slideDown:R("show"),slideUp:R("hide"),slideToggle:R("toggle"),fadeIn:{opacity:"show"},fadeOut:{opacity:"hide"},fadeToggle:{opacity:"toggle"}},function(e,t){it.fn[e]=function(e,n,r){return this.animate(t,e,n,r)}}),it.timers=[],it.fx.tick=function(){var e,t=it.timers,n=0;for(gn=it.now();n<t.length;n++)e=t[n],e()||t[n]!==e||t.splice(n--,1);t.length||it.fx.stop(),gn=void 0},it.fx.timer=function(e){it.timers.push(e),e()?it.fx.start():it.timers.pop()},it.fx.interval=13,it.fx.start=function(){mn||(mn=setInterval(it.fx.tick,it.fx.interval))},it.fx.stop=function(){clearInterval(mn),mn=null},it.fx.speeds={slow:600,fast:200,_default:400},it.fn.delay=function(e,t){return e=it.fx?it.fx.speeds[e]||e:e,t=t||"fx",this.queue(t,function(t,n){var r=setTimeout(t,e);n.stop=function(){clearTimeout(r)}})},function(){var e,t,n,r,o=gt.createElement("div");o.setAttribute("className","t"),o.innerHTML="  <link/><table></table><a href='/a'>a</a><input type='checkbox'/>",e=o.getElementsByTagName("a")[0],n=gt.createElement("select"),r=n.appendChild(gt.createElement("option")),t=o.getElementsByTagName("input")[0],e.style.cssText="top:1px",rt.getSetAttribute="t"!==o.className,rt.style=/top/.test(e.getAttribute("style")),rt.hrefNormalized="/a"===e.getAttribute("href"),rt.checkOn=!!t.value,rt.optSelected=r.selected,rt.enctype=!!gt.createElement("form").enctype,n.disabled=!0,rt.optDisabled=!r.disabled,t=gt.createElement("input"),t.setAttribute("value",""),rt.input=""===t.getAttribute("value"),t.value="t",t.setAttribute("type","radio"),rt.radioValue="t"===t.value,e=t=n=r=o=null}();var Tn=/\r/g;it.fn.extend({val:function(e){var t,n,r,o=this[0];{if(arguments.length)return r=it.isFunction(e),this.each(function(n){var o;1===this.nodeType&&(o=r?e.call(this,n,it(this).val()):e,null==o?o="":"number"==typeof o?o+="":it.isArray(o)&&(o=it.map(o,function(e){return null==e?"":e+""})),t=it.valHooks[this.type]||it.valHooks[this.nodeName.toLowerCase()],t&&"set"in t&&void 0!==t.set(this,o,"value")||(this.value=o))});if(o)return t=it.valHooks[o.type]||it.valHooks[o.nodeName.toLowerCase()],t&&"get"in t&&void 0!==(n=t.get(o,"value"))?n:(n=o.value,"string"==typeof n?n.replace(Tn,""):null==n?"":n)}}}),it.extend({valHooks:{option:{get:function(e){var t=it.find.attr(e,"value");return null!=t?t:it.text(e)}},select:{get:function(e){for(var t,n,r=e.options,o=e.selectedIndex,i="select-one"===e.type||0>o,a=i?null:[],s=i?o+1:r.length,u=0>o?s:i?o:0;s>u;u++)if(n=r[u],!(!n.selected&&u!==o||(rt.optDisabled?n.disabled:null!==n.getAttribute("disabled"))||n.parentNode.disabled&&it.nodeName(n.parentNode,"optgroup"))){if(t=it(n).val(),i)return t;a.push(t)}return a},set:function(e,t){for(var n,r,o=e.options,i=it.makeArray(t),a=o.length;a--;)if(r=o[a],it.inArray(it.valHooks.option.get(r),i)>=0)try{r.selected=n=!0}catch(s){r.scrollHeight}else r.selected=!1;return n||(e.selectedIndex=-1),o}}}}),it.each(["radio","checkbox"],function(){it.valHooks[this]={set:function(e,t){return it.isArray(t)?e.checked=it.inArray(it(e).val(),t)>=0:void 0}},rt.checkOn||(it.valHooks[this].get=function(e){return null===e.getAttribute("value")?"on":e.value})});var Cn,En,kn=it.expr.attrHandle,_n=/^(?:checked|selected)$/i,Sn=rt.getSetAttribute,Nn=rt.input;it.fn.extend({attr:function(e,t){return At(this,it.attr,e,t,arguments.length>1)},removeAttr:function(e){return this.each(function(){it.removeAttr(this,e)})}}),it.extend({attr:function(e,t,n){var r,o,i=e.nodeType;if(e&&3!==i&&8!==i&&2!==i)return typeof e.getAttribute===Et?it.prop(e,t,n):(1===i&&it.isXMLDoc(e)||(t=t.toLowerCase(),r=it.attrHooks[t]||(it.expr.match.bool.test(t)?En:Cn)),void 0===n?r&&"get"in r&&null!==(o=r.get(e,t))?o:(o=it.find.attr(e,t),null==o?void 0:o):null!==n?r&&"set"in r&&void 0!==(o=r.set(e,n,t))?o:(e.setAttribute(t,n+""),n):void it.removeAttr(e,t))},removeAttr:function(e,t){var n,r,o=0,i=t&&t.match(xt);if(i&&1===e.nodeType)for(;n=i[o++];)r=it.propFix[n]||n,it.expr.match.bool.test(n)?Nn&&Sn||!_n.test(n)?e[r]=!1:e[it.camelCase("default-"+n)]=e[r]=!1:it.attr(e,n,""),e.removeAttribute(Sn?n:r)},attrHooks:{type:{set:function(e,t){if(!rt.radioValue&&"radio"===t&&it.nodeName(e,"input")){var n=e.value;return e.setAttribute("type",t),n&&(e.value=n),t}}}}}),En={set:function(e,t,n){return t===!1?it.removeAttr(e,n):Nn&&Sn||!_n.test(n)?e.setAttribute(!Sn&&it.propFix[n]||n,n):e[it.camelCase("default-"+n)]=e[n]=!0,n}},it.each(it.expr.match.bool.source.match(/\w+/g),function(e,t){var n=kn[t]||it.find.attr;kn[t]=Nn&&Sn||!_n.test(t)?function(e,t,r){var o,i;return r||(i=kn[t],kn[t]=o,o=null!=n(e,t,r)?t.toLowerCase():null,kn[t]=i),o}:function(e,t,n){return n?void 0:e[it.camelCase("default-"+t)]?t.toLowerCase():null}}),Nn&&Sn||(it.attrHooks.value={set:function(e,t,n){return it.nodeName(e,"input")?void(e.defaultValue=t):Cn&&Cn.set(e,t,n)}}),Sn||(Cn={set:function(e,t,n){var r=e.getAttributeNode(n);return r||e.setAttributeNode(r=e.ownerDocument.createAttribute(n)),r.value=t+="","value"===n||t===e.getAttribute(n)?t:void 0}},kn.id=kn.name=kn.coords=function(e,t,n){var r;return n?void 0:(r=e.getAttributeNode(t))&&""!==r.value?r.value:null},it.valHooks.button={get:function(e,t){var n=e.getAttributeNode(t);return n&&n.specified?n.value:void 0},set:Cn.set},it.attrHooks.contenteditable={set:function(e,t,n){Cn.set(e,""===t?!1:t,n)}},it.each(["width","height"],function(e,t){it.attrHooks[t]={set:function(e,n){return""===n?(e.setAttribute(t,"auto"),n):void 0}}})),rt.style||(it.attrHooks.style={get:function(e){return e.style.cssText||void 0},set:function(e,t){return e.style.cssText=t+""}});var jn=/^(?:input|select|textarea|button|object)$/i,An=/^(?:a|area)$/i;it.fn.extend({prop:function(e,t){return At(this,it.prop,e,t,arguments.length>1)},removeProp:function(e){return e=it.propFix[e]||e,this.each(function(){try{this[e]=void 0,delete this[e]}catch(t){}})}}),it.extend({propFix:{"for":"htmlFor","class":"className"},prop:function(e,t,n){var r,o,i,a=e.nodeType;if(e&&3!==a&&8!==a&&2!==a)return i=1!==a||!it.isXMLDoc(e),i&&(t=it.propFix[t]||t,o=it.propHooks[t]),void 0!==n?o&&"set"in o&&void 0!==(r=o.set(e,n,t))?r:e[t]=n:o&&"get"in o&&null!==(r=o.get(e,t))?r:e[t]},propHooks:{tabIndex:{get:function(e){var t=it.find.attr(e,"tabindex");return t?parseInt(t,10):jn.test(e.nodeName)||An.test(e.nodeName)&&e.href?0:-1}}}}),rt.hrefNormalized||it.each(["href","src"],function(e,t){it.propHooks[t]={get:function(e){return e.getAttribute(t,4)}}}),rt.optSelected||(it.propHooks.selected={get:function(e){var t=e.parentNode;return t&&(t.selectedIndex,t.parentNode&&t.parentNode.selectedIndex),null}}),it.each(["tabIndex","readOnly","maxLength","cellSpacing","cellPadding","rowSpan","colSpan","useMap","frameBorder","contentEditable"],function(){it.propFix[this.toLowerCase()]=this}),rt.enctype||(it.propFix.enctype="encoding");var Ln=/[\t\r\n\f]/g;it.fn.extend({addClass:function(e){var t,n,r,o,i,a,s=0,u=this.length,l="string"==typeof e&&e;if(it.isFunction(e))return this.each(function(t){it(this).addClass(e.call(this,t,this.className))});if(l)for(t=(e||"").match(xt)||[];u>s;s++)if(n=this[s],r=1===n.nodeType&&(n.className?(" "+n.className+" ").replace(Ln," "):" ")){for(i=0;o=t[i++];)r.indexOf(" "+o+" ")<0&&(r+=o+" ");a=it.trim(r),n.className!==a&&(n.className=a)}return this},removeClass:function(e){var t,n,r,o,i,a,s=0,u=this.length,l=0===arguments.length||"string"==typeof e&&e;if(it.isFunction(e))return this.each(function(t){it(this).removeClass(e.call(this,t,this.className))});if(l)for(t=(e||"").match(xt)||[];u>s;s++)if(n=this[s],r=1===n.nodeType&&(n.className?(" "+n.className+" ").replace(Ln," "):"")){for(i=0;o=t[i++];)for(;r.indexOf(" "+o+" ")>=0;)r=r.replace(" "+o+" "," ");a=e?it.trim(r):"",n.className!==a&&(n.className=a)}return this},toggleClass:function(e,t){var n=typeof e;return"boolean"==typeof t&&"string"===n?t?this.addClass(e):this.removeClass(e):this.each(it.isFunction(e)?function(n){it(this).toggleClass(e.call(this,n,this.className,t),t)}:function(){if("string"===n)for(var t,r=0,o=it(this),i=e.match(xt)||[];t=i[r++];)o.hasClass(t)?o.removeClass(t):o.addClass(t);else(n===Et||"boolean"===n)&&(this.className&&it._data(this,"__className__",this.className),this.className=this.className||e===!1?"":it._data(this,"__className__")||"")})},hasClass:function(e){for(var t=" "+e+" ",n=0,r=this.length;r>n;n++)if(1===this[n].nodeType&&(" "+this[n].className+" ").replace(Ln," ").indexOf(t)>=0)return!0;return!1}}),it.each("blur focus focusin focusout load resize scroll unload click dblclick mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave change select submit keydown keypress keyup error contextmenu".split(" "),function(e,t){it.fn[t]=function(e,n){return arguments.length>0?this.on(t,null,e,n):this.trigger(t)}}),it.fn.extend({hover:function(e,t){return this.mouseenter(e).mouseleave(t||e)},bind:function(e,t,n){return this.on(e,null,t,n)},unbind:function(e,t){return this.off(e,null,t)},delegate:function(e,t,n,r){return this.on(t,e,n,r)},undelegate:function(e,t,n){return 1===arguments.length?this.off(e,"**"):this.off(t,e||"**",n)}});var On=it.now(),Dn=/\?/,Rn=/(,)|(\[|{)|(}|])|"(?:[^"\\\r\n]|\\["\\\/bfnrt]|\\u[\da-fA-F]{4})*"\s*:?|true|false|null|-?(?!0\d)\d+(?:\.\d+|)(?:[eE][+-]?\d+|)/g;it.parseJSON=function(t){if(e.JSON&&e.JSON.parse)return e.JSON.parse(t+"");var n,r=null,o=it.trim(t+"");return o&&!it.trim(o.replace(Rn,function(e,t,o,i){return n&&t&&(r=0),0===r?e:(n=o||t,r+=!i-!o,"")}))?Function("return "+o)():it.error("Invalid JSON: "+t)},it.parseXML=function(t){var n,r;if(!t||"string"!=typeof t)return null;try{e.DOMParser?(r=new DOMParser,n=r.parseFromString(t,"text/xml")):(n=new ActiveXObject("Microsoft.XMLDOM"),n.async="false",n.loadXML(t))}catch(o){n=void 0}return n&&n.documentElement&&!n.getElementsByTagName("parsererror").length||it.error("Invalid XML: "+t),n};var Mn,Hn,qn=/#.*$/,Fn=/([?&])_=[^&]*/,Pn=/^(.*?):[ \t]*([^\r\n]*)\r?$/gm,Bn=/^(?:about|app|app-storage|.+-extension|file|res|widget):$/,In=/^(?:GET|HEAD)$/,$n=/^\/\//,Wn=/^([\w.+-]+:)(?:\/\/(?:[^\/?#]*@|)([^\/?#:]*)(?::(\d+)|)|)/,zn={},Un={},Xn="*/".concat("*");try{Hn=location.href}catch(Jn){Hn=gt.createElement("a"),Hn.href="",Hn=Hn.href}Mn=Wn.exec(Hn.toLowerCase())||[],it.extend({active:0,lastModified:{},etag:{},ajaxSettings:{url:Hn,type:"GET",isLocal:Bn.test(Mn[1]),global:!0,processData:!0,async:!0,contentType:"application/x-www-form-urlencoded; charset=UTF-8",accepts:{"*":Xn,text:"text/plain",html:"text/html",xml:"application/xml, text/xml",json:"application/json, text/javascript"},contents:{xml:/xml/,html:/html/,json:/json/},responseFields:{xml:"responseXML",text:"responseText",json:"responseJSON"},converters:{"* text":String,"text html":!0,"text json":it.parseJSON,"text xml":it.parseXML},flatOptions:{url:!0,context:!0}},ajaxSetup:function(e,t){return t?I(I(e,it.ajaxSettings),t):I(it.ajaxSettings,e)},ajaxPrefilter:P(zn),ajaxTransport:P(Un),ajax:function(e,t){function n(e,t,n,r){var o,c,v,y,x,T=t;2!==b&&(b=2,s&&clearTimeout(s),l=void 0,a=r||"",w.readyState=e>0?4:0,o=e>=200&&300>e||304===e,n&&(y=$(f,w,n)),y=W(f,y,w,o),o?(f.ifModified&&(x=w.getResponseHeader("Last-Modified"),x&&(it.lastModified[i]=x),x=w.getResponseHeader("etag"),x&&(it.etag[i]=x)),204===e||"HEAD"===f.type?T="nocontent":304===e?T="notmodified":(T=y.state,c=y.data,v=y.error,o=!v)):(v=T,(e||!T)&&(T="error",0>e&&(e=0))),w.status=e,w.statusText=(t||T)+"",o?h.resolveWith(p,[c,T,w]):h.rejectWith(p,[w,T,v]),w.statusCode(m),m=void 0,u&&d.trigger(o?"ajaxSuccess":"ajaxError",[w,f,o?c:v]),g.fireWith(p,[w,T]),u&&(d.trigger("ajaxComplete",[w,f]),--it.active||it.event.trigger("ajaxStop")))}"object"==typeof e&&(t=e,e=void 0),t=t||{};var r,o,i,a,s,u,l,c,f=it.ajaxSetup({},t),p=f.context||f,d=f.context&&(p.nodeType||p.jquery)?it(p):it.event,h=it.Deferred(),g=it.Callbacks("once memory"),m=f.statusCode||{},v={},y={},b=0,x="canceled",w={readyState:0,getResponseHeader:function(e){var t;if(2===b){if(!c)for(c={};t=Pn.exec(a);)c[t[1].toLowerCase()]=t[2];t=c[e.toLowerCase()]}return null==t?null:t},getAllResponseHeaders:function(){return 2===b?a:null},setRequestHeader:function(e,t){var n=e.toLowerCase();return b||(e=y[n]=y[n]||e,v[e]=t),this},overrideMimeType:function(e){return b||(f.mimeType=e),this},statusCode:function(e){var t;if(e)if(2>b)for(t in e)m[t]=[m[t],e[t]];else w.always(e[w.status]);return this},abort:function(e){var t=e||x;return l&&l.abort(t),n(0,t),this}};if(h.promise(w).complete=g.add,w.success=w.done,w.error=w.fail,f.url=((e||f.url||Hn)+"").replace(qn,"").replace($n,Mn[1]+"//"),f.type=t.method||t.type||f.method||f.type,f.dataTypes=it.trim(f.dataType||"*").toLowerCase().match(xt)||[""],null==f.crossDomain&&(r=Wn.exec(f.url.toLowerCase()),f.crossDomain=!(!r||r[1]===Mn[1]&&r[2]===Mn[2]&&(r[3]||("http:"===r[1]?"80":"443"))===(Mn[3]||("http:"===Mn[1]?"80":"443")))),f.data&&f.processData&&"string"!=typeof f.data&&(f.data=it.param(f.data,f.traditional)),B(zn,f,t,w),2===b)return w;
u=f.global,u&&0===it.active++&&it.event.trigger("ajaxStart"),f.type=f.type.toUpperCase(),f.hasContent=!In.test(f.type),i=f.url,f.hasContent||(f.data&&(i=f.url+=(Dn.test(i)?"&":"?")+f.data,delete f.data),f.cache===!1&&(f.url=Fn.test(i)?i.replace(Fn,"$1_="+On++):i+(Dn.test(i)?"&":"?")+"_="+On++)),f.ifModified&&(it.lastModified[i]&&w.setRequestHeader("If-Modified-Since",it.lastModified[i]),it.etag[i]&&w.setRequestHeader("If-None-Match",it.etag[i])),(f.data&&f.hasContent&&f.contentType!==!1||t.contentType)&&w.setRequestHeader("Content-Type",f.contentType),w.setRequestHeader("Accept",f.dataTypes[0]&&f.accepts[f.dataTypes[0]]?f.accepts[f.dataTypes[0]]+("*"!==f.dataTypes[0]?", "+Xn+"; q=0.01":""):f.accepts["*"]);for(o in f.headers)w.setRequestHeader(o,f.headers[o]);if(f.beforeSend&&(f.beforeSend.call(p,w,f)===!1||2===b))return w.abort();x="abort";for(o in{success:1,error:1,complete:1})w[o](f[o]);if(l=B(Un,f,t,w)){w.readyState=1,u&&d.trigger("ajaxSend",[w,f]),f.async&&f.timeout>0&&(s=setTimeout(function(){w.abort("timeout")},f.timeout));try{b=1,l.send(v,n)}catch(T){if(!(2>b))throw T;n(-1,T)}}else n(-1,"No Transport");return w},getJSON:function(e,t,n){return it.get(e,t,n,"json")},getScript:function(e,t){return it.get(e,void 0,t,"script")}}),it.each(["get","post"],function(e,t){it[t]=function(e,n,r,o){return it.isFunction(n)&&(o=o||r,r=n,n=void 0),it.ajax({url:e,type:t,dataType:o,data:n,success:r})}}),it.each(["ajaxStart","ajaxStop","ajaxComplete","ajaxError","ajaxSuccess","ajaxSend"],function(e,t){it.fn[t]=function(e){return this.on(t,e)}}),it._evalUrl=function(e){return it.ajax({url:e,type:"GET",dataType:"script",async:!1,global:!1,"throws":!0})},it.fn.extend({wrapAll:function(e){if(it.isFunction(e))return this.each(function(t){it(this).wrapAll(e.call(this,t))});if(this[0]){var t=it(e,this[0].ownerDocument).eq(0).clone(!0);this[0].parentNode&&t.insertBefore(this[0]),t.map(function(){for(var e=this;e.firstChild&&1===e.firstChild.nodeType;)e=e.firstChild;return e}).append(this)}return this},wrapInner:function(e){return this.each(it.isFunction(e)?function(t){it(this).wrapInner(e.call(this,t))}:function(){var t=it(this),n=t.contents();n.length?n.wrapAll(e):t.append(e)})},wrap:function(e){var t=it.isFunction(e);return this.each(function(n){it(this).wrapAll(t?e.call(this,n):e)})},unwrap:function(){return this.parent().each(function(){it.nodeName(this,"body")||it(this).replaceWith(this.childNodes)}).end()}}),it.expr.filters.hidden=function(e){return e.offsetWidth<=0&&e.offsetHeight<=0||!rt.reliableHiddenOffsets()&&"none"===(e.style&&e.style.display||it.css(e,"display"))},it.expr.filters.visible=function(e){return!it.expr.filters.hidden(e)};var Vn=/%20/g,Gn=/\[\]$/,Qn=/\r?\n/g,Yn=/^(?:submit|button|image|reset|file)$/i,Kn=/^(?:input|select|textarea|keygen)/i;it.param=function(e,t){var n,r=[],o=function(e,t){t=it.isFunction(t)?t():null==t?"":t,r[r.length]=encodeURIComponent(e)+"="+encodeURIComponent(t)};if(void 0===t&&(t=it.ajaxSettings&&it.ajaxSettings.traditional),it.isArray(e)||e.jquery&&!it.isPlainObject(e))it.each(e,function(){o(this.name,this.value)});else for(n in e)z(n,e[n],t,o);return r.join("&").replace(Vn,"+")},it.fn.extend({serialize:function(){return it.param(this.serializeArray())},serializeArray:function(){return this.map(function(){var e=it.prop(this,"elements");return e?it.makeArray(e):this}).filter(function(){var e=this.type;return this.name&&!it(this).is(":disabled")&&Kn.test(this.nodeName)&&!Yn.test(e)&&(this.checked||!Lt.test(e))}).map(function(e,t){var n=it(this).val();return null==n?null:it.isArray(n)?it.map(n,function(e){return{name:t.name,value:e.replace(Qn,"\r\n")}}):{name:t.name,value:n.replace(Qn,"\r\n")}}).get()}}),it.ajaxSettings.xhr=void 0!==e.ActiveXObject?function(){return!this.isLocal&&/^(get|post|head|put|delete|options)$/i.test(this.type)&&U()||X()}:U;var Zn=0,er={},tr=it.ajaxSettings.xhr();e.ActiveXObject&&it(e).on("unload",function(){for(var e in er)er[e](void 0,!0)}),rt.cors=!!tr&&"withCredentials"in tr,tr=rt.ajax=!!tr,tr&&it.ajaxTransport(function(e){if(!e.crossDomain||rt.cors){var t;return{send:function(n,r){var o,i=e.xhr(),a=++Zn;if(i.open(e.type,e.url,e.async,e.username,e.password),e.xhrFields)for(o in e.xhrFields)i[o]=e.xhrFields[o];e.mimeType&&i.overrideMimeType&&i.overrideMimeType(e.mimeType),e.crossDomain||n["X-Requested-With"]||(n["X-Requested-With"]="XMLHttpRequest");for(o in n)void 0!==n[o]&&i.setRequestHeader(o,n[o]+"");i.send(e.hasContent&&e.data||null),t=function(n,o){var s,u,l;if(t&&(o||4===i.readyState))if(delete er[a],t=void 0,i.onreadystatechange=it.noop,o)4!==i.readyState&&i.abort();else{l={},s=i.status,"string"==typeof i.responseText&&(l.text=i.responseText);try{u=i.statusText}catch(c){u=""}s||!e.isLocal||e.crossDomain?1223===s&&(s=204):s=l.text?200:404}l&&r(s,u,l,i.getAllResponseHeaders())},e.async?4===i.readyState?setTimeout(t):i.onreadystatechange=er[a]=t:t()},abort:function(){t&&t(void 0,!0)}}}}),it.ajaxSetup({accepts:{script:"text/javascript, application/javascript, application/ecmascript, application/x-ecmascript"},contents:{script:/(?:java|ecma)script/},converters:{"text script":function(e){return it.globalEval(e),e}}}),it.ajaxPrefilter("script",function(e){void 0===e.cache&&(e.cache=!1),e.crossDomain&&(e.type="GET",e.global=!1)}),it.ajaxTransport("script",function(e){if(e.crossDomain){var t,n=gt.head||it("head")[0]||gt.documentElement;return{send:function(r,o){t=gt.createElement("script"),t.async=!0,e.scriptCharset&&(t.charset=e.scriptCharset),t.src=e.url,t.onload=t.onreadystatechange=function(e,n){(n||!t.readyState||/loaded|complete/.test(t.readyState))&&(t.onload=t.onreadystatechange=null,t.parentNode&&t.parentNode.removeChild(t),t=null,n||o(200,"success"))},n.insertBefore(t,n.firstChild)},abort:function(){t&&t.onload(void 0,!0)}}}});var nr=[],rr=/(=)\?(?=&|$)|\?\?/;it.ajaxSetup({jsonp:"callback",jsonpCallback:function(){var e=nr.pop()||it.expando+"_"+On++;return this[e]=!0,e}}),it.ajaxPrefilter("json jsonp",function(t,n,r){var o,i,a,s=t.jsonp!==!1&&(rr.test(t.url)?"url":"string"==typeof t.data&&!(t.contentType||"").indexOf("application/x-www-form-urlencoded")&&rr.test(t.data)&&"data");return s||"jsonp"===t.dataTypes[0]?(o=t.jsonpCallback=it.isFunction(t.jsonpCallback)?t.jsonpCallback():t.jsonpCallback,s?t[s]=t[s].replace(rr,"$1"+o):t.jsonp!==!1&&(t.url+=(Dn.test(t.url)?"&":"?")+t.jsonp+"="+o),t.converters["script json"]=function(){return a||it.error(o+" was not called"),a[0]},t.dataTypes[0]="json",i=e[o],e[o]=function(){a=arguments},r.always(function(){e[o]=i,t[o]&&(t.jsonpCallback=n.jsonpCallback,nr.push(o)),a&&it.isFunction(i)&&i(a[0]),a=i=void 0}),"script"):void 0}),it.parseHTML=function(e,t,n){if(!e||"string"!=typeof e)return null;"boolean"==typeof t&&(n=t,t=!1),t=t||gt;var r=pt.exec(e),o=!n&&[];return r?[t.createElement(r[1])]:(r=it.buildFragment([e],t,o),o&&o.length&&it(o).remove(),it.merge([],r.childNodes))};var or=it.fn.load;it.fn.load=function(e,t,n){if("string"!=typeof e&&or)return or.apply(this,arguments);var r,o,i,a=this,s=e.indexOf(" ");return s>=0&&(r=e.slice(s,e.length),e=e.slice(0,s)),it.isFunction(t)?(n=t,t=void 0):t&&"object"==typeof t&&(i="POST"),a.length>0&&it.ajax({url:e,type:i,dataType:"html",data:t}).done(function(e){o=arguments,a.html(r?it("<div>").append(it.parseHTML(e)).find(r):e)}).complete(n&&function(e,t){a.each(n,o||[e.responseText,t,e])}),this},it.expr.filters.animated=function(e){return it.grep(it.timers,function(t){return e===t.elem}).length};var ir=e.document.documentElement;it.offset={setOffset:function(e,t,n){var r,o,i,a,s,u,l,c=it.css(e,"position"),f=it(e),p={};"static"===c&&(e.style.position="relative"),s=f.offset(),i=it.css(e,"top"),u=it.css(e,"left"),l=("absolute"===c||"fixed"===c)&&it.inArray("auto",[i,u])>-1,l?(r=f.position(),a=r.top,o=r.left):(a=parseFloat(i)||0,o=parseFloat(u)||0),it.isFunction(t)&&(t=t.call(e,n,s)),null!=t.top&&(p.top=t.top-s.top+a),null!=t.left&&(p.left=t.left-s.left+o),"using"in t?t.using.call(e,p):f.css(p)}},it.fn.extend({offset:function(e){if(arguments.length)return void 0===e?this:this.each(function(t){it.offset.setOffset(this,e,t)});var t,n,r={top:0,left:0},o=this[0],i=o&&o.ownerDocument;if(i)return t=i.documentElement,it.contains(t,o)?(typeof o.getBoundingClientRect!==Et&&(r=o.getBoundingClientRect()),n=J(i),{top:r.top+(n.pageYOffset||t.scrollTop)-(t.clientTop||0),left:r.left+(n.pageXOffset||t.scrollLeft)-(t.clientLeft||0)}):r},position:function(){if(this[0]){var e,t,n={top:0,left:0},r=this[0];return"fixed"===it.css(r,"position")?t=r.getBoundingClientRect():(e=this.offsetParent(),t=this.offset(),it.nodeName(e[0],"html")||(n=e.offset()),n.top+=it.css(e[0],"borderTopWidth",!0),n.left+=it.css(e[0],"borderLeftWidth",!0)),{top:t.top-n.top-it.css(r,"marginTop",!0),left:t.left-n.left-it.css(r,"marginLeft",!0)}}},offsetParent:function(){return this.map(function(){for(var e=this.offsetParent||ir;e&&!it.nodeName(e,"html")&&"static"===it.css(e,"position");)e=e.offsetParent;return e||ir})}}),it.each({scrollLeft:"pageXOffset",scrollTop:"pageYOffset"},function(e,t){var n=/Y/.test(t);it.fn[e]=function(r){return At(this,function(e,r,o){var i=J(e);return void 0===o?i?t in i?i[t]:i.document.documentElement[r]:e[r]:void(i?i.scrollTo(n?it(i).scrollLeft():o,n?o:it(i).scrollTop()):e[r]=o)},e,r,arguments.length,null)}}),it.each(["top","left"],function(e,t){it.cssHooks[t]=_(rt.pixelPosition,function(e,n){return n?(n=nn(e,t),on.test(n)?it(e).position()[t]+"px":n):void 0})}),it.each({Height:"height",Width:"width"},function(e,t){it.each({padding:"inner"+e,content:t,"":"outer"+e},function(n,r){it.fn[r]=function(r,o){var i=arguments.length&&(n||"boolean"!=typeof r),a=n||(r===!0||o===!0?"margin":"border");return At(this,function(t,n,r){var o;return it.isWindow(t)?t.document.documentElement["client"+e]:9===t.nodeType?(o=t.documentElement,Math.max(t.body["scroll"+e],o["scroll"+e],t.body["offset"+e],o["offset"+e],o["client"+e])):void 0===r?it.css(t,n,a):it.style(t,n,r,a)},t,i?r:void 0,i,null)}})}),it.fn.size=function(){return this.length},it.fn.andSelf=it.fn.addBack,"function"==typeof define&&define.amd&&define("jquery",[],function(){return it});var ar=e.jQuery,sr=e.$;return it.noConflict=function(t){return e.$===it&&(e.$=sr),t&&e.jQuery===it&&(e.jQuery=ar),it},typeof t===Et&&(e.jQuery=e.$=it),it}),define("bower_components/jquery/dist/jquery",function(){});
        
        /*
 * Copyright (C) 2005 - 2015 TIBCO Software Inc. All rights reserved.
 * http://www.jaspersoft.com.
 * Licensed under commercial Jaspersoft Subscription License Agreement
 */

/**
 * @author: Igor Nesterenko, Zakhar Tomchenko
 */

/* global _, jasper */

;(function(root, _, jasper) {
    var version = "0.0.1a",
        visualizeData = {
            bis: {},
            auths: [],
            facts: {},
            config: {}
        };

    visualizeData.auths.find = function(server, auth) {
        return _.find(this, _.compose(_.partial(_.isEqual, _.extend({url: server}, auth)), function(auth) {
            return auth.properties();
        }));
    };

    var visualize = function (param, param2, param3, param4) {
        var properties, bi, callback, errback, always,
            dependencies = ["BiComponentFactory", "common/auth/Authentication", "jquery"];

        if (typeof param == 'function') {
            properties = visualizeData.config;
            callback = param;
            errback = param2;
            always = param3;
        } else {
            properties = _.extend({}, visualizeData.config, param);
            callback = param2;
            errback = param3;
            always = param4;
        }

        bi = visualizeData.bis[properties.server];
        if (!bi) {
            bi = jasper({
                url : properties.server,
                scripts: properties.scripts,
                logEnabled: properties.logEnabled,
                logLevel: properties.logLevel
            });
            visualizeData.bis[properties.server] = bi;
        }

        // this is temporary solution to show input controls inside dashboard
        // will be removed in next release
        if (properties._showInputControls) {
            dependencies = dependencies.concat([
                "csslink!" + properties.server + "/themes/reset.css",
                "css!theme.css",
                "css!buttons.css",
                "css!lists.css",
                "css!controls.css",
                "css!pageSpecific.css",
                "csslink!jquery-ui-custom-css-visualizejs"
            ]);
        }

        bi(dependencies, function(BiComponentFactory, Authentication, $) {
            var auth = visualizeData.auths.find(properties.server, properties.auth),
                factory = visualizeData.facts[properties.server];

            if (!properties.auth) {
                properties.auth = {
                    loginFn: function() { return $.Deferred().resolve() }
                };
            }

            if (!auth) {
                auth = new Authentication(_.extend({url: properties.server}, properties.auth));
                visualizeData.auths.push(auth);
            }

            if (!factory) {
                factory = new BiComponentFactory({
                    server: properties.server,
                    _showInputControls: properties._showInputControls
                });
                visualizeData.facts[properties.server] = factory;
            }

            auth._result || (auth._result = auth.run());

            auth._result
                .fail(errback)
                .always(function(result) {
                    if (auth._result.state() === "resolved") {
                        var v = createV(factory, auth);
                        callback && callback(v);
                        always && always(null, v);
                    } else {
                        always && always(result);
                    }
                });

        });
    };

    visualize.version = version;
    visualize.config = function(config) {
        _.extend(visualizeData.config, config);
    };

    function createV(factory, auth){
        var v = function (param) {
            if (typeof param == 'string' || Object.prototype.toString.call(param) === "[object String]") {
                // v("#container").report({...});
                return {
                    report: (function(selector) {
                        return function(options) {
                            factory.report(_.extend({container: selector}, options));
                        }
                    })(param),
                    dashboard: (function(selector) {
                        return function(options) {
                            factory.dashboard(_.extend({container: selector}, options));
                        }
                    })(param)
                }
            }
        };
        v.logout = auth.logout;
        v.login = auth.login;
        _.extend(v, factory);
        return v;
    }

    // noConflict functionality
    var _visualize = root.visualize;

    visualize.noConflict = function () {
        if (root.visualize === visualize) {
            root.visualize = _visualize;
        }

        return visualize;
    };

    // Add visualize to global scope
    root.visualize = visualize;

})(this, _, jasper);







    


visualize.config({
    server : "http://10.10.110.122:8080/jasperserver-pro",
    scripts : "scripts",
    logEnabled: true,
    logLevel: "error",
    _showInputControls: "false" === "true"
});
