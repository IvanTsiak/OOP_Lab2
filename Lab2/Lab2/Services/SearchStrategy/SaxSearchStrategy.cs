using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lab2.Services.SearchStrategy
{
    public class SaxSearchStrategy : IXmlSearchStrategy
    {
        public List<FacultyEntry> Search(string xmlFilePath, SearchCriteria criteria)
        {
            List<FacultyEntry> results = new List<FacultyEntry>();
            FacultyEntry currentEntry = null;
            Review currentReview = null;
            string currentElement = string .Empty;

            using (XmlReader reader = XmlReader.Create(xmlFilePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        currentElement = reader.Name;
                        if (currentElement == "Entry")
                        {
                            currentEntry = new FacultyEntry();
                            currentEntry.Id = reader.GetAttribute("id");
                            currentEntry.Type = reader.GetAttribute("type");
                            currentEntry.Department = reader.GetAttribute("department");
                        }
                        else if (currentElement == "Review" && currentEntry != null)
                        {
                            currentReview = new Review();
                            currentReview.User = reader.GetAttribute("reader");
                            currentReview.Score = reader.GetAttribute("score");
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Text && currentEntry != null)
                    {
                        switch (currentElement)
                        {
                            case "Title":
                                currentEntry.Title = reader.Value;
                                break;
                            case "Annotation":
                                currentEntry.Annotation = reader.Value;
                                break;
                            case "Name":
                                currentEntry.Authors.Add(reader.Value);
                                break;
                            case "Review":
                                if (currentReview != null)
                                {
                                    currentReview.Comment = reader.Value.Trim();
                                }
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Entry")
                    {
                        if (reader.Name == "Entry")
                        {
                            if (currentEntry != null && IsMatch(currentEntry, criteria))
                                results.Add(currentEntry);
                            currentEntry = null;
                        }
                        else if (reader.Name == "Review" && currentEntry != null && currentReview != null)
                        {
                            currentEntry.Reviews.Add(currentReview);
                            currentReview = null;
                        }
                    }
                }
            }
            return results;
        }

        private bool IsMatch(FacultyEntry entry, SearchCriteria criteria)
        {
            if (criteria.IsEmpty())
            {
                return true;
            }

            bool titleMatch = string.IsNullOrEmpty(criteria.Title) ||
                entry.Title?.Contains(criteria.Title) == true;

            bool departmentMatch = string.IsNullOrEmpty(criteria.Department) ||
                entry.Department?.Contains(criteria.Department) == true;

            bool authorMatch = string.IsNullOrEmpty(criteria.AuthorName) ||
                entry.Authors.Any(a => a.Contains(criteria.AuthorName));

            return titleMatch && departmentMatch && authorMatch;
        }
    }
}
