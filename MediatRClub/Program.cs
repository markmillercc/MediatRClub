using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MediatRClub
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
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
                })
                .Build()
                .Run();
        }
    }
}
