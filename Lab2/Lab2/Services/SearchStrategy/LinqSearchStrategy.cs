using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab2.Services.SearchStrategy
{
    public class LinqSearchStrategy : IXmlSearchStrategy
    {
        public List<FacultyEntry> Search(string xmlFilePath, SearchCriteria criteria)
        {
            List<FacultyEntry> results = new List<FacultyEntry>();
            XDocument doc = XDocument.Load(xmlFilePath);

            var query = from entry in doc.Descendants("Entry")
                        where IsMatch(entry, criteria)
                        select new FacultyEntry
                        {
                            Id = entry.Attribute("id")?.Value,
                            Type = entry.Attribute("type")?.Value,
                            Department = entry.Attribute("department")?.Value,
                            Title = entry.Element("Title")?.Value,
                            Annotation = entry.Element("Annotation")?.Value,
                            Authors = entry.Elements("Author")
                                         .Select(a => a.Element("Name")?.Value)
                                         .ToList(),
                            Reviews = entry.Element("Reviews")?
                                           .Elements("Review")
                                           .Select(r => new Review
                                           {
                                               User = r.Attribute("reader")?.Value,
                                               Score = r.Attribute("score")?.Value,
                                               Comment = r.Value.Trim()
                                           })
                                           .ToList() ?? new List<Review>()
                        };

            return query.ToList();
        }

        private bool IsMatch(XElement entry, SearchCriteria criteria)
        {
            if (criteria.IsEmpty()) return true;

            bool titleMatch = string.IsNullOrEmpty(criteria.Title) ||
                entry.Element("Title")?.Value.Contains(criteria.Title) == true;

            bool departmentMatch = string.IsNullOrEmpty(criteria.Department) ||
                entry.Attribute("department")?.Value.Contains(criteria.Department) == true;

            bool categoryMatch = string.IsNullOrEmpty(criteria.Category) ||
                entry.Element("Category")?.Value.Contains(criteria.Category) == true;

            bool authorMatch = string.IsNullOrEmpty(criteria.AuthorName) ||
                entry.Elements("Author").Elements("Name")
                     .Any(name => name.Value.Contains(criteria.AuthorName));

            return titleMatch && departmentMatch && categoryMatch && authorMatch;
        }
    }
}
