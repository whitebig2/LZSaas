using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using System.Net;
using LZ.Common.Extensions;
using LZ.Core.Authentication;
using LZ.Core.Redis;
using LZ.Core;
using LZ.Core.Filters;
using Microsoft.AspNetCore;
using Ocelot.Provider.Polly;
using Consul;
using Ocelot.Logging;
using Ocelot.Provider.Consul.Interfaces;




var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls(builder.Configuration.GetValue<string>("Url") ?? "");
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMCodeConsul();


builder.Services.AddOcelot().AddConsul<MyConsulServiceBuilder>();
builder.Configuration.AddJsonFile("appsettings.json", false, true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

var app = builder.Build();
//app.UseRedis(app.Configuration);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("AllowAngularApp");
//app.UseHttpsRedirection();
// 使用静态文件中间件
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseWebSockets();
app.UseHealthCheckMiddleware();
app.UseConsulCheckService();

app.UseOcelot().Wait();

app.MapRazorPages();

app.Run();



public class MyConsulServiceBuilder : DefaultConsulServiceBuilder
{
    public MyConsulServiceBuilder(Func<ConsulRegistryConfiguration> configurationFactory, IConsulClientFactory clientFactory, IOcelotLoggerFactory loggerFactory) : base(configurationFactory, clientFactory, loggerFactory) { }
    // I want to use the agent service IP address as the downstream hostname
    protected override string GetDownstreamHost(ServiceEntry entry, Node node) => entry.Service.Address;
}
