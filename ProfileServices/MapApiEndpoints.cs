using GraphMolWrap;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ProfileServices
{
    public static class ProfileServicesApi
    {
        public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder routes)
        {

            routes.MapPost("/GetImageFromMolFile", GetImageFromMolFile).WithTags("ChemServices");
            routes.MapGet("/GetImageFromSmiles", GetImageFromSmiles).WithTags("ChemServices");
            routes.MapPost("/GetSmilesFromMolString", GetSmilesFromMolString).WithTags("ChemServices");
            routes.MapPost("/GetImageFromMolString", GetImageFromMolString).WithTags("ChemServices");
            routes.MapGet("/GetMolStringFromSmiles", GetMolStringFromSmiles).WithTags("ChemServices");
            routes.MapGet("/GetSequenceFromHELM", GetSequenceFromHELM).WithTags("ChemServices")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Returns a sequence from HELM String",
                    Description = "Construct a molecule from a HELM string (currently only supports peptides, RNA and DNA)<br><code>PEPTIDE1{A.C.T.G.C.T.W.G.T.W.E.C.W.C.Q.W}|PEPTIDE2{A.C.T.G.C.T.W.G.T.W.E.Q}$PEPTIDE1,PEPTIDE1,5:R3-14:R3|PEPTIDE2,PEPTIDE1,2:R3-12:R3$$$</code><br><code>RNA1{P.R(A)P.R(U)P}$$$$</code><br><code>RNA1{\\[dR\\](A)P.\\[dR\\](C)P.\\[dR\\](G)P.\\[dR\\](A)}</code>"
                });
            routes.MapGet("/GetImageFromHELM", GetImageFromHELM).WithTags("ChemServices")
                .WithOpenApi(operation => new(operation)
                {
                    Summary =
                    "Returns an image from a HELM string",
                    Description = "Returns an image from a HELM string (currently only supports peptides, RNA and DNA)<br><code>PEPTIDE1{A.C.T.G.C.T.W.G.T.W.E.C.W.C.Q.W}|PEPTIDE2{A.C.T.G.C.T.W.G.T.W.E.Q}$PEPTIDE1,PEPTIDE1,5:R3-14:R3|PEPTIDE2,PEPTIDE1,2:R3-12:R3$$$</code><br><code>RNA1{P.R(A)P.R(U)P}$$$$</code><br><code>RNA1{\\[dR\\](A)P.\\[dR\\](C)P.\\[dR\\](G)P.\\[dR\\](A)}</code>"
                });
            
            routes.MapGet("/Get3DSDFStringFromSmiles", Get3DSDFStringFromSmiles).WithTags("ChemServices");

            return routes;
        }

        public static IResult GetMolStringFromSmiles(String smiles)
        {
            /*
            if (postRq.molBlock == null || postRq.molBlock.Length == 0)
            {
                return BadRequest();
            }
            */
            var mol = RWMol.MolFromSmiles(smiles);
            return Results.Content(RDKFuncs.MolToMolBlock(mol), "text/plain");
        }

        public static async Task<IResult> GetImageFromMolFile(IFormFile file, int width, int height)
        {
            /*
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }
            */

            var view = new MolDraw2DCairo(width, height);
            //view.drawOptions().explicitMethyl = true;
            view.drawOptions().maxFontSize = 10;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var molBlock = await reader.ReadToEndAsync();
                //_logger.LogInformation (molBlock);
                var toluene = RWMol.MolFromMolBlock(molBlock);
                //RDKFuncs.prepareMolForDrawing(toluene);
                view.drawMolecule(toluene);
                view.finishDrawing();
            }

            return Results.File(view.getImage().ToArray(), "image/png");
        }

        public static IResult GetImageFromSmiles(String smiles, int width, int height)
        {
            var mol = RWMol.MolFromSmiles(smiles);
            var drawer = getDrawer(mol, width, height);
            return Results.File(drawer.getImage().ToArray(), "image/png");
        }

        public static IResult GetSmilesFromMolString(PostRqMolBlockOnly request)
        {
            Console.WriteLine(request.molBlock);
            var mol = RWMol.MolFromMolBlock(request.molBlock);           
            return Results.Content(mol.MolToSmiles(true,true), "text/plain");
        }

        public static IResult GetImageFromMolString(PostRq request)
        {
            Console.WriteLine(request.molBlock);
            var mol = RWMol.MolFromMolBlock(request.molBlock);
            var drawer = getDrawer(mol,request.width,request.height);
            return Results.File(drawer.getImage().ToArray(), "image/png");
        }


        public static IResult GetSequenceFromHELM(String helmstring)
        {
            /*
            if (postRq.molBlock == null || postRq.molBlock.Length == 0)
            {
                return BadRequest();
            }
            */
            var mol = RWMol.MolFromHELM(helmstring, true);
            return Results.Content(mol.MolToSequence(), "text/plain");
        }

        public static IResult GetImageFromHELM(String helmstring, int width, int height)
        {
            var mol = RWMol.MolFromHELM(helmstring, true);
            var view = getDrawer(mol, width, height);

            return Results.File(view.getImage().ToArray(), "image/png");
        }

        //https://stackoverflow.com/questions/70335013/why-i-cant-get-3d-mol-structure-in-rdkit
        public static IResult Get3DSDFStringFromSmiles(String smiles)
        {

            var mol = RWMol.MolFromSmiles(smiles);
            RDKFuncs.addHs(mol);
            var result = DistanceGeom.EmbedMolecule(mol, 4, 0xf00d);
            if (result != 0)
            {
                //_logger.LogError("result = " + result + " Non zero exit status");
                return Results.BadRequest("result = " + result + " Non zero exit status");
            }
            //Don't delete below 2 lines of code
            //var c = mol.getConformer();
            //_logger.LogInformation (c.getPositions().ToString())  ;      


            return Results.Content(RDKFuncs.MolToMolBlock(mol), "text/plain");
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

    public class PostRqMolBlockOnly
    {
        public required string molBlock { get; set; }
    }

    public class PostRq
    {
        public required string molBlock { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}
