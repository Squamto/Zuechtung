// -----------------------------------------------------------------------
// <copyright file="UIMeasurement.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.Power
{
    using System;
    using System.Globalization;
    using System.IO.Ports;
    using Config;
    using Model.SerialPort;

    public class UIMeasurement
    {
        private readonly SerialPortReconnector serialPort;
        private readonly double[] iIst;
        private readonly double[] uIst;
        private double[] uMessFaktor;
        private double[] uMessOffset;
        private double[] iMessFaktor;
        private double[] iMessOffset;

        public UIMeasurement(SerialPortReconnector serialPort)
        {
            this.serialPort = serialPort;
            this.uIst = new double[7];
            this.iIst = new double[7];
            this.SetConfigValues();
        }

        public event EventHandler NewUIValuesReceived;

        public bool IsExistComPort { get; }

        public double GetIIst(int val)
        {
            return this.iIst[val];
        }

        public double GetUIst(int val)
        {
            return this.uIst[val];
        }

        // Abfrage Spannungs- Stromwerte: #01S1 +CR (#02S2 für den zweiten Schrank)
        // ANtwort: +00.005+00.003+00.003+00.001+00.045+00.034+00.018+00.011 +CR
        public void ReadUI()
        {
            if (this.serialPort.CheckWaitForReconnect())
            {
                return;
            }

            // Read current
            string command = $"#01S1";
            this.serialPort.WriteLine(command);
            string read = this.serialPort.ReadLine();
            Tuple<double[], double[]> values = this.InterpreteReadUI(read);
            this.InsertValues(this.uIst, values.Item1, 0, this.uMessFaktor, uMessOffset);
            this.InsertValues(this.iIst, values.Item2, 0, this.iMessFaktor, iMessOffset);

            command = $"#02S1";
            this.serialPort.WriteLine(command);
            read = this.serialPort.ReadLine();
            values = this.InterpreteReadUI(read);
            this.InsertValues(this.uIst, values.Item1, 1, this.uMessFaktor, this.uMessOffset);
            this.InsertValues(this.iIst, values.Item2, 1, this.iMessFaktor, this.iMessOffset);
            this.NewUIValuesReceived?.Invoke(this, EventArgs.Empty);
        }

        public void SetConfigValues()
        {
            this.uMessFaktor = new double[7]
{
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUFaktor, ZoneNames.Zone1)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUFaktor, ZoneNames.Zone2)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUFaktor, ZoneNames.Zone3)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUFaktor, ZoneNames.Zone4)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUFaktor, ZoneNames.Zone5)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUFaktor, ZoneNames.Zone6)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUFaktor, ZoneNames.Zone7)),
};
            this.uMessOffset = new double[7]
            {
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUOffset, ZoneNames.Zone1)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUOffset, ZoneNames.Zone2)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUOffset, ZoneNames.Zone3)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUOffset, ZoneNames.Zone4)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUOffset, ZoneNames.Zone5)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUOffset, ZoneNames.Zone6)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessUOffset, ZoneNames.Zone7)),
            };
            this.iMessFaktor = new double[7]
            {
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIFaktor, ZoneNames.Zone1)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIFaktor, ZoneNames.Zone2)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIFaktor, ZoneNames.Zone3)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIFaktor, ZoneNames.Zone4)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIFaktor, ZoneNames.Zone5)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIFaktor, ZoneNames.Zone6)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIFaktor, ZoneNames.Zone7)),
            };
            this.iMessOffset = new double[7]
            {
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIOffset, ZoneNames.Zone1)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIOffset, ZoneNames.Zone2)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIOffset, ZoneNames.Zone3)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIOffset, ZoneNames.Zone4)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIOffset, ZoneNames.Zone5)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIOffset, ZoneNames.Zone6)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MessIOffset, ZoneNames.Zone7)),
            };
        }

        private void InsertValues(double[] dest, double[] source, int slot, double[] faktor, double[] offset)
        {
            for (int i = 0; i < 4; i++)
            {
                int pos = PowerModel.APos(slot, i);
                if (pos < 7)
                {
                    double value = source[i] * faktor[pos] + offset[pos];
                    dest[pos] = value;
                }
            }
        }

        private Tuple<double[], double[]> InterpreteReadUI(string v)
        {
            this.CheckResponse(v, ">");
            v = v.TrimStart('>');
            double[] rvU = new double[4];
            double[] rvI = new double[4];
            if (v.Length == 56)
            {
                string[] strings = Enumerable.Range(0, 8).Select(i => v.Substring(i * 7, 7)).ToArray();
                int i = 0;
                foreach (string s in strings)
                {
                    if (i < 4)
                    {
                        rvU[i] = double.Parse(s, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        rvI[i - 4] = double.Parse(s, CultureInfo.InvariantCulture);
                    }
                    i++;
                }
            }
            return new Tuple<double[], double[]>(rvU, rvI);
        }

        private bool CheckResponse(string response, string expected)
        {
            if (response.Contains(expected))
            {
                return true;
            }
            else
            {
                Global.UserMsg($"Serial port {this.serialPort?.PortName} fail response: Expected:{expected} Response:{response}");
                return false;
            }
        }
    }
}
