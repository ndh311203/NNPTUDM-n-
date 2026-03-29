using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using spatc.Models;
using spatc.Services;
using System.Security.Claims;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class AdminChatController : Controller
    {
        private readonly ChatService _chatService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminChatController> _logger;

        public AdminChatController(
            ChatService chatService,
            ApplicationDbContext context,
            ILogger<AdminChatController> logger)
        {
            _chatService = chatService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View("~/Views/Admin/Chat/Index.cshtml");
        }
    }
}

