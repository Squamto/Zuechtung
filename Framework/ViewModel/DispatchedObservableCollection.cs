// -----------------------------------------------------------------------
// <copyright file="DispatchedObservableCollection.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.ViewModel
{
    using System.Collections.Generic;
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows.Threading;

    /// <summary>
    /// Observable collection witch dispatched collection changed event and a dispatched add and clear method.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    [Serializable]
    public class DispatchedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// 
        /// </summary>
        public DispatchedObservableCollection(Dispatcher dispatcher)
            : base()
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public DispatchedObservableCollection(List<T> list, Dispatcher dispatcher)
            : base(list)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        public DispatchedObservableCollection(IEnumerable<T> enumerable, Dispatcher dispatcher)
            : base(enumerable)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Collection changed event, fired in case of the collection was changed.
        /// </summary>
        public override event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Overriding the on collection changed method of the observable collection to make it thread save.
        /// </summary>
        /// <param name="e">The notify collection changed event arguments.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler? notifyCollectionChangedEventHandler = CollectionChanged;
            if (notifyCollectionChangedEventHandler != null)
            {
                if (this.dispatcher.CheckAccess() == false)
                {
                    this.dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => this.OnCollectionChanged(e)));
                }
                else
                {
                    foreach (NotifyCollectionChangedEventHandler nh in notifyCollectionChangedEventHandler.GetInvocationList().Cast<NotifyCollectionChangedEventHandler>())
                    {
                        nh.Invoke(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// Adding a element dispatched to the observable collection.
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
        /// Remove a element dispatched from the observable collection.
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
        /// Remove a element dispatched from the observable collection.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        public new void RemoveAt(int index)
        {
            if (this.dispatcher.CheckAccess() == false)
            {
                this.dispatcher.Invoke(() => base.RemoveAt(index));
            }
            else
            {
                base.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clear the observable collection dispatched.
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
        /// Insert a element dispatched to the observable collection.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="index">The index.</param>
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

        /// <summary>
        /// Moves an element dispatched to new location in the observable collection.
        /// </summary>
        /// <param name="fromIndex">The old index.</param>
        /// <param name="toIndex">The new index.</param>
        public new void Move(int fromIndex, int toIndex)
        {
            if (this.dispatcher.CheckAccess() == false)
            {
                this.dispatcher.Invoke(() => base.MoveItem(fromIndex, toIndex));
            }
            else
            {
                base.Move(fromIndex, toIndex);
            }
        }

    }
}
