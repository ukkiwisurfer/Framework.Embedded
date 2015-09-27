namespace Ignite.Framework.Micro.Common.Contract.Hardware
{
    /// <summary>
    /// Supports the capability to toggle an LED off or on.
    /// </summary>
    public interface ILed
    {
        /// <summary>
        /// Turns the LED on.
        /// </summary>
        void On();

        /// <summary>
        /// Turns the LED off.
        /// </summary>
        void Off();

        /// <summary>
        /// Flashes the LED.
        /// </summary>
        /// <param name="pulseDuration">
        /// The period in milliseconds that the LED remains on.
        /// </param>
        void Pulse(int pulseDuration);
    }
}
