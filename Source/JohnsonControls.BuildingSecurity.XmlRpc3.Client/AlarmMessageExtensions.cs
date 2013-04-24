/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using JohnsonControls.BuildingSecurity.XmlRpc3.Globalization;
using JohnsonControls.BuildingSecurity.XmlRpc3.Services;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Client
{
    public static class AlarmMessageExtensions
    {
        public static Alarm ConvertToAlarm(this AlarmMessage alarmMessage, TimeZoneInfo timeZone, CultureInfo culture)
        {
            if (alarmMessage == null) throw new ArgumentNullException("alarmMessage");

            return new Alarm(id: new Guid(alarmMessage.MessageDetails.AlarmGuid),
                            description: alarmMessage.MessageDetails.Description,
                            partitionId: Guid.Empty,
                            partitionName: alarmMessage.MessageBase.PartitionName,
                            isPublic: GetBoolFromStringInt(alarmMessage.MessageBase.IsPublic),
                            messageDateTime: TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(alarmMessage.MessageDetails.ConditionTimestampUtc, CultureInfo.InvariantCulture), timeZone),
                            alarmTypeDescription: alarmMessage.MessageDetails.AlarmTypeName,
                            category: alarmMessage.MessageBase.Category,
                            priority: Convert.ToInt32(alarmMessage.MessageBase.Priority, CultureInfo.InvariantCulture),
                            isResponseRequired: GetBoolFromStringInt(alarmMessage.MessageDetails.ResponseRequired),
                            isAcknowledgeRequired: GetBoolFromStringInt(alarmMessage.MessageDetails.AcknowledgeRequired),
                            alarmState: Convert.ToInt32(alarmMessage.MessageDetails.AlarmState, CultureInfo.CurrentCulture),
                            alarmStateDescription: Translator.GetString(CategoryType.AlarmStates, Convert.ToInt32(alarmMessage.MessageDetails.AlarmState, CultureInfo.CurrentCulture), culture),
                            stateDateTime: TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(alarmMessage.MessageBase.TimestampUtc, CultureInfo.InvariantCulture), timeZone),
                            conditionSequence: Convert.ToInt32(alarmMessage.MessageDetails.ConditionSequenceNumber, CultureInfo.InvariantCulture),
                            site: alarmMessage.MessageBase.SiteName,
                            sourceState: Convert.ToInt32(alarmMessage.MessageDetails.ConditionState, CultureInfo.CurrentCulture),
                            sourceStateDescription: Translator.GetString(CategoryType.ConditionStates, Convert.ToInt32(alarmMessage.MessageDetails.ConditionState, CultureInfo.CurrentCulture), culture),
                            escalation: Convert.ToInt32(alarmMessage.MessageBase.Escalation, CultureInfo.InvariantCulture),
                            instructions: alarmMessage.MessageDetails.InstructionText,
                            isPublicDescription: Translator.GetString(CategoryType.BooleanTypes, Convert.ToInt32(alarmMessage.MessageBase.IsPublic, CultureInfo.CurrentCulture), culture),
                            isPending: alarmMessage.IsPending(),
                            isCompletable: GetBoolFromStringInt(alarmMessage.MessageDetails.CanComplete),
                            isRespondable: GetBoolFromStringInt(alarmMessage.MessageDetails.CanRespond),
                            isRemovable: alarmMessage.IsRemovable(),
                            isCompleted: alarmMessage.IsCompleted());
        }

        private static bool GetBoolFromStringInt(string stringInteger)
        {
            int number = Int32.Parse(stringInteger, CultureInfo.InvariantCulture);
            return Convert.ToBoolean(number);
        }

        private static bool IsPending(this AlarmMessage alarmMessage)
        {
            return (int.Parse(alarmMessage.MessageDetails.AlarmState, CultureInfo.InvariantCulture) == (int)AlarmState.Pending);
        }

        private static bool IsRemovable(this AlarmMessage alarmMessage)
        {
            return (GetBoolFromStringInt(alarmMessage.MessageDetails.CanComplete) || int.Parse(alarmMessage.MessageDetails.AlarmState, CultureInfo.InvariantCulture) == (int)AlarmState.Completed);
        }

        private static bool IsCompleted(this AlarmMessage alarmMessage)
        {
            return (int.Parse(alarmMessage.MessageDetails.AlarmState, CultureInfo.InvariantCulture) == (int)AlarmState.Completed);
        }
    }
}