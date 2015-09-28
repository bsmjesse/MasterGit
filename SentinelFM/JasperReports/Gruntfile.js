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

        // !! This is the name of the task ('requirejs')
        requirejs: {
            compile:
            {

                // !! You can drop your app.build.js config wholesale into 'options'
                options: {
                    appDir: "src/",
                    baseUrl: ".",
                    dir: "target/",
                    optimize: 'uglify',
                    mainConfigFile: './src/main.js',
                    modules: [
                      {
                          name: 'MyModule'
                      }
                    ],
                    logLevel: 0,
                    findNestedDependencies: true,
                    fileExclusionRegExp: /^\./,
                    inlineText: true
                }
            }
        }
    });

    // Load the plugin that provides the "uglify" task.
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-wiredep');
    grunt.loadNpmTasks('grunt-contrib-requirejs');

    grunt.registerTask('uglify', ['uglify']);
    grunt.registerTask('requirejs', ['require']);

    // Default task(s).
    grunt.registerTask('default', ['wiredep']);

    require('load-grunt-tasks')(grunt); // npm install --save-dev load-grunt-tasks

    grunt.initConfig({
        babel: {

            options: {
                sourceMap: true
            },
            dist: {
                files: {
                    'dist/app.js': 'src/app.js'
                }
            }
        }
    });

    grunt.registerTask('babel', ['babel']);

};
