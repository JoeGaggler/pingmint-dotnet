using System.Transactions;
using Azure.Messaging.ServiceBus;

namespace Pingmint.Azure.ServiceBus;

/// <summary>
/// Extension methods for <see cref="ServiceBusSessionReceiver"/>.
/// </summary>
public static class ServiceBusSessionReceiverExtensions
{
    /// <summary>
    /// Completes a message and clears the session state in a new transaction.
    /// </summary>
    /// <param name="receiver">Service Bus session receiver</param>
    /// <param name="message">Message to complete</param>
    /// <param name="cancellationToken">Cancels the completion</param>
    /// <returns>Task that completes with the transaction</returns>
    public static async Task CompleteFinalMessageDetachedAsync(
        this ServiceBusSessionReceiver receiver,
        ServiceBusReceivedMessage message,
        CancellationToken cancellationToken = default)
    {
        using var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        await receiver.SetSessionStateAsync(null, cancellationToken);
        await receiver.CompleteMessageAsync(message, cancellationToken);
        transactionScope.Complete();
    }

    /// <summary>
    /// Completes a message and sets the session state in a new transaction.
    /// </summary>
    /// <param name="receiver">Service Bus session receiver</param>
    /// <param name="message">Message to complete</param>
    /// <param name="state">State to apply to the session</param>
    /// <param name="cancellationToken">Cancels the completion</param>
    /// <returns>Task that completes with the transaction</returns>
    public static async Task CompleteMessageDetachedAsync(
        this ServiceBusSessionReceiver receiver,
        ServiceBusReceivedMessage message,
        BinaryData? state,
        CancellationToken cancellationToken = default)
    {
        using var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        await receiver.SetSessionStateAsync(state, cancellationToken);
        await receiver.CompleteMessageAsync(message, cancellationToken);
        transactionScope.Complete();
    }

    /// <summary>
    /// Completes a message without updating the session state in a new transaction.
    /// </summary>
    /// <param name="receiver">Service Bus session receiver</param>
    /// <param name="message">Message to complete</param>
    /// <param name="cancellationToken">Cancels the completion</param>
    /// <returns>Task that completes with the transaction</returns>
    public static async Task CompleteMessageDetachedAsync(
        this ServiceBusSessionReceiver receiver,
        ServiceBusReceivedMessage message,
        CancellationToken cancellationToken = default)
    {
        using var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        await receiver.CompleteMessageAsync(message, cancellationToken);
        transactionScope.Complete();
    }

    /// <summary>
    /// Completes a message without updating the session state in a new transaction.
    /// </summary>
    /// <param name="receiver">Service Bus session receiver</param>
    /// <param name="state">Session state</param>
    /// <param name="cancellationToken">Cancels the completion</param>
    /// <returns>Task that completes with the transaction</returns>
    public static async Task SetSessionStateDetachedAsync(
        this ServiceBusSessionReceiver receiver,
        BinaryData? state,
        CancellationToken cancellationToken = default)
    {
        using var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        await receiver.SetSessionStateAsync(state, cancellationToken);
        transactionScope.Complete();
    }

    /// <summary>
    /// Renews the session lock at a specified interval until cancelled or disposed.
    /// </summary>
    /// <param name="receiver">Service Bus session receiver</param>
    /// <param name="renewInteval">Interval between renewal attempts</param>
    /// <param name="cancellationToken">Cancels the renewal task</param>
    /// <returns>Disposable which stops the session lock from renewing when disposed.</returns>
    public static IAsyncDisposable RenewSessionLockWithInterval(this ServiceBusSessionReceiver receiver, TimeSpan? renewInteval = null, CancellationToken cancellationToken = default) =>
        new SessionRenewingDisposable(receiver, renewInteval ?? TimeSpan.FromSeconds(30), cancellationToken);

    /// <summary>
    /// Renews the session lock at a specified interval until cancelled or disposed.
    /// </summary>
    public class SessionRenewingDisposable : IAsyncDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ServiceBusSessionReceiver receiver;
        private readonly Task task;

        internal SessionRenewingDisposable(ServiceBusSessionReceiver receiver, TimeSpan renewInteval, CancellationToken cancellationToken = default)
        {
            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this.receiver = receiver;

            this.task = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(renewInteval, cancellationTokenSource.Token).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                    if (cancellationTokenSource.IsCancellationRequested) { break; }

                    try
                    {
                        await receiver.RenewSessionLockAsync(cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch
                    {
                        // retry on next interval
                    }
                }
            }, cancellationTokenSource.Token);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            try
            {
                cancellationTokenSource.Cancel();
                await this.task;
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}
