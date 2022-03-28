namespace WindowsInput;

/// <summary>
/// Implements the <see cref="IInputMessageDispatcher"/> by calling <see cref="PInvoke.SendInput(uint, INPUT*, int)"/>.
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD1_1_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
internal class WindowsInputMessageDispatcher : IInputMessageDispatcher
{
    /// <summary>
    /// Dispatches the specified list of <see cref="INPUT"/> messages in their specified order by issuing a single called to <see cref="PInvoke.SendInput(uint, INPUT*, int)"/>.
    /// </summary>
    /// <param name="inputs">The list of <see cref="INPUT"/> messages to be dispatched.</param>
    /// <exception cref="ArgumentException">If the <paramref name="inputs"/> array is empty.</exception>
    /// <exception cref="ArgumentNullException">If the <paramref name="inputs"/> array is null.</exception>
    /// <exception cref="Exception">If the any of the commands in the <paramref name="inputs"/> array could not be sent successfully.</exception>
    public unsafe void DispatchInput(INPUT[] inputs)
    {
        if (inputs == null) throw new ArgumentNullException(nameof(inputs));
        if (inputs.Length == 0) throw new ArgumentException("The input array was empty", nameof(inputs));
        fixed(INPUT* inputsPtr = inputs)
        {
            var successful = PInvoke.SendInput((uint)inputs.Length, inputsPtr, sizeof(INPUT));
            if (successful != inputs.Length)
                throw new InvalidOperationException("Some simulated input commands were not sent successfully. The most common reason for this happening are the security features of Windows including User Interface Privacy Isolation (UIPI). Your application can only send commands to applications of the same or lower elevation. Similarly certain commands are restricted to Accessibility/UIAutomation applications. Refer to the project home page and the code samples for more information.");
        }
    }
}
