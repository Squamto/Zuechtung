// -----------------------------------------------------------------------
// <copyright file="AreaPowerConfig.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Config
{
	/// <summary>
	/// Defines the names of the config values of the dc power devices config.
	/// </summary>
	public enum AreaPowerConfig
    {
        ADAMPort,
        PowerSlot,
        PowerChannel,
        PowerUFaktor,
        PowerUOffset,
        MaxPowerOutput,
        MaxPowerOutputTemperature,
        MaxPowerOutputWhen0Grad,
        Wiederstand,
        MessUFaktor,
        MessUOffset,
        MessIFaktor,
        MessIOffset,
    }
}
