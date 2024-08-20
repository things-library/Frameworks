namespace ThingsLibrary.Entity.Types;

/// <summary>
/// Different types of entities stores that are suppored
/// </summary>
public enum EntityStoreType : short
{
    Azure_Table = 1,
    GCP_DataStore = 2,
    GCP_BigTable = 3,
    Local = 4,
    LocalFiles = 5,  // File support
    MongoDb = 6,
    Azure_Cosmos = 7
}
