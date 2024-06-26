using Deliscio.Apis.WebApi.Managers;
using Deliscio.Modules.QueuedLinks.Common.Enums;
using Deliscio.Modules.QueuedLinks.Common.Models;
using Deliscio.Modules.QueuedLinks.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Deliscio.Tests.Unit.WebApi.Managers;

public class LinksManagerTests
{
    private readonly LinksManager _testClass;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IBusControl> _bus;
    private readonly Mock<IQueuedLinksService> _queueService;
    private readonly Mock<ILogger<LinksManager>> _logger;

    public LinksManagerTests()
    {
        _mediator = new Mock<IMediator>();
        _bus = new Mock<IBusControl>();
        _queueService = new Mock<IQueuedLinksService>();
        _logger = new Mock<ILogger<LinksManager>>();
        _testClass = new LinksManager(_mediator.Object, _bus.Object, _queueService.Object, _logger.Object);
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new LinksManager(_mediator.Object, _bus.Object, _queueService.Object, _logger.Object);

        // Assert
        Assert.NotNull(instance);
    }

    [Fact]
    public void Cannot_Construct_WithNull_Mediator()
    {
        Assert.Throws<ArgumentNullException>(() => new LinksManager(default, _bus.Object, _queueService.Object, _logger.Object));
    }

    [Fact]
    public void Cannot_Construct_WithNull_Bus()
    {
        Assert.Throws<ArgumentNullException>(() => new LinksManager(_mediator.Object, default, _queueService.Object, _logger.Object));
    }

    [Fact]
    public void Cannot_Construct_WithNull_QueueService()
    {
        Assert.Throws<ArgumentNullException>(() => new LinksManager(_mediator.Object, _bus.Object, default, _logger.Object));
    }

    [Fact]
    public void Cannot_Construct_WithNull_Logger()
    {
        Assert.Throws<ArgumentNullException>(() => new LinksManager(_mediator.Object, _bus.Object, _queueService.Object, default));
    }

    [Fact]
    public async Task GetLinkAsync_Can_Call()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var token = CancellationToken.None;

        // Act
        var result = await _testClass.GetLinkAsync(id, token);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetLinkAsync_Cannot_Call_WithInvalid_IdAsync(string value)
    {
        // GetAsync uses Guard, which will throw a ArgumentException is the string is empty, but will throw a ArgumentNullException if the string is null.
        async void Checks()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _testClass.GetLinkAsync(value, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _testClass.GetLinkAsync(value, CancellationToken.None));
        }

        Assert.Multiple(Checks);
    }

    [Fact]
    public async Task GetLinksAsync_Can_Call()
    {
        // Arrange
        var pageNo = 84210672;
        var pageSize = 310510093;
        var token = CancellationToken.None;

        // Act
        var result = await _testClass.GetLinksAsync(pageNo, pageSize, token);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetLinksAsync_CanNotCall_With_InvalidPageNo(int pageNo)
    {
        // Arrange
        var pageSize = 310510093;

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _testClass.GetLinksAsync(pageNo, pageSize));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetLinksAsync_CanNotCall_With_Invalid_PageSize(int pageSize)
    {
        // Arrange
        var pageNo = 310510093;

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _testClass.GetLinksAsync(pageNo, pageSize));
    }

    [Fact]
    public async Task Can_Call_SubmitLinkAsync()
    {
        // Arrange
        var now = DateTime.Now.Ticks;
        var userId = Guid.NewGuid().ToString();

        var url = $"http://www.test-site.com/{now}";
        var submittedByUserId = userId;
        var usersTitle = $"{now}";
        var usersDescription = "TestValue477213193";
        var tags = new[] { "TestValue2070647648", "TestValue1655817658", "TestValue1311775846" };
        var token = CancellationToken.None;

        (bool IsSuccess, string Message, QueuedLink Link) expected = (true, $"Success Message: {now}", new QueuedLink
        {
            Url = url,
            SubmittedById = submittedByUserId,
            Title = usersTitle,
            Description = usersDescription,
            Tags = tags,
            State = QueuedStates.Finished,
        });

        _queueService.Setup(mock => mock.ProcessNewLinkAsync(It.IsAny<QueuedLink>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        // Act
        var actual = await _testClass.SubmitLinkAsync(url, submittedByUserId, usersTitle, usersDescription, tags, token);

        // Assert
        _queueService.Verify(mock => mock.ProcessNewLinkAsync(It.IsAny<QueuedLink>(), It.IsAny<CancellationToken>()));

        Assert.NotNull(actual);
        Assert.Equal(expected.Message, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cannot_Call_SubmitLinkAsync_WithInvalid_UrlAsync(string value)
    {
        async void Checks()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _testClass.SubmitLinkAsync(value, "UserId_TestValue324332177", "Title_TestValue1462936722", "Description_TestValue1143163502", new[] { "TestValue1550313992", "TestValue2081488401", "TestValue741571745" }, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _testClass.SubmitLinkAsync(value, "UserId_TestValue324332177", "Title_TestValue1462936722", "Description_TestValue1143163502", new[] { "TestValue1550313992", "TestValue2081488401", "TestValue741571745" }, CancellationToken.None));
        }

        Assert.Multiple(Checks);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cannot_Call_SubmitLinkAsync_WithInvalid_SubmittedByUserIdAsync(string value)
    {
        async void Checks()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _testClass.SubmitLinkAsync("http://www.TestValue17814585.com", value, "Title_TestValue1143163502", "Description_TestValue1462936722", new[] { "TestValue1569180359", "TestValue1765644044", "TestValue1590741572" }, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _testClass.SubmitLinkAsync("http://www.TestValue17814585.com", value, "Title_TestValue1143163502", "Description_TestValue1462936722", new[] { "TestValue1569180359", "TestValue1765644044", "TestValue1590741572" }, CancellationToken.None));
        }

        Assert.Multiple(Checks);
    }
}