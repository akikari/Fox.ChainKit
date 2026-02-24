//==================================================================================================
// Makes internal types visible to Fox.ChainKit.ResultKit and test projects.
// Allows ResultKit integration and defensive code testing without making internals public.
//==================================================================================================
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fox.ChainKit.ResultKit")]
[assembly: InternalsVisibleTo("Fox.ChainKit.Tests")]
