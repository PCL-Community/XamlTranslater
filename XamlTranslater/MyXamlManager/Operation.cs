using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamlTranslater.MyBase;

namespace XamlTranslater.MyXamlManager
{
    /// <summary>
    /// 批量操作
    /// </summary>
    internal class Operation
    {
        public static void Clear()
        {
            FileInfo.Files.Clear();
        }
        public static void Add(string DocFile)
        {
            FileInfo.Files.Add(new FileInfo(DocFile) );
        }
    }
}
