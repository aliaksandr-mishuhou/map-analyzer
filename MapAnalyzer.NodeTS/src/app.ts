import express = require('express')
import swaggerJsdoc from 'swagger-jsdoc'
const swaggerUi = require('swagger-ui-express')

import * as helpers from './core/helpers'
// controllers
import utils from './routes/utils'
import geo from './routes/geo'

let logger = helpers.createLogger('app')

logger.info('Starting...')

var app = express()

app.get('/', function (req, res) {
    res.send('NodeJS API Home')
})

logger.info('Initializing controllers...')

app.use('/', utils)
app.use('/geo', geo)

logger.info('Initializing generic handlers...')

app.use((req, res, next) => {
    /** Log the req */
    logger.debug(`Req: METHOD: [${req.method}], URL: [${req.url}], IP: [${req.socket.remoteAddress}]`)

    res.on('finish', () => {
        /** Log the res */
        logger.info(`Res: METHOD: [${req.method}], URL: [${req.url}], IP: [${req.socket.remoteAddress}], STATUS: [${res.statusCode}]`)
    })

    next()
})

app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*')
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept, Authorization')

    if (req.method == 'OPTIONS') {
        res.header('Access-Control-Allow-Methods', 'PUT, POST, PATCH, DELETE, GET')
        return res.status(200).json({})
    }

    next()
})

/** Error handling */
app.use((req, res, next) => {
    const error = new Error('Not found')

    res.status(404).json({
        message: error.message,
    })
})

logger.info('Initializing swagger...')

// swagger UI

const options = {
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
}
const swaggerSpecs = swaggerJsdoc(options)
app.use('/swagger', swaggerUi.serve, swaggerUi.setup(swaggerSpecs))

app.set('port', process.env.PORT || 4001)

logger.info('Starting listener...')

app.listen(app.get('port'), function () {
    logger.info(`Express started on port ${app.get('port')}`)
})
