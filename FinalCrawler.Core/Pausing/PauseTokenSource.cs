using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalCrawler.Core.Pausing
{
    public class PauseTokenSource
    {
        private volatile TaskCompletionSource<bool> m_paused;
        internal static readonly Task s_completedTask = Task.FromResult(true);

        public bool IsPaused
        {
            get { return m_paused != null; }
            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(
                        ref m_paused, new TaskCompletionSource<bool>(), null);
                }
                else
                {
                    while (true)
                    {
                        var tcs = m_paused;
                        if (tcs == null) return;
                        if (Interlocked.CompareExchange(ref m_paused, null, tcs) == tcs)
                        {
                            tcs.SetResult(true);
                            break;
                        }
                    }
                }
            }
        }
        public PauseToken Token => new PauseToken(this);

        internal Task WaitWhilePausedAsync()
        {
            var cur = m_paused;
            return cur != null ? cur.Task : s_completedTask;
        }
    }
}
