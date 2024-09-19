namespace ThingsLibrary.Database.Audit
{
    /// <summary>
    /// Audit Type
    /// </summary>    
    public static class AuditType
    {
        public const char Unknown = default;

        public const char Create = 'C';
        public const char Delete = 'D';
        public const char Update = 'U';
        public const char UnDelete = 'X';

        public const char Query = 'Q';
        public const char QueryHistory = 'H';
    }
}
