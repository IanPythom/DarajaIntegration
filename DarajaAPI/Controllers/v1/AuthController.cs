using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using DarajaAPI.Models.Domain;
using MailServiceAPI.Services.OAuth2;
using DarajaAPI.Models.Dto;

namespace DarajaAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiversion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager; // Added SignInManager
        private readonly Serilog.ILogger _logger;
        private readonly ITokenRepos _tokenRepos;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, Serilog.ILogger logger, ITokenRepos tokenRepos)
        {
            _userManager = userManager;
            _signInManager = signInManager; // Initialize SignInManager
            _logger = logger;
            _tokenRepos = tokenRepos;
        }

        // REGISTRATION DOESN'T USE SIGNIN MANAGER
        /// <summary>
        /// - This endpoint registers a new user into the system by accepting valid identity user credentials (username & password).
        /// </summary>
        /// <remarks>
        /// This endpoint registers a new user into the system by accepting valid identity user credentials (username & password).
        /// If the registration is successful, it returns a 200 status code with a success message.
        /// If the registration fails (e.g., invalid username or password), it returns a 400 status code.
        /// </remarks>
        /// <param name="registerRequestDto">The registration request data.</param>
        /// <returns>
        /// On successful login (User is registered successfully! please login) on failure (The username or password is invalid).
        /// </returns>
        /// <response code="200">User is registered successfully! please login.</response>
        /// <response code="400">The username or password is invalid.</response>
        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            try
            {
                var identityUser = new ApplicationUser
                {
                    UserName = registerRequestDto.UserName,
                    Email = registerRequestDto.UserName
                };

                var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

                if (identityResult.Succeeded)
                {
                    //Add roles to user
                    if (registerRequestDto.Password != null && registerRequestDto.Roles.Any())
                    {
                        identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                        if (identityResult.Succeeded)
                        {
                            _logger.Information($"User has been registered with the following details: {JsonSerializer.Serialize(identityUser)}");
                            return Ok("User is registered succesfully! please login");
                        }
                    }
                }
                return BadRequest("The username or password is invalid");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return Problem("Something went wrong", null, (int)HttpStatusCode.InternalServerError);
            }
        }

        // LOGIN
        /// <summary>
        /// - This endpoint validates users using their username and password in the LoginRequestDto.
        /// </summary>
        /// <remarks>
        /// This endpoint validates users based on their username and password provided in the LoginRequestDto.
        /// If the login is successful, it returns a 200 status code with a JWT token.
        /// If the login fails (username or password is incorrect), it returns a 400 status code.
        /// </remarks>
        /// <param name="loginRequestDto">The login request data.</param>
        /// <returns>
        /// Returns a response message on success (The following user identity user has been logged in) on failure (Username or password is incorrect).
        /// </returns>
        /// <response code="200">On Successful login.</response>
        /// <response code="400">On login Failure.</response>
        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginRequestDto.UserName) ?? await _userManager.FindByEmailAsync(loginRequestDto.UserName);
                if (user == null)
                {
                    return BadRequest("Username or password is incorrect");
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginRequestDto.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var jwtToken = _tokenRepos.GenerateJWT(user, roles.ToList());

                    var response = new LoginResponseDto
                    {
                        JWTToken = jwtToken,
                    };
                    _logger.Information($"The following user {JsonSerializer.Serialize(user)} has been logged in");
                    return Ok(response);
                }
                return BadRequest("Username or password is incorrect");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return Problem("Something went wrong", null, (int)HttpStatusCode.InternalServerError);
            }
        }

    }
}