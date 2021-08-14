using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class frmUpload : Form
    {
        public frmUpload()
        {
            InitializeComponent();
        }

        private void btnSelected_Click(object sender, EventArgs e)
        {
            SetProgress(0);
            PrintProgress("0%");
            lblduration.Text = "0.0秒";
            txtPath.Clear();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "please choose file";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "All(*json*)|*.json*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //string content = ReadBinaryFileToString(openFileDialog.OpenFile());
                txtPath.Text = openFileDialog.FileName;
            }
        }



        #region Basics Info
        /// <summary>
        /// set process
        /// </summary>
        /// <param name="value"></param>
        void SetProgress(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                this.progressBar1.BeginInvoke(new Action(() =>
                {
                    progressBar1.Value = value;
                }));
            }
            else
            {
                this.progressBar1.Value = value;
            }
        }

        /// <summary>
        /// pint process
        /// </summary>
        /// <param name="msg"></param>
        void PrintProgress(string msg)
        {
            if (this.InvokeRequired)
            {
                lblpercent.BeginInvoke(new Action(() =>
                {
                    lblpercent.Text = msg;
                }));
            }
            else
            {
                this.lblpercent.Text = msg;
            }
        }
        #endregion

        #region Read Text
        StringBuilder ReadBinaryFileToString(Stream inputSream)
        {
            //total length
            long totalBytes = inputSream.Length;

            byte[] buffer = new byte[1024];
            using BinaryReader binaryReader = new(inputSream);
            StringBuilder stringBuilder = new();
            while ((binaryReader.Read(buffer, 0, buffer.Length)) > 0)
            {
                var percent = (int)((double)binaryReader.BaseStream.Position / totalBytes * 100);

                SetProgress(percent);
                PrintProgress($"{percent}%");
                stringBuilder.Append(binaryReader.ReadString());
            }
            return stringBuilder;
        }

        /// <summary>
        /// read file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        async Task<StringBuilder> ReadFileToString(string filePath)
        {
            using StreamReader reader = new(filePath);
            long totalLength = reader.BaseStream.Length;

            StringBuilder stringBuilder = new();
            string content = string.Empty;
            while ((content = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                var percent = (int)((double)reader.BaseStream.Position / totalLength * 100);
                SetProgress(percent);
                PrintProgress($"{percent}%");
                stringBuilder.Append(content);
            }
            return stringBuilder;
        }

        #endregion

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            this.btnSelected.Enabled = false;
            this.btnUpload.Enabled = false;

            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                MessageBox.Show("sry.please choose file!");
                return;
            }

            Stopwatch stopwatch = new();
            stopwatch.Start();
            string content = (await ReadFileToString(txtPath.Text)).ToString();
            stopwatch.Stop();
            lblduration.Text = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 1)}秒";
            lblduration.Refresh();

            Form2 form2 = new();
            form2.content = content;
            this.Hide();
            form2.Show();
            this.btnSelected.Enabled = true;
            this.btnUpload.Enabled = true;
        }

        private void frmUpload_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit the program？", "exit program", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Dispose(true);
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
