# Embedded-Framework

This is an optionated framework for working with the .NET Micro platform. It is heavily based around concepts taken from SOA micro-services, but slimmed down for a constrained hardware platform.

The framework is based upon the definition of one or more Services, derived from the abstract class ThreadedService. Each concrete service is hosted, started and stopped by the MultiServiceHost. 

The current approach is to capture data locally, persist it locally onto SD, and then subsequently publish this data (read off the SD) to a AMQP server as a series of lightweight AMQP messages.

One of the standard services (StatusService) publishes periodic heart-beat messages indicating the state of the device in general, and how much free memory the device has remaining.

The current list of (non-abstract) services are:

StatusService - Publishes the status/health of the device in general.
DataTransferService - Publishes previously persisted datasets (to SD) to a remote server.
BufferedLoggingService - Captures log entries to a persistent storage (SD) locally.
BufferedDataCaptureService - Persists captured datasets to a persistent storage (SD) locally.

There are a number of abstract services that the above services derive from:

ThreadedService - The base service that all services derive from
BufferedDataService - Provides the capability to capture incoming data sets and interact with file streams
