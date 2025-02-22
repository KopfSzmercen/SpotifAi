﻿using Microsoft.AspNetCore.Identity;

namespace SpotifAi.Users;

internal sealed class User : IdentityUser<Guid>
{
    public SpotifyAccessToken? SpotifyAccessToken { get; set; }
}