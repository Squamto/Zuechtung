// -----------------------------------------------------------------------
// <copyright file="StepChannel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.FG
{
    public class StepChannel
    {
        public StepChannel(int cycles, double targetTemp)
        {
            this.Cycles = cycles;
            this.TargetTemp = targetTemp;
        }

        public int Cycles { get; }

        public double TargetTemp { get; }
    }
}
