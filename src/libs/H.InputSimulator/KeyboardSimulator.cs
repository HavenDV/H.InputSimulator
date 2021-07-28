using System;
using System.Collections.Generic;
using System.Threading;
using WindowsInput.Native;

namespace WindowsInput
{
    /// <summary>
    /// Implements the <see cref="IKeyboardSimulator"/> interface by calling 
    /// the an <see cref="IInputMessageDispatcher"/> to simulate Keyboard gestures.
    /// </summary>
    public class KeyboardSimulator : IKeyboardSimulator
    {
        #region Properties

        private IInputSimulator InputSimulator { get; }

        /// <summary>
        /// The instance of the <see cref="IInputMessageDispatcher"/> 
        /// to use for dispatching <see cref="INPUT"/> messages.
        /// </summary>
        private IInputMessageDispatcher MessageDispatcher { get; }

        /// <summary>
        /// Gets the <see cref="IMouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="IMouseSimulator"/> instance.</value>
        public IMouseSimulator Mouse => InputSimulator.Mouse;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardSimulator"/> class 
        /// using an instance of a <see cref="WindowsInputMessageDispatcher"/> for
        /// dispatching <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="inputSimulator">
        /// The <see cref="IInputSimulator"/> that owns this instance.
        /// </param>
        public KeyboardSimulator(IInputSimulator inputSimulator)
        {
            InputSimulator = inputSimulator ?? throw new ArgumentNullException(nameof(inputSimulator));
            MessageDispatcher = new WindowsInputMessageDispatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardSimulator"/> class 
        /// using the specified <see cref="IInputMessageDispatcher"/> for 
        /// dispatching <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="inputSimulator">
        /// The <see cref="IInputSimulator"/> that owns this instance.
        /// </param>
        /// <param name="messageDispatcher">
        /// The <see cref="IInputMessageDispatcher"/> to use for 
        /// dispatching <see cref="INPUT"/> messages.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If null is passed as the <paramref name="messageDispatcher"/>.
        /// </exception>
        internal KeyboardSimulator(
            IInputSimulator inputSimulator, 
            IInputMessageDispatcher messageDispatcher)
        {
            InputSimulator = inputSimulator ?? throw new ArgumentNullException(nameof(inputSimulator));
            MessageDispatcher = messageDispatcher ?? throw new InvalidOperationException(
                $"The {nameof(KeyboardSimulator)} cannot operate " +
                $"with a null {nameof(IInputMessageDispatcher)}. " +
                $"Please provide a valid {nameof(IInputMessageDispatcher)} instance " +
                $"to use for dispatching {nameof(INPUT)} messages.");
        }

        #endregion

        #region Private methods

        private static void ModifiersDown(
            InputBuilder builder, 
            IEnumerable<VirtualKeyCode> modifierKeyCodes)
        {
            if (modifierKeyCodes == null) return;
            foreach (var key in modifierKeyCodes) builder.AddKeyDown(key);
        }

        private static void ModifiersUp(
            InputBuilder builder, 
            IEnumerable<VirtualKeyCode> modifierKeyCodes)
        {
            if (modifierKeyCodes == null) return;

            // Key up in reverse (I miss LINQ)
            var stack = new Stack<VirtualKeyCode>(modifierKeyCodes);
            while (stack.Count > 0) builder.AddKeyUp(stack.Pop());
        }

        private static void KeysPress(
            InputBuilder builder, 
            IEnumerable<VirtualKeyCode> keyCodes)
        {
            if (keyCodes == null) return;
            foreach (var key in keyCodes) builder.AddKeyPress(key);
        }

        /// <summary>
        /// Sends the list of <see cref="INPUT"/> messages using 
        /// the <see cref="IInputMessageDispatcher"/> instance.
        /// </summary>
        /// <param name="inputList">
        /// The <see cref="Array"/> of <see cref="INPUT"/> messages to send.
        /// </param>
        private void SendSimulatedInput(INPUT[] inputList)
        {
            MessageDispatcher.DispatchInput(inputList);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Calls the Win32 SendInput method to simulate a KeyDown.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to press</param>
        public IKeyboardSimulator KeyDown(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyDown(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method to simulate a KeyUp.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to lift up</param>
        public IKeyboardSimulator KeyUp(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyUp(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method with a KeyDown and KeyUp message 
        /// in the same input sequence in order to simulate a Key PRESS.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to press</param>
        public IKeyboardSimulator KeyPress(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyPress(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a key press for each of the specified key codes 
        /// in the order they are specified.
        /// </summary>
        /// <param name="keyCodes"></param>
        public IKeyboardSimulator KeyPress(params VirtualKeyCode[] keyCodes)
        {
            var builder = new InputBuilder();
            KeysPress(builder, keyCodes);
            SendSimulatedInput(builder.ToArray());
            return this;
        }

        /// <summary>
        /// Simulates a simple modified keystroke like CTRL-C where CTRL is the modifierKey and C is the key.
        /// The flow is Modifier KeyDown, Key Press, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKeyCode">The modifier key</param>
        /// <param name="keyCode">The key to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(
            VirtualKeyCode modifierKeyCode, 
            VirtualKeyCode keyCode)
        {
            ModifiedKeyStroke(new[] { modifierKeyCode }, new[] { keyCode });
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers 
        /// and one key like CTRL-ALT-C where CTRL and ALT are the modifierKeys and C is the key.
        /// The flow is Modifiers KeyDown in order, Key Press, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of modifier keys</param>
        /// <param name="keyCode">The key to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(
            IEnumerable<VirtualKeyCode> modifierKeyCodes, 
            VirtualKeyCode keyCode)
        {
            ModifiedKeyStroke(modifierKeyCodes, new[] {keyCode});
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there is one modifier and 
        /// multiple keys like CTRL-K-C where CTRL is the modifierKey and K and C are the keys.
        /// The flow is Modifier KeyDown, Keys Press in order, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKey">The modifier key</param>
        /// <param name="keyCodes">The list of keys to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(
            VirtualKeyCode modifierKey, 
            IEnumerable<VirtualKeyCode> keyCodes)
        {
            ModifiedKeyStroke(new [] {modifierKey}, keyCodes);
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and 
        /// multiple keys like CTRL-ALT-K-C where CTRL and ALT are the modifierKeys and K and C are the keys.
        /// The flow is Modifiers KeyDown in order, Keys Press in order, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of modifier keys</param>
        /// <param name="keyCodes">The list of keys to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(
            IEnumerable<VirtualKeyCode> modifierKeyCodes, 
            IEnumerable<VirtualKeyCode> keyCodes)
        {
            var builder = new InputBuilder();
            ModifiersDown(builder, modifierKeyCodes);
            KeysPress(builder, keyCodes);
            ModifiersUp(builder, modifierKeyCodes);

            SendSimulatedInput(builder.ToArray());
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method with a stream of KeyDown and 
        /// KeyUp messages in order to simulate uninterrupted text entry via the keyboard.
        /// </summary>
        /// <param name="text">The text to be simulated.</param>
        public IKeyboardSimulator TextEntry(string text)
        {
            text = text ?? throw new ArgumentNullException(nameof(text));
            if (text.Length > UInt32.MaxValue / 2) throw new ArgumentException(
                $"The text parameter is too long. It must be less than {UInt32.MaxValue / 2} characters.", nameof(text));
            var inputList = new InputBuilder().AddCharacters(text).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a single character text entry via the keyboard.
        /// </summary>
        /// <param name="character">The unicode character to be simulated.</param>
        public IKeyboardSimulator TextEntry(char character)
        {
            var inputList = new InputBuilder().AddCharacter(character).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

#if !NETSTANDARD1_1

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="millsecondsTimeout">The number of milliseconds to wait.</param>
        public IKeyboardSimulator Sleep(int millsecondsTimeout)
        {
            Thread.Sleep(millsecondsTimeout);
            return this;
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="timeout">The time to wait.</param>
        public IKeyboardSimulator Sleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            return this;
        }

#endif

        #endregion
    }
}