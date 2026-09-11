using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text;
using LanChat.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

Console.WriteLine("============================");
Console.WriteLine("====LanChat Relay Server====");
Console.WriteLine("============================");

Console.WriteLine("Enter IP address to listen on (default:localhost)");
var ipAddress = Console.ReadLine()?.Trim();
if (string.IsNullOrWhiteSpace(ipAddress)) ipAddress = "localhost";

Console.Write("Enter port to listen on (default:5000): ");
var port = Console.ReadLine()?.Trim();
if (string.IsNullOrWhiteSpace(port)) port = "5000";

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls($"http://{ipAddress}:{port}");

Console.Write("Enter a custom JWT secret (min. 16 chars, empty = auto-generate random:");
var jwtKey = Console.ReadLine()?.Trim();

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 16)
{
    Console.WriteLine("[WARNING] No custom key provided. Auto-generating a secure random session key...");
    jwtKey = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
            {
                context.Token = accessToken;
            }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

builder.Services.AddCors(o=>o.AddPolicy("AllowAll", b=> b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();