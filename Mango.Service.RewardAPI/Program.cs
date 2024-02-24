using Mango.Service.RewardAPI.Messaging;
using Mango.Service.RewardAPI.Services;
using Mango.Services.RewardAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext> (options =>
{
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefualtConnection"));
});
// Add services to the container.


var optionBuilder=new DbContextOptionsBuilder<AppDbContext> ();
optionBuilder.UseSqlServer (builder.Configuration.GetConnectionString ("DefualtConnection"));
builder.Services.AddSingleton (new RewardService (optionBuilder.Options));


builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer> ();

builder.Services.AddControllers ();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerUI (c =>
{
    if ( !app.Environment.IsDevelopment () )
    {
        c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Reward API");
        c.RoutePrefix = string.Empty;
    }
});

app.UseHttpsRedirection ();

app.UseAuthorization ();

app.MapControllers ();
app.UseAzureServiceBusConsumer ();
ApplyMigration ();
app.Run ();


void ApplyMigration ( )
{
    using ( var scope = app.Services.CreateScope () )
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if ( _db.Database.GetPendingMigrations ().Count () > 0 )
        {
            _db.Database.Migrate ();
        }
    }
}
