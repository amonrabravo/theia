namespace Theia.Areas.Admin.Utils.DataTables
{
    public class Parameters
    {
        public int Draw { get; set; }
        public Column[] Columns { get; set; }
        public Order[] Order { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; }
        public string SortOrder
        {
            get
            {
                return Columns != null && Order != null && Order.Length > 0
                    ? (Columns[Order[0].Column].Data + (Order[0].Dir == OrderDir.DESC ? " " + Order[0].Dir : string.Empty))
                    : null;
            }
        }
    }

}
