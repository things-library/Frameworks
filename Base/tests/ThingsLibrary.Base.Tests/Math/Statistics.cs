using ThingsLibrary.Math.Statistics;

namespace ThingsLibrary.Tests.Math.Statistics
{
    [TestClass, ExcludeFromCodeCoverage]
    public class StatisticsTests
    {
        #region --- Byte Statistics ---

        [TestMethod]
        public void AnalysisEntity_Init()
        {
            var analysis = new AnalysisEntity();

            Assert.AreEqual(byte.MinValue, analysis.Min);
            Assert.AreEqual(byte.MaxValue, analysis.Max);
            Assert.AreEqual(0, analysis.Sum);
            Assert.AreEqual(0, analysis.SumOfDerivation);
            Assert.AreEqual(0, analysis.Count);
            Assert.AreEqual(0, analysis.Range);

            Assert.AreEqual(0, analysis.Mean);
            Assert.AreEqual(0, analysis.StdDev);
            Assert.AreEqual(0, analysis.StdErr);
            Assert.AreEqual(0, analysis.Variance);

            Assert.AreEqual(0, analysis.CalculateMode());
            Assert.AreEqual(0, analysis.GetLimit(0));

            // init 2

            var list = new List<byte>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            analysis = new AnalysisEntity();
            analysis.Add(list.ToArray());

            var analysis2 = new AnalysisEntity(analysis.Min, analysis.Max, analysis.Sum, analysis.SumOfDerivation, analysis.Count);

            Assert.AreEqual(5, analysis2.Min);
            Assert.AreEqual(220, analysis2.Max);
            Assert.AreEqual(695, analysis2.Sum);
            Assert.AreEqual(110525, analysis2.SumOfDerivation);
            Assert.AreEqual(8, analysis2.Count);
            Assert.AreEqual(215, analysis2.Range);

            Assert.AreEqual(86.875, analysis2.Mean);

            Assert.AreEqual(79.173, System.Math.Round(analysis2.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis2.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis2.Variance, 3));

            Assert.AreEqual(analysis2.Mean, analysis.GetLimit(0));
            Assert.AreEqual(analysis2.Mean + analysis2.StdDev, analysis.GetLimit(1));

            Assert.AreEqual(0, analysis.GetByteLimit(-200));
            Assert.AreEqual(255, analysis.GetByteLimit(200));
        }

        [TestMethod]
        public void AnalysisEntity_Frequency()
        {
            var analysis = new AnalysisEntity();
            analysis.Add(100, 10);
            analysis.Add(111, 0);   // 0 frequency

            Assert.AreEqual(100, analysis.Min);
            Assert.AreEqual(100, analysis.Max);
            Assert.AreEqual(1000, analysis.Sum);
            Assert.AreEqual(10, analysis.Count);

            Assert.AreEqual(10, analysis.Histogram[100]);

            // now add something less than the last
            analysis.Add(10, 10);

            Assert.AreEqual(10, analysis.Min);
            Assert.AreEqual(100, analysis.Max);
            Assert.AreEqual(1100, analysis.Sum);
            Assert.AreEqual(20, analysis.Count);

            Assert.AreEqual(10, analysis.Histogram[10]);

            // now add something more than all
            analysis.Add(200, 5);

            Assert.AreEqual(10, analysis.Min);
            Assert.AreEqual(200, analysis.Max);
            Assert.AreEqual(2100, analysis.Sum);
            Assert.AreEqual(25, analysis.Count);

            Assert.AreEqual(5, analysis.Histogram[200]);
        }

        [TestMethod]
        public void AnalysisEntity_Basic()
        {
            var list = new List<byte>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity();
            analysis.Add(list.ToArray());

            //Verified with https://www.calculator.net/statistics-calculator.html

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);
            Assert.AreEqual(110525, analysis.SumOfDerivation);
            Assert.AreEqual(8, analysis.Count);
            Assert.AreEqual(215, analysis.Range);

            Assert.AreEqual(86.875, analysis.Mean);
            Assert.AreEqual(100, analysis.CalculateMode());

            Assert.AreEqual(79.173, System.Math.Round(analysis.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis.Variance, 3));
        }

        [TestMethod]
        public void AnalysisEntity_AddAnalysis()
        {
            // populate a root analysis object
            var list = new List<byte>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var rootAnalysis = new AnalysisEntity();
            rootAnalysis.Add(list.ToArray());

            // add the root to it and we should get the same expected values since the original was blank
            var analysis = new AnalysisEntity();
            analysis.Add(rootAnalysis);

            analysis.Add((AnalysisEntity)null);
            analysis.Add(new AnalysisEntity());

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);
            Assert.AreEqual(110525, analysis.SumOfDerivation);
            Assert.AreEqual(8, analysis.Count);
            Assert.AreEqual(215, analysis.Range);

            Assert.AreEqual(86.875, analysis.Mean);
            Assert.AreEqual(100, analysis.CalculateMode());

            Assert.AreEqual(79.173, System.Math.Round(analysis.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis.Variance, 3));


            // should be the same output initalizing it this way as well
            var analysis2 = new AnalysisEntity(analysis.Min, analysis.Max, analysis.Sum, analysis.SumOfDerivation, analysis.Count);

            Assert.AreEqual(5, analysis2.Min);
            Assert.AreEqual(220, analysis2.Max);
            Assert.AreEqual(695, analysis2.Sum);
            Assert.AreEqual(110525, analysis2.SumOfDerivation);
            Assert.AreEqual(8, analysis2.Count);
            Assert.AreEqual(215, analysis2.Range);

            Assert.AreEqual(86.875, analysis2.Mean);
            //NOTE: MODE WILL NOT COME OVER SINCE WE DONT PULL OVER THE HISTOGRAM
            Assert.AreEqual(79.173, System.Math.Round(analysis2.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis2.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis2.Variance, 3));

            // Add a new min and max and verify
            var analysis3 = new AnalysisEntity();
            analysis3.Add(new List<byte>() { 2, 225 }.ToArray());

            analysis2.Add(analysis3);
            Assert.AreEqual(2, analysis2.Min);
            Assert.AreEqual(225, analysis2.Max);
        }

        [TestMethod]
        public void AnalysisEntity_Clone()
        {
            var list = new List<byte>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity();
            analysis.Add(list.ToArray());

            var clone = analysis.Clone();
            Assert.AreEqual(analysis.Min, clone.Min);
            Assert.AreNotSame(analysis.Min, clone.Min);

            Assert.AreEqual(analysis.Max, clone.Max);
            Assert.AreNotSame(analysis.Max, clone.Max);

            Assert.AreEqual(analysis.Sum, clone.Sum);
            Assert.AreNotSame(analysis.Sum, clone.Sum);

            Assert.AreEqual(analysis.Count, clone.Count);
            Assert.AreNotSame(analysis.Count, clone.Count);
        }

        [TestMethod]
        public void AnalysisEntity_Histogram()
        {
            var list = new List<byte>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity();
            analysis.Add(list.ToArray());

            //Verified with https://www.calculator.net/statistics-calculator.html

            Assert.AreEqual(1, analysis.Histogram[5]);
            Assert.AreEqual(1, analysis.Histogram[10]);
            Assert.AreEqual(1, analysis.Histogram[20]);
            Assert.AreEqual(1, analysis.Histogram[40]);
            Assert.AreEqual(2, analysis.Histogram[100]);
            Assert.AreEqual(1, analysis.Histogram[200]);
            Assert.AreEqual(1, analysis.Histogram[220]);

            // ADD HISTOGRAM TEST
            var analysis2 = new AnalysisEntity(analysis.Histogram);
            
            Assert.AreEqual(1, analysis2.Histogram[5]);
            Assert.AreEqual(1, analysis2.Histogram[10]);
            Assert.AreEqual(1, analysis2.Histogram[20]);
            Assert.AreEqual(1, analysis2.Histogram[40]);
            Assert.AreEqual(2, analysis2.Histogram[100]);
            Assert.AreEqual(1, analysis2.Histogram[200]);
            Assert.AreEqual(1, analysis2.Histogram[220]);
                        
            // Invalid Histogram size
            Assert.ThrowsException<ArgumentException>(() => analysis2.AddHistogram(new long[20]));
        }

        [TestMethod]
        public void AnalysisEntity_Limit()
        {
            var list = new List<byte>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity();
            analysis.Add(list.ToArray());

            //Verified with https://www.calculator.net/statistics-calculator.html

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);

            Assert.AreEqual(245, analysis.GetByteLimit(2));
            Assert.AreEqual(245.22, System.Math.Round(analysis.GetLimit(2), 2));
        }

        #endregion

        #region --- UShort Statistics ---

        [TestMethod]
        public void AnalysisEntity16_Init()
        {
            var analysis = new AnalysisEntity16();

            Assert.AreEqual(ushort.MinValue, analysis.Min);
            Assert.AreEqual(ushort.MaxValue, analysis.Max);
            Assert.AreEqual(0, analysis.Sum);
            Assert.AreEqual(0, analysis.SumOfDerivation);
            Assert.AreEqual(0, analysis.Count);
            Assert.AreEqual(0, analysis.Range);

            Assert.AreEqual(0, analysis.Mean);
            Assert.AreEqual(0, analysis.StdDev);
            Assert.AreEqual(0, analysis.StdErr);
            Assert.AreEqual(0, analysis.Variance);

            Assert.AreEqual(0, analysis.CalculateMode());
            Assert.AreEqual(0, analysis.GetLimit(0));

            // INIT 2
            var list = new List<ushort>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            analysis = new AnalysisEntity16();
            analysis.Add(list.ToArray());

            var analysis2 = new AnalysisEntity16(analysis.Min, analysis.Max, analysis.Sum, analysis.SumOfDerivation, analysis.Count);

            Assert.AreEqual(5, analysis2.Min);
            Assert.AreEqual(220, analysis2.Max);
            Assert.AreEqual(695, analysis2.Sum);
            Assert.AreEqual(110525, analysis2.SumOfDerivation);
            Assert.AreEqual(8, analysis2.Count);
            Assert.AreEqual(215, analysis2.Range);

            Assert.AreEqual(86.875, analysis2.Mean);

            Assert.AreEqual(79.173, System.Math.Round(analysis2.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis2.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis2.Variance, 3));

            Assert.AreEqual(analysis2.Mean, analysis.GetLimit(0));
            Assert.AreEqual(analysis2.Mean + analysis2.StdDev, analysis.GetLimit(1));

            Assert.AreEqual(ushort.MinValue, analysis.GetUshortLimit(-200000));
            Assert.AreEqual(ushort.MaxValue, analysis.GetUshortLimit(20000));
        }

        [TestMethod]
        public void AnalysisEntity16_Frequency()
        {
            var analysis = new AnalysisEntity16();
            analysis.Add(100, 10);
            analysis.Add(111, 0);

            Assert.AreEqual(100, analysis.Min);
            Assert.AreEqual(100, analysis.Max);
            Assert.AreEqual(1000, analysis.Sum);
            Assert.AreEqual(10, analysis.Count);

            Assert.AreEqual(10, analysis.Histogram[100]);

            // now add something less than the last
            analysis.Add(10, 10);

            Assert.AreEqual(10, analysis.Min);
            Assert.AreEqual(100, analysis.Max);
            Assert.AreEqual(1100, analysis.Sum);
            Assert.AreEqual(20, analysis.Count);

            Assert.AreEqual(10, analysis.Histogram[10]);

            // now add something more than all
            analysis.Add(200, 5);

            Assert.AreEqual(10, analysis.Min);
            Assert.AreEqual(200, analysis.Max);
            Assert.AreEqual(2100, analysis.Sum);
            Assert.AreEqual(25, analysis.Count);

            Assert.AreEqual(5, analysis.Histogram[200]);
        }

        [TestMethod]
        public void AnalysisEntity16_Basic()
        {
            var list = new List<ushort>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity16();
            analysis.Add(list.ToArray());

            //Verified with https://www.calculator.net/statistics-calculator.html

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);
            Assert.AreEqual(110525, analysis.SumOfDerivation);
            Assert.AreEqual(8, analysis.Count);
            Assert.AreEqual(215, analysis.Range);

            Assert.AreEqual(86.875, analysis.Mean);
            Assert.AreEqual(100, analysis.CalculateMode());

            Assert.AreEqual(79.173, System.Math.Round(analysis.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis.Variance, 3));
        }

        [TestMethod]
        public void AnalysisEntity16_AddAnalysis()
        {
            // populate a root analysis object
            var list = new List<ushort>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var rootAnalysis = new AnalysisEntity16();
            rootAnalysis.Add(list.ToArray());

            // add the root to it and we should get the same expected values since the original was blank
            var analysis = new AnalysisEntity16();
            analysis.Add(rootAnalysis);

            //analysis.Add((AnalysisEntity16)null);
            analysis.Add(new AnalysisEntity16());

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);
            Assert.AreEqual(110525, analysis.SumOfDerivation);
            Assert.AreEqual(8, analysis.Count);
            Assert.AreEqual(215, analysis.Range);

            Assert.AreEqual(86.875, analysis.Mean);
            Assert.AreEqual(100, analysis.CalculateMode());

            Assert.AreEqual(79.173, System.Math.Round(analysis.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis.Variance, 3));

            // should be the same output initalizing it this way as well
            var analysis2 = new AnalysisEntity16(analysis.Min, analysis.Max, analysis.Sum, analysis.SumOfDerivation, analysis.Count);

            Assert.AreEqual(5, analysis2.Min);
            Assert.AreEqual(220, analysis2.Max);
            Assert.AreEqual(695, analysis2.Sum);
            Assert.AreEqual(110525, analysis2.SumOfDerivation);
            Assert.AreEqual(8, analysis2.Count);
            Assert.AreEqual(215, analysis2.Range);

            Assert.AreEqual(86.875, analysis2.Mean);
            //NOTE: MODE WILL NOT COME OVER SINCE WE DONT PULL OVER THE HISTOGRAM
            Assert.AreEqual(79.173, System.Math.Round(analysis2.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis2.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis2.Variance, 3));

            // Add a new min and max and verify
            var analysis3 = new AnalysisEntity16();
            analysis3.Add(new List<ushort>() { 2, 225 }.ToArray());

            analysis2.Add(analysis3);
            Assert.AreEqual(2, analysis2.Min);
            Assert.AreEqual(225, analysis2.Max);
        }

        [TestMethod]
        public void AnalysisEntity16_Clone()
        {
            var list = new List<ushort>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity16();
            analysis.Add(list.ToArray());

            var clone = analysis.Clone();
            Assert.AreEqual(analysis.Min, clone.Min);
            Assert.AreNotSame(analysis.Min, clone.Min);

            Assert.AreEqual(analysis.Max, clone.Max);
            Assert.AreNotSame(analysis.Max, clone.Max);

            Assert.AreEqual(analysis.Sum, clone.Sum);
            Assert.AreNotSame(analysis.Sum, clone.Sum);

            Assert.AreEqual(analysis.Count, clone.Count);
            Assert.AreNotSame(analysis.Count, clone.Count);
        }

        [TestMethod]
        public void AnalysisEntity16_Histogram()
        {
            var list = new List<ushort>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity16();
            analysis.Add(list.ToArray());

            //Verified with https://www.calculator.net/statistics-calculator.html

            Assert.AreEqual(1, analysis.Histogram[5]);
            Assert.AreEqual(1, analysis.Histogram[10]);
            Assert.AreEqual(1, analysis.Histogram[20]);
            Assert.AreEqual(1, analysis.Histogram[40]);
            Assert.AreEqual(2, analysis.Histogram[100]);
            Assert.AreEqual(1, analysis.Histogram[200]);
            Assert.AreEqual(1, analysis.Histogram[220]);

            // ADD HISTOGRAM TEST
            var analysis2 = new AnalysisEntity16(analysis.Histogram);

            Assert.AreEqual(1, analysis2.Histogram[5]);
            Assert.AreEqual(1, analysis2.Histogram[10]);
            Assert.AreEqual(1, analysis2.Histogram[20]);
            Assert.AreEqual(1, analysis2.Histogram[40]);
            Assert.AreEqual(2, analysis2.Histogram[100]);
            Assert.AreEqual(1, analysis2.Histogram[200]);
            Assert.AreEqual(1, analysis2.Histogram[220]);

            // Invalid Histogram size
            Assert.ThrowsException<ArgumentException>(() => analysis2.AddHistogram(new long[20]));
        }

        [TestMethod]
        public void AnalysisEntity16_Limit()
        {
            var list = new List<ushort>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity16();
            analysis.Add(list.ToArray());

            //Verified with https://www.calculator.net/statistics-calculator.html

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);
            
            Assert.AreEqual(245, analysis.GetUshortLimit(2));
        }

        #endregion

        #region --- Long Statistics ---

        [TestMethod]
        public void AnalysisEntity64_Init()
        {
            var analysis = new AnalysisEntity64();

            Assert.AreEqual(double.MinValue, analysis.Min);
            Assert.AreEqual(double.MaxValue, analysis.Max);
            Assert.AreEqual(0, analysis.Sum);
            Assert.AreEqual(0, analysis.SumOfDerivation);
            Assert.AreEqual(0, analysis.Count);
            Assert.AreEqual(0, analysis.Range);

            Assert.AreEqual(0, analysis.Mean);
            Assert.AreEqual(0, analysis.StdDev);
            Assert.AreEqual(0, analysis.StdErr);
            Assert.AreEqual(0, analysis.Variance);

            Assert.AreEqual(0, analysis.GetLimit(0));


            //INIT 2
            var list = new List<double>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            analysis = new AnalysisEntity64();
            analysis.Add(list.ToArray());

            var analysis2 = new AnalysisEntity64(analysis.Min, analysis.Max, analysis.Sum, analysis.SumOfDerivation, analysis.Count);

            Assert.AreEqual(5, analysis2.Min);
            Assert.AreEqual(220, analysis2.Max);
            Assert.AreEqual(695, analysis2.Sum);
            Assert.AreEqual(110525, analysis2.SumOfDerivation);
            Assert.AreEqual(8, analysis2.Count);
            Assert.AreEqual(215, analysis2.Range);

            Assert.AreEqual(86.875, analysis2.Mean);

            Assert.AreEqual(79.173, System.Math.Round(analysis2.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis2.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis2.Variance, 3));

            Assert.AreEqual(analysis2.Mean, analysis.GetLimit(0));
            Assert.AreEqual(analysis2.Mean + analysis2.StdDev, analysis.GetLimit(1));
        }

        [TestMethod]
        public void AnalysisEntity64_Frequency()
        {
            var analysis = new AnalysisEntity64();
            analysis.Add(100, 10);
            analysis.Add(111, 0);

            Assert.AreEqual(100, analysis.Min);
            Assert.AreEqual(100, analysis.Max);
            Assert.AreEqual(1000, analysis.Sum);
            Assert.AreEqual(10, analysis.Count);

            // now add something less than the last
            analysis.Add(10, 10);

            Assert.AreEqual(10, analysis.Min);
            Assert.AreEqual(100, analysis.Max);
            Assert.AreEqual(1100, analysis.Sum);
            Assert.AreEqual(20, analysis.Count);

            // now add something more than all
            analysis.Add(200, 5);

            Assert.AreEqual(10, analysis.Min);
            Assert.AreEqual(200, analysis.Max);
            Assert.AreEqual(2100, analysis.Sum);
            Assert.AreEqual(25, analysis.Count);
        }

        [TestMethod]
        public void AnalysisEntity64_Basic()
        {
            var list = new List<double>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity64();
            analysis.Add(list.ToArray());

            //Verified with https://www.calculator.net/statistics-calculator.html

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);
            Assert.AreEqual(110525, analysis.SumOfDerivation);
            Assert.AreEqual(8, analysis.Count);
            Assert.AreEqual(215, analysis.Range);

            Assert.AreEqual(86.875, analysis.Mean);

            Assert.AreEqual(79.173, System.Math.Round(analysis.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis.Variance, 3));
        }

        [TestMethod]
        public void AnalysisEntity64_AddAnalysis()
        {
            // populate a root analysis object
            var list = new List<double>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var rootAnalysis = new AnalysisEntity64();
            rootAnalysis.Add(list.ToArray());

            // add the root to it and we should get the same expected values since the original was blank
            var analysis = new AnalysisEntity64();
            analysis.Add(rootAnalysis);

            analysis.Add((AnalysisEntity64)null);
            analysis.Add(new AnalysisEntity64());

            Assert.AreEqual(5, analysis.Min);
            Assert.AreEqual(220, analysis.Max);
            Assert.AreEqual(695, analysis.Sum);
            Assert.AreEqual(110525, analysis.SumOfDerivation);
            Assert.AreEqual(8, analysis.Count);
            Assert.AreEqual(215, analysis.Range);

            Assert.AreEqual(86.875, analysis.Mean);

            Assert.AreEqual(79.173, System.Math.Round(analysis.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis.Variance, 3));

            // should be the same output initalizing it this way as well
            var analysis2 = new AnalysisEntity64(analysis.Min, analysis.Max, analysis.Sum, analysis.SumOfDerivation, analysis.Count);

            Assert.AreEqual(5, analysis2.Min);
            Assert.AreEqual(220, analysis2.Max);
            Assert.AreEqual(695, analysis2.Sum);
            Assert.AreEqual(110525, analysis2.SumOfDerivation);
            Assert.AreEqual(8, analysis2.Count);
            Assert.AreEqual(215, analysis2.Range);

            Assert.AreEqual(86.875, analysis2.Mean);
            //NOTE: MODE WILL NOT COME OVER SINCE WE DONT PULL OVER THE HISTOGRAM
            Assert.AreEqual(79.173, System.Math.Round(analysis2.StdDev, 3));
            Assert.AreEqual(27.992, System.Math.Round(analysis2.StdErr, 3));
            Assert.AreEqual(6268.359, System.Math.Round(analysis2.Variance, 3));

            // Add a new min and max and verify
            var analysis3 = new AnalysisEntity64();
            analysis3.Add(new List<double>() { 2, 225 }.ToArray());

            analysis2.Add(analysis3);
            Assert.AreEqual(2, analysis2.Min);
            Assert.AreEqual(225, analysis2.Max);
        }

        [TestMethod]
        public void AnalysisEntity64_Clone()
        {
            var list = new List<double>()
            {
                10, 20, 40, 100, 100, 200, 220, 5
            };

            var analysis = new AnalysisEntity64();
            analysis.Add(list.ToArray());

            var clone = analysis.Clone();
            Assert.AreEqual(analysis.Min, clone.Min);
            Assert.AreNotSame(analysis.Min, clone.Min);

            Assert.AreEqual(analysis.Max, clone.Max);
            Assert.AreNotSame(analysis.Max, clone.Max);

            Assert.AreEqual(analysis.Sum, clone.Sum);
            Assert.AreNotSame(analysis.Sum, clone.Sum);

            Assert.AreEqual(analysis.Count, clone.Count);
            Assert.AreNotSame(analysis.Count, clone.Count);
        }

        #endregion
    }
}
