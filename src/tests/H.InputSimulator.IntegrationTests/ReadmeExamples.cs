using WindowsInput.Native;

namespace WindowsInput.Tests;

[TestFixture]
public class ReadmeExamples
{
    [Test]
    [Explicit]
    public void SelectCopyPaste()
    {
        new InputSimulator().Keyboard
            .ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A)
            .ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C)
            .ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
    }

    [Test]
    [Explicit]
    public void OpenWindowsExplorer()
    {
        new InputSimulator().Keyboard
            .ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_E);
    }

    [Test]
    [Explicit]
    public void SelfDestructMessage()
    {
        new InputSimulator().Keyboard
           .ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_R)
           .Sleep(1000)
           .TextEntry("notepad")
           .Sleep(1000)
           .KeyPress(VirtualKeyCode.RETURN)
           .Sleep(1000)
           .TextEntry("These are your orders if you choose to accept them...")
           .TextEntry("This message will self destruct in 5 seconds.")
           .Sleep(5000)
           .ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.F4)
           .KeyPress(VirtualKeyCode.VK_N);
    }

    [Test]
    [Explicit]
    public void OpenPaintAndCreateLine()
    {
        new InputSimulator().Keyboard
           .ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_R)
           .Sleep(1000)
           .TextEntry("mspaint")
           .Sleep(1000)
           .KeyPress(VirtualKeyCode.RETURN)
           .Sleep(1000)
           .Mouse
           .LeftButtonDown()
           .MoveMouseToPositionOnVirtualDesktop(65535 / 2, 65535 / 2)
           .LeftButtonUp();
    }
}
