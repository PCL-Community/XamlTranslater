using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;

namespace XamlSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            // XAML文件的路径
            List<string> inputFilePaths = new List<string>();
            inputFilePaths = Directory.EnumerateFiles("Lang", "*.xaml").ToList();
            // XAML样板文件路径
            string exampleFilePath = "zh-CN.xaml";
            List<string> CorrectKeyOrder = new List<string>();

            XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

            XDocument xamlExampleDoc = XDocument.Load(exampleFilePath);
            XElement xamlExampleRoot = xamlExampleDoc.Root;

            // 获取标准序列
            var elementsExampleWithKeys = xamlExampleRoot.Descendants()
                .Where(e => e.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace == xNamespace && a.Name.LocalName == "Key"))
                .ToList();
            foreach (var item in elementsExampleWithKeys)
            {
                CorrectKeyOrder.Add(item.Attribute(xNamespace+"Key")?.Value);
            }

            foreach (var inputFilePath in inputFilePaths)
            {
                XDocument xamlDoc = XDocument.Load(inputFilePath);
                XElement xamlRoot = xamlDoc.Root;

                // 定义XAML的命名空间


                // 查找所有包含x:Key属性的元素
                var elementsWithKeys = xamlRoot.Descendants()
                    .Where(e => e.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace == xNamespace && a.Name.LocalName == "Key"))
                    .ToList();

                List<XElement> sortedElements = new List<XElement>();
                // 根据x:Key属性对元素进行排序
                foreach (var item in CorrectKeyOrder)
                {
                    var target = elementsWithKeys.Where(e => e.Attribute(xNamespace + "Key").Value.Equals(item));
                    if (target is null || !target.Any()) //目标文件中没有此项
                    {
                        sortedElements.Add(elementsExampleWithKeys.Where(e => e.Attribute(xNamespace + "Key").Value.Equals(item)).First());
                    }
                    else
                    {
                        sortedElements.Add(target.First());
                    }
                }

                // 创建一个新的XElement，用在构建排序后的XAML
                XElement sortedXamlRoot = new XElement(xamlRoot.Name,
                    xamlRoot.Attributes(),
                    sortedElements
                );

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.IndentChars = "    ";  // Indent 4 Spaces

                XDocument sortedXamlDoc = new XDocument(sortedXamlRoot);
                using (XmlWriter writer = XmlTextWriter.Create(inputFilePath, settings))
                {
                    sortedXamlDoc.Save(writer);
                }
                Console.WriteLine($"已对 {inputFilePath} 进行排序");
            }
        }
    }
}