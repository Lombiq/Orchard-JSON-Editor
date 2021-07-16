const nodeModulesBasePath = './node_modules/';
const distBasePath = './wwwroot/';
const lombiqBasePath = './Assets/Scripts/';

module.exports = {
    vendorAssets: [
        {
            name: 'jsoneditor',
            path: nodeModulesBasePath + 'jsoneditor/dist/**',
        }
    ],
    lombiqAssets: {
        base: lombiqBasePath,
        all: lombiqBasePath + '**/*.js',
    },
    dist: {
        vendors: distBasePath + 'vendors/',
        lombiq: distBasePath + 'lombiq/',
    },
};
