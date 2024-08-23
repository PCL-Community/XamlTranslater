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

        private async Task<string> TranslateContent(string content, string targetLanguage)
        {
            using (var httpClient = new HttpClient())
            {
                var apiKey = TextSetkey.Text;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", apiKey);

                var requestBody = new Dictionary<string, string>
                {
                    { "text", content },
                    { "target_lang", targetLanguage }
                };

                var response = await httpClient.PostAsync("https://api-free.deepl.com/v2/translate", new FormUrlEncodedContent(requestBody));

                if (response.StatusCode !=HttpStatusCode.OK)
                {
                    MessageBox.Show($"获取翻译失败：{response.ToString()}");
                    response.EnsureSuccessStatusCode();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var translationResult = JsonConvert.DeserializeObject<DeepLResponse>(jsonResponse);
                return translationResult.Translations[0].Text;
            }
        }

        public class DeepLResponse
        {
            public List<Translation> Translations { get; set; }
        }

        public class Translation
        {
            public string Text { get; set; }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("请先选择文件");
                return;
            }
            foreach (var item in ListViewTranslation.Items.OfType<ContentStructure>().ToList())
            {
                item.TranslatedText = await TranslateContent(item.OriginalContent,ComboLang.Text);
            }
            MessageBox.Show("所有生草已完成！");
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if( MessageBox.Show("即将修改当前文件，是否继续？","确认",MessageBoxButton.YesNo) != MessageBoxResult.OK)
            {
                return;
            }
            XDocument xdoc = XDocument.Load(filePath);
            foreach (var item in ListViewTranslation.Items.OfType<ContentStructure>().ToList())
            {
                var elementToModify = xdoc.Descendants().Where(e =>e.Attributes("x:Key").Any(a => a.Value == item.Key)).FirstOrDefault();
                if (elementToModify != null)
                {
                    var contentElement = elementToModify.Element("Content");
                    if (contentElement != null)
                    {
                        contentElement.Value = item.TranslatedText;
                    }
                }
            }
            xdoc.Save(filePath);
        }
    }
}