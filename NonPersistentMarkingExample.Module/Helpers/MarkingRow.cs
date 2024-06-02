using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NonPersistentMarkingExample.Module.Helpers
{
    [DomainComponent]
    [XafDefaultProperty(nameof(Code))]
    public class MarkingRow<TOwner> : NonPersistentBaseObject
        , IAssignable<MarkingRow<TOwner>>
        where TOwner : MarkingObjectBase<TOwner>, new()

    {
        [Browsable(false)]
        [Key]
        [DevExpress.ExpressApp.Data.Key]
        [JsonIgnore]
        public virtual long RowKey => ((long)OwnerId << 32) + RowId;

        [Browsable(false)]
        [JsonIgnore]
        public virtual int RowId { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public virtual int OwnerId { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public virtual TOwner Owner { get; set; }

        [MaxLength(4)]
        public virtual string Code { get; set; }
        public virtual string Value { get; set; }


        void IAssignable<MarkingRow<TOwner>>.Assign(MarkingRow<TOwner> source)
        {
            RowId = source.RowId;
            OwnerId = source.OwnerId;
            Code = source.Code;
            Value = source.Value;
        }
    }
}