using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Application.DTOs.userAuthDto;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using AuthTokenServices;

using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{


    private readonly UserManager<User> _userManager;
    private readonly AuthTokenService _tokenService;
    public UserController(UserManager<User> userManager, AuthTokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> AddUser([FromBody] RegisterDto userDot)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

            return BadRequest(errors);
        }


        var doesEmailExist = await _userManager.FindByEmailAsync(userDot.email);
        if (doesEmailExist != null)
        {
            return BadRequest("Email already exists");
        }


        var user = new User
        {
            Email = userDot.email,
            UserName = userDot.email
        };


        var result = await _userManager.CreateAsync(user, userDot.password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(_tokenService.CreateUserWithToken(user));

    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await _userManager.Users
                    .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email");
        }

        var isPasswordValied = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValied)
        {
            return Unauthorized("Invalid password");
        }

        return Ok(_tokenService.CreateUserWithToken(user));

    }


    [Authorize]
    [HttpGet("testAuth")]
    public IActionResult AuthTest()
    {
        return Ok("authorization is working");
    }



    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {

        var user = await _userManager.Users
        .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

    // get all users
    [Authorize]
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

}
