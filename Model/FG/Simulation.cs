// -----------------------------------------------------------------------
// <copyright file="Simulation.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.FG
{
    using System;
    using Framework.Helper;

    public class Simulation
    {
        private double currentTempertaure;
        private Random random;
        private DigitalFilter pt1;
        private double m;
        private double n;

        public Simulation(int nr, double m, double n, double tk)
        {
            this.random = new Random(nr);
            this.pt1 = new DigitalFilter(tk, true);
            this.m = m;
            this.n = n;
        }

        public double ReadTemperature()
        {
            return this.currentTempertaure;
        }

        public void WritePower(double power)
        {
            this.currentTempertaure = this.pt1.Step(power * m + n) + (this.random.NextDouble() * 0.05);
        }
    }
}
