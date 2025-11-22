using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace Lab2.Services
{
    public class TransformationService
    {
        private readonly string _xslResourceName = "Lab2.style.xsl";

        public void TransformToStream(string xmlFilePath, Stream outputStream)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(_xslResourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Не вдалося знайти вбудований ресурс XSL. Перевірте назву: '{_xslResourceName}', хоча, якщо чесно, я не знаю, як для дурня допоможе перевірка назви.");
                }

                using (XmlReader reader = XmlReader.Create(stream))
                {
                    xslt.Load(reader);
                }
            }

            xslt.Transform(xmlFilePath, null, outputStream);
        }
    }
}
