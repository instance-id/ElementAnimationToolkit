using System;
using instance.id.EATK.Extensions;
using UnityEngine;

namespace instance.id.EATK
{
    [Serializable]
    public class AnimColorStore
    {
        [SerializeField] public Color initial;
        [SerializeField] public Color final;
        [SerializeField] public int duration;
        [SerializeField] public int delay;

        public Action callback;
        public Action reverseCallback;

        public AnimColorStore(Color initial, Color final, int duration, int delay, Action callback = default, Action reverseCallback = default)
        {
            this.initial = initial;
            this.final = final;
            this.duration = duration;
            this.delay = delay;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
        }

        public AnimColorStore(string initial, string final, int duration, int delay, Action callback = default, Action reverseCallback = default)
        {
            this.initial = initial.FromHex();
            this.final = final.FromHex();
            this.duration = duration;
            this.delay = delay;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
        }
    }
}
