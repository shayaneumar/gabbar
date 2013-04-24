/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    public class InMemoryAlarmRepository : IEnumerable<Alarm>
    {
        private readonly Action<Alarm> _callOnUpdate;
        private readonly ConcurrentDictionary<Guid, Alarm> _alarms;
        private readonly ConcurrentDictionary<Guid, List<HistoryEntry>> _responses;
        private readonly Random _rand = new Random();

        public InMemoryAlarmRepository(Action<Alarm> callOnUpdate)
        {
            _callOnUpdate = callOnUpdate;
            _alarms = new ConcurrentDictionary<Guid, Alarm>();
            _responses = new ConcurrentDictionary<Guid, List<HistoryEntry>>();
        }

        public IEnumerator<Alarm> GetEnumerator()
        {
            return _alarms.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CompleteAlarm(Guid id)
        {
            Alarm a;
            if (!_alarms.TryGetValue(id, out a)) return;
            var removedAlarm = new Alarm(a.Id, a.Description, a.PartitionId, a.PartitionName, a.IsPublic, a.MessageDateTime, a.AlarmTypeDescription, a.Category, a.Priority, false, false, a.AlarmState, a.AlarmStateDescription, a.StateDateTime, a.ConditionSequence, a.Site, a.SourceState, a.SourceStateDescription, a.Escalation, a.Instructions, a.IsPublicDescription, a.IsPending, false, a.IsRespondable, true, true);
            Alarm unused;
            _alarms.TryRemove(id, out unused);
            List<HistoryEntry> historyEntries;
            _responses.TryRemove(id, out historyEntries);
            _callOnUpdate(removedAlarm);
        }

        public void CreateAlarm(AlarmData alarm)
        {

            var updatedAlarm = new Alarm(
                id: IdHelpers.GetGuid(alarm.Id),
                description: alarm.Description,
                partitionId: alarm.PartitionId ?? Guid.NewGuid(),
                partitionName: alarm.PartitionName,
                isPublic: alarm.IsPublic ?? true,
                messageDateTime: alarm.MessageDateTime ?? DateTime.UtcNow,
                alarmTypeDescription: alarm.AlarmTypeDescription,
                category: alarm.Category,
                priority: alarm.Priority ?? _rand.Next(0, 255),
                isResponseRequired: alarm.IsResponseRequired ?? true,
                isAcknowledgeRequired: alarm.IsAcknowledgeRequired ?? true,
                alarmState: alarm.AlarmState ?? 2,
                alarmStateDescription: alarm.AlarmStateDescription ?? "Secure",
                stateDateTime: alarm.StateDateTime ?? DateTime.UtcNow,
                conditionSequence: alarm.ConditionSequence ?? 0,
                site: alarm.Site, sourceState: alarm.SourceState ?? 2,
                sourceStateDescription: alarm.SourceStateDescription,
                escalation: alarm.Escalation ?? 0,
                instructions: alarm.Instructions,
                isPublicDescription: alarm.IsPublicDescription,
                isPending: alarm.IsPending ?? true,
                isCompletable: alarm.IsCompletable ?? true,
                isRespondable: alarm.IsRespondable ?? true,
                isRemovable: alarm.IsRemovable ?? true,
                isCompleted: alarm.IsCompleted ?? false);
            _alarms[updatedAlarm.Id] = updatedAlarm;
            _responses[updatedAlarm.Id] = new List<HistoryEntry>();
            _callOnUpdate(updatedAlarm);
        }

        public void UpdateAlarm(AlarmData update)
        {
            var id = IdHelpers.GetGuid(update.Id);
            Alarm a;
            if (!_alarms.TryGetValue(id, out a)) return;
            var updatedAlarm = a.CloneWith(
                id: id,
                description: update.Description,
                partitionId: update.PartitionId,
                partitionName: update.PartitionName,
                isPublic: update.IsPublic,
                messageDateTime: update.MessageDateTime,
                alarmTypeDescription: update.AlarmTypeDescription,
                category: update.Category,
                priority: update.Priority,
                isResponseRequired: update.IsResponseRequired,
                isAcknowledgeRequired: update.IsAcknowledgeRequired,
                alarmState: update.AlarmState,
                alarmStateDescription: update.AlarmStateDescription,
                stateDateTime: update.StateDateTime,
                conditionSequence: update.ConditionSequence,
                site: update.Site,
                sourceState: update.SourceState,
                sourceStateDescription: update.SourceStateDescription,
                escalation: update.Escalation,
                instructions: update.Instructions,
                isPublicDescription: update.IsPublicDescription,
                isPending: update.IsPending,
                isCompletable: update.IsCompletable,
                isRespondable: update.IsRespondable,
                isRemovable: update.IsRemovable,
                isCompleted: update.IsCompleted);
            _alarms[updatedAlarm.Id] = updatedAlarm;
            _callOnUpdate(updatedAlarm);
        }

        //string id, DateTimeOffset timestamp, string operatorName, string alarmStatus, string alarmState, string response
        public void RespondToAlarm(Guid alarmId, string response, DateTime? timestamp = null, string userid = null, string alarmStatus = null, string alarmState = null)
        {
            if (!_alarms.ContainsKey(alarmId)) return;
            List<HistoryEntry> responses;
            if (!_responses.TryGetValue(alarmId, out responses)) return;

            lock (responses)
            {
                responses.Add(new HistoryEntry(Guid.NewGuid().ToString(), timestamp ?? DateTimeOffset.UtcNow, userid, "", _alarms[alarmId].AlarmStateDescription, response));
            }

            _callOnUpdate(_alarms[alarmId]);
        }

        public IEnumerable<HistoryEntry> GetResponses(Guid alarmId)
        {
            List<HistoryEntry> result;
            if (_responses.TryGetValue(alarmId, out result))
            {
                lock (result)
                {
                    return _responses[alarmId].ToList();
                }
            }
            return Enumerable.Empty<HistoryEntry>();
        }

        public Alarm GetAlarm(Guid id)
        {
            Alarm a;
            _alarms.TryGetValue(id, out a);
            return a;//Null will be returned if the alarm is not found.
        }

        public void Clear()
        {
            _alarms.Clear();
            _responses.Clear();
        }
    }
}