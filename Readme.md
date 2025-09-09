# DotnetAIProjects

This repository contains multiple .NET projects focused on AI, server utilities, PDF translation, and prompt engineering. Each project is organized in its own folder and serves a distinct purpose.

## Projects Overview

### FunkyMCP

A .NET console application for setting up the local MCP Server with its own Tool in the VS Code.  
**Path:** `FunkyMCP/`

- Entry point: `Program.cs`
- Solution: `FunkyMCP.sln`
- Purpose: General utility or demonstration (details can be expanded).

### MCPServer

A server-side .NET application with supporting tools and web API using HTTP Client.  
**Path:** `MCPServer/`

- Main app: `Program.cs`, `MCPServer.csproj`
- Web API: `MCPServer.WebApi/` (controllers, models, wrappers)
- Test project: `MCPServer.Test/`
- Tools: `EchoTool.cs`
- Config: `appsettings.json`, `appsettings.Development.json`
- Purpose: Server utilities and web API for MCP-related tasks.

### PromptExample

A .NET Web API project demonstrating prompt engineering and OpenAI integration.  
**Path:** `PromptExample/`

- API controllers: `Controllers/`
- Data models: `Models/`
- Services: `Services/OpenAIService.cs`
- Database context: `Data/AppDbContext.cs`
- Config: `appsettings.json`, `appsettings.Development.json`
- Purpose: Example of using prompts and OpenAI in a web API.

### TranslatePDF

A .NET Web API for PDF translation from English to Hindi intacting the translation and file upload.  
**Path:** `TranslatePDF/`

- API controllers: `Controllers/UploadFileController.cs`
- Services: `Services/TranslateService.cs`, `Services/UploadFileService.cs`
- Resources: `Resources/Test.pdf`
- Config: `appsettings.json`, `appsettings.Development.json`
- Purpose: Upload and translate PDF files via API.

## Getting Started

1. Clone the repository.
2. Open the desired solution file (`*.sln`) in Visual Studio.
3. Restore NuGet packages and build the project.
4. Update configuration files (`appsettings.json`) as needed.
5. Run the project using Visual Studio or `dotnet run`.

## Folder Structure

- Each project has its own folder, source files, and configuration.
- `bin/` and `obj/` folders contain build outputs.
- `Properties/` folders contain launch settings for debugging.

## Requirements

- .NET 9.0 SDK or compatible runtime
- Visual Studio 2022+ or VS Code
- Open AI API key for projects using OpenAI services

## Contributing

Pull requests and issues are welcome. Please open an issue to discuss changes.
