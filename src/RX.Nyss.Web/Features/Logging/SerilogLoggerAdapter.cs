﻿using Serilog;

namespace RX.Nyss.Web.Features.Logging
{
    public class SerilogLoggerAdapter : ILoggerAdapter
        {
            private readonly ILogger _logger;

            public SerilogLoggerAdapter(ILogger logger)
            {
                _logger = logger;
            }

            public void Debug(object obj) => _logger.Debug(obj?.ToString());

            public void DebugWithCaller(string caller, string message) =>
                _logger.ForContext(CallerLogEventEnricher.CallerPropertyName, caller)
                    .Debug(message);

            public void DebugFormat(string format, params object[] args) => _logger.Debug(string.Format(format, args));

            public void Warn(object obj) => _logger.Warning(obj?.ToString());

            public void WarnFormat(string format, params object[] args) => _logger.Warning(string.Format(format, args));

            public void Info(object obj) => _logger.Information(obj?.ToString());

            public void InfoFormat(string format, params object[] args) => _logger.Information(string.Format(format, args));

            public void Error(object obj) => _logger.Error(obj?.ToString());

            public void ErrorFormat(string format, params object[] args) => _logger.Error(string.Format(format, args));
        }
    }
