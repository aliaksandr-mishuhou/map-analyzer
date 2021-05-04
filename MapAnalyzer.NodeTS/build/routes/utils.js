"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * GET service status info
 */
var express = require("express");
var router = express.Router();
/**
 * @swagger
 * /health:
 *    get:
 *      description: Health check
 */
router.get('/health', function (req, res) {
    res.send('OK');
});
/**
 * @swagger
 * /warmup:
 *    get:
 *      description: Warmup
 */
router.get('/warmup', function (req, res) {
    res.send('OK');
});
exports.default = router;
//# sourceMappingURL=utils.js.map