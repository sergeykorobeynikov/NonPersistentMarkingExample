using DevExpress.Pdf.Native;
using DevExpress.Persistent.Base;
using NonPersistentMarkingExample.Module.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonPersistentMarkingExample.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Warehouse : MarkingObjectBase<Warehouse>
    {
        [Browsable(false)]
        public virtual int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        public virtual string Name { get; set; }
        public virtual string GLN { get; set; }
    }
}
