## MediatRClub

This project is intended to demostrate the use of mismatched-arity open generic notification handlers with MediatR.

This means something like this:

```c#
public class CommandSendingNotificationHandler<TNotification, TOperation> : INotificationHandler<TNotification>
  where TNotification : INotification
  where TOperation : IRequest
{
}
```

Where `TNotification` is the notification/event, and `TOperation` is the operation to perform when handling this event - typically an `IRequest` object.

This scenario is useful for quickly mapping various commands or operations to various events without the need to create distinct handlers, and maintaining a decoupled relationship between the operation and the event. Since an operation that is triggered by an event may be used in other scenarios unrelated to the triggering of that event, this pattern allows the operation to remain distinct and decoupled from the narrow use case of event handling. In other words, the same operation may be performed for various reasons, eg being called directly or being triggered by a dispatched event, with very little code in between.

In this demo project, we have a single event, `MembershipExpired`, that when dispatched requires several distinct operations to be performed. When a membership expires, we need to freeze that members account, notify the member, and queue up a new lead for the sales team to follow up on. These operations are distinct from one another and are not dependent on the `MembershipExpired` event, as these operations may need to be performed elsewhere for a variety of other reasons.

To facilitate this, we need only the `INotification` object, the `IRequest` that it triggers, and the generic handler. The set up looks something like this:

```c#
services.AddTransient<
  INotificationHandler<MembershipExpired>,
  CommandSendingEventHandler<MembershipExpired, FreezeMembership.Command>>();

services.AddTransient<
  INotificationHandler<MembershipExpired>,
  CommandSendingEventHandler<MembershipExpired, NotifyMember.Command>>();

services.AddTransient<
  INotificationHandler<MembershipExpired>,
  CommandSendingEventHandler<MembershipExpired, CreateSalesLead.Command>>();
```

Using AutoMapper, we can smoothly map events to commands and facilitate these side effects without the need to create distinct handlers. 

This pattern has worked well, until this recent change to the dotnet runtime: https://github.com/dotnet/runtime/pull/51167.

Previously, registering an open generic with a mismatched arity between service and implementation was allowed - even though the service is not valid and would throw an exception upon instantiation. The new change brings more transparency and catches these errors on startup, rather than later on during application use - ultimately, a good change in my opinion, since it is clear why something like this won't work: 

`services.AddTransient(typeof(INotificationHandler<>), typeof(MyHandler<,>))`. 

That is not a valid service registration. For this reason, MediatR requires that the concrete implementation be registered manually in order for it to work its magic: 

`services.AddTransient(typeof(INotificationHandler<Event>), typeof(MyHandler<Event, Operation>))`.

However, MediatR is still registering these invalid services automatically, even though they will never actually be instantiated. Now that the dotnet runtime catches this error on registration, the generic notification handler pattern above fails. 

To reproduce this error, simply upgrade the Microsoft.Extensions packages to their latest versions, 6.0.0.
