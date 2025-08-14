using HotelCostaAzul.API.Models.Entities;
using HotelCostaAzul.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Hotelify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var roles = _rolesService.GetAll();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var role = _rolesService.GetById(id);
            if (role == null)
                return NotFound($"Rol con ID {id} no encontrado.");
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Roles role)
        {
            try
            {
                var createdRole = await _rolesService.Create(role);
                return CreatedAtAction(nameof(Get), new { id = createdRole.Id }, createdRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el rol: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Roles role)
        {
            try
            {
                var updatedRole = await _rolesService.Update(id, role);
                return Ok(updatedRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el rol: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _rolesService.Delete(id);
                if (!success)
                    return NotFound($"Rol con ID {id} no encontrado.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el rol: {ex.Message}");
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Hotelify.API.Models.Entities;
using Hotelify.API.Services;

namespace Hotelify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var roles = _rolesService.GetAll();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var role = _rolesService.GetById(id);
            if (role == null)
                return NotFound($"Rol con ID {id} no encontrado.");
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Roles role)
        {
            try
            {
                var createdRole = await _rolesService.Create(role);
                return CreatedAtAction(nameof(Get), new { id = createdRole.Id }, createdRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el rol: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Roles role)
        {
            try
            {
                var updatedRole = await _rolesService.Update(id, role);
                return Ok(updatedRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el rol: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _rolesService.Delete(id);
                if (!success)
                    return NotFound($"Rol con ID {id} no encontrado.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el rol: {ex.Message}");
            }
        }
    }
}
