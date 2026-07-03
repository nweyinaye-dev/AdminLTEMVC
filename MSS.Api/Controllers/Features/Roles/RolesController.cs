using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSS.Api.Authorization;
using MSS.Domain.Entities;
using MSS.Domain.Features.Roles;

namespace MSS.Api.Controllers.Features.Roles;

/// <summary>
/// Roles API controller - Feature-based organization
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RolesController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    // GET: api/Roles
    [HttpGet]
   [HasPermission("role:list")]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(roles);
    }

    // GET: api/Roles/5
    [HttpGet("{id}")]
   // [HasPermission("role:list")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(role);
    }

    // GET: api/Roles/5/permissions
    [HttpGet("{id}/permissions")]
    //[HasPermission("role:list")]
    public async Task<ActionResult<Role>> GetRoleWithPermissions(int id)
    {
        var role = await _roleRepository.GetRoleWithPermissionsAsync(id);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(role);
    }

    // POST: api/Roles
    [HttpPost]
    //[HasPermission("role:create")]
    public async Task<ActionResult<Role>> CreateRole(Role role)
    {
        var id = await _roleRepository.CreateAsync(role);
        role.Id = id;
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
    }

    // PUT: api/Roles/5
    [HttpPut("{id}")]
   // [HasPermission("role:edit")]
    public async Task<IActionResult> UpdateRole(int id, Role role)
    {
        if (id != role.Id)
        {
            return BadRequest();
        }

        var result = await _roleRepository.UpdateAsync(role);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Roles/5
    [HttpDelete("{id}")]
  //  [HasPermission("role:delete")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var result = await _roleRepository.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Roles/5/permissions/{permissionId}
    [HttpPost("{id}/permissions/{permissionId}")]
    public async Task<IActionResult> AssignPermission(int id, int permissionId)
    {
        var result = await _roleRepository.AssignPermissionAsync(id, permissionId);
        if (!result)
        {
            return BadRequest();
        }

        return NoContent();
    }

    // DELETE: api/Roles/5/permissions/{permissionId}
    [HttpDelete("{id}/permissions/{permissionId}")]
    public async Task<IActionResult> RemovePermission(int id, int permissionId)
    {
        var result = await _roleRepository.RemovePermissionAsync(id, permissionId);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

