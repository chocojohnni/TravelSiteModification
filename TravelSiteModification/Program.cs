using TravelSiteModification.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddHttpClient<EventsAPIClient>(client =>
{
    var baseUrl = builder.Configuration["EventsApi:BaseUrl"];
    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
});

builder.Services.AddHttpClient<FlightsAPIClient>(client =>
{
    string baseUrl = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tun31378/WebAPI/";
    client.BaseAddress = new Uri(baseUrl);
});

//builder.Services.AddHttpClient<EventsAPIClient>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7272/");
//});

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
