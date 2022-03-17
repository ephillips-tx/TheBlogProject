using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlogProject.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string? BlogUserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)] // guidelines for the Name property. {} means variables of
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)] // guidelines for the Name property. {} means variables of
        public string Description { get; set; } = string.Empty;

        [DataType(DataType.Date)] //treats input as Date instead of Date & Time
        [Display(Name = "Created Date")] //helpful when displaying 
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; } //nullable DateTime

        [Display(Name = "Blog Image")]
        public byte[]? ImageData { get; set; } //image as a byte stream
        
        [Display(Name = "Image Type")]
        public string? ContentType { get; set; } // file extension so we can rebuild

        [NotMapped]
        public IFormFile? Image { get; set; } //represents the file the user selects
        
        //Navigation Property
        [Display(Name="Author")]
        public virtual BlogUser? BlogUser { get; set; } // Blog is a "child" of Author but a parent to a Post.
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>(); 

    }
}
