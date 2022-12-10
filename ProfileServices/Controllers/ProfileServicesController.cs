using GraphMolWrap;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ProfileServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileServicesController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<ProfileServicesController> _logger;

        public ProfileServicesController(ILogger<ProfileServicesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetImageFromSmiles")]
        public async Task<IActionResult> GetImageFromSmiles(String smiles, int width, int height)
        {
            var mol = RWMol.MolFromSmiles(smiles);
            var drawer = new MolDraw2DCairo(width, height);
            drawer.drawOptions().addAtomIndices = true;
            drawer.drawOptions().maxFontSize = 10;
            drawer.drawMolecule(mol);
            drawer.finishDrawing();
            return File(  drawer.getImage().ToArray() , "image/png" );
        }
    }
}