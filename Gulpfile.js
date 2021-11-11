const gulp = require('gulp');
const recommendedSetup = require('../../Utilities/Lombiq.Gulp.Extensions/recommended-setup');

const assets = [
    {
        name: 'jsoneditor',
        path: './node_modules/jsoneditor/dist/**',
    },
];

recommendedSetup
    .setupVendorsCopyAssets(assets)
    .setupRecommendedScssTasks();

gulp.task('default', gulp.parallel('build:styles', 'copy:vendor-assets'));
gulp.task('clean', gulp.parallel('clean:styles', 'clean:vendor-assets'));
