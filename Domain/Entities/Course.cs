﻿namespace Domain.Entities
{
    public class Course
    {
        #region Constants
        public const int NAME_MIN_LENGTH = 3;
        public const int NAME_MAX_LENGTH = 50;
        #endregion

        #region Properties
        public Guid Id { get; set; }
        public required string Name { get; set; }
        #endregion

        #region Navigation properties
        public List<Class> Classes { get; set; } = [];
        #endregion
    }
}
