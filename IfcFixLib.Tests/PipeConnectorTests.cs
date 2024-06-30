using IfcFixLib.IfcPipelineDefinition;
using NSubstitute;

namespace IfcFixLib.Tests;
public class PipeConnectorTests
{
	[Fact]
	public async Task InitiateOwnProcessAsync_Should_UpdateStatus_InCaseError()
	{
		// Arrange
		string expectedErrorMessage = "Error occured.";
		var errorFilter = Substitute.For<IPipeFilter>();
		var previousOutput = Substitute.For<IPipeOut>();
		errorFilter.ProcessAsync(Arg.Any<CancellationToken>())
			.Returns(async (ct) =>
			{
				await Task.Delay(10);
				throw new Exception(expectedErrorMessage);
			});
		var connector = new PipeConnector(errorFilter);

		// Act
		connector.SetUpConnetion(previousOutput);
		await connector.InitiateOwnProcessAsync(CancellationToken.None);

		// Assert
		Assert.Equal(ProcessStatus.Error, connector.Status);
		Assert.Equal(expectedErrorMessage, connector.StatusDescription);
	}

	[Fact]
	public async Task InitiateOwnProcessAsync_Should_UpdateStatus_InCaseCancel()
	{
		// Arrange
		string expectedErrorMessage = ProcessStatus.Cancelled.ToString();
		var errorFilter = Substitute.For<IPipeFilter>();
		var previousOutput = Substitute.For<IPipeOut>();
		errorFilter.ProcessAsync(Arg.Any<CancellationToken>())
			.Returns(async (callInfo) =>
			{
				var ct = callInfo.Arg<CancellationToken>();
				await Task.Delay(1000, ct);
				ct.ThrowIfCancellationRequested();
			});
		var connector = new PipeConnector(errorFilter);
		var cancelTokenSource = new CancellationTokenSource();

		// Act
		connector.SetUpConnetion(previousOutput);
		var task = connector.InitiateOwnProcessAsync(cancelTokenSource.Token);
		await Task.Delay(100);
		cancelTokenSource.Cancel();
		await task;

		// Assert
		Assert.Equal(ProcessStatus.Cancelled, connector.Status);
		Assert.Equal(expectedErrorMessage, connector.StatusDescription);
	}
}
