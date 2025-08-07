using Microsoft.Extensions.Logging;

using MyApp.Application.Services.Interfaces;

namespace MyApp.ConsoleApp.Services;

public class AppRunner : IAppRunner
{
    private readonly IUserService _userService;
    private readonly ICatalogService _catalogService;
    private readonly ILogger<AppRunner> _logger;

    public AppRunner(IUserService userService, ICatalogService catalogService, ILogger<AppRunner> logger)
    {
        _userService = userService;
        _catalogService = catalogService;
        _logger = logger;
    }

    public async Task Run()
    {
        _logger.LogInformation("App started");

        int userId = 1;

        var userDto = await _userService.GetUserDetailsAsync(userId);

        _logger.LogInformation("Result: {Result}", userDto);

        var result = await _catalogService.AddCategoryAndProductAsync("Test Category", "Test Product");

        _logger.LogInformation("Result: {Result}", result);

        _logger.LogInformation("App finished");
    }
}