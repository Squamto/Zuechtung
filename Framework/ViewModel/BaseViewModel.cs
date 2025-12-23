// -----------------------------------------------------------------------
// <copyright file="BaseViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Threading;

	/// <summary>
	/// Base class for all view models.
	/// </summary>
	public class BaseViewModel : DispatcherObject, INotifyPropertyChanged
	{
		private readonly Dictionary<string, object> propertyBackingDictionary;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseViewModel"/> class.
		/// </summary>
		public BaseViewModel()
		{
            this.propertyBackingDictionary = [];
		}

		/// <summary>
		/// The property changed event, for data binding, tells the property, that the item has been changed
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Re-query commands.
        /// </summary>
        public static void RequeryCommands()
		{
			Application.Current?.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
		}

		/// <summary>
		/// Call theis method if the property was changed, to tell the view to change binding value.
		/// </summary>
		/// <param name="name">The name of the property that has been changed.</param>
		public void OnNotifyPropertyChanged([CallerMemberName] string name = "")
		{
			if (name != null && this.PropertyChanged != null)
			{
				this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
			}
		}

		/// <summary>
		/// Get the property.
		/// </summary>
		/// <param name="propertyName"> The property name. </param>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <returns>The property. </returns>
		/// <exception cref="ArgumentNullException">If property name is null. </exception>
		public T Get<T>([CallerMemberName] string propertyName = "")
		{
			ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

            if (this.propertyBackingDictionary.TryGetValue(propertyName, out object? value))
            {
                return (T)value;
            }

            return default;
		}

		/// <summary>
		/// Set a property with calling notify property name.
		/// </summary>
		/// <param name="newValue"> The new value. </param>
		/// <param name="propertyName"> The property name. </param>
		/// <typeparam name="T">The type of the property.</typeparam>
		public void Set<T>(T newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.CheckAccess())
			{
				if (propertyName != null)
				{
					this.SetUndispatched(newValue, propertyName);
				}
				else
				{
					this.SetUndispatched(newValue, string.Empty);
				}
			}
			else
			{
				try
				{
					this.Dispatcher.Invoke(() => this.SetUndispatched(newValue, propertyName));
				}
				catch (TaskCanceledException)
				{
					// do nothing
				}
			}
		}

		/// <summary>
		/// Set to all relay commands in this view model the names.
		/// </summary>
		public void EnableExeutionLog(Action<LogCategories, string, string>? logAction)
		{
			Type type = this.GetType();
			MemberInfo[] memberInfos = type.GetMembers();
			foreach (MemberInfo memberInfo in memberInfos)
			{
				if (memberInfo.MemberType == MemberTypes.Property)
				{
					PropertyInfo? propertyInfo = type.GetProperty(memberInfo.Name);
					if (propertyInfo.PropertyType == typeof(RelayCommand))
					{
						object? property = this.GetType().GetProperty(memberInfo.Name).GetValue(this, null);
                        if (property is RelayCommand relayCommand)
                        {
                            relayCommand.Name = memberInfo.Name;
                            relayCommand.ParentClassName = type.Name;
                            relayCommand.ExecutionLog = logAction;
                        }
                    }
				}
			}
		}

		/// <summary>
		/// Set the property undispatched and call notify property change.
		/// </summary>
		/// <param name="newValue"> The new value. </param>
		/// <param name="propertyName"> The property name. </param>
		/// <typeparam name="T">The type of the property. </typeparam>
		/// <exception cref="ArgumentNullException">If property name is null. </exception>
		private void SetUndispatched<T>(T newValue, string propertyName)
		{
			ArgumentNullException.ThrowIfNull(nameof(newValue));
			if (EqualityComparer<T>.Default.Equals(newValue, this.GetWithoutCallerMemberName<T>(propertyName)))
			{
				return;
			}

			this.propertyBackingDictionary[propertyName] = newValue;
			this.OnPropertyChangedPur(propertyName);
		}

		/// <summary>
		/// Get the property without caller member name.
		/// </summary>
		/// <param name="propertyName"> The property name. </param>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <returns>The property. </returns>
		/// <exception cref="ArgumentNullException">If property name is null. </exception>
		private T GetWithoutCallerMemberName<T>(string propertyName)
		{
			ArgumentNullException.ThrowIfNullOrWhiteSpace(propertyName);
            if (this.propertyBackingDictionary.TryGetValue(propertyName, out object value))
            {
                return (T)value;
            }

            return default;
		}

		/// <summary>
		/// The property changed event invocator without caller member name.
		/// </summary>
		/// <param name="propertyName"> The property name. </param>
		private void OnPropertyChangedPur(string propertyName = "")
		{
			PropertyChangedEventHandler? propertyChanged = this.PropertyChanged;
			propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
