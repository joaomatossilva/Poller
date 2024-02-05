using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Index(IMediator mediator) : PageModel
{
    public IEnumerable<Poll> MyPolls { get; set; }

    public async Task<IActionResult> OnGet() =>
        await mediator.Send(new Query { UserId = User.GetUserId() })
            .MatchPageResult(x => MyPolls, this);

    public class Query : IRequest<Result<IEnumerable<Poll>>>
    {
        public string UserId { get; init; }
    }

    public class QueryHandler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<IEnumerable<Poll>>>
    {
        public async Task<Result<IEnumerable<Poll>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var polls = await dbContext.Polls
                .Where(x => x.UserId == request.UserId)
                .OrderByDescending(x => x.CreatedUtc)
                .ToListAsync(cancellationToken);
            return Result<IEnumerable<Poll>>.Success(polls);
        }
    }
}