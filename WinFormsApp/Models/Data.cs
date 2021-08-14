using System.Collections.Generic; 

namespace WinFormsApp.Models
{
    public class Data
    {
        public Data()
        {
            Records = new List<Record>();
        }

        public List<Record> Records { get; set; }
    }
}
