using ProfileServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Routing should be after PathBase
app.UsePathBase("/chemservices");
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();


app.MapApiEndpoints();

app.Run();

//By adding this public partial class,
//the test project will get access to Program and lets you write tests against it.
//The WebApplicationFactory<Program> class creates an in-memory application that you can test.
public partial class Program { }

