using System.Collections.Generic;

namespace Ai_Tree
{
    public class Sequence : BTNode
    {
        private readonly List<BTNode> nodes;

        public Sequence(IEnumerable<BTNode> nodes)
        {
            this.nodes = new List<BTNode>(nodes);
        }

        public override bool Evaluate()
        {
            foreach (var node in nodes)
            {
                if (!node.Evaluate())
                {
                    return false;
                }
            }
            return true;
        }
    }
}