# Kinect2Remote
---------------

## Introduction

Kinect2Remote is a .net4 project which connects to the Kinect sensor, handles body messages, packages them and sends over the wire via RabbitMQ to possibly multiple receivers.

This has multiple uses, including:
* Displacing heavy computation onto a separate computer.  Kinect2 can be GPU intensive, and you may want to free up this GPU power for your rendering, displacing any body data filtering and processing to a dedicated machine.
* Using Kinect data on devices where the Kinect SDK is not supported (such as OSX or Linux).
* Sending Kinect data to an application where you don't have access to .Net 4.5 (such as Unity)
* Connecting more devices on a single application than the SDK currently allows.
* Sharing a Kinect for testing purposes.

You'll need a [RabbitMQ server](http://rabbitmq.com).

_Warning: This is based on preliminary software and/or hardware, subject to change._


## Projects 


The following libraries are included:

* Arges.KinectRemote.Sensor: handles communication with the Kinect device.
* Arges.KinectRemote.Data: types used to encapsulate the Kinect data.
* Arges.KinectRemote.Transport: encapsulates the RabbitMQ transport functions.

It also includes two test applications:

* Arges.KinectRemote.Transmitter: a console application which connects to the Kinect devices, encapsulates the skeleton data, and sends it over the wire via RabbitMQ.
* Arges.KinectRemote.TestConsole: a test console application which receives Kinect data and logs it.


## General notes

* The producer and consumer need to be configured using the IP address of a RabbitMQ server, as well as an exchange name.
* The exchange is created as a topic, meaning that all applications connecting to the same exchang will potentially receive the same data BUT can choose which remotes to subscribe to (default is all).
* The sender id for binding to body data is {remoteName}.body, defaulting to defaultRemote.body.
* The producer will send all available body information for all connected sensors to the same exchange. The sensor ID is encoded on the information sent.
* The current Dequeue method is a blocking call.  Do not use it from an Update method on a Unity application, use DequeueNoWait instead.
* Messages on the consumer queues have a TTL of 30ms, since we don't really care about outdated frames. We could expose this as a parameter, but I'm not complicating things right now.
* KeepLastOnlyConsumer is a custom subclass of DefaultBasicConsumer which does not keep a queue, but instead stores only the last message received.

## TODO

* Consider if keeping KinectBodyReceiver and KinectBodyReceiverLastOnly separate or if to integrate them.

(To be expanded)