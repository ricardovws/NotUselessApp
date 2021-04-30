using MassTransit;
using Microsoft.Extensions.Logging;
using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo
{
    public class TicketConsumer : IConsumer<Ticket>
    {
        private readonly ILogger<TicketConsumer> logger;
        public TicketConsumer(ILogger<TicketConsumer> logger)
        {
            this.logger = logger;
        }
        
        public async Task Consume(ConsumeContext<Ticket> context)
        {
            await Console.Out.WriteLineAsync(context.Message.UserName);

            logger.LogInformation($"Nova mensagem recebida: " + $" {context.Message.UserName} {context.Message.Location}");
        }
    }
}
