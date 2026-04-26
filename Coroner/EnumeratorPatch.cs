using System;
using System.Collections;

#nullable enable

namespace Coroner
{
    
    class EnumeratorPatch : IEnumerable
    {
        // The underlying enumerator that we are patching.
        public IEnumerator targetEnumerator;

        // (Optional) The action to perform before the first step.
        public Action? prefixAction;
        // (Optional) The action to perform after the last step.
        public Action? postfixAction;

        // (Optional) The action to perform before each step, with the current value as a parameter.
        public Action<object?>? preStepAction;
        // (Optional) The action to perform after each step, with the current value as a parameter. Not affected by mutateValueFunc.
        public Action<object?>? postStepAction;

        // (Optional) A function to mutate the value from each step before returning it.
        public Func<object?, object?>? stepAction;

        public EnumeratorPatch(IEnumerator targetEnumerator) {
            this.targetEnumerator = targetEnumerator;
        }

        public IEnumerator GetEnumerator() {
            if (prefixAction != null) prefixAction();
            while (targetEnumerator.MoveNext()) {
                var value = targetEnumerator.Current;
                
                if (preStepAction != null) preStepAction(value);
                yield return stepAction != null ? stepAction(value) : value;
                if (postStepAction != null) postStepAction(value);
            }
            if (postfixAction != null) postfixAction();
        }
    }
}