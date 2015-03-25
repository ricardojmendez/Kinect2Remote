# Kinect 2 Remote  1.1

## Introduction

Kinect2Remote is a .Net project which connects to the new Kinect for Windows sensor, handles body and gesture messages, packages them and sends over the wire via to a RabbitMQ server, where they can be queried by an arbitrary number of receivers.

It was built initially to centralize data from multiple Kinect sensors into a single large interactive installation, but it has other uses, including:

* Using Kinect data on devices where the Kinect SDK is not supported (such as OSX or Linux).
* Displacing heavy GPU computation onto a separate computer.  Kinect2 is GPU intensive, and you may want to free up this GPU power for your rendering, displacing any body data filtering and processing to a dedicated machine.
* Displacing heavy CPU computation onto a separate computer. The Kinect2Remote includes a framework for writing body processors to pre-calculate body-related data, which can then be sent along with the body.
* Since the client libraries are .Net 3.5, it can be used for sending Kinect data to an application where you don't have access to .Net 4.5.
* Connecting more devices on a single application than the SDK currently allows.

## Dependencies

This version of the remote has been tested against the [final release of the Kinect 2 SDK](http://go.microsoft.com/fwlink/?LinkId=403899).  You will need to get the Kinect and Kinect Gesture Builder packages from NuGet. 

It includes references two Microsoft-provided assemblies, AdaBoostTech.dll and RFRProgessTech.dll, which are included with the SDK. _This means the solution won't build until you have added them_, which will likely happen when you get the gesture builder DLLs from NuGet.

You'll need a [RabbitMQ server](http://rabbitmq.com).

## Projects 

The following libraries are included:

* Arges.KinectRemote.Sensor: handles communication with the Kinect device.
* Arges.KinectRemote.Data: types used to encapsulate the Kinect data.
* Arges.KinectRemote.Transport: encapsulates the RabbitMQ transport functions.

It also includes three test applications:

* Arges.KinectRemote.Transmitter: a console application which connects to the Kinect devices, encapsulates the body data, and sends it over the wire via RabbitMQ.
* Arges.KinectRemote.TestBodyConsole: a test console application which receives Kinect body data and logs it.
* Arges.KinectRemote.TestGestureConsole: a test console application which receives Kinect gesture data and logs it.


## General notes

* The producer and consumer need to be configured using the IP address of a RabbitMQ server, as well as an exchange name.
* The exchange is created as a topic, meaning that all applications connecting to the same exchange will potentially receive the same data BUT can choose which remotes to subscribe to (default is all).
* The sender id for binding to body data is {remoteName}.body, defaulting to *.body.
* The sender id for binding to gesture data is {remoteName}.gesture, defaulting to *.gesture.
* The producer will send all available body and gesture information for all connected sensors to the same exchange. The sensor ID is encoded on the information sent.
* The current Dequeue method is a blocking call.  Do not use it from an Update method on a Unity application, use DequeueNoWait instead.
* Messages on the consumer queues have a TTL of 30ms, since we don't really care about outdated frames. We could expose this as a parameter, but I'm not complicating things right now.
* KeepLastOnlyConsumer is a custom subclass of DefaultBasicConsumer which does not keep a queue, but instead stores only the last message received.  We don’t care about a history of body data, just its last state.


## Kinect2Remote and Unity

Using the Kinect 2 Remote with Unity?  [You'll need to build a custom RabbitMQ .Net client](https://github.com/ricardojmendez/rabbitmq-dotnet-client) for now, since there are two issues with Unity and the official RabbitMQ .Net clients:

* The Mono version Unity 4.5 includes will throw a SocketException on IPv6-enabled operating systems when attempting to connect to a IPv4 address.  [See this pull request for details](https://github.com/rabbitmq/rabbitmq-dotnet-client/pull/24).
* Unity 4.6b17 crashes on my tests when attempting to use either of the final RabbitMQ .Net 3.3.5 clients - neither .Net 3.0 nor 2.0 work.

## Changelog

* v1.1 removes BodyFlags, use the Tags collection.


## Future extensions

Some possible extensions I’m considering are:

* Being able to receive commands to turn on/off some features (such as the data that is being pre-calculated and sent over the wire).
* Compressing and transmitting depth data.

(To be expanded)