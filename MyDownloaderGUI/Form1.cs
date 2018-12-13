using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDownloaderGUI
{
    public partial class Form1 : Form
    {
        List<string> listfile1 = new List<string>();
        List<string> listfile2 = new List<string>();
        List<string> listfile3 = new List<string>();

        int curIndex1 = 0;
        string dfile1 = "";
        bool Completed1 = false;

        int curIndex2 = 0;
        string dfile2 = "";
        bool Completed2 = false;

        int curIndex3 = 0;
        string dfile3 = "";
        bool Completed3 = false;

        BackgroundWorker bk1 = new BackgroundWorker();
        BackgroundWorker bk2 = new BackgroundWorker();
        BackgroundWorker bk3 = new BackgroundWorker();

        string downloadPath = "";
        string MovieName = "";
        string DownloadLink = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            MovieName = txtName.Text;
            
            DownloadLink = txtPath.Text;

            System.Net.ServicePointManager.DefaultConnectionLimit = 100;

            downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), MovieName.Replace(" ", ""));
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            listStat.Items.Add("Fetching Links...");

            var listfiles = MyScrapper.Webscraper(DownloadLink);

            if (listfiles == null)
            {
                listStat.Items.Add($"Did not get any file.");
                return;
            }

            listStat.Items.Add($"Got {listfiles.Count} files.");

            listStat.Items.Add("Initiating Download...");

            for (int i = 0; i < listfiles.Count; i++)
            {
                if (i % 3 == 0)
                {
                    listfile1.Add(listfiles[i]);
                }
                else if (i % 3 == 1)
                {
                    listfile2.Add(listfiles[i]);
                }
                else
                {
                    listfile3.Add(listfiles[i]);
                }
            }

            listStat.Items.Add("Starting Download...");

            bk1.DoWork += Bk1_DoWork;
            bk1.RunWorkerCompleted += Bk1_RunWorkerCompleted;
            bk1.ProgressChanged += Bk1_ProgressChanged;
            bk1.RunWorkerAsync();

            bk2.DoWork += Bk2_DoWork;
            bk2.ProgressChanged += Bk2_ProgressChanged;
            bk2.RunWorkerCompleted += Bk2_RunWorkerCompleted;
            bk2.RunWorkerAsync();

            bk3.DoWork += Bk3_DoWork;
            bk3.ProgressChanged += Bk3_ProgressChanged;
            bk3.RunWorkerCompleted += Bk3_RunWorkerCompleted;
            bk3.RunWorkerAsync();

            listStat.Items.Add("Download started.");
        }

        private void Bk1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Bk1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Completed1) listStat.Items.Add("Done - " + dfile1);
            else
            {
                listStat.Items.Add("Download failed for -- " + dfile1 + ". Will try again later.");
                listfile1.Add(listfile1[curIndex1]);
                if (File.Exists(Path.Combine(downloadPath, dfile1)))
                {
                    File.Delete(Path.Combine(downloadPath, dfile1));
                }
            }

            curIndex1++;
            if (curIndex1 >= listfile1.Count)
            {
                listStat.Items.Add("Download 1 complete.");
            }
            else bk1.RunWorkerAsync();

        }

        private void Bk1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var wc = new WebClient())
            {
                var curfilename = listfile1[curIndex1];
                dfile1 = curfilename.Split('/').Last();
                if (!File.Exists(Path.Combine(downloadPath, dfile1)))
                {
                    try
                    {
                        wc.DownloadProgressChanged += Wc1_DownloadProgressChanged;
                        wc.DownloadFile(curfilename, Path.Combine(downloadPath, dfile1));
                        Completed1 = true;
                    }
                    catch (Exception)
                    {
                        Completed1 = false;
                    }
                }
            }
        }

        private void Wc1_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                
                bk1.ReportProgress(int.Parse(Math.Truncate(percentage).ToString()));
            });
        }

        private void Bk2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }

        private void Bk2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Completed2) listStat.Items.Add("Done - " + dfile2);
            else
            {
                listStat.Items.Add("Download failed for -- " + dfile2 + ". Will try again later.");
                listfile2.Add(listfile2[curIndex2]);
                if (File.Exists(Path.Combine(downloadPath, dfile2)))
                {
                    File.Delete(Path.Combine(downloadPath, dfile2));
                }
            }

            curIndex2++;
            if (curIndex2 >= listfile2.Count)
            {
                listStat.Items.Add("Download 2 complete.");
            }
            else bk2.RunWorkerAsync();

        }

        private void Bk2_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var wc = new WebClient())
            {
                var curfilename = listfile2[curIndex2];
                dfile2 = curfilename.Split('/').Last();
                if (!File.Exists(Path.Combine(downloadPath, dfile2)))
                {
                    try
                    {
                        wc.DownloadProgressChanged += Wc2_DownloadProgressChanged;
                        wc.DownloadFile(curfilename, Path.Combine(downloadPath, dfile2));
                        Completed2 = true;
                    }
                    catch (Exception)
                    {
                        Completed2 = false;
                    }
                }
            }
        }

        private void Wc2_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;

                bk2.ReportProgress(int.Parse(Math.Truncate(percentage).ToString()));
            });
        }

        private void Bk3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar3.Value = e.ProgressPercentage;
        }

        private void Bk3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Completed3) listStat.Items.Add("Done - " + dfile3);
            else
            {
                listStat.Items.Add("Download failed for -- " + dfile3 + ". Will try again later.");
                listfile3.Add(listfile3[curIndex3]);
                if (File.Exists(Path.Combine(downloadPath, dfile3)))
                {
                    File.Delete(Path.Combine(downloadPath, dfile3));
                }
            }

            curIndex3++;
            if (curIndex3 >= listfile3.Count)
            {
                listStat.Items.Add("Download 3 complete.");
            }
            else bk3.RunWorkerAsync();

        }

        private void Bk3_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var wc = new WebClient())
            {
                var curfilename = listfile3[curIndex3];
                dfile3 = curfilename.Split('/').Last();
                if (!File.Exists(Path.Combine(downloadPath, dfile3)))
                {
                    try
                    {
                        wc.DownloadProgressChanged += Wc3_DownloadProgressChanged;
                        wc.DownloadFile(curfilename, Path.Combine(downloadPath, dfile3));
                        Completed3 = true;
                    }
                    catch (Exception)
                    {
                        Completed3 = false;
                    }
                }
            }
        }

        private void Wc3_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;

                bk3.ReportProgress(int.Parse(Math.Truncate(percentage).ToString()));
            });
        }
    }
}
