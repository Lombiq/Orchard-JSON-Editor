# Browsersync



This helper enables you to see client-side changes in real-time using [Browsersync](https://browsersync.io). It works by spinning up a local server that watches files (according to the configuration) for changes to be able reload/inject static resources with or without reloading the page for every active browser session. Find more information in the [official documentation](https://browsersync.io/docs/options).

Usage:
```js
const cssFilesPath = '**/*.css';
const jsFilesPath = '**/*.js';

const browsersyncOptions = {    
    files: [
        cssFilesPath,
        jsFilesPath
    ]
}

gulp.task('browsersyncStart', () => browsersync.browsersyncServe(browsersyncOptions));
```

Using [proxy mode](https://browsersync.io/docs/options#option-proxy) requires an existing vhost added in the options, which Browsersync will create a proxy for.

If you are not using the proxy mode, the terminal will ask you to add a snippet just before the closing </body> tag, for example:
```html
<script id="__bs_script__">//<![CDATA[
    document.write("<script async src='http://HOST:3000/browser-sync/browser-sync-client.js'><\/script>".replace("HOST", location.hostname));
//]]></script>
```
