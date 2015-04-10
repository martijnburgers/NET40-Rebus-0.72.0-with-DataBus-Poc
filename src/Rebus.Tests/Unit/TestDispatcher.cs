﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rebus.Bus;
using Rebus.Configuration;
using Rebus.Persistence.InMemory;
using Rebus.Tests.Persistence.Sagas;
using Rhino.Mocks;
using Shouldly;

namespace Rebus.Tests.Unit
{
    [TestFixture]
    public class TestDispatcher : FixtureBase
    {
        Dispatcher dispatcher;
        HandlerActivatorForTesting activator;
        RearrangeHandlersPipelineInspector pipelineInspector;

        protected override void DoSetUp()
        {
            activator = new HandlerActivatorForTesting();
            pipelineInspector = new RearrangeHandlersPipelineInspector();
            dispatcher = new Dispatcher(new InMemorySagaPersister(),
                                        activator,
                                        new InMemorySubscriptionStorage(),
                                        pipelineInspector,
                                        new DeferredMessageHandlerForTesting(),
                                        null);
        }

        [Test]
        public void ThrowsIfTwoSagaHandlersArePresentInHandlerPipeline()
        {
            // arrange
            activator.UseHandler(new FirstSaga());
            activator.UseHandler(new SecondSaga());
            var messageThatCanBeHandledByBothSagas = new SomeMessage();

            // act
            var exception =
                Assert.Throws<MultipleSagaHandlersFoundException>(
                    () => dispatcher.Dispatch(messageThatCanBeHandledByBothSagas));

            // assert
            exception.Message.ShouldContain("FirstSaga");
            exception.Message.ShouldContain("SecondSaga");
            exception.Message.ShouldContain("SomeMessage");
        }

        [Test]
        public void DoesNotThrowIfTwoSagaHandlersArePresentInHandlerPipeline_ButSagaPersisterCanUpdateMultipleSagaDatasAtomically()
        {
            // arrange
            var fakePersister = MockRepository.GenerateMock<IStoreSagaData, ICanUpdateMultipleSagaDatasAtomically>();
            
            dispatcher = new Dispatcher(fakePersister,
                                        activator,
                                        new InMemorySubscriptionStorage(),
                                        pipelineInspector,
                                        new DeferredMessageHandlerForTesting(),
                                        null);


            activator.UseHandler(new FirstSaga());
            activator.UseHandler(new SecondSaga());
            var messageThatCanBeHandledByBothSagas = new SomeMessage();

            // act
            Assert.DoesNotThrow(() => dispatcher.Dispatch(messageThatCanBeHandledByBothSagas));
        }

        class FirstSaga : Saga<SomeSagaData>, IHandleMessages<SomeMessage>
        {
            public override void ConfigureHowToFindSaga()
            {
            }

            public void Handle(SomeMessage message)
            {
            }
        }

        class SecondSaga : Saga<SomeSagaData>, IHandleMessages<SomeMessage>
        {
            public override void ConfigureHowToFindSaga()
            {
            }

            public void Handle(SomeMessage message)
            {
            }
        }

        class SomeSagaData : ISagaData
        {
            public Guid Id { get; set; }
            public int Revision { get; set; }
        }

        [Test]
        public void ThrowsIfNoHandlersCanBeFound()
        {
            // arrange
            var theMessage = new SomeMessage();

            // act
            var ex = Assert.Throws<UnhandledMessageException>(() => dispatcher.Dispatch(theMessage));

            // assert
            ex.UnhandledMessage.ShouldBe(theMessage);
        }

        [Test]
        public void PolymorphicDispatchWorksLikeExpected()
        {
            // arrange
            var calls = new List<string>();
            activator.UseHandler(new AnotherHandler(calls))
                .UseHandler(new YetAnotherHandler(calls))
                .UseHandler(new AuthHandler(calls));

            pipelineInspector.SetOrder(typeof (AuthHandler), typeof (AnotherHandler));

            // act
            dispatcher.Dispatch(new SomeMessage());

            // assert
            calls.Count.ShouldBe(5);
            calls[0].ShouldBe("AuthHandler: object");
            calls[1].ShouldStartWith("AnotherHandler");
            calls[2].ShouldStartWith("AnotherHandler");
            calls[3].ShouldStartWith("AnotherHandler");
            calls[4].ShouldBe("YetAnotherHandler: another_interface");
        }


        [Test]
        public void NewSagaIsMarkedAsSuch()
        {
            var saga = new SmallestSagaOnEarthCorrelatedOnInitialMessage();
            activator.UseHandler(saga);
            dispatcher.Dispatch(new SomeMessageWithANumber(1));
            saga.IsNew.ShouldBe(true);
        }

        [Test]
        public void SagaInitiatedTwiceIsNotMarkedAsNewTheSecondTime()
        {
            var saga = new SmallestSagaOnEarthCorrelatedOnInitialMessage();
            activator.UseHandler(saga);
            dispatcher.Dispatch(new SomeMessageWithANumber(1));
            dispatcher.Dispatch(new SomeMessageWithANumber(1));
            saga.IsNew.ShouldBe(false);
        }

        interface ISomeInterface
        {
        }

        interface IAnotherInterface
        {
        }

        class SomeMessage : ISomeInterface, IAnotherInterface
        {
        }

        class SomeMessageWithANumber
        {
            public SomeMessageWithANumber(int theNumber)
            {
                TheNumber = theNumber;
            }

            public int TheNumber { get; private set; }
        }

        class InitiatingMessageWithANumber
        {
            public InitiatingMessageWithANumber(int theNumber)
            {
                TheNumber = theNumber;
            }

            public int TheNumber { get; private set; }
        }

        class SmallestSagaOnEarthCorrelatedOnInitialMessage : Saga<SagaData>, IAmInitiatedBy<SomeMessageWithANumber>
        {
            public void Handle(SomeMessageWithANumber message)
            {
                Data.TheNumber = message.TheNumber;
            }

            public override void ConfigureHowToFindSaga()
            {
                Incoming<SomeMessageWithANumber>(m => m.TheNumber).CorrelatesWith(d => d.TheNumber);
            }
        }

        class SmallestSagaOnEarthNotCorrelatedOnInitialMessage : Saga<SagaData>,
                                                                 IAmInitiatedBy<InitiatingMessageWithANumber>,
                                                                 IHandleMessages<SomeMessageWithANumber>
        {
            public int TimesHandlingSomeMessageWithANumber { get; set; }

            public void Handle(SomeMessageWithANumber message)
            {
                TimesHandlingSomeMessageWithANumber++;
            }

            public void Handle(InitiatingMessageWithANumber message)
            {
                Data.TheNumber = message.TheNumber;
            }

            public override void ConfigureHowToFindSaga()
            {
                Incoming<SomeMessageWithANumber>(m => m.TheNumber).CorrelatesWith(d => d.TheNumber);
            }
        }

        class SagaData : ISagaData
        {
            public Guid Id { get; set; }
            public int Revision { get; set; }
            public int TheNumber { get; set; }
        }

        class AuthHandler : IHandleMessages<object>
        {
            readonly List<string> calls;

            public AuthHandler(List<string> calls)
            {
                this.calls = calls;
            }

            public void Handle(object message)
            {
                calls.Add("AuthHandler: object");
            }
        }

        class AnotherHandler : IHandleMessages<ISomeInterface>, IHandleMessages<object>,
                               IHandleMessages<IAnotherInterface>
        {
            readonly List<string> calls;

            public AnotherHandler(List<string> calls)
            {
                this.calls = calls;
            }

            public void Handle(ISomeInterface message)
            {
                calls.Add("AnotherHandler: some_interface");
            }

            public void Handle(object message)
            {
                calls.Add("AnotherHandler: object");
            }

            public void Handle(IAnotherInterface message)
            {
                calls.Add("AnotherHandler: another_interface");
            }
        }

        class YetAnotherHandler : IHandleMessages<IAnotherInterface>
        {
            readonly List<string> calls;

            public YetAnotherHandler(List<string> calls)
            {
                this.calls = calls;
            }

            public void Handle(IAnotherInterface message)
            {
                calls.Add("YetAnotherHandler: another_interface");
            }
        }
    }
}