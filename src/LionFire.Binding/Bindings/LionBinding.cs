//#define TRACE_ENDCHANGED
//#define TRACE_SOURCECHANGED
//#define TRACE_UPDATE
#define TRACE_COLLECTION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Linq.Expressions;
using Expression = System.Linq.Expressions.Expression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LionFire.Bindings
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Current requirements
    ///  - Must set Path before Object, for either Source/Target
    ///  - Objects must implement INotifyPropertyChanged
    ///  
    /// Ideas:
    ///  - Looks like Source/Target must be set in ctor right now
    ///  - Could/should it be restructured to be a recursive binding?
    /// </remarks>
    public class LionBinding : IDisposable
    {
        #region (Public) Configuration

        public bool AutoAsync = true;

        #endregion

        #region (Public) Parameters

        #region Source

        public object Source
        {
            get { return SourceBindingNode.BindingObject; }
            set
            {
                if (SourceBindingNode == null) throw new LionBindingException("SourcePath must be set before setting Source object.");
                SourceBindingNode.BindingObject = value;

                //UpdateSourceAttachment();
            }
        }

        //public object Source
        //{
        //    get { return source; }
        //    set
        //    {
        //        source = value;
        //        if (IsBound)
        //        {
        //            SourceBindingNode.BindingObject = source;
        //        }
        //        else
        //        {
        //            UpdateSourceAttachment();
        //        }
        //    }
        //} private object source;

        #endregion

        #region Target

        public object Target
        {
            get { return TargetBindingNode.BindingObject; }
            set
            {
                if (TargetBindingNode == null) throw new LionBindingException("TargetPath must be set before setting Target object.");
                TargetBindingNode.BindingObject = value;

                //UpdateTargetAttachment();
            }
        }

        //public object Target
        //{
        //    get { return TargetBindingNode.BindingObject; }
        //    set
        //    {
        //        if (TargetBindingNode == null) throw new LionBindingException("TargetPath must be set before setting Target object.");
        //        TargetBindingNode.BindingObject = value;

        //        UpdateTargetAttachment();
        //    }
        //}  private object target;

        #endregion

        #region ToSource

        public bool ToSource
        {
            get { return toSource; }
            set
            {
                if (toSource == value) return;
                toSource = value;
                OnPropertyChanged("ToSource");
            }
        } private bool toSource;

        #endregion

        #region ToTarget

        public bool ToTarget
        {
            get { return toTarget; }
            set
            {
                if (toTarget == value) return;
                toTarget = value;
                OnPropertyChanged("ToTarget");
            }
        } private bool toTarget = true;

        #endregion

        #region SourcePath

        public string SourcePath
        {
            get { return sourcePath; }
            set
            {
                if (sourcePath == value) return;
                sourcePath = value;

                UpdateSourceAttachment();
            }
        } private string sourcePath;

        private string[] SourcePathChunks
        {
            //get { return SourcePath.Split('.'); }
            get { return GetChunks(SourcePath); }
        }

        private static string[] GetChunks(string pathString)
        {
            bool isAngleBrackets = false;
            char[] path = pathString.ToCharArray();
            for (int i = 0; i < path.Length; i++)
            {
                if (isAngleBrackets)
                {
                    if (path[i] == '>')
                    {
                        isAngleBrackets = false;
                        continue;
                    }
                    if (path[i] == '.') path[i] = '^';
                }
                if (path[i] == '<')
                {
                    isAngleBrackets = true;
                }
            }

            string[] chunks = new string(path).Split('.');
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i] = chunks[i].Replace('^', '.');
            }

            return chunks;
        }

        #endregion

        #region TargetPath

        public string TargetPath
        {
            get { return targetPath; }
            set
            {
                if (targetPath == value) return;
                targetPath = value;

                UpdateTargetAttachment();
            }
        }private string targetPath;

        private string[] TargetPathChunks
        {
            //get { return TargetPath.Split('.'); }
            get { return GetChunks(TargetPath); }
        }

        #endregion

        #region Flags

        public static LionBindingFlags DefaultFlags = LionBindingFlags.ThrowOnSetException;

        /// <summary>
        /// If true, Source has precedence over target when both are bound.  If false,
        /// Target has precedence and the Source will be overwritten when both ends are available.
        /// </summary>
        public bool SourcePrecedence = true;

        public bool SetSourceValueAsync = false;
        public bool SetTargetValueAsync = false;

        public bool ThrowOnMissingEvents
        {
            get { return flags.HasFlag(LionBindingFlags.ThrowOnMissingEvents); }
        }

        #region ThrowOnSetException

        public bool ThrowOnSetException
        {
            get { return flags.HasFlag(LionBindingFlags.ThrowOnSetException); }
        }

        #endregion

        #region BindToCollectionEvents

        public bool BindToCollectionEvents
        {
            get
            {
                return !this.Flags.HasFlag(LionBindingFlags.IgnoreCollectionEvents);
            }
        }

        #endregion

        #region Flags

        public LionBindingFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        } private LionBindingFlags flags = DefaultFlags;

        #endregion

        #endregion

        #endregion

        #region (Public) Status

        /// <summary>
        /// Set to true during construction
        /// </summary>
        private bool IsConstructing { get; set; }

        public bool IsBound { get { return HasEnds && !IsConstructing; } }
        public bool HasEnds { get { return HasEndSource && HasEndTarget; } }
        public object EndTarget { get { return TargetEndBindingNode.BindingObject; } }
        public object EndSource { get { return SourceEndBindingNode.BindingObject; } }
        public bool HasEndTarget { get { return TargetEndBindingNode != null && EndTarget != null; } }
        public bool HasEndSource { get { return SourceEndBindingNode != null && EndSource != null; } }
        public bool HasSource { get { return SourceBindingNode != null && Source != null; } }
        public bool HasTarget { get { return TargetBindingNode != null && Target != null; } }

        #endregion

        // It would be nice to set these in C# initializers but it doesn't seem to work, not sure why. So set it in ctor only.
        public Func<object, object> ToSourceConversion { get; internal set; }
        public Func<object, object> ToTargetConversion { get; internal set; }

        #region Construction

        //public LionBinding() { }

        // TODO: Move more params to flags
        public LionBinding(object source, string sourcePath, object target, string targetPath, bool toSource = false, bool toTarget = true, bool sourcePrecedence = true, LionBindingFlags? flags = null, Func<object, object> toSourceConversion = null, Func<object, object> toTargetConversion = null)
        {
            if (flags.HasValue) this.flags = flags.Value;

            IsConstructing = true;

            this.SourcePrecedence = sourcePrecedence;
            this.TargetPath = targetPath;
            this.SourcePath = sourcePath;

            this.ToSource = toSource;
            this.ToTarget = toTarget;

            // Set Source/Target after setting Paths
            this.Source = source;
            this.Target = target;

            if(toSourceConversion != null) this.ToSourceConversion = toSourceConversion;
            if(toTargetConversion != null) this.ToTargetConversion = toTargetConversion;

            IsConstructing = false;

            OnBothEndsChanged();

            //if (ThrowOnMissingEvents)
            //{
            //    if (toTarget && !SourceEndBindingNode.IsEventAttached)
            //    {
            //        throw new 
            //    }

            //}
        }

        //public LionBinding(
        //    object source, Expression sourceExpression, object target, Expression targetExpression
        //    LionBinding lionBinding,  bool isTargetNode)
        //{
        //    var type = expression.Type;
        //    MemberExpression memberExpression;

        //}

        #endregion

        #region (Public) Methods

        public void ManualRefresh()
        {
            // TODO - For a debug version, make this bool, and check whether any values are changed.  Return true if this actually did something.
            OnBothEndsChanged();
        }

        #endregion

        #region (Private) Methods

        private void UpdateTargetValueFromSource()
        {
            if (IsUpdatingSourceFromTarget > 0) return;
            UpdateTargetValueFromSource(SourceValue);
        }

        public bool IsCollectionModeToTarget
        {
            get { return TargetEndBindingNode.IsPropertyCollection /* && SourceEndBindingNode.IsEnumerable */; }
        }
        public bool IsCollectionModeToSource
        {
            get { return SourceEndBindingNode.IsPropertyCollection /* && TargetEndBindingNode.IsEnumerable */; }
        }

        private void UpdateTargetValueFromSource(object newValue)
        {
            if (IsUpdatingSourceFromTarget > 0) return;

            bool isLocked;
            if (!(isLocked = Monitor.TryEnter(updatingLock, 10000))) // HARDTIME
            {
                lError.Warn("Acquire lock timeout: Reentrant binding detected.  New value: " + newValue + " Stack: " + Environment.NewLine + Environment.StackTrace);
            }
            try
            //lock (updatingLock)
            {
                if (IsUpdatingSourceFromTarget > 0) return;

                try
                {
                    //if (IsCollectionBound) return; // TOTEST: Collection refresh should be triggered by recalculation of CommonCollectionTypes

                    IsUpdatingTargetFromSource++;

                    try
                    {
                        if (IsCollectionModeToTarget)
                        {
                            CopySourceCollectionToTarget();
                        }
                        else
                        {
                            if (TargetEndBindingNode.CanWrite)
                            {
                                TargetValue = newValue;
                            }
                        }


#if TRACE_UPDATE
                        try
                        {
                            l.Trace(this.ToString() + " :> " + (SourceCachedValue == null ? "null" : SourceCachedValue.ToString())
                                //                    +
                                //((TargetEndBindingNode.PropertyInfo != null && TargetEndBindingNode.PropertyInfo.CanRead) ? "" : "(readonly target)")
                                + Environment.NewLine + Environment.StackTrace
            );

                            //if (TargetEndBindingNode.PropertyInfo != null && TargetEndBindingNode.PropertyInfo.CanRead)
                            //{

                            //}
                            //else
                            //{
                            //    l.Trace("[bind] " + SourcePath + " >> " + TargetPath + ": " + "(readonly target)");
                            //}

                        }
                        catch (Exception ex) { l.Trace("[bind] Set TargetValue to [exception] - " + ex.ToString()); }
#endif

                    }
                    catch (Exception ex)
                    {
                        if (this.ThrowOnSetException)
                        {
                            l.Warn("Exception when UpdateTargetValueFromSource. (Rethrowing) " + ex.ToString());
                            throw;
                        }
                        else
                        {
                            l.Error("Exception when UpdateTargetValueFromSource. (Ignoring)" + ex.ToString());
                        }
                    }

                    var ev = TargetValueUpdated;
                    if (ev != null)
                    {
                        Task.Factory.StartNew(() =>
                                {
                                    try
                                    {

                                        ev();
                                    }
                                    catch (Exception ex)
                                    {
                                        l.Error("Exception when raising TargetValueUpdated. " + ex.ToString());
                                    }
                                });
                    }
                }
                finally
                {
                    IsUpdatingTargetFromSource--;
                }
            }
            finally
            {
                if (isLocked)
                {
                    Monitor.Exit(updatingLock);
                }
            }
        }

        public event Action TargetValueUpdated;

        private void UpdateSourceValueFromTarget()
        {
            if (IsUpdatingTargetFromSource > 0) return;
            UpdateSourceValueFromTarget(TargetValue);
        }
        private void UpdateSourceValueFromTarget(object newValue)
        {
            if (IsUpdatingTargetFromSource > 0) return;

            lock (updatingLock)
            {
                if (IsUpdatingTargetFromSource > 0) return;

                try
                {
                    IsUpdatingSourceFromTarget++;

                    if (ToTargetConversion != null) newValue = ToTargetConversion(newValue);

                    //if (IsCollectionBound) return; // TOTEST: Collection refresh should be triggered by recalculation of CommonCollectionTypes

                    try
                    {
                        if (IsCollectionModeToSource)
                        {
                            CopyTargetCollectionToSource();
                        }
                        else
                        {
                            if (SourceEndBindingNode.CanWrite)
                            {
                                SourceValue = newValue;
                            }
                        }

#if TRACE_UPDATE
                        try
                        {

                            l.Trace(this.ToString() + " <: " + (SourceCachedValue == null ? "null" : SourceCachedValue.ToString())
                                //                        +
                                //((SourceEndBindingNode.PropertyInfo != null && SourceEndBindingNode.PropertyInfo.CanRead) ? "" : "(readonly source)")
                                + Environment.NewLine + Environment.StackTrace

        );
                            //l.Trace(this.ToString() + ": " + (TargetValue == null ? "null" : TargetValue.ToString()));
                        }
                        catch (Exception ex) { l.Trace("[bind] Set SourceValue to [exception] - " + ex.ToString()); }
#endif
                    }
                    catch (Exception ex)
                    {

                        if (this.ThrowOnSetException)
                        {
                            l.Warn("Exception when UpdateSourceValueFromTarget. (Rethrowing) " + ex.ToString());
                            throw;
                        }
                        else
                        {
                            l.Error("Exception when UpdateSourceValueFromTarget. (Ignoring)" + ex.ToString());
                        }
                    }

                    var ev = SourceValueUpdated;
                    if (ev != null)
                    {
                        try
                        {
                            ev();
                        }
                        catch (Exception ex)
                        {
                            l.Error("Exception when raising SourceValueUpdated. " + ex.ToString());
                        }
                    }
                }
                finally
                {
                    IsUpdatingSourceFromTarget--;
                }
            }
        }

        public event Action SourceValueUpdated;

        #endregion

        #region End Changed Handlers

        private void OnBothEndsChanged()
        {
            try
            {
                OnEndChanged();
                var ev = EndSourceChanged; if (ev != null) ev();
                var ev2 = EndTargetChanged; if (ev2 != null) ev2();

                //try
                //{
                //    l.Trace("[bind] " + SourcePath + " >> " + TargetPath + ": BothEndsChanged");
                //}
                //catch (Exception ex) { l.Trace("[bind] OnBothEndsChanged [exception] - " + ex.ToString()); }
            }
            catch (Exception ex)
            {
                l.Error("Binding exception: " + ex.ToString());
            }
        }

        private void OnEndSourceBindingObjectChanged(object old, object newEndBindingObject)
        {
            try
            {
                if (newEndBindingObject != null)
                {
#if TRACE_SOURCECHANGED
                    try
                    {
                        l.Trace("[bind EndSourceChanged] " + this.ToString() + ": " + newEndBindingObject.ToString());
                    }
                    catch (Exception ex) { l.Trace("[bind] OnEndSourceChanged [exception] - " + ex.ToString()); }
#endif

                    //object endValue = SourceValue;
                    //if (endValue != null)
                    //{
                    //    SourceCollectionTypes = GetCollectionTypes(endValue.GetType());
                    //}
                    //else
                    //{
                    //    SourceCollectionTypes = null;
                    //}
                }
                //else
                //{
                //    SourceCollectionTypes = null;
                //}

                OnEndChanged();
                var ev = EndSourceChanged; if (ev != null) ev();
            }
            catch (Exception ex)
            {
                l.Error("Binding exception: " + ex.ToString());
            }
        }

        public event Action EndSourceChanged;

        private void OnEndTargetBindingObjectChanged(object old, object newEndBindingObject)
        {
            try
            {
                if (newEndBindingObject != null)
                {
#if TRACE_ENDCHANGED
                    try
                    {
                        l.Trace("[bind EndTargetChanged] " + this.ToString() + ": " + newEndBindingObject.ToString());
                    }
                    catch (Exception ex) { l.Trace("[bind] EndTargetChanged [exception] - " + ex.ToString()); }
#endif
                    //TargetCollectionTypes = GetCollectionTypes(newEndBindingObject.GetType());

                    //object endValue = TargetValue;
                    //if (endValue != null)
                    //{
                    //    TargetCollectionTypes = GetCollectionTypes(endValue.GetType());
                    //}
                    //else
                    //{
                    //    TargetCollectionTypes = null;
                    //}
                }
                //else
                //{
                //    TargetCollectionTypes = null;
                //}

                OnEndChanged();
                var ev = EndTargetChanged; if (ev != null) ev();
            }
            catch (Exception ex)
            {
                l.Error("Binding exception: " + ex.ToString());
            }
        }

        public event Action EndTargetChanged;

        private void OnEndChanged()
        {
            // Note: not wrapped in try/catch

            //UpdateCollectionBound(IsBound);

            if (IsBound)
            {
                if (ToTarget && ToSource)
                {
                    // Use SourcePrecedence to determine which way the initial binding occurs
                    if (SourcePrecedence)
                    {
                        UpdateTargetValueFromSource();
                    }
                    else
                    {
                        UpdateSourceValueFromTarget();
                    }
                }
                else if (ToTarget)
                {
                    UpdateTargetValueFromSource();
                }
                else if (ToSource)
                { UpdateSourceValueFromTarget(); }
            }

        }

        #endregion

        #region Nodes

        #region TargetBindingNode

        protected LionBindingNode TargetBindingNode
        {
            get { return targetBindingNode; }
            set
            {
                if (targetBindingNode == value) return;
                if (targetBindingNode != null)
                {
                    // Recurses through the chain
                    targetBindingNode.Detach();
                }

                targetBindingNode = value;
            }
        } private LionBindingNode targetBindingNode;

        #endregion

        #region SourceBindingNode

        protected LionBindingNode SourceBindingNode
        {
            get { return sourceBindingNode; }
            set
            {
                if (sourceBindingNode == value) return;
                if (sourceBindingNode != null)
                {
                    sourceBindingNode.Detach();
                }
                sourceBindingNode = value;
            }
        } private LionBindingNode sourceBindingNode;

        #endregion

        #region SourceEndBindingNode

        public LionBindingNode SourceEndBindingNode
        {
            get { return sourceEndBinding; }
            set
            {
                if (sourceEndBinding == value) return;

                if (sourceEndBinding != null)
                {
                    sourceEndBinding.ValueChanged -= OnSourceValueChanged;

                    if (BindToCollectionEvents)
                    {
                        sourceEndBinding.CollectionChanged -= OnSourceCollectionChanged;
                    }
                    sourceEndBinding.BindingObjectChanged -= new ValueChangedHandler<object>(OnEndSourceBindingObjectChanged);
                    //sourceEndBinding.IsBoundToNotifyCollectionChangedChanged -= new Action(OnEndSourceBinding_IsBoundToNotifyCollectionChangedChanged);
                    //sourceEndBinding.ValueCollectionTypesChanged -= new Action(sourceEndBinding_ValueCollectionTypesChanged);
                }

                sourceEndBinding = value;

                if (sourceEndBinding != null)
                {
                    sourceEndBinding.ValueChanged += OnSourceValueChanged;

                    if (BindToCollectionEvents)
                    {
                        sourceEndBinding.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnSourceCollectionChanged);
                    }

                    sourceEndBinding.BindingObjectChanged += new ValueChangedHandler<object>(OnEndSourceBindingObjectChanged);
                    //sourceEndBinding.IsBoundToNotifyCollectionChangedChanged += new Action(OnEndSourceBinding_IsBoundToNotifyCollectionChangedChanged);
                    //sourceEndBinding.ValueCollectionTypesChanged += new Action(sourceEndBinding_ValueCollectionTypesChanged);
                }
            }
        }private LionBindingNode sourceEndBinding;

        #endregion

        #region TargetEndBindingNode

        public LionBindingNode TargetEndBindingNode
        {
            get { return targetEndBinding; }
            set
            {
                if (targetEndBinding == value) return;

                if (targetEndBinding != null)
                {
                    targetEndBinding.ValueChanged -= OnTargetValueChanged;
                    if (BindToCollectionEvents)
                    {
                        targetEndBinding.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnTargetCollectionChanged);
                    }
                    targetEndBinding.BindingObjectChanged -= new ValueChangedHandler<object>(OnEndTargetBindingObjectChanged);
                    //targetEndBinding.IsBoundToNotifyCollectionChangedChanged -= new Action(OnEndTargetBinding_IsBoundToNotifyCollectionChangedChanged);

                    //targetEndBinding.ValueCollectionTypesChanged -= new Action(targetEndBinding_ValueCollectionTypesChanged);
                }

                targetEndBinding = value;

                if (targetEndBinding != null)
                {
                    targetEndBinding.ValueChanged += OnTargetValueChanged;

                    if (BindToCollectionEvents)
                    {
                        targetEndBinding.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnTargetCollectionChanged);
                    }

                    targetEndBinding.BindingObjectChanged += new ValueChangedHandler<object>(OnEndTargetBindingObjectChanged);
                    //targetEndBinding.IsBoundToNotifyCollectionChangedChanged += new Action(OnEndTargetBinding_IsBoundToNotifyCollectionChangedChanged);
                    //targetEndBinding.ValueCollectionTypesChanged += new Action(targetEndBinding_ValueCollectionTypesChanged);
                }
            }
        } private LionBindingNode targetEndBinding;


        #endregion

        #endregion

        #region Values

        public object TargetValue
        {
            get
            {
                return TargetEndBindingNode.Value;
            }
            set
            {
                TargetEndBindingNode.Value = value;
            }
        }

        public object SourceValue
        {
            get
            {
                return SourceEndBindingNode.Value;
            }
            set
            {
                SourceEndBindingNode.Value = value;
            }
        }

        #endregion

        #region CachedValues

        public object TargetCachedValue
        {
            get
            {
                return TargetEndBindingNode.CachedValue;
            }
            //set
            //{
            //    TargetEndBindingNode.CachedValue = value;
            //}
        }

        public object SourceCachedValue
        {
            get
            {
                return SourceEndBindingNode.CachedValue;
            }
            //set
            //{
            //    SourceEndBindingNode.CachedValue = value;
            //}
        }

        #endregion

        #region On Value Changed

        private int IsUpdatingTargetFromSource;
        private int IsUpdatingSourceFromTarget;
        private object updatingLock = new object();

        private void OnTargetValueChanged(object targetValue)
        {
            if (!IsBound) return;

            if (ToSource)
            {
                UpdateSourceValueFromTarget(targetValue);
                //SourceValue = targetValue;
            }
        }

        private void OnSourceValueChanged(object sourceValue)
        {
            if (!IsBound) return;

            if (ToTarget)
            {
                UpdateTargetValueFromSource(sourceValue);
                //TargetValue = sourceValue;
            }
        }

        #endregion

        #region On Collection Changed

        #region Collection Bindings On / Off

        //public bool IsCollectionBound
        //{
        //    get
        //    {
        //        // Collection binding is in effect if and only if one or both of IsAttachedTo___NotifyCollectionChanged is true
        //        return IsAttachedToSourceNotifyCollectionChanged || IsAttachedToTargetNotifyCollectionChanged;
        //        //return isCollectionBound;
        //    }
        //    //set
        //    //{
        //    //    if (isCollectionBound) return;
        //    //    isCollectionBound = value;
        //    //}
        //} //private bool isCollectionBound;

        //private void OnCollectionBoundChanged()
        //{
        //    if (IsCollectionBound)
        //    {
        //        if (ToSource && ToTarget)
        //        {
        //            if (SourcePrecedence)
        //            {
        //                CopySourceCollectionToTarget();
        //            }
        //            else
        //            {
        //                CopyTargetCollectionToSource();
        //            }
        //        }
        //        else if (ToTarget)
        //        {
        //            CopySourceCollectionToTarget();
        //        }
        //        else if (ToSource)
        //        {
        //            CopyTargetCollectionToSource();
        //        }
        //    }
        //}

        //void OnEndSourceBinding_IsBoundToNotifyCollectionChangedChanged()
        //{
        //    UpdateCollectionBound();
        //}

        //void OnEndTargetBinding_IsBoundToNotifyCollectionChangedChanged()
        //{
        //    UpdateCollectionBound();
        //}

        //void sourceEndBinding_ValueCollectionTypesChanged()
        //{
        //    CalculateCommonCollectionTypes();
        //    UpdateCollectionBound();
        //}
        //void targetEndBinding_ValueCollectionTypesChanged()
        //{
        //    CalculateCommonCollectionTypes();
        //    UpdateCollectionBound();
        //}

        ///// <summary>
        ///// Invoked when an end object changes
        ///// </summary>
        ///// <param name="desiredValue"></param>
        //private void UpdateCollectionBound(bool desiredValue = true)
        //{
        //    //if (!IsBound) desiredValue = false;

        //    if (!desiredValue || CommonCollectionTypes == null || CommonCollectionTypes.Count == 0)
        //    {
        //        IsAttachedToSourceNotifyCollectionChanged = false;
        //        IsAttachedToTargetNotifyCollectionChanged = false;
        //        return;
        //    }

        //    if (ToTarget)
        //    {
        //        if (SourceBindingNode.IsBoundToNotifyCollectionChanged)
        //        {
        //            IsAttachedToSourceNotifyCollectionChanged = true;
        //        }
        //        else
        //        {
        //            //IsAttachedToSourceNotifyCollectionChanged = false;

        //            IsAttachedToSourceNotifyCollectionChanged = true;

        //        }
        //    }

        //    if (ToSource)
        //    {
        //        if (TargetBindingNode.IsBoundToNotifyCollectionChanged)
        //        {
        //            IsAttachedToTargetNotifyCollectionChanged = true;
        //        }
        //        else
        //        {
        //            IsAttachedToTargetNotifyCollectionChanged = false;
        //        }
        //    }
        //}

        //public bool IsAttachedToSourceNotifyCollectionChanged
        //{
        //    get
        //    {
        //        return isAttachedToSourceNotifyCollectionChanged;
        //    }
        //    set
        //    {
        //        if (value == isAttachedToSourceNotifyCollectionChanged) return;

        //        if (SourceBindingNode == null) throw new Exception("SourceBindingNode is null");

        //        if (value)
        //        {
        //            SourceBindingNode.CollectionChanged += new NotifyCollectionChangedEventHandler(SourceBindingNode_CollectionChanged);
        //        }
        //        else
        //        {
        //            SourceBindingNode.CollectionChanged -= new NotifyCollectionChangedEventHandler(SourceBindingNode_CollectionChanged);
        //        }
        //        isAttachedToSourceNotifyCollectionChanged = value;
        //        OnCollectionBoundChanged();
        //    }
        //} private bool isAttachedToSourceNotifyCollectionChanged;

        //public bool IsAttachedToTargetNotifyCollectionChanged
        //{
        //    get
        //    {
        //        return isAttachedToTargetNotifyCollectionChanged;
        //    }
        //    set
        //    {
        //        if (value == isAttachedToTargetNotifyCollectionChanged) return;

        //        if (TargetBindingNode == null) throw new Exception("TargetBindingNode is null");

        //        if (value)
        //        {
        //            TargetBindingNode.CollectionChanged += new NotifyCollectionChangedEventHandler(TargetBindingNode_CollectionChanged);
        //        }
        //        else
        //        {
        //            TargetBindingNode.CollectionChanged -= new NotifyCollectionChangedEventHandler(TargetBindingNode_CollectionChanged);
        //        }
        //        isAttachedToTargetNotifyCollectionChanged = value;
        //        OnCollectionBoundChanged();
        //    }
        //} private bool isAttachedToTargetNotifyCollectionChanged;

        #endregion

        #region On Collection Changed


        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (!ToTarget)
            {
                // OPTIMIZE - Detach this evet when ToTarget is false.
                //l.Trace("UNEXPECTED: Got Source CollectionChanged event when ToTarget is false."); 
                return;
            }

            //Type destinationType = Target.GetType();
            OnBindingNodeCollectionChanged(args, true);
        }

        private void OnTargetCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (!ToSource)
            {
                // OPTIMIZE - Detach this evet when ToSource is false.
                //l.Trace("UNEXPECTED/OPTIMIZE: Got Target CollectionChanged event when ToSource is false."); 
                return;
            }

            //Type destinationType = Source.GetType();
            OnBindingNodeCollectionChanged(args, false);
        }

        //void SourceBindingNode_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (!ToTarget) { l.Trace("UNEXPECTED: Got Source CollectionChanged event when ToTarget is false."); return; }

        //    Type destinationType = Target.GetType();
        //    OnBindingNodeCollectionChanged(e, destinationType, true);
        //}

        //void TargetBindingNode_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (!ToSource) { l.Trace("UNEXPECTED: Got Target CollectionChanged event when ToSource is false."); return; }
        //    Type destinationType = Source.GetType();
        //    OnBindingNodeCollectionChanged(e, destinationType, true);
        //}

        bool isUpdatingTargetCollection;
        bool isUpdatingSourceCollection;
        //object isUpdatingTargetCollectionLock = new object();
        //object isUpdatingSourceCollectionLock = new object();
        object isUpdatingCollectionLock = new object();


        private void OnBindingNodeCollectionChanged(NotifyCollectionChangedEventArgs e, /*Type destinationType,*/ bool isToTarget)
        {
            lock (isUpdatingCollectionLock)
            {
                #region Loop detection (partial)

                if (isUpdatingSourceCollection && isToTarget)
                {
                    //Monitor.Exit(isUpdatingSourceCollectionLock);
                    l.Trace("UNTESTED - Avoiding loopback in full duplex copy to target.  TODO  - Review this and check values (what if different values are coming the other way?).");
                    return;
                }

                if (isUpdatingTargetCollection && !isToTarget)
                {
                    //Monitor.Exit(isUpdatingTargetCollectionLock);
                    l.Trace("UNTESTED - Avoiding loopback in full duplex copy to target.  TODO  - Review this and check values (what if different values are coming the other way?)");
                    return;
                }

                try
                {
                    if (isToTarget)
                    {
                        isUpdatingTargetCollection = true;
                    }
                    else
                    {
                        isUpdatingSourceCollection = true;
                    }
                #endregion

                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        if (isToTarget)
                        {
                            this.SourceEndBindingNode.RetrieveValue();
                            CopySourceCollectionToTarget();
                        }
                        else
                        {
                            this.TargetEndBindingNode.RetrieveValue();
                            CopyTargetCollectionToSource();
                        }
                    }
                    else if (
                        //e.Action == NotifyCollectionChangedAction.Replace || // REVIEW - Replace can be treated the same as add/remove?
                        e.Action == NotifyCollectionChangedAction.Move)
                    {
                        l.Debug("[binding] Ignored: Collection Move events."); // Need to get ICollection<> here and call Clear
                    }
                    else
                    {
                        //LionBindingNode destination = isToTarget ? SourceEndBindingNode : TargetEndBindingNode; // TODO FIXME REVIEW - this was backwards?
                        LionBindingNode destination = isToTarget ? TargetEndBindingNode : SourceEndBindingNode;

                        if (e.NewItems != null && e.NewItems.Count > 0)
                        {
                            destination.AddRange(e.NewItems);
                        }
                        if (e.OldItems != null && e.OldItems.Count > 0)
                        {
                            destination.RemoveRange(e.OldItems);
                        }
                    }
                }
                finally
                {
                    //  try
                    //{
                    //    Monitor.Exit(isUpdatingSourceCollectionLock);
                    //}
                    //catch (SynchronizationLockException) { }  // REVIEW
                    //try
                    //{
                    //    Monitor.Exit(isUpdatingTargetCollectionLock);
                    //}
                    //catch (SynchronizationLockException) { }

                    #region Loop detection (partial)
                    if (isToTarget)
                    {
                        isUpdatingTargetCollection = false;
                    }
                    else
                    {
                        isUpdatingSourceCollection = false;
                    }
                    #endregion
                }
            }

            var ev = isToTarget ? SourceCollectionChanged : TargetCollectionChanged;
            if (ev != null) ev(e);
        }

        public event Action<NotifyCollectionChangedEventArgs> TargetCollectionChanged;
        public event Action<NotifyCollectionChangedEventArgs> SourceCollectionChanged;

        #endregion

        #region Copy Entire Collection

        private static void CopyCollection(LionBindingNode sourceNode, LionBindingNode targetNode)
        {
            if (!targetNode.IsPropertyCollection)
            {
                l.Warn("CopyCollection called but !targetNode.IsPropertyCollection: " + targetNode);
                return;
            }
            if (!sourceNode.IsEnumerable)
            {
                l.Warn("CopyCollection called but !sourceNode.IsEnumerable: " + sourceNode);
                return;
            }

            var sourceValue = sourceNode.Value;
            if (sourceValue == null)
            {
                if (targetNode.CanWrite)
                {
                    targetNode.Value = null;
                }
                return;
            }

            targetNode.EnsureCollectionCreatedAndClear();
            targetNode.AddRange(sourceNode.Value, sourceNode.ValueEnumerableTypes[0]); // FUTURE: Replace the [0] index accessor to support collection type matching
        }

        private void CopySourceCollectionToTarget()
        {
#if TRACE_COLLECTION
            l.Trace(this.ToString() + ": CopySourceCollectionToTarget()");
#endif
            CopyCollection(SourceEndBindingNode, TargetEndBindingNode);

            // OLD
            //if (!TargetEndBindingNode.IsValueCollection) return;
            //if (!SourceEndBindingNode.IsEnumerable) return;

            //TargetEndBindingNode.Clear();
            //TargetEndBindingNode.AddRange(SourceValue, SourceEndBindingNode.ValueEnumerableTypes[0]); // FUTURE: Replace the [0] index accessor to support collection type matching
        }

        private void CopyTargetCollectionToSource()
        {
#if TRACE_COLLECTION
            l.Trace(this.ToString() + ": CopyTargetCollectionToSource()");
#endif
            CopyCollection(TargetEndBindingNode, SourceEndBindingNode);


            //if (!SourceEndBindingNode.IsValueCollection) return;
            //if (!TargetEndBindingNode.IsEnumerable) return;

            //SourceEndBindingNode.Clear();
            //SourceEndBindingNode.AddRange(TargetValue, TargetEndBindingNode.ValueEnumerableTypes[0]);
        }

        #endregion

        #endregion

        #region Source/Target Object Changed Handlers

        private void AttachObject(string[] pathChunks, bool isTarget)
        {
            //if (attachObject == null) throw new ArgumentNullException("attachObject must not be null");
            if (pathChunks == null) throw new ArgumentNullException("pathChunks must not be null");
            if (pathChunks.Length < 1) throw new ArgumentNullException("Path must have at least one item");

            LionBindingNode topBinding = null;
            LionBindingNode binding = null;
            LionBindingNode previousBinding = null;

            //Type propertyOwnerType = attachObject.GetType();

            for (int bindingIndex = 0; bindingIndex < pathChunks.Length; bindingIndex++)
            {
                string propertyName = pathChunks[bindingIndex];

                binding = new LionBindingNode(this, propertyName, isTarget);

                if (bindingIndex != 0)
                {
                    previousBinding.NextBinding = binding;
                }

                if (bindingIndex == 0)
                {
                    topBinding = binding;
                }
                previousBinding = binding;
            }

            if (isTarget)
            {
                object previousBindingObject = HasTarget ? Target : null;
                TargetBindingNode = topBinding;
                TargetBindingNode.BindingObject = previousBindingObject;
                TargetEndBindingNode = binding;
            }
            else
            {
                object previousBindingObject = HasSource ? Source : null;
                SourceBindingNode = topBinding;
                SourceBindingNode.BindingObject = previousBindingObject;
                SourceEndBindingNode = binding;
            }
        }

        #region Collection Types

        #region SourceCollectionTypes

        public List<Type> SourceCollectionTypes
        {
            get { return SourceEndBindingNode.ValueCollectionTypes; }
            //set { sourceCollectionTypes = value; CalculateCommonCollectionTypes(); UpdateCollectionBound(); }
        } //private List<Type> sourceCollectionTypes;

        #endregion

        #region TargetCollectionTypes

        public List<Type> TargetCollectionTypes
        {
            get { return SourceEndBindingNode.ValueCollectionTypes; }
            //set { targetCollectionTypes = value; CalculateCommonCollectionTypes(); UpdateCollectionBound(); }
        } //private List<Type> targetCollectionTypes;

        private void CalculateCommonCollectionTypes()
        {
            if (SourceCollectionTypes == null || TargetCollectionTypes == null) return;

            List<Type> commonTypes = new List<Type>();
            foreach (Type type in SourceCollectionTypes)
            {
                if (TargetCollectionTypes.Contains(type))
                {
                    commonTypes.Add(type);
                }
            }
            CommonCollectionTypes = commonTypes;
        }

        #endregion

        private List<Type> CommonCollectionTypes;

        #endregion

        #endregion

        #region Path Changed Handlers

        private void UpdateSourceAttachment()
        {
            //if (!IsBound) return;

            if (SourcePath == null)
            {
                SourceBindingNode = null;
                SourceEndBindingNode = null;
            }
            else
            {
                AttachObject(SourcePathChunks, false);
            }
        }

        private void UpdateTargetAttachment()
        {
            //if (!IsBound) return;

            if (TargetPath == null)
            {
                TargetBindingNode = null;
                TargetEndBindingNode = null;
            }
            else
            {
                AttachObject(TargetPathChunks, true);
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private static ILogger l = Log.Get();
        private static ILogger lError = Log.Get("LinFire.Bindings.Errors");
        public int AsyncRetries = 5;

        public void Dispose()
        {
            this.TargetPath = null;
            this.SourcePath = null;
            //this.Source = null;
            //this.Target = null;
        }


        public override string ToString()
        {
            string direction;
            if (ToSource) direction = ToTarget ? " <> " : " <- ";
            else direction = ToTarget ? " -> " : " -- ";

            return "{" + SourcePath + direction + TargetPath + "}";
        }

    }

    [Flags]
    public enum LionBindingFlags
    {
        None = 0,
        ThrowOnMissingEvents = 1 << 1,
        ThrowOnSetException = 1 << 2,
        IgnoreCollectionEvents = 1 << 3,

    }
}
