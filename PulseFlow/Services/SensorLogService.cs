using Microsoft.EntityFrameworkCore.Storage;
using PulseFlow.Interfaces;
using PulseFlow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PulseFlow.Services
{
    internal class SensorLogService : IDatabase<SensorLog>
    {
        private readonly PulseFlowDbContext pulseFlowDbContext;

        public SensorLogService(PulseFlowDbContext pulseFlowDbContext)
        {
            this.pulseFlowDbContext = pulseFlowDbContext;
        }

        public void Create(SensorLog entity)
        {
            pulseFlowDbContext.SensorLogs.Add(entity);
            pulseFlowDbContext.SaveChanges();
        }

        public void Delete(int? id)
        {
            var validData = pulseFlowDbContext.SensorLogs.FirstOrDefault(x => x.Id == id);

            if (validData != null)
            {
                this.pulseFlowDbContext.SensorLogs.Remove(validData);
                this.pulseFlowDbContext.SaveChanges();
            
            }
            else
            {
                throw new InvalidOperationException();
            }

        }

        public List<SensorLog> Get()
        {
            return this.pulseFlowDbContext.SensorLogs.ToList();
        }

        public SensorLog? GetDetail(int? id)
        {
            var validData = pulseFlowDbContext.SensorLogs.FirstOrDefault(x => x.Id == id);
            if(validData != null)
            {
                return validData;
            }
            else
            {
                throw new InvalidOperationException();
            }

        }

        public void Update(SensorLog entity)
        {
            this.pulseFlowDbContext.SensorLogs.Update(entity);
            this.pulseFlowDbContext.SaveChanges();
        }
    }
}
