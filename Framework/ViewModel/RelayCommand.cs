// -----------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.ViewModel
{
	using System;
	using System.Windows.Input;

	/// <summary>
	/// A simple relay command.
	/// </summary>
	public class RelayCommand : ICommand
	{
		/// <summary>
		/// Predicate that determines if an object can execute.
		/// </summary>
		private readonly Predicate<bool> canExecute;

		/// <summary>
		/// The action to execute when the command is invoked.
		/// </summary>
		private readonly Action<object> executeWithParam;

        /// <summary>
        /// The action to execute when the command is invoked.
        /// </summary>
        private readonly Action execute;
        
		/// <summary>
        /// The action to execute if the execution of the command gats an exception.
        /// </summary>
        private readonly Action<Exception> exceptionAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The action that should be executest, if the command should executed.</param>
		/// <param name="canExecute">This function gives true as result if the command could be executed.</param>
		public RelayCommand(Action execute, Predicate<bool> canExecute, Action<Exception> exceptionAction = null)
		{
			ArgumentNullException.ThrowIfNull(execute, nameof(execute));
			this.execute = execute;
			this.canExecute = canExecute;
			this.exceptionAction = exceptionAction;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The action that should be executest, if the command should executed.</param>
        /// <param name="canExecute">This function gives true as result if the command could be executed.</param>
        public RelayCommand(Action<object> execute, Predicate<bool> canExecute, Action<Exception> exceptionAction = null)
        {
            ArgumentNullException.ThrowIfNull(execute, nameof(execute));
            this.executeWithParam = execute;
            this.canExecute = canExecute;
            this.exceptionAction = exceptionAction;
        }


        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler? CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;

			remove => CommandManager.RequerySuggested -= value;
		}

		/// <summary>
		/// Gets or sets the name of the command.
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Gets or sets the name of the command.
		/// </summary>
		public string? ParentClassName { get; set; }

		/// <summary>
		/// Gets or sets a log method to log in case of execuetion.
		/// </summary>
		public Action<LogCategories, string, string>? ExecutionLog { get; set; }

		/// <summary>
		/// Determines if the command can execute.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <returns>
		/// <code>true</code> if the command can execute; otherwise. <code>false</code>.
		/// </returns>
		public bool CanExecute(object? parameter)
		{
			if (this.canExecute == null)
			{
				return true;
			}

			if (parameter == null)
			{
				return this.canExecute(false);
			}

			if (!(parameter is bool))
			{
				return this.canExecute(false);
			}

			return this.canExecute((bool)parameter);
		}

		/// <summary>
		/// The execute.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		public void Execute(object? parameter)
		{
			try
			{
                this.ExecutionLog?.Invoke(LogCategories.Always, "Command", this.ParentClassName + "->" + this.Name);
				if (this.executeWithParam == null)
				{
					this.execute.Invoke();
				}
				else
				{
					this.executeWithParam(parameter);
				}
			}
			catch (Exception exception)
			{
				if (this.exceptionAction != null)
				{
					this.exceptionAction(exception);
				}
				else
				{
					throw;
				}
			}
		}
	}
}
