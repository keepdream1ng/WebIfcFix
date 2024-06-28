﻿using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;

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
        var strategy = new StringFilterStrategy();
        strategy.StringChecker = checker;
        strategy.FilteredString = expected;
        strategy.StringValueGetter = valueGetter;
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
        var strategy = new StringFilterStrategy();
        strategy.StringChecker = checker;
        strategy.FilteredString = expected;
        strategy.StringValueGetter = valueGetter;
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
}
