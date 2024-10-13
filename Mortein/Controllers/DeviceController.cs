using Microsoft.AspNetCore.Mvc;
using Mortein.Types;

namespace Mortein.Controllers;

/// <summary>
/// API controller for devices.
/// </summary>
///
/// <param name="context">The context which enables interaction with the database.</param>
[ApiController]
[Route("[controller]")]
public class DeviceController(DatabaseContext context) : ControllerBase
{
    /// The context which enables interaction with the database.
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Get All Devices
    /// </summary>
    ///
    /// <remarks>
    /// Retrieve all devices.
    /// </remarks>
    [HttpGet()]
    [ProducesResponseType<IEnumerable<Device>>(StatusCodes.Status200OK)]
    public IEnumerable<Device> GetAllDevices()
    {
        return [.. _context.Devices];
    }

    /// <summary>
    /// Get Device
    /// </summary>
    ///
    /// <remarks>
    /// Retrieve a device by ID.
    /// </remarks>
    ///
    /// <returns>The device.</returns>
    ///
    /// <param name="id">The ID of the device to get.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<Device>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Device?>> GetDevice(Guid id)
    {
        Device? device = await _context.Devices.FindAsync(id);

        return device == null ? (ActionResult<Device?>)NotFound() : (ActionResult<Device?>)device;
    }

    /// <summary>
    /// Create Device
    /// </summary>
    ///
    /// <remarks>
    /// Create a new device.
    /// </remarks>
    ///
    /// <returns>The created device.</returns>
    ///
    /// <param name="name">The name of the device to create.</param>
    [HttpPost()]
    [ProducesResponseType<Device>(StatusCodes.Status201Created)]
    public async Task<ActionResult<Device>> CreateDevice(string name)
    {
        Device device = new() { Id = Guid.NewGuid(), Name = name };

        await _context.Devices.AddAsync(device);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDevice), new { device.Id }, device);
    }

    /// <summary>
    /// Update Device
    /// </summary>
    ///
    /// <remarks>
    /// Update a device by ID.
    /// </remarks>
    ///
    /// <returns>The updated device.</returns>
    ///
    /// <param name="id">The ID of the device to update.</param>
    /// <param name="name">The updated name of the device.</param>
    [HttpPut("{id}")]
    [ProducesResponseType<Device>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Device>> UpdateDevice(Guid id, string name)
    {
        Device? device = await _context.Devices.FindAsync(id);

        if (device == null)
        {
            return NotFound();
        }

        device.Name = name;

        await _context.SaveChangesAsync();

        return device;
    }

    /// <summary>
    /// Delete Device
    /// </summary>
    ///
    /// <remarks>
    /// Delete a device by ID.
    /// </remarks>
    ///
    /// <param name="id">The ID of the device to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        Device? device = await _context.Devices.FindAsync(id);

        if (device == null)
        {
            return NotFound();
        }

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
