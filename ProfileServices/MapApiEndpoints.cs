using GraphMolWrap;

namespace ProfileServices
{
    public static class ProfileServicesApi
    {
        public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/GetImageFromSmiles", GetImageFromSmiles).WithTags("ChemServices");
            routes.MapPost("/GetSmilesFromMolString", GetSmilesFromMolString).WithTags("ChemServices");

            return routes;
        }

        public static IResult GetImageFromSmiles(String smiles, int width, int height)
        {
            var mol = RWMol.MolFromSmiles(smiles);
            var drawer = new MolDraw2DCairo(width, height);
            drawer.drawOptions().addAtomIndices = true;
            drawer.drawOptions().maxFontSize = 10;
            drawer.drawMolecule(mol);
            drawer.finishDrawing();
            return Results.File(drawer.getImage().ToArray(), "image/png");
        }

        public static IResult GetSmilesFromMolString(PostRequest request)
        {
            Console.WriteLine(request.molFileString);
            var mol = RWMol.MolFromMolBlock(request.molFileString);           
            return Results.Content(mol.MolToSmiles(true,true), "text/plain");
        }
    }

    public class PostRequest
    {
        public String molFileString { get; set; }
        //public int width { get; set; }
        //public int height { get; set; }
    }
}
