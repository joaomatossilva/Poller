using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class Create(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public string Title { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPost()
    {
        var userId = User.GetUserId();
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Code = "12",
            CreatedUtc = DateTime.UtcNow,
            Title = Title,
            UserId = userId
        };

        dbContext.Polls.Add(poll);
        await dbContext.SaveChangesAsync();

        return RedirectToPage("Edit", new { Id = poll.Id });
    }
}