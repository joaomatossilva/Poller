using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Details(ApplicationDbContext dbContext) : PageModel
{
    public Poll Poll { get; set; }
    public bool hasVoted { get; set; }

    public async Task<IActionResult> OnGet(Guid id)
    {
        //Good candidate for projection
        var poll = await dbContext.Polls
            .Include(x => x.Options)
            .ThenInclude(x => x.Votes)
            //.ThenInclude(x => x.)
            .AsSplitQuery()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (poll is null)
        {
            return NotFound();
        }

        //Check if user can vote/view

        var userId = User.GetUserId();
        Poll = poll;
        hasVoted = poll.Options.Any(x => x.Votes.Any(x => x.UserId == userId));
        return Page();
    }

    public async Task<IActionResult> OnPostVoteOption(Guid id, Guid optionId)
    {
        var poll = await dbContext.Polls
            .Include(x => x.Options)
            .ThenInclude(x => x.Votes)
            //.ThenInclude(x => x.)
            .AsSplitQuery()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (poll is null)
        {
            return NotFound();
        }

        var option = poll.Options
            .FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            return NotFound();
        }

        var userId = User.GetUserId();
        var optionVote = new PollOptionVote
        {
            Id = Guid.NewGuid(),
            PollOptionId = option.Id,
            UserId = userId,
            VoteDateUtc = DateTime.UtcNow
        };

        dbContext.PollOptionVotes.Add(optionVote);
        await dbContext.SaveChangesAsync();

        return RedirectToPage(new { id });
    }
}