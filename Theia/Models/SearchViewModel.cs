using System.Text.RegularExpressions;

namespace Theia.Models
{
    public class SearchViewModel
    {
        public int? CategoryId { get; set; }
        public string Keyword { get; set; }

        public string[] Keywords => Regex.Split(Keyword?.ToLower() ?? "", @"\s+");
    }
}
