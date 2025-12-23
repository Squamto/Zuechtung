// -----------------------------------------------------------------------
// <copyright file="ISamplerFile.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.Log
{
    public interface ISampler
    {
        List<string> CurrentLog { get; }

        void Refresh();

        string LogStart { get; }

        string LogEnd { get; }

        string LogDuration { get; }

        void ReadNextTemperaturLog();

        void ReadPreviuosTemperaturLog();


        List<double> GetLineValues(string line, int start);

        Tuple<double, double, double> GetLineValuesFromZone(string line, int zone);

        List<List<double>> GetAllValues(int start);

        public List<Tuple<double, double, double>> GetAllValuesFromZone(int zone);

        public List<Tuple<double, double, double, double, double, double, double>> GetValueTable(int valueRow);

    }
}
