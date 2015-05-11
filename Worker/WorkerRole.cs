using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;

using Elasticsearch.Net;
using Elasticsearch.Net.Connection;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

using Newtonsoft.Json;

namespace Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "ProcessingQueue";
        const string CommandQueueName = "Commands";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient EventClient;
        QueueClient CommandClient;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            this.EventClient.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        // Process the message
                        var stream = receivedMessage.GetBody<Stream>();
                        var reader = new StreamReader(stream);
                        var json = reader.ReadToEnd();

                        var sensorEvent = JsonConvert.DeserializeObject<SensorEvent>(json);

                        var node = new Uri("http://plantduino.cloudapp.net:9200");
                        var config = new ConnectionConfiguration(node);
                        var client = new ElasticsearchClient(config);

                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());

                        //if (sensorEvent.Temperature < 15)
                        //{
                        //    CommandClient.Send(new BrokeredMessage(new StreamWriter()));
                        //}
                        var response = client.Index(
                            "temperature",
                            "temperature",
                            new
                                {
                                    Temperature = sensorEvent.Temperature, 
                                    Lux = sensorEvent.Lux,
                                    Timestamp = receivedMessage.EnqueuedTimeUtc
                                });
                    }
                    catch (Exception ex)
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }
            if (!namespaceManager.QueueExists(CommandQueueName))
            {
                namespaceManager.CreateQueue(CommandQueueName);
            }

            // Initialize the connection to Service Bus Queue
            this.EventClient = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            this.CommandClient = QueueClient.CreateFromConnectionString(connectionString, CommandQueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            this.EventClient.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }

    [DataContract]
    public class SensorEvent
    {
        [DataMember]
        public float Temperature { get; set; }

        [DataMember]
        public float Lux { get; set; }
    }
}
