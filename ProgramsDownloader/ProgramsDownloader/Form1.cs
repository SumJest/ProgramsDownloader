using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace ProgramsDownloader
{
    public partial class Form1 : Form
    {
        //Fields
        long size;
        readonly string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        List<string> urls = new List<string>();
        
        //Initialize
        public Form1()
        { 
            InitializeComponent();
        }

        //Methods
        private void onNewClick(object sender, EventArgs args)
        {
            alalalal();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                HttpWebResponse res = (HttpWebResponse)HttpWebRequest.Create("http://sumjest.ru/programsinfo/programs.txt").GetResponse();
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new StreamReader(res.GetResponseStream(), encoding))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] linea = line.Split(';');
                        
                        if (line.Split(';')[0].Contains("ProgramsDownloader"))
                        {
                            Version v;
                            if (Version.TryParse(line.Split(';')[1], out v)) { if (v.CompareTo(Version.Parse(Application.ProductVersion)) > 0) { linea[1] += " - Новая версия!"; menuStrip1.Items.Add("Вышла новая версия программы!", null, onNewClick); } }

                        }
                        comboBox1.Items.Add(linea[0] + " " + linea[1]);
                        urls.Add(line);
                    }
                    comboBox1.Enabled = true;
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message , ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void download(string url, string path)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)HttpWebRequest.Create(url).GetResponse();
                size = response.ContentLength;
                response.Close();

                WebClient client = new WebClient();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                client.DownloadFileAsync(new Uri(url), path);

                toolStripStatusLabel1.Text = "0 / " + getSize(size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + url, ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            toolStripProgressBar1.Value = args.ProgressPercentage;
            toolStripStatusLabel1.Text = getSize(size * args.ProgressPercentage/100) + " / " + getSize(size);
        }
        private void Completed(object sender, AsyncCompletedEventArgs args)
        {
            toolStripProgressBar1.Value = 0;
            size = 0;
            toolStripStatusLabel1.Text = "Done";
            
        }
        private string getSize(long sizeoffile)
        {
            double len = double.Parse(sizeoffile.ToString());

            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }


            return String.Format("{0:0.##} {1}", Math.Round(len, 2), sizes[order]);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            label4.Text = urls.ToArray()[comboBox1.SelectedIndex].Split(';')[0];
            label5.Text = urls.ToArray()[comboBox1.SelectedIndex].Split(';')[1];
        }
        private string getNumFileName(string filename, int number)
        {
            return Path.GetFileNameWithoutExtension(filename) + " (" + number + ")" + Path.GetExtension(filename);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient client = new WebClient();
                var data = client.DownloadData(urls.ToArray()[comboBox1.SelectedIndex].Split(';')[2]);
                string[] fileNameA = urls.ToArray()[comboBox1.SelectedIndex].Split(';')[2].Split('/');
                string fileName = fileNameA[fileNameA.Length - 1];
                // Try to extract the filename from the Content-Disposition header
                //if (!String.IsNullOrEmpty(client.ResponseHeaders["Content-Disposition"]))
                //{
                //    fileName = client.ResponseHeaders["Content-Disposition"].Substring(client.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 10).Replace("\"", "").Replace("t; filename*=UTF-8\'\'", "");
                //}
                SaveFileDialog sfd = new SaveFileDialog();
                string extension = Path.GetExtension(fileName);
                //object value = Microsoft.Win32.Registry.GetValue("HKEY_CLASSES_ROOT\\" + extension, "", null);
                //if (value != null)
                //{
                //    string opis = new System.Globalization.CultureInfo("ru-RU", false).TextInfo.ToTitleCase(value.ToString());
                //    opis = opis.Replace('_', ' ');
                //    sfd.Filter = opis + " | *" + extension;
                //}
                //else
                //{
                //    sfd.Filter = new System.Globalization.CultureInfo("ru-RU", false).TextInfo.ToTitleCase(extension.Remove(0, 1)) + " | *" + extension;
                //}
                sfd.Filter = "Zip-archive | *" + extension;
                sfd.DefaultExt = extension.Replace(".", "");
                sfd.InitialDirectory = Application.StartupPath;
                if (File.Exists(Application.StartupPath + @"\" + fileName))
                {
                    for (int i = 1; i < int.MaxValue; i++)
                    {
                        if (File.Exists(Application.StartupPath + @"\" + getNumFileName(fileName, i)))
                        {
                            continue;
                        }
                        else
                        {
                            sfd.FileName = getNumFileName(fileName, i);
                            break;
                        }
                    }
                }
                else
                {
                    sfd.FileName = fileName;
                }


                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    download(urls.ToArray()[comboBox1.SelectedIndex].Split(';')[2], sfd.FileName);
                    System.Diagnostics.Process.Start("explorer.exe", /*Path.GetDirectoryName(sfd.FileName) + */"/select, " + sfd.FileName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + urls.ToArray()[comboBox1.SelectedIndex], ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
}

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }
        private void alalalal()
        {
            HttpWebRequest proxy_request = (HttpWebRequest)WebRequest.Create("http://sumjest.ru/index/programs_downloader/0-6");
            proxy_request.Method = "GET";
            proxy_request.Timeout = 20000;
            HttpWebResponse resp = proxy_request.GetResponse() as HttpWebResponse;
            string html = "";
            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                html = sr.ReadToEnd();
            string a = Regex.Match(html, @"<!--Dangerous--><p>Change log:([\s\S]*)\<!--Dangerous-->").ToString();
            a = a.Replace("<!--Dangerous-->", "");
            a = a.Replace("<p>", "");
            a = a.Replace("</p>", "");
            a = a.Replace("<br/>", "");
            a = a.Replace("&nbsp;", "  ");
            MessageBox.Show(a);

        }
    }
}
