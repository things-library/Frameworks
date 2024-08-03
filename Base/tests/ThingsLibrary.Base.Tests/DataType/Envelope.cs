using ThingsLibrary.DataType;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class EnvelopeTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //nothing
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_BadPoints()
        {
            var envelope = new Envelope(new List<(double, double)>
            {
                new (30, 40),
                new (31.6, 41.2),
                new (32.2, 44.1),
                new (32.3, 45.3),
                new (34.6, 46.3),
                new (34.7, 43.2),
                new (35.1, 46.1),
                new (35.2, 48.5),
                new (39.2, 48.6)                
            }, 4326);

            envelope.Add((40, 50), 9999);
        }

        [TestMethod]
        public void Constructor_BaseState()
        {
            var shape = new Envelope();

            Assert.AreEqual(double.MaxValue, shape.MinX);
            Assert.AreEqual(double.MinValue, shape.MaxX);
            Assert.AreEqual(double.MaxValue, shape.MinY);
            Assert.AreEqual(double.MinValue, shape.MaxY);
            Assert.AreEqual(0, shape.SRID);

            Assert.AreEqual(0, shape.CenterX);
            Assert.AreEqual(0, shape.CenterY);
            
            Assert.AreEqual(0, shape.Width);
            Assert.AreEqual(0, shape.Height);
        }
        
        [TestMethod]
        public void Add_Coordinates()
        {
            var shape = new Envelope(new List<(double, double)>
            {
                new (30, 40),
                new (31.6, 41.2),
                new (32.2, 44.1),
                new (32.3, 45.3),
                new (34.6, 46.3),
                new (34.7, 43.2),
                new (35.1, 46.1),
                new (35.2, 48.5),
                new (39.2, 48.6),
                new (40, 50),
            }, 4326);

            Assert.AreEqual(30, shape.MinX);
            Assert.AreEqual(40, shape.MaxX);
            Assert.AreEqual(40, shape.MinY);
            Assert.AreEqual(50, shape.MaxY);
            Assert.AreEqual(4326, shape.SRID);

            Assert.AreEqual(10, shape.Width);
            Assert.AreEqual(10, shape.Height);

            Assert.AreEqual(35, shape.CenterX);
            Assert.AreEqual(45, shape.CenterY);
        }

        [TestMethod]
        public void Add_EachCoordinates()
        {
            var shape = new Envelope();

            var coordinates = new List<(double, double)>
            {
                new (30, 40),
                new (31.6, 41.2),
                new (32.2, 44.1),
                new (32.3, 45.3),
                new (34.6, 46.3),
                new (34.7, 43.2),
                new (35.1, 46.1),
                new (35.2, 48.5),
                new (39.2, 48.6),
                new (40, 50),
            };

            // now add the data
            coordinates.ForEach(coordinate => shape.Add(coordinate, 4326));
            
            Assert.AreEqual(30, shape.MinX);
            Assert.AreEqual(40, shape.MaxX);
            Assert.AreEqual(40, shape.MinY);
            Assert.AreEqual(50, shape.MaxY);
            Assert.AreEqual(4326, shape.SRID);

            Assert.AreEqual(10, shape.Width);
            Assert.AreEqual(10, shape.Height);

            Assert.AreEqual(35, shape.CenterX);
            Assert.AreEqual(45, shape.CenterY);
        }

        [TestMethod]
        public void Add_Envelope()
        {
            var shape = new Envelope(new List<(double, double)>
            {
                new (30, 40),
                new (31.6, 41.2),
                new (32.2, 44.1),
                new (32.3, 45.3),
                new (34.6, 46.3)
            }, 4326);
       
            // add second envelope
            var envelope2 = new Envelope(new List<(double, double)>
            {
                new (34.7, 43.2),
                new (35.1, 46.1),
                new (35.2, 48.5),
                new (39.2, 48.6),
                new (40, 50),
            }, 4326);

            shape.Add(envelope2);

            Assert.AreEqual(30, shape.MinX);
            Assert.AreEqual(40, shape.MaxX);
            Assert.AreEqual(40, shape.MinY);
            Assert.AreEqual(50, shape.MaxY);
            Assert.AreEqual(4326, shape.SRID);

            Assert.AreEqual(10, shape.Width);
            Assert.AreEqual(10, shape.Height);

            Assert.AreEqual(35, shape.CenterX);
            Assert.AreEqual(45, shape.CenterY);
        }

        [TestMethod]
        public void Contains()
        {
            var shape = new Envelope();

            // create a bounds of 30.1 to 39.4, 40.1 to 49.1
            shape.Add(new List<(double,double)>
            {
                new (30.1, 40.1),
                new (31.6, 41.2),
                new (32.2, 44.1),
                new (32.3, 45.3),
                new (34.6, 46.3),
                new (34.7, 43.2),
                new (35.1, 46.1),
                new (35.2, 48.5),
                new (39.2, 48.6),
                new (39.4, 49.1),
            }, 4326);

            // now add the data            
            Assert.IsTrue(shape.Contains(new (31, 41)));
            Assert.IsTrue(shape.Contains(new (39, 49)));
            Assert.IsTrue(shape.Contains(new (30.1, 40.1)));
            Assert.IsTrue(shape.Contains(new (39.1, 49.1)));

            Assert.IsFalse(shape.Contains(new (30, 40)));
            Assert.IsFalse(shape.Contains(new (40, 50)));
            Assert.IsFalse(shape.Contains(new (30.09, 41)));
            Assert.IsFalse(shape.Contains(new (39.11, 51)));
        }
    }
}
