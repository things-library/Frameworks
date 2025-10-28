using System.Diagnostics;
using System.IO;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ThroughputWatchTests
    {
        [TestMethod]
        public void Constructor()
        {
            var watch = new ThingsLibrary.DataType.ThroughputWatch();

            // check all initaization variables
            Assert.AreEqual(0, watch.IncrementCount);
            Assert.AreEqual(0, watch.TotalIncrements);

            Assert.AreEqual(0, watch.ValueCount);
            Assert.AreEqual(0, watch.TotalValue);

            Assert.AreEqual(0, watch.SlidingWindowSize);
            Assert.AreEqual(0, watch.SampleCount);

            Assert.AreEqual(0, watch.CalculateIncrementsPerSecond());
            Assert.AreEqual(0, watch.CalculateValuePerSecond());            
        }

        [TestMethod]
        public void ConstructorWithParameters()
        {
            var totalIncrements = 100;
            var totalValue = 120L;
            var windowSize = 4;
            var incrementSize = 15;

            var watch = new ThingsLibrary.DataType.ThroughputWatch(windowSize)
            {
                TotalIncrements = totalIncrements,
                TotalValue = totalValue
            };

            // check all initaization variables
            Assert.AreEqual(0, watch.IncrementCount);
            Assert.AreEqual(totalIncrements, watch.TotalIncrements);

            Assert.AreEqual(0, watch.ValueCount);
            Assert.AreEqual(totalValue, watch.TotalValue);

            Assert.AreEqual(windowSize, watch.SlidingWindowSize);
            Assert.AreEqual(0, watch.SampleCount);

            Assert.AreEqual(0, watch.CalculateIncrementsPerSecond());
            Assert.AreEqual(null, watch.CalculateETA());

            // make sure increment/value is setting correctly
            watch.Increment(incrementSize);
            Assert.AreEqual(1, watch.IncrementCount);
            Assert.AreEqual(incrementSize, watch.ValueCount);
            Assert.AreEqual(1, watch.SampleCount);

            // make sure the increment addition is working properly
            watch.Increment(incrementSize);
            Assert.AreEqual(2, watch.IncrementCount);
            Assert.AreEqual(2L * incrementSize, watch.ValueCount);
            Assert.AreEqual(2, watch.SampleCount);

            // increment more than the window size
            watch.Increment(incrementSize);
            watch.Increment(incrementSize);
            watch.Increment(incrementSize);
            watch.Increment(incrementSize);
            Assert.AreEqual(windowSize, watch.SampleCount); // sliding window so should be the same as window size
            Assert.AreEqual(6, watch.IncrementCount);
            Assert.AreEqual(watch.IncrementCount * incrementSize, watch.ValueCount);
            Assert.AreEqual(totalIncrements - 6, watch.IncrementsLeft);
            Assert.AreEqual(totalValue - (6 * incrementSize), watch.ValueLeft);

            // no sliding window test
            watch = new ThingsLibrary.DataType.ThroughputWatch(0);
            Assert.AreEqual(0, watch.SlidingWindowSize);
        }

        [DataTestMethod]
        [DataRow(100, 0, 200)] // => (incrementSize, totalValue, windowSize, incrementWaitMS)
        [DataRow(100, 5, 200)]
        public void CalculateIncrements(int totalIncrements, int windowSize, int incrementWaitMS)
        {
            var watch = new ThingsLibrary.DataType.ThroughputWatch(windowSize)
            {
                TotalIncrements = totalIncrements,
            };
            Thread.Sleep(50);

            // we don't have any increments to use yet
            Assert.AreEqual(null, watch.CalculateETA());

            // the first increment should be the slowest in our testing due to memory allociations and such
            double worseCaseIPS = 0;

            // do 20 iterations of work            
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(incrementWaitMS);
                watch.Increment();
                if (i == 0) { worseCaseIPS = watch.EllapsedTime.TotalSeconds; }
            }
            Thread.Sleep(100);

            var secondsPerIncrement = watch.CalculateSecondsPerIncrement();
            var completionTime = watch.CalculateIncrementETA();
            
            // ================================================================================
            // THROUGHPUT (Increments Per Second = IPS)
            // ================================================================================
            var bestCaseIPS = (double)incrementWaitMS / 1000d; // not possible to be faster than this
             
            Assert.IsTrue(secondsPerIncrement >= bestCaseIPS);
            Assert.IsTrue(secondsPerIncrement <= worseCaseIPS);

            // ================================================================================
            // ETA CALCULATION
            // ================================================================================
            var bestCaseETA = (totalIncrements - watch.IncrementCount) * bestCaseIPS;
            var worseCaseETA = (totalIncrements - watch.IncrementCount) * worseCaseIPS;

            Assert.IsTrue(completionTime.Value.TotalSeconds >= bestCaseETA);
            Assert.IsTrue(completionTime.Value.TotalSeconds <= worseCaseETA);

            // finish incrementing
            var total = watch.IncrementsLeft;
            for (int i = 0; i <= total; i++)
            {
                watch.Increment();
            }

            Assert.AreEqual(-1, watch.IncrementsLeft);
            Assert.AreEqual(TimeSpan.Zero, watch.CalculateETA());
        }

        [DataTestMethod]
        [DataRow(10, 1000, 0, 100)]
        [DataRow(10, 1000, 10, 100)]
        [DataRow(10, 1000, 5, 50)]
        public void CalculateValue(long incrementSize, long totalValue, int windowSize, int incrementWaitMS)
        {            
            var watch = new ThingsLibrary.DataType.ThroughputWatch(windowSize)
            {
                TotalValue = totalValue
            };
            Thread.Sleep(50);

            // we don't have any increments to use yet
            Assert.AreEqual(null, watch.CalculateETA());

            double worseCaseVPS = 0;

            // do 20 iterations of work
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(incrementWaitMS);
                watch.Increment(incrementSize);
                if (i == 0) { worseCaseVPS = incrementSize / watch.EllapsedTime.TotalSeconds; }    // first loop should be slowest loop because of the contructor
            }

            var actualVPS = watch.CalculateValuePerSecond(); // VPS = value per second
            var completionTime = watch.CalculateValueETA();

            // ================================================================================
            // THROUGHPUT VPS (Value per second)
            // ================================================================================            
            double bestCaseVPS = incrementSize * (1000d / incrementWaitMS); // not possible to be faster than this
            
            Assert.IsTrue(actualVPS >= worseCaseVPS);
            Assert.IsTrue(actualVPS <= bestCaseVPS);

            // ================================================================================
            // ETA CALCULATION
            // ================================================================================
            var bestCaseETA = (totalValue - watch.ValueCount) / bestCaseVPS;
            var worseCaseETA = (totalValue - watch.ValueCount) / worseCaseVPS;

            Assert.IsTrue(completionTime.Value.TotalSeconds >= bestCaseETA);
            Assert.IsTrue(completionTime.Value.TotalSeconds <= worseCaseETA);

            // finish incrementing            
            var incrementsLeft = (int)System.Math.Ceiling(watch.ValueLeft / incrementSize);
            for (int i = 0; i <= incrementsLeft; i++)
            {
                watch.Increment(incrementSize);
            }

            Assert.IsTrue(watch.ValueLeft <= 0);
            Assert.AreEqual(TimeSpan.Zero, watch.CalculateETA());
        }

        [TestMethod]
        public void ResetBAD()
        {
            // no expected totals are provided so no way to calculate when you will reach there
            var watch = new ThingsLibrary.DataType.ThroughputWatch();

            Assert.Throws<ArgumentException>(() => watch.Reset(1));            
        }

        [TestMethod]
        public void CalculateEtaBAD()
        {
            // no expected totals are provided so no way to calculate when you will reach there
            var watch = new ThingsLibrary.DataType.ThroughputWatch(20)
            {
                TotalIncrements = 0,
                TotalValue = 0
            };

            Assert.Throws<ArgumentException>(() => watch.CalculateETA());
            Assert.Throws<ArgumentException>(() => watch.CalculateIncrementETA());
            Assert.Throws<ArgumentException>(() => watch.CalculateValueETA());
        }
    }
}
