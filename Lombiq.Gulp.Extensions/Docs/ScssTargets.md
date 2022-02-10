# SCSS targets Gulp task



This helper lints (with [Stylelint](https://stylelint.io/)), compiles and minifies the given scss files and copies the output to the given folder.

Looking for something similar for .NET? Check out our [.NET Analyzers project](https://github.com/Lombiq/.NET-Analyzers).

You can use it as follows:

1. Copy *example.stylelintrc* from the *ESLint* folder of this project to the root folder of your solution (i.e. where you have the sln file), rename it to *.stylelintrc*, and specify *lombiq-base.stylelintrc*'s location inside.
2. Import the *Tasks/scss-targets.js* file in your Gulpfile.
3. Create a Gulp task that uses this helper as a pipeline, see below.

```js
const scssTargets = require('path/to/Lombiq.Gulp.Extensions/Tasks/scss-targets');

gulp.task('build:styles', scssTargets.build('./Assets/Styles/', './wwwroot/css/'));
```

You'll see any errors or linter rule violations show up in the Gulp console.

Unfortunately, there's no Visual Studio editor support to see linter violations in real-time. You can, however, write SCSS in Visual Studio Code and use the official [Stylelint extension](https://marketplace.visualstudio.com/items?itemName=stylelint.vscode-stylelint): Just install it and configure it to validate SCSS files too under its "Stylelint: Validate" option or use the below snippet in VS Code's *settings.json*.

```json
"stylelint.validate": [
    "css",
    "less",
    "postcss",
    "scss"
],
```


## Stylelint rules

The rules are found in 2 files:
- *lombiq-base.stylelintrc.json*: These rules are Lombiq overrides for [stylelint-config-standard-scss](https://www.npmjs.com/package/stylelint-config-standard-scss).
- *.stylelintrc*: In this file you can define your own overriding rules.

Details on rules can be found in the [Stylelint documentation](https://stylelint.io/user-guide/rules/list). If you want to find out what the currently applied configuration is, coming from all the various extended configuration files, then run `npx stylelint --print-config . > rules.json` at the given location.

The MSBuild output or the Gulp task runner will show you all of the Stylelint rule violations in a detailed manner.

If a certain rule's violation is incorrect in a given location, or you want to suppress it locally, [you can ignore the affected code](https://stylelint.io/user-guide/ignore-code/). Just always comment such ignores so it's apparent why it was necessary.
