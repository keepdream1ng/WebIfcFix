using IfcFixLib.FilterStrategy;
using IfcFixLib.PipelineFilters;
using IfcFixLib.IfcPipelineDefinition;
using NSubstitute;

namespace IfcFixLib.Tests;
public class PipelineIntegrationTests
{
    string ifcPath = Path.Combine(Directory.GetCurrentDirectory(), "test.ifc");

    [Fact]
    public async Task Pipeline_ProcessesSuccesfully()
    {
        // Arrange
        string expected = "TestPlate";
        FileStream stream = new FileStream(ifcPath, FileMode.Open);
        var parser = new DbParser();
        var checker = new StringChecker();
        checker.FilterType = StringFilterType.Contains;
        var valueGetter = new StringValueGetter();
        valueGetter.ValueType = ElementStringValueType.Name;
        var strategy = new StringFilterStrategy()
        {
            StringChecker = checker,
            FilteredString = expected,
            StringValueGetter = valueGetter
        };
        var filter = new ElementsFilter(strategy);
        var dublicator = new DbDuplicator();
        var dbSerializer = new DbSerializer();

        var pipelineManager = new PipelineManager(parser);
        pipelineManager.AddToPipeline(filter);
        pipelineManager.AddToPipeline(dublicator);
        pipelineManager.PipeEnd.PipeInto(dbSerializer);

        // Act
        var token = pipelineManager.GetNewCancelToken();
        await parser.ParseFromStreamAsync(stream, token);
        string actual = dbSerializer.Output!;

        // Assert
        Assert.True(actual.Length > 0);
        Assert.Contains(expected, actual);
        Assert.DoesNotContain("testWall", actual);
    }

    [Fact]
    public async Task PipelineManager_StopsAndContinuesSuccesfully()
    {
        // Arrange
        string expected = "plate beam";
        FileStream stream = new FileStream(ifcPath, FileMode.Open);
        var parser = new DbParser();
        var checker = new StringChecker();
        checker.FilterType = StringFilterType.Contains_Any;
        var valueGetter = new StringValueGetter();
        valueGetter.ValueType = ElementStringValueType.Name;
        var strategy = new StringFilterStrategy()
        {
            StringChecker = checker,
            FilteredString = expected,
            StringValueGetter = valueGetter
        };
        var filter = new ElementsFilter(strategy);
        var dublicator = new DbDuplicator();
        bool processDone = false;

        var pipelineManager = new PipelineManager(parser);
        pipelineManager.AddToPipeline(filter);
        for (int i = 0; i < 20; i++)
        {
            pipelineManager.AddToPipeline(new DbDuplicator());
        }
        pipelineManager.PipeEnd.ProcessDone += (ct) =>
        {
            processDone = true;
            return ValueTask.CompletedTask;
        };
        var dbSerializer1 = new DbSerializer();
        dbSerializer1.SubscribeToOutput(pipelineManager.PipeEnd);

        // Act 1
        var token = pipelineManager.GetNewCancelToken();
        var data = parser.ParseFromStreamAsync(stream, token);
        await parser.GetCompletionTask();
        pipelineManager.StopProcessing();

        // Assert 1
        Assert.NotNull(data);
        Assert.False(processDone);
        Assert.Null(pipelineManager.PipeEnd.Output);
        Assert.Null(dbSerializer1.Output);

        // Act 2.
        dbSerializer1.UnsubscribeFrom(pipelineManager.PipeEnd);
        var dbSerializer2 = new DbSerializer();
        dbSerializer2.SubscribeToOutput(pipelineManager.PipeEnd);
        await pipelineManager.ContinueProcessingAsync();
        string actual = dbSerializer2.Output!;

        // Assert 2.
        Assert.True(actual.Length > 0);
        Assert.Null(dbSerializer1.Output);
        Assert.Contains("TestPlate", actual);
        Assert.Contains("TestBeam", actual);
        Assert.DoesNotContain("testWall", actual);
        Assert.DoesNotContain("testColumn", actual);
    }

    [Fact]
    public async Task PipelineNodesStatus_IndicatesCorrectly_InCaseError()
    {
        // Arrange
		string expectedErrorMessage = "Error occured.";
        FileStream stream = new FileStream(ifcPath, FileMode.Open);
        var parser = new DbParser();
        var dublicator = new DbDuplicator();
		var errorFilter = Substitute.For<IPipeFilter>();
		errorFilter.ProcessAsync(Arg.Any<CancellationToken>())
			.Returns(async (ct) =>
			{
				await Task.Delay(10);
				throw new Exception(expectedErrorMessage);
			});
        var dbSerializer = new DbSerializer();

        var pipelineManager = new PipelineManager(parser);
        var dublicatorNode = pipelineManager.AddToPipeline(dublicator);
        var errorFilterNode = pipelineManager.AddToPipeline(errorFilter);
        pipelineManager.PipeEnd.PipeInto(dbSerializer);

        // Act
        var token = pipelineManager.GetNewCancelToken();
        await parser.ParseFromStreamAsync(stream, token);

        // Assert
        Assert.Equal(ProcessStatus.Done, dublicatorNode.Value.Status);
        Assert.Equal(ProcessStatus.Error, errorFilterNode.Value.Status);
        Assert.Null(dbSerializer.Output);
    }

    [Fact]
    public async Task Pipeline_ResetsSuccesfully()
    {
        // Arrange
        string expected = "TestPlate";
        FileStream stream = new FileStream(ifcPath, FileMode.Open);
        var parser = new DbParser();
        var checker = new StringChecker();
        checker.FilterType = StringFilterType.Contains;
        var valueGetter = new StringValueGetter();
        valueGetter.ValueType = ElementStringValueType.Name;
        var strategy = new StringFilterStrategy()
        {
            StringChecker = checker,
            FilteredString = Guid.NewGuid().ToString(),
            StringValueGetter = valueGetter
        };
        var filter = new ElementsFilter(strategy);
        var dublicator = new DbDuplicator();
        var dbSerializer = new DbSerializer();

        var pipelineManager = new PipelineManager(parser);
        var filterNode = pipelineManager.AddToPipeline(filter);
        var dublicatorNode = pipelineManager.AddToPipeline(dublicator);
        dbSerializer.SubscribeToOutput(pipelineManager.PipeEnd);

        // Act
        await parser.ParseFromStreamAsync(stream);
		ProcessStatus dublicatorFirstTryStatus = dublicatorNode.Value.Status;

        strategy.FilteredString = expected;
        pipelineManager.ResetFromNode(filterNode);

        await pipelineManager.ContinueProcessingAsync();
		ProcessStatus dublicatorSecondTryStatus = dublicatorNode.Value.Status;
        string actual = dbSerializer.Output!;

        // Assert
        Assert.True(actual.Length > 0);
        Assert.Contains(expected, actual);
        Assert.Equal(ProcessStatus.Error, dublicatorFirstTryStatus);
        Assert.Equal(ProcessStatus.Done, dublicatorSecondTryStatus);
        Assert.DoesNotContain("testWall", actual);
    }
}
