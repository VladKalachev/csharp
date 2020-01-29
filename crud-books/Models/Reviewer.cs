using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApiProject.Models
{
    public class Reviewer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters")]
        public string LastName { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual Book Book { get; set; }
    }
}