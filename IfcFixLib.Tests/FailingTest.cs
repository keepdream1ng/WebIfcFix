namespace IfcFixLib.Tests;
public class FailingTest
{
	[Fact]
	public void WillFail()
	{
		Assert.Fail();
	}
}
