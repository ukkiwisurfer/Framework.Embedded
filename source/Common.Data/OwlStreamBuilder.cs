
using System;
using System.Text;
using Ignite.Framework.Micro.Common.Core;

namespace Ignite.Framework.Micro.Common.Data
{
    using System.IO;

    public class OwlStreamBuilder
    {
        private StreamWriter m_StreamWriter;
        private Stream m_Stream;

        private string m_IPAddress;
        private string m_Timestamp;
        private string m_Payload;

        /// <summary>
        /// Initiaises an instance of the <see cref="OwlStreamBuilder"/> class.
        /// </summary>
        /// <param name="stream"></param>
        public OwlStreamBuilder(Stream stream)
        {
            m_Stream = stream;
            m_StreamWriter = new StreamWriter(stream);
        }

        /// <summary>
        /// Sets the IP address of the device the writer is executing on.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public OwlStreamBuilder SetIPAddress(string payload)
        {
            m_IPAddress = payload;
            return this;
        }

        public OwlStreamBuilder SetTimestamp(DateTime timestamp)
        {
            m_Timestamp = timestamp.ToString("u");
            return this;
        }

        public OwlStreamBuilder SetPayload(byte[] payload)
        {
            m_Payload = new string(UTF8Encoding.UTF8.GetChars(payload));
            return this;
        }

        /// <summary>
        /// Builds the stream.
        /// </summary>
        public void Build()
        {
            m_StreamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            m_StreamWriter.WriteLine("<DataCapture version=\"1.1\">");

            m_StreamWriter.WriteLine("<CaptureDevice>");
            m_StreamWriter.Write("<IPAddress>");
            m_StreamWriter.Write(m_IPAddress);
            m_StreamWriter.WriteLine("</IPAddress>");
            m_StreamWriter.WriteLine("</CaptureDevice>");

            m_StreamWriter.Write("<CaptureTimeStamp>");
            m_StreamWriter.Write(m_Timestamp);
            m_StreamWriter.WriteLine("</CaptureTimeStamp>");

            m_StreamWriter.WriteLine("<Payload>");
            m_StreamWriter.WriteLine(m_Payload);
            m_StreamWriter.WriteLine("</Payload>");

            m_StreamWriter.WriteLine("</DataCapture>");
            m_StreamWriter.Flush();
        }
    }
}
