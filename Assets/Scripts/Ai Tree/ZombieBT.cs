using Ai_Tree;
using UnityEngine;
using Mirror; // Добавлено

public class NetworkZombieBT : EnemyBase
{
    private BTNode rootNode;

    protected override void Start()
    {
        base.Start();

        if (!isServer) return; // AI дерево только на сервере

        PatrolTask patrol = new PatrolTask(this);
        ChaseTask chase = new ChaseTask(this);
        AttackTask attack = new AttackTask(this);

        rootNode = new Selector(new BTNode[] { attack, chase, patrol });
    }

    private void Update()
    {
        if (!isServer) return; // Только сервер управляет NPC
        rootNode.Evaluate();
    }
}