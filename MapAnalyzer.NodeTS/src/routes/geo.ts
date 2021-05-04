import express = require('express')
const router = express.Router()

router.get('/matrix', (req: express.Request, res: express.Response) => {
    res.send('OK')
})

export default router
