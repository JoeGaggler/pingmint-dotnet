using System.Transactions;
using Azure.Messaging.ServiceBus;

namespace Pingmint.Azure.ServiceBus;

/// <summary>
/// Extension methods for <see cref="ServiceBusSender"/>.
/// </summary>
public static class ServiceBusSenderExtensions
{
    /// <summary>
    /// Sends a message in a new transaction.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="message">Message</param>
    /// <param name="cancellationToken">Cancels the operation</param>
    /// <returns>Task that completes with the transaction</returns>
    public static async Task SendMessageDetachedAsync(this ServiceBusSender sender, ServiceBusMessage message, CancellationToken cancellationToken = default)
    {
        using var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        await sender.SendMessageAsync(message, cancellationToken);
        transactionScope.Complete();
    }
}
