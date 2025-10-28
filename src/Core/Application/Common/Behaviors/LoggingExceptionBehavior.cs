using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    internal sealed class LoggingExceptionBehavior<TRequest, TResponse>(
        ILogger<LoggingExceptionBehavior<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor,
        ILoggingHelperService loggingHelperService)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (Exception exception)
            {
                string requestName = typeof(TRequest).Name;
                string clientIp = loggingHelperService.GetClientIpAddress(httpContextAccessor);
                string clientName = loggingHelperService.GetClientName(httpContextAccessor);
                string hostName = Environment.MachineName;
                string requestParams = loggingHelperService.SerializeRequestParameters(request);

                logger.LogError(exception,
                    "API Call Failed - Method: {MethodName}, Client IP: {ClientIp}, Client Name: {ClientName}, Host: {HostName}, Parameters: {Parameters}, Error: {ErrorMessage}",
                    requestName, clientIp, clientName, hostName, requestParams, exception.Message);

                throw;
            }
        }
    }
}