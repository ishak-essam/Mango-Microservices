using AutoMapper;
using CourseUdemy.Extensions;
using Mango.Services.AuthAPI.Extensions;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext> (options =>
{
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefualtConnection"));
});

IMapper mapper=MappingConfig.RegisterMap().CreateMapper();
builder.Services.AddSingleton (mapper);
builder.Services.AddAutoMapper (AppDomain.CurrentDomain.GetAssemblies ()); 

builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (ele =>
{
    ele.AddSecurityDefinition (name:JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    ele.AddSecurityRequirement (new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme,
                }
            },
            new string[]{ }
        }
    });
});
builder.Services.AddControllers ();
//builder.Services.AddIdetityServices (builder.Configuration);
builder.AddAppAuthetication ();
builder.Services.AddAuthorization (
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerUI (c =>
{
    if ( !app.Environment.IsDevelopment () )
    {
        c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Coupon API");
        c.RoutePrefix = string.Empty;
    }
});
app.UseHttpsRedirection ();
app.UseAuthentication ();
app.UseAuthorization ();
app.MapControllers ();
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection ("Strip:SecretKey").Get<string> ();
ApplyMigruation ();

app.Run ();

void ApplyMigruation ( )
{
    using ( var scope = app.Services.CreateScope () )
    {
        var _db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if ( _db.Database.GetPendingMigrations().Count()>0 ) {
            _db.Database.Migrate ();
        }
    }
}