using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Mango.Services.ProductAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext> (options =>
{
    //options.UseSqlServer (builder.Configuration.GetConnectionString ("DefulatConnections"));
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefualtConnection"));
});


IMapper mapper=MappingConfig.RegisterMap().CreateMapper();
builder.Services.AddSingleton (mapper);
builder.Services.AddAutoMapper (AppDomain.CurrentDomain.GetAssemblies ());

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
    if ( !app.Environment.IsDevelopment () )
    {
        c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Product API");
        c.RoutePrefix = string.Empty;
    }
});

app.UseHttpsRedirection ();
app.UseStaticFiles ();	
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