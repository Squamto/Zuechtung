// -----------------------------------------------------------------------
// <copyright file="SamplerBase.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.Log
{
    public abstract class SamplerBase
    {
        public const int STARTTABSETVALUES = 1;
        public const int STARTTABTEMPERATURES = 2;
        public const int STARTTABPOWER = 3;

        public SamplerBase()
        {
            this.CurrentLog = new List<string>();
        }

        public List<string> CurrentLog { get; protected set; }

        public string LogStart
        {
            get
            {
                return this.GetDateTime(0, DateTime.MinValue.ToString());
            }
        }

        public string LogEnd
        {
            get
            {
                return this.GetDateTime(CurrentLog.Count - 1, DateTime.MaxValue.ToString());
            }
        }

        public string LogDuration
        {
            get
            {
                DateTime start = DateTime.Parse(this.LogStart);
                DateTime end = DateTime.Parse(this.LogEnd);
                return (end - start).ToString();
            }
        }

        public List<double> GetLineValues(string line, int start)
        {
            List<double> rv = new List<double>();
            List<string> parts = line.Split('\t').ToList();
            if (parts.Count > 34)
            {
                rv.Add(double.Parse(parts[start]));
                rv.Add(double.Parse(parts[start + 4]));
                rv.Add(double.Parse(parts[start + 8]));
                rv.Add(double.Parse(parts[start + 12]));
                rv.Add(double.Parse(parts[start + 16]));
                rv.Add(double.Parse(parts[start + 20]));
                rv.Add(double.Parse(parts[start + 24]));
            }
            return rv;
        }

        public Tuple<double, double, double> GetLineValuesFromZone(string line, int zone)
        {
            List<string> parts = line.Split('\t').ToList();
            if (parts.Count > 22)
            {
                double setval = double.Parse(parts[STARTTABSETVALUES + (zone - 1) * 3]);
                double temp = double.Parse(parts[STARTTABTEMPERATURES + (zone - 1) * 3]);
                double power = double.Parse(parts[STARTTABPOWER + (zone - 1) * 3]);
                return new Tuple<double, double, double>(setval, temp, power);
            }
            return new Tuple<double, double, double>(double.NaN, double.NaN, double.NaN);
        }

        protected Tuple<double, double, double, double, double, double, double> GetValues(int start, string line)
        {
            List<string> parts = line.Split('\t').ToList();
            if (parts.Count > 22)
            {
                double t1 = double.Parse(parts[start + 0 * 3]);
                double t2 = double.Parse(parts[start + 1 * 3]);
                double t3 = double.Parse(parts[start + 2 * 3]);
                double t4 = double.Parse(parts[start + 3 * 3]);
                double t5 = double.Parse(parts[start + 4 * 3]);
                double t6 = double.Parse(parts[start + 5 * 3]);
                double t7 = double.Parse(parts[start + 6 * 3]);
                return new Tuple<double, double, double, double, double, double, double>(t1, t2, t3, t4, t5, t6, t7);
            }
            return new Tuple<double, double, double, double, double, double, double>(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
        }

        private string GetDateTime(int line, string sub)
        {
            string rv = sub;
            if (this.CurrentLog.Count == 0)
            {
                return rv;
            }

            List<string> parts = this.CurrentLog[line].Split('\t').ToList();
            if (parts.Count > 1)
            {
                string dateTimeStr = parts[0];
                List<string> dtParts = dateTimeStr.Split(' ').ToList();
                string timeStr = dtParts[1];
                string dateStr = dtParts[0];
                rv = dateStr + " " + timeStr;
            }
            return rv;
        }
    }
}
