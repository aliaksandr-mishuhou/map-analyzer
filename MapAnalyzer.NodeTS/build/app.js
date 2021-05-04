"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var express = require("express");
var swagger_jsdoc_1 = __importDefault(require("swagger-jsdoc"));
var swaggerUi = require('swagger-ui-express');
var helpers = __importStar(require("./core/helpers"));
// controllers
var utils_1 = __importDefault(require("./routes/utils"));
var geo_1 = __importDefault(require("./routes/geo"));
var logger = helpers.createLogger('app');
logger.info('Starting...');
var app = express();
app.get('/', function (req, res) {
    res.send('NodeJS API Home');
});
logger.info('Initializing controllers...');
app.use('/', utils_1.default);
app.use('/geo', geo_1.default);
logger.info('Initializing generic handlers...');
app.use(function (req, res, next) {
    /** Log the req */
    logger.info("Request: METHOD: [" + req.method + "], URL: [" + req.url + "], IP: [" + req.socket.remoteAddress + "]");
    res.on('finish', function () {
        /** Log the res */
        logger.info("Response: METHOD: [" + req.method + "], URL: [" + req.url + "], IP: [" + req.socket.remoteAddress + "], STATUS: [" + res.statusCode + "]");
    });
    next();
});
app.use(function (req, res, next) {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept, Authorization');
    if (req.method == 'OPTIONS') {
        res.header('Access-Control-Allow-Methods', 'PUT, POST, PATCH, DELETE, GET');
        return res.status(200).json({});
    }
    next();
});
/** Error handling */
app.use(function (req, res, next) {
    var error = new Error('Not found');
    res.status(404).json({
        message: error.message,
    });
});
logger.info('Initializing swagger...');
// swagger UI
var options = {
    swaggerDefinition: {
        // Like the one described here: https://swagger.io/specification/#infoObject
        info: {
            title: 'MapAnalyzer',
            version: '1.0.0',
            description: 'MapAnalyzer API',
        },
    },
    // List of files to be processes. You can also set globs './routes/*.js'
    apis: ['./routes/geo.ts', './routes/utils.ts'],
};
var swaggerSpecs = swagger_jsdoc_1.default(options);
app.use('/swagger', swaggerUi.serve, swaggerUi.setup(swaggerSpecs));
app.set('port', process.env.PORT || 4001);
logger.info('Starting listener...');
app.listen(app.get('port'), function () {
    logger.info("Express started on port " + app.get('port'));
});
//# sourceMappingURL=app.js.map