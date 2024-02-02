using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sqids;

[Authorize]
public class Create(ApplicationDbContext dbContext, SqidsEncoder<int> sqidsEncoder) : PageModel
{
    [BindProperty]
    public string Title { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPost()
    {
        var userId = User.GetUserId();
        var id = Guid.NewGuid();
        var poll = new Poll
        {
            Id = id,
            //This is collision prone but since this is only academic, it's ok
            Code = sqidsEncoder.Encode(Math.Abs(id.GetHashCode())),
            CreatedUtc = DateTime.UtcNow,
            Title = Title,
            UserId = userId
        };

        dbContext.Polls.Add(poll);
        await dbContext.SaveChangesAsync();

        return RedirectToPage("Edit", new { Id = poll.Id });
    }
}