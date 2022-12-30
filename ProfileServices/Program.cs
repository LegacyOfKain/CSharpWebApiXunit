using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using ProfileServices;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //The generated Swagger JSON file will have these properties.
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chemservices",
        Description = "Chemservices API helps you do awesome stuff &#x1F680;" + Environment.MachineName
        + "&emsp;<a href='/chemservices/GetImageFromSmiles?smiles="
        //Isomeric SMILES allows for specifying isotopism and stereochemistry of a molecule.
        // Isomeric SMILES for RNA (poly(U)) , link : https://pubchem.ncbi.nlm.nih.gov/compound/RNA-_poly_U
        + System.Uri.EscapeDataString("C1=CN(C(=O)NC1=O)[C@H]2[C@@H]([C@@H]([C@H](O2)COP(=O)(O)OC3[C@H](O[C@H]([C@@H]3O)N4C=CC(=O)NC4=O)COP(=O)(O)OC5[C@H](O[C@H]([C@@H]5O)N6C=CC(=O)NC6=O)CO)O)O")
        + "&width=600&height=400'>Test URL</a> ",
        Version = "v1"
    });
});

var app = builder.Build();

// Routing should be after PathBase
app.UsePathBase("/chemservices");
app.UseRouting();  // Add explicitly

app.UseSwagger();
app.UseSwaggerUI(c =>
                // Fix for swagger endpoint under IIS website
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1245
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chemservices v1")
                c.SwaggerEndpoint("v1/swagger.json", "Chemservices v1")
                );


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")),
    RequestPath = "/StaticFiles"
});

app.MapApiEndpoints();

app.Run();

//By adding this public partial class,
//the test project will get access to Program and lets you write tests against it.
//The WebApplicationFactory<Program> class creates an in-memory application that you can test.
public partial class Program { }

