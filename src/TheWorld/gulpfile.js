/// <binding BeforeBuild='minify' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
gulp-ng-annotate is needed for minification ($http,$routeParams are not properly minified without this package 
*/

var gulp = require('gulp');
var uglify = require("gulp-uglify");
var ngAnnotate = require("gulp-ng-annotate");

gulp.task('minify', function () {
    return gulp.src("wwwroot/js/*.js")
    .pipe(ngAnnotate())
    .pipe(uglify())
    .pipe(gulp.dest("wwwroot/lib/_app"));
    // place code for your default task here
});