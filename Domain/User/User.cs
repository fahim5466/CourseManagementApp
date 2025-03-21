using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public class User
    {
        public Guid Id { get; private set; }

        public User()
        {
            Id = Guid.NewGuid();
        }

        public User(Guid id)
        {
            Id = id;
        }
    }
}
