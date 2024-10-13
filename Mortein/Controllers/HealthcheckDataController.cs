using Microsoft.AspNetCore.Mvc;
using Mortein.Types;

namespace Mortein.Controllers;

/// <summary>
/// API controller for healthcheck data.
/// </summary>
///
/// <param name="context">The context which enables interaction with the database.</param>
[ApiController]
[Route("Device/{deviceId}/[controller]")]
public class HealthcheckDataController(DatabaseContext context) : ControllerBase
{
    /// The context which enables interaction with the database.
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Get All Healthcheck Data for Device
    /// </summary>
    ///
    /// <remarks>
    /// Retrieve all healthcheck data by device ID.
    ///
    /// The data is in descending order by timestamp; that is, the latest datum is first.
    /// </remarks>
    [HttpGet()]
    [ProducesResponseType<IEnumerable<HealthcheckDatum>>(StatusCodes.Status200OK)]
    public IEnumerable<HealthcheckDatum> GetAllHealthcheckDataForDevice(Guid deviceId)
    {
        return [.. _context.HealthcheckData
            .Where(datum => datum.DeviceId == deviceId)
            .OrderByDescending(datum => datum.Timestamp)
        ];
    }

    /// <summary>
    /// Get Latest Healthcheck Datum for Device
    /// </summary>
    ///
    /// <remarks>
    /// Retrieve the latest healthcheck datum by device ID.
    /// </remarks>
    ///
    /// <returns>The latest datum for the device.</returns>
    ///
    /// <param name="deviceId">The ID of the device for which to get the latest datum.</param>
    [HttpGet("Latest")]
    [ProducesResponseType<HealthcheckDatum>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<HealthcheckDatum?> GetLatestHealthcheckDatumForDevice(Guid deviceId)
    {
        try
        {
            return _context.HealthcheckData
                .Where(datum => datum.DeviceId == deviceId)
                .OrderByDescending(datum => datum.Timestamp)
                .First();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
