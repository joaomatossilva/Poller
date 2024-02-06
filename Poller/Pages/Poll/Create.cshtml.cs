using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sqids;

[Authorize]
public class Create(IMediator mediator) : PageModel
{
    [BindProperty]
    public string Title { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost() =>
        await mediator.Send(new Command { Title = Title, UserId = User.GetUserId() })
            .MatchRedirectToPage(nameof(Edit));

    public class RouteValues
    {
        public Guid Id { get; set; }
    }

    public class Command : IRequest<Result<RouteValues>>
    {
        public string UserId { get; init; }
        public string Title { get; init; }
    }

    public class CommandHandler(ApplicationDbContext dbContext, SqidsEncoder<int> sqidsEncoder) : IRequestHandler<Command, Result<RouteValues>>
    {
        public async Task<Result<RouteValues>> Handle(Command request, CancellationToken cancellationToken) =>
            await Task.FromResult(Result<Unit>.Success(Unit.Value))
                .MapAsync((_, _) =>
                {
                    var id = Guid.NewGuid();
                    var poll = new Poll
                    {
                        Id = id,
                        //This is collision prone but since this is only academic, it's ok
                        Code = sqidsEncoder.Encode(Math.Abs(id.GetHashCode())),
                        CreatedUtc = DateTime.UtcNow,
                        Title = request.Title,
                        UserId = request.UserId
                    };

                    dbContext.Polls.Add(poll);
                    return Task.FromResult(Result<Poll>.Success(poll));

                }, cancellationToken)
                .MapSaveChangesResultAsync(dbContext, cancellationToken)
                .MapAsync<RouteValues, Poll>((poll, _) => Task.FromResult(Result<RouteValues>.Success(new RouteValues { Id = poll.Id})), cancellationToken);

    }
}