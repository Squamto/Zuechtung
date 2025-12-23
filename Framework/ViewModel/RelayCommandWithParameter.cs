// -----------------------------------------------------------------------
// <copyright file="RelayCommandWithParameter.cs" company="IB Hermann">
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
    /// <remarks>
    /// Initializes a new instance of the <see cref="RelayCommandWithParameter"/> class.
    /// </remarks>
    /// <param name="execute">The action that should be executed, if the command should executed.</param>
    /// <param name="canExecute">This function gives true as result if the command could be executed.</param>
    /// <param name="exceptionAction">The action which is executed when the execution action throws an exception.</param>
    public class RelayCommandWithParameter(Action<object> execute, Predicate<object?> canExecute, Action<Exception> exceptionAction = null) : ICommand
	{
		/// <summary>
		/// Predicate that determines if an object can execute.
		/// </summary>
		private readonly Predicate<object?>? canExecute = canExecute;

		/// <summary>
		/// The action to execute when the command is invoked.
		/// </summary>
		private readonly Action<object> execute = execute ?? throw new ArgumentNullException(nameof(execute));

		/// <summary>
		/// The action to execute if the execution of the command gats an exception.
		/// </summary>
		private readonly Action<Exception> exceptionAction = exceptionAction;

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
		/// Gets or sets a log method to log in case of execution.
		/// </summary>
		public Action<string, string>? ExecutionLog { get; set; }

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

			return this.canExecute(parameter);
		}

		/// <summary>
		/// The execute.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		public void Execute(object? parameter)
		{
			try
			{
				this.ExecutionLog?.Invoke("Command", $"{this.ParentClassName}->{this.Name}({parameter})");
				this.execute.Invoke(parameter);
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
