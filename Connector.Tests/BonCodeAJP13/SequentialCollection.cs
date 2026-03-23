using Xunit;

namespace Connector.Tests.BonCodeAJP13
{
    /// <summary>
    /// Collection definition that disables parallelization for test classes
    /// sharing static BonCodeAJP13Settings (LOG_LEVEL, LOG_DIR).
    /// Without this class, [Collection("Sequential")] on test classes is a no-op
    /// and tests can race on the shared static state.
    /// </summary>
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SequentialCollection { }
}