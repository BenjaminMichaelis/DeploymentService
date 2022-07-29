using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace DeploymentService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SquirrelController : ControllerBase
{
    [HttpGet("{appName}/RELEASES")]
    public string Get(string appName)
    {
        string? rootRepo = AppMap.GetRepo(appName);
        if (rootRepo == null)
        {
            return "Not Found";
        }
        return $"{appName}";

    }

    [HttpGet("{appName}/{fileName}")]
    public IActionResult Get(string appName, string fileName)
    {
        return Get(appName, "production", fileName);
    }

    [HttpGet("{appName}/{channel}/{fileName}")]
    public IActionResult Get(string appName, string channel, string fileName)
    {
        string? rootRepo = AppMap.GetRepo(appName);
        if (rootRepo is null)
        {
            return NotFound();
        }
        Uri fileUri = new Uri(rootRepo, UriKind.Absolute);
        if (string.Equals("RELEASES", fileName, StringComparison.OrdinalIgnoreCase))
        {
            fileUri = new Uri(fileUri, "releases/latest/download/RELEASES");
            return Redirect(fileUri.AbsoluteUri);
        }
        var match = Regex.Match(fileName, @".+-(?<Version>\d+\.\d\.\d)-.+");
        if (!match.Success)
        {
            return BadRequest();
        }
        string version = match.Groups["Version"].Value;
        fileUri = new Uri(fileUri, $"releases/download/{version}/{fileName}");
        return Redirect(fileUri.AbsoluteUri);
    }
}

public static class AppMap
{
    public static string? GetRepo(string appName)
    {
        if (string.Equals("SpreadsheetApplication", appName, StringComparison.OrdinalIgnoreCase))
        {
            return "https://github.com/BenjaminMichaelis/SpreadsheetApplication/";
        }
        return null;
    }
}