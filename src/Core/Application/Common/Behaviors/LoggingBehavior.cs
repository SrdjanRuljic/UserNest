using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handling {typeof(TRequest).Name}");

            TResponse response = await next(cancellationToken);

            logger.LogInformation($"Handled {typeof(TResponse).Name}");

            return response;
        }
    }
}