using GraphMolWrap;

namespace ProfileServices
{
    public static class ProfileServicesApi
    {
        public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/GetImageFromSmiles", GetImageFromSmiles).WithTags("ChemServices");
            routes.MapPost("/GetSmilesFromMolString", GetSmilesFromMolString).WithTags("ChemServices");
            routes.MapPost("/GetImageFromMolString", GetImageFromMolString).WithTags("ChemServices");

            return routes;
        }

        public static IResult GetImageFromSmiles(String smiles, int width, int height)
        {
            var mol = RWMol.MolFromSmiles(smiles);
            var drawer = getDrawer(mol, width, height);
            return Results.File(drawer.getImage().ToArray(), "image/png");
        }

        public static IResult GetSmilesFromMolString(PostRequest request)
        {
            Console.WriteLine(request.molFileString);
            var mol = RWMol.MolFromMolBlock(request.molFileString);           
            return Results.Content(mol.MolToSmiles(true,true), "text/plain");
        }

        public static IResult GetImageFromMolString(PostRequest2 request)
        {
            Console.WriteLine(request.molFileString);
            var mol = RWMol.MolFromMolBlock(request.molFileString);
            var drawer = getDrawer(mol,request.width,request.height);
            return Results.File(drawer.getImage().ToArray(), "image/png");
        }

        public static MolDraw2DCairo getDrawer(RWMol mol, int width, int height)
        {
            var drawer = new MolDraw2DCairo(width, height);
            drawer.drawOptions().maxFontSize = 10;
            drawer.drawMolecule(mol);
            drawer.finishDrawing();
            return drawer;
        }
    }

    public class PostRequest
    {
        public String molFileString { get; set; }
    }

    public class PostRequest2
    {
        public String molFileString { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}
