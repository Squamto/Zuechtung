// -----------------------------------------------------------------------
// <copyright file="Smartlink.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.Smartlink
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Model.SerialPort;

    public class Smartlink
    {
        private SerialPortReconnector serialPort;
        private double[] values;
        private bool measInBackgroundIsRunning;
        private bool requestStop;
        private string[] types;
        private int numberOfMeasurementsafterStart;

        public Smartlink(int port, string[] types)
        {
            this.serialPort = new SerialPortReconnector(port);
            this.serialPort.SerialPortAlarmOccured += this.OnSerialPortAlarmOccured;
            this.serialPort.CompleteReinitNeeded += this.OnSerialPortCompleteReinitNeeded;
            this.values = new double[6];
            this.types = types;
        }

        public event EventHandler<bool> AlarmOccured;

        public event EventHandler CompleteReinitNeeded;

        public bool IsRunning => this.measInBackgroundIsRunning;

        public bool IsInitialized { get; private set; }

        public void Init()
        {
            try
            {
                this.serialPort.Open();
                this.serialPort.WriteLine("*IDN?");
                string read = this.serialPort.ReadLine();
                if (!read.Contains("Keithley Network Meas. Model KNM-TC42"))
                {
                    Global.UserMsg($"Serial port {this.serialPort.PortName} fail response to command \"IDN?\" Response:{read}");
                    return;
                }
                this.WriteConfig(":Config 1 VDC AUTO DIFF ~1");
                this.WriteConfig(":Config 2 VDC AUTO DIFF ~2");
                this.WriteConfig(":Config 3 VDC AUTO DIFF ~3");
                this.WriteConfig(":Config 4 VDC AUTO DIFF ~4");
                this.WriteConfig(":Config 5 VDC AUTO DIFF ~5");
                this.WriteConfig(":Config 6 VDC AUTO DIFF ~6");
                this.WriteConfig(":Config:Data:Fields Read&Chan_Tag");
                this.WriteConfig(":Limits 1 On");
                this.WriteConfig(":Limits 2 On");
                this.WriteConfig(":Limits 3 On");
                this.WriteConfig(":Limits 4 On");
                this.WriteConfig(":Limits 5 On");
                this.WriteConfig(":Limits 6 On");
                this.WriteConfig(":Config:Meas:Average 1");
                this.WriteConfig(":Filter:Dig 1 On");
                this.WriteConfig(":Filter:Dig 2 On");
                this.WriteConfig(":Filter:Dig 3 On");
                this.WriteConfig(":Filter:Dig 4 On");
                this.WriteConfig(":Filter:Dig 5 On");
                this.WriteConfig(":Filter:Dig 6 On");
                this.WriteConfig(":Config:Meas:Trig Immediate");

                //command = "sav";
                //this.serialPort.WriteLine(command);
                //read = this.serialPort.ReadLine();
                //this.CheckResponse(read, command);

                this.serialPort.Close();
            }
            catch (Exception exception)
            {
                Global.UserMsg(new Exception($"Exception on Smartlink Port: {this.serialPort.PortName}", exception));
            }
        }
        public void Stop()
        {
            this.requestStop = true;
        }

        public void Start()
        {
            if (this.measInBackgroundIsRunning)
            {
                return;
            }
            this.measInBackgroundIsRunning = true;
            this.numberOfMeasurementsafterStart = 0;
            this.serialPort.Open();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        if (this.requestStop)
                        {
                            return;
                        }
                        this.Meas();
                        if (this.numberOfMeasurementsafterStart > 3)
                        {
                            this.IsInitialized = true;
                        }
                        else
                        {
                            this.numberOfMeasurementsafterStart++;
                        }
                        Thread.Sleep(5);
                    }
                }
                catch (Exception exception)
                {
                    Global.UserMsg(new Exception($"Exception on Smartlink Port: {this.serialPort.PortName}", exception));
                }
                finally
                {
                    this.serialPort.Close();
                    this.requestStop = false;
                    this.measInBackgroundIsRunning = false;
                }
            });
        }

        public double GetValue(int channel)
        {
            if ((channel < 1) || (channel > 6))
            {
                throw new Exception($"Smartlink Port: {this.serialPort.PortName} GetValue with not expected channel: {channel}");
            }

            return this.values[channel - 1];
        }

        private void Meas()
        {
            if (this.serialPort.CheckWaitForReconnect())
            {
                return;
            }

            string command = $":Meas? 1,2,3,4,5,6";
            string read = string.Empty;
            try
            {
                this.serialPort.WriteLine(command);
                read = this.serialPort.ReadLine() + "$";
                read += this.serialPort.ReadLine() + "$";
                read += this.serialPort.ReadLine() + "$";
                read += this.serialPort.ReadLine() + "$";
                read += this.serialPort.ReadLine() + "$";
                read += this.serialPort.ReadLine() + "$";
                read += this.serialPort.ReadLine() + "$";
                string existing = this.serialPort.ReadExisting();
                if (this.CheckResponse(command, read))
                {
                    read = read.Replace("=>$", string.Empty);
                    List<string> parts = read.Split(new char[] { '$' }).ToList();
                    int i = 0;
                    foreach (string part in parts)
                    {
                        // Wenn die Temperatur < 3.7E-5 wert mit 2702702.702703 multiplizieren
                        // sonst:
                        // double rawTemp = this.ReadValue(part);
                        // double temp = Math.Round((Math.Pow(Math.E, (3.157246657 + 0.411621711 * Math.Log((rawTemp * 1000000) - 3))) * 1.1) * 1000) / 1000;
                        // Wenn der Tag = ~ dann einfach temp = this.ReadValue(part); (vermutlich ist das im normalen Themoelementmode der Fall)
                        // immer den Tempertaur offset dazuaddieren
                        // wenn alle Temperaturen über 360 Grad sind, dann wird auf Thermoelementmode umgeschaltet.
                        // mit der Ausgabe an den Smartlink: Config 1 Temp TC B (1 ist der Kanal und B ist der Konfigurierbare Thermoelementtyp (B oder S)
                        // danach muss eine Sekunde Pause sein, bevor weiter gemessen wird
                        if ((i >= 0) && (i < values.Length))
                        {
                            double t = this.ReadValue(part);
                            if (part.Contains("~"))
                            {
                                if ((t < -3000.0) || (t > 3000.0))
                                {
                                    t = -0.123;
                                }
                                else
                                {
                                    if (this.types[i].Contains("S"))
                                    {
                                        t = (t * 1000) * 200;
                                        if (t > 120)
                                        {
                                            this.WriteConfig($":Config {i+1} Temp TC S OpenTCOn @{i+1}");
                                        }
                                    }
                                    if (this.types[i].Contains("B"))
                                    {
                                        if (t < 3.7E-5)
                                        {
                                            t = t * 2702702.702703;
                                        }
                                        else
                                        {
                                            t = (Math.Round((Math.Pow(Math.E, (3.157246657 + 0.411621711 * Math.Log((t * 1000000) - 3))) * 1.1) * 1000) / 1000) - 9.0;
                                            if (t > 360)
                                            {
                                                this.WriteConfig($":Config {i + 1} Temp TC B OpenTCOn @{i + 1}");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this.types[i].Contains("S"))
                                {
                                    if (t < 110)
                                    {
                                        this.WriteConfig($":Config {i + 1} VDC AUTO DIFF ~{i + 1}");
                                    }
                                }
                                if (this.types[i].Contains("B"))
                                {
                                    if (t < 350)
                                    {
                                        this.WriteConfig($":Config {i + 1} VDC AUTO DIFF ~{i + 1}");
                                    }
                                }
                            }
                            this.values[i] = t;
                        }
                        i++;
                    }
                }
            }
            catch (Exception exception)
            {
                Global.UserMsg(new Exception($"Error in measurement on serial port Port:{this.serialPort.PortName} Comand: {command} Response:{read}", exception));
            }
        }

        public void InjectError()
        {
            this.serialPort.ErrorInjection();
        }

        private double ReadValue(string value)
        {
            string[] parts = value.Split(new char[] { ' ' });
            if (parts.Length > 1)
            {
                double rv = double.Parse(parts[0].ToUpper(), CultureInfo.InvariantCulture);
                return rv;
            }
            return -0.456;
        }

        private void WriteConfig(string command)
        {
            this.serialPort.ReadExisting();
            this.serialPort.WriteLine(command);
            string prompt = this.serialPort.ReadLine();
            string response = this.serialPort.ReadLine();
            this.CheckResponse(command, response, prompt);
        }
       
        private bool CheckResponse(string command, string prompt, string response = null)
        {
            if (!string.IsNullOrEmpty(response) && (response.ToUpper() != command.ToUpper()))
            {
                Global.UserMsg($"Serial port {this.serialPort.PortName} fail response to Command:{command} Response:{prompt}");
                return false;
            }
            if (prompt.Contains("->") || prompt.Contains("=>"))
            {
                return true;
            }
            else
            {
                Global.UserMsg($"Serial port {this.serialPort.PortName} fail response to Command:{command} Response:{prompt}");
                return false;
            }
        }

        private void OnSerialPortAlarmOccured(object? sender, bool e)
        {
            this.AlarmOccured?.Invoke(sender, e);
        }

        private void OnSerialPortCompleteReinitNeeded(object? sender, EventArgs e)
        {
            this.CompleteReinitNeeded?.Invoke(sender, e);
        }
    }
}
