using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProToolRent.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public Role Role { get; set; } = null!;
    }
}
