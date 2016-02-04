﻿using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using GitHub;
using GitHub.Internals;
using Xunit;

public class TheScientistClass
{
    public class TheScienceMethod
    {
        [Fact]
        public void RunsBothBranchesOfTheExperimentAndReportsSuccess()
        {
            bool candidateRan = false;
            bool controlRan = false;

            // We introduce side effects for testing. Don't do this in real life please.
            // Do we do a deep comparison?
            Func<int> control = () => { controlRan = true; return 42; };
            Func<int> candidate = () => { candidateRan = true; return 42; };

            var result = Scientist.Science<int>("success", experiment =>
            {
                experiment.Use(control);
                experiment.Try(candidate);
            });

            Assert.Equal(42, result);
            Assert.True(candidateRan);
            Assert.True(controlRan);
            Assert.True(((InMemoryPublisher)Scientist.MeasurementPublisher).Measurements.First(m => m.Name == "success").Success);
        }

        [Fact]
        public async Task RunsBothBranchesOfTheExperimentAsyncAndReportsFailure()
        {
            bool candidateRan = false;
            bool controlRan = false;

            // We introduce side effects for testing. Don't do this in real life please.
            Func<Task<int>> control = () => { controlRan = true; return Task.FromResult(42); };
            Func<Task<int>> candidate = () => { candidateRan = true; return Task.FromResult(43); };

            var result = await Scientist.ScienceAsync<int>("failure", experiment =>
            {
                experiment.Use(control);
                experiment.Try(candidate);
            });

            Assert.Equal(42, result);
            Assert.True(candidateRan);
            Assert.True(controlRan);
            Assert.False(((InMemoryPublisher)Scientist.MeasurementPublisher).Measurements.First(m => m.Name == "failure").Success);
        }

        [Fact]
        public void RunsBothBranchesOfTheExperimentAndReportsSuccessWithDurations()
        {
            bool candidateRan = false;
            bool controlRan = false;

            // We introduce side effects for testing. Don't do this in real life please.
            // Do we do a deep comparison?
            Func<int> control = () => { controlRan = true; return 42; };
            Func<int> candidate = () => { candidateRan = true; return 42; };

            var result = Scientist.Science<int>("success", experiment =>
            {
                experiment.Use(control);
                experiment.Try(candidate);
            });

            Assert.Equal(42, result);
            Assert.True(candidateRan);
            Assert.True(controlRan);
            Assert.True(((InMemoryPublisher)Scientist.MeasurementPublisher).Measurements.First(m => m.Name == "success").Success);
            Assert.True(((InMemoryPublisher)Scientist.MeasurementPublisher).Measurements.First(m => m.Name == "success").ControlDuration.Ticks > 0);
            Assert.True(((InMemoryPublisher)Scientist.MeasurementPublisher).Measurements.First(m => m.Name == "success").CandidateDuration.Ticks > 0);
        }

        [Fact]
        public void RunsBothBranchesOfTheExperimentWithResultComparisonSetAndReportsSuccess()
        {
            bool candidateRan = false;
            bool controlRan = false;

            // We introduce side effects for testing. Don't do this in real life please.
            // Do we do a deep comparison?
            Func<ComplexResult> control = () => { controlRan = true; return new ComplexResult {Count = 10, Name = "Tester"}; };
            Func<ComplexResult> candidate = () => { candidateRan = true; return new ComplexResult {Count = 10, Name = "Tester"}; };

            var result = Scientist.Science<ComplexResult>("success", experiment =>
            {
                experiment.ResultComparison = (a, b) => a.Count == b.Count && a.Name == b.Name;
                experiment.Use(control);
                experiment.Try(candidate);
            });

            Assert.Equal(10, result.Count);
            Assert.Equal("Tester", result.Name);
            Assert.True(candidateRan);
            Assert.True(controlRan);
            Assert.True(((InMemoryPublisher)Scientist.MeasurementPublisher).Measurements.First(m => m.Name == "success").Success);
        }

        [Fact]
        public void RunsBothBranchesOfTheExperimentWithResultComparisonSetAndReportsFailure()
        {
            bool candidateRan = false;
            bool controlRan = false;

            // We introduce side effects for testing. Don't do this in real life please.
            // Do we do a deep comparison?
            Func<ComplexResult> control = () => { controlRan = true; return new ComplexResult { Count = 10, Name = "Tester" }; };
            Func<ComplexResult> candidate = () => { candidateRan = true; return new ComplexResult { Count = 10, Name = "Tester2" }; };

            var result = Scientist.Science<ComplexResult>("success", experiment =>
            {
                experiment.ResultComparison = (a, b) => a.Count == b.Count && a.Name == b.Name;
                experiment.Use(control);
                experiment.Try(candidate);
            });

            Assert.Equal(10, result.Count);
            Assert.Equal("Tester", result.Name);
            Assert.True(candidateRan);
            Assert.True(controlRan);
            Assert.False(((InMemoryPublisher)Scientist.MeasurementPublisher).Measurements.First(m => m.Name == "success").Success);
        }
    }
}