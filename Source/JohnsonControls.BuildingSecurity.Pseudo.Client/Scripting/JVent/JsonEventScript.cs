/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent
{
    public class JsonEventScript : IOrderedEnumerable<JsonEvent>
    {
        private readonly List<JsonEvent> _events;  
        public TimeSpan InitialDelay { get; private set; }
        public string RawScript { get; private set; }
        public JsonEventScript(string script)
        {
            RawScript = script;
            var parser = JObject.Parse(script);

            InitialDelay = TimeSpan.Parse(((string)parser["delayStart"]) ?? "0");
            var tempList = (from e in parser["events"] let at = TimeSpan.Parse((string) e["at"]) select new JsonEvent(at, (string) e["name"], e["value"].ToString()));
            _events = tempList.OrderBy(x=>x.At).ToList();
        }

        public IOrderedEnumerable<JsonEvent> CreateOrderedEnumerable<TKey>(Func<JsonEvent, TKey> keySelector, IComparer<TKey> comparer, bool @descending)
        {
            return @descending ? _events.ToList().OrderByDescending(keySelector, comparer) : _events.ToList().OrderBy(keySelector, comparer);
        }

        public IEnumerator<JsonEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
