using Shop.Contracts.Catalog;
using Shop.Web.Clients;
using Shop.Contracts.Catalog;
using Shop.Contracts.Cart;
using Shop.Web.Clients;
using Shop.Contracts.Orders;
using Shop.Contracts.Payments;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// HttpClient para el catálogo (DummyJSON)
builder.Services.AddHttpClient<ICatalogClient, DummyJsonCatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://dummyjson.com");
});
builder.Services.AddSingleton<ICartClient, InMemoryCartClient>();
builder.Services.AddSingleton<IOrdersClient, InMemoryOrdersClient>();
builder.Services.AddSingleton<IPaymentsClient, InMemoryPaymentsClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalog}/{action=Index}/{id?}");


app.Run();
