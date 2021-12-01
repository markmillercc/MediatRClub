using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MediatRClub.Tests
{
    public class MembershipExpiredTests
    {
        private readonly IServiceProvider provider;

        public MembershipExpiredTests()
        {
            provider = BuildServiceProvider();
        }

        [Theory]
        [InlineData(typeof(CommandSendingEventHandler<MembershipExpired, FreezeMembership.Command>))]
        [InlineData(typeof(CommandSendingEventHandler<MembershipExpired, NotifyMember.Command>))]
        [InlineData(typeof(CommandSendingEventHandler<MembershipExpired, CreateSalesLead.Command>))]
        public void Should_have_handler_registered_for_membership_expired_event(Type expectedHandler)
        {
            var handlers = provider.GetServices<INotificationHandler<MembershipExpired>>();

            handlers.Count(h => h.GetType() == expectedHandler).ShouldBe(1);
        }

        [Theory]
        [InlineData(typeof(FreezeMembership.Command))]
        [InlineData(typeof(NotifyMember.Command))]
        [InlineData(typeof(CreateSalesLead.Command))]
        public async Task Should_send_command_on_membership_expired_event(Type expectedCommand)
        {
            var logger = provider.GetRequiredService<SimpleLogger>();
            var mediator = provider.GetRequiredService<IMediator>();

            await mediator.Publish(new MembershipExpired());

            logger.Entries
                .Count(entry => entry == $"{typeof(MembershipExpired)} --> {expectedCommand}")
                .ShouldBe(1);

            logger.Entries
                .Count(entry => entry == $"Handled {expectedCommand}")
                .ShouldBe(1);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<SimpleLogger>();

            services.AddMediatR(typeof(Member));

            services.AddTransient<
                INotificationHandler<MembershipExpired>,
                CommandSendingEventHandler<MembershipExpired, FreezeMembership.Command>>();

            services.AddTransient<
                INotificationHandler<MembershipExpired>,
                CommandSendingEventHandler<MembershipExpired, NotifyMember.Command>>();

            services.AddTransient<
                INotificationHandler<MembershipExpired>,
                CommandSendingEventHandler<MembershipExpired, CreateSalesLead.Command>>();

            services.AddAutoMapper(config =>
            {
                config.CreateMap<MembershipExpired, FreezeMembership.Command>();

                config.CreateMap<MembershipExpired, NotifyMember.Command>()
                    .ForMember(dest => dest.Message, opt => opt.MapFrom(src => new NotifyMember.Message { }));

                config.CreateMap<MembershipExpired, CreateSalesLead.Command>()
                    .ForMember(dest => dest.Details, opt => opt.MapFrom(src => new CreateSalesLead.Details { }));

            }, typeof(Member));

            return services.BuildServiceProvider();
        }
    }
}
