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
			.Returns(async (ct) => throw new Exception(expectedErrorMessage));
		var connector = new PipeConnector(errorFilter);

		// Act
		connector.SetUpConnetion(previousOutput);
		await connector.InitiateOwnProcessAsync(CancellationToken.None);

		// Assert
		Assert.Equal(ProcessStatus.Error, connector.Status);
		Assert.Equal(expectedErrorMessage, connector.StatusDescription);
	}
}
