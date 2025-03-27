using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Relationships
{
    public class UserRole
    {
        #region Properties
        [Key, Column(Order = 0)]
        public Guid UserId { get; set; }

        [Key, Column(Order = 1)]
        public Guid RoleId { get; set; }
        #endregion

        #region Navigation properties
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
        #endregion
    }
}
