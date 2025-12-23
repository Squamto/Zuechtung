// -----------------------------------------------------------------------
// <copyright file="SamplerFile.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.Log
{
    using System.Collections.Generic;
    using System.IO;
    using Config;

    public class SamplerFile : SamplerBase, ISampler
    {
        private int currentLogNr;
        private List<string> logNames;

        public SamplerFile() 
        {
            this.logNames = new List<string>();
        }

        public void Refresh()
        {
            this.logNames.Clear();
            string logPath = Conf.I.GetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LogTemperaturePowerPath));
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            this.logNames = Directory.EnumerateFiles(logPath).ToList();
            if (this.logNames.Count > 1)
            {
                this.currentLogNr = logNames.Count - 1;
            }
            this.ReadLines(this.currentLogNr);
        }

        public void ReadNextTemperaturLog()
        {
            if (this.currentLogNr < this.logNames.Count-1)
            {
                this.currentLogNr++;
            }
            this.ReadLines(this.currentLogNr);
        }

        public void ReadPreviuosTemperaturLog()
        {
            if (this.currentLogNr > 0)
            {
                this.currentLogNr--;
            }
            this.ReadLines(this.currentLogNr);
        }

        public void ReadLines(int temperaturLogNr)
        {
            if (this.logNames.Count > 0)
            {
                this.CurrentLog = File.ReadAllLines(this.logNames[currentLogNr]).ToList();
            }
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
