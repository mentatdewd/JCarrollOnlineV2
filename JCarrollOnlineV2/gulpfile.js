/// <binding BeforeBuild='clean, less' Clean='clean' />
"use strict";

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  uglify = require("gulp-uglify"),
  fs = require("fs"),
  less = require("gulp-less");

var path = require('path');
var plumber = require('gulp-plumber');

var paths = {
  webroot: "./Content/"
};

var itemsToCopy = {
    './node_modules/angular/angular*.js': paths.webroot + "./Scripts/Angular"
}

paths.js = "./Scripts/**/*.js";
paths.minJs = "./Scripts/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.less = "./**/*.less";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
  rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
  rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
  return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
    .pipe(concat(paths.concatJsDest))
    .pipe(uglify())
    .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
  return gulp.src([paths.css, "!" + paths.minCss])
    .pipe(concat(paths.concatCssDest))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);

//gulp.task('less', function () {
//    return gulp.src('./Content/less/**/*.less')
//        .pipe(plumber())
//        .pipe(less({
//            paths: [path.join(__dirname, 'less', 'includes')]
//        }))
//        .pipe(gulp.dest('./content/css'));
//});

// Compiles LESS > CSS 
gulp.task('build-less', function () {
    return gulp.src('./Content/less/*.less')
        .pipe(less())
        .pipe(gulp.dest('./content/css'));
});

gulp.task('copy', function () {
    for (var src in itemsToCopy) {
        if (!itemsToCopy.hasOwnProperty(src)) continue;
        gulp.src(src)
            .pipe(gulp.dest(itemsToCopy[src]));
    }
});