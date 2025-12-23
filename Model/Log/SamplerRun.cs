// -----------------------------------------------------------------------
// <copyright file="SamplerRun.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.IO;

namespace Model.Log
{
    public class SamplerRun : SamplerBase, ISampler
    {
        public SamplerRun() 
        {
        }

        public List<List<double>> GetAllValues(int start)
        {
            List<List<double>> rv = new List<List<double>>();
            foreach (string line in this.CurrentLog)
            {
                rv.Add(this.GetLineValues(line, start));
            }
            return rv;
        }

        public void Refresh(){}
        public void ReadNextTemperaturLog() { }
        public void ReadPreviuosTemperaturLog() { }

        public List<Tuple<double, double, double>> GetAllValuesFromZone(int zone)
        {
            List<Tuple<double, double, double>> rv = new List<Tuple<double, double, double>>();
            foreach (string line in this.CurrentLog)
            {
                rv.Add(this.GetLineValuesFromZone(line, zone));
            }
            return rv;
        }

        public List<Tuple<double, double, double, double, double, double, double>> GetValueTable(int valueRow)
        {
            List<Tuple<double, double, double, double, double, double, double>> rv = new List<Tuple<double, double, double, double, double, double, double>>();
            foreach (string line in this.CurrentLog)
            {
                rv.Add(this.GetValues(valueRow, line));
            }
            return rv;
        }
    }

}
