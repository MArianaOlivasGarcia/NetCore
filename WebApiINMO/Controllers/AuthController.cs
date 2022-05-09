using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiINMO.DTOs;

namespace WebApiINMO.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController: ControllerBase
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly IConfiguration Configuration;
        private readonly SignInManager<IdentityUser> SignInManager;

        public AuthController(UserManager<IdentityUser> userManager,
                IConfiguration configuration,
                SignInManager<IdentityUser> signInManager)
        {
            UserManager = userManager;
            Configuration = configuration;
            SignInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(UserCredentialsDTO userCredentials)
        {
            var user = new IdentityUser
            {
                UserName = userCredentials.UserName,
                Email = userCredentials.Email
            };
            var result = await UserManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return CreateToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }





        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(UserCredentialsDTO userCredentials)
        {
            var result = await SignInManager.PasswordSignInAsync(userCredentials.Email,
                userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return CreateToken(userCredentials);
            }
            else
            {
                return BadRequest("Credenciales incorrectas.");
            }
        }



        private AuthResponseDTO CreateToken(UserCredentialsDTO userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email )
            };

            // Crear el JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSEED"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(4);

            var securityToken = new JwtSecurityToken( issuer: null, audience: null, claims: claims, expires: expires, signingCredentials: credentials );

            return new AuthResponseDTO()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expires = expires
            };
        }

    }
}
