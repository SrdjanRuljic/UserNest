using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    public sealed class LoggingExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
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
                logger.LogError($"Exception for Request {typeof(TRequest).Name}, with message: {exception.Message}");

                throw;
            }
        }
    }
}