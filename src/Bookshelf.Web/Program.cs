using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Security.Claims;
using Bookshelf.Application;
using Bookshelf.Application.Users;
using Bookshelf.Infrastructure;
using Bookshelf.Infrastructure.Persistence;
using Bookshelf.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.WebEncoders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // обов'язкові поля перевіряє FluentValidation, без цього MVC додає ще й свої англійські помилки
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var messages = options.ModelBindingMessageProvider;
    messages.SetAttemptedValueIsInvalidAccessor((value, _) => $"Неправильне значення «{value}».");
    messages.SetValueIsInvalidAccessor(value => $"Неправильне значення «{value}».");
    messages.SetValueMustBeANumberAccessor(_ => "Поле має бути числом.");
    messages.SetValueMustNotBeNullAccessor(_ => "Заповніть це поле.");
});
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
builder.Services.Configure<LibraryContactsOptions>(builder.Configuration.GetSection(LibraryContactsOptions.SectionName));
builder.Services.AddApiOutputCache(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddApplication();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var googleEnabled = !string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret);

var authentication = builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/Login");

if (googleEnabled)
{
    authentication.AddGoogle(options =>
    {
        options.ClientId = googleClientId!;
        options.ClientSecret = googleClientSecret!;
        options.Events.OnCreatingTicket = async context =>
        {
            var email = context.Principal!.FindFirstValue(ClaimTypes.Email)!;
            var name = context.Principal!.FindFirstValue(ClaimTypes.Name) ?? email;
            var users = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            var user = await users.FindOrCreateAsync(email, name, context.HttpContext.RequestAborted);
            context.Identity!.AddClaim(new Claim(PlanFilter.ClaimType, user.Plan));
        };
    });
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<PlanFilter>()
    .UseDisabledFeaturesHandler((_, context) =>
        context.Result = new ViewResult { ViewName = "~/Views/Premium/Unavailable.cshtml", StatusCode = StatusCodes.Status403Forbidden });

builder.Services.AddAuthorization(options =>
{
    if (!googleEnabled)
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build();
    }
});
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await app.Services.ApplyMigrationsAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var culture = new CultureInfo("uk-UA");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = [culture],
    SupportedUICultures = [culture],
});

app.UseWhen(context => IsApiRequest(context), api => api.UseExceptionHandler());
app.UseWhen(context => !IsApiRequest(context), web => web.UseStatusCodePagesWithReExecute("/Home/Status/{0}"));
if (!app.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER"))
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static bool IsApiRequest(HttpContext context) => context.Request.Path.StartsWithSegments("/api");
