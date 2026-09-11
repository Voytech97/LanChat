using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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

var usersDb = new ConcurrentDictionary<string, AuthRequest>();

app.MapPost("/api/register", (AuthRequest req) =>
{
    if (usersDb.ContainsKey(req.Username))
        return Results.BadRequest(new AuthResponse { Success = false, Message = "Username already exists." });

    usersDb[req.Username] = req;
    Console.WriteLine($"[SERVER] User registered: {req.Username}");
    return Results.Ok(new AuthResponse { Success = true });

});

app.MapPost("/api/login", (AuthRequest req) =>
{
    if (usersDb.TryGetValue(req.Username, out var user) && user.PasswordHash == req.PasswordHash)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, req.Username) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        Console.WriteLine($"[SERVER] User logged in: {req.Username}");
        return Results.Ok(new AuthResponse { Success = true, Token = tokenHandler.WriteToken(token) });
    }

    return Results.BadRequest(new AuthResponse { Success = false, Message = "Invalid credentials." });
});

app.MapGet("/api/publicKey/{username}", (string username) =>
    usersDb.TryGetValue(username, out var user) ? Results.Ok(user.PublicKey) : Results.NotFound());
app.MapHub<ChatHub>("/chathub");

Console.WriteLine($"Starting server on http://{ipAddress}:{port}...");
app.Run();

// --- SIGNALR HUB (Message Relay) ---
public class ChatHub : Hub
{
    // Dictionary mapping the username to their unique connection ID (ConnectionId)
    private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

    public override Task OnConnectedAsync()
    {
        var username = Context.UserIdentifier;
        if (username != null)
        {
            OnlineUsers[username] = Context.ConnectionId;
            Console.WriteLine($"[SIGNALR] User {username} joined the chat.");
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.UserIdentifier;
        if (username != null)
        {
            OnlineUsers.TryRemove(username, out _);
            Console.WriteLine($"[SIGNALR] User {username} disconnected.");
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(ChatMessageDto msg)
    {
        // The server acts as a blind relay - it decrypts nothing, just finds the receiver and forwards the packet
        if (OnlineUsers.TryGetValue(msg.Receiver, out var receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", msg);
            Console.WriteLine($"[SIGNALR] Packet forwarded from {msg.Sender} to {msg.Receiver}");
        }
        else
        {
            Console.WriteLine($"[SIGNALR] Receiver {msg.Receiver} is offline.");
        }
    }
}