using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRClub
{
    public class CommandSendingEventHandler<TEvent, TCommand> : INotificationHandler<TEvent>
        where TEvent : INotification
        where TCommand : IRequest
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly SimpleLogger logger;

        public CommandSendingEventHandler(IMediator mediator, IMapper mapper, SimpleLogger logger)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task Handle(TEvent notification, CancellationToken cancellationToken)
        {
            logger.Log($"{typeof(TEvent)} --> {typeof(TCommand)}");

            var command = mapper.Map<TCommand>(notification);
            await mediator.Send(command);
        }
    }
}
