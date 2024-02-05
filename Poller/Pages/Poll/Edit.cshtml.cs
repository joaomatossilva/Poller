using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Poller.Pages.Poll;

using Data;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Edit(IMediator mediator) : PageModel
{
    public Poll Poll { get; set; }

    [BindProperty]
    public string NewOptionText { get; set; }

    public async Task<IActionResult> OnGet(Guid id) =>
        await mediator.Send(new Query { Id = id, UserId = User.GetUserId() })
            .MatchPageResult(x => Poll, this);

    public async Task<IActionResult> OnPostAddOption(Guid id) =>
        await mediator.Send(new AddOptionCommand { Id = id, UserId = User.GetUserId(), NewOptionText = NewOptionText })
            .MatchPageResult(x => Poll, this);

    public async Task<IActionResult> OnPostDeleteOption(Guid id, Guid optionId) =>
        await mediator.Send(new DeleteOptionCommand { Id = id, UserId = User.GetUserId(), OptionId = optionId })
            .MatchPageResult(x => Poll, this);

    public class Query : IRequest<Result<Poll>>
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
    }

    public class AddOptionCommand: IRequest<Result<Poll>>
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public string NewOptionText { get; init; }
    }

    public class DeleteOptionCommand : IRequest<Result<Poll>>
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public Guid OptionId { get; init; }
    }

    public class QueryHandler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<Poll>>
    {
        public async Task<Result<Poll>> Handle(Query request, CancellationToken cancellationToken) =>
            await dbContext.Polls
                .Include(p => p.Options)
                .FirstOrFailResultAsync(x => x.Id == request.Id && x.UserId == request.UserId, cancellationToken);
    }

    public class AddOptionCommandHandler(ApplicationDbContext dbContext) : IRequestHandler<AddOptionCommand, Result<Poll>>
    {
        public async Task<Result<Poll>> Handle(AddOptionCommand request, CancellationToken cancellationToken) =>
            await dbContext.Polls
                .Include(p => p.Options)
                .FirstOrFailResultAsync(x => x.Id == request.Id && x.UserId == request.UserId, cancellationToken)
                .MapAsync(async (poll, ct) =>
                {
                    var newOption = new PollOption
                    {
                        PollId = poll.Id,
                        Id = Guid.NewGuid(),
                        Text = request.NewOptionText
                    };
                    poll.Options.Add(newOption);
                    dbContext.PollOptions.Add(newOption);

                    await dbContext.SaveChangesAsync(ct);
                    return Result<Poll>.Success(poll);
                }, cancellationToken);
    }

    public class DeleteOptionCommandHandler(ApplicationDbContext dbContext) : IRequestHandler<DeleteOptionCommand, Result<Poll>>
    {
        public async Task<Result<Poll>> Handle(DeleteOptionCommand request, CancellationToken cancellationToken) =>
            await dbContext.Polls
                .Include(p => p.Options)
                .FirstOrFailResultAsync(x => x.Id == request.Id && x.UserId == request.UserId, cancellationToken)
                .MapAsync(async (poll, ct) => await dbContext.PollOptions.FirstOrFailResultAsync(x => x.Id == request.OptionId, ct)
                    .MapAsync(async (opt, ct2) =>
                    {
                        poll.Options.Remove(opt);
                        dbContext.PollOptions.Remove(opt);
                        await dbContext.SaveChangesAsync(ct2);

                        return Result<Poll>.Success(poll);
                    }, ct), cancellationToken);
    }
}