const gulp = require('gulp');
const watch = require('gulp-watch');
const babel = require('gulp-babel');
const scssTargets = require('./Tasks/scss-targets');
const jsTargets = require('./Tasks/js-targets');
const copyAssets = require('./Tasks/copy-assets');

const defaultAssetsBasePath = './Assets/';
const defaultDistBasePath = './wwwroot/';
const defaultStylesAssetsBasePath = defaultAssetsBasePath + 'Styles/';
const defaultScriptsAssetsBasePath = defaultAssetsBasePath + 'Scripts/';

function setupRecommendedScssTasks(
    stylesDistBasePath = defaultDistBasePath + 'css/',
    stylesAssetsBasePath = defaultStylesAssetsBasePath) {
    gulp.task('build:styles', scssTargets.build(stylesAssetsBasePath, stylesDistBasePath));
    gulp.task('clean:styles', scssTargets.clean(stylesDistBasePath));
    gulp.task('watch:styles', () => watch(stylesAssetsBasePath + '**/*.scss', { verbose: true }, gulp.series('build:styles')));
    gulp.task('default', gulp.series('build:styles'));
    gulp.task('clean', gulp.series('clean:styles'));

    return this;
}

function setupRecommendedJsTasks(
    scriptsDistBasePath = defaultDistBasePath + 'js/',
    scriptsAssetsBasePath = defaultScriptsAssetsBasePath) {
    gulp.task(
        'build:scripts',
        () => jsTargets.compile(
            scriptsAssetsBasePath, scriptsDistBasePath, (pipeline) => pipeline.pipe(babel({ presets: ['@babel/preset-env'] }))));

    gulp.task('clean:scripts', jsTargets.clean(scriptsDistBasePath));
    gulp.task('watch:scripts', () => watch(scriptsAssetsBasePath + '**/*.js', { verbose: true }, gulp.series('build:scripts')));
    gulp.task('default', gulp.series('build:scripts'));
    gulp.task('clean', gulp.series('clean:scripts'));

    return this;
}

function setupRecommendedScssAndJsTasks(
    stylesDistBasePath,
    scriptsDistBasePath,
    stylesAssetsBasePath,
    scriptsAssetsBasePath) {
    setupRecommendedScssTasks(stylesDistBasePath, stylesAssetsBasePath);
    setupRecommendedJsTasks(scriptsDistBasePath, scriptsAssetsBasePath);

    gulp.task('clean', gulp.parallel('clean:styles', 'clean:scripts'));
    gulp.task('watch', gulp.parallel('watch:styles', 'watch:scripts'));
    gulp.task('default', gulp.parallel('build:styles', 'build:scripts'));

    return this;
}

function setupVendorsCopyAssets(assets, assetsDistBasePath = defaultDistBasePath + 'vendors') {
    gulp.task('copy:vendor-assets', () => copyAssets.copy(assets, assetsDistBasePath));
    gulp.task('clean:vendor-assets', () => copyAssets.clean(assetsDistBasePath));

    gulp.task('default', gulp.series('copy:vendor-assets'));
    gulp.task('clean', gulp.series('clean:vendor-assets'));

    return this;
}

function setupRecommendedScssAndJsTasksAndVendorsCopyAssets(
    assets,
    stylesDistBasePath,
    scriptsDistBasePath,
    assetsDistBasePath,
    stylesAssetsBasePath,
    scriptsAssetsBasePath) {
    setupRecommendedScssTasks(stylesDistBasePath, stylesAssetsBasePath);
    setupRecommendedJsTasks(scriptsDistBasePath, scriptsAssetsBasePath);
    setupVendorsCopyAssets(assets, assetsDistBasePath);

    gulp.task('default', gulp.parallel('build:styles', 'build:scripts', 'copy:vendor-assets'));
    gulp.task('clean', gulp.parallel('clean:styles', 'clean:scripts', 'clean:vendor-assets'));

    return this;
}

function setupCopyAssets(assets, assetsDistBasePath = defaultDistBasePath) {
    const paths = assets.map((asset) => assetsDistBasePath + asset.name);

    gulp.task('copy:assets', () => copyAssets.copy(assets, assetsDistBasePath));
    gulp.task(
        'clean:assets',
        async () => {
            for (let i = 0; i < paths.length; i++) {
                copyAssets.clean(paths[i]);
            }
        });

    gulp.task('default', gulp.series('copy:assets'));
    gulp.task('clean', gulp.series('clean:assets'));

    return this;
}

function setupRecommendedScssAndJsTasksAndCopyAssets(
    assets,
    stylesDistBasePath,
    scriptsDistBasePath,
    stylesAssetsBasePath,
    scriptsAssetsBasePath) {
    setupRecommendedScssTasks(stylesDistBasePath, stylesAssetsBasePath);
    setupRecommendedJsTasks(scriptsDistBasePath, scriptsAssetsBasePath);
    setupCopyAssets(assets);

    gulp.task('default', gulp.parallel('build:styles', 'build:scripts', 'copy:assets'));
    gulp.task('clean', gulp.parallel('clean:styles', 'clean:scripts', 'clean:assets'));

    return this;
}

module.exports = {
    setupRecommendedScssTasks,
    setupRecommendedJsTasks,
    setupRecommendedScssAndJsTasks,
    setupVendorsCopyAssets,
    setupRecommendedScssAndJsTasksAndVendorsCopyAssets,
    setupCopyAssets,
    setupRecommendedScssAndJsTasksAndCopyAssets,
};
