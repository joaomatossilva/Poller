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
public class Details(IMediator mediator) : PageModel
{
    public PollDetailsViewModel PollDetails { get; set; }

    public async Task<IActionResult> OnGet(Guid id) =>
        await mediator.Send(new Query { Id = id, UserId = User.GetUserId() })
            .MatchPageResult(x => PollDetails, this);

    public async Task<IActionResult> OnPostVoteOption(Guid id, Guid optionId) =>
        await mediator.Send(new Command { Id = id, OptionId = optionId, UserId = User.GetUserId() })
            .MatchRedirectToPage(nameof(Details));

    public class PollDetailsViewModel
    {
        public Poll Poll { get; init; }
        public bool HaveVoted { get; init; }
    }

    public class Query : IRequest<Result<PollDetailsViewModel>>
    {
        public string UserId { get; init; }
        public Guid Id { get; init; }
    }

    public class RouteValues
    {
        public Guid Id { get; init; }
    }

    public class Command : IRequest<Result<RouteValues>>
    {
        public Guid Id { get; init; }
        public Guid OptionId { get; init; }
        public string UserId { get; init; }
    }

    public class QueryHandler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<PollDetailsViewModel>>
    {
        public async Task<Result<PollDetailsViewModel>> Handle(Query request, CancellationToken cancellationToken) =>
            //Good candidate for projection
            await dbContext.Polls
                .WithSpecification(new PollSpecification(request.Id))
                .FirstOrFailResultAsync(cancellationToken)
                .MapAsync((poll, _) =>
                {
                    //Check if user can vote/view
                    var userId = request.UserId;
                    var haveVoted = poll.Options.Any(x => x.Votes.Any(x => x.UserId == userId));
                    return Task.FromResult(Result<PollDetailsViewModel>.Success(new PollDetailsViewModel
                    {
                        HaveVoted = haveVoted,
                        Poll = poll
                    }));
                }, cancellationToken);
    }

    public class CommandHandler(ApplicationDbContext dbContext) : IRequestHandler<Command, Result<RouteValues>>
    {
        public async Task<Result<RouteValues>> Handle(Command request, CancellationToken cancellationToken) =>
            await dbContext.PollOptions
                .WithSpecification(new PollOptionSpecification(request.Id, request.OptionId))
                .FirstOrFailResultAsync(cancellationToken)
                .MapAsync((option, _) =>
                {
                    var optionVote = new PollOptionVote
                    {
                        Id = Guid.NewGuid(),
                        PollOptionId = option.Id,
                        UserId = request.UserId,
                        VoteDateUtc = DateTime.UtcNow
                    };

                    dbContext.PollOptionVotes.Add(optionVote);
                    return Task.FromResult(Result<RouteValues>.Success(new RouteValues { Id = request.Id }));
                }, cancellationToken)
                .MapSaveChangesResultAsync(dbContext, cancellationToken);
    }

    public class PollSpecification : Specification<Poll>
    {
        public PollSpecification(Guid id)
        {
            Query
                .Include(x => x.Options)
                .ThenInclude(x => x.Votes)
                .AsSplitQuery()
                .Where(x => x.Id == id);
        }
    }

    public class PollOptionSpecification : Specification<PollOption>
    {
        public PollOptionSpecification(Guid pollId, Guid optionId)
        {
            Query
                .Where(x => x.Poll.Id == pollId && x.Id == optionId);
        }
    }
}