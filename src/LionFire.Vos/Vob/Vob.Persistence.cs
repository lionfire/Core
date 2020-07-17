using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public partial class Vob
    {

#if TOPORT
        #region Retrieve

        //public Mount GetMountForHandle(H handleParam) OLD Unneeded
        //{
        //    if (handleParam == null) return null;

        //    if(handleParam.Mount != null) return handleParam.Mount;

        //    foreach (var kvp in ReadHandlesWithMounts)
        //    {
        //        var handle = kvp.Key;
        //        if (handleParam.Equals(handle))
        //        {
        //            //l.LogCritical("== - " + handle + " " + handleParam);
        //            return kvp.Value;
        //        }
        //        else
        //        {
        //            //l.Warn("!= - " + handle + " " + handleParam);
        //        }
        //    }
        //    return null;
        //}

        //public bool TryRetrieve(bool setToNullOnFail = true)
        //{
        //    object result = null;

        //    foreach (var kvp in ReadHandlesWithMounts)
        //    {
        //        var handle = kvp.Key;
        //        if (handle.TryRetrieve())
        //        {
        //            result = handle.Object;

        //            this.UnitypeHandle = handle;
        //            this.ObjectHandleMount = kvp.Value;

        //            this.OnRetrieved(result);

        //            return true;
        //        }
        //    }

        //    if (setToNullOnFail) SetObjectToNull();
        //    //OnRetrievedNothing();
        //    return false;
        //}

        //public object TryRetrieve(VobReference reference)
        //{
        //    if (reference.Path != this.Path)
        //    {
        //        throw new ArgumentException("Path invalid for this vob");
        //    }
        //    if (reference.Type != null)
        //    {
        //        return TryRetrieve(type: reference.Type, reference.Package );
        //    }
        //    else
        //    {
        //        return TryRetrieveByTypeName(typeName: reference.TypeName, reference.Package);
        //    }
        //}

        //public object TryRetrieveByTypeName(string typeName = null, string location = null, )
        //{
        //    Type type = Type.GetType(typeName, true);
        //    return TryRetrieve(type: type, location: location, );
        //}

        protected void OnRetrieved(object retrievedObject)
        {
            //base.OnRetrieved(retrievedObject);

            if (retrievedObject != null)
            {
                //this.Type = retrievedObject.GetType();
            }
        }

        //protected void OnRetrievedNothing()
        //{
        //}

        //private void SetObjectToNull()
        //{
        //    this._object = null;
        //    this.UnitypeHandle = null;
        //    this.ObjectHandleMount = null;
        //    //objectHandle = null;

        //    //objectHandles = null;
        //}

        
        public IEnumerable<R> AllRetrievableReadHandles
        {
            get
            {
                foreach (var handle in ReadHandles)
                {
                    if (!handle.TryEnsureRetrieved())
                    {
                        continue;
                    }

                    yield return handle;
                }
            }
        }
        
#if !AOT
        // FUTURE ENH: Return H<T>'s? or VobHandle<T>'s, with T based on detected type of the object?
        public IEnumerable<H> AllHandlesOfType<T>()
            where T : class
        {
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                T obj = handle.Object as T;
                if (obj != null)
                {
                    yield return handle;
                }

                IReadOnlyMultiTyped mt = obj as IReadOnlyMultiTyped;
                obj = mt.AsType<T>();
                if (obj != null)
                {
                    yield return handle;
                }
            }
        }

        internal RetrieveType TryRetrieve<RetrieveType>(VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class => TryEnsureRetrieved_(true, vosLogicalHandle);

        internal RetrieveType TryEnsureRetrieved<RetrieveType>(VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class => TryEnsureRetrieved_(false, vosLogicalHandle);

#if FUTURE // Get read handle
        private IVobHandle TryGetReadHandle<RetrieveType>(bool reload, VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class
        {
            IVobHandle resultHandle;
            object resultObj;
            RetrieveType result = null;

        #region UnitypeHandle

            if (UnitypeHandle != null)
            {
                if (!reload)
                {
                    //UnitypeHandle.Retrieve();

                    object uniTypeObj = UnitypeHandle.HasObject ? UnitypeHandle.Object : null;

                    result = uniTypeObj as RetrieveType;
                    if (result != null) return UnitypeHandle;
                    else
                    {
                        if (uniTypeObj != null)
                        {
                            l.Trace("UNTESTED - UnitypeHandle has object of different type (" + uniTypeObj.GetType().Name + "), but type " + typeof(RetrieveType).Name + " was requested.");
                        }
                    }
                }
            }

        #endregion


            //    //return (RetrieveType)TryRetrieve(type: typeof(RetrieveType), location: vh.Location, mount: vh.Mount);
            //    return (RetrieveType)TryRetrieve(typeof(RetrieveType), vh);
            //}

            //public object TryRetrieve<RetrieveType>(Type type = null,
            //    //string location = null, Mount mount = null, 
            //    VobHandle<RetrieveType> vosLogicalHandle = null)
            //    where RetrieveType : class
            //{
            string location = vosLogicalHandle?.Store;
            Mount mount = vosLogicalHandle?.Mount;

#if SanityChecks
            if (vosLogicalHandle?.Path != this.Path)
            {
                throw new UnreachableCodeException("vh.Path != this.Path: " + vosLogicalHandle.Path + " != " + this.Path);
            }
#endif


            //if (layer == null && type == null) -- what was this for?  infiniteloop
            //{
            //    TryRetrieve();
            //    return _object;
            //}

            // REVIEW: Yikes lots of branches

            try
            {
                if (location != null)
                {
                    if (mount == null)
                    {
                        mount = effectiveMountsByName.TryGetValue(location);

                    }
                    else
                    {
                        // TOVALIDATE - verify mount matches location?
                        if (mount.Store != location)
                        {
                            l.Warn("VALIDATION fail: mount.LayerName != location --> " + mount.Store + " != " + location);
                        }
                    }
                    if (mount == null)
                    {
                        result = null;
                        return null;
                    }

                    H<RetrieveType> handle = GetMountHandle<RetrieveType>(mount);

                    //if (type == null) Identical to block below. OLD DELETE
                    //{
                    //    if (handle.TryEnsureRetrieved())
                    //    {
                    //        System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                    //        resultObj = handle.Object;
                    //        result = resultObj as RetrieveType;
                    //        if (resultObj != null && result == null)
                    //        {
                    //            lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                    //        }
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        result = null;
                    //        return result;
                    //    }
                    //}
                    //else
                    {
                        // FUTURE: RetrieveAsType? multitype stuff?
                        if (reload ? handle.TryRetrieve() : handle.TryEnsureRetrieved())
                        {
                            System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                            resultObj = handle.Object;
                            result = resultObj as RetrieveType;
                            if (resultObj != null && result == null)
                            {
                                lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                            }
                            return result;
                            //if (resultObj==null || typeof(RetrieveType).IsAssignableFrom(resultObj.GetType()))
                            //{
                            //    return resultObj;
                            //}
                            //else
                            //{
                            //    result = null;
                            //    return resultObj;
                            //}
                        }
                        else
                        {
                            result = null;
                            return result;
                        }
                    }
                }
                else // location == null
                {
                    //if (this.IsMultiTypeObjectMerged) // Never set!
                    //{
                    //    l.Debug("UNTESTED REVIEW IsMultiTypeObjectMerged - may not make sense");
                    //    var results = TryEnsureRetrievedAllLayers().OfType<RetrieveType>();
                    //    if (results == null || !results.Any())
                    //    {
                    //        result = null;
                    //        return result;
                    //    }

                    //    if (results.ElementAtOrDefault(2) != null) // if more than 1, create a multitype
                    //    {
                    //        #region Not Implemented: Same types:

                    //        List<Type> types = new List<Type>();
                    //        foreach (var result in results)
                    //        {
                    //            if (types.Contains(result.GetType())) throw new NotImplementedException("Vobs with multiple results of same type");
                    //            types.Add(result.GetType());
                    //        }

                    //        #endregion

                    //        MultiType mt = new MultiType(results);
                    //        // TODO: Set objectHandles; (doh - need to get from TryRetrieveAllLayers?)
                    //        resultObj = mt;
                    //        result = mt.AsType<RetrieveType>();
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        resultObj = results.ElementAt(0);
                    //        return resultObj;
                    //    }
                    //}
                    //else // Return the first object in the Vos Read Stack
                    {
                        H<RetrieveType> handle = null; // FUTURE: get hint from VobHandle.TargetHandle?

                        if (mount != null) handle = GetMountHandle<RetrieveType>(mount);

        #region If mount is already known, try it first

                        if (handle != null && mount != null)
                        {
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload);
                            if (result != null)
                            {
                                // REVIEW - allow mount hints? make this an option?  Downside is that desired mount may change, and hint would not be updated

                                //l.Trace("Retrieved obj from mount hint: " + mount);
                                return result;
                            }
                        }
        #endregion

                        //IMergeable mergeable = null;

        #region Get the first hit on the read stack

                        foreach (var readMount in ReadHandleMounts)
                        {
                            handle = GetMountHandle<RetrieveType>(readMount);
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload);
                            if (result != null)
                            {
                                return result;
                            }
                        }

        #endregion

                        return null;
                    }
                }
            }
            finally
            {
                if (result != null && UnitypeHandle == null)
                {
                    UnitypeHandle = vosLogicalHandle;
                }
            }
        }
#endif

        // REVIEW - this is too complicated?
        private RetrieveType TryEnsureRetrieved_<RetrieveType>(bool reload, VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class
        {
            object resultObj;
            RetrieveType result = null;

#if VosUnitype
            if (UnitypeHandle != null)
            {
                if (!reload)
                {
                    //UnitypeHandle.Retrieve();

                    object uniTypeObj = UnitypeHandle.HasObject ? UnitypeHandle.Object : null;

                    result = uniTypeObj as RetrieveType;
                    if (result != null) return result;
                    else
                    {
                        if (uniTypeObj != null)
                        {
                            l.Trace("UNTESTED - UnitypeHandle has object of different type (" + uniTypeObj.GetType().Name + "), but type " + typeof(RetrieveType).Name + " was requested.");
                        }
                    }
                }
            }
#endif

            //    //return (RetrieveType)TryRetrieve(type: typeof(RetrieveType), location: vh.Location, mount: vh.Mount);
            //    return (RetrieveType)TryRetrieve(typeof(RetrieveType), vh);
            //}

            //public object TryRetrieve<RetrieveType>(Type type = null,
            //    //string location = null, Mount mount = null, 
            //    VobHandle<RetrieveType> vosLogicalHandle = null)
            //    where RetrieveType : class
            //{
            string location = vosLogicalHandle.Store;
            Mount mount = vosLogicalHandle.Mount;

#if SanityChecks
            if (vosLogicalHandle.Path != this.Path)
            {
                throw new UnreachableCodeException("vh.Path != this.Path: " + vosLogicalHandle.Path + " != " + this.Path);
            }
#endif

            //if (layer == null && type == null) -- what was this for?  infiniteloop
            //{
            //    TryRetrieve();
            //    return _object;
            //}

            // REVIEW: Yikes lots of branches
            H<RetrieveType> handle = null;
            Mount readFromMount = null;
            try
            {
                if (location != null)
                {
                    if (mount == null)
                    {
                        mount = effectiveMountsByName.TryGetValue(location);
                    }
                    else
                    {
                        // TOVALIDATE - verify mount matches location?
                        if (mount.Store != location)
                        {
                            l.Warn("VALIDATION fail: mount.LayerName != location --> " + mount.Store + " != " + location);
                        }
                    }
                    if (mount == null) // No mount for specified location
                    {
                        l.Trace($"Retrieve specified location '{location}' but no mount was found at {Path}");
                        result = null;
                        goto end;
                    }

                    handle = GetHandleFromMount<RetrieveType>(mount);

                    //if (type == null) Identical to block below. OLD DELETE
                    //{
                    //    if (handle.TryEnsureRetrieved())
                    //    {
                    //        System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                    //        resultObj = handle.Object;
                    //        result = resultObj as RetrieveType;
                    //        if (resultObj != null && result == null)
                    //        {
                    //            lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                    //        }
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        result = null;
                    //        return result;
                    //    }
                    //}
                    //else
                    {
                        // FUTURE: RetrieveAsType? multitype stuff?
                        if (reload ? handle.TryRetrieve() : handle.TryEnsureRetrieved())
                        {

                            System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                            resultObj = handle.Object;
                            result = resultObj as RetrieveType;
                            if (resultObj != null && result == null)
                            {
                                lLoad.Warn($"Specified location found object of type {resultObj.GetType().Name} but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                            }
                            readFromMount = mount;
                            goto end;
                            //if (resultObj==null || typeof(RetrieveType).IsAssignableFrom(resultObj.GetType()))
                            //{
                            //    return resultObj;
                            //}
                            //else
                            //{
                            //    result = null;
                            //    return resultObj;
                            //}
                        }
                        else
                        {
                            result = null;
                            goto end;
                        }
                    }
                }
                else // location == null
                {
                    //if (this.IsMultiTypeObjectMerged) // Never set!
                    //{
                    //    l.Debug("UNTESTED REVIEW IsMultiTypeObjectMerged - may not make sense");
                    //    var results = TryEnsureRetrievedAllLayers().OfType<RetrieveType>();
                    //    if (results == null || !results.Any())
                    //    {
                    //        result = null;
                    //        return result;
                    //    }

                    //    if (results.ElementAtOrDefault(2) != null) // if more than 1, create a multitype
                    //    {
                    //        #region Not Implemented: Same types:

                    //        List<Type> types = new List<Type>();
                    //        foreach (var result in results)
                    //        {
                    //            if (types.Contains(result.GetType())) throw new NotImplementedException("Vobs with multiple results of same type");
                    //            types.Add(result.GetType());
                    //        }

                    //        #endregion

                    //        MultiType mt = new MultiType(results);
                    //        // TODO: Set objectHandles; (doh - need to get from TryRetrieveAllLayers?)
                    //        resultObj = mt;
                    //        result = mt.AsType<RetrieveType>();
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        resultObj = results.ElementAt(0);
                    //        return resultObj;
                    //    }
                    //}
                    //else // Return the first object in the Vos Read Stack
                    {
                        handle = null; // FUTURE: get hint from VobHandle.TargetHandle?

                        if (mount != null) { handle = GetHandleFromMount<RetrieveType>(mount); }

        #region If mount is already known, try it first

                        if (handle != null && mount != null)
                        {
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload);
                            if (result != null)
                            {
                                readFromMount = mount;
                                // REVIEW - allow mount hints? make this an option?  Downside is that desired mount may change, and hint would not be updated

                                l.Trace($"Retrieved obj of type '{typeof(RetrieveType).Name}' from mount hint: '{mount}' at {Path}");
                                goto end;
                            }
                        }
        #endregion

                        //IMergeable mergeable = null;

        #region Get the first hit on the read stack

                        foreach (var readMount in ReadHandleMounts)
                        {
                            handle = GetHandleFromMount<RetrieveType>(readMount);
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload); // REVIEW - FIXME should this mount be readMount?
                            if (result != null)
                            {
                                readFromMount = readMount;
                                goto end;
                            }
                        }

        #endregion

                        result = null;
                        goto end;
                    }
                }
            }
            finally
            {
#if VosUnitype
                if (result != null && UnitypeHandle == null)
                {
                    UnitypeHandle = vosLogicalHandle;
                }
#endif
            }
            end:
            if (result != null && vosLogicalHandle.VosRetrieveInfo != null)
            {
                vosLogicalHandle.VosRetrieveInfo.Mount = readFromMount;
                vosLogicalHandle.VosRetrieveInfo.ReadHandle = handle;
            }
            return result;
        }
#else
        internal object TryRetrieve(IVobHandle vosLogicalHandle)
		{
			Type retrieveType = vosLogicalHandle.Type;
			object resultObj;
			object result = null;
			
			if (UnitypeHandle != null)
			{
				object uniTypeObj = UnitypeHandle.HasObject ? UnitypeHandle.Object : null;

				if(uniTypeObj != null && uniTypeObj.GetType() == retrieveType)
				{
					return uniTypeObj;
				}
//				result = uniTypeObj as RetrieveType;
//				if (result != null) return result;
				else
				{
					if (uniTypeObj != null)
					{
						if(retrieveType != typeof(object) && !retrieveType.IsInterface ) // TEMP - avoid this?  - TODO REVIEW
						{
							l.Trace("UNTESTED - UnitypeHandle has object of different type (" + uniTypeObj.GetType().Name + "), but type " + retrieveType.Name + " was requested.");
						}
					}
				}
			}
			
			//    //return (RetrieveType)TryRetrieve(type: typeof(RetrieveType), location: vh.Location, mount: vh.Mount);
			//    return (RetrieveType)TryRetrieve(typeof(RetrieveType), vh);
			//}
			
			//public object TryRetrieve<RetrieveType>(Type type = null,
			//    //string location = null, Mount mount = null, 
			//    VobHandle<RetrieveType> vosLogicalHandle = null)
			//    where RetrieveType : class
			//{
			string location = vosLogicalHandle.Store;
			Mount mount = vosLogicalHandle.Mount;
			
#if SanityChecks
			if (vosLogicalHandle.Path != this.Path)
			{
				throw new UnreachableCodeException("vh.Path != this.Path: " + vosLogicalHandle.Path + " != " + this.Path);
			}
#endif
			
			
			//if (layer == null && type == null) -- what was this for?  infiniteloop
			//{
			//    TryRetrieve();
			//    return _object;
			//}
			
			// REVIEW: Yikes lots of branches
			
			try
			{
				if (location != null)
				{
					if (mount == null)
					{
						mount = 
#if AOT
								(Mount)
#endif
								effectiveMountsByName.TryGetValue(location);
						
					}
					else
					{
						// TOVALIDATE - verify mount matches location?
						if (mount.Store != location)
						{
							l.Warn("VALIDATION fail: mount.LayerName != location --> " + mount.Store + " != " + location);
						}
					}
					if (mount == null)
					{
						result = null;
						return result;
					}
					
					H handle = GetMountHandle(mount);
					
					//if (type == null) Identical to block below. OLD DELETE
					//{
					//    if (handle.TryEnsureRetrieved())
					//    {
					//        System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
					//        resultObj = handle.Object;
					//        result = resultObj as RetrieveType;
					//        if (resultObj != null && result == null)
					//        {
					//            lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
					//        }
					//        return result;
					//    }
					//    else
					//    {
					//        result = null;
					//        return result;
					//    }
					//}
					//else
					{
						// FUTURE: RetrieveAsType? multitype stuff?
						if (handle.TryEnsureRetrieved())
						{
							System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
							resultObj = handle.Object;
							if (resultObj != null && resultObj.GetType() != retrieveType)
							{
								lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + retrieveType.Name);
							}
							return result;
							//if (resultObj==null || typeof(RetrieveType).IsAssignableFrom(resultObj.GetType()))
							//{
							//    return resultObj;
							//}
							//else
							//{
							//    result = null;
							//    return resultObj;
							//}
						}
						else
						{
							result = null;
							return result;
						}
					}
				}
				else // location == null
				{
					//if (this.IsMultiTypeObjectMerged) // Never set!
					//{
					//    l.Debug("UNTESTED REVIEW IsMultiTypeObjectMerged - may not make sense");
					//    var results = TryEnsureRetrievedAllLayers().OfType<RetrieveType>();
					//    if (results == null || !results.Any())
					//    {
					//        result = null;
					//        return result;
					//    }
					
					//    if (results.ElementAtOrDefault(2) != null) // if more than 1, create a multitype
					//    {
					//        #region Not Implemented: Same types:
					
					//        List<Type> types = new List<Type>();
					//        foreach (var result in results)
					//        {
					//            if (types.Contains(result.GetType())) throw new NotImplementedException("Vobs with multiple results of same type");
					//            types.Add(result.GetType());
					//        }
					
					//        #endregion
					
					//        MultiType mt = new MultiType(results);
					//        // TODO: Set objectHandles; (doh - need to get from TryRetrieveAllLayers?)
					//        resultObj = mt;
					//        result = mt.AsType<RetrieveType>();
					//        return result;
					//    }
					//    else
					//    {
					//        resultObj = results.ElementAt(0);
					//        return resultObj;
					//    }
					//}
					//else // Return the first object in the Vos Read Stack
					{
						H handle = null; // FUTURE: get hint from VobHandle.TargetHandle?
						
						if (mount != null) handle = GetMountHandle(mount);
						
        #region If mount is already known, try it first
						
						if (handle != null && mount != null)
						{
							result = _TryRetrieve(handle, mount, vosLogicalHandle, retrieveType);
							if (result != null)
							{
								//l.Trace("Retrieved obj from mount hint: " + mount);
								return result;
							}
						}
        #endregion
						
						//IMergeable mergeable = null;
						
        #region Get the first hit on the read stack
						
						foreach (KeyValuePair<H, Mount> kvp in
#if AOT
 (IEnumerable)
#endif
                            ReadHandlesWithMounts)
						{
							result = _TryRetrieve(kvp.Key, kvp.Value, vosLogicalHandle, retrieveType);
							if (result != null)
							{
								return result;
							}
						}
						
        #endregion
						
						return null;
					}
				}
			}
			finally
			{
				if (result != null && UnitypeHandle == null)
				{
					UnitypeHandle = vosLogicalHandle;
				}
			}
		}
#endif

#if !AOT
        private ObjType _TryRetrieve<ObjType>(R<ObjType> handle, Mount mount, VobHandle<ObjType> vosLogicalHandle, bool reload)
            where ObjType : class
        {
            if (reload)
            {
                if (!handle.TryRetrieve(setToNullOnFail: true))
                {
                    return null;
                }
                //handle.ForgetObject(); // FUTURE?
                //if (!handle.TryEnsureRetrieved()) return null;
            }
            else
            {
                if (!handle.TryEnsureRetrieved())
                {
                    return null;
                }
            }

            //ObjType result = handle.Object as ObjType;
            ObjType result = handle.Object;

            if (result == null) // OLD UNREACHABLE 
            {
                l.Warn("UNREACHABLE Retrieved object of unexpected type for handle.  Expected: " + typeof(ObjType).Name + ".  Got: " + handle.Object.GetType().Name);
                return result;
            }

            if (vosLogicalHandle != null)
            {
                //l.LogCritical("TEMP Retrieve succeeded.  Setting VobHandle<>.Mount: " + vosLogicalHandle + " => " + mount);
                vosLogicalHandle.Mount = mount;
                // REVIEW - also store handle as a hint to go with mount, or instead of it?
            }

            //IVobHandle vh = handle as IVobHandle;
            //if (vh != null)
            //{
            //    l.Debug("vh != null in _TryRetrieve. setting mount.");
            //    vh.Mount = mount;
            //}
            //else
            //{
            //    l.Trace("vh == null in _TryRetrieve");
            //}

            //this.Object = handle.Object;
#if VosUnitype
            if (_unitypeHandle == null)
            {
                //UnitypeHandle = handle;
                UnitypeHandle = vosLogicalHandle;
            }
#endif
            //ObjectHandleMount = mount;

            OnRetrieved(result);
            return result;
        }
#else
        private object _TryRetrieve(R handle, Mount mount, IVobHandle vosLogicalHandle, Type objType)
//            where ObjType : class
        {
            if (!handle.TryEnsureRetrieved()) return null;

			object result = handle.Object ;
//			as ObjType;

            if (result != null && result.GetType() != objType)
            {
				if(objType != typeof(object))
				{
            	    l.Warn("Retrieved object of unexpected type for handle.  Expected: " + objType. Name + ".  Got: " + handle.Object.GetType().Name);
				}
//                return result;
            }

            if (vosLogicalHandle != null)
            {
                //l.LogCritical("TEMP Retrieve succeeded.  Setting VobHandle<>.Mount: " + vosLogicalHandle + " => " + mount);
                vosLogicalHandle.Mount = mount;
                // REVIEW - also store handle as a hint to go with mount, or instead of it?
            }

            //IVobHandle vh = handle as IVobHandle;
            //if (vh != null)
            //{
            //    l.Debug("vh != null in _TryRetrieve. setting mount.");
            //    vh.Mount = mount;
            //}
            //else
            //{
            //    l.Trace("vh == null in _TryRetrieve");
            //}

            //this.Object = handle.Object;
            if (_unitypeHandle == null)
            {
                //UnitypeHandle = handle;
                UnitypeHandle = vosLogicalHandle;
            }
            //ObjectHandleMount = mount;

            OnRetrieved(result);
            return result;
        }
#endif

        //public object TryRetrieve(VobReference reference, string[] pathChunks, int pathIndex)
        //{
        //    List<Mount> mounts = new List<Mount>();

        //    object obj=null;

        //    if(reference.Layer != null)
        //    {
        //        Mount mount = effectiveMountsByName.TryGetValue(reference.Layer);

        //        if(mount != null)
        //        {
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Layer not mounted: " + (reference.Layer ?? "null"));
        //        }
        //    }
        //    else
        //    {
        //    foreach(var kvp in effectiveMountsByReadPriority)
        //    {

        //        if(kvp.Value.Count > 1)
        //        {
        //            throw new NotImplementedException("Equal mount read priorities");
        //        }
        //        else if(kvp.Value.Count==1)
        //        {
        //            Mount mount = kvp.Value.First();


        //            mount.RootHandle.Reference.Path
        //        }
        //    }
        //    }

        //    Vob vob;

        //    for(vob = this; vob != null; vob = vob.Parent)
        //    {
        //        mounts.Add(vob.Mounts)
        //    }
        //    //foreach (var mount in Mounts.GetMountsForPath(reference.Path))
        //    //{
        //    //    mount.RootObject
        //    //}
        //    return null;
        //}

        #endregion

#endif
        
        #region Delete

        //public DeleteMode DeleteMode { get; set; }
        //enum DeleteMode // FUTURE
        //{
        //    Unspecified = 0,
        //    DeleteRetrieved,
        //    //DeleteOne, - silly?
        //    DeleteAll,
        //}

        #endregion

        #region Save

#if TOPORT
        public VobHandle<T> SaveObject<T>(T obj)
            where T : class
        {
            IVobHandle vhObj = obj as IVobHandle;
            VobHandle<T> actualVH;

        #region IVobHandle

            if (vhObj != null)
            {
                throw new ArgumentException("Use Save method to save VobHandles.");
                //Type vhType = vhObj.Type;
                //if (vhType != typeof(T))
                //{

                //}
                //actualVH = (VobHandle<T>)TrySetHandle(vhObj, vhType);


                ////VobHandle<T> vhGen = vhObj as VobHandle<T>;
                ////if(vhGen==null)
                ////{
                ////    l.TraceWarn("Unexpected vobHandle type, not attempting to use it as an authoritative VobHandle for this Vob: " + obj.GetType().FullName + " (" + this.ToString()+ ")");
                ////    var realVHGen = this.GetHandle<T>(); 
                ////    realVHGen
                ////}

                ////if (this.UnitypeHandle == null) 
                ////{
                ////    this.UnitypeHandle = vhObj; 
                ////}

                ////VobHandle<T> vhGen = this.UnitypeHandle as VobHandle<T>;
                ////if (vhGen != null)
                ////{
                ////    var authoritativeHandle = this.TrySetHandle<T>(vhGen);

                ////    return authoritativeHandle;
                ////}
                ////else

            }
        #endregion
            else
            {
                //VobHandle<T> h = this.UnitypeHandle as VobHandle<T>;
                actualVH = GetHandle<T>();
                actualVH.Object = obj;
            }

#if AOT
            Save(h, typeof(T));
#else
            Save<T>(actualVH);
#endif

#if VosUnitype
            if (UnitypeHandle == null)
            {
                UnitypeHandle = actualVH;
            }
#endif
            return actualVH;
        }

#if AOT
		public void Save<T>(VobHandle<T> vosLogicalHandle)
			where T : class
		{
			Save(vosLogicalHandle, typeof(T));

		}
        public void Save(IVobHandle vosLogicalHandle, Type T, bool preview = false)
		{
			if(Path == null)
				throw new ArgumentException("this.Path cannot be null");
			if(vosLogicalHandle == null)
				throw new ArgumentNullException("vosLogicalHandle");
			if(T == null)
				throw new ArgumentNullException("T");

			object obj = null;
#else
        public void Save<T>(VobHandle<T> vosLogicalHandle, bool allowDelete = false, bool preview = false)
            where T : class
        {
            T obj = null;

#endif
#if AOT && true // AOTTEMP
#else
            //l.Info("Vob.Save " + vosLogicalHandle.Path + "<"+T.Name+">  " + Environment.StackTrace);
            //this.Type = typeof(T);
            if (vosLogicalHandle.HasObject)
            {
                try
                {
                    obj = vosLogicalHandle.Object;
                }
                catch (Exception ex)
                {
                    l.Error("Failed to get Object from VobHandle: " + ex);
                    throw;
                }
            }
            //vosLogicalHandle.Object = obj;
            //vosLogicalHandle.Save();

            if (obj == null) // FUTURE: test IsDeleted flag instead or in addition?
            {
                if (!preview) { l.Trace("REVIEW - Save called when !HasObject.  Attempting Delete. - " + allowDelete + " " + ToString()); }
                if (allowDelete)
                {
                    TryDelete(preview: preview);
                }
                else
                {
                    l.Trace(() => "Attempt to Vob.Save((VobHandle<>) vh, allowDelete: false) when vh.Object is null.  Doing nothing. " + this);
                }
                return;
            }

#if TOPORT
            if (!preview)
            {
                INotifyOnSavingTo nos = obj as INotifyOnSavingTo;
                if (nos != null)
                {
                    nos.OnSavingTo(this);
                }
            }
#endif

            //    Save(_object, package, location);
            //}
            //public void Save(object obj, string package = null, string location = null)
            //{
            var context = VosContext.Current;
            string package = vosLogicalHandle.EffectivePackage;
            string location = vosLogicalHandle.EffectiveStore;

            if (package == null)
            {
                if (context != null)
                {
                    package = context.Package;
                }
            }
            if (location == null)
            {
                if (context != null)
                {
                    location = context.Store;
                }
            }

            H<T> saveHandle = null;

            if (vosLogicalHandle.Mount != null
                //|| vosLogicalHandle.Location != null || vosLogicalHandle.Package != null
                )
            {
                // TOVALIDATE?
                saveHandle = GetHandleFromMount<T>(vosLogicalHandle.Mount);
                //l.Trace("SAVE: specifying Mount.  Got handle: " + saveHandle + ". TODO: Does this mean this object was saved more than once, unintentionally?");
            }

            //if (
            //     location != null 
            //     || package != null
            //    )
            //{
            //    l.Warn("SAVE: Not implemented: specifying Location/Package ");
            //}

            ////AssetContext.Current.DefaultSavePackage
            //string packageSubpath = null;
            //var packageMount = effectiveMountsByName.TryGetValue(package);
            //if (packageMount != null)
            //{
            //}

            if (saveHandle == null)
            {
                // TODO: 
                // - Test this.IsPackage(packageName), same for location.
                // - lock down Vob branches as being only for a location/package. 

#if AOT
                var tempSaveHandle = "".PathToVobHandle( package, location,T);
#else
                var tempSaveHandle = "".PathToVobHandle<T>(package, location);
#endif
                //#error SAving Timeline--no save location??
                if (!string.IsNullOrWhiteSpace(tempSaveHandle.Path.TrimEnd(LionPath.PathDelimiter)) && Path.StartsWith(tempSaveHandle.Path.TrimEnd(LionPath.PathDelimiter))) // TEMP approach TODO
                {
                    saveHandle = GetFirstWriteHandle<T>();

                }
                else
                {
                    // First time: don't save to generic mounts (null package/store)
                    foreach (var kvp in WriteHandleMounts)
                    {
                        var mount = kvp;

                        if (package != null && package != mount.Package)
                        {
                            continue;
                        }

                        if (location != null && location != mount.Store)
                        {
                            continue;
                        }

                        //saveHandle = kvp.Key;
                        //saveHandle = mount.Vob.GetHandle<T>();
                        //saveHandle = mount.Root.GetHandle<T>(); - wrong
                        saveHandle = GetHandleFromMount<T>(mount);

                        if (vosLogicalHandle.Mount == null)
                        {
                            vosLogicalHandle.Mount = mount;
                        }
                        break;
                    }

                    // First time: save to any generic mounts that are available (null package/store)
                    if (saveHandle == null)
                    {
                        //foreach (var kvp in WriteHandlesWithMounts)
                        foreach (var kvp in WriteHandleMounts)
                        {
                            var mount = kvp;

                            if (package != null && mount.Package != null && package != mount.Package)
                            {
                                continue;
                            }

                            if (location != null && mount.Store != null && location != mount.Store)
                            {
                                continue;
                            }

                            //saveHandle = kvp.Key;
                            //saveHandle = mount.Vob.GetHandle<T>();
                            //saveHandle = mount.Root.GetHandle<T>(); - wrong
                            saveHandle = GetHandleFromMount<T>(mount);

                            if (vosLogicalHandle.Mount == null)
                            {
                                vosLogicalHandle.Mount = mount;
                            }
                            break;
                        }
                    }
                }

                //saveHandle = this.Path.PathToVobHandleRENAME<T>(package, location);
                //if (saveHandle == this)
                //{
                //    saveHandle = GetFirstWriteHandle();
                //}
            }

            //if (saveHandle == null)
            //{
            //    saveHandle = objectHandle;
            //}
            //if (saveHandle == null)
            //{
            //    saveHandle = GetFirstWriteHandle();
            //    //objectHandle = saveHandle;
            //}

            //H mountHandle = FirstWriteHandle;

            if (saveHandle == null)
            {
                Vos.OnNoSaveLocation(this);
                return;
            }

            if (!preview)
            {
                if (saveHandle.HasObject)
                {
                    if (!System.Object.ReferenceEquals(saveHandle.Object, obj))
                    {
#if WARN_VOB
                        if (saveHandle.Object.GetType() != obj.GetType())
                        {
                            lSave.Warn(ToString() + ": Vob.Save Replacing object '" + saveHandle.Object.GetType().Name + "' in concrete OBase with object of different type: " + obj.GetType().Name);
                        }
                        else
                        {
                            lSave.Info(ToString() + ": Vob.Save Replacing object in concrete OBase with object of same type. "
                                           //							           + Environment.NewLine + Environment.StackTrace
                                           );
                        }
#endif
                        saveHandle.Object = obj;
                    }
                }
                else
                {
                    saveHandle.Object = obj;
                }

                MBus.Current.Publish(new VobSaveEvent(VobReference, saveHandle.Reference));


#if INFO_VOB
            if (saveHandle.ToString().Contains("file:"))
            {
                lSave.Info("[SAVE] Vob " + vosLogicalHandle.ToString() + " => " + saveHandle.ToString());
            }
            else
            {
                lSave.Debug("[save] Vob " + vosLogicalHandle.ToString() + " => " + saveHandle.ToString());
            }
#endif

                saveHandle.Save();

#if VosUnitype
                if (this.UnitypeHandle == null)
                {
                    this.UnitypeHandle = vosLogicalHandle;
                }
#endif
            }
#endif
        }
               

        // OLD, but rework?  - no more saving without specifying object.
        //        /// <summary>
        //        /// Save to a Vob location, which may be a virtual and layered location.  (If you wanted to save to a particular location or package, you should save to
        //        /// another Vob representing that desire.)
        //        /// </summary>
        //        public void Save()
        //        {
        //            //    this.Save(null, null);
        //            //}
        //            //public void Save(string package = null, string location = null)
        //            //{
        //            object obj = _object;

        //            if (obj == null) // FUTURE: test IsDeleted flag instead or in addition?
        //            {
        //                l.Warn("REVIEW - Save called when !HasObject.  Attempting Delete.");
        //                TryDelete();
        //                return;
        //            }

        //            //    Save(_object, package, location);
        //            //}
        //            //public void Save(object obj, string package = null, string location = null)
        //            //{
        //            var context = VosContext.Current;

        //            H saveHandle = null;

        //            //if (package == null)
        //            //{
        //            //    if (context != null) package = context.Package;
        //            //}
        //            //if (location == null)
        //            //{
        //            //    if (context != null) location = context.Location;
        //            //}

        //            ////AssetContext.Current.DefaultSavePackage
        //            //string packageSubpath = null;
        //            //var packageMount = effectiveMountsByName.TryGetValue(package);
        //            //if (packageMount != null)
        //            //{
        //            //}

        //            //if (location != null)
        //            //{

        //            //    saveHandle = this.Path.PathToVobHandle<object>(package, location);
        //            //}

        //            //if (saveHandle == null)
        //            //{
        //            //    saveHandle = objectHandle;
        //            //}
        //            if (saveHandle == null)
        //            {
        //                saveHandle = GetFirstWriteHandle();
        //                //objectHandle = saveHandle;
        //            }

        //            //H mountHandle = FirstWriteHandle;

        //            if (saveHandle == null)
        //            {
        //                this.Vos.OnNoSaveLocation(this);
        //                return;
        //            }

        //            if (saveHandle.HasObject)
        //            {
        //                if (!System.Object.ReferenceEquals(saveHandle.Object, obj))
        //                {
        //#if WARN_VOB
        //                    if (saveHandle.Object.GetType() != obj.GetType())
        //                    {
        //                        l.Warn(this.ToString() + ": Replacing object in concrete OBase with object of different type.");
        //                    }
        //                    else
        //                    {
        //                        l.Info(this.ToString() + ": Replacing object in concrete OBase with object of same type.");
        //                    }
        //#endif
        //                    saveHandle.Object = obj;
        //                }
        //            }
        //            else
        //            {
        //                saveHandle.Object = obj;
        //            }

        //#if INFO_VOB
        //            if (saveHandle.ToString().Contains("file:"))
        //            {
        //                lSave.Info("[SAVE] Vob " + this.ToString() + " => " + saveHandle.ToString());
        //            }
        //            else
        //            {
        //                lSave.Debug("[save] Vob " + this.ToString() + " => " + saveHandle.ToString());
        //            }
        //#endif
        //            saveHandle.Save();
        //        }
#endif
        #endregion

    }
}
