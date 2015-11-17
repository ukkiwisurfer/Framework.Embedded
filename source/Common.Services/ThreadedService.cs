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

using Ignite.Framework.Micro.Common.Exceptions;

namespace Ignite.Framework.Micro.Common.Services
{
    using System;
    using System.Threading;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.Core;

    /// <summary>
    /// A service that supports threaded operations.
    /// </summary>
    /// <remarks>
    /// Supports graceful termination, and thread safe property management to
    /// alter the behaviour of the internal threading logic. 
    /// </remarks>
    public abstract class ThreadedService : IThreadedService
    {
        private readonly IResourceLoader m_ResourceLoader;
        private Thread m_WorkerThread;
        private readonly object m_SyncLock;
        private bool m_IsProcessingEnabled;
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
            protected set { m_IsRunning = value; }
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
            set { m_ServiceName = value; }
        }

        /// <summary>
        /// Indicates whether processing of work is to be performed.
        /// </summary>
        public bool IsProcessingEnabled
        {
            get
            {
                lock (m_SyncLock)
                {
                    return m_IsProcessingEnabled;
                }
            }
            set
            {
                lock (m_SyncLock)
                {
                    m_IsProcessingEnabled = value;
                    OnProcessingStateChanged(value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedService"/> class. 
        /// </summary>
        protected ThreadedService(Type serviceType)
        {
            m_ServiceId = Guid.NewGuid().ToString();
            m_ServiceName = serviceType.Name;

            m_CancellationRequestEvent = new ManualResetEvent(false);
            m_CancellationCompleteEvent = new ManualResetEvent(false);
            m_WorkDetectedEvent = new ManualResetEvent(false);
            m_ResourceLoader = new ServicesResourceLoader();

            m_WorkerThread = new Thread(this.PerformWork);
            m_SyncLock = new object();
            m_IsProcessingEnabled = true;

            m_SleepPeriodInMilliseconds = 10000;
            m_WaitForShutdownPeriodInMilliseconds = 10000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedService"/> class. 
        /// </summary>
        /// <param name="logger">
        /// A logging provider.
        /// </param>
        protected ThreadedService(ILogger logger, Type serviceType) : this(serviceType)
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
            LogInfo(m_ResourceLoader.GetString(Resources.StringResources.AttemptingToStartService), m_ServiceId, m_ServiceName);

            m_CancellationRequestEvent.Reset();
            m_CancellationCompleteEvent.Reset();
            m_WorkDetectedEvent.Reset();

            m_WorkerThread = new Thread(this.PerformWork);
            m_WorkerThread.Start();
        }

        /// <summary>
        /// Attempts to stop the service.
        /// </summary>
        public void Stop()
        {
            LogInfo(m_ResourceLoader.GetString(Resources.StringResources.AttemptingToStopService), m_ServiceId, m_ServiceName);


            if (IsRunning)
            {
                try
                {
                    // Request graceful termination.
                    m_CancellationRequestEvent.Set();

                    // Wait for thread to terminate gracefully.
                    if (!m_CancellationCompleteEvent.WaitOne(m_WaitForShutdownPeriodInMilliseconds, false))
                    {
                        // If the thread didn't terminate gracefully, chop it off at the knees.
                        m_WorkerThread.Abort();
                    }
                }
                catch (Exception ex)
                {
                    LogFatal(m_ResourceLoader.GetString(Resources.StringResources.ErrorAttemptingToStopService), ex, m_ServiceId, m_ServiceName);
                }
            }

            LogInfo(m_ResourceLoader.GetString(Resources.StringResources.ServiceHasStopped));
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
                            Stop();
                        }
                        catch (Exception ex)
                        {
                            LogFatal(m_ResourceLoader.GetString(Resources.StringResources.DisposeError), ex, m_ServiceId, m_ServiceName);

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
        protected virtual void PerformWork()
        {
            IsRunning = true;

            try
            {
                LogInfo(m_ResourceLoader.GetString(Resources.StringResources.StartingService), m_ServiceId, m_ServiceName);

                OnOpening();

                // If the sensor platform is active.
                if (IsServiceActive)
                {
                    int signalled = WaitHandle.WaitTimeout;

                    // Main processing loop. Terminate if the cancellation request has been detected.
                    while (signalled != 0)
                    {
                        CheckIfWorkExists();

                        signalled = WaitForEvent();
                    }

                    LogDebug(m_ResourceLoader.GetString(Resources.StringResources.CancellationRequest), m_ServiceId, m_ServiceName);
                }
            }
            catch (Exception ex)
            {
                LogError("Unhandled exception occurred in service {0}'s PerformWork. StackTrace: '{1}'", ex, ServiceName);
            }
            finally
            {
                OnClosing();
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
           
            DetermineIfWorkDetected(signalled);

            return signalled;
        }

        /// <summary>
        /// Processes the signalled event to determine if work needs to be performed.
        /// </summary>
        /// <param name="signalled">
        /// Indicates the resource handle that indicated it was triggered.
        /// </param>
        protected virtual void DetermineIfWorkDetected(int signalled)
        {
            if (signalled == WaitHandle.WaitTimeout)
            {
                LogDebug(m_ResourceLoader.GetString(Resources.StringResources.TimeoutEventOccurred), m_ServiceId);
            }

            // If the work detected event has been signalled, perform the task.
            if (signalled == 1 && IsProcessingEnabled)
            {
                DoWork();
            }            
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
        protected void LogError(string message, Exception ex, params object[] formatting)
        {
            if (m_IsLoggingEnabled)
            {
                m_Logger.Error(message, ex.CreateApplicationException("Unexpected exception"), formatting);
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
        protected virtual void OnOpening()
        {
            LogDebug("Starting service '{0}'", ServiceName);
        }

        /// <summary>
        /// On the request to close the sensor platform, perform this action.
        /// </summary>
        protected virtual void OnClosing()
        {
            LogDebug("Stopping service '{0}'", ServiceName);
        }

        /// <summary>
        /// Is the service currently active.
        /// </summary>
        public abstract bool IsServiceActive { get; }

        /// <summary>
        /// Checks to see if work exists
        /// </summary>
        /// <param name="hasWork">
        /// Flag to indicate whether there is work to be performed.
        /// </param>
        public virtual void CheckIfWorkExists(bool hasWork = false)
        {
        }

        /// <summary>
        /// Event handler that is triggered when the processing state is changed. 
        /// </summary>
        /// <param name="processingState">
        /// Indicates the new processing flag state.
        /// </param>
        protected virtual void OnProcessingStateChanged(bool processingState)
        {
            
        }
    }
}
