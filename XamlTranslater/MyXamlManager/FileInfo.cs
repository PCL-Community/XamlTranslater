using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XamlTranslater.MyBase;

namespace XamlTranslater.MyXamlManager
{
    internal class FileInfo
    {
        public static List<FileInfo> Files = new List<FileInfo>();

        public string FileName { get; set; }
        public string Language { get; set; }
        public XDocument Document { get; set; }
        private List<XElement> element;
        
        public FileInfo(string fileName)
        {
            FileName= fileName;
            Language=IO.GetFileNameFromPath(fileName);
            Document = XDocument.Load(fileName);
            element= Document.Descendants().ToList();
        }
        public bool HasKey(string key)
        {
            return element.Where(e => e.Attributes().ToList()[0].Value.Equals(key)) is not null;
        }
        public void AddP(string Key,string Content)
        {
            if (!HasKey(Key))
            {
                //TODO: 增加字段
            }
        }
        public void DelP(string Key)
        {
            if (HasKey(Key))
            {
                //TODO: 删除字段
            }
        }
    }
}
