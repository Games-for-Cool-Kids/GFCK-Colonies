using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// How to use this class:
// 1. For your class-that-needs-to-handle-input: Inherit from InputResolverStep
// 2. Implement InputResolverStep::ResolveInput() to handle all your input
// 3. Add your class to InputResolver::_orderedStepTypes (prioritized from top to bottom)

// Currently, this class assumes a static, ordered list of inputSteps. 
// In the future, we can consider making this dynamic; adding/removing input behaviours depending on context.
public class InputResolver : MonoBehaviour
{
    private List<System.Type> _orderedStepTypes = new()
    {
        typeof(BuildHand),
        typeof(PlayerHand),
        typeof(RemoveBuildingHand)
    };

    private List<InputResolverStep> _steps = new();

    public void AddSolveableInputStep(InputResolverStep step)
    {
        Debug.Assert(!_steps.Contains(step));
        Debug.Assert(_orderedStepTypes.Contains(step.GetType()));

        var type = step.GetType();
        var priorityIndex = _orderedStepTypes.IndexOf(type);

        if (_steps.Count == 0)
        {
            _steps.Add(step);
            return;
        }

        for (int i = 0; i < _steps.Count; i++)
        {
            var typeOther = _steps[i].GetType();
            var priorityIndexOther = _orderedStepTypes.IndexOf(typeOther);

            if (priorityIndexOther > priorityIndex || i == _steps.Count - 1 )
            {
                _steps.Insert(i, step);
                return;
            }
        }
    }

    public void Update()
    {
        foreach(var step in _steps)
        {
            if(!step.ResolveInput())
            {
                return;
            }
        }
    }
}
