using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviors
{
    public sealed class RequestPerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger) :
        IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly Stopwatch _timer = new();

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next(cancellationToken);

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 500)
            {
                var name = typeof(TRequest).Name;

                logger.LogWarning("UserNest long running request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                                   name,
                                   _timer.ElapsedMilliseconds,
                                   request);
            }

            return response;
        }
    }
}