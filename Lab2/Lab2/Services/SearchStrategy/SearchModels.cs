using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Services.SearchStrategy
{
    public class Review
    {
        public string User { get; set; }
        public string Score { get; set; }
        public string Comment { get; set; }
    }
    public class SearchCriteria
    {
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Title) &&
                string.IsNullOrEmpty(AuthorName) &&
                string.IsNullOrEmpty(Department) &&
                string.IsNullOrEmpty(Category);
        }
    }

    public class FacultyEntry
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string Annotation { get; set; }
        public List<string> Authors { get; set; } = new List<string>();

        public List<Review> Reviews { get; set; } = new List<Review>();
        public string AuthorsDisplay => string.Join(", ", Authors);
        public override string ToString()
        {
            return $"[{Type}] {Title}";
        }
    }
}
