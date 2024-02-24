using Mango.Gateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//builder.AddAppAuthetication ();
builder.Services.AddOcelot (builder.Configuration);

var app = builder.Build();

app.MapGet ("/", ( ) => "Hello World!");
app.UseOcelot ();
app.Run ();
