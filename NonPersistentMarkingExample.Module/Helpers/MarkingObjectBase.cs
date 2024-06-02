using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NonPersistentMarkingExample.Module.Helpers
{
    public abstract class MarkingObjectBase<TOwner> : IXafEntityObject
        where TOwner : MarkingObjectBase<TOwner>, new()
    {
        private readonly ObservableCollection<MarkingRow<TOwner>> MarkingRowsCollection = [];
        protected MarkingObjectBase()
        {
            MarkingRowsCollection.CollectionChanged += MarkingRowsCollection_CollectionChanged;
        }


        [Browsable(false)]
        public virtual int Id { get; set; }

        [MaxLength(4500)]
        public virtual string Marking { get; set; }

        [Aggregated]
        [NotMapped]
        public virtual IList<MarkingRow<TOwner>> MarkingRows => MarkingRowsCollection;



        private void MarkingRowsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    var obj = (MarkingRow<TOwner>)e.NewItems[i];
                    obj.OwnerId = Id;
                    obj.RowId = i + 1;
                }
            }
        }


        void IXafEntityObject.OnCreated() { }
        void IXafEntityObject.OnLoaded()
        {
            if (SerializationHelper.TryDeserialize(Marking, out List<MarkingRow<TOwner>> list))
            {
                foreach (var row in list)
                {
                    MarkingRowsCollection.Add(row);
                }
            }
        }

        void IXafEntityObject.OnSaving()
        {
            Marking = MarkingRows.Count > 0 ? SerializationHelper.Serialize(MarkingRowsCollection) : null;
        }
    }
}
