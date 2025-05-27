using Common.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.Producers
{
    public interface IEventProducer
    {
        Task ProduceAsync(string topic, BaseEvent @event);
    }
}
