# Copy assets Gulp task



This helper makes it possible to copy one or multiple assets to a destination folder. 

Import the *Tasks/copy-assets.js* file in your Gulpfile then create a Gulp task that uses this helper as a pipeline.

The first input parameter is an array of objects where it is possible to specify the source and destination of each assets. Each object should have a `name` property which will be the name of the subfolder created in the destination, and a `path` property which defines one or more files that need to be copied.

Usage:

```js
const copyAssets = require('path/to/Lombiq.Gulp.Extensions/Tasks/copy-assets');

// Note how "name" can contain subfolders too!
const assets = [
    {
        name: 'vendors/jquery-validation',
        path: './node_modules/jquery-validation/dist/*',
    },
    {
        name: 'images',
        path: './Assets/images/**/*',
    },
];

gulp.task('copy:assets', () => copyAssets.copy(assets, './wwwroot/'));
gulp.task('clean:assets', () => copyAssets.clean('./wwwroot/'));
```
