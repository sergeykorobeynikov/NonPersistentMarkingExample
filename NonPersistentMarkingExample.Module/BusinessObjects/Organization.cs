using DevExpress.Persistent.Base;
using NonPersistentMarkingExample.Module.Helpers;
using System.Collections.ObjectModel;

namespace NonPersistentMarkingExample.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Organization:MarkingObjectBase<Organization>
    {
        public virtual string Name { get; set; }
        public virtual string TaxId {  get; set; }

        public virtual IList<Warehouse> Warehouses { get; set; } = new ObservableCollection<Warehouse>();
    }
}
