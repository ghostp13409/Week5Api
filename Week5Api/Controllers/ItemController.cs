using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Week5Api.Data;
using Week5Api.Models;

namespace Week5Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetItems()
        {
            var items = _context.Items.ToList();
            return Ok(items);
        }

        [HttpPost]
        public IActionResult CreateItem([FromBody] Item item)
        {
            
            if( item.Name == null)
            {
                return BadRequest(new ErrorMessage
                {
                    error = "InvalidParameter",
                    message = "Item name cannot be empty."
                });
            }

            if ( item.Quantity < 0)
            {
                return BadRequest(new ErrorMessage
                {
                    error = "InvalidParameter",
                    message = "Quantity must be greater than zero"
                });
            }

            _context.Items.Add(item);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetItems), new { id = item.Id }, item);
        }
    }
}
