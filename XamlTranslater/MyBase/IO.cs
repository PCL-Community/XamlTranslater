using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamlTranslater.MyBase
{
    internal class IO
    {
        public static string GetFileNameFromPath(string path)
        {
            string[] paths = path.Split('\\');
            return paths.Last();
        }
    }
}
