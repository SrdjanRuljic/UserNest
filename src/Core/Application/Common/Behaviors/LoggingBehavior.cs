using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    internal sealed class LoggingBehavior<TRequest, TResponse>(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor,
        ILoggingHelperService loggingHelperService)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;
            string responseName = typeof(TResponse).Name;
            
            string clientIp = loggingHelperService.GetClientIpAddress(httpContextAccessor);
            string clientName = loggingHelperService.GetClientName(httpContextAccessor);
            string hostName = Environment.MachineName;
            string requestParams = loggingHelperService.SerializeRequestParameters(request);

            logger.LogInformation(
                "API Call Started - Method: {MethodName}, Client IP: {ClientIp}, Client Name: {ClientName}, Host: {HostName}, Parameters: {Parameters}",
                requestName, clientIp, clientName, hostName, requestParams);

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            TResponse response = await next(cancellationToken);
            stopwatch.Stop();

            logger.LogInformation(
                "API Call Completed - Method: {MethodName}, Response: {ResponseName}, Duration: {Duration}ms, Client IP: {ClientIp}, Host: {HostName}",
                requestName, responseName, stopwatch.ElapsedMilliseconds, clientIp, hostName);

            return response;
        }
    }
}