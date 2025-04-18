﻿using System.ComponentModel.DataAnnotations;
using static Domain.Entities.Course;

namespace Application.DTOs.Course
{
    public class CourseRequestDto
    {
        public const string NAME_REQ_ERR_MSG = "Name is required";
        public const string NAME_MINLEN_ERR_MSG = $"Name cannot be less than {NAME_MIN_LENGTH_STR} characters";
        public const string NAME_MAXLEN_ERR_MSG = $"Name cannot be more than {NAME_MAX_LENGTH_STR} characters";
        public const string NAME_ALPHNUM_ERR_MSG = $"Name can only contain alpha-numeric characters";
        public const string CLASS_IDS_REQ_ERR_MSG = $"Class ids are required";

        [Required(ErrorMessage = NAME_REQ_ERR_MSG)]
        [MinLength(NAME_MIN_LENGTH, ErrorMessage = NAME_MINLEN_ERR_MSG)]
        [MaxLength(NAME_MAX_LENGTH, ErrorMessage = NAME_MAXLEN_ERR_MSG)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = NAME_ALPHNUM_ERR_MSG)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = CLASS_IDS_REQ_ERR_MSG)]
        public List<string> ClassIds { get; set; } = [];

        public void Preprocess()
        {
            Name = Name.Trim();
        }
    }
}
