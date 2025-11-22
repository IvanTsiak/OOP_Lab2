using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lab2.Services.SearchStrategy
{
    public class DomSearchStrategy : IXmlSearchStrategy
    {
        public List<FacultyEntry> Search(string xmlFilePath, SearchCriteria criteria)
        {
            List<FacultyEntry> results = new List<FacultyEntry>();
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            XmlNodeList entryNodes = doc.SelectNodes("//Entry");

            foreach (XmlNode entryNode in entryNodes)
            {
                if (IsMatch(entryNode, criteria))
                {
                    var entry = new FacultyEntry
                    {
                        Id = entryNode.Attributes["id"]?.Value,
                        Type = entryNode.Attributes["type"]?.Value,
                        Department = entryNode.Attributes["department"]?.Value,
                        Title = entryNode.SelectSingleNode("Title")?.InnerText,
                        Annotation = entryNode.SelectSingleNode("Annotation")?.InnerText
                    };

                    XmlNodeList authorNodes = entryNode.SelectNodes("Author/Name");
                    foreach (XmlNode authorNode in authorNodes)
                    {
                        entry.Authors.Add(authorNode.InnerText);
                    }

                    XmlNodeList reviewNodes = entryNode.SelectNodes("Reviews/Review");
                    if (reviewNodes != null)
                    {
                        foreach (XmlNode rNode in reviewNodes)
                        {
                            entry.Reviews.Add(new Review
                            {
                                User = rNode.Attributes["reader"]?.Value,
                                Score = rNode.Attributes["score"]?.Value,
                                Comment = rNode.InnerText.Trim()
                            });
                        }
                    }

                    results.Add(entry);
                }
            }
            return results;
        }

        private bool IsMatch(XmlNode entryNode, SearchCriteria criteria)
        {
            if (criteria.IsEmpty())
            {
                return true;
            }

            bool titleMatch = string.IsNullOrEmpty(criteria.Title) ||
                entryNode.SelectSingleNode("Title")?.InnerText.Contains(criteria.Title) == true;

            bool departmentMatch = string.IsNullOrEmpty(criteria.Department) ||
                entryNode.Attributes["department"]?.Value.Contains(criteria.Department) == true;

            bool categoryMatch = string.IsNullOrEmpty(criteria.Category) ||
                entryNode.SelectSingleNode("Category")?.InnerText.Contains(criteria.Category) == true;

            bool authorMatch = string.IsNullOrEmpty(criteria.AuthorName);
            if (!authorMatch)
            {
                XmlNodeList authorNames = entryNode.SelectNodes("Author/Name");
                foreach (XmlNode authorName in authorNames)
                {
                    if (authorName.InnerText.Contains(criteria.AuthorName))
                    {
                        authorMatch = true;
                        break;
                    }
                }
            }

            return titleMatch && departmentMatch && categoryMatch && authorMatch;
        }
    }
}
