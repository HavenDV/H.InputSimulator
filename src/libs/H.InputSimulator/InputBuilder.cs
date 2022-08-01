namespace WindowsInput;

/// <summary>
/// A helper class for building a list of <see cref="INPUT"/> messages ready 
/// to be sent to the native Windows API.
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD1_1_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
internal class InputBuilder : List<INPUT>
{
    /// <summary>
    /// Determines if the <see cref="VirtualKeyCode"/> is an ExtendedKey
    /// </summary>
    /// <param name="keyCode">The key code.</param>
    /// <returns>true if the key code is an extended key; otherwise, false.</returns>
    /// <remarks>
    /// The extended keys consist of the ALT and CTRL keys on the right-hand side of the keyboard; 
    /// the INS, DEL, HOME, END, PAGE UP, PAGE DOWN, and arrow keys in the clusters 
    /// to the left of the numeric keypad; the NUM LOCK key; the BREAK (CTRL+PAUSE) key; 
    /// the PRINT SCRN key; and the divide (/) and ENTER keys in the numeric keypad.
    /// 
    /// See http://msdn.microsoft.com/en-us/library/ms646267(v=vs.85).aspx Section "Extended-Key Flag"
    /// </remarks>
    public static bool IsExtendedKey(VirtualKeyCode keyCode)
    {
        return
            keyCode == VirtualKeyCode.MENU ||
            keyCode == VirtualKeyCode.RMENU ||
            keyCode == VirtualKeyCode.CONTROL ||
            keyCode == VirtualKeyCode.RCONTROL ||
            keyCode == VirtualKeyCode.INSERT ||
            keyCode == VirtualKeyCode.DELETE ||
            keyCode == VirtualKeyCode.HOME ||
            keyCode == VirtualKeyCode.END ||
            keyCode == VirtualKeyCode.PRIOR ||
            keyCode == VirtualKeyCode.NEXT ||
            keyCode == VirtualKeyCode.RIGHT ||
            keyCode == VirtualKeyCode.UP ||
            keyCode == VirtualKeyCode.LEFT ||
            keyCode == VirtualKeyCode.DOWN ||
            keyCode == VirtualKeyCode.NUMLOCK ||
            keyCode == VirtualKeyCode.CANCEL ||
            keyCode == VirtualKeyCode.SNAPSHOT ||
            keyCode == VirtualKeyCode.DIVIDE;
    }

    /// <summary>
    /// Adds a key down to the list of <see cref="INPUT"/> messages.
    /// </summary>
    /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddKeyDown(VirtualKeyCode keyCode)
    {
        var down = new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = new KEYBDINPUT
                {
                    wVk = (VIRTUAL_KEY)keyCode,
                    wScan = (ushort)(PInvoke.MapVirtualKey((uint)keyCode, 0) & 0xFFU),
                    dwFlags = IsExtendedKey(keyCode)
                        ? KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY
                        : 0,
                    time = 0,
                    dwExtraInfo = 0,
                },
            },
        };

        Add(down);

        return this;
    }

    /// <summary>
    /// Adds a key up to the list of <see cref="INPUT"/> messages.
    /// </summary>
    /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddKeyUp(VirtualKeyCode keyCode)
    {
        var up = new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = new KEYBDINPUT
                {
                    wVk = (VIRTUAL_KEY) keyCode,
                    wScan = (ushort)(PInvoke.MapVirtualKey((uint)keyCode, 0) & 0xFFU),
                    dwFlags = IsExtendedKey(keyCode)
                        ? KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP | KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY
                        : KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP,
                    time = 0,
                    dwExtraInfo = 0,
                },
            },
        };

        Add(up);

        return this;
    }

    /// <summary>
    /// Adds a key press to the list of <see cref="INPUT"/> messages which is equivalent to a key down followed by a key up.
    /// </summary>
    /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddKeyPress(VirtualKeyCode keyCode)
    {
        AddKeyDown(keyCode);
        AddKeyUp(keyCode);

        return this;
    }

    /// <summary>
    /// Adds the character to the list of <see cref="INPUT"/> messages.
    /// </summary>
    /// <param name="character">The <see cref="char"/> to be added to the list of <see cref="INPUT"/> messages.</param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddCharacter(char character)
    {
        ushort scanCode = character;

        var down = new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = scanCode,
                    dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_UNICODE,
                    time = 0,
                    dwExtraInfo = 0,
                },
            },
        };

        var up = new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = scanCode,
                    dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP | KEYBD_EVENT_FLAGS.KEYEVENTF_UNICODE,
                    time = 0,
                    dwExtraInfo = 0,
                },
            },
        };

        // Handle extended keys:
        // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
        // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
        if ((scanCode & 0xFF00) == 0xE000)
        {
            down.Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY;
            up.Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY;
        }

        Add(down);
        Add(up);

        return this;
    }

    /// <summary>
    /// Adds all of the characters in the specified <see cref="IEnumerable{T}"/> of <see cref="char"/>.
    /// </summary>
    /// <param name="characters">The characters to add.</param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddCharacters(IEnumerable<char> characters)
    {
        foreach (var character in characters)
        {
            AddCharacter(character);
        }

        return this;
    }

    /// <summary>
    /// Adds the characters in the specified <see cref="string"/>.
    /// </summary>
    /// <param name="characters">The string of <see cref="char"/> to add.</param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddCharacters(string characters)
    {
        return AddCharacters(characters.ToCharArray());
    }

    /// <summary>
    /// Moves the mouse relative to its current position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddRelativeMouseMovement(int x, int y)
    {
        var movement = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        movement.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE;
        movement.Anonymous.mi.dx = x;
        movement.Anonymous.mi.dy = y;

        Add(movement);

        return this;
    }

    /// <summary>
    /// Move the mouse to an absolute position.
    /// </summary>
    /// <param name="absoluteX"></param>
    /// <param name="absoluteY"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddAbsoluteMouseMovement(int absoluteX, int absoluteY)
    {
        var movement = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        movement.Anonymous.mi.dwFlags =
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE |
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE;
        movement.Anonymous.mi.dx = absoluteX;
        movement.Anonymous.mi.dy = absoluteY;

        Add(movement);

        return this;
    }

    /// <summary>
    /// Move the mouse to the absolute position on the virtual desktop.
    /// </summary>
    /// <param name="absoluteX"></param>
    /// <param name="absoluteY"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddAbsoluteMouseMovementOnVirtualDesktop(int absoluteX, int absoluteY)
    {
        var movement = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        movement.Anonymous.mi.dwFlags =
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE |
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE |
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_VIRTUALDESK;
        movement.Anonymous.mi.dx = absoluteX;
        movement.Anonymous.mi.dy = absoluteY;

        Add(movement);

        return this;
    }

    /// <summary>
    /// Adds a mouse button down for the specified button.
    /// </summary>
    /// <param name="button"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseButtonDown(MouseButton button)
    {
        var buttonDown = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        buttonDown.Anonymous.mi.dwFlags = ToMouseButtonDownFlag(button);

        Add(buttonDown);

        return this;
    }

    /// <summary>
    /// Adds a mouse button down for the specified button.
    /// </summary>
    /// <param name="xButtonId"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseXButtonDown(int xButtonId)
    {
        var buttonDown = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        buttonDown.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN;
        buttonDown.Anonymous.mi.mouseData = xButtonId;
        Add(buttonDown);

        return this;
    }

    /// <summary>
    /// Adds a mouse button up for the specified button.
    /// </summary>
    /// <param name="button"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseButtonUp(MouseButton button)
    {
        var buttonUp = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        buttonUp.Anonymous.mi.dwFlags = ToMouseButtonUpFlag(button);
        Add(buttonUp);

        return this;
    }

    /// <summary>
    /// Adds a mouse button up for the specified button.
    /// </summary>
    /// <param name="xButtonId"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseXButtonUp(int xButtonId)
    {
        var buttonUp = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        buttonUp.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP;
        buttonUp.Anonymous.mi.mouseData = xButtonId;
        Add(buttonUp);

        return this;
    }

    /// <summary>
    /// Adds a single click of the specified button.
    /// </summary>
    /// <param name="button"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseButtonClick(MouseButton button)
    {
        return AddMouseButtonDown(button).AddMouseButtonUp(button);
    }

    /// <summary>
    /// Adds a single click of the specified button.
    /// </summary>
    /// <param name="xButtonId"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseXButtonClick(int xButtonId)
    {
        return AddMouseXButtonDown(xButtonId).AddMouseXButtonUp(xButtonId);
    }

    /// <summary>
    /// Adds a double click of the specified button.
    /// </summary>
    /// <param name="button"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseButtonDoubleClick(MouseButton button)
    {
        return AddMouseButtonClick(button).AddMouseButtonClick(button);
    }

    /// <summary>
    /// Adds a double click of the specified button.
    /// </summary>
    /// <param name="xButtonId"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseXButtonDoubleClick(int xButtonId)
    {
        return AddMouseXButtonClick(xButtonId).AddMouseXButtonClick(xButtonId);
    }

    /// <summary>
    /// Scroll the vertical mouse wheel by the specified amount.
    /// </summary>
    /// <param name="scrollAmount"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseVerticalWheelScroll(int scrollAmount)
    {
        var scroll = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        scroll.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL;
        scroll.Anonymous.mi.mouseData = scrollAmount;

        Add(scroll);

        return this;
    }

    /// <summary>
    /// Scroll the horizontal mouse wheel by the specified amount.
    /// </summary>
    /// <param name="scrollAmount"></param>
    /// <returns>This <see cref="InputBuilder"/> instance.</returns>
    public InputBuilder AddMouseHorizontalWheelScroll(int scrollAmount)
    {
        var scroll = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
        };
        scroll.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_HWHEEL;
        scroll.Anonymous.mi.mouseData = scrollAmount;

        Add(scroll);

        return this;
    }

    private static MOUSE_EVENT_FLAGS ToMouseButtonDownFlag(MouseButton button)
    {
        return button switch
        {
            MouseButton.LeftButton => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN,
            MouseButton.MiddleButton => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN,
            MouseButton.RightButton => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN,
            _ => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN,
        };
    }

    private static MOUSE_EVENT_FLAGS ToMouseButtonUpFlag(MouseButton button)
    {
        return button switch
        {
            MouseButton.LeftButton => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP,
            MouseButton.MiddleButton => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP,
            MouseButton.RightButton => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP,
            _ => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP,
        };
    }
}
