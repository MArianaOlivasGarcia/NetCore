using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiINMO.DTOs;
using Microsoft.AspNetCore.DataProtection;
using WebApiINMO.Services;

namespace WebApiINMO.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly IConfiguration Configuration;
        private readonly SignInManager<IdentityUser> SignInManager;
        private readonly IDataProtector DataProtector;
        private readonly HashService HashService;

        public AuthController(UserManager<IdentityUser> userManager,
                IConfiguration configuration,
                SignInManager<IdentityUser> signInManager,
                IDataProtectionProvider dataProtectionProvider,
                HashService hashService)
        {
            UserManager = userManager;
            Configuration = configuration;
            SignInManager = signInManager;
            DataProtector = dataProtectionProvider.CreateProtector("super_secreto");
            HashService = hashService;  
        }




        [HttpGet("renew", Name = "renewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthResponseDTO>> Renew()
        {
            var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;

            var userCredentials = new UserCredentialsDTO()
            {
                Email = email
            };

            return await CreateToken(userCredentials);
        }




        [HttpPost("register", Name = "register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(UserCredentialsDTO userCredentials)
        {
            var user = new IdentityUser
            {
                UserName = userCredentials.Email,
                Email = userCredentials.Email
            };
            var result = await UserManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return await CreateToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }





        [HttpPost("login", Name = "login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(UserCredentialsDTO userCredentials)
        {
            var result = await SignInManager.PasswordSignInAsync(userCredentials.Email,
                userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await CreateToken(userCredentials);
            }
            else
            {
                return BadRequest("Credenciales incorrectas.");
            }
        }



        private async Task<AuthResponseDTO> CreateToken(UserCredentialsDTO userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email )
            };

            var user = await UserManager.FindByEmailAsync(userCredentials.Email);
            var claimsdb = await UserManager.GetClaimsAsync(user);

            // Juntar las dos listas, los claims y los claimsdb
            // Añadir los claimsdb a claims
            claims.AddRange(claimsdb);


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


        [HttpPost("addRole", Name = "addRoleToUser")]
        public async Task<ActionResult> AddRole(AddRoleDTO addRoleDTO)
        {
            var user = await UserManager.FindByEmailAsync(addRoleDTO.Email);

            await UserManager.AddClaimAsync(user, new Claim(addRoleDTO.Role, "1"));

            return NoContent();

        }


        [HttpPost("removeRole", Name = "removeRoleToUser")]
        public async Task<ActionResult> RemoveRole(AddRoleDTO addRoleDTO)
        {
            var user = await UserManager.FindByEmailAsync(addRoleDTO.Email);

            await UserManager.RemoveClaimAsync(user, new Claim(addRoleDTO.Role, "1"));

            return NoContent();

        }





        [HttpGet("hash", Name = "hash")]
        public async Task<ActionResult> Hash()
        {
            var textPlane = "Mariana Olivas";
            var result1 = HashService.Hash(textPlane);
            var result2 = HashService.Hash(textPlane);

            return Ok(new
            {
                textPlane = textPlane,
                hash1 = result1,
                hash2 = result2
            });

        }




        [HttpGet("encriptar", Name = "encriptar")]
        public async Task<ActionResult> Encriptar()
        {
            var textPlane = "Mariana Olivas";
            var textEncripted = DataProtector.Protect(textPlane);
            var textDesencripted = DataProtector.Unprotect(textEncripted);

            return Ok(new
            {
                textPlane = textPlane,
                textEncripted = textEncripted,
                textDesencripted = textDesencripted
            });

        }

    }
}
