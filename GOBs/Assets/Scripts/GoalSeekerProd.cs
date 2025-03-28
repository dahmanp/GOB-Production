using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalSeekerProd : MonoBehaviour
{
    Goal[] mGoals;
    Action[] mActions;
    Action mChangeOverTime;
    const float TICK_LENGTH = 5.0f;
    public TMP_Text stats;
    public TMP_Text action;
    public TMP_Text lowTitle;
    public GameObject[] wolves;

    public Animator _animator;

    void Start()
    {
        mGoals = new Goal[5];
        mGoals[0] = new Goal("Hunger", 4);
        mGoals[1] = new Goal("Exhaustion", 3);
        mGoals[2] = new Goal("Cleanliness", 3);
        mGoals[3] = new Goal("Social", 4);
        mGoals[4] = new Goal("Enrichment", 5);

        mActions = new Action[7];
        mActions[0] = new Action("hunt for food alone");
        mActions[0].targetGoals.Add(new Goal("Hunger", -2f));
        mActions[0].targetGoals.Add(new Goal("Exhaustion", -1f));
        mActions[0].targetGoals.Add(new Goal("Cleanliness", +1f));
        mActions[0].targetGoals.Add(new Goal("Social", 0f));
        mActions[0].targetGoals.Add(new Goal("Enrichment", -1f));
        mActions[0].animation = "eat";

        mActions[1] = new Action("nap in the grass");
        mActions[1].targetGoals.Add(new Goal("Hunger", +2f));
        mActions[1].targetGoals.Add(new Goal("Exhaustion", -4f));
        mActions[1].targetGoals.Add(new Goal("Cleanliness", +2f));
        mActions[1].targetGoals.Add(new Goal("Social", 0f));
        mActions[1].targetGoals.Add(new Goal("Enrichment", +1f));
        mActions[1].animation = "sleep";
        mActions[1].pack = false;

        mActions[2] = new Action("groom my fur.");
        mActions[2].targetGoals.Add(new Goal("Hunger", 0f));
        mActions[2].targetGoals.Add(new Goal("Exhaustion", 0f));
        mActions[2].targetGoals.Add(new Goal("Cleanliness", -4f));
        mActions[2].targetGoals.Add(new Goal("Social", -2f));
        mActions[2].targetGoals.Add(new Goal("Enrichment", +1f));
        mActions[2].animation = "sleep";
        mActions[2].pack = false;

        mActions[3] = new Action("play with the pack.");
        mActions[3].targetGoals.Add(new Goal("Hunger", +1f));
        mActions[3].targetGoals.Add(new Goal("Exhaustion", -1f));
        mActions[3].targetGoals.Add(new Goal("Cleanliness", -1f));
        mActions[3].targetGoals.Add(new Goal("Social", -4f));
        mActions[3].targetGoals.Add(new Goal("Enrichment", -1f));
        mActions[3].animation = "run";
        mActions[3].pack = true;

        mActions[4] = new Action("hunt for food with the pack.");
        mActions[4].targetGoals.Add(new Goal("Hunger", -2f));
        mActions[4].targetGoals.Add(new Goal("Exhaustion", -1f));
        mActions[4].targetGoals.Add(new Goal("Cleanliness", +1f));
        mActions[4].targetGoals.Add(new Goal("Social", -2f));
        mActions[4].targetGoals.Add(new Goal("Enrichment", -1f));
        mActions[4].animation = "eat";
        mActions[4].pack = true;

        mActions[5] = new Action("sunbathe with the pack.");
        mActions[5].targetGoals.Add(new Goal("Hunger", +2f));
        mActions[5].targetGoals.Add(new Goal("Exhaustion", -4f));
        mActions[5].targetGoals.Add(new Goal("Cleanliness", +1f));
        mActions[5].targetGoals.Add(new Goal("Social", -3f));
        mActions[5].targetGoals.Add(new Goal("Enrichment", +1f));
        mActions[5].animation = "sleep";
        mActions[5].pack = true;

        mActions[6] = new Action("smell new scents around the forest.");
        mActions[6].targetGoals.Add(new Goal("Hunger", +1f));
        mActions[6].targetGoals.Add(new Goal("Exhaustion", +2f));
        mActions[6].targetGoals.Add(new Goal("Cleanliness", +1f));
        mActions[6].targetGoals.Add(new Goal("Social", 0f));
        mActions[6].targetGoals.Add(new Goal("Enrichment", -4f));
        mActions[6].animation = "walk";
        mActions[6].pack = false;

        mChangeOverTime = new Action("tick");
        mChangeOverTime.targetGoals.Add(new Goal("Hunger", +4f));
        mChangeOverTime.targetGoals.Add(new Goal("Exhaustion", +1f));
        mChangeOverTime.targetGoals.Add(new Goal("Cleanliness", +2f));
        mChangeOverTime.targetGoals.Add(new Goal("Social", +3f));
        mChangeOverTime.targetGoals.Add(new Goal("Enrichment", +3f));

        lowTitle.text = "1 hour will pass every " + TICK_LENGTH + " seconds.";
        InvokeRepeating("Tick", 0f, TICK_LENGTH);
    }

    void Tick()
    {
        foreach (Goal goal in mGoals)
        {
            goal.value += mChangeOverTime.GetGoalChange(goal);
            goal.value = Mathf.Max(goal.value, 0);
        }
        _animator.Play("breathe");
        action.text = "Idle.";
        foreach (GameObject wolf in wolves)
        {
            wolf.SetActive(false);
        }
        PrintGoals();
    }

    void PrintGoals()
    {
        string goalString = "";
        foreach (Goal goal in mGoals)
        {
            goalString += goal.name + ": " + goal.value + "\n";
        }
        goalString += "\nDiscontentment: " + CurrentDiscontentment();
        stats.text = goalString;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Action bestAction = ChooseAction(mActions, mGoals);
            action.text = "I think I will " + bestAction.name;

            foreach (Goal goal in mGoals)
            {
                goal.value += bestAction.GetGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
            }
            _animator.Play(bestAction.animation);
            if (bestAction.pack)
            {
                foreach (GameObject wolf in wolves)
                {
                    wolf.SetActive(true);
                    Animator anim = wolf.GetComponent<Animator>();
                    anim.Play(bestAction.animation);
                }
            } else
            {
                foreach (GameObject wolf in wolves)
                {
                    wolf.SetActive(false);
                }
            }
            PrintGoals();
        }
    }

    Action ChooseAction(Action[] actions, Goal[] goals)
    {
        Action bestAction = null;
        float bestValue = float.PositiveInfinity;

        foreach (Action action in actions)
        {
            float thisValue = Discontentment(action, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = action;
            }
        }
        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        float discontentment = 0f;

        foreach (Goal goal in goals)
        {
            float newValue = goal.value + action.GetGoalChange(goal);
            newValue = Mathf.Max(newValue, 0);
            discontentment += goal.GetDiscontentment(newValue);
        }
        return discontentment;
    }

    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach (Goal goal in mGoals)
        {
            total += (goal.value * goal.value);
        }
        return total;
    }
}