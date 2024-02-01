namespace Poller.Data;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey("Id")]
public class PollOption
{
    public Guid Id { get; set; }

    public Guid PollId { get; set; }

    [ForeignKey("PollId")]
    public virtual Poll Poll { get; set; }

    public string Text { get; set; }

    public ICollection<PollOptionVote> Votes { get; set; }
}