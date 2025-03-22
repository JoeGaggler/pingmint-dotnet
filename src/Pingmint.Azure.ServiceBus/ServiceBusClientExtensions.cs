using Azure.Messaging.ServiceBus;

namespace Pingmint.Azure.ServiceBus;

/// <summary>
/// Extension methods for <see cref="ServiceBusClient"/>.
/// </summary>
public static class ServiceBusClientExtensions
{
    /// <summary>
    /// Attempts to accept the next session from the specified queue.
    /// </summary>
    /// <param name="sbClient">Service Bus client</param>
    /// <param name="queueName">Queue name</param>
    /// <param name="sessionReceiverOptions">Session receiver options</param>
    /// <param name="cancellationToken">Cancels the operation</param>
    /// <returns>A receiver if a session message is available for processing.</returns>
    public static async Task<ServiceBusSessionReceiver?> TryAcceptNextSessionAsync(this ServiceBusClient sbClient, String queueName, ServiceBusSessionReceiverOptions sessionReceiverOptions, CancellationToken cancellationToken = default)
    {
        try
        {
            return await sbClient.AcceptNextSessionAsync(queueName, sessionReceiverOptions, cancellationToken);
        }
        catch (ServiceBusException exc)
        {
            // Azure Service Bus considers an empty queue to be "exceptional" => ServiceBusException(Reason=ServiceTimeout)
            if (exc.Reason == ServiceBusFailureReason.ServiceTimeout)
            {
                return null;
            }

            throw; // rethrow other exceptions
        }
    }

    /// <summary>
    /// Waits for a specific session from the specified queue, retrying if the session is busy.
    /// </summary>
    /// <param name="sbClient">Service Bus client</param>
    /// <param name="queueName">Queue name</param>
    /// <param name="sessionId">Session ID</param>
    /// <param name="sessionReceiverOptions">Session receiver options</param>
    /// <param name="cancellationToken">Cancels the operation</param>
    /// <returns>A receiver for the specified session</returns>
    public static async Task<ServiceBusSessionReceiver?> WaitForSessionAsync(this ServiceBusClient sbClient, String queueName, String sessionId, ServiceBusSessionReceiverOptions sessionReceiverOptions, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            try
            {
                return await sbClient.AcceptSessionAsync(
                    queueName,
                    sessionId,
                    sessionReceiverOptions,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (ServiceBusException exc) when (exc.Reason == ServiceBusFailureReason.SessionCannotBeLocked) // retry lock if session is busy
            {
                // Do not retry if the token will never be canceled
                if (!cancellationToken.CanBeCanceled) { return null; }

                // skip retry
                if (cancellationToken.IsCancellationRequested) { return null; }
            }
            catch
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Waits for the next session from the specified queue.
    /// </summary>
    /// <param name="sbClient">Service Bus client</param>
    /// <param name="queueName">Queue name</param>
    /// <param name="sessionReceiverOptions">Session receiver options</param>
    /// <param name="cancellationToken">Cancels the operation</param>
    /// <returns>A receiver for the next session in the queue</returns>
    public static async Task<ServiceBusSessionReceiver?> WaitForNextSessionAsync(this ServiceBusClient sbClient, String queueName, ServiceBusSessionReceiverOptions sessionReceiverOptions, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested) { return null; }

            try
            {
                return await sbClient.AcceptNextSessionAsync(queueName, sessionReceiverOptions, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (ServiceBusException exc) when (exc.Reason == ServiceBusFailureReason.ServiceTimeout)
            {
                // Do not retry if the token will never be canceled
                if (!cancellationToken.CanBeCanceled) { return null; }

                // skip retry
                if (cancellationToken.IsCancellationRequested) { return null; }

                // retry, because Azure Service Bus considers an empty queue to be "exceptional" => ServiceBusException(Reason=ServiceTimeout)
                continue;
            }
        }
    }
}
