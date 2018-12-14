using FileDownloader;
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

        int curIndex1 = -1;
        string dfile1 = "";

        int curIndex2 = -1;
        string dfile2 = "";

        int curIndex3 = -1;
        string dfile3 = "";

        IFileDownloader  fd1 = new FileDownloader.FileDownloader(new DownloadCache());
        IFileDownloader fd2 = new FileDownloader.FileDownloader(new DownloadCache());
        IFileDownloader fd3 = new FileDownloader.FileDownloader(new DownloadCache());
        
        string downloadPath = "";
        string MovieName = "";
        string DownloadLink = "";

        public Form2()
        {
            InitializeComponent();

            fd1.DownloadProgressChanged += Fd_DownloadProgressChanged; 
            fd2.DownloadProgressChanged += Fd_DownloadProgressChanged;
            fd3.DownloadProgressChanged += Fd_DownloadProgressChanged;

            fd1.DownloadFileCompleted += Fd_DownloadFileCompleted; 
            fd2.DownloadFileCompleted += Fd_DownloadFileCompleted;
            fd3.DownloadFileCompleted += Fd_DownloadFileCompleted;

            fd1.Tag = 1;
            fd2.Tag = 2;
            fd3.Tag = 3;
        }

        private void Fd_DownloadProgressChanged(object sender, DownloadFileProgressChangedArgs e)
        {
            var fd = (IFileDownloader)sender;

            if (fd.Tag == 1)
            {
                progressBar1.Value = e.ProgressPercentage;
            }
            else if (fd.Tag == 2)
            {
                progressBar2.Value = e.ProgressPercentage;
            }
            else if (fd.Tag == 3)
            {
                progressBar3.Value = e.ProgressPercentage;
            }
        }

        private void Fd_DownloadFileCompleted(object sender, DownloadFileCompletedArgs e)
        {
            var fd = (IFileDownloader)sender;

            if (fd.Tag == 1)
            {
                if (e.State == CompletedState.Succeeded)
                {
                    listStat.Items.Add("Done - " + dfile1);
                }
                else if (e.State == CompletedState.Failed)
                {
                    listStat.Items.Add("Download failed for -- " + dfile1 + ". Retrying...");
                    curIndex1--;
                }

                var rtn = false;

                while (!rtn)
                {
                    rtn = StartDownload(ref dfile1, listfile1, 1, ref curIndex1, fd1);
                }
            }
            else if(fd.Tag == 2)
            {
                if (e.Error == null)
                {
                    listStat.Items.Add("Done - " + dfile2);
                    listStat.TopIndex = listStat.Items.Count - 1;
                }
                else
                {
                    listStat.Items.Add("Download failed for -- " + dfile2 + ". Retrying...");
                    curIndex2--;
                }

                var rtn = false;

                while (!rtn)
                {
                    rtn = StartDownload(ref dfile2, listfile2, 2, ref curIndex2, fd2);
                }
            }
            else if (fd.Tag == 3)
            {
                if (e.Error == null)
                {
                    listStat.Items.Add("Done - " + dfile3);
                    listStat.TopIndex = listStat.Items.Count - 1;
                }
                else
                {
                    listStat.Items.Add("Download failed for -- " + dfile3 + ". Retrying...");
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
                    rtn = StartDownload(ref dfile3, listfile3, 3, ref curIndex3, fd3);
                }
            }
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

        private bool StartDownload(ref string dfile, List<string> curList, int listNum, ref int curIndex, IFileDownloader curFD)
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
                    curFD.DownloadFileAsync(new Uri(curfilename), Path.Combine(downloadPath, dfile));
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
        
        private void btnDownload_Click_1(object sender, EventArgs e)
        {
            if (chkFileList.Items.Count < 1)
            {
                MessageBox.Show("No file in the download list. Click fetch to get the Fies you want to download.");
                return;
            }

            if (fd1.isBusy || fd2.isBusy || fd3.isBusy)
            {
                MessageBox.Show("Download is ongoing, Please wait until its done.");
                return;
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

            var curfilename = listfile1[curIndex1];

            if (listfile1.Count < 0)
            {
                listStat.Items.Add("No file in list 1...");
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                dfile1 = Uri.UnescapeDataString(curfilename).Split('/').Last();
                
                var rtn = false;

                while (!rtn)
                {
                    rtn = StartDownload(ref dfile1, listfile1, 1, ref curIndex1, fd1);
                }

            }

            if (listfile2.Count < 0)
            {
                listStat.Items.Add("No file in list 2...");
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                curfilename = listfile2[curIndex2];
                dfile2 = Uri.UnescapeDataString(curfilename).Split('/').Last();
                
                var rtn = false;

                while (!rtn)
                {
                    rtn = StartDownload(ref dfile2, listfile2, 2, ref curIndex2, fd2);
                }
            }


            if (listfile3.Count < 0)
            {
                listStat.Items.Add("No file in list 3...");
                listStat.TopIndex = listStat.Items.Count - 1;
            }
            else
            {
                curfilename = listfile3[curIndex3];
                dfile3 = Uri.UnescapeDataString(curfilename).Split('/').Last();

                var rtn = false;

                while (!rtn)
                {
                    rtn = StartDownload(ref dfile3, listfile3, 3, ref curIndex3, fd3);
                }
            }
        }
        
    }
}
