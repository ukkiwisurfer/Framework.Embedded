
namespace Ignite.Framework.Micro.Common.Data
{
    using System.IO;
    using System;
    using System.Text;

    /// <summary>
    /// Writes a XML document to a stream, containing the captured data and metadata related 
    /// to the data's capture.
    /// </summary>
    public class DataStreamBuilder
    {
        private readonly StreamWriter m_StreamWriter;

        private string m_IPAddress;
        private string m_Timestamp;
        private string m_Payload;

        /// <summary>
        /// Initiaises an instance of the <see cref="DataStreamBuilder"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream to write to.
        /// </param>
        public DataStreamBuilder(Stream stream)
        {
            m_StreamWriter = new StreamWriter(stream);
        }

        /// <summary>
        /// Sets the IP address of the device the writer is executing on.
        /// </summary>
        /// <param name="ipAddress">
        /// The IP address where the data capture was recorded.
        /// </param>
        /// <returns></returns>
        public void SetIPAddress(string ipAddress)
        {
            m_IPAddress = ipAddress;
        }

        /// <summary>
        /// Sets the timestamp of when the data packet was captured locally.
        /// </summary>
        /// <param name="timestamp">
        /// The timestamp to associate with the data packet.
        /// </param>
        public void SetTimestamp(DateTime timestamp)
        {
            m_Timestamp = timestamp.ToString("u");
        }

        /// <summary>
        /// Thes the payload of the data packet.
        /// </summary>
        /// <param name="payload">
        /// The raw data. 
        /// </param>
        public void SetPayload(byte[] payload)
        {
            m_Payload = new string(UTF8Encoding.UTF8.GetChars(payload));
        }

        /// <summary>
        /// Writes the data stream.
        /// </summary>
        public void Build()
        {
            m_StreamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            m_StreamWriter.WriteLine("<DataCapture version=\"1.2\">");

            m_StreamWriter.WriteLine("<Metadata>");
            m_StreamWriter.Write("<IPAddress>");
            m_StreamWriter.Write(m_IPAddress);
            m_StreamWriter.WriteLine("</IPAddress>");
            m_StreamWriter.Write("<CaptureTimeStamp>");
            m_StreamWriter.Write(m_Timestamp);
            m_StreamWriter.WriteLine("</CaptureTimeStamp>");
            m_StreamWriter.WriteLine("</Metadata>");

            m_StreamWriter.WriteLine("<Payload>");
            m_StreamWriter.WriteLine(m_Payload);
            m_StreamWriter.WriteLine("</Payload>");

            m_StreamWriter.WriteLine("</DataCapture>");
            m_StreamWriter.Flush();
        }
    }
}
