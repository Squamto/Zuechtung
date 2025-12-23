// -----------------------------------------------------------------------
// <copyright file="ConfigNames.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Config
{
	/// <summary>
	/// Defines the config names.
	/// </summary>
	public class ConfigNames
	{
		private static string ConfigArea => "Config";

		private static string ConfigBaseArea => ConfigArea + "." + ConfigAreas.Base;

		private static string ConfigSmartlinkArea => ConfigArea + "." + ConfigAreas.Spmartlink;

        private static string ConfigPowerArea => ConfigArea + "." + ConfigAreas.Power;

        private static string ConfigPidArea => ConfigArea + "." + ConfigAreas.Pid;

        private static string ConfigZoneArea => ConfigArea + "." + ConfigAreas.Zone;

        public static string ValDeviceBase(AreaBaseConfig value)
		{
			return ConfigBaseArea + "." + value;
		}

        public static string ValDeviceSmartlink(AreaSmartlinkConfig value)
        {
            return ConfigSmartlinkArea + "." + value;
        }

        public static string ValDeviceSmartlink(AreaSmartlinkConfig value, ZoneNames zone)
        {
            return ConfigSmartlinkArea + "." + value + "." + zone;
        }

        public static string ValDevicePower(AreaPowerConfig value)
        {
            return ConfigPowerArea + "." + value;
        }

        public static string ValDevicePower(AreaPowerConfig value, ZoneNames zone)
        {
            return ConfigPowerArea + "." + value + "." + zone;
        }

        public static string ValDevicePid(AreaPidConfig value, ZoneNames zone)
        {
            return ConfigPidArea + "." + value + "." + zone;
        }

        public static string ValDeviceZone(AreaZoneConfig value, ZoneNames zone)
        {
            return ConfigZoneArea + "." + value + "." + zone.ToString();
        }
    }
}
