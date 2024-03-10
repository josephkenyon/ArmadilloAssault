using ArmadilloAssault.Configuration.Scenes;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Environment.Flows
{
    public class FlowManager
    {
        public FlowJson FlowConfiguration { get; private set; }
        public List<Flow> Flows { get; private set; }

        public FlowManager(FlowJson flowJson)
        {
            FlowConfiguration = flowJson;
            Flows = [];

            if (flowJson != null)
            {
                AddNewFlow(0);
                AddNewFlow(FlowConfiguration.Size.X * 2);
            }
        }

        public void UpdateFlows()
        {
            Flows.RemoveAll(flow => flow.X > 1920);

            if (Flows.All(flow => flow.X >= 0) && FlowConfiguration != null)
            {
                AddNewFlow(-FlowConfiguration.Size.X * 2);
            }

            foreach (var flow in Flows)
            {
                flow.X++;
            }
        }

        private void AddNewFlow(int x)
        {
            Flows.Add(new Flow(FlowConfiguration, x));
        }
    }
}
