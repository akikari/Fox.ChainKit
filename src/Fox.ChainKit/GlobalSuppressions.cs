//==================================================================================================
// Global assembly-level suppressions for Code Analyzer.
// Suppresses false positive warnings that don't apply to this library.
//==================================================================================================
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Scope = "type", Target = "~T:Fox.ChainKit.IChainBuilder`1", Justification = "Handler-related naming (AddHandler, IHandler) is industry standard in .NET (e.g., WPF AddHandler/RemoveHandler). VB.NET keyword conflict is acceptable for a C# library.")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Scope = "member", Target = "~M:Fox.ChainKit.IChainBuilder`1.Build~Fox.ChainKit.IChain{`0}", Justification = "Build is the industry standard naming for builder patterns (e.g., StringBuilder, WebApplicationBuilder).")]
