using DevExpress.ExpressApp;
using System.ComponentModel;

namespace NonPersistentMarkingExample.Module.Helpers
{
    public interface IAssignable<T>
    {
        void Assign(T source);
    }

    public abstract class NonPersistentObjectAdapter<TObject, TKey>
    {
        private readonly NonPersistentObjectSpace _objectSpace;
        private readonly Dictionary<TKey, TObject> objectMap;
        protected NonPersistentObjectSpace ObjectSpace { get { return _objectSpace; } }

        public NonPersistentObjectAdapter(NonPersistentObjectSpace npos)
        {
            this._objectSpace = npos;
            _objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            _objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            _objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            _objectSpace.Reloaded += ObjectSpace_Reloaded;
            _objectSpace.ObjectReloading += ObjectSpace_ObjectReloading;
            _objectSpace.CustomCommitChanges += ObjectSpace_CustomCommitChanges;
            objectMap = new Dictionary<TKey, TObject>();
        }

        protected virtual void GuardKeyNotEmpty(TObject obj)
        {
            if (Object.Equals(GetKeyValue(obj), default(TKey)))
                throw new InvalidOperationException(); // DEBUG
        }

        protected virtual TKey GetKeyValue(TObject obj)
        {
            return (TKey)_objectSpace.GetKeyValue(obj);
        }

        private void AcceptObject(TObject obj)
        {
            GuardKeyNotEmpty(obj);
            if (!objectMap.TryGetValue(GetKeyValue(obj), out TObject result))
            {
                objectMap.Add(GetKeyValue(obj), obj);
            }
            else
            {
                if (!Object.Equals(result, obj))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        protected TObject GetObjectByKey(TKey key)
        {
            if (!objectMap.TryGetValue(key, out TObject result))
            {
                result = LoadObjectByKey(key);
                if (result != null)
                {
                    AcceptObject(result);
                }
            }
            return result;
        }

        protected virtual TObject LoadObjectByKey(TKey key)
        {
            throw new NotSupportedException();
        }

        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e)
        {
            if (e.Key != null)
            {
                if (e.ObjectType == typeof(TObject))
                {
                    e.Object = GetObjectByKey((TKey)e.Key);
                }
            }
        }

        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e)
        {
            if (e.ObjectType == typeof(TObject))
            {
                var objects = GetObjects();
                e.Objects = (System.Collections.IList)objects;
            }
        }

        protected virtual IList<TObject> GetObjects()
        {
            throw new NotSupportedException();
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            objectMap.Clear();
        }

        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e)
        {
            if (e.SourceObject is TObject obj)
            {
                if (e.SourceObject is IObjectSpaceLink link)
                {
                    GuardKeyNotEmpty(obj);
                    if (link.ObjectSpace == null)
                    {
                        e.TargetObject = AcceptOrUpdate(obj);
                    }
                    else
                    {
                        if (link.ObjectSpace.IsNewObject(obj))
                        {
                            if (link.ObjectSpace.Equals(_objectSpace))
                            {
                                e.TargetObject = e.SourceObject;
                            }
                            else
                            {
                                e.TargetObject = null;
                            }
                        }
                        else
                        {
                            if (link.ObjectSpace.Equals(_objectSpace))
                            {
                                e.TargetObject = AcceptOrUpdate(obj);
                            }
                            else
                            {
                                if (!objectMap.TryGetValue(GetKeyValue(obj), out TObject result))
                                {
                                    result = LoadSameObject(obj);
                                    if (result != null)
                                    {
                                        AcceptObject(result);
                                    }
                                }
                                e.TargetObject = result;
                            }
                        }
                    }
                }
            }
        }

        protected virtual bool ThrowOnAcceptingMismatchedObject { get { return false; } }

        private TObject AcceptOrUpdate(TObject obj)
        {
            var key = GetKeyValue(obj);
            if (!objectMap.TryGetValue(key, out TObject result))
            {
                objectMap.Add(key, obj);
                result = obj;
            }
            else
            {
                // if objectMap contains an object with the same key, assume SourceObject is a reloaded copy.
                // then refresh contents of the found object and return it.
                if (!Object.Equals(result, obj))
                {
                    if (ThrowOnAcceptingMismatchedObject)
                        throw new InvalidOperationException();
                    if (result is IAssignable<TObject> a)
                    {
                        a.Assign(obj);
                    }
                }
            }
            return result;
        }

        protected virtual TObject LoadSameObject(TObject obj)
        {
            return LoadObjectByKey(GetKeyValue(obj));
        }

        private void ObjectSpace_ObjectReloading(object sender, ObjectGettingEventArgs e)
        {
            if (e.SourceObject is TObject tobj)
            {
                e.TargetObject = ReloadObject(tobj);
            }
        }

        protected virtual TObject ReloadObject(TObject obj)
        {
            return obj;
        }

        private void ObjectSpace_CustomCommitChanges(object sender, HandledEventArgs e)
        {
            var list = new List<TObject>();
            foreach (var obj in _objectSpace.ModifiedObjects)
            {
                if (obj is TObject @object)
                {
                    list.Add(@object);
                }
            }
            if (list.Count > 0)
            {
                CommitChanges(list);
            }
        }

        protected virtual void CommitChanges(List<TObject> objects)
        {
        }
    }

}
