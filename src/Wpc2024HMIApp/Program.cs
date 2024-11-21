using Radzen;
using Wpc2024HMIApp.Components;
using Wpc2024HMIApp.Options;
using Wpc2024HMIApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection(key: nameof(MqttOptions)));
builder.Services.Configure<IotHubOptions>(builder.Configuration.GetSection(key: nameof(IotHubOptions)));
builder.Services.Configure<EventGridOptions>(builder.Configuration.GetSection(key: nameof(EventGridOptions)));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddRadzenQueryStringThemeService();

builder.Services.AddScoped<Radzen.DialogService>();
builder.Services.AddScoped<Radzen.NotificationService>();
builder.Services.AddScoped<Radzen.ThemeService>();

builder.Services
    .AddHostedService<WpcBackgroundService>()
    .AddSingleton<IHmiService, HmiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
