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
    public class InMemoryCaseRepository : IEnumerable<Case>
    {
        private readonly ConcurrentDictionary<string, Case> _cases;
        private readonly Action<Case> _callOnUpdate;

        public InMemoryCaseRepository()
            : this(x => { }, Enumerable.Empty<Case>())
        { }

        public InMemoryCaseRepository(IEnumerable<Case> initialCases)
            : this(x => { }, initialCases)
        { }

        public InMemoryCaseRepository(Action<Case> callOnUpdate)
            : this(callOnUpdate, Enumerable.Empty<Case>())
        { }

        public InMemoryCaseRepository(Action<Case> callOnUpdate, IEnumerable<Case> initialCases)
        {
            _callOnUpdate = callOnUpdate;
            _cases = new ConcurrentDictionary<string, Case>(initialCases.ToDictionary(x => x.Id, x => x));
        }

        public IEnumerator<Case> GetEnumerator()
        {
            return _cases.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Case RetrieveCase(string id)
        {
            Case caseDetails;
            _cases.TryGetValue(id, out caseDetails);
            return caseDetails; //Null will be returned if the case is not found.
        }

        public string CreateCase(CaseData caseData)
        {
            var updatedCase = new Case(id: IdHelpers.GetId(caseData.Id), title: caseData.Title, createdBy: caseData.CreatedBy, createdDateTime: caseData.CreatedDateTime, owner: caseData.Owner, notes: Enumerable.Empty<CaseNote>(), status:caseData.StatusEnum);
            _cases[updatedCase.Id] = updatedCase;

            _callOnUpdate(updatedCase);
            return updatedCase.Id;
        }

        public void Clear()
        {
            _cases.Clear();
        }

        public CaseNote CreateCaseNote(DateTime timestamp, CaseNoteData caseNoteData)
        {
            if (caseNoteData == null || string.IsNullOrWhiteSpace(caseNoteData.CaseId))
                return null;

            Case existingCase;

            if (!_cases.TryGetValue(caseNoteData.CaseId, out existingCase))
                return null;

            var id = IdHelpers.GetGuid(caseNoteData.Id);
            var newNote = new CaseNote(
                id: id,
                timestampUtc: timestamp,
                createdBy: caseNoteData.CreatedBy,
                text: caseNoteData.Text
                );
            _cases[caseNoteData.CaseId] = existingCase.CloneWith(notes: existingCase.Notes.Where(x => x.Id != id).Concat(new[] { newNote }));

            _callOnUpdate(_cases[caseNoteData.CaseId]);
            return newNote;
        }

        public void ClearCaseNotes(string caseId)
        {
            if (caseId == null)
                return;
            
            Case caseDetails;
            if (_cases.TryGetValue(caseId, out caseDetails))
            {
                _cases[caseId] = caseDetails.CloneWith(notes: Enumerable.Empty<CaseNote>());
            }

            _callOnUpdate(_cases[caseId]);
        }

        public void UpdateCase(CaseData caseUpdate)
        {
            Case existingCase;
            if (!_cases.TryGetValue(caseUpdate.Id, out existingCase))
            {
                return;
            }

            var updatedCase = existingCase.CloneWith(title: caseUpdate.Title, owner: caseUpdate.Owner, status:caseUpdate.StatusEnum);
            _cases[caseUpdate.Id] = updatedCase;
            _callOnUpdate(updatedCase);
        }
    }
}
