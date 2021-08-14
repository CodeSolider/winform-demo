using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp.Extensions;
using WinFormsApp.Models;

namespace WinFormsApp
{
    public partial class Form2 : Form
    {
        /// <summary>
        /// content
        /// </summary>
        public string content = string.Empty;

        /// <summary>
        ///  records
        /// </summary>
        private List<Record> recordsList;

        private int pageIndex = 1;

        private int pageSize = 10;

        /// <summary>
        /// log 
        /// </summary>

        private readonly ILogger logger;

        private int totalPage = 0;

        private const string url = "http://120.25.147.194:10000/RequestTaskHandler.ashx";
        private HttpClient httpClient;
        public Form2()
        {
            InitializeComponent();
            logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
        }

        /// <summary>
        /// close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
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

        private async void Form2_Load(object sender, EventArgs e)
        {
            await LoadData();
            AddDatagridviewCheckBox();
        }

        async Task LoadData()
        {
            recordsList = JsonConvert.DeserializeObject<Data>(content)?.Records;
            pageIndex = 1;
            pageSize = 10;
            totalPage = (int)Math.Ceiling(recordsList.Count / (double)pageSize);
            updateLblPage();
            BindDatagridviewData(await recordsList.ToPage(pageIndex, pageSize: pageSize));
        }

        #region Simple Write Pagination

        /// <summary>
        /// Bind data
        /// </summary>
        /// <param name="records"></param>
        void BindDatagridviewData(List<Record> records)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.BeginInvoke(new Action(() =>
                {
                    dataGridView1.DataSource = records;
                }));
            }
            else
            {
                dataGridView1.DataSource = records;
            }
        }

        void AddDatagridviewCheckBox()
        {
            DataGridViewCheckBoxColumn checkbox = new DataGridViewCheckBoxColumn();
            checkbox.HeaderText = "IsSelected";
            checkbox.Name = "IsChecked";
            checkbox.TrueValue = true;
            checkbox.FalseValue = false;
            checkbox.DataPropertyName = "IsChecked";
            checkbox.Width = 20;
            checkbox.Resizable = DataGridViewTriState.False;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.Columns.Insert(0, checkbox);
        }

        /// <summary>
        /// first page  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnFirst_Click(object sender, EventArgs e)
        {
            if (pageIndex == 1)
            {
                MessageBox.Show("sry.the current page already home page.", "Warning Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            pageIndex = 1;
            BindDatagridviewData(await recordsList.ToPage(pageIndex, pageSize: pageSize));
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            //last page
            if (pageIndex == totalPage)
            {
                MessageBox.Show("sry.the current page already last page.", "Warning Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            pageIndex++;
            updateLblPage();
            BindDatagridviewData(await recordsList.ToPage(pageIndex, pageSize: pageSize));
        }


        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            if (pageIndex == 1)
            {
                MessageBox.Show("sry.the current page is home page,no previous page.", "Warning Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            pageIndex--;
            BindDatagridviewData(await recordsList.ToPage(pageIndex, pageSize: pageSize));
        }

        private async void btnLast_Click(object sender, EventArgs e)
        {
            pageIndex = totalPage;
            updateLblPage();

            BindDatagridviewData(await recordsList.ToPage(pageIndex, pageSize: pageSize));
        }

        private async void txtJumpPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJumpPage.Text))
            {
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                if (!RegexInteger(txtJumpPage.Text))
                {
                    MessageBox.Show("input character is illegal", "Warning Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (Convert.ToInt32(this.txtJumpPage.Text) > totalPage)
                {
                    MessageBox.Show("sry.input page number cann't be greater than the total page number", "Warning Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                pageIndex = Convert.ToInt32(this.txtJumpPage.Text);
                updateLblPage();
                BindDatagridviewData(await recordsList.ToPage(pageIndex, pageSize: pageSize));
                this.txtJumpPage.Clear();
            }
        }




        void updateLblPage()
        {
            if (dataGridView1.InvokeRequired)
            {
                this.lblPage.BeginInvoke(new Action(() =>
                {
                    lblPage.Text = $"当前第{pageIndex}页，总共{(int)Math.Ceiling(recordsList.Count / (double)pageSize)}页,每页{pageSize}条数据";
                }));
            }
            else
            {
                lblPage.Text = $"当前第{pageIndex}页，总共{(int)Math.Ceiling(recordsList.Count / (double)pageSize)}页,每页{pageSize}条数据";
            }
        }

        bool RegexInteger(string IInteger)
        {
            Regex regex = new Regex(@"^[0-9]\d*$");
            return regex.IsMatch(IInteger);
        }

        #endregion

        #region Seleced Action

        /// <summary>
        /// selected All
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    bool.TryParse(row.Cells["IsChecked"].EditedFormattedValue.ToString(), out bool isChecked);
                    if (!isChecked)
                        row.Cells["IsChecked"].Value = "True";
                }
            });
        }

        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    bool.TryParse(row.Cells["IsChecked"].EditedFormattedValue.ToString(), out bool isChecked);
                    if (isChecked)
                        row.Cells["IsChecked"].Value = "False";
                }
            });
        }

        private void btnReverseSelect_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    bool.TryParse(row.Cells["IsChecked"].EditedFormattedValue.ToString(), out bool isChecked);
                    if (isChecked)
                        row.Cells["IsChecked"].Value = "False";
                    else
                        row.Cells["IsChecked"].Value = "True";
                }
            });
        }
        #endregion



        #region  Submit
        private async void btnSubmit_Click(object sender, EventArgs e)
        {

            if (!IsHasSelected())
            {
                MessageBox.Show("sry.please select at least one row", "Warning Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("comfire submit ？", "submit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await SubmitData();
            }



        }


        async Task SubmitData()
        {
            //send fail count;
            int sendExceptionCount = 0, resposeFailCount = 0, submitData = 0;
            List<Task<HttpResponseMessage>> taskHttpRequestMsgList = new();
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                _ = bool.TryParse(row.Cells["IsChecked"].EditedFormattedValue.ToString(), out bool isChecked);
                if (!isChecked)
                {
                    continue;
                }
                submitData++;

                string postParam = CreatePostParam(new Dictionary<string, string>
                {
                    { "name","zhangsan"},
                    { "message_id",row.Cells["message_id"].Value.ToString()},
                    { "logistics_interface",row.Cells["logistics_interface"].Value.ToString()},
                    { "data_digest",row.Cells["data_digest"].Value.ToString()},
                    { "partner_code",row.Cells["partner_code"].Value.ToString()},
                    { "from_code",row.Cells["from_code"].Value.ToString()},
                    { "msg_type",row.Cells["msg_type"].Value.ToString()},
                    { "msg_id",row.Cells["msg_id"].Value.ToString()},
                    { "create_date",row.Cells["create_date"].Value.ToString()},
                }, true);

                taskHttpRequestMsgList.Add(GetResponseMessageAysnc(row.Cells["msg_id"].Value.ToString(), postParam, sendExceptionCount));
            }

            var responseMessagesList = await Task.WhenAll(taskHttpRequestMsgList);
            foreach (var responseMessage in responseMessagesList.Where(o => o != null))
            {
                string responseMsg = await responseMessage.Content.ReadAsStringAsync();
                if (!responseMessage.IsSuccessStatusCode)
                {
                    resposeFailCount++;
                    logger.Info("sending data success.but response fail");
                    logger.Warn($"the response data is 【{responseMsg}】");
                    continue;
                }
                logger.Info($"sending data success.the response data is {responseMsg}");
            }

            if (sendExceptionCount > 0 || resposeFailCount > 0)
            {
                MessageBox.Show($"sry.the post data happen error count is {sendExceptionCount}." +
                                $"the  response not success count is {resposeFailCount}" +
                                $"the response data success count is {submitData - (sendExceptionCount + resposeFailCount)}"
                    , "Warning Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (submitData - (sendExceptionCount + resposeFailCount) == submitData)
            {
                MessageBox.Show($"congratulations.all data submit success."
                   , "Success Msg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("no data is sending or response.");
        }

        async Task<HttpResponseMessage> GetResponseMessageAysnc(string mes_id, string param, int sendExceptionCount)
        {

            try
            {
                HttpContent httpContent = new StringContent(param, Encoding.UTF8, "application/x-www-form-urlencoded");
                logger.Info($"post data 【{mes_id}】 is sending.");
                return await httpClient.PostAsync($"{url}", httpContent);
            }
            catch (Exception ex)
            {
                sendExceptionCount++;
                logger.Info($"post data 【{mes_id}】 happen exception.");
                logger.Error($"{ex.Message}");
                return null;
            }
        }


        bool IsHasSelected()
        {
            bool flag = false;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                bool.TryParse(row.Cells["IsChecked"].EditedFormattedValue.ToString(), out bool isChecked);
                if (isChecked)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        #endregion

        /// <summary>
        /// create post param
        /// </summary>
        /// <param name="PostParameters"></param>
        /// <param name="IsEncode"></param>
        /// <returns></returns>
        string CreatePostParam(Dictionary<string, string> postParameters, bool isEncode = false)
        {
            var query = from s in postParameters select s.Key + "=" + (isEncode ? System.Web.HttpUtility.UrlEncode(s.Value) : s.Value);
            string[] parameters = query.ToArray();
            return string.Join("&", parameters);
        }

    }
}
