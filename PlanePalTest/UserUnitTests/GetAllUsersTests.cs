using AutoMapper;
using Microsoft.OpenApi.Any;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.User;
using PlanePal.Enums;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Implementation;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class GetAllUsersTests
    {
        private readonly IUserService userService;
        private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IMapper mapper = Substitute.For<IMapper>();
        private readonly IAzureStorage _a = Substitute.For<IAzureStorage>();
        public GetAllUsersTests()
        {
            this.userService = new UserService(unitOfWork, mapper, _a);
        }
        [Fact]
        public async void GetAll_ReturnsNull_ReturnsError()
        {
            //Arrange
            unitOfWork.UserRepository.GetAllUsers().ReturnsNull();
            //Act
            var result = await userService.GetAll();
            //Assert
            Assert.Equal("Users cannot be fetched!", result.Message);
            Assert.False(result.Success);
        }
        [Fact]
        public async void GetAll_CannotMap_ReturnsError()
        {
            //Arrange
            mapper.Map<IEnumerable<ReadUserDTO>>(Arg.Any<Object>()).ReturnsNull();
            //Act
            var result = await userService.GetAll();

            //Assert
            Assert.Equal("Users cannot be mapped!", result.Message);
            Assert.False(result.Success);
        }
        [Fact]
        public async void GetAll_HappyPath_ReturnsSuccess()
        {
            //Arrange
            var users = new List<User>();
            users.Add(new User());
            var usersDTO = new List<ReadUserDTO>();
            usersDTO.Add(new ReadUserDTO()); 
            unitOfWork.UserRepository.GetAllUsers().Returns(users);
            mapper.Map<IEnumerable<ReadUserDTO>>(Arg.Any<Object>()).Returns(usersDTO);

            //Act
            var result = await userService.GetAll();

            //Assert
            Assert.Equal("Users successfully fetched.", result.Message);
            Assert.True(result.Success);
        }
    }
}
