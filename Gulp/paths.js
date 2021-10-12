const assetsBasePath = './Assets/';
const stylesBasePath = assetsBasePath + 'Styles/';
const nodeModulesBasePath = './node_modules/';
const distBasePath = './wwwroot/';

module.exports = {
    vendorAssets: [
        {
            name: 'jsoneditor',
            path: nodeModulesBasePath + 'jsoneditor/dist/**',
        },
    styles: {
        base: stylesBasePath,
        all: stylesBasePath + '**/*.scss',
        },
    ],
    dist: {
        vendors: distBasePath + 'vendors/',
        css: distBasePath + 'css/',
    },
};
