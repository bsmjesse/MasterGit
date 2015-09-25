module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> */\n'
            },
            build: {
                src: 'src/<%= pkg.name %>.js',
                dest: 'build/<%= pkg.name %>.min.js'
            }
        },

        wiredep: {

            task: {

                // Point to the files that should be updated when
                // you run `grunt wiredep`
                src: [
                  'Default.aspx',   // .html support...
                ],

                options: {
                    // See wiredep's configuration documentation for the options
                    // you may pass:

                    // https://github.com/taptapship/wiredep#configuration
                }
            }
        },

        requirejs: {
            compile: {
                options: {
                    baseUrl: "js",
                    mainConfigFile: "js/config.js",
                    out: "js/optimized.js",
                    modules: [
                        {
                            name: '../common',
                            //List common dependencies here. Only need to list
                            //top level dependencies, "include" will find
                            //nested dependencies.
                            include: [
                              'jquery',
                              'js/controls',
                            ]
                        }
                    ]
                },
            }
        }
    });

    // Load the plugin that provides the "uglify" task.
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-wiredep');

    grunt.registerTask('uglify', ['uglify']);
    grunt.registerTask('requirejs', ['require']);

    // Default task(s).
    grunt.registerTask('default', ['wiredep']);

};