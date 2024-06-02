using DevExpress.Persistent.Base;
using NonPersistentMarkingExample.Module.Helpers;
using System.ComponentModel.DataAnnotations;

namespace NonPersistentMarkingExample.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Product : MarkingObjectBase<Product>
    {
        public virtual string Name { get; set; }

        [MaxLength(14)]
        public virtual string GTIN { get; set; }
    }
}
