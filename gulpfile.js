var gulp = require('gulp'),
    sass = require('gulp-sass'),
    cleanCSS = require('gulp-clean-css'),
    gp_concat = require('gulp-concat'),
    gp_rename = require('gulp-rename'),
    gp_uglify = require('gulp-uglify');

gulp.task('minjs', function(){
    return gulp.src('nuwns_libraries/*.js')
        .pipe(gp_concat('concat.js'))
        .pipe(gulp.dest('dist'))
        .pipe(gp_rename('main.min.js'))
        .pipe(gp_uglify())
        .pipe(gulp.dest('dist/nuwn'));
});

gulp.task('sass', function () {
  return gulp.src('./scss/*.scss')
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest('./css'));
});

gulp.task('mincss', function() {
  return gulp.src('./css/*.css')
    .pipe(cleanCSS({compatibility: '*'}))
    .pipe(gulp.dest('./css'));
});

gulp.task('watch_c#', function(){
    gulp.watch('../EAJCTP/Assets/Nuwn/*', ['C#']); 
})
gulp.task('C#', function () {
  return gulp.src('../EAJCTP/Assets/Nuwn/*')
    .pipe(gulp.dest('./Unity/Nuwn'));
});
