/*
 * GET service status info
 */
import express = require('express')
const router = express.Router()

/**
 * @swagger
 * /health:
 *    get:
 *      description: Health check
 */
router.get('/health', (req: express.Request, res: express.Response) => {
    res.send('OK')
})

/**
 * @swagger
 * /warmup:
 *    get:
 *      description: Warmup
 */
router.get('/warmup', (req: express.Request, res: express.Response) => {
    res.send('OK')
})

export default router
