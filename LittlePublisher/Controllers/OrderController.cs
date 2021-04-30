using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModel;

namespace LittlePublisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IBus _bus;

        public OrderController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            if(ticket != null)
            {
                ticket.Booked = DateTime.Now;
                Uri uri = new Uri("rabbitmq://localhost/orderTicketQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(ticket);
                return Ok();
            }
            return BadRequest();
        }
    }
}
