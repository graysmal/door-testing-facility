using door_testing_facility.Components;
using door_testing_facility.Models;
using MudBlazor.Services;
using ApexCharts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Import SQL Server data ----------------------------------------------
builder.Services.AddSingleton(sp => new ReaderEventService(builder.Configuration["CSVPath"]!));


// Access account data from CyberArk
/*string PDAddress = "", PDUser = "", PDPassword = "";
var DatabaseAccount = builder.Configuration.GetConnectionString("DatabaseAccount");
HttpClient client = new HttpClient();
try
{
    var request = new HttpRequestMessage(
        HttpMethod.Get,
        "PASSKEY VAULT REQUEST HERE" +
        DatabaseAccount
        );
    var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
    JsonObject stream = JsonNode.Parse(response.Content.ReadAsStream())!.AsObject();
    PDAddress = stream["Address"]!.ToString();
    PDUser = stream["UserName"]!.ToString();
    PDPassword = stream["Content"]!.ToString();
}
catch (Exception e) { Console.WriteLine(e.Message); }
client.Dispose();

// Build connection string and connect to SQL server
var PDConnectionString = "BUILD SQL CONNECTION STRING HERE";
try {
    builder.Services.AddDbContextFactory<PhysicalDeviceTestingContext>((DbContextOptionsBuilder options) =>
        options.UseSqlServer(PDConnectionString));
    builder.Services.AddSingleton(sp => new ReaderEventService(sp.GetRequiredService<IDbContextFactory<PhysicalDeviceTestingContext>>()));
}
catch (Exception e) {
    Console.WriteLine("something went wrong connecting to db | " + e.Message);
}*/

//-----------------------------------------------------------------------

builder.Services.AddMudServices();
builder.Services.AddApexCharts();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
