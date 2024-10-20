﻿using System;
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
            // XAML样板文件路径
            string exampleFilePath = "example.xaml";
            List<string> CorrectKeyOrder = new List<string>();
            // 输出文件的路径
            string outputFilePath = "sortedOutput.xaml";

            XDocument xamlExampleDoc = XDocument.Load(exampleFilePath);
            XElement xamlExampleRoot = xamlExampleDoc.Root;

            XDocument xamlDoc = XDocument.Load(inputFilePath);
            XElement xamlRoot = xamlDoc.Root;

            // 定义XAML的命名空间
            XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

            // 查找所有包含x:Key属性的元素
            var elementsExampleWithKeys = xamlExampleRoot.Descendants()
                .Where(e => e.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace == xNamespace && a.Name.LocalName == "Key"))
                .ToList();
            foreach (var item in elementsExampleWithKeys)
            {
                CorrectKeyOrder.Add(item.Attribute(xNamespace+"Key")?.Value);
            }

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