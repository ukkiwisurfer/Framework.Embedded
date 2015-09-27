namespace Ignite.Framework.Micro.Common.Hardware
{
    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    /// <summary>
    /// Provides the ability to control the onboard LED.
    /// </summary>
    public class OnboardLed : Led
    {
        /// <summary>
        /// Initialises an instance of the <see cref="OnboardLed"/> class.
        /// </summary>
        public OnboardLed() : base(Pins.ONBOARD_LED, false)
        {
            
        }
    }
}