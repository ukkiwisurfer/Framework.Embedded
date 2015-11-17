
namespace Ignite.Framework.Micro.Common.Data
{
    using System.IO;
    using System;
    using System.Text;

    /// <summary>
    /// Writes a XML document to a stream, containing the captured data and metadata related 
    /// to the data's capture.
    /// </summary>
    public class DataStreamBuilder : IDisposable
    {
        private StreamWriter m_StreamWriter;

        private string m_IPAddress;
        private string m_Timestamp;
        private string m_Payload;
        private bool m_IsDisposed;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!m_IsDisposed)
            {
                if (isDisposing)
                {
                    m_StreamWriter.Dispose();
                    m_StreamWriter = null;
                }
            }
        }

        /// <summary>
        /// Sets the IP address of the device the writer is executing on.
        /// </summary>
        /// <param name="ipAddress">
        /// The IP address where the data capture was recorded.
        /// </param>
        /// <returns></returns>
        public void SetMetadata(string ipAddress, DateTime timestamp)
        {
            m_StreamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            m_StreamWriter.WriteLine("<DataCapture version=\"1.2\">");

            m_StreamWriter.WriteLine("<Metadata>");
            m_StreamWriter.Write("<IPAddress>");
            m_StreamWriter.Write(ipAddress);
            m_StreamWriter.WriteLine("</IPAddress>");
            m_StreamWriter.Write("<CaptureTimeStamp>");
            m_StreamWriter.Write( timestamp.ToString("u"));
            m_StreamWriter.WriteLine("</CaptureTimeStamp>");
            m_StreamWriter.WriteLine("</Metadata>");
        }

        /// <summary>
        /// Thes the payload of the data packet.
        /// </summary>
        /// <param name="payload">
        /// The raw data. 
        /// </param>
        public void SetPayload(byte[] payload)
        {
            m_StreamWriter.WriteLine("<Payload>");
            m_StreamWriter.WriteLine(new string(UTF8Encoding.UTF8.GetChars(payload)));
            m_StreamWriter.WriteLine("</Payload>");
            m_StreamWriter.WriteLine("</DataCapture>");
            m_StreamWriter.Flush();
        }
    }
}
