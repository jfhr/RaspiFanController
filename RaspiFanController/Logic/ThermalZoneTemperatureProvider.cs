using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace RaspiFanController.Logic
{
    /// <summary>
    /// Reads the temperature from the thermal_zone*/temp file in linux.
    /// </summary>
    public class ThermalZoneTemperatureProvider : ITemperatureProvider
    {
        private readonly int _thermalZone;

        public ThermalZoneTemperatureProvider(ILogger<ThermalZoneTemperatureProvider> logger) : this(0, logger) { }

        public ThermalZoneTemperatureProvider(int thermalZone, ILogger<ThermalZoneTemperatureProvider> logger)
        {
            _thermalZone = thermalZone;
            Logger = logger;
        }

        private ILogger<ThermalZoneTemperatureProvider> Logger { get; }

        /// <inheritdoc />
        public (double, string) GetTemperature()
        {
            var fallbackValue = (double.NaN, "#");

            var result = File.ReadAllText($"/sys/class/thermal/thermal_zone{_thermalZone}/temp");

            if (double.TryParse(result, out var millidegreesCelsius))
            {
                return (millidegreesCelsius / 1000, "C");
            }
            else
            {
                Logger.LogDebug($"Could not parse double from '{result}'");
                return fallbackValue;
            }
        }

        /// <inheritdoc />
        public bool IsPlatformSupported()
        {
            var isPlatformSupported = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            Logger.LogDebug($"Is platform supported: {isPlatformSupported}");
            return isPlatformSupported;
        }
    }
}