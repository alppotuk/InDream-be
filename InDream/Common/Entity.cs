using System.ComponentModel.DataAnnotations;

namespace InDream.Interfaces
{
    public class Entity
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreationDateUtc { get; set; }

       

        public Entity()
        {
            CreationDateUtc = DateTime.UtcNow;
        }
    }
}
