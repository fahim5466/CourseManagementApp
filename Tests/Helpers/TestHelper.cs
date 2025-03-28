﻿using AutoFixture;
using AutoFixture.Dsl;
using Domain.Entities;
using FluentAssertions;
using static Application.Helpers.ResultHelper;

namespace Tests.Helpers
{
    public static class TestHelper
    {
        public static void TestSuccess(Result result)
        {
            result.IsSuccessful.Should().BeTrue();
        }

        public static void TestSuccess<TValue>(Result<TValue> result)
        {
            result.IsSuccessful.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }

        public static void TestError<TError>(Result result)
        {
            result.IsSuccessful.Should().BeFalse();
            result.ProblemDetails.Should().NotBeNull();
            result.StatusCode.Should().Be(result.ProblemDetails.Status);
            result.ProblemDetails.Should().BeAssignableTo<TError>();
        }

        public static void TestError<TValue, TError>(Result<TValue> result)
        {
            result.IsSuccessful.Should().BeFalse();
            result.Value.Should().BeNull();
            result.ProblemDetails.Should().NotBeNull();
            result.StatusCode.Should().Be(result.ProblemDetails.Status);
            result.ProblemDetails.Should().BeAssignableTo<TError>();
        }

        public static IPostprocessComposer<User> UserFixture()
        {
            Fixture fixture = new Fixture();
            return fixture.Build<User>()
                          .With(x => x.Roles, []);
        }

        public static IPostprocessComposer<Role> RoleFixture()
        {
            Fixture fixture = new Fixture();
            return fixture.Build<Role>()
                          .With(x => x.Users, []);
        }

        public static IPostprocessComposer<Class> ClassFixture()
        {
            Fixture fixture = new Fixture();
            return fixture.Build<Class>()
                          .With(x => x.Courses, []);
        }


        public static IPostprocessComposer<Course> CourseFixture()
        {
            Fixture fixture = new Fixture();
            return fixture.Build<Course>()
                          .With(x => x.Classes, []);
        }
    }
}
