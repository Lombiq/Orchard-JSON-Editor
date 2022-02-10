# JS targets (ESLint) Gulp task



This helper makes it possible to copy one or multiple javascript files to a destination folder, after applying linting (with [ESLint](https://eslint.org/)) on it.

Looking for something similar for .NET? Check out our [.NET Analyzers project](https://github.com/Lombiq/.NET-Analyzers).

You can use ESLint as follows:

1. Copy *example.eslintrc* from the *ESLint* folder of this project to the root folder of your solution (i.e. where you have the sln file), rename it to *.eslintrc*, and specify *lombiq-base.js*'s location inside.
2. Import the *Tasks/js-targets.js* file in your Gulpfile then create a Gulp task that uses this helper as a pipeline.
3. If you use [Visual Studio's built-in ESLint](https://docs.microsoft.com/en-us/visualstudio/ide/reference/options-text-editor-javascript-linting?view=vs-2019), it will recognize the rules and show any violations after the copying of *.eslintrc* as mentioned above. Note that you have to enable the ESLint integration for it to work in the editor. The *vs-eslint-package.json* file is automatically copied into your solution directory as *package.json* to make this work; gitignore it in your repository along the lines of:

    ```
    /src/package.json
    ```


## Configuring the Gulp task

The input parameters are `string`s of the source and destination folders containing scripts that need to be analyzed and copied.

Usage:
```js
const jsTargets = require('path/to/Lombiq.Gulp.Extensions/Tasks/js-targets');

const source = './Assets/Scripts/'
const destination = './wwwroot/js'

gulp.task('build:scripts', () => jsTargets.compile(source, destination));

// Or you can pass a function to modify the pipeline before saving the files and e.g. run Babel:
gulp.task(
    'build:scripts',
    () => jsTargets.compile(source, destination, (pipeline) => pipeline.pipe(babel({ presets: ['@babel/preset-env'] }))));

// You can also pass additional options to ESLint.
// Here's an example for fixing automatically fixable rule violations in-place:
gulp.task(
    'build:scripts--fix',
    () => jsTargets.compile(scriptsBasePath, scriptsBasePath, null, { fix: true }));
```

Read more about `options` in the CLI [documentation](https://eslint.org/docs/developer-guide/nodejs-api#cliengine).


## ESlint rules

The rules are found in 2 files:
- *lombiq-base.js*: These rules are Lombiq overrides for the extended [Airbnb rules](https://github.com/airbnb/javascript/tree/master/packages/eslint-config-airbnb-base/rules).
- *.eslintrc*: In this file you can define your own overriding rules.

Details on rules can be found in the [ESLint documentation](https://eslint.org/docs/rules/).

The MSBuild output or the Gulp task runner will show you all of the ESLint rule violations in a detailed manner.

If a certain rule's violation is incorrect in a given location, or you want to suppress it locally, [you can disable them](https://eslint.org/docs/2.13.1/user-guide/configuring#disabling-rules-with-inline-comments). Just always comment such disables so it's apparent why it was necessary.

The Lombiq rules enforce CRLF line endings in JS files. To ensure that during Git checkout the files have such line endings, you can add the followig _.gitattributes_ file:

```
*.js text eol=crlf
```
