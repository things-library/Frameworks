using Starlight.Attributes;

namespace Starlight.Entity.Tests.Integration.Mongo.TestData
{
    public class TestClass
    {
        [Key]
        public string Id { get; set; }

        [PartitionKey]
        public string PartitionKey { get; set; }

        public Guid GuidValue { get; set; }
        public Guid? GuidNullibleValue { get; set; }

        // 8bit
        //public byte ByteValue { get; set; }
        //public byte? ByteNullibleValue { get; set; }

        // 16bit
        //public short ShortValue { get; set; }
        //public short? ShortNullibleValue { get; set; }

        // 32bit
        public int IntValue { get; set; }
        public int? IntNullibleValue { get; set; }

        // 64bit
        public long LongValue { get; set; }
        public long? LongNullibleValue { get; set; }

        public double DoubleValue { get; set; }
        public double? DoubleNullibleValue { get; set; }

        public float FloatValue { get; set; }
        public float? FloatNullibleValue { get; set; }

        // ASCII
        public string StringValue { get; set; }

        // BOOL
        public bool BoolValue { get; set; }
        public bool? BoolNullibleValue { get; set; }

        // DATE 
        public DateTime DateTimeValue { get; set; }
        public DateTime? DateTimeNullibleValue { get; set; }

        public DateTimeOffset DateTimeOffsetValue { get; set; }
        public DateTimeOffset? DateTimeOffsetNullibleValue { get; set; }


        public static TestClass Get() => new TestClass
        {
            PartitionKey = "Partition1",
            Id = new Guid("99999999-9999-9999-9999-999999999991").ToString(),
            
            GuidValue = new Guid("99999999-9999-9999-9999-999999999991"),
            GuidNullibleValue = new Guid("99999999-9999-9999-9999-999999999992"),

            //ByteValue = 12,
            //ByteNullibleValue = 13,

            //ShortValue = 123,
            //ShortNullibleValue = 1234,

            IntValue = 99999991,
            IntNullibleValue = 99999992,

            LongValue = 123456789012,
            LongNullibleValue = 123456789013,

            DoubleValue = 2345678890.12345,
            DoubleNullibleValue = 345678890.2345,

            FloatValue = 345678890.12345f,
            FloatNullibleValue = 45678890.2345f,

            StringValue = "String1234567890",

            BoolValue = true,
            BoolNullibleValue = false,

            DateTimeValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),    //UTC kind required
            DateTimeNullibleValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),    //UTC kind required

            DateTimeOffsetValue = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2)),
            DateTimeOffsetNullibleValue = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(5))
        };

        public static TestClass GetNulled() => new TestClass
        {
            PartitionKey = "Partition2",
            Id = new Guid("99999999-9999-9999-9999-999999999991").ToString(),
            
            GuidValue = new Guid("99999999-9999-9999-9999-999999999991"),
            GuidNullibleValue = null,

            //ByteValue = 12,
            //ByteNullibleValue = null,

            //ShortValue = 123,
            //ShortNullibleValue = null,

            IntValue = 99999991,
            IntNullibleValue = null,

            LongValue = 123456789012,
            LongNullibleValue = null,

            DoubleValue = 2345678890.12345,
            DoubleNullibleValue = null,

            FloatValue = 345678890.12345f,
            FloatNullibleValue = null,

            StringValue = null,

            BoolValue = true,
            BoolNullibleValue = null,

            DateTimeValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
            DateTimeNullibleValue = null,

            DateTimeOffsetValue = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2)),
            DateTimeOffsetNullibleValue = null,
        };
    }

    [ExcludeFromCodeCoverage]
    public class TestInheritedClass : TestClass
    {
        public int SecondIntValue { get; set; }

        public string SecondStringValue { get; set; }

        public DateTime? SecondDateTimeNullibleValue { get; set; }


        public static TestInheritedClass GetInherited() => new TestInheritedClass
        {
            PartitionKey = "Partition3",
            Id = new Guid("99999999-9999-9999-9999-999999999991").ToString(),

            GuidValue = new Guid("99999999-9999-9999-9999-999999999991"),
            GuidNullibleValue = new Guid("99999999-9999-9999-9999-999999999992"),

            //ByteValue = 12,
            //ByteNullibleValue = 13,

            //ShortValue = 123,
            //ShortNullibleValue = 1234,

            IntValue = 99999991,
            IntNullibleValue = 99999992,

            LongValue = 123456789012,
            LongNullibleValue = 123456789013,

            DoubleValue = 2345678890.12345,
            DoubleNullibleValue = 345678890.2345,

            FloatValue = 345678890.12345f,
            FloatNullibleValue = 45678890.2345f,

            StringValue = "String1234567890",

            BoolValue = true,
            BoolNullibleValue = false,

            DateTimeValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
            DateTimeNullibleValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),

            DateTimeOffsetValue = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2)),
            DateTimeOffsetNullibleValue = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(5)),

            SecondStringValue = "Gamgee",
            SecondIntValue = 4321,
            SecondDateTimeNullibleValue = new DateTime(1999, 1, 2, 3, 4, 5, DateTimeKind.Utc)
        };
    }
}
