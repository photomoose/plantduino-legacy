using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Rumr.Plantduino.Worker.Handlers;
using Rumr.Plantduino.Worker.Messages;

namespace Rumr.Plantduino.Worker.Tests
{
    [TestFixture]
    public class SensorEventHandlerTests
    {
        public class SensorEventHandlerScenario
        {
            protected IElasticSearchWrapper IndexClient;
            protected ITopicPublisher TopicPublisher;

            protected SensorEventHandler CreateHandler()
            {
                IndexClient = Substitute.For<IElasticSearchWrapper>();
                TopicPublisher = Substitute.For<ITopicPublisher>();

                return new SensorEventHandler(new SensorEventIndex(IndexClient), TopicPublisher);
            }

            protected SensorEvent CreateMessage(double temp = 21)
            {
                return new SensorEvent(temp, 1000)
                {
                    EnqueuedTimeUtc = DateTime.UtcNow
                };
            }
        }

        public class WhenAValidMessageIsReceived : SensorEventHandlerScenario
        {
            private SensorEvent _msg;

            [SetUp]
            public void SetUp()
            {
                var handler = CreateHandler();
                _msg = CreateMessage();

                handler.ProcessAsync(_msg).Wait();
            }

            [Test]
            public void ThenSensorEventShouldBeIndexed()
            {
                IndexClient.Received().Index("plantduino", "plantduino",
                    Arg.Is<SensorEvent>(se => se.Temperature == _msg.Temperature && se.Lux == _msg.Lux));
            }
        }


        public class WhenTempFallsToMin : SensorEventHandlerScenario
        {
            private const double Temp = 2.0;

            [SetUp]
            public void SetUp()
            {
                var handler = CreateHandler();

                var msg = CreateMessage(Temp);

                handler.ProcessAsync(msg).Wait();
            }

            [Test]
            public void ThenColdPeriodBeginMessageShouldBePublished()
            {
                TopicPublisher.Received().SendToTopicAsync(Arg.Is<ColdPeriodBegin>(x => x.Temperature == Temp));
            }
        }

        public class WhenTempRemainsAtMin : SensorEventHandlerScenario
        {
            private const double Temp = 2.0;

            [SetUp]
            public void SetUp()
            {
                var handler = CreateHandler();

                var msg1 = CreateMessage(Temp);
                var msg2 = CreateMessage(Temp);

                handler.ProcessAsync(msg1).Wait();
                TopicPublisher.ClearReceivedCalls();

                handler.ProcessAsync(msg2).Wait();
            }

            [Test]
            public void ThenColdPeriodBeginMessageShouldNotBePublished()
            {
                TopicPublisher.DidNotReceive().SendToTopicAsync(Arg.Any<ColdPeriodBegin>());
            }
        }

        public class WhenTempRisesAboveMin : SensorEventHandlerScenario
        {
            [SetUp]
            public void SetUp()
            {
                var handler = CreateHandler();

                const double temp = 2.0;

                var msg1 = CreateMessage(temp);
                var msg2 = CreateMessage(temp + 0.1);

                handler.ProcessAsync(msg1).Wait();
                handler.ProcessAsync(msg2).Wait();
            }

            [Test]
            public void ThenColdPeriodEndMessageShouldBePublished()
            {
                TopicPublisher.Received().SendToTopicAsync(Arg.Any<ColdPeriodEnd>());
            }
        }

        public class WhenTempRemainsAboveMin : SensorEventHandlerScenario
        {
            [SetUp]
            public void SetUp()
            {
                var handler = CreateHandler();

                const double temp = 2.1;

                var msg1 = CreateMessage(temp);
                var msg2 = CreateMessage(temp);

                handler.ProcessAsync(msg1).Wait();
                handler.ProcessAsync(msg2).Wait();
            }

            [Test]
            public void ThenColdPeriodEndMessageShouldNotBePublished()
            {
                TopicPublisher.DidNotReceive().SendToTopicAsync(Arg.Any<ColdPeriodEnd>());
            }
        }
    }
}
