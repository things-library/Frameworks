using ThingsLibrary.Interfaces;

namespace ThingsLibrary.Database.Base
{
    public interface IEntityBase : IId, IPartitionKey, IRevisionNumber, ILastEditDate
    {        
        /// <summary>
        /// Record Created DateTime (UTC)
        /// </summary>
        public DateTimeOffset CreatedOn { get; }
    }
}
