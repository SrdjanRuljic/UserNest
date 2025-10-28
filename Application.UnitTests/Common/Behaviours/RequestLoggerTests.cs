using Application.Common.Behaviors;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Common.Behaviours
{
    public sealed class RequestLoggerTests
    {
        private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> _loggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ILoggingHelperService> _loggingHelperServiceMock;
        private readonly LoggingBehavior<TestRequest, TestResponse> _loggingBehavior;

        public RequestLoggerTests()
        {
            _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _loggingHelperServiceMock = new Mock<ILoggingHelperService>();

            _loggingBehavior = new LoggingBehavior<TestRequest, TestResponse>(
                _loggerMock.Object,
                _httpContextAccessorMock.Object,
                _loggingHelperServiceMock.Object);
        }

        [Test]
        public async Task Handle_ShouldLogStartAndCompletion_WhenRequestIsSuccessful()
        {
            // Arrange
            TestRequest request = new TestRequest(1, "Test");
            TestResponse expectedResponse = new TestResponse("Success");
            string clientIp = "192.168.1.1";
            string clientName = "TestClient";
            string hostName = Environment.MachineName;
            string requestParams = "{\"Id\":1,\"Name\":\"Test\"}";

            _loggingHelperServiceMock.Setup(x => x.GetClientIpAddress(_httpContextAccessorMock.Object))
                .Returns(clientIp);
            _loggingHelperServiceMock.Setup(x => x.GetClientName(_httpContextAccessorMock.Object))
                .Returns(clientName);
            _loggingHelperServiceMock.Setup(x => x.SerializeRequestParameters(request))
                .Returns(requestParams);

            Mock<RequestHandlerDelegate<TestResponse>> nextDelegate = new();
            nextDelegate.Setup(x => x(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            TestResponse result = await _loggingBehavior.Handle(request, nextDelegate.Object, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));

            // Verify logging helper service calls
            _loggingHelperServiceMock.Verify(x => x.GetClientIpAddress(_httpContextAccessorMock.Object), Times.Once);
            _loggingHelperServiceMock.Verify(x => x.GetClientName(_httpContextAccessorMock.Object), Times.Once);
            _loggingHelperServiceMock.Verify(x => x.SerializeRequestParameters(request), Times.Once);

            // Verify logger calls
            VerifyLogInformationCall(
                "API Call Started - Method: {MethodName}, Client IP: {ClientIp}, Client Name: {ClientName}, Host: {HostName}, Parameters: {Parameters}");

            VerifyLogInformationCall(
                "API Call Completed - Method: {MethodName}, Response: {ResponseName}, Duration: {Duration}ms, Client IP: {ClientIp}, Host: {HostName}");
        }

        [Test]
        public async Task Handle_ShouldMeasureAndLogDuration_WhenRequestIsSuccessful()
        {
            // Arrange
            TestRequest request = new TestRequest(1, "Test");
            TestResponse expectedResponse = new TestResponse("Success");
            TimeSpan delay = TimeSpan.FromMilliseconds(100);

            SetupDefaultMocks();

            Mock<RequestHandlerDelegate<TestResponse>> nextDelegate = new();
            nextDelegate.Setup(x => x(It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(delay);
                    return expectedResponse;
                });

            // Act
            TestResponse result = await _loggingBehavior.Handle(request, nextDelegate.Object, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));

            // Verify that duration is logged and is approximately correct
            VerifyLogInformationCall(
                "API Call Completed - Method: {MethodName}, Response: {ResponseName}, Duration: {Duration}ms, Client IP: {ClientIp}, Host: {HostName}");
        }

        [Test]
        public async Task Handle_ShouldPassCancellationTokenToNext_WhenCancellationTokenIsProvided()
        {
            // Arrange
            TestRequest request = new TestRequest(1, "Test");
            TestResponse expectedResponse = new TestResponse("Success");
            CancellationToken cancellationToken = new();

            SetupDefaultMocks();

            Mock<RequestHandlerDelegate<TestResponse>> nextDelegate = new();
            nextDelegate.Setup(x => x(cancellationToken))
                .ReturnsAsync(expectedResponse);

            // Act
            TestResponse result = await _loggingBehavior.Handle(request, nextDelegate.Object, cancellationToken);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
            nextDelegate.Verify(x => x(cancellationToken), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldUseCorrectRequestAndResponseTypeNames_WhenDifferentTypesAreUsed()
        {
            // Arrange
            AnotherTestRequest request = new AnotherTestRequest("TestValue");
            AnotherTestResponse expectedResponse = new AnotherTestResponse("OK");

            LoggingBehavior<AnotherTestRequest, AnotherTestResponse> behavior = new(
                new Mock<ILogger<LoggingBehavior<AnotherTestRequest, AnotherTestResponse>>>().Object,
                _httpContextAccessorMock.Object,
                _loggingHelperServiceMock.Object);

            SetupDefaultMocks();

            Mock<RequestHandlerDelegate<AnotherTestResponse>> nextDelegate = new();
            nextDelegate.Setup(x => x(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            AnotherTestResponse result = await behavior.Handle(request, nextDelegate.Object, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));

            // Verify that the correct type names are used in logging
            VerifyLogInformationCall(
                "API Call Started - Method: {MethodName}, Client IP: {ClientIp}, Client Name: {ClientName}, Host: {HostName}, Parameters: {Parameters}");

            VerifyLogInformationCall(
                "API Call Completed - Method: {MethodName}, Response: {ResponseName}, Duration: {Duration}ms, Client IP: {ClientIp}, Host: {HostName}");
        }

        [Test]
        public async Task Handle_ShouldReturnResponseFromNextDelegate_WhenNextDelegateReturnsValue()
        {
            // Arrange
            TestRequest request = new TestRequest(1, "Test");
            TestResponse expectedResponse = new TestResponse("CustomResult");

            SetupDefaultMocks();

            Mock<RequestHandlerDelegate<TestResponse>> nextDelegate = new();
            nextDelegate.Setup(x => x(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            TestResponse result = await _loggingBehavior.Handle(request, nextDelegate.Object, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
            Assert.That(result.Result, Is.EqualTo("CustomResult"));
        }

        [Test]
        public async Task Handle_ShouldLogWithCorrectHostName_WhenEnvironmentMachineNameIsUsed()
        {
            // Arrange
            TestRequest request = new TestRequest(1, "Test");
            TestResponse expectedResponse = new TestResponse("Success");
            string expectedHostName = Environment.MachineName;

            SetupDefaultMocks();

            Mock<RequestHandlerDelegate<TestResponse>> nextDelegate = new();
            nextDelegate.Setup(x => x(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            TestResponse result = await _loggingBehavior.Handle(request, nextDelegate.Object, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));

            // Verify that the host name from Environment.MachineName is used
            VerifyLogInformationCall(
                "API Call Started - Method: {MethodName}, Client IP: {ClientIp}, Client Name: {ClientName}, Host: {HostName}, Parameters: {Parameters}");

            VerifyLogInformationCall(
                "API Call Completed - Method: {MethodName}, Response: {ResponseName}, Duration: {Duration}ms, Client IP: {ClientIp}, Host: {HostName}");
        }

        private void SetupDefaultMocks()
        {
            _loggingHelperServiceMock.Setup(x => x.GetClientIpAddress(_httpContextAccessorMock.Object))
                .Returns("127.0.0.1");
            _loggingHelperServiceMock.Setup(x => x.GetClientName(_httpContextAccessorMock.Object))
                .Returns("TestClient");
            _loggingHelperServiceMock.Setup(x => x.SerializeRequestParameters(It.IsAny<object>()))
                .Returns("{}");
        }

        private void VerifyLogInformationCall(string message)
        {
            ArgumentNullException.ThrowIfNull(message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        // Test request and response records
        public record TestRequest(int Id, string Name);

        public record TestResponse(string Result);

        public record AnotherTestRequest(string Value);

        public record AnotherTestResponse(string Status);
    }
}