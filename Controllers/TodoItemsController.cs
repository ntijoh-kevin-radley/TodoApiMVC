using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
        {

            //Get TODOs with Comments (include = join) from DB
            var todoItems = await _context.TodoItems
                .Include(todoItem => todoItem.Comments)
                .ToListAsync();

            //Map the TodoItems in DTOs
            var todoItemsDto = todoItems
                .Select(todoItem => new TodoItemDto(todoItem))
                .ToList();

            //Wraps DTO in ObjectResult
            return Ok(todoItemsDto);

        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        // Ensure all comments have the correct TodoItemId
        {
            foreach (var comment in todoItem.Comments)
            {
                comment.TodoItemId = todoItem.Id; // Link comments to the new TodoItem
            }

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }


        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        [HttpGet("{todoItemId}/comments")]
        [Tags("Comments")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetComments(int todoItemId)
        {
            var todoItem = await _context.TodoItems
                                         .Include(t => t.Comments)
                                         .FirstOrDefaultAsync(t => t.Id == todoItemId);
            if (todoItem == null)
                return NotFound();

            return Ok(todoItem.Comments);
        }

        [HttpPost("{todoItemId}/comments")]
        [Tags("Comments")]
        public async Task<ActionResult<Comment>> PostComments(int todoItemId, [FromBody] Comment comment)
        {
            var todoItem = await _context.TodoItems
                                        .Include(t => t.Comments)
                                        .FirstOrDefaultAsync(t => t.Id == todoItemId);
            if (todoItem == null)
                return NotFound();

            comment.TodoItemId = todoItem.Id; // Ensure the foreign key is set
            _context.Comments.Add(comment);  // Add the comment to the DbSet
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComments", new { todoItemId = todoItem.Id }, comment);
        }

        [HttpGet("{todoItemId}/comments/{id}")]
        [Tags("Comments")]
        public async Task<ActionResult<Comment>> GetComment(int todoItemId, int id)
        {
            var todoItem = await _context.TodoItems
                                        .Include(t => t.Comments)
                                        .FirstOrDefaultAsync(t => t.Id == todoItemId);
            if (todoItem == null)
                return NotFound();

            var comment = todoItem.Comments.Find(c => c.Id == id);

            if (comment == null)
                return NotFound();

            return Ok(comment);
        }

        [HttpPut("{todoItemId}/comments/{commentId}")]
        [Tags("Comments")]
        public async Task<IActionResult> PutComment(long commentId, Comment incomingComment)
        {
            if (commentId != incomingComment.Id)
            {
                Console.WriteLine($"Incoming request at ID #{commentId} and #{incomingComment.Id}");
                return BadRequest();
            }

            var currentComment = await _context.Comments
                                         .FirstOrDefaultAsync(c => c.Id == commentId);

            if (currentComment == null)
                return NotFound();

            currentComment.Text = incomingComment.Text;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{todoItemId}/comments/{id}")]
        [Tags("Comments")]
        public async Task<ActionResult<Comment>> DeleteComment(int todoItemId, int id)
        {
            var todoItem = await _context.TodoItems
                                        .Include(t => t.Comments)
                                        .FirstOrDefaultAsync(t => t.Id == todoItemId);
            if (todoItem == null)
                return NotFound();

            var comment = todoItem.Comments.Find(c => c.Id == id);

            if (comment == null)
                return NotFound();

            todoItem.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}