namespace Ignite.Framework.Micro.Common.Services
{
    using System;
    using System.Threading;
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.Logging;
    using NetMf.CommonExtensions;

    /// <summary>
    /// A service that supports threaded operations.
    /// </summary>
    /// <remarks>
    /// Supports graceful termination, and thread safe property management to
    /// alter the behaviour of the internal threading logic. 
    /// </remarks>
    public abstract class ThreadedService : IThreadedService
    {
        
        private Thread m_WorkerThread;
        private readonly object m_SyncLock;
        private bool m_IsDisposed;

        private readonly ILogger m_Logger;
        /// <summary>
        /// Provides the ability to log messages.
        /// </summary>
        protected ILogger Logger
        {
            get {  return m_Logger;}
        }

        private readonly ManualResetEvent m_CancellationRequestEvent;
        /// <summary>
        /// Provides the synchronisation context for detecting and requesting the cancellation of a request.
        /// </summary>
        protected ManualResetEvent CancellationRequestEvent
        {
            get {  return m_CancellationRequestEvent;}
        }

        private readonly ManualResetEvent m_CancellationCompleteEvent;
        /// <summary>
        /// Provides the synchronisation context for detecting (and indicating) the completion of a request.
        /// </summary>
        protected ManualResetEvent CancellationCompleteEvent
        {
            get {  return m_CancellationCompleteEvent;}
        }

        private readonly ManualResetEvent m_WorkDetectedEvent;
        /// <summary>
        /// Provides the synchronisation context for detecting (and indicating) the detection of work to be performed.
        /// </summary>
        protected ManualResetEvent WorkDetectedEvent
        {
            get {  return m_WorkDetectedEvent;}
        }

        private bool m_IsLoggingEnabled;
        /// <summary>
        /// Provides the ability to turn off and on logging, when a logging provider
        /// has been provided at construction time.
        /// </summary>
        public bool IsLoggingEnabled
        {
            get { return m_IsLoggingEnabled; }
            set
            {
                if (m_Logger != null)
                {
                    m_IsLoggingEnabled = value;
                }
            }
        }

        private bool m_IsRunning;
        /// <summary>
        /// Indicates the run status of the service.
        /// </summary>
        public bool IsRunning
        {
            get { return m_IsRunning; }
            internal set { m_IsRunning = value; }
        }

        private int m_SleepPeriodInMilliseconds;
        /// <summary>
        /// The duration to sleep for before rechecking the cancellation token.
        /// </summary>
        public int SleepPeriodInMilliseconds
        {
            get
            {
                lock (m_SyncLock)
                {
                    return m_SleepPeriodInMilliseconds;
                }
            }
            set
            {
                lock (m_SyncLock)
                {
                    m_SleepPeriodInMilliseconds = value;
                }
            }
        }

        private int m_WaitForShutdownPeriodInMilliseconds;
        /// <summary>
        /// The duration to wait for before forcing shutdown.
        /// </summary>
        public int WaitForShutdownPeriodInMilliseconds
        {
            get
            {
                lock (m_SyncLock)
                {
                    return m_WaitForShutdownPeriodInMilliseconds;
                }
            }
            set
            {
                lock (m_SyncLock)
                {
                    m_WaitForShutdownPeriodInMilliseconds = value;
                }
            }
        }

        private readonly string m_ServiceId;
        /// <summary>
        /// The unique identifier of the service.
        /// </summary>
        public string ServiceId
        {
            get { return m_ServiceId; }
        }

        private string m_ServiceName;
        /// <summary>
        /// The service name to associate with the service.
        /// </summary>
        /// <remarks>
        /// If not explicitly set, will default to the ServiceId property value.
        /// </remarks>
        public string ServiceName
        {
            get { return m_ServiceName; }
            protected set { m_ServiceName = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedService"/> class. 
        /// </summary>
        protected ThreadedService()
        {
            m_ServiceId = Guid.NewGuid().ToString();
            m_ServiceName = m_ServiceId;

            this.m_CancellationRequestEvent = new ManualResetEvent(false);
            this.m_CancellationCompleteEvent = new ManualResetEvent(false);
            this.m_WorkDetectedEvent = new ManualResetEvent(false);

            m_WorkerThread = new Thread(this.PerformWork);
            m_SyncLock = new object();

            m_SleepPeriodInMilliseconds = 10000;
            m_WaitForShutdownPeriodInMilliseconds = 10000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedService"/> class. 
        /// </summary>
        /// <param name="logger">
        /// A logging provider.
        /// </param>
        protected ThreadedService(ILogger logger) : this()
        {
            logger.ShouldNotBeNull();

            m_Logger = logger;
            m_IsLoggingEnabled = true;
        }

        /// <summary>
        /// Releases any unmanaged resources.
        /// </summary>
        ~ThreadedService()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts the service running.
        /// </summary>
        public void Start()
        {
            LogInfo("Attempting to start the service. ServiceId: {0}, ServiceName: {1}.", m_ServiceId, m_ServiceName);

            this.m_CancellationRequestEvent.Reset();
            this.m_CancellationCompleteEvent.Reset();
            this.m_WorkDetectedEvent.Reset();

            m_WorkerThread = new Thread(this.PerformWork);
            m_WorkerThread.Start();
        }

        /// <summary>
        /// Attempts to stop the service.
        /// </summary>
        public void Stop()
        {
            LogInfo("Attempting to stop the service. ServiceId: {0}, ServiceName: {1}.", m_ServiceId, m_ServiceName);

            if (IsRunning)
            {
                try
                {
                    // Request graceful termination.
                    this.m_CancellationRequestEvent.Set();

                    // Wait for thread to terminate gracefully.
                    if (!this.m_CancellationCompleteEvent.WaitOne(m_WaitForShutdownPeriodInMilliseconds, false))
                    {
                        // If the thread didn't terminate gracefully, chop it off at the knees.
                        m_WorkerThread.Abort();
                    }
                }
                catch (Exception ex)
                {
                    LogFatal("Error attempting to stop the service. ServiceId: {0}, ServiceName: {1}.", ex, m_ServiceId, m_ServiceName);
                }
            }

            LogInfo("The service has stopped. ServiceId: {0}, ServiceName: {1}.", m_ServiceId, m_ServiceName);
        }

        /// <summary>
        /// Disposes of any unmanaged resources.
        /// </summary>
        /// <remarks>
        /// Attempts to shut down gracefully before forcing shutdown and disposing of unmanaged resources. 
        /// </remarks>
        /// <param name="isDisposing">
        /// Indicates whether the disposal is deterministic or not.
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!m_IsDisposed)
            {
                try
                {
                    if (isDisposing)
                    {
                        try
                        {
                            this.Stop();
                        }
                        catch (Exception ex)
                        {
                            LogFatal("Encountered an error occurred while trying to process IDispose.Dispose(). ServiceId: {0}, ServiceName: {1}.", ex, m_ServiceId, m_ServiceName);
                        }
                        finally
                        {
                            m_WorkerThread = null;
                        }
                    }
                }
                finally
                {
                    m_IsDisposed = true;
                }
            }
        }

        /// <summary>
        /// Carries out the main processing.
        /// </summary>
        private void PerformWork()
        {
            IsRunning = true;

            try
            {
                LogInfo("Starting the service. ServiceId: {0}, ServiceName: {1}.", m_ServiceId, m_ServiceName);
                
                this.OnOpening();

                // If the sensor platform is active.
                if (this.IsServiceActive)
                {
                    int signalled = WaitHandle.WaitTimeout;

                    // Main processing loop. Terminate if the cancellation request has been detected.
                    while (signalled != 0)
                    {
                        this.CheckIfWorkExists();

                        signalled = WaitForEvent();
                    }

                    LogDebug("Cancellation requested. About to start controlled shutdown of service. ServiceId: {0}, ServiceName: {1}.", m_ServiceId, m_ServiceName);
                }
            }
            finally
            {
                this.OnClosing();
                IsRunning = false;
            }

            // Signal that all cancellation processing logic complete. 
            this.m_CancellationCompleteEvent.Set();
        }

        /// <summary>
        /// Waits for an event to signal or a timeout event to occur.
        /// </summary>
        /// <returns>
        /// The integer position  of the event that signalled from the array of supplied <see cref="WaitHandle"/> instances.
        /// </returns>
        protected virtual int WaitForEvent()
        {
            int signalled = WaitHandle.WaitAny(new WaitHandle[] { m_CancellationRequestEvent, m_WorkDetectedEvent }, SleepPeriodInMilliseconds, false);
            if (signalled == WaitHandle.WaitTimeout)
            {
                LogDebug("Waited for event - no event detected. ServiceId: {0}, ServiceName: {1}.", m_ServiceId, m_ServiceName);
            }

            // If the work detected event has been signalled, perform the task.
            if (signalled == 1)
            {
                this.DoWork();
            }

            return signalled;
        }

        /// <summary>
        /// Logs a debugging message.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        protected void LogDebug(string message)
        {
            if (m_IsLoggingEnabled)
            {
                m_Logger.Debug(message);
            }            
        }

        /// <summary>
        /// Logs a debugging message.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="formatting">
        /// Formatting parameters associated with the logging message.
        /// </param>
        protected void LogDebug(string message, params object[] formatting)
        {
            if (m_IsLoggingEnabled)
            {
                m_Logger.Debug(message, formatting);
            }
        }

        /// <summary>
        /// Logs a debugging message.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="formatting">
        /// Formatting parameters associated with the logging message.
        /// </param>
        protected void LogInfo(string message, params object[] formatting)
        {
            if (m_IsLoggingEnabled)
            {
                m_Logger.Info(message, formatting);
            }
        }

        /// <summary>
        /// Logs a debugging message.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// <param name="ex">
        /// The exception associated with the fatal error.
        /// </param>
        /// <param name="formatting">
        /// Formatting parameters associated with the logging message.
        /// </param>
        protected void LogFatal(string message, Exception ex, params object[] formatting)
        {
            if (m_IsLoggingEnabled)
            {
                m_Logger.Fatal(StringUtility.Format(message, formatting), ex);
            }
        }

        /// <summary>
        /// Set the event to indicate that there is work to be done.
        /// </summary>
        protected void SignalWorkToBeDone()
        {
            lock (m_SyncLock)
            {
                m_WorkDetectedEvent.Set();    
            }
        }

        /// <summary>
        /// Reset the event that indicates that there was work to do.  
        /// </summary>
        protected void SignalWorkCompleted()
        {
            lock (m_SyncLock)
            {
                m_WorkDetectedEvent.Reset();
            }
        }

        /// <summary>
        /// The task to perform.
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// On the request to open the sensor platform perform this action.
        /// </summary>
        protected abstract void OnOpening();

        /// <summary>
        /// On the request to close the sensor platform, perform this action.
        /// </summary>
        protected abstract void OnClosing();

        /// <summary>
        /// Is the service currently active.
        /// </summary>
        public abstract bool IsServiceActive { get; }

        /// <summary>
        /// Checks to see if work exists
        /// </summary>
        public virtual void CheckIfWorkExists(bool hasWork = false)
        {
            
        }
    }
}
