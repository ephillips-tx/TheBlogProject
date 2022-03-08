using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TheBlogProject.Enums;

namespace TheBlogProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; } = String.Empty;
        public string ModeratorId { get; set; } = String.Empty;

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1}.", MinimumLength = 2)]
        [Display(Name = "Comment")]
        public string Body { get; set; } = String.Empty;

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Moderated { get; set; }
        public DateTime? Deleted { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1}.", MinimumLength = 2)]
        [Display(Name = "Moderated Comment")]
        public string ModeratedBody { get; set; } = String.Empty;

        public ModerationType ModerationType { get; set; } //Limit # of reasons a moderator can use

        //Navigation properties
        public virtual Post? Post { get; set; }
        public virtual IdentityUser? Author { get; set; }
        public virtual IdentityUser? Moderator { get; set; }
        //No ICollection because all of these are referencing "parent." Comment is only a "child" of other models. 
    }
}
