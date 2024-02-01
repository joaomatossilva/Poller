namespace Poller.Data;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[Index("Code", IsUnique = true)]
[PrimaryKey("Id")]
public class Poll
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Title { get; set; }

    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; }

    public ICollection<PollOption> Options { get; set; }

    public DateTime CreatedUtc { get; set; }
}