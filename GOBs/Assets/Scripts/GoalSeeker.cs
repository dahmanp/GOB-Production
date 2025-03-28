using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalSeeker : MonoBehaviour
{
    Goal[] mGoals;
    Action[] mActions;
    Action mChangeOverTime;
    const float TICK_LENGTH = 5.0f;
    public TMP_Text stats;
    public TMP_Text action;
    public TMP_Text lowTitle;

    void Start()
    {
        mGoals = new Goal[3];
        mGoals[0] = new Goal("Eat", 4);
        mGoals[1] = new Goal("Sleep", 3);
        mGoals[2] = new Goal("Bathroom", 3);

        mActions = new Action[3];
        mActions[0] = new Action("Eat Snack");
        mActions[0].targetGoals.Add(new Goal("Eat", -2f));
        mActions[0].targetGoals.Add(new Goal("Sleep", -1f));
        mActions[0].targetGoals.Add(new Goal("Bathroom", +1f));

        mActions[1] = new Action("Sleep In Bed");
        mActions[1].targetGoals.Add(new Goal("Eat", +2f));
        mActions[1].targetGoals.Add(new Goal("Sleep", -4f));
        mActions[1].targetGoals.Add(new Goal("Bathroom", +2f));

        mActions[2] = new Action("Go To Bathroom");
        mActions[2].targetGoals.Add(new Goal("Eat", 0f));
        mActions[2].targetGoals.Add(new Goal("Sleep", 0f));
        mActions[2].targetGoals.Add(new Goal("Bathroom", -4f));

        mChangeOverTime = new Action("tick");
        mChangeOverTime.targetGoals.Add(new Goal("Eat", +4f));
        mChangeOverTime.targetGoals.Add(new Goal("Sleep", +1f));
        mChangeOverTime.targetGoals.Add(new Goal("Bathroom", +2f));

        //Debug.Log("1 hour will pass every " + TICK_LENGTH + " seconds.");
        lowTitle.text = "1 hour will pass every " + TICK_LENGTH + " seconds.";
        InvokeRepeating("Tick", 0f, TICK_LENGTH);
        //Debug.Log("Hit E to do something.");
    }

    void Tick()
    {
        foreach (Goal goal in mGoals)
        {
            goal.value += mChangeOverTime.GetGoalChange(goal);
            goal.value = Mathf.Max(goal.value, 0);
        }
        PrintGoals();
    }

    void PrintGoals()
    {
        string goalString = "";
        foreach (Goal goal in mGoals)
        {
            goalString += goal.name + ": " + goal.value + "; \n";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        stats.text = goalString;
        //Debug.Log(goalString);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Action bestAction = ChooseAction(mActions, mGoals);
            //Debug.Log("I think I will " + bestAction.name);
            action.text = "I think I will " + bestAction.name;

            foreach (Goal goal in mGoals)
            {
                goal.value += bestAction.GetGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
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