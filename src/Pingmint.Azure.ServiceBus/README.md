# Pingmint.Azure.ServiceBus

A dotnet library with for working with Azure Service Bus.

# Preamble

The extensions to `Azure.Messaging.ServiceBus` implement patterns that I find myself frequently using.
Copy the useful files or methods as needed, or import the NuGet package for convenience.

# Patterns

## Transactions

The `Azure.Messaging.ServiceBus` library uses the ambient `System.Transactions.TransactionScope` to provide transactional support.
Since application code typically accesses multiple resources that *cannot* be enlisted in the same transaction, isolating Service Bus operations becomes unnecessarily complicated.
This library provides several extension methods that perform operations in a new "detached" transaction scope, for example:

```csharp
// Before
using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
{
    await receiver.SetSessionStateAsync(state, cancellationToken);
    await receiver.CompleteMessageAsync(message, cancellationToken);
    transactionScope.Complete();
}

// After
await receiver.CompleteMessageDetachedAsync(message, state, cancellationToken);
```

## Exception Handling

The `ServiceBusClient.AcceptNextSessionAsync` method throws `ServiceBusException (Reason=ServiceTimeout)`  when there are no more sessions to accept.
For code that does not wish to consider this exceptional (handling all messages is a good thing!), this library adds a `TryAcceptNextSessionAsync` method that returns a `null` `ServiceBusSessionReceiver` when there are no more sessions to accept.

Similarly, `WaitForSessionAsync` and `WaitForNextSessionAsync` try to accept a session until cancelled, returning `null` instead of throwing an exception.

## Session Lock Renewal

Some awaited tasks in a session message handler have an unknown duration, and may take longer than the session lock duration. This library provides a `RenewSessionLockWithInterval` method periodically renews session lock until its return value is disposed.

```csharp
await using (receiver.RenewSessionLockWithInterval(TimeSpan.FromSeconds(30), cancellationToken))
{
    // do async work here
}
```
