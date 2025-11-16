using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab2.Services.SearchStrategy
{
    public interface IXmlSearchStrategy
    {
        List<FacultyEntry> Search(string xmlFilePath, SearchCriteria criteria);
    }
}
