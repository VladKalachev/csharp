using System.Collections.Generic;

namespace BookApiProject.Models
{
    public class Reviewer
    {
        public int Id { get; set; }
        public string FirtName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual Book Book { get; set; }
    }
}