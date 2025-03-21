using System.ComponentModel.Design.Serialization;

namespace Domain.Entities.Roles
{
    public class Role
    {
        #region Constants
        public const string ADMIN = "Admin";
        public const string STAFF = "Staff";
        public const string STUDENT = "Student";

        public const int NAME_MAX_LENGTH = 50;
        #endregion

        #region Properties
        public Guid Id { get; set; }
        public string Name { get; set; }
        #endregion
    }
}
