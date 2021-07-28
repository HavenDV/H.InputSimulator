﻿namespace WindowsInput
{
    /// <summary>
    /// Implements the <see cref="IInputSimulator"/> interface to 
    /// simulate Keyboard and Mouse input and provide the state of those input devices.
    /// </summary>
    public class InputSimulator : IInputSimulator
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="IKeyboardSimulator"/> instance for simulating Keyboard input.
        /// </summary>
        /// <value>The <see cref="IKeyboardSimulator"/> instance.</value>
        public IKeyboardSimulator Keyboard { get; }

        /// <summary>
        /// Gets the <see cref="IMouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="IMouseSimulator"/> instance.</value>
        public IMouseSimulator Mouse { get; }

        /// <summary>
        /// Gets the <see cref="IInputDeviceStateAdaptor"/> instance for 
        /// determining the state of the various input devices.
        /// </summary>
        /// <value>The <see cref="IInputDeviceStateAdaptor"/> instance.</value>
        public IInputDeviceStateAdaptor InputDeviceState { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSimulator"/> class using 
        /// the specified <see cref="IKeyboardSimulator"/>, <see cref="IMouseSimulator"/> 
        /// and <see cref="IInputDeviceStateAdaptor"/> instances.
        /// </summary>
        /// <param name="keyboardSimulator">
        /// The <see cref="IKeyboardSimulator"/> instance to use for simulating keyboard input.
        /// </param>
        /// <param name="mouseSimulator">
        /// The <see cref="IMouseSimulator"/> instance to use for simulating mouse input.
        /// </param>
        /// <param name="inputDeviceStateAdaptor">
        /// The <see cref="IInputDeviceStateAdaptor"/> instance to use 
        /// for interpreting the state of input devices.
        /// </param>
        public InputSimulator(
            IKeyboardSimulator keyboardSimulator, 
            IMouseSimulator mouseSimulator, 
            IInputDeviceStateAdaptor inputDeviceStateAdaptor)
        {
            Keyboard = keyboardSimulator;
            Mouse = mouseSimulator;
            InputDeviceState = inputDeviceStateAdaptor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSimulator"/> class 
        /// using the default <see cref="KeyboardSimulator"/>, <see cref="MouseSimulator"/> 
        /// and <see cref="WindowsInputDeviceStateAdaptor"/> instances.
        /// </summary>
        public InputSimulator()
        {
            Keyboard = new KeyboardSimulator(this);
            Mouse = new MouseSimulator(this);
            InputDeviceState = new WindowsInputDeviceStateAdaptor();
        }

        #endregion
    }
}