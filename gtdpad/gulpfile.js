/// <binding ProjectOpened='watch' />
'use strict';

const gulp = require('gulp');

const sassCompiler = require('sass');
const sass = require('gulp-sass')(sassCompiler);

const config = {
    sass: {
        root: './static/css',
        sources: './static/css/**/*.scss',
        options: {
            precision: 10,
            outputStyle: 'expanded'
        }
    }
};

gulp.task('sass', () => gulp.src(config.sass.sources)
    .pipe(sass(config.sass.options).on('error', sass.logError))
    .pipe(gulp.dest(config.sass.root)));

gulp.task('watch', () => gulp.watch(config.sass.sources, gulp.series('sass')));
