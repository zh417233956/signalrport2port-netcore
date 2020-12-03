using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Web.Hubs;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IHubContext<P2PHub> _hubContext;
        public UserContext db => Startup.db;

        public MessageController(ILogger<MessageController> logger, IHubContext<P2PHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        // GET: api/Message
        [HttpGet]
        public List<User> Get()
        {
            return db.Users.Take(50).ToList();
        }

        // GET: api/Message/5
        [HttpGet("{id}", Name = "Get")]
        public User Get(int id)
        {
            var user = db.Users.SingleOrDefault(u => u.UserID == id);
            return user;
        }

        // GET: api/Message/Send
        [HttpGet("Send")]
        public async Task<string> Send(int id, string msg)
        {
            var user = db.Users.SingleOrDefault(u => u.UserID == id);
            foreach (var item in user.Connections)
            {
                await _hubContext.Clients.Client(item.ConnectionID).SendAsync("ReceiveMessage", "server", $"{ msg }");
            }

            return "ok";
        }
    }
}
