using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages
{
    public class IndexModel(ILogger<IndexModel> logger) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("./Poll/Index");
            }

            return Page();
        }
    }
}
