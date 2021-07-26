# [H.InputSimulator](https://github.com/HavenDV/H.InputSimulator/) 

[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/H.InputSimulator/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/HavenDV/H.InputSimulator.svg?label=License&maxAge=86400)](LICENSE.md) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Standard%202.0-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
[![Build Status](https://github.com/HavenDV/H.InputSimulator/workflows/.NET/badge.svg?branch=master)](https://github.com/HavenDV/H.InputSimulator/actions?query=workflow%3A%22.NET%22)

Allows you to simulate global mouse and keyboard events.
Features:
- Supports scan codes and multi-language input.
- Supports WPF/WinForms/Console windows apps.
- Supports .NET Standard, .Net Core and .Net 5.

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


## Contacts
* [mail](mailto:havendv@gmail.com)
