using System.Collections.Generic;
using System.Linq;

namespace nmcforecasting
{
    class Backlog
    {
        private readonly int[] _itemCycleTimes;
        
        public Backlog(int[] itemCycleTimes) { _itemCycleTimes = itemCycleTimes; }

        
        public int CalculateDeliveryTime(int numberOfResources) {
            var resources = new int[numberOfResources];
            
            // While there are issues to process...
            var issues = new Queue<int>(_itemCycleTimes);
            while (issues.Any()) {
                // ...find the first "idle" resource (ie. the resource with least busyness)...
                var least_busyness = resources.Min();
                var i_resource_with_least_busyness = 0;
                while (resources[i_resource_with_least_busyness] != least_busyness)
                    i_resource_with_least_busyness++;
                // ...and assign it the current issue's cycle time.
                resources[i_resource_with_least_busyness] += issues.Dequeue();
            }
            
            // The delivery time equals the larges sum of cycle times of any resource.
            // It's the resource which worked longest on issues.
            return resources.Max();
        }
    }
}