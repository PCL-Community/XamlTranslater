using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XamlSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            // XAML文件的路径
            string inputFilePath = "input.xaml";
            // 输出文件的路径
            string outputFilePath = "sortedOutput.xaml";

            // 加载XAML文件
            XDocument xamlDoc = XDocument.Load(inputFilePath);

            // 获取XAML的根元素
            XElement xamlRoot = xamlDoc.Root;

            // 定义XAML的命名空间
            XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

            // 查找所有包含x:Key属性的元素
            var elementsWithKeys = xamlRoot.Descendants()
                .Where(e => e.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace == xNamespace && a.Name.LocalName == "Key"))
                .ToList();

            // 根据x:Key属性对元素进行排序
            var sortedElements = elementsWithKeys.OrderBy(e => e.Attribute(xNamespace + "Key")?.Value);

            // 创建一个新的XElement，用于构建排序后的XAML
            XElement sortedXamlRoot = new XElement(xamlRoot.Name,
                xamlRoot.Attributes(),
                sortedElements
            );

            // 保存排序后的XAML到新文件
            XDocument sortedXamlDoc = new XDocument(sortedXamlRoot);
            sortedXamlDoc.Save(outputFilePath);

            Console.WriteLine($"Sorted XAML has been saved to {outputFilePath}");
        }
    }
}