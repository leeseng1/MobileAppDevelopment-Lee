using FitnessTracker.Api.DTOs;
using FitnessTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers;


/// <summary>
/// Provides endpoints for managing workouts.
/// </summary>
/// <remarks>Supports listing, retrieving, creating, updating, and deleting workouts.</remarks>
[ApiController]
[Route("api/workouts")]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutService _service;

    public WorkoutsController(IWorkoutService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkoutResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? type, CancellationToken ct)
    {
        var workouts = await _service.GetAllAsync(type, ct);
        return Ok(workouts);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var workout = await _service.GetByIdAsync(id, ct);
        return workout is null ? NotFound() : Ok(workout);
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkoutResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(WorkoutRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);

        return result.Outcome switch
        {
            WorkoutOutcome.Success => CreatedAtAction(
                nameof(GetById), new { id = result.Workout!.Id }, result.Workout),
            WorkoutOutcome.Duplicate => DuplicateProblem(),
            _ => throw new InvalidOperationException($"Unexpected outcome: {result.Outcome}")
        };
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(WorkoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, WorkoutRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);

        return result.Outcome switch
        {
            WorkoutOutcome.Success => Ok(result.Workout),
            WorkoutOutcome.NotFound => NotFound(),
            WorkoutOutcome.Duplicate => DuplicateProblem(),
            _ => throw new InvalidOperationException($"Unexpected outcome: {result.Outcome}")
        };
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var outcome = await _service.DeleteAsync(id, ct);
        return outcome == WorkoutOutcome.Success ? NoContent() : NotFound();
    }

    private ObjectResult DuplicateProblem() => Problem(
        title: "Duplicate workout",
        detail: "A workout of this activity type is already logged for that day.",
        statusCode: StatusCodes.Status409Conflict);
}