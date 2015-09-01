
namespace Ignite.Framework.Micro.Common.Contract.Messaging
{

    /// <summary>
    /// Message handler for processing raw sensor readings
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles raw sensor messages.
        /// </summary>
        /// <param name="message">
        /// The raw message.
        /// </param>
        void HandleMessage(byte[] message);
    }
}