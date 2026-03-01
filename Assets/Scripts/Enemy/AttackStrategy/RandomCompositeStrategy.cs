using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCompositeStrategy : IAttackStrategy
{
    private Enemy enemy;
    private List<IAttackStrategy> strategies;
    private IAttackStrategy lastStrategy;
    private System.Random random;

    public RandomCompositeStrategy(Enemy enemy, List<IAttackStrategy> strategies)
    {
        this.enemy = enemy;
        this.strategies = strategies;
        this.random = new System.Random();
    }

    public void ExecuteAttack(Enemy enemy)
    {
        IAttackStrategy strategy = GetRandomStrategy();
        lastStrategy = strategy;

        Debug.Log($"[Boss Phase3] 선택된 공격 전략: {strategy.GetType().Name}");
        strategy.ExecuteAttack(enemy);
    }

    private IAttackStrategy GetRandomStrategy()
    {
        if (strategies.Count <= 1) return strategies[0];

        IAttackStrategy selected;
        do
        {
            selected = strategies[random.Next(strategies.Count)];
        } while (selected == lastStrategy);

        return selected;
    }
}
