# Contributing to Fox.ChainKit

Thank you for your interest in contributing to Fox.ChainKit! This document provides guidelines and instructions for contributing to the project.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for all contributors.

## How to Contribute

### Reporting Issues

If you find a bug or have a feature request:

1. Check if the issue already exists in the [GitHub Issues](https://github.com/akikari/Fox.ChainKit/issues)
2. If not, create a new issue with:
   - Clear, descriptive title
   - Detailed description of the problem or feature
   - Steps to reproduce (for bugs)
   - Expected vs actual behavior
   - Code samples if applicable
   - Environment details (.NET version, OS, etc.)

### Submitting Changes

1. **Fork the repository** and create a new branch from `main`
2. **Make your changes** following the coding guidelines below
3. **Write or update tests** for your changes
4. **Update documentation** if needed (README, XML comments)
5. **Ensure all tests pass** (`dotnet test`)
6. **Ensure build succeeds** (`dotnet build`)
7. **Submit a pull request** with:
   - Clear description of changes
   - Reference to related issues
   - Summary of testing performed

## Coding Guidelines

Fox.ChainKit follows strict coding standards. Please review the [Copilot Instructions](.github/copilot-instructions.md) for detailed guidelines.

### Key Standards

#### General
- **Language**: All code, comments, and documentation must be in English
- **Line Endings**: CRLF
- **Indentation**: 4 spaces (no tabs)
- **Namespaces**: File-scoped (`namespace MyNamespace;`)
- **Nullable**: Enabled
- **Language Version**: latest

#### Naming Conventions
- **Private Fields**: camelCase without underscore prefix (e.g., `value`, not `_value`)
- **Public Members**: PascalCase
- **Local Variables**: camelCase

#### Code Style
- Use expression-bodied members for simple properties and methods
- Use auto-properties where possible
- Prefer `var` only when type is obvious
- Maximum line length: 100 characters
- Add blank line after closing brace UNLESS next line is also `}`

#### Documentation
- **XML Comments**: Required for all public APIs
- **Language**: English
- **Decorators**: 98 characters width using `//======` (no space after prefix)
- **File Headers**: 3-line header (purpose + technical description + decorators)

Example:
```csharp
//==================================================================================================
// Implements the chain execution engine.
// Executes handlers sequentially with diagnostics and exception handling support.
//==================================================================================================

namespace Fox.ChainKit;

//==================================================================================================
/// <summary>
/// Implements the chain of responsibility pattern for executing handlers sequentially.
/// </summary>
/// <typeparam name="TContext">The type of context object to process.</typeparam>
//==================================================================================================
public sealed class Chain<TContext> : IChain<TContext>
{
    private readonly IServiceProvider serviceProvider;

    //==============================================================================================
    /// <summary>
    /// Initializes a new instance of the <see cref="Chain{TContext}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve handlers.</param>
    //==============================================================================================
    public Chain(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task RunAsync(TContext context, CancellationToken cancellationToken = default)
    {
        // Implementation
    }
}
```

## Testing Requirements

- **Framework**: xUnit
- **Assertions**: FluentAssertions
- **Test Naming**: `MethodName_should_expected_behavior`
- **Coverage**: Aim for 100% coverage of new code
- **Test Structure**:
  - Arrange: Setup test data
  - Act: Execute the method under test
  - Assert: Verify expected behavior

Example:
```csharp
[Fact]
public async Task RunAsync_should_execute_handlers_in_order()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddTransient<TestHandlerA>();
    services.AddTransient<TestHandlerB>();
    var serviceProvider = services.BuildServiceProvider();

    var chain = new ChainBuilder<TestContext>(serviceProvider)
        .AddHandler<TestHandlerA>()
        .AddHandler<TestHandlerB>()
        .Build();

    var context = new TestContext();

    // Act
    await chain.RunAsync(context);

    // Assert
    context.ExecutedHandlers.Should().ContainInOrder(
        nameof(TestHandlerA),
        nameof(TestHandlerB));
}
```

## Architecture Principles

Fox.ChainKit follows Clean Architecture and SOLID principles:

- **Single Responsibility**: Each class has one clear purpose
- **Open/Closed**: Open for extension (via interfaces), closed for modification
- **Liskov Substitution**: All implementations are substitutable
- **Interface Segregation**: Small, focused interfaces
- **Dependency Inversion**: Depend on abstractions, not concretions

### Design Guidelines

- **No Reflection**: All handler resolution through DI
- **No Runtime Discovery**: Handlers are explicitly registered
- **No Magic**: Behavior is explicit and predictable
- **Explicit API**: Clear method names and parameters
- **DI-First**: Designed for dependency injection from the ground up

## Project Structure

```
Fox.ChainKit/
├── src/
│   ├── Fox.ChainKit/              # Core package
│   │   ├── *.cs                   # Public interfaces and types
│   │   ├── Internal/              # Internal implementation details
│   │   └── Extensions/            # Extension methods
│   └── Fox.ChainKit.ResultKit/    # ResultKit integration
│       ├── *.cs                   # Public interfaces
│       ├── Internal/              # Internal adapters
│       └── Extensions/            # Extension methods
├── tests/
│   ├── Fox.ChainKit.Tests/        # Core tests
│   └── Fox.ChainKit.ResultKit.Tests/  # ResultKit tests
└── samples/
    └── Fox.ChainKit.Demo/         # Demo application
```

## Pull Request Process

1. **Update tests**: Ensure your changes are covered by tests
2. **Update documentation**: Keep README and XML comments up to date
3. **Follow coding standards**: Use provided `.editorconfig` and copilot instructions
4. **Keep commits clean**: 
   - Use clear, descriptive commit messages
   - Squash commits if needed before merging
5. **Update CHANGELOG.md**: Add entry under `[Unreleased]` section
6. **Ensure CI passes**: All tests must pass and build must succeed

### Commit Message Format

Use clear, imperative commit messages:

```
Add support for async conditional handlers

- Implement AddConditionalHandlerAsync method
- Add unit tests for async conditions
- Update documentation
```

## Feature Requests

When proposing new features, please consider:

1. **Scope**: Does this fit the focused nature of Fox.ChainKit?
2. **Complexity**: Does this add unnecessary complexity?
3. **Dependencies**: Does this require new external dependencies?
4. **Breaking Changes**: Will this break existing code?
5. **Use Cases**: What real-world scenarios does this address?

Fox.ChainKit aims to be lightweight and focused. Features should align with the core Chain of Responsibility pattern.

## Development Setup

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022+ or Rider (recommended)
- Git

### Getting Started

1. Clone the repository:
```bash
git clone https://github.com/akikari/Fox.ChainKit.git
cd Fox.ChainKit
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run tests:
```bash
dotnet test
```

5. Run the sample application:
```bash
dotnet run --project samples/Fox.ChainKit.Demo/Fox.ChainKit.Demo.csproj
```

## Questions?

If you have questions about contributing, feel free to:
- Open a [GitHub Discussion](https://github.com/akikari/Fox.ChainKit/discussions)
- Create an issue labeled `question`
- Reach out to the maintainers

## License

By contributing to Fox.ChainKit, you agree that your contributions will be licensed under the MIT License.

Thank you for contributing to Fox.ChainKit! 🎉
