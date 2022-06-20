using System;

namespace Monovi.Model.Concrete
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public decimal IsDeleted { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
