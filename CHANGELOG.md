# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

_No unreleased changes yet._

## [1.0.0] - 2026-02-20

### Added

#### Fox.ChainKit (Core Library)
- Chain of Responsibility pattern implementation with fluent API
- `IHandler<TContext>` interface for implementing handlers
- `IChain<TContext>` interface for executing chains
- `IChainBuilder<TContext>` fluent builder for constructing chains
- `HandlerResult` enum (Continue/Stop) for controlling chain flow
- Conditional handler support with predicate-based execution
- Exception handling with custom exception handlers
- Built-in diagnostics with execution timing and early stop detection
- `ChainDiagnostics` and `HandlerDiagnostics` for performance monitoring
- Zero reflection overhead (factory-based handler resolution)
- Full dependency injection integration
- XML documentation for all public APIs

#### Fox.ChainKit.ResultKit (Integration Package)
- ResultKit integration for Railway Oriented Programming
- `IResultHandler<TContext>` interface for Result-based handlers
- Automatic conversion between `Result` and `HandlerResult`
- `AddResultHandler<TContext, THandler>()` extension methods
- `AddConditionalResultHandler<TContext, THandler>()` for conditional Result handlers
- Result diagnostics with `FormatResultDiagnostics()` extension
- Seamless integration with Fox.ResultKit

#### Documentation
- Comprehensive README.md with architecture diagrams
- Design principles documentation
- Contributing guidelines with build policy
- MIT License (2026)
- Comparison table vs MediatR and PipelineNet

#### Samples
- Demo application demonstrating all features:
  - Basic chain execution
  - Early exit scenarios
  - Result-based chains
  - Conditional handlers
  - Diagnostics usage
- Separated handlers into individual files with proper namespacing
- Logger abstraction for clean separation of concerns

#### Tests
- 90 comprehensive unit tests (100% passing)
- Fox.ChainKit.Tests: 55 tests covering:
  - Chain execution scenarios
  - Conditional handlers
  - Exception handling
  - Diagnostics tracking
  - Dependency injection integration
- Fox.ChainKit.ResultKit.Tests: 35 tests covering:
  - Result handler execution
  - Result-to-HandlerResult conversion
  - Conditional Result handlers
  - Result diagnostics

#### Build & CI/CD
- Multi-targeting: .NET 8.0, .NET 9.0, .NET 10.0
- NuGet package metadata with icon and README embedding
- Symbol packages (.snupkg) with portable PDB and embedded sources
- Global usings for common namespaces

### Initial Release
- Production-ready code quality
- All nullable reference types enabled
- Follows Microsoft coding conventions
- Comprehensive XML documentation in English
- Full test coverage
- CRLF line endings and UTF-8 encoding (BOM-less)
