/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client.Scripting.JVent.Runtime
{
    public class InMemorySimulationClient : ISimulatorClient
    {
        private readonly PseudoBuildingSecurityClient _buildingSecurityClient;
        private readonly InMemoryAlarmRepository _alarmRepository;
        private readonly InMemoryCaseRepository _caseRepository;
        private readonly Scheduler _scheduler;

        public InMemorySimulationClient(IBuildingSecurityClient buildingSecurityClient, Scheduler scheduler)
        {
            _buildingSecurityClient = buildingSecurityClient as PseudoBuildingSecurityClient;
            Debug.Assert(_buildingSecurityClient != null, "_buildingSecurityClient != null");
            _alarmRepository = _buildingSecurityClient.AlarmRepository;
            _caseRepository = _buildingSecurityClient.CaseRepository;
            _scheduler = scheduler;
            RegisterEventHanders();
        }

        private void RegisterEventHanders()
        {
            _scheduler.RegisterEventHandler(@"CreateAlarm",CreateAlarm);
            _scheduler.RegisterEventHandler(@"RespondToAlarm", RespondToAlarm);
            _scheduler.RegisterEventHandler(@"UpdateAlarm", UpdateAlarm);
            _scheduler.RegisterEventHandler(@"ClearAlarms", ClearAlarms);
            _scheduler.RegisterEventHandler(@"AddCannedResponseText", AddCannedResponseText);
            _scheduler.RegisterEventHandler(@"CreateCase", CreateCase);
            _scheduler.RegisterEventHandler(@"ClearCases", ClearCases);
            _scheduler.RegisterEventHandler(@"CreateCaseNote", CreateCaseNote);
            _scheduler.RegisterEventHandler(@"ClearCaseNotes", ClearCaseNotes);
            _scheduler.RegisterEventHandler(@"UpdateCase", UpdateCase);
        }

        private void AddCannedResponseText(DateTime time, JsonEvent arg2)
        {
            _buildingSecurityClient.AddCannedResponseText(arg2.Value);
        }

        private void ClearAlarms(DateTime time, JsonEvent arg2)
        {
            _alarmRepository.Clear();
        }

        private void UpdateAlarm(DateTime time, JsonEvent arg2)
        {
            var updatedAlarm = JsonConvert.DeserializeObject<AlarmData>(arg2.Value);
            _alarmRepository.UpdateAlarm(updatedAlarm);
        }

        private void RespondToAlarm(DateTime time, JsonEvent arg2)
        {
            var response = JsonConvert.DeserializeObject<ResponseData>(arg2.Value);
            _alarmRepository.RespondToAlarm(response.NormalizedId, response.Response, time, response.UserId, response.AlarmStatus, response.AlarmState);
        }

        private void CreateAlarm(DateTime time, JsonEvent arg2)
        {
            var alarm = JsonConvert.DeserializeObject<AlarmData>(arg2.Value);
            _alarmRepository.CreateAlarm(alarm);
        }

        public void Run(string script)
        {
            _scheduler.Run(new JsonEventScript(script));
        }

        public void ResetAll()
        {
            _scheduler.StopAll();
        }

        private void CreateCase(DateTime time, JsonEvent arg2)
        {
            var caseData = JsonConvert.DeserializeObject<CaseData>(arg2.Value);
            caseData.CreatedDateTime = time;
            _caseRepository.CreateCase(caseData);
        }

        private void ClearCases(DateTime time, JsonEvent arg2)
        {
            _caseRepository.Clear();
        }

        private void CreateCaseNote(DateTime time, JsonEvent arg2)
        {
            var caseNoteData = JsonConvert.DeserializeObject<CaseNoteData>(arg2.Value);
            _caseRepository.CreateCaseNote(time, caseNoteData);
        }

        private void ClearCaseNotes(DateTime time, JsonEvent arg2)
        {
            var caseId = JsonConvert.DeserializeObject<string>(arg2.Value);
            _caseRepository.ClearCaseNotes(caseId);
        }

        private void UpdateCase(DateTime arg1, JsonEvent arg2)
        {
            var caseUpdate= JsonConvert.DeserializeObject<CaseData>(arg2.Value);
            _caseRepository.UpdateCase(caseUpdate);
        }
    }
}
