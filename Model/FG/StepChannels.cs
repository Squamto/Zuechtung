// -----------------------------------------------------------------------
// <copyright file="StepChannels.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.FG
{
    using System.Collections.Generic;
    using Config;

    public class StepChannels
    {
        public StepChannels(int cycles, double targetZ1, double targetZ2, double targetZ3, double targetZ4, double targetZ5, double targetZ6, double targetZ7)
        {
            this.Cycles = cycles;
            this.TargetTemps = new Dictionary<ZoneNames, double>();
            this.TargetTemps.Add(ZoneNames.Zone1, targetZ1);
            this.TargetTemps.Add(ZoneNames.Zone2, targetZ2);
            this.TargetTemps.Add(ZoneNames.Zone3, targetZ3);
            this.TargetTemps.Add(ZoneNames.Zone4, targetZ4);
            this.TargetTemps.Add(ZoneNames.Zone5, targetZ5);
            this.TargetTemps.Add(ZoneNames.Zone6, targetZ6);
            this.TargetTemps.Add(ZoneNames.Zone7, targetZ7);
        }

        public int Cycles { get; }

        public Dictionary<ZoneNames, double> TargetTemps { get; }
    }
}
