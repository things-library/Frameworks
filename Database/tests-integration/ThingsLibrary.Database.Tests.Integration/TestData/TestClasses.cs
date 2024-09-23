// ================================================================================
// <copyright file="TestClasses.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Database.Base;
using ThingsLibrary.DataType;


namespace ThingsLibrary.Database.Tests.Integration.TestData
{
    [ExcludeFromCodeCoverage]
    [Table("test_class"), Index(nameof(IndexProperty), IsUnique = true)]    
    public class TestClass : EntityBase
    {
        //[PartitionKey]
        //public string PartitionKey { get; set; } = string.Empty;

        //[Key]
        //public Guid Id { get; set; } = SequentialGuid.NewGuid();

        //[Timestamp]        
        //public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        [Column("guid_nullable_value")]
        public Guid? GuidNullableValue { get; set; }

        // 8bit
        [Column("byte_value")]
        public byte ByteValue { get; set; }
        
        [Column("byte_nullable_value")]
        public byte? ByteNullableValue { get; set; }

        // 16bit
        [Column("short_value")]
        public short ShortValue { get; set; }

        [Column("short_nullable_value")]
        public short? ShortNullableValue { get; set; }

        // 32bit
        [Column("int_value")]
        public int IntValue { get; set; }
        
        [Column("int_nullable_value")]
        public int? IntNullableValue { get; set; }

        // 64bit
        [Column("long_value")]
        public long LongValue { get; set; }
        
        [Column("long_nullable_value")]
        public long? LongNullableValue { get; set; }

        [Column("double_value")]
        public double DoubleValue { get; set; }

        [Column("double_nullable_value")]
        public double? DoubleNullableValue { get; set; }

        [Column("float_value")]
        public float FloatValue { get; set; }

        [Column("float_nullable_value")]
        public float? FloatNullableValue { get; set; }

        // ASCII
        [Column("string_value")]
        public string StringValue { get; set; } = string.Empty;

        [Column("string_nullable_value")]
        public string? StringNullableValue { get; set; }

        // BOOL
        [Column("bool_value")]
        public bool BoolValue { get; set; }

        [Column("bool_nullable_value")]
        public bool? BoolNullableValue { get; set; }

        // DATE 
        [Column("datetime_value")]
        public DateTime DateTimeValue { get; set; }

        [Column("datetime_nullable_value")]
        public DateTime? DateTimeNullableValue { get; set; }

        [Column("datetimeoffset_value")]
        public DateTimeOffset DateTimeOffsetValue { get; set; }

        [Column("datetimeoffset_nullable_value")]
        public DateTimeOffset? DateTimeOffsetNullableValue { get; set; }

        [Column("index_field")]
        public Guid IndexProperty { get; set; } = SequentialGuid.NewGuid();


        public static TestClass Get() => new TestClass
        {
            PartitionKey = "Partition1",
            Id = new Guid("99999999-9999-9999-9999-999999999991"), //.ToString(),

            GuidNullableValue = new Guid("99999999-9999-9999-9999-999999999992"),

            //ByteValue = 12,
            //ByteNullibleValue = 13,

            //ShortValue = 123,
            //ShortNullibleValue = 1234,

            IntValue = 99999991,
            IntNullableValue = 99999992,

            LongValue = 123456789012,
            LongNullableValue = 123456789013,

            DoubleValue = 2345678890.12345,
            DoubleNullableValue = 345678890.2345,

            FloatValue = 345678890.12345f,
            FloatNullableValue = 45678890.2345f,

            StringValue = "String1234567890",

            BoolValue = true,
            BoolNullableValue = false,

            DateTimeValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),    //UTC kind required
            DateTimeNullableValue = new DateTime(2001, 1, 2, 3, 4, 5, DateTimeKind.Utc),    //UTC kind required

            DateTimeOffsetValue = new DateTimeOffset(2002, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2)),
            DateTimeOffsetNullableValue = new DateTimeOffset(2003, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(5))
        };

        public static TestClass GetNulled() => new TestClass
        {
            PartitionKey = "Partition2",
            Id = new Guid("99999999-9999-9999-9999-999999999991"), //.ToString(),

            GuidNullableValue = null,

            //ByteValue = 12,
            //ByteNullibleValue = null,

            //ShortValue = 123,
            //ShortNullibleValue = null,

            IntValue = 99999991,
            IntNullableValue = null,

            LongValue = 123456789012,
            LongNullableValue = null,

            DoubleValue = 2345678890.12345,
            DoubleNullableValue = null,

            FloatValue = 345678890.12345f,
            FloatNullableValue = null,

            StringValue = null,

            BoolValue = true,
            BoolNullableValue = null,

            DateTimeValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
            DateTimeNullableValue = null,

            DateTimeOffsetValue = new DateTimeOffset(2002, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2)),
            DateTimeOffsetNullableValue = null
        };
    }

    [ExcludeFromCodeCoverage]
    [Table("test_inherited_class")]
    public class TestInheritedClass : TestClass
    {
        public int SecondIntValue { get; set; }

        public string SecondStringValue { get; set; } = string.Empty;

        public DateTime? SecondDateTimeNullibleValue { get; set; }


        public static TestInheritedClass GetInherited() => new TestInheritedClass
        {
            PartitionKey = "Partition3",
            Id = new Guid("99999999-9999-9999-9999-999999999991"), //.ToString(),

            GuidNullableValue = new Guid("99999999-9999-9999-9999-999999999992"),

            //ByteValue = 12,
            //ByteNullibleValue = 13,

            //ShortValue = 123,
            //ShortNullibleValue = 1234,

            IntValue = 99999991,
            IntNullableValue = 99999992,

            LongValue = 123456789012,
            LongNullableValue = 123456789013,

            DoubleValue = 2345678890.12345,
            DoubleNullableValue = 345678890.2345,

            FloatValue = 345678890.12345f,
            FloatNullableValue = 45678890.2345f,

            StringValue = "String1234567890",

            BoolValue = true,
            BoolNullableValue = false,

            DateTimeValue = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc),
            DateTimeNullableValue = new DateTime(2001, 1, 2, 3, 4, 5, DateTimeKind.Utc),

            DateTimeOffsetValue = new DateTimeOffset(2002, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2)),
            DateTimeOffsetNullableValue = new DateTimeOffset(2003, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(5)),

            SecondStringValue = "Gamgee",
            SecondIntValue = 4321,
            SecondDateTimeNullibleValue = new DateTime(1999, 1, 2, 3, 4, 5, DateTimeKind.Utc),

            IndexProperty = SequentialGuid.NewGuid()
        };
    }
}
