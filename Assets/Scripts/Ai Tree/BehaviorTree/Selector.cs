namespace Ai_Tree
{
    public class Selector : BTNode
    {
        private BTNode[] nodes;

        public Selector(BTNode[] nodes)
        {
            this.nodes = nodes;
        }

        public override bool Evaluate()
        {
            foreach (var node in nodes)
            {
                if (node.Evaluate())
                    return true;

               
            }
            return false;
        }
    }
}