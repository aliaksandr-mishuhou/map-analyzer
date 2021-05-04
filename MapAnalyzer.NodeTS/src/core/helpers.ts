export function createLogger(loggerName: string) {
    const winston = require('winston')

    const logger = winston.createLogger({
        level: 'info',
        format: winston.format.combine(
            winston.format.timestamp(),
            winston.format.colorize(),
            winston.format.printf((msg: { timestamp: any; level: any; message: any }) => {
                return `${msg.timestamp} - ${msg.level}: ${msg.message}`
            })
        ),
        defaultMeta: { service: loggerName },
        transports: [new winston.transports.Console(), new winston.transports.File({ filename: loggerName + '.log' })],
    })

    return logger
}
