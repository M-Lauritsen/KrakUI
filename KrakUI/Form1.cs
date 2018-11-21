using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net.Http;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KrakUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => Start());
            t.Start();
        }

        async void Start()
        {
            string q = textBox1.Text;

            var url = "https://www.krak.dk/" + q + "/personer";

            try
            {
                // Using the http client with Async
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);

                // Load the html document
                var HtmlDocument = new HtmlDocument();
                HtmlDocument.LoadHtml(html);

                // a list of every result
                var NameList = HtmlDocument.DocumentNode.Descendants("ul")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("ResultList")).ToList();

                // Single person added to list
                var NameListPeople = NameList[0].Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("PersonResultListItem")).ToList();

                foreach (HtmlNode person in NameListPeople)
                {
                    try
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            richTextBox1.Text = richTextBox1.Text + person.Descendants("div")
                       .Where(node => node.GetAttributeValue("class", "")
                       .Equals("personName")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                            // Address
                            richTextBox1.Text = richTextBox1.Text + person.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("address")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                            // PhoneNumber
                            richTextBox1.Text = richTextBox1.Text + person.Descendants("div")
                            .Where(node => node.GetAttributeValue("role", "")
                            .Equals("button")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t').Substring(0, 11) +Environment.NewLine;
                        }));
                    }
                    catch (Exception ex)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            richTextBox1.Text = richTextBox1.Text + " Skjult" + Environment.NewLine;
                            
                        }));
                    }


                };
            }
            catch (Exception)
            {

                richTextBox1.Text = richTextBox1.Text + "blank" + Environment.NewLine;
            }

            int count = 0;

            
        }
    }
}
