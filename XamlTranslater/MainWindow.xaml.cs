using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Linq;

namespace XamlTranslater
{
    public class ContentStructure
    {
        public string Key { get; set; }
        public string OriginalContent { get; set; }
        public string TranslatedText { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string filePath = "";

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XAML 文件 (*.xaml)|*.xaml";
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                BtnBrowse.Visibility = Visibility.Collapsed;
                TextFilePath.Text = filePath;
                TextFilePath.Visibility = Visibility.Visible;
                var xaml = new FileStream(filePath, FileMode.Open);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Async = true;
                List<ContentStructure> contentStructures = new List<ContentStructure>();
                using (XmlReader reader = XmlReader.Create(xaml, settings))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "s:String")
                            {
                                string key = reader.GetAttribute("x:Key");
                                reader.Read();
                                string content = reader.ReadContentAsString();
                                contentStructures.Add(new ContentStructure { Key = key, OriginalContent = content });
                            }
                        }
                    }
                }
                ListViewTranslation.ItemsSource = contentStructures;
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var translater = new Translater.Ali(TextSetID.Text,TextSetkey.Text);
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("请先选择文件");
                return;
            }
            var newList = ListViewTranslation.Items.OfType<ContentStructure>().ToList();
            foreach (var item in newList)
            {
                //item.TranslatedText = "测试加入" + item.Key;
                //continue;
                item.TranslatedText = await translater.GetTranslation(item.OriginalContent,ComboLang.Text);
                if (item.TranslatedText == item.OriginalContent)
                {
                    if (MessageBox.Show("内容疑似未翻译成功，是否继续？", "翻译疑似有问题", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
            }
            ListViewTranslation.ItemsSource = newList;
            MessageBox.Show("所有生草已完成！");
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            if( MessageBox.Show("即将修改当前文件，是否继续？","确认",MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            XDocument xdoc = XDocument.Load(filePath);
            int ModifyNum = 0;
            foreach (var item in ListViewTranslation.Items.OfType<ContentStructure>().ToList())
            {
                try
                {
                    var elementToModify = xdoc.Descendants().ToList().Where(e => e.Attributes().ToList()[0].Value == item.Key).FirstOrDefault();
                    if (elementToModify != null)
                    {
                        elementToModify.Value = item.TranslatedText;
                        ModifyNum++;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(),"替换翻译字符失败",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
            }
            xdoc.Save(filePath);
            MessageBox.Show($"共修改 {ModifyNum} 项","完成！");
        }
    }
}