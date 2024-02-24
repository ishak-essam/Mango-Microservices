using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Mango.Services.CartAPI.Extensions;
using Mango.Services.CartAPI;
using Mango.Services.CartAPI.Data;
using Mango.Service.CartAPI.Services.IServices;
using Mango.Service.CartAPI.Services;
using Mango.Service.CartAPI.Utility;
using System.Net.Http;
using Mango.MessageBus;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext> (options =>
{
    //options.UseSqlServer (builder.Configuration.GetConnectionString ("DefulatConnections"));
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefualtConnection"));
});


IMapper mapper=MappingConfig.RegisterMap().CreateMapper();
builder.Services.AddSingleton (mapper);
builder.Services.AddAutoMapper (AppDomain.CurrentDomain.GetAssemblies ());
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler> ();
builder.Services.AddHttpContextAccessor ();
builder.Services.AddScoped<IProductService, ProductService> ();
builder.Services.AddScoped<ICouponService, CouponService> ();
builder.Services.AddScoped<IMessageBus, MessageBus> ();
builder.Services.AddHttpClient ("Product",u=>u.BaseAddress=new Uri (builder.Configuration [ "ServicesUrls:ProductAPI" ])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddHttpClient ("Coupon",u=>u.BaseAddress=new Uri (builder.Configuration [ "ServicesUrls:CouponAPI" ]));


// Add services to the container.
builder.Services.AddControllers ();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (option =>
{
    option.AddSecurityDefinition (name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement (new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
});
builder.AddAppAuthetication ();

builder.Services.AddAuthorization ();

var app = builder.Build();

ApplyMigration ();

// Configure the HTTP request pipeline.
app.UseSwaggerUI (c =>
{
        c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Cart API");
        c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection ();
app.UseAuthentication ();
app.UseAuthorization ();
app.UseStaticFiles ();
app.MapControllers ();
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