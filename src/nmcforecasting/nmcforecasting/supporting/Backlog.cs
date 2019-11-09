using System.Collections.Generic;
using System.Linq;

namespace nmcforecasting.supporting
{
    /*
     * Simulate how a number of backlog items get worked on by 1 or more resources.
     * On each resource items are worked on sequentially, whenever the resource is free again.
     * If more than one resource is employed, backlog items get distributed in a round robin
     * fashion to the least busy (idle) resource (always starting from the beginning of the
     * resource list.)
     *
     * Example:
     *     Issues with cycle times: 2, 7, 3, 4, 1, 5
     *     resource #1: 2, 4
     *     resource #2: 7
     *     resource #3: 3, 1, 5
     *     Delivery time: 9 = 3+1+5, resource #3 is "slowest".
     */
    class Backlog
    {
        private readonly int[] _itemCycleTimes;
        
        public Backlog(int[] itemCycleTimes) { _itemCycleTimes = itemCycleTimes; }

        
        public int CalculateDeliveryTime(int numberOfResources) {
            var resources = new int[numberOfResources];
            
            var issues = new Queue<int>(_itemCycleTimes);
            while (issues.Any()) {
                var least_busyness = resources.Min();
                var i_resource_with_least_busyness = 0;
                while (resources[i_resource_with_least_busyness] != least_busyness)
                    i_resource_with_least_busyness++;
                resources[i_resource_with_least_busyness] += issues.Dequeue();
            }
            
            return resources.Max();
        }
    }
}