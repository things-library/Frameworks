// ================================================================================
// <copyright file="UnitConvert.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class UnitConvertTests
    {
        #region --- Basic Unit Conversions ---

        [TestMethod]
        [DataRow(1.25, "m2", "m2", 1.25)]
        [DataRow(1.25, "sq m", "sq m", 1.25)]
        [DataRow(1.25, "m2", "sq m", 1.25)]
        [DataRow(1.25, "sq m", "m2", 1.25)]
        [DataRow(12557.5, "m2", "ac", 3.1031)]
        [DataRow(12557.5, "m2", "ha", 1.2557)]
        [DataRow(1.25, "m2", "sq ft", 13.4549)]
        [DataRow(1.25, "m2", "ft2", 13.4549)]

        [DataRow(1.25, "ha", "ha", 1.25)]
        [DataRow(1.25, "ha", "m2", 12500)]
        [DataRow(1.25, "ha", "sq m", 12500)]
        [DataRow(1.25, "ha", "sq ft", 134548.877)]
        [DataRow(1.25, "ha", "ft2", 134548.877)]
        [DataRow(1.25, "ha", "ac", 3.0888)]        

        [DataRow(1.25, "ft", "ft", 1.25)]
        [DataRow(10.25, "ft", "in", 123)]
        [DataRow(1.25, "ft", "cm", 38.1)]
        [DataRow(1.25, "ft", "m", 0.381)]
        [DataRow(10254.25, "ft", "km", 3.1255)]
        [DataRow(10.25, "ft", "yd", 3.4167)]
        [DataRow(125658.25, "ft", "mi", 23.7984)]

        [DataRow(123.56, "lb", "lb", 123.56)]
        [DataRow(123.56, "lbs", "lbs", 123.56)]
        [DataRow(123.56, "lb", "lbs", 123.56)]
        [DataRow(123.56, "lbs", "lb", 123.56)]
        [DataRow(123.56, "lb", "kg", 56.0459)]
        [DataRow(123.56, "lbs", "kg", 56.0459)]

        [DataRow(123.56, "mph", "mph", 123.56)]
        [DataRow(123.56, "mph", "mi/hr", 123.56)]
        [DataRow(123.56, "mph", "mi/h", 123.56)]
        [DataRow(123.56, "mph", "km/hr", 198.8505)]
        [DataRow(123.56, "mph", "km/h", 198.8505)]
        [DataRow(123.56, "mph", "km1hr-1", 198.8505)]

        [DataRow(123.56, "km/h", "km/hr", 123.56)]
        [DataRow(123.56, "km/h", "km/h", 123.56)]
        [DataRow(123.56, "km/h", "km1hr-1", 123.56)]
        [DataRow(123.56, "km/h", "mph", 76.7766)]
        [DataRow(123.56, "km/h", "mi/hr", 76.7766)]
        [DataRow(123.56, "km/h", "mi/h", 76.7766)]

        [DataRow(123.56, "[m3]1ha-1", "m3/ha", 123.56)]   //unit convert
        [DataRow(123.56, "m3/ha", "[m3]1ha-1", 123.56)]   //unit convert
        [DataRow(123.56, "m3/ha", "bu/ac", 1418.9631)]   

        [DataRow(10.25, "seeds/ha", "seeds1ha-1", 10.25)]   //unit convert
        [DataRow(10.25, "seeds1ha-1", "seeds/ha", 10.25)]   //unit convert
        [DataRow(10.25, "seeds/ha", "seeds/ac", 25.3283)]

        [DataRow(10.25, "l/ha", "l1ha-1", 10.25)]
        [DataRow(10.25, "l1ha-1", "l/ha", 10.25)]
        [DataRow(10.25, "l/ha", "gal/ac", 1.0958)]
        public void ConvertUnitValue(double sourceValue, string sourceUnits, string destinationUnits, double expectedValue)
        {            
            var outputValue = UnitConvert.ConvertUnitValue(sourceValue, sourceUnits, destinationUnits);
                        
            Assert.AreEqual(expectedValue, System.Math.Round(outputValue, 4));            
        }

        [TestMethod]
        public void ConvertUnitValue_Exceptions()
        {
            Assert.Throws<ArgumentNullException>(() => UnitConvert.ConvertUnitValue(7, null, "km"));
            Assert.Throws<ArgumentNullException>(() => UnitConvert.ConvertUnitValue(7, "km", null));

            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "bad", "km"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "m2", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "ha", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "ft", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "lbs", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "mph", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "km/h", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "m3/ha", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "seeds/ha", "BAD"));
            Assert.Throws<ArgumentException>(() => UnitConvert.ConvertUnitValue(7, "l/ha", "BAD"));
        }

        #endregion

        #region --- Epoch Time ---

        [TestMethod]
        [DataRow(1581984000, "2020-02-18T00:00:00.0000000")]
        [DataRow(1641081600, "2022-01-02T00:00:00.0000000")]
        [DataRow(1641092645, "2022-01-02T03:04:05.0000000")]
        [DataRow(17517175500, "2525-02-05T02:05:00.0000000")]
        public void FromUnixTime(long testValue, string expectedValue)
        {
            var expectedDateTime = DateTime.Parse(expectedValue);
            Assert.AreEqual(expectedDateTime, UnitConvert.FromUnixTime(testValue));
        }

        [TestMethod]
        [DataRow("2020-02-18T00:00:00.0000000", 1581984000)]
        [DataRow("2022-01-02T00:00:00.0000000", 1641081600)]
        [DataRow("2022-01-02T03:04:05.0000000", 1641092645)]
        [DataRow("2525-02-05T02:05:00.0000000", 17517175500)]
        public void ToUnixTime(string dateTimeStr, long expectedValue)
        {
            var dateTime = DateTime.Parse(dateTimeStr);
            Assert.AreEqual(expectedValue, UnitConvert.ToUnixTime(dateTime));
        }

        [TestMethod]
        [DataRow("2020-02-18T00:00:00.0000000")]
        [DataRow("2022-01-02T00:00:00.0000000")]
        [DataRow("2022-01-02T03:04:05.0000000")]
        [DataRow("2525-02-05T02:05:00.0000000")]
        public void Epoch_Agreement(string dateTimeStr)
        {
            var dateTime = DateTime.Parse(dateTimeStr);

            var epoch = UnitConvert.ToUnixTime(dateTime);

            Assert.AreEqual(dateTime, UnitConvert.FromUnixTime(epoch));
        }

        [TestMethod]
        [DataRow(1581984000)]
        [DataRow(1641081600)]
        [DataRow(1641092645)]
        [DataRow(17517175500)]
        public void Epoch_AgreementReverse(long epoch)
        {
            var dateTime = UnitConvert.FromUnixTime(epoch);

            Assert.AreEqual(epoch, UnitConvert.ToUnixTime(dateTime));
        }

        [TestMethod]
        [DataRow("1969-01-01T00:00:00.0000000")]
        public void Epoch_BadData(string dateTimeStr)
        {
            var dateTime = DateTime.Parse(dateTimeStr);

            Assert.Throws<ArgumentException>(() => UnitConvert.ToUnixTime(dateTime));

            //Assert.AreEqual(dateTime, UnitConvert.FromUnixTime(epoch));
        }

        #endregion

        #region --- Epoch Day ---

        [TestMethod]
        [DataRow(18310, "2020-02-18T00:00:00.0000000")]
        [DataRow(18994, "2022-01-02T00:00:00.0000000")]
        [DataRow(32767, "2059-09-18T00:00:00.0000000")]        
        public void FromUnixDay(int testValue, string expectedValue)
        {
            var expectedDateTime = DateTime.Parse(expectedValue);
            Assert.AreEqual(expectedDateTime, UnitConvert.FromUnixDay(Convert.ToInt16(testValue)));
        }

        [TestMethod]
        [DataRow("2020-02-18T00:00:00.0000000", 18310)]  
        [DataRow("2022-01-02T00:00:00.0000000", 18994)]
        [DataRow("2059-09-18T00:00:00.0000000", 32767)]
        public void ToUnixDay(string dateTimeStr, long expectedValue)
        {
            var dateTime = DateTime.Parse(dateTimeStr);
            Assert.AreEqual(expectedValue, UnitConvert.ToUnixDay(dateTime));
        }

        [TestMethod]
        [DataRow("2020-02-18T00:00:00.0000000")]
        [DataRow("2022-01-02T00:00:00.0000000")]
        [DataRow("2059-09-18T00:00:00.0000000")]
        public void EpochDay_Agreement(string dateTimeStr)
        {
            var dateTime = DateTime.Parse(dateTimeStr);

            var epoch = UnitConvert.ToUnixDay(dateTime);

            Assert.AreEqual(dateTime, UnitConvert.FromUnixDay(epoch));
        }

        [TestMethod]
        [DataRow(18310)]
        [DataRow(18994)]
        [DataRow(32767)]
        public void EpochDay_AgreementReverse(int epoch)
        {            
            var dateTime = UnitConvert.FromUnixDay(Convert.ToInt16(epoch));

            Assert.AreEqual(epoch, UnitConvert.ToUnixDay(dateTime));
        }

        //[DataTestMethod, ExpectedException(typeof(ArgumentException))]
        //[DataRow("2059-09-19T00:00:00.0000000")] //short + 1
        //[DataRow("2079-08-21T00:00:00.0000000")]
        //[DataRow("1969-01-01T00:00:00.0000000")]
        //public void EpochDay_BadData(string dateTimeStr)
        //{
        //    var dateTime = DateTime.Parse(dateTimeStr);
        //    var epoch = UnitConvert.ToUnixDay(dateTime);

        //    Assert.AreEqual(dateTime, UnitConvert.FromUnixDay(epoch));
        //}

        #endregion

        #region --- Hex Strings ---

        [TestMethod]
        [DataRow("68656c6c6f", "hello")]
        [DataRow("737472696e67", "string")]
        [DataRow("48656c6c6f20576f726c64", "Hello World")]
        [DataRow("54686520616e7377657220697320343221", "The answer is 42!")]
        public void ConvertHexStringToByteArray(string testValue, string expectedValue)
        {
            var bytes = ThingsLibrary.DataType.UnitConvert.ConvertHexStringToByteArray(testValue);
            var str = System.Text.Encoding.Default.GetString(bytes);

            Assert.AreEqual(expectedValue, str);            
        }

        [TestMethod]
        public void ConvertHexStringToByteArray_BadData()
        {
            //BAD DATA 
            Assert.Throws<ArgumentException>(() => ThingsLibrary.DataType.UnitConvert.ConvertHexStringToByteArray("123"));
            Assert.Throws<ArgumentException>(() => ThingsLibrary.DataType.UnitConvert.ConvertHexStringToByteArray("12345"));
        }

        #endregion
    }
}
