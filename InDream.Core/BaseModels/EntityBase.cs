using System.ComponentModel.DataAnnotations;

namespace InDream.Core.BaseModels
{
    public class EntityBase
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreationDateUtc { get; set; }

       

        public EntityBase()
        {
            CreationDateUtc = DateTime.UtcNow;
        }
    }
}
