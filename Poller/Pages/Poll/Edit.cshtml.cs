using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Edit(ApplicationDbContext dbContext) : PageModel
{
    public Poll Poll { get; set; }

    [BindProperty]
    public string NewOptionText { get; set; }

    public async Task<IActionResult> OnGet(Guid id)
    {
        var userId = User.GetUserId();
        var poll = await dbContext.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (poll is null)
        {
            return NotFound();
        }

        Poll = poll;

        return Page();
    }

    public async Task<IActionResult> OnPostAddOption(Guid id)
    {
        var userId = User.GetUserId();
        var poll = await dbContext.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (poll is null)
        {
            return NotFound();
        }

        Poll = poll;

        var newOption = new PollOption
        {
            PollId = Poll.Id,
            Id = Guid.NewGuid(),
            Text = NewOptionText
        };
        Poll.Options.Add(newOption);
        dbContext.PollOptions.Add(newOption);

        await dbContext.SaveChangesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteOption(Guid id, Guid optionId)
    {
        var userId = User.GetUserId();
        var poll = await dbContext.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (poll is null)
        {
            return NotFound();
        }

        Poll = poll;

        var option = dbContext.PollOptions.FirstOrDefault(x => x.Id == optionId);
        if (option is null)
        {
            return Page();
        }

        Poll.Options.Remove(option);
        dbContext.PollOptions.Remove(option);

        await dbContext.SaveChangesAsync();
        return Page();
    }

}