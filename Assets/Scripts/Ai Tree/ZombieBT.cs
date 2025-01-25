using Ai_Tree;
using UnityEngine;

public class ZombieBT : EnemyBase
{
    private BTNode rootNode;
    
    protected override void Start()
    {
        base.Start();
        
        PatrolTask patrol = new PatrolTask(this);
        ChaseTask chase = new ChaseTask(this);
        AttackTask attack = new AttackTask(this);

        rootNode = new Selector(new BTNode[] { patrol, chase,attack });
    }

    private void Update()
    {
        rootNode.Evaluate();
    }
}