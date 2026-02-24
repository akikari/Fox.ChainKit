//==================================================================================================
// Makes internal types visible to test projects.
// Allows defensive code testing without making internals public.
//==================================================================================================
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fox.ChainKit.ResultKit.Tests")]
