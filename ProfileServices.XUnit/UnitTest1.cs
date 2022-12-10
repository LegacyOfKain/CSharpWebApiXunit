using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProfileServices.Controllers;
using Xunit.Abstractions;


namespace ProfileServices.XUnit
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void GetImageFromSmiles_ReturnsPNG()
        {
            //Arrange
            var loggerStub = new Mock<ILogger<ProfileServicesController>>();
            var controller = new ProfileServicesController(loggerStub.Object);

            //Act
            var result = await controller.GetImageFromSmiles("", 200, 200);

            //Assert
            _testOutputHelper.WriteLine("Return type "+result.GetType());
            Assert.IsType<FileContentResult>(result);

            FileContentResult file = (FileContentResult)result;
            Assert.Equal("image/png", file.ContentType);
             

        }
    }
}