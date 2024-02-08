using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Data;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class Index(IMediator mediator) : PageModel
{
    public List<Poll> MyPolls { get; set; }

    public async Task<IActionResult> OnGet() =>
        await mediator.Send(new Query { UserId = User.GetUserId() })
            .MatchPageResult(x => MyPolls, this);

    public class Query : IRequest<Result<List<Poll>>>
    {
        public string UserId { get; init; }
    }

    public class QueryHandler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<List<Poll>>>
    {
        public async Task<Result<List<Poll>>> Handle(Query request, CancellationToken cancellationToken) =>
            await dbContext.Polls
                .WithSpecification(new MyPollsSpecification(request.UserId))
                .ToListResultAsync(cancellationToken);
    }

    public class MyPollsSpecification : Specification<Poll>
    {
        public MyPollsSpecification(string userId)
        {
            Query
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedUtc);
        }
    }
}