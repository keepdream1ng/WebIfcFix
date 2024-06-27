using IfcFixLib.IfcPipelineDefinition;
using NSubstitute;

namespace IfcFixLib.Tests;
public class PipeConnectorTests
{
	[Fact]
	public void InitiateOwnProcess_Should_UpdateStatus_InCaseError()
	{
		// Arrange
		string expectedErrorMessage = "Error occured.";
		var errorFilter = Substitute.For<IPipeFilter>();
		var previousOutput = Substitute.For<IPipeOut>();
		errorFilter.StartProcess(Arg.Any<CancellationToken>())
			.Returns(async (ct) => new Exception(expectedErrorMessage));
		var connector = new PipeConnector(errorFilter);

		// Act
		connector.SetUpConnetion(previousOutput);
		connector.InitiateOwnProcess(errorFilter, CancellationToken.None);

		// Assert
		Assert.Equal(ProcessStatus.Error, connector.Status);
		Assert.Equal(expectedErrorMessage, connector.StatusDescription);
	}
}
