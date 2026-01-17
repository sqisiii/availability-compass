using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using MediatR;
using NSubstitute;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.ManageSources.Sources;

public class SourceServiceBaseTests
{
    private readonly HttpClient _httpClient = new();
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact]
    public async Task RefreshSourceDataAsync_ShouldCallExtractSourceDataAsync()
    {
        // Arrange
        var expectedItems = new List<SourceDataItem>
        {
            new() { SourceId = "TestSource", SeqNo = 1, Title = "Test Trip" }
        };
        var sut = new TestSourceService(_httpClient, _mediator, expectedItems);

        // Act
        var result = await sut.RefreshSourceDataAsync(CancellationToken.None);

        // Assert
        result.ShouldBe(expectedItems);
        sut.ExtractSourceDataAsyncCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task RefreshSourceDataAsync_ShouldSendReplaceSourceDataInDbRequest()
    {
        // Arrange
        var expectedItems = new List<SourceDataItem>
        {
            new() { SourceId = "TestSource", SeqNo = 1, Title = "Test Trip" }
        };
        var sut = new TestSourceService(_httpClient, _mediator, expectedItems);

        // Act
        await sut.RefreshSourceDataAsync(CancellationToken.None);

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<ReplaceSourceDataInDbRequest>(r => r.Sources == expectedItems),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFilters_ShouldQueryOptionsForAllFilterFieldNames()
    {
        // Arrange
        var filterFieldNames = new[] { "Destination", "Type", "Remarks" };
        var sut = new TestSourceService(_httpClient, _mediator, [], filterFieldNames);

        _mediator.Send(Arg.Any<GetFilterOptionsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetFilterOptionsResponse
            {
                FilterOptions = new Dictionary<string, List<string>>
                {
                    { "Destination", ["Poland", "Spain"] },
                    { "Type", ["Biking", "Trekking"] },
                    { "Remarks", ["Limited spots"] }
                }
            });

        // Act
        var filters = await sut.GetFilters(CancellationToken.None);

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<GetFilterOptionsQuery>(q =>
                    q.SourceId == "TestSource" &&
                    q.FieldNames.Count == 3 &&
                    q.FieldNames.Contains("Destination") &&
                    q.FieldNames.Contains("Type") &&
                    q.FieldNames.Contains("Remarks")),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFilters_ShouldReturnFilterForEachFilterFieldName()
    {
        // Arrange
        var filterFieldNames = new[] { "Destination", "Type" };
        var sut = new TestSourceService(_httpClient, _mediator, [], filterFieldNames);

        _mediator.Send(Arg.Any<GetFilterOptionsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetFilterOptionsResponse
            {
                FilterOptions = new Dictionary<string, List<string>>
                {
                    { "Destination", ["Poland", "Spain"] },
                    { "Type", ["Biking"] }
                }
            });

        // Act
        var filters = await sut.GetFilters(CancellationToken.None);

        // Assert
        filters.Count.ShouldBe(2);
        filters.ShouldContain(f => f.Label == "Destination");
        filters.ShouldContain(f => f.Label == "Type");
    }

    [Fact]
    public async Task GetFilters_ShouldSetMultiSelectTypeForAllFilters()
    {
        // Arrange
        var filterFieldNames = new[] { "Destination", "Type" };
        var sut = new TestSourceService(_httpClient, _mediator, [], filterFieldNames);

        _mediator.Send(Arg.Any<GetFilterOptionsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetFilterOptionsResponse { FilterOptions = new Dictionary<string, List<string>>() });

        // Act
        var filters = await sut.GetFilters(CancellationToken.None);

        // Assert
        filters.ShouldAllBe(f => f.Type == SourceFilterType.MultiSelect);
    }

    [Fact]
    public async Task GetFilters_ShouldPopulateOptionsFromQueryResponse()
    {
        // Arrange
        var filterFieldNames = new[] { "Destination" };
        var expectedOptions = new List<string> { "Poland", "Spain", "Italy" };
        var sut = new TestSourceService(_httpClient, _mediator, [], filterFieldNames);

        _mediator.Send(Arg.Any<GetFilterOptionsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetFilterOptionsResponse
            {
                FilterOptions = new Dictionary<string, List<string>>
                {
                    { "Destination", expectedOptions }
                }
            });

        // Act
        var filters = await sut.GetFilters(CancellationToken.None);

        // Assert
        var destinationFilter = filters.Single(f => f.Label == "Destination");
        destinationFilter.Options.ShouldBe(expectedOptions);
    }

    [Fact]
    public void ReportProgress_ShouldRaiseRefreshProgressChangedEvent()
    {
        // Arrange
        var sut = new TestSourceService(_httpClient, _mediator, []);
        SourceRefreshProgressEventArgs? receivedArgs = null;
        sut.RefreshProgressChanged += (_, args) => receivedArgs = args;

        // Act
        sut.TestReportProgress(50.0);

        // Assert
        receivedArgs.ShouldNotBeNull();
        receivedArgs.ProgressPercentage.ShouldBe(50.0);
    }

    [Fact]
    public void ReportProgress_ShouldIncludeCorrectSourceIdInEvent()
    {
        // Arrange
        var sut = new TestSourceService(_httpClient, _mediator, []);
        SourceRefreshProgressEventArgs? receivedArgs = null;
        sut.RefreshProgressChanged += (_, args) => receivedArgs = args;

        // Act
        sut.TestReportProgress(75.0);

        // Assert
        receivedArgs.ShouldNotBeNull();
        receivedArgs.SourceId.ShouldBe("TestSource");
    }

    [SourceService("TestSource", "Test Source", "EN")]
    private class TestSourceService : SourceServiceBase
    {
        private readonly string[] _filterFieldNames;
        private readonly IReadOnlyCollection<SourceDataItem> _itemsToReturn;

        public TestSourceService(
            HttpClient httpClient,
            IMediator mediator,
            IReadOnlyCollection<SourceDataItem> itemsToReturn,
            string[]? filterFieldNames = null)
            : base(httpClient, mediator)
        {
            _itemsToReturn = itemsToReturn;
            _filterFieldNames = filterFieldNames ?? ["Destination", "Type"];
        }

        public bool ExtractSourceDataAsyncCalled { get; private set; }

        protected override IReadOnlyList<string> FilterFieldNames => _filterFieldNames;

        protected override Task<IReadOnlyCollection<SourceDataItem>> ExtractSourceDataAsync(CancellationToken ct)
        {
            ExtractSourceDataAsyncCalled = true;
            return Task.FromResult(_itemsToReturn);
        }

        public void TestReportProgress(double progressPercentage)
        {
            ReportProgress(progressPercentage);
        }
    }
}