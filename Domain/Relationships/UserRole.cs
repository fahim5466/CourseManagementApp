using Domain.Entities.Roles;
using Domain.Entities.Users;

namespace Domain.Relationships
{
    public class UserRole
    {
        #region Properties
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        #endregion

        #region Navigation properties
        public User User { get; set; }
        public Role Role { get; set; }
        #endregion
    }
}
