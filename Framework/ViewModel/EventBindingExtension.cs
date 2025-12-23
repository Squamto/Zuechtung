// -----------------------------------------------------------------------
// <copyright file="EventBindingExtension.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.ViewModel
{
	using System;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Markup;

    /// <summary>
    /// Xaml markup extension to bind events to commands.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EventBindingExtension"/> class.
    /// </remarks>
    /// <param name="command">The command name to bind to the event.</param>
    public class EventBindingExtension(string command) : MarkupExtension
    {
        #region public methods

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider"> A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Retrieve a reference to the InvokeCommand helper method declared below, using reflection
            MethodInfo invokeCommand = this.GetType().GetMethod("InvokeCommand", BindingFlags.Instance | BindingFlags.NonPublic);
            if (invokeCommand != null)
            {
                // Check if the current context is an event or a method call with two parameters
                var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
                if (target != null)
                {
                    var property = target.TargetProperty;
                    if (property is EventInfo)
                    {
                        // If the context is an event, simply return the helper method as delegate
                        // (this delegate will be invoked when the event fires)
                        var eventHandlerType = (property as EventInfo).EventHandlerType;
                        return invokeCommand.CreateDelegate(eventHandlerType, this);
                    }
                    else if (property is MethodInfo)
                    {
                        // Some events are represented as method calls with 2 parameters:
                        // The first parameter is the control that acts as the event's sender,
                        // the second parameter is the actual event handler
                        var methodParameters = (property as MethodInfo).GetParameters();
                        if (methodParameters.Length == 2)
                        {
                            var eventHandlerType = methodParameters[1].ParameterType;
                            return invokeCommand.CreateDelegate(eventHandlerType, this);
                        }
                    }
                }
            }
            throw new InvalidOperationException("The EventBinding markup extension is valid only in the context of events.");
        }

        #endregion

        #region private methods

        /// <summary>
        /// method to invoke the command.
        /// </summary>
        /// <param name="sender">The sender of the method</param>
        /// <param name="args">Event arguments.</param>
        private void InvokeCommand(object sender, EventArgs args)
        {
            if (!String.IsNullOrEmpty(command))
            {
                var control = sender as FrameworkElement;
                if (control != null)
                {
                    // Find control's ViewModel
                    var viewmodel = control.DataContext;
                    if (viewmodel != null)
                    {
                        // Command must be declared as public property within ViewModel
                        var commandProperty = viewmodel.GetType().GetProperty(command);
                        if (commandProperty != null)
                        {
                            var command = commandProperty.GetValue(viewmodel) as ICommand;
                            if (command != null)
                            {
                                // Execute Command and pass event arguments as parameter
                                if (command.CanExecute(args))
                                {
                                    command.Execute(args);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
