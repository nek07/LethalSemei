using Ai_Tree;
using UnityEngine;

public class IzkeserBT : EnemyBase
{
    private BTNode rootNode;
    
    protected override void Start()
    {
        base.Start();
        
        PatrolTask patrol = new PatrolTask(this);
        ChaseTask chase = new ChaseTask(this);
        AttackTask attack = new AttackTask(this);

        rootNode = new Selector(new BTNode[] { attack, chase, patrol });

    }

    private void Update()
    {
        rootNode.Evaluate();
    }
}