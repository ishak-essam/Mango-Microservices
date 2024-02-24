using Mango.Web.Services;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews ();
builder.Services.AddHttpContextAccessor ();
builder.Services.AddHttpClient ();
builder.Services.AddHttpClient<ICouponService,CouponServices> ();
builder.Services.AddHttpClient<IAuthService, AuthService> ();
builder.Services.AddHttpClient<IProduct,ProductService> ();
builder.Services.AddHttpClient<IOrderServices, OrderServices> ();
builder.Services.AddHttpClient<ICartService, CartService> ();
SD.CouponAPIBase=builder.Configuration["ServiceUrls:CouponAPI"];
SD.AuthAPIBase=builder.Configuration[ "ServiceUrls:AuthAPI" ]; 
SD.ProductAPIBase=builder.Configuration[ "ServiceUrls:ProductAPI" ]; 
SD.CartAPIBase=builder.Configuration[ "ServiceUrls:CartAPI" ];
SD.OrderAPIBase=builder.Configuration[ "ServiceUrls:OrderAPI" ];

builder.Services.AddScoped<IBaseServices,BaseServices> ();
builder.Services.AddScoped<ITokenProvider,TokenProvider> ();
builder.Services.AddScoped<IProduct,ProductService> ();
builder.Services.AddScoped<IAuthService, AuthService> ();
builder.Services.AddScoped<IOrderServices, OrderServices> ();
builder.Services.AddScoped<ICartService, CartService> ();
builder.Services.AddScoped<ICouponService,CouponServices> ();
builder.Services.AddAuthentication (CookieAuthenticationDefaults.AuthenticationScheme).AddCookie (optionis => {
    optionis.ExpireTimeSpan = TimeSpan.FromHours (10);
    optionis.LoginPath = "/Auth/Login";
    optionis.AccessDeniedPath = "/Auth/AccessDeniedPath";

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( !app.Environment.IsDevelopment () )
{
    app.UseExceptionHandler ("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts ();
}

app.UseHttpsRedirection ();
app.UseStaticFiles ();

app.UseRouting ();

app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllerRoute (
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run ();
