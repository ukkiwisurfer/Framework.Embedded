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

namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.Text;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// An adapter that wraps the <see cref="BufferedDataCaptureService"/>.
    /// </summary>
    public class BufferedDataHandler : IMessageHandler
    {
        private readonly BufferedDataService m_Service;
        /// <summary>
        /// The service to capture the data messages.
        /// </summary>
        protected BufferedDataService Service
        {
            get {  return m_Service;}
        }

        /// <summary>
        /// Initialises an instance  of the <see cref="BufferedDataHandler"/> class.
        /// </summary>
        /// <param name="service"></param>
        public BufferedDataHandler(BufferedDataService service)
        {
            service.ShouldNotBeNull();

            m_Service = service;
        }

        /// <summary>
        /// See <see cref="IMessageHandler.HandleMessage"/> for more detais.
        /// </summary>
        /// <param name="message">
        /// The raw message to be processed.
        /// </param>
        public void HandleMessage(ref byte[] message)
        {
            var dataItem = new DataItem();
            dataItem.CaptureTimestamp = DateTime.UtcNow;
            dataItem.Payload = message;

            Service.AddDataItem(dataItem);
        }
    }
}
