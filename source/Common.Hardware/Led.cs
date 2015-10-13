//--------------------------------------------------------------------------- 
//   Copyright 2014-2015 Igniteous Limited
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
//----------------------------------------------------------------------------- 

namespace Ignite.Framework.Micro.Common.Hardware
{
    using System.Threading;
    
    using Ignite.Framework.Micro.Common.Contract.Hardware;

    using Microsoft.SPOT.Hardware;

    /// <summary>
    /// Provides the ability to control a LED.
    /// </summary>
    public class Led : ILed
    {
        private readonly OutputPort m_OnboardLed;
        private readonly ManualResetEvent m_PulseWait;

        /// <summary>
        /// Initialises an instance of the <see cref="Led"/> class.
        /// </summary>
        /// <param name="pin">
        /// The pin to use for signalling an LED using the hardware controller.
        /// </param>
        /// <param name="initialState">
        /// The initial value of the LED to set.
        /// </param>
        public Led(Cpu.Pin pin, bool initialState = false)
        {
            m_OnboardLed = new OutputPort(pin, initialState);
            m_PulseWait = new ManualResetEvent(false);
        }

        /// <summary>
        /// See <see cref="ILed.On"/> for more details.
        /// </summary>
        public void On()
        {
            m_OnboardLed.Write(true);
        }

        /// <summary>
        /// See <see cref="ILed.Off"/> for more details.
        /// </summary>
        public void Off()
        {
            m_OnboardLed.Write(false);
        }

        /// <summary>
        /// See <see cref="ILed.Pulse"/> for more details.
        /// </summary>
        /// <remarks>
        /// Assumes the initial state for the LED is off.
        /// </remarks>
        /// <param name="pulsePeriod">
        /// The period in milliseconds to keep the LED on. Defaults to 300 milliseconds.
        /// </param>
        public virtual void Pulse(int pulsePeriod = 300)
        {
            // Make sure the event is not set.
            m_PulseWait.Reset();

            try
            {
                // Turn LED on.
                On();

                // Wait for the timeout to occur before turning LED off.
                m_PulseWait.WaitOne(pulsePeriod, true);
            }
            finally
            {
                // Turn LED off.
                Off();
            }
        }


    }
}
