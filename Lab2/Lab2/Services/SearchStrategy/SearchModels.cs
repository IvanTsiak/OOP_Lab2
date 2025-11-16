using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Services.SearchStrategy
{
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

        public override string ToString()
        {
            return $"[{Type}] {Title} (Автори: {string.Join(", ", Authors)})";
        }
    }
}
