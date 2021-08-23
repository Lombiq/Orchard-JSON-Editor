const gulp = require('gulp');
const paths = require('./Gulp/paths');
const scssTargets = require('../../Utilities/Lombiq.Gulp.Extensions/Tasks/scss-targets');
const copyAssets = require('./Gulp/tasks/copy-assets');

gulp.task('build:styles', scssTargets.build(paths.styles.base, paths.dist.css));
gulp.task('copy:vendor-assets', () => copyAssets(paths.vendorAssets, paths.dist.vendors));
gulp.task('default', gulp.parallel('build', 'copy:vendor-assets'));
