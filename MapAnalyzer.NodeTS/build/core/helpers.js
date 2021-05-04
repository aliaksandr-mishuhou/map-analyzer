"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.createLogger = void 0;
function createLogger(loggerName) {
    var winston = require('winston');
    var logger = winston.createLogger({
        level: 'info',
        format: winston.format.combine(winston.format.timestamp(), winston.format.colorize(), winston.format.printf(function (msg) {
            return msg.timestamp + " - " + msg.level + ": " + msg.message;
        })),
        defaultMeta: { service: loggerName },
        transports: [new winston.transports.Console(), new winston.transports.File({ filename: loggerName + '.log' })],
    });
    return logger;
}
exports.createLogger = createLogger;
//# sourceMappingURL=helpers.js.map