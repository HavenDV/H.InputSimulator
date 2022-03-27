namespace WindowsInput.Tests;

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
        builder[0].type.Should().Be(Windows.Win32.UI.Input.KeyboardAndMouse.INPUT_TYPE.INPUT_KEYBOARD);
        builder[0].Anonymous.ki.wVk.Should().Be(Windows.Win32.UI.Input.KeyboardAndMouse.VIRTUAL_KEY.VK_A);
    }
}
