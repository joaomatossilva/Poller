namespace Poller.Data;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[PrimaryKey("Id")]
[Index("Id", "PollOptionId", "UserId", IsUnique = true)]
public class PollOptionVote
{
    public Guid Id { get; set; }

    public Guid PollOptionId { get; set; }

    [ForeignKey("PollOptionId")]
    public PollOption PollOption { get; set; }

    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public IdentityUser User { get; set; }

    public DateTime VoteDateUtc { get; set; }
}