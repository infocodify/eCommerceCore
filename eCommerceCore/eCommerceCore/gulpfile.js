//const gulp = require('gulp');
//const imagemin = require('gulp-image-optimization');

//gulp.task('ImageOptimize', () =>
//    gulp.src('wwwroot/images/ProductImage*')
//        .pipe(imagemin())
//        .pipe(gulp.dest('wwwroot/images/ProductImage-min'))
//);
/// <binding />
var gulp = require('gulp');
var uglify = require("gulp-uglify");

var imageOptimize = require('gulp-image-optimization');
gulp.task('optimizeImageJPG', function () {
    return gulp.src('wwwroot/images/*.jpg')
        .pipe(imageOptimize())
        .pipe(gulp.dest("wwwroot/images/"));
});
gulp.task('optimizeImagePNG', function () {
    return gulp.src('wwwroot/images/ProductImage/*.png')
        .pipe(imageOptimize())
        .pipe(gulp.dest("wwwroot/images/ProductImage/"));
});
gulp.task('optimizeImageIcons', function () {
    return gulp.src('wwwroot/icons/images/*')
        .pipe(imageOptimize())
        .pipe(gulp.dest("wwwroot/icons/"));
});