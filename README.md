# [H.InputSimulator](https://github.com/HavenDV/H.InputSimulator/) 
Allows you to simulate global mouse and keyboard events.
Features:
- Supports scan codes and multi-language input.
- Supports WPF/WinForms/Console windows apps.
- Supports .NET Standard 2.0+, .Net Framework 4.6.2 and .Net 8+.
- Supports trimming/nativeAOT, nullability, and other modern C# features.

Supported OS:
- Windows

## Nuget

[![NuGet](https://img.shields.io/nuget/dt/H.InputSimulator.svg?style=flat-square&label=H.InputSimulator)](https://www.nuget.org/packages/H.InputSimulator/)

```
Install-Package H.InputSimulator
```

## Examples

### SelectCopyPaste
```cs
new InputSimulator().Keyboard
    .ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A)
    .ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C)
    .ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
```

### OpenWindowsExplorer
```cs
new InputSimulator().Keyboard
    .ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_E);
```

### SelfDestructMessage
```cs
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
```

### OpenPaintAndCreateLine
```cs
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
```

## Common problems
### Some simulated input commands were not sent successfully.
Please think of the library as a high-level wrapper over Win32 `SendInput` call. 
Unfortunately, this is a limitation of the API itself, according to this documentation:
- https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput#remarks
- https://en.wikipedia.org/wiki/User_Interface_Privilege_Isolation

The easiest way to get around this is to run your application as an administrator.

## Support
Priority place for bugs: https://github.com/HavenDV/H.InputSimulator/issues  
Priority place for ideas and general questions: https://github.com/HavenDV/H.InputSimulator/discussions  
I also have a Discord support channel:  
https://discord.gg/g8u2t9dKgE
