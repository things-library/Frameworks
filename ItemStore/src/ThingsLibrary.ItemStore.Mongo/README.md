# Starlight Entity Mango

This library wraps the noSQL MongoDB.Driver libaries with a generic CRUD wrapping methods that understand the system attributes such a Key, Index, and PartitionKey

# External Links

[Introduction to MongoDb with .NET part 15: object serialisation](https://dotnetcodr.com/2016/04/25/introduction-to-mongodb-with-net-part-15-object-serialisation-continued/)

# Timestamps

The [TimeStamp] field can leverage a number of different techniques for timestamping from just stamping the DateTime to stamping a EPOCH all depending on the field data type being used.

| Data Type | Stored Value |
| -- | -- |
| DateTime | DateTime.UtcNow |
| DateTimeOffset | DateTimeOffset.UtcNow |
| Short | Day EPOCH - days since 1/1/1970 |
| Long | Seconds EPOCH - seconds since 1/1/1970 |

