# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Build the solution
dotnet build availability-compass.sln

# Build release
dotnet build -c Release

# Run tests
dotnet test tests/AvailabilityCompass.Core.Tests.Unit/

# Run a single test
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run the WPF application
dotnet run --project src/AvailabilityCompass.WpfClient/
```

## Architecture

This is a .NET 10.0 WPF desktop application using **Vertical Slice Architecture (VSA)** with **MVVM**.

### Key Architectural Decisions

- **Vertical Slice Architecture**: Features are self-contained slices cutting through all layers. Each slice contains its own Commands, Queries, ViewModels, and handlers colocated together.
- **MVVM Pattern**: ViewModels in Core project are framework-agnostic (using CommunityToolkit.MVVM) for potential MAUI reuse.
- **Two-Project Split**: `AvailabilityCompass.Core` contains ViewModels and business logic; `AvailabilityCompass.WpfClient` contains XAML views and WPF-specific code.

### Communication Patterns

- **Pull (Synchronous)**: MediatR for command/query processing between slices
- **Push (Asynchronous)**: Custom EventBus using Reactive Extensions (Rx.NET) for cross-slice events

### Project Structure

```
src/AvailabilityCompass.Core/
├── Features/
│   ├── ManageCalendars/    # Calendar CRUD, single/recurring dates
│   ├── ManageSources/      # Trip source integrations (web scraping)
│   ├── SearchRecords/      # Search functionality
│   └── ManageSettings/     # Application settings
└── Shared/                 # EventBus, Navigation, Database, Validators

src/AvailabilityCompass.WpfClient/
├── Pages/                  # XAML views matching Core features
└── Application/            # DI extensions, Bootstrapper
```

### Adding New Source Integrations

Implement `ISourceService` interface and mark with `[SourceService]` attribute. See existing implementations in `Features/ManageSources/Sources/` (e.g., BarentsEngService).

### Database

SQLite with Dapper ORM. Custom type handlers exist for `DateOnly` and `Guid`. Connection configured in `appsettings.json`.

## Testing

- **Framework**: xUnit
- **Mocking**: NSubstitute (preferred), Moq available
- **Assertions**: Shouldly
- Test files mirror the feature structure under `tests/AvailabilityCompass.Core.Tests.Unit/Features/`

## Key Dependencies

- CommunityToolkit.Mvvm - MVVM base classes (ObservableValidator, RelayCommand)
- MediatR - Mediator pattern for commands/queries
- System.Reactive - EventBus implementation
- HtmlAgilityPack - HTML parsing for web scraping sources
- MaterialDesignThemes - UI theming
