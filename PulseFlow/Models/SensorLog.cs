using System;
using System.Collections.Generic;

namespace PulseFlow.Models;

public partial class SensorLog
{
    public int Id { get; set; }

    public string MachineName { get; set; } = null!;

    public double Temperature { get; set; }

    public double Pressure { get; set; }

    public DateTime LoggedAt { get; set; }
}
