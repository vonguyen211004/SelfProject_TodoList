using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoList.Models;
using TodoList.Services;

namespace TodoList.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly TodoService _todoService;

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        // GET: Todo
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todos = await _todoService.GetByUserIdAsync(userId);
            return View(todos);
        }

        // GET: Todo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                todo.CreatedAt = DateTime.Now;
                todo.IsDone = false;
                todo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _todoService.CreateAsync(todo);
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todo/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todo = await _todoService.GetByIdAndUserIdAsync(id, userId);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        // POST: Todo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Description,IsDone,CreatedAt,DueDate,UserId")] Todo todo)
        {
            if (id != todo.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (todo.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                await _todoService.UpdateAsync(id, todo);
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todo/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todo = await _todoService.GetByIdAndUserIdAsync(id, userId);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todo = await _todoService.GetByIdAndUserIdAsync(id, userId);
            if (todo == null)
            {
                return NotFound();
            }

            await _todoService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Todo/ToggleStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _todoService.ToggleStatusAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}