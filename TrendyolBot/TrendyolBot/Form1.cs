using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using OpenQA.Selenium.Chrome;

using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using Dapper;

namespace TrendyolBot
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        IWebDriver driver;

        private void button1_Click(object sender, EventArgs e)
        {
            CreateAccount(Convert.ToInt32(textBox1.Text));
        }

        public async void CreateAccount(int number)
        {
          
            Random random = new Random();
            

            for (int i = 0; i < number; i++)
            {

             ;

                driver = new ChromeDriver("driver/chromedriver.exe");

                driver.Manage().Window.Maximize();
                RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
                driver.Url = "https://www.trendyol.com/uyelik";
                var mail = await GetMail();
                richTextBox1.Text = mail;
                var dom = mail.Split('@');
                string par1 = dom[0];
                string par2 = dom[1];
                var password = randomNumberGenerator.RandomPassword();
                driver.FindElement(By.Id("register-email")).SendKeys(mail);
                driver.FindElement(By.Id("register-password-input")).SendKeys(password);

                // driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 400)");
                var checkbox = driver.FindElements(By.ClassName("ty-check")).Last();
                checkbox.Click();

                driver.FindElement(By.XPath("//*[@id='login-register']/div[3]/div[1]/form/button")).Click();
                string kod = await GetMailBox(par1, par2);
                if (kod.Length < 4)
                {
                    driver.Quit();
                    continue;
                }

                driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div[3]/div[1]/div/div/div/div[2]/form/div[2]/input")).Click();
                driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div[3]/div[1]/div/div/div/div[2]/form/div[2]/input")).SendKeys(kod);

                driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div[3]/div[1]/div/div/div/div[2]/form/div[3]/button[1]")).Click();
                string sonuc = mail + "  " + password;
                hesaplar.Add(sonuc);
                Thread.Sleep(5000);
                driver.Quit();
                

            }
            writetxt(hesaplar);



            //bdd-
        }
            List<string> hesaplar = new List<string>();
        public void writetxt(List<string> text)
        {
            string file = "C:\\Users\\Msi\\Desktop\\hesaplar.txt";

            //Ýþlem yapacaðýmýz dosyanýn yolunu belirtiyoruz.
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Write);
            //Bir file stream nesnesi oluþturuyoruz. 1.parametre dosya yolunu,
            //2.parametre dosya varsa açýlacaðýný yoksa oluþturulacaðýný belirtir,
            //3.parametre dosyaya eriþimin veri yazmak için olacaðýný gösterir.
            StreamWriter sw = new StreamWriter(fs);
            //Yazma iþlemi için bir StreamWriter nesnesi oluþturduk.
            foreach (var item in text)
            {
                sw.WriteLine(item);

            }

            //Dosyaya ekleyeceðimiz iki satýrlýk yazýyý WriteLine() metodu ile yazacaðýz.
            sw.Flush();
            //Veriyi tampon bölgeden dosyaya aktardýk.
            sw.Close();
            fs.Close();



        }
        public class RandomNumberGenerator
        {
            // Generate a random number between two numbers    
            public int RandomNumber(int min, int max)
            {
                Random random = new Random();
                return random.Next(min, max);
            }

            // Generate a random string with a given size and case.   
            // If second parameter is true, the return string is lowercase  
            public string RandomString(int size, bool lowerCase)
            {
                StringBuilder builder = new StringBuilder();
                Random random = new Random();
                char ch;
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                if (lowerCase)
                    return builder.ToString().ToLower();
                return builder.ToString();
            }

            // Generate a random password of a given length (optional)  
            public string RandomPassword(int size = 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(RandomString(4, true));
                builder.Append(RandomNumber(1000, 9999));
                builder.Append(RandomString(2, false));
                return builder.ToString();
            }
        }
        public async Task<string> GetMail()
        {
            String result = "";
            HttpClient client = new HttpClient();
            using (var response = await client.GetAsync("https://www.1secmail.com/api/v1/?action=genRandomMailbox&count=1").ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    result = result.Remove(0, 2);
                    int len = result.Length;
                    len = len - 2;
                    result = result.Remove(len, 2);

                }
            }
            return result;

        }

        public async Task<string> GetMailBox(string parameter1, string parameter2)
        {

            String result = "";
            HttpClient client = new HttpClient();
            string kod = "";
            string url = "https://www.1secmail.com/api/v1/?action=getMessages&login=" + parameter1 + "&domain=" + parameter2;
            using (var response = await client.GetAsync(url).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var response1 = response;
                    DateTime dt = DateTime.Now;
                    result = await response1.Content.ReadAsStringAsync().ConfigureAwait(false);
                    while (result.Length < 3)
                    {
                        DateTime dt1 =  DateTime.Now;
                        if ((dt1 - dt).Seconds > 30)
                        {
                            return "";
                        }
                        response1 = await client.GetAsync(url).ConfigureAwait(false);
                        result = await response1.Content.ReadAsStringAsync().ConfigureAwait(false);


                    }
                    var model = await Task.Run(() =>
                           JsonConvert.DeserializeObject<List<MailModel>>(result)
                        ).ConfigureAwait(false);
                    var mail = model.FirstOrDefault();
                    string mesage = mail.Subject;
                    kod = mesage.Substring(mesage.Length - 6, 6);



                }
            }
            return kod;



        }
        public partial class MailModel
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("from")]
            public string From { get; set; }

            [JsonProperty("subject")]
            public string Subject { get; set; }

            [JsonProperty("date")]
            public DateTimeOffset Date { get; set; }
        }
    }
}