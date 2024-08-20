namespace ThingsLibrary.Storage
{
    /// <summary>
    /// Storage System Type
    /// </summary>
    public enum CloudFileStoreType : byte
    {
        Azure_Blob = 1,
        AWS_S3 = 2,
        GCP_Storage = 3,
        Wasabi = 4,
        Local = 5
    }
}
