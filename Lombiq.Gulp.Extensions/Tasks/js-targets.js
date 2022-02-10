const fs = require('fs');
const gulp = require('gulp');
const del = require('del');
const eslint = require('gulp-eslint');

/**
 * This callback type is called `pipelineModifier` and is displayed as a global symbol.
 *
 * @callback pipelineModifier
 * @param {NodeJS.ReadWriteStream} pipeline The files pipeline.
 * @returns {NodeJS.ReadWriteStream} The files pipeline.
 */

/**
 * This method processes all JavaScript files in the `source` folder incl. all subfolders, and saves the resulting
 * files in the `destination` folder.
 *
 * @param {string | NodeJS.ReadWriteStream} source A source file or folder path, or a `gulp.src()` object.
 * @param {string} destination The destination folder path; if null, `source` will be used.
 * @param {pipelineModifier} pipelineModifier A callback to be executed on the source files before saving them to disk.
 * @param {object} options ESLint CLI modifier options.
 * @returns {NodeJS.ReadWriteStream} The files pipeline.
 */
function compile(source, destination, pipelineModifier, options) {
    if (typeof source !== 'string' && !destination) {
        throw new Error('Please provide a "destination" folder for the processed files!');
    }

    const copyDestination = destination || source;
    let sourceStream = source;

    if (typeof source === 'string') {
        sourceStream = fs.statSync(source).isDirectory()
            // If `source` is a directory, include all .js files in all subfolders.
            ? gulp.src(source + '**/*.js')
            // `source` represents a single file.
            : gulp.src(source);
    }

    let pipeline = sourceStream
        .pipe(eslint(options))
        .pipe(eslint.format('tap'));

    if (typeof pipelineModifier === 'function') {
        pipeline = pipelineModifier(pipeline);
    }

    return pipeline.pipe(gulp.dest(copyDestination));
}

function clean(destination) {
    return () => del(destination + '**/*.js');
}

module.exports = { compile, clean };
