using System;
using System.Collections.Generic;
using System.Numerics;
using instance.id.EATK.Extensions;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK
{
    interface IAnimValueStore { }

    [Serializable]
    public class AnimValueStore<TValue, TStyle> : IAnimValueStore
        where TValue : struct
        where TStyle : struct
    {
        public AnimateElement element;
        [SerializeField] private TValue _initial;
        [SerializeField] private TStyle _final;
        [SerializeField] public int duration;
        [SerializeField] public int delay;
        ValueAnimation<StyleValues> _animation;

        public Action callback;
        public Action reverseCallback;

        public TValue initial
        {
            get => _initial;
            set => _initial = value;
        }

        public TStyle final
        {
            get => _final;
            set => _final = value;
        }

        public ValueAnimation<StyleValues> Animation
        {
            get => _animation;
            set => _animation = value;
        }

        public AnimValueStore() { }
        public AnimValueStore(TValue initial = default, TStyle final = default, int duration = 0, int delay = 0, AnimateElement element = default,
            Action callback = default, Action reverseCallback = default, ValueAnimation<StyleValues> Animation = default)
        {
            Animation ??= new ValueAnimation<StyleValues>();
            this.initial = initial;
            this.final = final;
            this.duration = duration;
            this.delay = delay;
            this.element = element;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
            this.Animation = Animation;
        }

        public AnimValueStore(AnimateElement element = default, TValue initial = default, TStyle final = default, int duration = 0, int delay = 0,
            Action callback = default, Action reverseCallback = default, ValueAnimation<StyleValues> Animation = default)
        {
            Animation ??= new ValueAnimation<StyleValues>();
            this.initial = initial;
            this.final = final;
            this.duration = duration;
            this.delay = delay;
            this.element = element;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
            this.Animation = Animation;
        }
    }

    [Serializable]
    public class AnimValueStore<T> : IAnimValueStore
        where T : struct
    {
        public AnimateElement element;

        [SerializeField] private T _initial;
        [SerializeField] private T _final;
        [SerializeField] public int duration;
        [SerializeField] public int delay;
        ValueAnimation<StyleValues> _animation;

        public Action callback;
        public Action reverseCallback;

        public T initial
        {
            get => _initial;
            set => _initial = value;
        }

        public T final
        {
            get => _final;
            set => _final = value;
        }

        public ValueAnimation<StyleValues> Animation
        {
            get => _animation;
            set => _animation = value;
        }

        public AnimValueStore() { }

        public AnimValueStore(AnimValueStore<T> other)
        {
            initial = other.initial;
            final = other.final;
            duration = other.duration;
            delay = other.delay;
            element = other.element;
            callback = other.callback;
            reverseCallback = other.reverseCallback;
            Animation = other.Animation;
        }

        public AnimValueStore(T initial = default, T final = default, int duration = 0, int delay = 0, AnimateElement element = default,
            Action callback = default, Action reverseCallback = default, ValueAnimation<StyleValues> Animation = default)
        {
            Animation ??= new ValueAnimation<StyleValues>();
            this.initial = initial;
            this.final = final;
            this.duration = duration;
            this.delay = delay;
            this.element = element;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
            this.Animation = Animation;
        }

        public AnimValueStore(AnimateElement element = default, T initial = default, T final = default,
            int duration = 0, int delay = 0, Action callback = default, Action reverseCallback = default, ValueAnimation<StyleValues> Animation = default)
        {
            Animation ??= new ValueAnimation<StyleValues>();
            this.initial = initial;
            this.final = final;
            this.duration = duration;
            this.delay = delay;
            this.element = element;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
            this.Animation = Animation;
        }
    }

    // --| Basic -----------------------------------------------
    [Serializable]
    public class AnimValueStore : IAnimValueStore
    {
        [SerializeField] private float _initial;
        [SerializeField] private float _final;
        [SerializeField] public int duration;
        [SerializeField] public int delay;
        [SerializeField] private Easing _easing;
        ValueAnimation<StyleValues> _animation;
        private Func<float, float> _easingFunc;
        [SerializeField] public bool animEnabled = true;
        
        public Func<float, float> easing
        {
            set => _easingFunc = value;
            get => _easingFunc ?? GetEasing.Get(_easing);
        }

        public Action callback;
        public Action reverseCallback;

        public float initial
        {
            get => _initial == 0 ? 0.Zero() : _initial;
            set => _initial = value;
        }

        public float final
        {
            get => _final == 0 ? 0.Zero() : _final;
            set => _final = value;
        }

        public ValueAnimation<StyleValues> Animation
        {
            get => _animation;
            set => _animation = value;
        }

        public Dictionary<string, float> userData = new Dictionary<string, float>();

        public AnimValueStore() { }
        public AnimValueStore(AnimValueStore other)
        {
            initial = other.initial;
            final = other.final;
            duration = other.duration;
            delay = other.delay;
            callback = other.callback;
            reverseCallback = other.reverseCallback;
            Animation = other.Animation;
            userData = other.userData;
        }
        
        public AnimValueStore(
            int duration = default,
            int delay = default, 
            Easing easing = default,
            Action callback = default,
            Action reverseCallback = default,
            ValueAnimation<StyleValues> Animation = default)
        {
            Animation ??= new ValueAnimation<StyleValues>();
            this.initial = default;
            this.final = default;
            this.duration = duration;
            this.delay = delay;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
            this.Animation = Animation;
            this._easing = easing;
        }

        public AnimValueStore(
            float initial = default,
            float final = default,
            int duration = 0,
            int delay = 0, 
            Action callback = default,
            Action reverseCallback = default,
            ValueAnimation<StyleValues> Animation = default, 
            Func<float, float> easing = default)
        {
            Animation ??= new ValueAnimation<StyleValues>();
            this.initial = initial;
            this.final = final;
            this.duration = duration;
            this.delay = delay;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
            this.Animation = Animation;
            _easingFunc = easing;
        }
        
        public AnimValueStore(
            float initial,
            float final,
            int duration,
            int delay, 
            Easing easing,
            Action callback = default,
            Action reverseCallback = default,
            ValueAnimation<StyleValues> Animation = default)
        {
            Animation ??= new ValueAnimation<StyleValues>();
            this.initial = initial;
            this.final = final;
            this.duration = duration;
            this.delay = delay;
            this.callback = callback;
            this.reverseCallback = reverseCallback;
            this.Animation = Animation;
            this._easing = easing;
        }

        public void AddStyleValue(string name, float styleValue) => userData.Add(name, styleValue);
        public float GetStyleValue(string name) => userData.TryGetVal(name);

    }
}
