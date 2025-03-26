namespace Domain.Entities
{
    public class Class
    {
        #region Constants
        public const int NAME_MIN_LENGTH = 3;
        public const string NAME_MIN_LENGTH_STR = "3";
        public const int NAME_MAX_LENGTH = 50;
        public const string NAME_MAX_LENGTH_STR = "50";
        #endregion

        #region Properties
        public Guid Id { get; set; }
        public required string Name { get; set; }
        #endregion

        #region Navigation properties
        public List<Course> Courses { get; set; } = [];
        #endregion
    }
}
