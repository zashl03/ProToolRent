using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProToolRent.Domain.Entities
{
    public class Tool
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Price { get; set; }
        public Category Category { get; set; } = null!;
        public User User { get; set; } = null!;

    }
}
