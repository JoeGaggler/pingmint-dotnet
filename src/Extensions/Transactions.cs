using System.Transactions;

namespace Pingmint;

public static class TransactionExtensions
{
    /// <summary>
    /// Create a new <see cref="TransactionScope"/> with the specified <see cref="TransactionScopeOption"/>, also with async flow enabled.
    /// </summary>
    /// <param name="option">Expected scope option</param>
    public static TransactionScope CreateForAsync(this TransactionScopeOption option) => new(option, TransactionScopeAsyncFlowOption.Enabled);
}
