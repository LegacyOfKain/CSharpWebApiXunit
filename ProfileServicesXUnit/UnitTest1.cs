using Microsoft.AspNetCore.Http.HttpResults;
using ProfileServices;
using Xunit.Abstractions;

namespace ProfileServicesXUnit
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetImageFromSmiles_ReturnsPNG()
        {
            var result = ProfileServicesApi.GetImageFromSmiles("", 200, 200);

            //Assert
            _testOutputHelper.WriteLine("Return type " + result.GetType());
            Assert.IsType<FileContentHttpResult>(result);

            FileContentHttpResult file = (FileContentHttpResult)result;
            Assert.Equal("image/png", file.ContentType);
        }

        [Fact]
        public void GetImageFromSmiles_ReturnsSmiles()
        {
            PostRequest postreq = new PostRequest();
            postreq.molFileString = "\n  Marvin  10310613082D          \n\n  6  6  0  0  0  0            999 V2000\n    0.7145   -0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n    0.0000   -0.8250    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n    0.7145    0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n    0.0000    0.8250    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n   -0.7145   -0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n   -0.7145    0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n  2  1  2  0  0  0  0\n  3  1  1  0  0  0  0\n  4  3  2  0  0  0  0\n  5  2  1  0  0  0  0\n  6  4  1  0  0  0  0\n  5  6  2  0  0  0  0\nM  END\n";
            var result = ProfileServicesApi.GetSmilesFromMolString(postreq);

            //Assert
            _testOutputHelper.WriteLine("Return type " + result.GetType());
            Assert.IsType<ContentHttpResult>(result);

            ContentHttpResult content = (ContentHttpResult)result;
            Assert.Equal("text/plain", content.ContentType);
            Assert.Equal("C1=CC=CC=C1", content.ResponseContent);

        }
    }
}