// -----------------------------------------------------------------------
// <copyright file="DispatchedBindingList.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.ViewModel
{
    using System.ComponentModel;
    using System.Windows.Threading;

    /// <summary>
    /// Binding list witch dispatched add and clear method.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class DispatchedBindingList<T>(Dispatcher dispatcher) : BindingList<T>
    {
        private readonly Dispatcher dispatcher = dispatcher;

        /// <summary>
        /// Adding a element dispatched to the binding list.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public new void Add(T value)
        {
            if (this.dispatcher.CheckAccess() == false)
            {
                this.dispatcher.Invoke(() => base.Add(value));
            }
            else
            {
                base.Add(value);
            }
        }

        /// <summary>
        /// Remove a element dispatched to the binding list.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public new void Remove(T value)
        {
            if (this.dispatcher.CheckAccess() == false)
            {
                this.dispatcher.Invoke(() => base.Remove(value));
            }
            else
            {
                base.Remove(value);
            }
        }

        /// <summary>
        /// Clear the binding list dispatched.
        /// </summary>
        public new void Clear()
        {
            if (this.dispatcher.CheckAccess() == false)
            {
                this.dispatcher.Invoke(() => base.Clear());
            }
            else
            {
                base.Clear();
            }
        }

        /// <summary>
        /// Insert one element the binding list dispatched.
        /// </summary>
        /// <param name="index">The index, where the object shouldd be insertetd.</param>
        /// <param name="value">The value to insert.</param>
        public new void Insert(int index, T value)
        {
            if (this.dispatcher.CheckAccess() == false)
            {
                this.dispatcher.Invoke(() => base.Insert(index, value));
            }
            else
            {
                base.Insert(index, value);
            }
        }
    }
}
