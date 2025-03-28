﻿using Domain.Entities;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Relationships
{
    public class ClassEnrollment : IAuditable
    {
        #region Properties
        [Key, Column(Order = 0)]
        public Guid ClassId { get; set; }

        [Key, Column(Order = 1)]
        public Guid StudentId { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        #endregion

        #region Navigation properties
        public Class Class { get; set; } = null!;
        public User Student { get; set; } = null!;
        #endregion
    }
}
