using AutoMapper;
using IdentityServer.Core.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Core.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        public UserController(
            IIdentityServerInteractionService interaction,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel loginViewModel)
        {
            var context = await _interaction.GetAuthorizationContextAsync(loginViewModel.ReturnUrl);
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginViewModel.Password) && context != null)
            {
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(identity));
                return new JsonResult(new { RedirectUrl = loginViewModel.ReturnUrl, IsOk = true });
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<bool>> Register([FromBody] RegisterViewModel registerViewModel)
        {
            var user = _mapper.Map<ApplicationUser>(registerViewModel);

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            return result.Succeeded;
        }
    }
}
