using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PomodoroTimer.Api.Entities;
using PomodoroTimer.Api.Persistence;
using PomodoroTimer.Api.Services;
using PomodoroTimer.Common.Account;
using System.Security.Claims;

namespace PomodoroTimer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<PomodoroUser> userManager;
        private readonly TokenGenerator tokenGenerator;
        private PomodoroTimerDbContext dbContext;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            UserManager<PomodoroUser> userManager,
            TokenGenerator tokenGenerator,
            PomodoroTimerDbContext dbContext,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.tokenGenerator = tokenGenerator;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new RegisterResult { Errors = new[] { "Model is invalid." } });

            var user = new PomodoroUser { UserName = model.Email, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return Ok(new RegisterResult { Errors = result.Errors?.Select(e => e.Description) });

            var createdUser = await userManager.FindByEmailAsync(user.Email);
            if (createdUser is null)
                return Ok(new RefreshTokenResult { Error = "Error on check existing new user." });

            var addRoleClaimRsult = await userManager.AddClaimAsync(createdUser, new Claim(ClaimTypes.Role, "freeUser"));
            if (!addRoleClaimRsult.Succeeded)
                return Ok(new RefreshTokenResult { Error = "Error on add default role claim." });

            logger.LogInformation("Register a new user.");

            var tokens = await tokenGenerator.GetTokens(createdUser);
            var registerResult = new RegisterResult
            {
                Succeeded = true,
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken
            };

            return Ok(registerResult);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new LoginResult { Error = "Model is invalid." });

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Ok(new LoginResult { Error = $"Invalid login attempt. Can't find user with these user and password." });

            var isValidPassword = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isValidPassword)
                return Ok(new LoginResult { Error = "Invalid login attempt." });

            logger.LogInformation("User logged in.");

            var tokens = await tokenGenerator.GetTokens(user);
            var result = new LoginResult
            {
                Succeeded = true,
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken
            };

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new RefreshTokenResult { Error = "Model is invalid." });

            var claimsPrincipal = tokenGenerator.Validate(model.RefreshToken);
            if (claimsPrincipal is null)
                return Ok(new RefreshTokenResult { Error = "Refresh token is invalid." });

            if (dbContext.BlackListRefreshTokens.Any(t => t.Token == model.RefreshToken))
            {
                logger.LogInformation("Refresh token already in black list.");
                return Ok(new RefreshTokenResult { Error = "Can't refresh token from black list." });
            }

            var user = await userManager.GetUserAsync(claimsPrincipal);

            await dbContext.BlackListRefreshTokens.AddAsync(new RefreshToken
            {
                Token = model.RefreshToken,
                User = user
            });
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Refresh token added to black list. (refresh)");

            var tokens = await tokenGenerator.GetTokens(user);
            var result = new RefreshTokenResult
            {
                Succeeded = true,
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken
            };

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new LogoutResult { Error = "Model is invalid." });

            var claimsPrincipal = tokenGenerator.Validate(model.RefreshToken);
            if (claimsPrincipal is null)
                return Ok(new LogoutResult { Error = "Refresh token is invalid." });

            if (dbContext.BlackListRefreshTokens.Any(t => t.Token == model.RefreshToken))
            {
                logger.LogInformation("Refresh token already in black list.");
                return Ok(new LogoutResult { Error = "Can't refresh token from black list." });
            }

            var user = await userManager.GetUserAsync(claimsPrincipal);

            await dbContext.BlackListRefreshTokens.AddAsync(new RefreshToken
            {
                Token = model.RefreshToken,
                User = user
            });
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Refresh token added to black list. (logout)");

            return Ok(new LogoutResult { Succeeded = true });
        }

        [Authorize]
        [HttpPost("updateUserRole")]
        public async Task<IActionResult> UpdateUserRoleClaim(UpdateUserRoleClaimModel model)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (user is null)
                return Ok(new UpdateUserRoleClaimResult { Error = "User doesn't exist." });

            var claim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (claim is null)
            {
                var addNewRoleClaimRsult = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, model.Role));
                if (!addNewRoleClaimRsult.Succeeded)
                    return Ok(new UpdateUserRoleClaimResult { Error = "Error on add new role claim." });
            }

            var removeRoleClaimResult = await userManager.RemoveClaimAsync(user, claim);
            if (!removeRoleClaimResult.Succeeded)
                return Ok(new UpdateUserRoleClaimResult { Error = "Error on remove role claim." });

            var addRoleClaimRsult = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, model.Role));
            if (!addRoleClaimRsult.Succeeded)
                return Ok(new UpdateUserRoleClaimResult { Error = "Error on update role claim." });

            return Ok(new UpdateUserRoleClaimResult { Succeeded = true });
        }
    }
}
