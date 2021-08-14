namespace WinFormsApp.Models
{
    public class Record
    {
        /// <summary>
        /// order no
        /// </summary>
        public string msg_id { get; set; }
        public string msg_type { get; set; }

        public string message_id { get; set; }

        public string logistics_interface { get; set; }

        public string partner_code { get; set; }

        public string from_code { get; set; }

        public string data_digest { get; set; }

        public string create_date { get; set; }
    }
}
