using System;

namespace AuroraPatch
{
    /// <summary>
    /// Abstract representation of a Aurora Patch.
    /// Every patch needs to implement the Start() abstract void method.
    /// All patches have access to the TypeManager object which will be
    /// populated by the time Start() is executed.
    /// </summary>
    public abstract class Patch
    {
        protected TypeManager TypeManager;

        /// <summary>
        /// The main patch program is responsible for executing the Run method
        /// and injecting the TypeManager object to every patch implementation.
        /// </summary>
        /// <param name="typeManager"></param>
        internal void Run(TypeManager typeManager)
        {
            TypeManager = typeManager;
            Start();
        }

        protected abstract void Start();

        /// <summary>
        /// Invoke an action on the main Aurora UI thread.
        /// </summary>
        /// <param name="action"></param>
        protected void InvokeOnUIThread(Action action)
        {
            TypeManager.GetFormInstance(AuroraFormType.TacticalMap).Invoke(action);
        }

        /// <summary>
        /// Invoke a parametarized delegate method on the main Aurora UI thread.
        /// Allows for capturing a return value.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected object InvokeOnUIThread(Delegate method, params object[] args)
        {
            return TypeManager.GetFormInstance(AuroraFormType.TacticalMap).Invoke(method, args);
        }
    }
}
