namespace IfcFixLib.Tests;
public class TestFileFixture : IDisposable
{
	public byte[] TestIfcBytes { get; private set; }
    string ifcPath = Path.Combine(Directory.GetCurrentDirectory(), "test.ifc");

    public TestFileFixture()
    {
        TestIfcBytes = File.ReadAllBytes(ifcPath);
    }

    public void Dispose()
	{
	}
}
