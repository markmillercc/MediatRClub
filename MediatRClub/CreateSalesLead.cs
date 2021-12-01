using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRClub
{
    public class CreateSalesLead
    {
        public class Command : IRequest
        {
            public Details Details { get; set; }
        }

        public class Details
        {
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly SimpleLogger logger;
            public Handler(SimpleLogger logger)
            {
                this.logger = logger;
            }
            protected override Task Handle(Command request, CancellationToken cancellationToken)
            {
                logger.Log($"Handled {typeof(Command)}");

                return Task.CompletedTask;
            }
        }
    }
}
