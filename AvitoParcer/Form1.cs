using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System.Threading;
using System.Text.RegularExpressions;


namespace AvitoParcer
{
    public partial class Form1 : Form
    {
        ChromeDriver ChDriver;
        public Form1()
        {
            InitializeComponent();
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disk-cache-size=1", "--incognito");
            ChDriver = new ChromeDriver(options);
            ChDriver.Navigate().GoToUrl("https://www.avito.ru/istra/avtomobili/audi?radius=0");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private List<string> SeparatePageSourceCode(string pageSource)
        {
            List<string> item = new List<string>();
            var temp = Parce.FindElements(pageSource, By.regexForPosts);
            item = (from Match arr in temp
                    select arr.Groups[0].Value).ToList();
            return item;
        }

        public delegate void Method();
        

        private void WriterToJson(string data, int index, ref bool isCompleted)
        {
            AvitoPost avitoPost = AvitoPost.FormObject(data);

            string fileName = "AvitoPost" + index + ".txt";
            //File.WriteAllText(fileName, JsonConvert.SerializeObject(avitoPost, Formatting.Indented));
            StreamWriter streamWriter = new StreamWriter(fileName);
            streamWriter.Write(JsonConvert.SerializeObject(avitoPost, Formatting.Indented));
            streamWriter.Close();

            isCompleted = true;
        }
        private void WriterToJson(string data, int index, ref bool isCompleted, Method breaker)
        {

            breaker();
            AvitoPost avitoPost = AvitoPost.FormObject(data);

            string fileName = "AvitoPost" + index + ".txt";
            //File.WriteAllText(fileName, JsonConvert.SerializeObject(avitoPost, Formatting.Indented));
            StreamWriter streamWriter = new StreamWriter(fileName);
            streamWriter.Write(JsonConvert.SerializeObject(avitoPost, Formatting.Indented));
            streamWriter.Close();

            isCompleted = true;
        }
        // static Task[] tasks;

        private void button1_Click(object sender, EventArgs e)
        {
            string pageCode = ChDriver.PageSource;

            List<string> separatedPage = new List<string>();
            separatedPage = SeparatePageSourceCode(pageCode);
            bool[] isCompleted = new bool[separatedPage.Count()];
            Task[] tasks = new Task[separatedPage.Count()];
            for (int i = 0; i < separatedPage.Count(); i++)
            {
                int j = i;
                isCompleted[i] = false;
                if (!(j == 6  || j == 0))
                    tasks[j] = Task.Factory.StartNew(() => WriterToJson(separatedPage[j], j, ref isCompleted[j]));
                else
                {
                    tasks[j] = Task.Factory.StartNew((() => WriterToJson(separatedPage[j], j, ref isCompleted[j], (Method)(() => Thread.CurrentThread.Abort()))));
                }
            }

            while (!tasks.All(task => task.IsCompleted) || !isCompleted.All(x => x))
            {
                for (int i = 0; i < separatedPage.Count(); i++)
                {
                    int j = i;

                    if (tasks[j].IsFaulted && !isCompleted[j])
                    {
                        Task avaiableTask = (from task in tasks
                                             where task.Status != TaskStatus.Faulted
                                                || task.Status != TaskStatus.Canceled
                                             select task).First();
                        avaiableTask.ContinueWith(antecedent => WriterToJson(separatedPage[j], j, ref isCompleted[j]));
                        Thread.Sleep(3000);
                    }
                }
            }
        }
    }
}
