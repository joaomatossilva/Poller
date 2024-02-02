using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using Microsoft.EntityFrameworkCore;

public class Index(ApplicationDbContext dbContext) : PageModel
{
    public IEnumerable<Poll> MyPolls { get; set; }

    public async Task OnGet()
    {
        var userId = User.GetUserId();
        MyPolls = await dbContext.Polls
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedUtc)
            .ToListAsync();
    }
}