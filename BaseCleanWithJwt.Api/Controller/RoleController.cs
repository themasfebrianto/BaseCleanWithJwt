using BaseCleanWithJwt.Api.Common.Response;
using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Domain.DTO.RoleDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseCleanWithJwt.Api.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleService roleService, ILogger<RoleController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await roleService.GetAllRoles();
            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Success", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetAll roles failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleRequestDTO request)
    {
        try
        {
            var response = await roleService.CreateRole(request);
            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Success", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create role failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RoleRequestDTO request)
    {
        try
        {
            var response = await roleService.UpdateRole(request);
            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Success", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update role failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Guid id)
    {
        try
        {
            var response = await roleService.DeleteRole(id);
            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Success", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete role failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }
}