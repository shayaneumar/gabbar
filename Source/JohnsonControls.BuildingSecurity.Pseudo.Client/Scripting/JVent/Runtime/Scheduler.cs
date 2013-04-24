/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime
{
    public class Scheduler
    {
        private readonly ConcurrentDictionary<string, Action<DateTime, JsonEvent>> _eventHandlers = new ConcurrentDictionary<string, Action<DateTime, JsonEvent>>();
        private readonly List<Tuple<DateTime, JsonEvent>> _currentSchedule = new List<Tuple<DateTime, JsonEvent>>();
        private readonly Timer _timer;

        public Scheduler() : this(TimeSpan.FromSeconds(0.5))
        {
        }

        public Scheduler(TimeSpan resolution)
        {
            _timer = new Timer(resolution.TotalMilliseconds);
            _timer.Elapsed += OnQuantum;
            _timer.AutoReset = true;
            RegisterEventHandler("MergeInScript", (t, e) => MergeInScript(new JsonEventScript(e.Value)));
            _timer.Start();
        }

        public void RegisterEventHandler(string eventName, Action<DateTime, JsonEvent> handler)
        {
            _eventHandlers.TryAdd(eventName, handler);
        }

        private void OnQuantum(object sender, ElapsedEventArgs e)
        {
            var currentTime =  DateTime.UtcNow;
            IEnumerable<Tuple<DateTime, JsonEvent>> eventsToRun;
            lock (_currentSchedule)
            {
                eventsToRun = _currentSchedule.TakeWhile(x => x.Item1 < currentTime).ToList();
                for (int i = 0; i < eventsToRun.Count(); i++)
                {
                    _currentSchedule.RemoveAt(0);
                }//Remove all events being run in this quantum
            }

            foreach (var scriptEvent in eventsToRun)
            {
                Action<DateTime, JsonEvent> handler;
                if (_eventHandlers.TryGetValue(scriptEvent.Item2.Name, out handler))
                {
                    handler(scriptEvent.Item1, scriptEvent.Item2);
                }
            }
        }

        public void Run(JsonEventScript script)
        {
            if (script.InitialDelay.TotalSeconds <= 0)
            {
                MergeInScript(script);
            }//Schedule script for merge at a later point in time
            else
            {
                lock (_currentSchedule)
                {
                    _currentSchedule.Add(Tuple.Create(DateTime.UtcNow.Add(script.InitialDelay), new JsonEvent(script.InitialDelay, "MergeInScript", script.RawScript)));
                    _currentSchedule.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                }
            }
        }

        private void MergeInScript(IEnumerable<JsonEvent> script)
        {
            var currentTime = DateTime.UtcNow;
            lock (_currentSchedule)
            {
                foreach (var e in script)
                {
                    _currentSchedule.Add(Tuple.Create(currentTime.Add(e.At), e));
                    _currentSchedule.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                }
            }
        }

        public void StopAll()
        {
            lock (_currentSchedule)
            {
                _currentSchedule.Clear();
            }
        }
    }
}
