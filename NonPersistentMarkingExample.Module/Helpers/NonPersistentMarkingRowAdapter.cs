using DevExpress.ExpressApp;

namespace NonPersistentMarkingExample.Module.Helpers
{
    public class NonPersistentMarkingRowAdapter<TOwner> : NonPersistentObjectAdapter<MarkingRow<TOwner>, long>
        where TOwner : MarkingObjectBase<TOwner>, new()
    {
        public NonPersistentMarkingRowAdapter(NonPersistentObjectSpace npos) : base(npos) { }

        protected override void GuardKeyNotEmpty(MarkingRow<TOwner> obj)
        {
            base.GuardKeyNotEmpty(obj);
        }

        protected override MarkingRow<TOwner> LoadObjectByKey(long key)
        {
            var ownerObjectSpace = ObjectSpace.Owner as CompositeObjectSpace;

            int ownerKey = (int)((key & 0x7FFF_FFFF_0000_0000) >> 32);

            var owner = GetOwnerByKey(ownerObjectSpace, ownerKey);
            var result = GetObjectFromOwner(owner, key);

            if (result == null)
            {
                owner = ReloadOwner(ownerObjectSpace, owner);
                result = GetObjectFromOwner(owner, key);
            }

            return result;
        }

        private TOwner ReloadOwner(CompositeObjectSpace ownerObjectSpace, TOwner owner)
        {
            if (ownerObjectSpace.ModifiedObjects.Contains(owner))
                throw new NotSupportedException();
            return (TOwner)(ownerObjectSpace ?? ObjectSpace).ReloadObject(owner);
        }

        private TOwner GetOwnerByKey(CompositeObjectSpace os, int ownerKey)
        {
            return (os ?? ObjectSpace).GetObjectByKey<TOwner>(ownerKey);
        }

        private static MarkingRow<TOwner> GetObjectFromOwner(TOwner owner, long key)
        {
            return owner.MarkingRows.FirstOrDefault(x => x.RowKey == key);
        }

    }
}
