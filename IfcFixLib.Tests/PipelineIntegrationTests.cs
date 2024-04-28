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
        bool processDone = false;
        int attemptsCount = 100;

        var pipelineManager = new PipelineManager(parser);
        pipelineManager.AddToPipeline(filter);
        pipelineManager.AddToPipeline(dublicator);
        pipelineManager.PipeEnd.ProcessDone += (obj, ct) => processDone = true;

        // Act
        var token = pipelineManager.GetNewCancelToken();
        await parser.ParseFromStreamAsync(stream);
        while (!processDone)
        {
            await Task.Delay(100);
            if (--attemptsCount == 0) break;
        }
        string actual = pipelineManager.PipeEnd.Output!.DatabaseIfc.ToString(GeometryGym.Ifc.FormatIfcSerialization.STEP);

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
        int attemptsCount = 100;

        var pipelineManager = new PipelineManager(parser);
        pipelineManager.AddToPipeline(filter);
        for (int i = 0; i < 20; i++)
        {
            pipelineManager.AddToPipeline(new DbDuplicator());
        }
        pipelineManager.PipeEnd.ProcessDone += (obj, ct) => processDone = true;

        // Act 1
        var token = pipelineManager.GetNewCancelToken();
        var data = await parser.ParseFromStreamAsync(stream);
        await Task.Delay(1);
        pipelineManager.StopProcessing();

        // Assert 1
        Assert.NotNull(data);
        Assert.False(processDone);
        Assert.Null(pipelineManager.PipeEnd.Output);

        // Act 2.
        pipelineManager.ContinueProcessing();
        while (!processDone)
        {
            await Task.Delay(100);
            if (--attemptsCount == 0) break;
        }
        string actual = pipelineManager.PipeEnd.Output!.DatabaseIfc.ToString(GeometryGym.Ifc.FormatIfcSerialization.STEP);

        // Assert 2.
        Assert.True(actual.Length > 0);
        Assert.Contains("TestPlate", actual);
        Assert.Contains("TestBeam", actual);
        Assert.DoesNotContain("testWall", actual);
        Assert.DoesNotContain("testColumn", actual);
    }
}
