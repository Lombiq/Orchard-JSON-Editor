const nodeModulesBasePath = './node_modules/';
const distBasePath = './wwwroot/';

module.exports = {
    vendorAssets: [
        {
            name: 'jsoneditor',
            path: nodeModulesBasePath + 'jsoneditor/dist/**',
        },
    ],
    dist: {
        vendors: distBasePath + 'vendors/',
        lombiq: distBasePath + 'lombiq/',
    },
};
