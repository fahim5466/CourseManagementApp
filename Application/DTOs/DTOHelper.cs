using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.DTOs.User;

namespace Application.DTOs
{
    public static class DTOHelper
    {
        public static UserResponseDto ToUserResponseDto(this Domain.Entities.User user)
        {
            return new()
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified
            };
        }

        public static CourseResponseDto ToCourseResponseDto(this Domain.Entities.Course course)
        {
            return new()
            {
                Id = course.Id.ToString(),
                Name = course.Name
            };
        }

        public static CourseResponseDtoWithClasses ToCourseResponseDtoWithClasses(this Domain.Entities.Course course)
        {
            return new()
            {
                Id = course.Id.ToString(),
                Name = course.Name,
                ClassNames = course.Classes.Select(c => c.Name).ToList()
            };
        }

        public static ClassResponseDto ToClassResponseDto(this Domain.Entities.Class clss)
        {
            return new()
            {
                Id = clss.Id.ToString(),
                Name = clss.Name
            };
        }
    }
}
