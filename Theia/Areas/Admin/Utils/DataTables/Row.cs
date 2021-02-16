namespace Theia.Areas.Admin.Utils.DataTables
{
    public abstract class Row
    {
        public virtual string DT_RowId => null;
        public virtual string DT_RowClass => null;
        public virtual object DT_RowData => null;
    }
    
}
