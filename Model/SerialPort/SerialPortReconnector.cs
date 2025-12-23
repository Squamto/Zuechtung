// -----------------------------------------------------------------------
// <copyright file="SerialPortReconnector.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.SerialPort
{
    using System;
    using System.Diagnostics;
    using System.IO.Ports;

    public class SerialPortReconnector
    {
        private SerialPort serialPort;
        private DateTime lastRetryReconect;
        private bool isWaitForReconnect;
        private bool isWasAlarm;
        private List<DateTime> lastReconnects;

        public SerialPortReconnector(int port)
        {
            this.lastRetryReconect = DateTime.Now;
            this.serialPort = new SerialPort("COM" + port, 9600, Parity.None, 8, StopBits.One);
            this.serialPort.Handshake = Handshake.None;
            this.serialPort.NewLine = "\r";
            this.serialPort.ReadTimeout = 1000;
            this.serialPort.WriteTimeout = 1000;
            this.serialPort.ErrorReceived += this.OnSerialPortErrorReceived;
            this.lastReconnects = new List<DateTime>();
        }

        public event EventHandler<bool> SerialPortAlarmOccured;

        public event EventHandler CompleteReinitNeeded;

        public bool IsOpen => this.serialPort.IsOpen;

        public string PortName => this.serialPort.PortName;

        public void Open()
        {
            try
            {
                if (this.serialPort.IsOpen)
                {
                    this.serialPort.Close();
                }
                this.serialPort.Open();
            }
            catch (Exception exception)
            {
                this.serialPort.Close();
                this.lastRetryReconect = DateTime.Now;
                this.isWaitForReconnect = true;
                this.isWasAlarm = true;
                this.SerialPortAlarmOccured?.Invoke(this, true);
                Global.UserMsg(new Exception($"Exception on Smartlink Port: {this.serialPort.PortName}", exception));
            }
        }

        public void Close()
        {
            try
            {
                this.serialPort.Close();
            }
            catch
            {
                // do not give message in case of close
            }
        }

        public void WriteLine(string line)
        {
            try
            {
                if (!this.isWaitForReconnect)
                {
                    this.serialPort.WriteLine(line);
                    if (this.isWasAlarm && (DateTime.Now - this.lastRetryReconect) > TimeSpan.FromSeconds(5))
                    {
                        // remove alarm
                        this.SerialPortAlarmOccured?.Invoke(this, false);
                        this.isWasAlarm = false;
                    }
                }
            }
            catch (Exception exception)
            {
                this.serialPort.Close();
                this.lastRetryReconect = DateTime.Now;
                this.isWaitForReconnect = true;
                this.isWasAlarm = true;
                this.SerialPortAlarmOccured?.Invoke(this, true);
                Global.UserMsg(new Exception($"Error when write to serial port Port:{this.serialPort.PortName} Comand: {line}", exception));
            }
        }

        public string ReadLine()
        {
            try
            {
                if (!this.isWaitForReconnect)
                {
                    string line = this.serialPort.ReadLine();
                    if (this.isWasAlarm && (DateTime.Now - this.lastRetryReconect) > TimeSpan.FromSeconds(5))
                    {
                        // remove alarm
                        this.SerialPortAlarmOccured?.Invoke(this, false);
                        this.isWasAlarm = false;
                    }
                    return line;
                }
            }
            catch (Exception exception)
            {
                this.serialPort.Close();
                this.lastRetryReconect = DateTime.Now;
                this.isWaitForReconnect = true;
                this.isWasAlarm = true;
                this.SerialPortAlarmOccured?.Invoke(this, true);
                Global.UserMsg(new Exception($"Error when read from serial port Port:{this.serialPort.PortName}", exception));
            }
            return string.Empty;
        }

        public string ReadExisting()
        {
            try
            {
                if (!this.isWaitForReconnect)
                {
                    return this.serialPort.ReadExisting();
                }
            }
            catch (Exception exception)
            {
                this.serialPort.Close();
                this.lastRetryReconect = DateTime.Now;
                this.isWaitForReconnect = true;
                this.isWasAlarm = true;
                this.SerialPortAlarmOccured?.Invoke(this, true);
                Global.UserMsg(new Exception($"Error when read from serial port Port:{this.serialPort.PortName}", exception));
            }
            return string.Empty;
        }

        public void ErrorInjection()
        {
            this.Close();
        }

        public bool CheckWaitForReconnect()
        {
            if (this.isWaitForReconnect)
            {
                if (this.serialPort.IsOpen)
                {
                    this.serialPort.Close();
                }
                if ((DateTime.Now - this.lastRetryReconect) > TimeSpan.FromSeconds(3))
                {
                    try
                    {  
                        this.serialPort.Open();
                        this.serialPort.ReadExisting();
                        this.lastRetryReconect = DateTime.Now;
                        this.isWaitForReconnect = false;
                        List<DateTime> toRemove = new List<DateTime>();
                        if (this.lastReconnects.Count > 0)
                        {
                            DateTime border = DateTime.Now - TimeSpan.FromMinutes(2);
                            foreach (DateTime t in this.lastReconnects) 
                            { 
                                if (t < border)
                                {
                                    toRemove.Add(t);
                                }
                            }
                            foreach (DateTime t in toRemove)
                            {
                                this.lastReconnects.Remove(t);
                            }
                        }
                        this.lastReconnects.Add(DateTime.Now);
                        if (this.lastReconnects.Count > 4)
                        {
                            this.CompleteReinitNeeded?.Invoke(this, new EventArgs());
                        }
                    }
                    catch 
                    { 
                        // Do not send any message here
                    }
                }
            }
            return this.isWaitForReconnect;
        }

        private void OnSerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Global.UserMsg($"Error on serial port {this.serialPort.PortName} Errortype: {e.EventType}");
        }
    }
}
