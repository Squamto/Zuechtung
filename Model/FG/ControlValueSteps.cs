// -----------------------------------------------------------------------
// <copyright file="ControlValueSteps.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.FG
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.VisualBasic.FileIO;

    [Serializable]
    public class ControlValueSteps
    {
        public ControlValueSteps()
        {
            this.Steps = [];
        }

        public ControlValueSteps(string filename)
            : this()
        {
            if ((Path.GetExtension(filename) == ".xml"))
            {
                this.ConvertFromXml(filename);
            }
            else
            {
                this.ConvertFromTxt(filename);
            }
        }

        public List<ControlValueStep> Steps { get; set; }

        public void ConvertToXml(string filename)
        {
            StreamWriter? writer = null;
            try
            {
                writer = new StreamWriter(filename, false, Encoding.UTF8);
                XmlSerializer serializer = new(typeof(ControlValueSteps));
                serializer.Serialize(writer, this);
            }
            finally
            {
                writer?.Close();
            }
        }

        public void ConvertToTxt(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            foreach (ControlValueStep step in this.Steps)
            {
                string line = step.Cycles.ToString() + "\t";
                line += step.Zone1 + "\t";
                line += step.Zone2 + "\t";
                line += step.Zone3 + "\t";
                line += step.Zone4 + "\t";
                line += step.Zone5 + "\t";
                line += step.Zone6 + "\t";
                line += step.Zone7 + "\r\n";
                File.AppendAllText(filename, line);
            }
        }


        private void ConvertFromXml(string filename)
        {
            XmlTextReader? reader = null;
            try
            {
                reader = new XmlTextReader(filename);
                XmlSerializer serializer = new(typeof(ControlValueSteps));
                ControlValueSteps? steps = serializer.Deserialize(reader) as ControlValueSteps;
                this.Steps = steps.Steps;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            finally
            {
                reader?.Close();
            }
        }

        private void ConvertFromTxt(string filename)
        {
            string txt = File.ReadAllText(filename);
            this.Steps.Clear();
            List<string> lines = [.. txt.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries)];
            foreach(string line in lines)
            {
                List<string> entries = [.. line.Split(["\t"], StringSplitOptions.RemoveEmptyEntries)];
                if (entries.Count >= 8)
                {
                    ControlValueStep step = new();
                    int cycle = int.Parse(entries[0]);
                    step.Cycles = cycle;
                    double val = double.Parse(entries[1]);
                    step.Zone1 = val;
                    val = double.Parse(entries[2]);
                    step.Zone2 = val;
                    val = double.Parse(entries[3]);
                    step.Zone3 = val;
                    val = double.Parse(entries[4]);
                    step.Zone4 = val;
                    val = double.Parse(entries[5]);
                    step.Zone5 = val;
                    val = double.Parse(entries[6]);
                    step.Zone6 = val;
                    val = double.Parse(entries[7]);
                    step.Zone7 = val;
                    this.Steps.Add(step);
                }
            }
        }
    }
}
