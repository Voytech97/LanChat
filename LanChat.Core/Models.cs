using System;
using System.Collections.Generic;
using System.Text;

namespace LanChat.Core;
public class AuthRequest
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
}
