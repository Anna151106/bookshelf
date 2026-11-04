using System.Security.Claims;
using Bookshelf.Application.Users;
using Bookshelf.Domain.Entities;
using Bookshelf.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

public class AccountController(IUserService userService, IAuthenticationSchemeProvider schemes) : Controller
{
    public async Task<IActionResult> Login(string? returnUrl)
    {
        if (await schemes.GetSchemeAsync(GoogleDefaults.AuthenticationScheme) is null)
        {
            return NotFound();
        }

        var redirectUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Home");
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePlan(string plan, CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null || plan is not (AppUser.FreePlan or AppUser.PremiumPlan))
        {
            return BadRequest();
        }

        var user = await userService.ChangePlanAsync(email, plan, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var claims = User.Claims.Where(c => c.Type != PlanFilter.ClaimType).Append(new Claim(PlanFilter.ClaimType, user.Plan));
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var user = email is null ? null : await userService.GetByEmailAsync(email, cancellationToken);
        return user is null ? NotFound() : View(user);
    }
}
