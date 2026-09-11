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