using Ai_Tree;
using UnityEngine;

public class Babaltas : EnemyBase
{
    private BTNode rootNode;
    [SerializeField] LayerMask layerMask;

    protected override void Start()
    {
        base.Start();
        

        var lookCheck = new IsPlayerLookingTask(this, player, layerMask);
        var idle = new IdleTask(this, player);
        var chase = new ChaseTask(this);
        AttackTask attack = new AttackTask(this);
        
        
        // Если игрок смотрит → idle, иначе → chase
        rootNode = new Selector(new BTNode[]
        {
            new Sequence(new BTNode[] { lookCheck, idle }),
            attack, chase
        });
    }

    private void Update()
    {
        
        rootNode.Evaluate();
    }
}