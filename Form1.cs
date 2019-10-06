using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActInstagramCekilis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        public IWebDriver driver { get; set; }
        public async void DriverBaslat(string proxy)
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions chromeOptions = new ChromeOptions();
            if (!string.IsNullOrEmpty(proxy))
            {
                chromeOptions.AddArgument("--proxy-server=" + proxy);
            }
            chromeOptions.AddExcludedArgument("enable-automation");
            chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
            chromeOptions.AddArguments("--allow-file-access");
            chromeOptions.AddArgument("--disable-web-security");
            chromeOptions.AddArgument("--allow-running-insecure-content");
            driver = new ChromeDriver(chromeDriverService, chromeOptions, TimeSpan.FromMinutes(30.0));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            await Task.Delay(2000);
        }

        private async void btnCekilisYap_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    DriverBaslat("");
                    driver.Navigate().GoToUrl(txtGonderiLink.Text);

                    var carpi = driver.FindElement(By.ClassName("xqRnw"));
                    carpi.Click();

                    string xpath = "//*[@id='react-root']/section/main/div/div/article/div[2]/div[1]/ul/li/div/button";
                    var loadMoreBtn = driver.FindElement(By.XPath(xpath));
                    while (loadMoreBtn.Displayed)
                    {
                        loadMoreBtn.Click();
                        try
                        {
                            loadMoreBtn = driver.FindElement(By.XPath(xpath));
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    IList<IWebElement> yorumlar = driver.FindElements(By.ClassName("gElp9"));
                    foreach (var item in yorumlar)
                    {
                        var container = item.FindElement(By.ClassName("C4VMK"));
                        var kullaniciAdi = container.FindElement(By.ClassName("_6lAjh")).Text;
                        lstKatilimcilar.Items.Add(kullaniciAdi);
                    }
                    lstKatilimcilar.Items.RemoveAt(0);
                    Thread.Sleep(2000);
                    driver.Dispose();
                    driver.Quit();


                    int kazananKisiSayisi = Convert.ToInt32(nudKazanacakKisi.Value);
                    int yedekKisiSayisi = Convert.ToInt32(nudYedekKisi.Value);

                    for (int i = 0; i < kazananKisiSayisi; i++)
                    {
                        int cikanKisi = rast.Next(0, lstKatilimcilar.Items.Count);
                        lstKazananlar.Items.Add(lstKatilimcilar.Items[cikanKisi]);
                        lstKatilimcilar.Items.RemoveAt(cikanKisi);
                    }

                    for (int i = 0; i < yedekKisiSayisi; i++)
                    {
                        int cikanKisi = rast.Next(0, lstKatilimcilar.Items.Count);
                        lstYedekler.Items.Add(lstKatilimcilar.Items[cikanKisi]);
                        lstKatilimcilar.Items.RemoveAt(cikanKisi);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        Random rast;
        private void Form1_Load(object sender, EventArgs e)
        {
            rast = new Random();
        }

        public void DosyaKaydet(ListBox lst, string adi)
        {
            try
            {
                string yol = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + adi;
                using (StreamWriter sw = new StreamWriter(yol))
                {
                    foreach (var item in lst.Items)
                    {
                        sw.WriteLine(item.ToString());
                    }
                    sw.Close();
                    MessageBox.Show("Masaüstüne \" " + adi + " \" olarak kayıt edildi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnKatilimciAktar_Click(object sender, EventArgs e)
        {
            DosyaKaydet(lstKatilimcilar, "Katılımcılar.txt");
        }

        private void btnKazananAktar_Click(object sender, EventArgs e)
        {
            DosyaKaydet(lstKazananlar, "Kazananlar.txt");
        }

        private void btnYedekAktar_Click(object sender, EventArgs e)
        {
            DosyaKaydet(lstYedekler, "Yedekler.txt");
        }
    }
}
