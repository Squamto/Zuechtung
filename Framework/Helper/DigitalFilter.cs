// -----------------------------------------------------------------------
// <copyright file="DigitalFilter.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.Helper
{
	using System;

	/// <summary>
	/// Defines digital filter fuctionality.
	/// </summary>
	public class DigitalFilter
	{
		private double oldValue;
		private double factorValue;
		private double factorOldValue;
		private double currentCoeff;
		private bool isCoeffReached;

		/// <summary>
		/// Initializes a new instance of the <see cref="DigitalFilter"/> class.
		/// </summary>
		/// <param name="coeff">The filter coefficient (must be greater the 0.0).</param>
		public DigitalFilter(double coeff, bool isCoeffConst = false)
		{
			if (coeff <= 0.0)
			{
				throw new Exception("Exception in " + nameof(DigitalFilter) + ". Intializing with filter coeffizent 0.0 is not possible");
			}

			this.Coeff = coeff;
			this.IsCoeffConst = isCoeffConst;
			if (this.IsCoeffConst)
			{
				this.currentCoeff = this.Coeff;
				this.isCoeffReached = true;
			}
			else
			{
				this.currentCoeff = this.Coeff / 100.0;
				this.isCoeffReached = false;
			}

			this.CalculateFactors();
		}

		/// <summary>
		/// Gets the filter coefficient.
		/// </summary>
		public double Coeff { get; }

		/// <summary>
		/// Gets a value indicating whether the coeffisient is constant or not.
		/// </summary>
		public bool IsCoeffConst { get; }

		public void SetOldValue(double oldvalue)
		{ 
			this.oldValue = oldvalue; 
		}

		/// <summary>
		/// Calculate one filter step.
		/// </summary>
		/// <param name="value">The new value to filter.</param>
		/// <returns>The filter result.</returns>
		public double Step(double value)
		{
			this.CalculateFactors();
			this.oldValue = (value * this.factorValue) + (this.oldValue * this.factorOldValue);
			if (double.IsNaN(this.oldValue))
			{
				this.oldValue = value * this.factorValue;
			}
			return this.oldValue;
		}

		/// <summary>
		/// Calculate the factors.
		/// </summary>
		private void CalculateFactors()
		{
			if (!this.isCoeffReached)
			{
				if (Math.Abs(this.currentCoeff - this.Coeff) > 0.0000000001)
				{
					this.currentCoeff = this.currentCoeff + (this.Coeff / 100.0);
				}
				else
				{
					this.currentCoeff = this.Coeff;
					this.isCoeffReached = true;
				}
			}

			this.factorValue = 1.0 / this.currentCoeff;
			this.factorOldValue = 1.0 - this.factorValue;
		}
	}
}
