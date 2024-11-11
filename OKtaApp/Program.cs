using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookies as the default
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Challenge with OpenID Connect
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Path to redirect unauthenticated users
    options.AccessDeniedPath = "/Account/AccessDenied";
})
.AddOpenIdConnect(options =>
{
    options.ClientId = builder.Configuration["Okta:ClientId"];
    options.ClientSecret = builder.Configuration["Okta:ClientSecret"];
    options.Authority = builder.Configuration["Okta:Domain"];
    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("groups");
    options.SaveTokens = true;

    options.CallbackPath = "/authorization-code/callback"; // Ensure this matches your Okta app settings
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            // Extract the group claims and add them to the user's claims
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity)context.Principal.Identity;
            var groups = context.Principal.FindFirst("groups")?.Value;

            if (!string.IsNullOrEmpty(groups))
            {
                // If the groups claim is a JSON array, deserialize it
                var groupList = JsonSerializer.Deserialize<List<string>>(groups);
                foreach (var group in groupList)
                {
                    claimsIdentity.AddClaim(new Claim("group", group));
                }
            }

            await Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/Home/Error");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure this is before app.UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
