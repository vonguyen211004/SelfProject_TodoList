using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoList.Services;

namespace TodoList.ViewComponents
{
    public class TodoListViewComponent : ViewComponent
    {
        private readonly TodoService _todoService;

        public TodoListViewComponent(TodoService todoService)
        {
            _todoService = todoService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var todos = await _todoService.GetByUserIdAsync(userId);
            return View(todos);
        }
    }
}