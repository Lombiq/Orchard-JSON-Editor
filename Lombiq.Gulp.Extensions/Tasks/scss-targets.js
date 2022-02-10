const gulp = require('gulp');
const cache = require('gulp-cached');
const plumber = require('gulp-plumber');
const sass = require('gulp-dart-sass');
const rename = require('gulp-rename');
const cleanCss = require('gulp-clean-css');
const postcss = require('gulp-postcss');
const sourcemaps = require('gulp-sourcemaps');
const autoprefixer = require('autoprefixer');
const del = require('del');
const stylelint = require('gulp-stylelint');

const defaultCompatibleBrowsers = [
    'last 1 Chrome version',
    'last 1 Edge version',
    'last 1 Firefox version',
    'last 1 IE version',
    'last 1 iOS version'];

function compile(source, destination, compatibleBrowsers) {
    const compileDestination = destination || source;
    const compileCompatibleBrowsers = compatibleBrowsers || defaultCompatibleBrowsers;

    return gulp.src(source + '**/*.scss')
        .pipe(cache('scss'))
        .pipe(plumber())
        .pipe(stylelint({
            reporters: [
                { formatter: 'verbose', console: true },
            ],
        }))
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(sass({ linefeed: 'crlf' }).on('error', sass.logError))
        .pipe(postcss([autoprefixer({ overrideBrowserslist: compileCompatibleBrowsers })]))
        .pipe(sourcemaps.write('.', { includeContent: true }))
        .pipe(gulp.dest(compileDestination));
}

function minify(destination) {
    return gulp.src([destination + '**/*.css', '!' + destination + '**/*.min.css'])
        .pipe(cache('css'))
        .pipe(cleanCss({ compatibility: 'ie8' }))
        .pipe(rename({ extname: '.min.css' }))
        .pipe(gulp.dest(destination));
}

function clean(destination) {
    return () => del([destination + '**/*.css', destination + '**/*.css.map']);
}

function build(source, destination, compatibleBrowsers) {
    const buildDestination = destination || source;
    const buildCompatibleBrowsers = compatibleBrowsers || defaultCompatibleBrowsers;

    return gulp.series(
        () => new Promise((resolve) => compile(source, buildDestination, buildCompatibleBrowsers)
            .on('end', resolve)),
        () => new Promise((resolve) => minify(buildDestination).on('end', resolve)));
}

module.exports = {
    build, compile, minify, clean,
};
