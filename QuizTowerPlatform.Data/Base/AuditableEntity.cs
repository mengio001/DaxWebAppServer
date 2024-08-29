using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizTowerPlatform.Data.Base
{
    public abstract class AuditableEntity : IConcurrencyAware
    {
        [Key]
        [Column("ID", Order = 0)]
        public int Id { get; set; }

        [Column("CREATED", Order = 1)]
        public virtual DateTime Created { get; set; }

        [Column("CREATEDBY", Order = 2), MaxLength(100)]
        public virtual string CreatedBy { get; set; }

        [Column("CHANGED", Order = 3)]
        public virtual DateTime Changed { get; set; }

        [Column("CHANGEDBY", Order = 4), MaxLength(100)]
        public virtual string ChangedBy { get; set; }

        [ConcurrencyCheck]
        [Column("ConcurrencyStamp", Order = 5)]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
