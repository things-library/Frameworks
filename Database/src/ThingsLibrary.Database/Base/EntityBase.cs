using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThingsLibrary.Attributes;
using ThingsLibrary.DataType;

namespace ThingsLibrary.Database.Base
{
    public class EntityBase : IEntityBase
    {
        [PartitionKey]        
        public string PartitionKey { get; set; } = string.Empty;

        [Key]
        public Guid Id { get; set; } = SequentialGuid.NewGuid();

        [RevisionNumber]
        public int Version { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    }
}
