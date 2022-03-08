using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheBlogProject.Enums;

namespace TheBlogProject.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int BlogId { get; set; } // foreign key but primary key of Blog class
        public string AuthorId { get; set; } = String.Empty; // foreign key for writer of Post

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.", MinimumLength = 2)] //{0} is the name of property
        public string Title { get; set; } = String.Empty;

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.", MinimumLength = 2)]
        public string Abstract { get; set; } = String.Empty;

        [Required]
        public string Content { get; set; } = String.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        //public bool IsReady { get; set; } = false; | Insufficient to describe the state of the post. Introducing enums folder
        public ReadyStatus ReadyStatus { get; set; }

        public string? Slug { get; set; }

        public byte[]? ImageData { get; set; }

        public string ContentType { get; set; } = String.Empty;

        [NotMapped]
        public IFormFile? Image { get; set; }

        //Navigation Property: reference foreign keys
        public virtual Blog? Blog { get; set; }
        public virtual IdentityUser? Author { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>(); //Tags are held in DB table. HashSet is a concrete class that implements the interface of ICollection.
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>(); //A post could have a collection of comments

    }
}
