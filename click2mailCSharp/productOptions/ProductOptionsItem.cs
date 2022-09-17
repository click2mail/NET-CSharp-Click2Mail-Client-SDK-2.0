using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2mAPI
{
    public class ProductOptionsItem
    {
        public string DocumentClass { get; set; } = "";
        public string Layout { get; set; } = "";
        public string ProductionTime { get; set; } = "";
        public string Envelope { get; set; } = "";
        public string PrintColor { get; set; } = "";
        public string PaperType { get; set; } = "";
        public string PrintOption { get; set; } = "";
        public string MailClass { get; set; } = "";
        public ProductOptionsItem(string documentClass, string layout, string productionTime, string envelope, string printColor, string paperType, string printOption, string mailClass)
        {
            DocumentClass = documentClass;
            Layout = layout;
            ProductionTime = productionTime;
            Envelope = envelope;
            PrintColor = printColor;
            PaperType = paperType;
            PrintOption = printOption;
            MailClass = mailClass;
        }
        public ProductOptionsItem()
        {
        }
    }
}
