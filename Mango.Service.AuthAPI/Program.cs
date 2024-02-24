using Mango.MessageBus;
using Mango.Service.AuthAPI.Models;
using Mango.Service.AuthAPI.Service;
using Mango.Service.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers ();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<AppDbContext> (options =>
{
    options.UseSqlServer ( builder.Configuration.GetConnectionString("DefualtConnection"));
});
builder.Services.Configure<JwtOptions> (builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole> ().AddEntityFrameworkStores<AppDbContext> ().AddDefaultTokenProviders ();
builder.Services.AddScoped<IMessageBus, MessageBus> ();
builder.Services.AddScoped<IAuth, AuthService> ();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGeneratorService> ();



builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if ( app.Environment.IsDevelopment () )
//{
//}
app.UseSwagger ();
app.UseSwaggerUI (c =>
{
    if ( !app.Environment.IsDevelopment () )
    {
        c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Auth API");
        c.RoutePrefix = string.Empty;
    }
});

app.UseHttpsRedirection ();
app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllers ();

ApplyMigruation ();

app.Run ();

void ApplyMigruation ( )
{
    using ( var scope = app.Services.CreateScope () )
    {
        var _db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if ( _db.Database.GetPendingMigrations ().Count () > 0 )
        {
            _db.Database.Migrate ();
        }
    }
}