// -----------------------------------------------------------------------
// <copyright file="AreaPidConfig.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Config
{
	/// <summary>
	/// Defines the names of the config values of the dc power devices config.
	/// </summary>
	public enum AreaPidConfig
    {
        PidKInWattPerGrad,
        PidK2InWattPerGrad,
        PidTnInSeconds,
        PrefactorInWPerGrad,
        IPartAllowedTemperatureDifference,
        SetpointIncreaseIPartAllowed,
        FilterCoeffSetpointGradient,
    }
}
