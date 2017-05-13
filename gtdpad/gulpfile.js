/// <binding ProjectOpened='watch' />
'use strict';

// Load gulp and the modules we need
var gulp = require('gulp'),
    sass = require('gulp-sass');

// Set some configuration
var config = {
    sass: {
        root: './static/css',
        sources: './static/css/**/*.scss',
        options: {
            precision: 10,
            outputStyle: 'expanded'
        }
    }
};

// Task to compile Sass
gulp.task('sass', function () {
    gulp.src(config.sass.sources)
        .pipe(sass(config.sass.options).on('error', sass.logError))
        .pipe(gulp.dest(config.sass.root));
});

// This task watches all the source files for changes
gulp.task('watch', function () {
    gulp.watch(config.sass.sources, ['sass']);
});