using System;
using System.Collections.Generic;
using System.Text;

namespace PulseFlow.Interfaces
{
    public interface IMachineControlService
    {
        Task<bool> StopMachineAsync(string machineName);
        Task<bool> StartMachineAsync(string machineName);
    }
}
