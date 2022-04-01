using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    internal class Links
    {
        public string Title;
        public string HtmlLink;

        public Links(string _title, string _htmlLink)
        {
            this.Title = _title;
            this.HtmlLink = _htmlLink;
        }

        public string GetTitle()
        {
            return this.Title;
        }

        public string GetHtmlLink()
        {
            return this.HtmlLink;
        }
    }
}
