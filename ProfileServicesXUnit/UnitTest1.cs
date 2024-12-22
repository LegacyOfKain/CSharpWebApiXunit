using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using ProfileServices;
using System;
using System.Drawing;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace ProfileServicesXUnit
{
    public class UnitTest1
    {
        private static String molFileStringBenzene = "\n  Marvin  10310613082D          \n\n  6  6  0  0  0  0            999 V2000\n    0.7145   -0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n    0.0000   -0.8250    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n    0.7145    0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n    0.0000    0.8250    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n   -0.7145   -0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n   -0.7145    0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n  2  1  2  0  0  0  0\n  3  1  1  0  0  0  0\n  4  3  2  0  0  0  0\n  5  2  1  0  0  0  0\n  6  4  1  0  0  0  0\n  5  6  2  0  0  0  0\nM  END\n";
        private static int width = 300;
        private static int height = 300;
        private static String smilesBenzene = "C1=CC=CC=C1";
        private static String smilesRNA = "C1=CN(C(=O)NC1=O)[C@H]2[C@@H]([C@@H]([C@H](O2)COP(=O)(O)OC3[C@H](O[C@H]([C@@H]3O)N4C=CC(=O)NC4=O)COP(=O)(O)OC5[C@H](O[C@H]([C@@H]5O)N6C=CC(=O)NC6=O)CO)O)O";
        private readonly ITestOutputHelper _testOutputHelper;
        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetImageFromSmiles_ReturnsPNG()
        {
            var result = ProfileServicesApi.GetImageFromSmiles(smilesRNA, width, height);

            //Assert
            _testOutputHelper.WriteLine("Return type " + result.GetType());
            Assert.IsType<FileContentHttpResult>(result);

            FileContentHttpResult file = (FileContentHttpResult)result;
            Assert.Equal("image/png", file.ContentType);
        }

        [Fact]
        public void GetSmilesFromMolString_ReturnsSmiles()
        {
            PostRqMolBlockOnly postreq = new PostRqMolBlockOnly
            {
                molBlock = molFileStringBenzene
            };
            var result = ProfileServicesApi.GetSmilesFromMolString(postreq);

            //Assert
            _testOutputHelper.WriteLine("Return type " + result.GetType());
            Assert.IsType<ContentHttpResult>(result);

            ContentHttpResult content = (ContentHttpResult)result;
            Assert.Equal("text/plain", content.ContentType);
            Assert.Equal(smilesBenzene, content.ResponseContent);

        }

        [Fact]
        public void GetImageFromMolString_ReturnsPNG()
        {
            PostRq postreq = new PostRq
            {
                molBlock = molFileStringBenzene,
                width = width,
                height = height
            };
            var result = ProfileServicesApi.GetImageFromMolString(postreq);

            //Assert
            _testOutputHelper.WriteLine("Return type " + result.GetType());
            Assert.IsType<FileContentHttpResult>(result);

            FileContentHttpResult file = (FileContentHttpResult)result;
            Assert.Equal("image/png", file.ContentType);

        }

        [Fact]
        public async Task GetImageFromSmiles_TestEndpoint_From_Webapp()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var uri = (string.Format("/GetImageFromSmiles?smiles={0}&width={1}&height={2}",
                Uri.EscapeDataString(smilesRNA)
                , width.ToString(), height.ToString()));
            _testOutputHelper.WriteLine("uri " + uri);
            var response = await client.GetByteArrayAsync(uri);


            Assert.True(IsValidImage(response));
        }

        // runs on windows only
        public bool IsValidImage(byte[] bytes)
        {
            try
            {
                MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
                ms.Position = 0; // this is important
                var returnImage = System.Drawing.Image.FromStream(ms, true);
                String imageFormat = returnImage.RawFormat.ToString();
                _testOutputHelper.WriteLine("image format "  + imageFormat);
                if (imageFormat.ToUpper().Equals("PNG"))
                {
                    return true;
                }
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}