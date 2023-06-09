using System;
using System.Collections;
using System.Collections.Generic;

namespace instance.id.EATK
{
    public sealed class SingleTask : ITaskSequence
    {
        private readonly List<Action<Action>> m_list = new List<Action<Action>>();
        private readonly bool m_isReuse;
        private bool m_isPlaying;

        public SingleTask() : this(false) { }
        public SingleTask(bool isReuse) => m_isReuse = isReuse;

        public void Add(string text, Action<Action> task)
        {
            if (task == null || m_isPlaying) return;
            m_list.Add(task);
        }

        public void Run(string text, Action onCompleted)
        {
            if (m_list.Count <= 0)
            {
                onCompleted?.Invoke();
                return;
            }

            int count = 0;

            Action task = null;
            task = () =>
            {
                if (m_list.Count <= count)
                {
                    m_isPlaying = false;

                    if (!m_isReuse) m_list.Clear();
                    onCompleted?.Invoke();
                    return;
                }

                Action nextTask = task;
                m_list[count++](() =>
                {
                    if (nextTask == null) return;
                    nextTask();
                    nextTask = null;
                });
            };
            m_isPlaying = true;
            task();
        }

        // --------------------------------------- GetEnumerator
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
