using System.Runtime.InteropServices;
using Deliscio.Apis.WebApi.Managers;
using Deliscio.Modules.QueuedLinks.Common.Models;
using Deliscio.Modules.QueuedLinks.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Deliscio.Tests.Unit.WebApi.Managers;

public class LinksManagerTests
{
    private LinksManager _testClass;
    private Mock<IMediator> _mediator;
    private Mock<IBusControl> _bus;
    private Mock<IQueuedLinksService> _queueService;
    private Mock<ILogger<LinksManager>> _logger;

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
        Assert.Throws<ArgumentNullException>(() => new LinksManager(default(IMediator), _bus.Object, _queueService.Object, _logger.Object));
    }

    [Fact]
    public void Cannot_Construct_WithNull_Bus()
    {
        Assert.Throws<ArgumentNullException>(() => new LinksManager(_mediator.Object, default(IBusControl), _queueService.Object, _logger.Object));
    }

    [Fact]
    public void Cannot_Construct_WithNull_QueueService()
    {
        Assert.Throws<ArgumentNullException>(() => new LinksManager(_mediator.Object, _bus.Object, default(IQueuedLinksService), _logger.Object));
    }

    [Fact]
    public void Cannot_Construct_WithNull_Logger()
    {
        Assert.Throws<ArgumentNullException>(() => new LinksManager(_mediator.Object, _bus.Object, _queueService.Object, default(ILogger<LinksManager>)));
    }

    [Fact]
    public async Task Can_Call_GetLinkAsync()
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
    public async Task Cannot_Call_GetLinkAsync_WithInvalid_IdAsync(string value)
    {
        // GetAsync uses Guard, which will throw a ArgumentException is the string is empty, but will throw a ArgumentNullException if the string is null.
        Assert.Multiple(() =>
        {
            Assert.ThrowsAsync<ArgumentException>(() =>
                _testClass.GetLinkAsync(value, CancellationToken.None));
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                _testClass.GetLinkAsync(value, CancellationToken.None));
        });
    }

    [Fact]
    public async Task Can_Call_GetLinksAsync()
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
        var url = "TestValue2034192875";
        var submittedByUserId = "TestValue1704909426";
        var usersTitle = "TestValue2122871760";
        var usersDescription = "TestValue477213193";
        var tags = new[] { "TestValue2070647648", "TestValue1655817658", "TestValue1311775846" };
        var token = CancellationToken.None;

        _queueService.Setup(mock => mock.ProcessNewLinkAsync(It.IsAny<QueuedLink>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<(bool IsSuccess, string Message, QueuedLink Link)>());

        // Act
        var result = await _testClass.SubmitLinkAsync(url, submittedByUserId, usersTitle, usersDescription, tags, token);

        // Assert
        _queueService.Verify(mock => mock.ProcessNewLinkAsync(It.IsAny<QueuedLink>(), It.IsAny<CancellationToken>()));

        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Cannot_Call_SubmitLinkAsync_WithInvalid_UrlAsync(string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SubmitLinkAsync(value, "TestValue324332177", "TestValue932041627", "TestValue1462936722", new[] { "TestValue1550313992", "TestValue2081488401", "TestValue741571745" }, CancellationToken.None));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Cannot_Call_SubmitLinkAsync_WithInvalid_SubmittedByUserIdAsync(string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SubmitLinkAsync("TestValue17814585", value, "TestValue1143163502", "TestValue1317416754", new[] { "TestValue1569180359", "TestValue1765644044", "TestValue1590741572" }, CancellationToken.None));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Cannot_Call_SubmitLinkAsync_WithInvalid_UsersTitleAsync(string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SubmitLinkAsync("TestValue9617144", "TestValue374577968", value, "TestValue43246622", new[] { "TestValue1329053182", "TestValue1059139476", "TestValue1733123712" }, CancellationToken.None));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Cannot_Call_SubmitLinkAsync_WithInvalid_UsersDescriptionAsync(string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SubmitLinkAsync("TestValue1361125351", "TestValue50538018", "TestValue763252262", value, new[] { "TestValue568751364", "TestValue151885363", "TestValue1414406010" }, CancellationToken.None));
    }
}