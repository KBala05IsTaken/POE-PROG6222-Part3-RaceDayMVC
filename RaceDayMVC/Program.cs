
using RaceDayMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers and views.
builder.Services.AddControllersWithViews();

// Register memory storage for user sessions.
builder.Services.AddDistributedMemoryCache();

// Configure session settings.
builder.Services.AddSession(options =>
{
    // Automatically expire inactive sessions after 30 minutes.
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Prevent client-side scripts from accessing the session cookie.
    options.Cookie.HttpOnly = true;

    // Allow the session cookie to be stored.
    options.Cookie.IsEssential = true;

    // Give the session cookie a project-specific name.
    options.Cookie.Name = ".RaceDayMVC.Session";
});

// Allow services to access the current HTTP context and session.
builder.Services.AddHttpContextAccessor();

// Register the HTTP client used to communicate with the RaceDay API.
builder.Services.AddHttpClient("RaceDayAPI", client =>
{
    // Read the API address from appsettings.json.
    string? apiUrl = builder.Configuration["ApiSettings:BaseUrl"];

    if (string.IsNullOrWhiteSpace(apiUrl))
    {
        throw new InvalidOperationException(
            "The RaceDay API BaseUrl is missing from appsettings.json.");
    }

    client.BaseAddress = new Uri(apiUrl);

    // Set a maximum waiting time for API requests.
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register the API communication service for dependency injection.
builder.Services.AddScoped<RaceDayApiService>();

var app = builder.Build();

// Configure error handling for production.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Allow the application to serve CSS, JavaScript and image files.
app.UseStaticFiles();

// Enable MVC routing.
app.UseRouting();

// Enable session management before controllers execute.
app.UseSession();

// Configure the default MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start the MVC application.
app.Run();
