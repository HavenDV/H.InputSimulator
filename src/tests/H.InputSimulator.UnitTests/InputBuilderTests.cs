namespace WindowsInput.Native.Tests;

[TestClass]
public class InputBuilderTests
{
    [TestMethod]
    public void AddKeyDown()
    {
        var builder = new InputBuilder();
        builder.ToArray().Should().BeEmpty();

        builder.AddKeyDown(VirtualKeyCode.VK_A);
        builder.Count.Should().Be(1);
        builder[0].Type.Should().Be((uint)InputType.Keyboard);
        builder[0].Data.Keyboard.KeyCode.Should().Be((ushort)VirtualKeyCode.VK_A);
    }
}
