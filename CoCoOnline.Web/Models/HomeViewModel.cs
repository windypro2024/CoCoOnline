using CoCoOnline.Models;
using Microsoft.AspNetCore.Html;

namespace CoCoOnline.Web.Models
{
    public class HomeViewModel
    {
        public Books? LastBook { get; set; }
        public IEnumerable<Books> NewBooks { get; set; }
        public IEnumerable<Books> OldBooks { get; set; }
        public IEnumerable<Books> ClickBooks { get; set; }
        public IEnumerable<Publishers> Publishers { get; set; }
        public IEnumerable<Categories> Categories { get; set; }
    }
}
