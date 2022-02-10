const browsersync = require('browser-sync').create('Browsersync Server');

async function start(options) {
    const defaultOptions = {
        open: false,
        online: false,
    };

    // Merge object properties with the spread operator.
    // In the case of a key collision, the right-most (last) object's value wins out.
    browsersync.init({ ...defaultOptions, ...options });

    browsersync.watch(options.files, (event, file) => {
        if (event === 'change') {
            browsersync.notify(`${file} has been changed.`, 2000);
        }
    });
}

module.exports = { start };
