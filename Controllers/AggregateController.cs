using Microsoft.AspNetCore.Mvc;
using WarikakeWeb.Data;

namespace WarikakeWeb.Controllers
{
    public class AggregateController : Controller
    {
        private readonly WarikakeWebContext _context;

        public AggregateController(WarikakeWebContext context)
        {
            _context = context; 
        }
            
        

        public IActionResult Index()
        {
            return View();
        }
    }
}
