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
    public partial class Form2 : Form
    {
        List<string> listfile1 = new List<string>();
        List<string> listfile2 = new List<string>();
        List<string> listfile3 = new List<string>();

        int curIndex1 = 0;
        string dfile1 = "";

        int curIndex2 = 0;
        string dfile2 = "";

        int curIndex3 = 0;
        string dfile3 = "";

        WebClient wc1 = new WebClient();
        WebClient wc2 = new WebClient();
        WebClient wc3 = new WebClient();

        string downloadPath = "";
        string MovieName = "";
        string DownloadLink = "";

        public Form2()
        {
            InitializeComponent();

            wc1.DownloadProgressChanged += Wc1_DownloadProgressChanged;
            wc2.DownloadProgressChanged += Wc2_DownloadProgressChanged;
            wc3.DownloadProgressChanged += Wc3_DownloadProgressChanged;

            wc1.DownloadFileCompleted += Wc1_DownloadFileCompleted;
            wc2.DownloadFileCompleted += Wc2_DownloadFileCompleted;
            wc3.DownloadFileCompleted += Wc3_DownloadFileCompleted;
            
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            MovieName = txtName.Text;

            DownloadLink = txtPath.Text;

            ServicePointManager.DefaultConnectionLimit = 100;

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
                Cursor = Cursors.Arrow;
                return;
            }
            else
            {
                chkFileList.Items.Clear();

                foreach (var item in listfiles)
                {
                    chkFileList.Items.Add(item, true);
                }
            }

            listStat.Items.Add($"Got {listfiles.Count} files.");

            Cursor = Cursors.Arrow;
        }

        private bool done(ref string dfile, List<string> curList, int listNum, ref int curIndex, WebClient curWebClient)
        {
            curIndex++;

            if (curIndex >= curList.Count)
            {
                listStat.Items.Add($"Download {listNum.ToString()} complete.");
            }
            else
            {
                var curfilename = curList[curIndex];
                dfile = curfilename.Split('/').Last();
                if (!File.Exists(Path.Combine(downloadPath, dfile)))
                {
                    curWebClient.DownloadFileAsync(new Uri(curfilename), Path.Combine(downloadPath, dfile));
                    listStat.Items.Add($"Downloading file {curIndex + 1} out of {curList.Count}");
                }
                else
                {
                    listStat.Items.Add($"{dfile} already exists. Loading Next...");
                    listStat.TopIndex = listStat.Items.Count - 1;
                    return false;
                }
            }

            listStat.TopIndex = listStat.Items.Count - 1;
            return true;
        }
        private void Wc1_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                listStat.Items.Add("Done - " + dfile1);
            }
            else
            {
                listStat.Items.Add("Download failed for -- " + dfile1 + ". Will try again later.");
                listfile1.Add(listfile1[curIndex1]);
                if (File.Exists(Path.Combine(downloadPath, dfile1)))
                {
                    File.Delete(Path.Combine(downloadPath, dfile1));
                }
            }

            var rtn = false;

            while (!rtn)
            {
                rtn = done(ref dfile1, listfile1, 1, ref curIndex1, wc1);
            }
        }

        private void Wc2_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                listStat.Items.Add("Done - " + dfile2);
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                listStat.Items.Add("Download failed for -- " + dfile2 + ". Will try again later.");
                listStat.TopIndex = listStat.Items.Count - 1;
                listfile2.Add(listfile2[curIndex2]);
                if (File.Exists(Path.Combine(downloadPath, dfile2)))
                {
                    File.Delete(Path.Combine(downloadPath, dfile2));
                }
            }

            var rtn = false;

            while (!rtn)
            {
                rtn = done(ref dfile2, listfile2, 2, ref curIndex2, wc2);
            }
        }

        private void Wc3_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                listStat.Items.Add("Done - " + dfile3);
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                listStat.Items.Add("Download failed for -- " + dfile3 + ". Will try again later.");
                listStat.TopIndex = listStat.Items.Count - 1;

                listfile3.Add(listfile3[curIndex3]);
                if (File.Exists(Path.Combine(downloadPath, dfile3)))
                {
                    File.Delete(Path.Combine(downloadPath, dfile3));
                }
            }

            var rtn = false;

            while (!rtn)
            {
                rtn = done(ref dfile3, listfile3, 3, ref curIndex3, wc3);
            }
        }
        
        private void Wc1_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Wc2_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }

        private void Wc3_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar3.Value = e.ProgressPercentage;
        }

        private void btnDownload_Click_1(object sender, EventArgs e)
        {
            if (chkFileList.Items.Count < 1)
            {
                MessageBox.Show("No file in the download list. Click fetch to get the Fies you want to download.");
            }

            var listfiles = new List<string>();
            
            foreach (string item in chkFileList.CheckedItems)
            {
                listfiles.Add(item);
            }


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
            listStat.TopIndex = listStat.Items.Count - 1;

            if (listfile1.Count < 0)
            {
                listStat.Items.Add("No file in list 1...");
                listStat.TopIndex = listStat.Items.Count - 1;
            }

            var curfilename = listfile1[curIndex1];
            dfile1 = curfilename.Split('/').Last();
            if (!File.Exists(Path.Combine(downloadPath, dfile1)))
            {
                wc1.DownloadFileAsync(new Uri(curfilename), Path.Combine(downloadPath, dfile1));
                listStat.Items.Add($"Downloading file {curIndex1 + 1} out of {listfile1.Count}");
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                var rtn = false;

                while (!rtn)
                {
                    rtn = done(ref dfile1, listfile1, 1, ref curIndex1, wc1);
                }
            }

            if (listfile2.Count < 0)
            {
                listStat.Items.Add("No file in list 2...");
                listStat.TopIndex = listStat.Items.Count - 1;
            }

            curfilename = listfile2[curIndex2];
            dfile2 = curfilename.Split('/').Last();
            if (!File.Exists(Path.Combine(downloadPath, dfile2)))
            {
                wc2.DownloadFileAsync(new Uri(curfilename), Path.Combine(downloadPath, dfile2));
                listStat.Items.Add($"Downloading file {curIndex2 + 1} out of {listfile2.Count}");
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                var rtn = false;

                while (!rtn)
                {
                    rtn = done(ref dfile2, listfile2, 2, ref curIndex2, wc2);
                }
            }


            if (listfile3.Count < 0)
            {
                listStat.Items.Add("No file in list 3...");
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            curfilename = listfile3[curIndex3];
            dfile3 = curfilename.Split('/').Last();
            if (!File.Exists(Path.Combine(downloadPath, dfile3)))
            {
                wc3.DownloadFileAsync(new Uri(curfilename), Path.Combine(downloadPath, dfile3));
                listStat.Items.Add($"Downloading file {curIndex3 + 1} out of {listfile3.Count}");
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                var rtn = false;

                while (!rtn)
                {
                    rtn = done(ref dfile3, listfile3, 3, ref curIndex3, wc3);
                }
            }
        }
    }
}
