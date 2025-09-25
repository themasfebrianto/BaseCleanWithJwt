using BaseCleanWithJwt.Api.Common.Response;
using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Domain.DTO.AuthDTO;
using BaseCleanWithJwt.Domain.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniBlog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    private readonly IUserService userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var response = await userService.GetAllAsync();
            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Success", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetAll users failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var response = await userService.GetByIdAsync(id);
            if (response == null)
                return CustomResponse.StatusCode(StatusCodes.Status404NotFound, "User not found.");

            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Success", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Get user by ID failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
    {
        try
        {
            var user = await userService.GetByEmailAsync(request.Email) ?? throw new Exception("User not found");
            var result = await userService.ChangePasswordAsync(user.Id, request.OldPassword, request.NewPassword);
            if (!result)
                return CustomResponse.StatusCode(StatusCodes.Status400BadRequest, "Password change failed. Old password may be incorrect.");

            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Password changed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Change password failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateUser([FromBody] UserRequestDTO request)
    // {
    //     try
    //     {
    //         var response = await userService.InsertOneAsync(request);
    //         return CustomResponse.StatusCode(StatusCodes.Status201Created, "User created successfully.", response);
    //     }
    //     catch (Exception ex)
    //     {
    //         return CustomResponse.Exception(ex);
    //     }
    // }
}