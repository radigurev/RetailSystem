namespace CentralApp.Models;

/// <summary>
/// DTO for Config DB Entity
/// </summary>
/// <param name="Key"></param>
/// <param name="Value"></param>
/// <param name="Description"></param>
internal record ConfigDTO(
    string Key,
    string Value,
    string Description);