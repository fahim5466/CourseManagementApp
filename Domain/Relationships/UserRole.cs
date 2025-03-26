using Domain.Entities;

namespace Domain.Relationships
{
    public class UserRole
    {
        #region Properties
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        #endregion

        #region Navigation properties
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
        #endregion
    }
}
