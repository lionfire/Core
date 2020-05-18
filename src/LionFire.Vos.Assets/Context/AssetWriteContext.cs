#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using LionFire.Copying;
using LionFire.ExtensionMethods.Copying;

namespace LionFire.Vos.Assets
{
    public class AssetWriteContext : IDisposable
    {
        #region Static

        /// <summary>
        /// The current context, using an AsyncLocal with a global static fallback of RootContext
        /// </summary>
        public static AssetWriteContext? Current
        {
            get
            {
                var s = ContextStack;
#if SanityChecks
                if (s == null)
                {
                    throw new UnreachableCodeException("contextStack == null");
                }
#endif
                return ContextStack.Count > 0 ? ContextStack.Peek() : rootContext;
            }
        }

        private static Stack<AssetWriteContext> ContextStack
        {
            get
            {
                if (contextStack.Value == null)
                {
                    contextStack.Value = new Stack<AssetWriteContext>();
                }
                return contextStack.Value;
            }
        }
        private static AsyncLocal<Stack<AssetWriteContext>> contextStack = new AsyncLocal<Stack<AssetWriteContext>>();

        /// <summary>
        /// The default shared context for the process
        /// </summary>
        public static AssetWriteContext RootContext => null;// rootContext;
        private static AssetWriteContext rootContext => null;

        static AssetWriteContext()
        {
            contextStack = new AsyncLocal<Stack<AssetWriteContext>>();
            //rootContext = new AssetWriteContext();
        }
        #endregion

        public AssetWriteContext(VosReference writeLocation)
        {
            WriteLocation = writeLocation;
            ContextStack.Push(this);
        }

        public void Dispose()
        {
            if (rootContext != null && object.ReferenceEquals(this, rootContext))
            {
                throw new InvalidOperationException("Disposing root context is not allowed.");
            }

            var vosContext = ContextStack.Peek();
            if (Object.ReferenceEquals(vosContext, this))
            {
                ContextStack.Pop();
            }
            else
            {
                throw new InvalidOperationException("Cannot dispose context when it is not on top of the context stack.");
            }
        }

        #region (Public) Properties

        public VosReference WriteLocation { get; }

        #endregion
    }
}

