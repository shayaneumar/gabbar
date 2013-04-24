/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;

namespace JohnsonControls.BuildingSecurity
{
    /// <summary>
    /// A typedef for a tuple containing an alarm id and a sequence number.
    /// </summary>
    public class AlarmIdSequenceTuple : Tuple<Guid, int>
    {
        public AlarmIdSequenceTuple(Guid alarmId, int conditionSequence) : base(alarmId, conditionSequence)
        {
        }
    }
}
