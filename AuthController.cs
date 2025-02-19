﻿using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using static AspNet.Security.OAuth.GitHub.GitHubAuthenticationConstants;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.Azure.SignalR.VideoChat
{
    public class AuthController : Controller
    {
        public const string AvatarUrlClaim = "urn:github:avatar_url";

        private readonly string TurnServerUrl;

        private readonly string TurnServerUser;

        private readonly string TurnServerCredential;

        public AuthController(IConfiguration config)
        {
            TurnServerUrl = config["TurnServerUrl"];
            TurnServerUser = config["TurnServerUser"];
            TurnServerCredential = config["TurnServerCredential"];
        }

        [HttpGet("signin")]
        public IActionResult Login(string redirectUrl)
        {
            if (!User.Identity.IsAuthenticated) return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl ?? "/" }, GitHubAuthenticationDefaults.AuthenticationScheme);
            return Redirect(redirectUrl ?? "/");
        }

        [Authorize]
        [HttpGet("signout")]
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile(string id)
        {
            if (id != null)
            {
                var user = Users.Instance.GetUser(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                return Ok(new
                {
                    Id = user.Id,
                    Name = user.Name,
                    Avatar = user.Avatar
                });
            }
            else
            {
                return Ok(new
                {
                    Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
                    Name = User.Claims.FirstOrDefault(c => c.Type == Claims.Name)?.Value ?? User.Identity.Name,
                    Avatar = User.Claims.FirstOrDefault(c => c.Type == AvatarUrlClaim)?.Value
                });
            }
        }

        [Authorize]
        [HttpGet("turn")]
        public IActionResult GetTurnServer()
        {
            return Ok(new
            {
                Urls = TurnServerUrl,
                Username = TurnServerUser,
                Credential = TurnServerCredential
            });
        }
    }
}
