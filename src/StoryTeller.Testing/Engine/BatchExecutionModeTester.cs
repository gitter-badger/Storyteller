﻿using NUnit.Framework;
using Shouldly;
using StoryTeller.Engine;
using StoryTeller.Engine.Batching;
using StoryTeller.Model;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class BatchExecutionModeTester : InteractionContext<BatchExecutionMode>
    {
        private Specification theSpecification;
        private SpecResults theResults;
        private SpecRunnerStatus theStatus = SpecRunnerStatus.Valid;

        protected override void beforeEach()
        {
            theSpecification = new Specification();
            theResults = new SpecResults
            {
                HadCriticalException = false
            };

            theStatus = SpecRunnerStatus.Valid;
        }

        [Test]
        public void never_retry_a_spec_marked_acceptance()
        {
            theSpecification.Lifecycle = Lifecycle.Acceptance;
            ShouldBeTestExtensions.ShouldBe(ClassUnderTest.ShouldRetry(theResults, theSpecification, theStatus), false);
        }


        [Test]
        public void never_retry_when_the_spec_encountered_a_critical_exception()
        {
            theSpecification.Lifecycle = Lifecycle.Regression;
            theResults.HadCriticalException = true;
            ShouldBeTestExtensions.ShouldBe(ClassUnderTest.ShouldRetry(theResults, theSpecification, theStatus), false);
        }

        [Test]
        public void never_retry_when_the_spec_runner_is_in_an_invalid_state()
        {
            theSpecification.Lifecycle = Lifecycle.Regression;
            theStatus = SpecRunnerStatus.Invalid;
            ShouldBeTestExtensions.ShouldBe(ClassUnderTest.ShouldRetry(theResults, theSpecification, theStatus), false);
        }

        [Test]
        public void retry_if_the_number_of_attempts_is_less_than_the_max_retries()
        {
            theSpecification.Lifecycle = Lifecycle.Regression;
            theResults.Attempts = 1;

            theSpecification.MaxRetries = 0;
            ShouldBeTestExtensions.ShouldBe(ClassUnderTest.ShouldRetry(theResults, theSpecification, theStatus), false);

            theSpecification.MaxRetries = 1;
            ShouldBeTestExtensions.ShouldBe(ClassUnderTest.ShouldRetry(theResults, theSpecification, theStatus), true);

            theResults.Attempts = 2;
            theSpecification.MaxRetries = 1;
            ShouldBeTestExtensions.ShouldBe(ClassUnderTest.ShouldRetry(theResults, theSpecification, theStatus), false);
        }

    }
}