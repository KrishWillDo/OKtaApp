using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        // Start the OpenID Connect authentication process
        var redirectUrl = Url.Action("Index", "Home");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Get the ID token
        var idToken = await HttpContext.GetTokenAsync("id_token");

        // Sign out of OpenID Connect
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        // Redirect to the Okta logout URL
        var oktaDomain = "https://dev-58532666.okta.com"; // Update with your Okta domain
        var returnUrl = "https://localhost:7058/"; // Adjust to your application's URL
        var logoutUrl = $"{oktaDomain}/oauth2/default/v1/logout?id_token_hint={idToken}&post_logout_redirect_uri={returnUrl}";

        return Redirect(logoutUrl);
        // return RedirectToAction("Index", "Home");
    }




}
