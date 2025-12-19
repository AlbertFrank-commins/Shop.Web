using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Shop.Contracts.Catalog;
using Shop.Contracts.Cart;
using Shop.Contracts.Orders;
using Shop.Contracts.Payments;
using Shop.Web.Clients;
using Shop.Web.Data;
using Shop.Web.Repositories.Identity;
using Shop.Web.Services.Identity;
using Shop.Web.Repositories.Payments;
using Shop.Web.Services.Payments;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DB
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Auth (Cookies)
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddAuthorization();

// DI Identity
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Catalog (DummyJSON)
builder.Services.AddHttpClient<ICatalogClient, DummyJsonCatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://dummyjson.com");
});

// InMemory clients (Cart/Orders/Payments)
builder.Services.AddSingleton<ICartClient, InMemoryCartClient>();
builder.Services.AddSingleton<IOrdersClient, InMemoryOrdersClient>();
builder.Services.AddSingleton<IPaymentsClient, InMemoryPaymentsClient>();

// Payments (DB)
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Crear DB + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ OBLIGATORIO si usas [Authorize]
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();

