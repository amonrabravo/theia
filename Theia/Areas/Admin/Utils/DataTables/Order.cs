namespace Theia.Areas.Admin.Utils.DataTables
{
    public enum DTOrderDir
    {
        ASC,
        DESC
    }

    public class Order
    {
        public int Column { get; set; }

        public DTOrderDir Dir { get; set; }
    }

}
