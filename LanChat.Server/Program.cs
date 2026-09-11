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