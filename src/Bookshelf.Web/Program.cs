using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Bookshelf.Application;
using Bookshelf.Infrastructure;
using Bookshelf.Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.WebEncoders;

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
builder.Services.AddApplication();
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

app.UseStatusCodePagesWithReExecute("/Home/Status/{0}");
if (!app.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER"))
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
