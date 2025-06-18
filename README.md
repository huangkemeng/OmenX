# OmenX - Health Monitoring for .NET Applications

## Introduction
OmenX provides comprehensive health monitoring for your .NET applications through customizable checkpoints and an intuitive UI.

## Installation
Add the OmenX package to your project using the .NET CLI:

```shell
dotnet add package OmenX --version 1.1.2
```

## Quick Start

### Basic Integration
Add OmenX to your ASP.NET Core application:

```cs
var builder = WebApplication.CreateBuilder(args);

// Register OmenX services
builder.Services.AddOmenX(typeof(Core).Assembly); // or builder.Services.AddOmenX();

var app = builder.Build();

// Enable OmenX middleware
app.UseOmenX();

// Other application configuration
```

## Creating Checkpoints

### Implementing a Checkpoint
Create custom health checks by implementing `IOmenXCheckPoint`:

```cs
[CheckPointMetadata(
    Title = "Database Connectivity Check", 
    Description = "Verifies database connection health")]
public class DatabaseCheck : IOmenXCheckPoint
{
    public Task CheckAsync(OmeXCheckPointContext context)
    {
        // Implement your check logic
        context.Success(true, "Database connection successful");
        return Task.CompletedTask;
    }
}
```

## Using the Dashboard

### Accessing the UI
After setup, access the OmenX dashboard at:
```
https://[your-server]/omenx-ui
```

### Performing Checks
- **Start Check**: Executes all checks sequentially
- **Individual Check**: Run a specific checkpoint

![OmenX Dashboard](omenx-1.png "OmenX Dashboard Interface")

### Viewing Results
Checkpoint results are displayed with detailed status information:

![Check Results](image.png "OmenX Check Results")

## Advanced Features

### Swagger Integration
Enhance your API documentation with OmenX endpoints:

```cs
builder.Services.AddSwaggerGen(options =>
{
    options.AddOmenXApiDoc();
});

app.UseSwaggerUI(options =>
{
    options.UseOmenXApiDoc();
});
```

![Swagger Integration](image-1.png "OmenX in Swagger UI")

## Best Practices
- Implement checkpoints for critical system components
- Use descriptive metadata for each checkpoint
- Monitor frequently accessed endpoints
- Combine with your existing monitoring solutions
```

Establish monitoring checkpoints for early issue detection!
