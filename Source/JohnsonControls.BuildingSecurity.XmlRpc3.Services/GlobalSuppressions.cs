// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Justification = "No better solution for DTO's using XML Serialization available", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.AlarmGetResponseTextListReply.#ResponseTexts")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Justification = "No better solution for DTO's using XML Serialization available", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.UserDetails.#Partitions")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Justification = "No better solution for DTO's using XML Serialization available", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.UserDetails.#Permissions")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Justification = "No better solution for DTO's using XML Serialization available", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.AlarmDetailsReply.#AlarmHistories")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Justification = "No better solution for DTO's using XML Serialization available", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.AlarmGetListExReply.#AlarmMessages")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Justification = "No better solution for DTO's using XML Serialization available", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.SortOrder.#SortKeys")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ack", Justification = "This allows us to exactly match names used by P2000 APIs", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.AlarmResponse.#AlarmAckRequired")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ISessionManagementService.#P2000Logout(System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ITypedSessionManagement.#P2000Login(System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Scope = "type", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.P2000LoginReply")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Scope = "type", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.P2000LoginReplyWrapper")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.P2000LoginReplyWrapper.#P2000LoginReply")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.Permission")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ISessionManagementService.#P2000Login(System.String,System.String,System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HeartBeat", Scope = "member", Justification = "This allows us to exactly match names used by P2000 APIs", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ISessionManagementService.#P2000SessionHeartBeat(System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HeartBeat", Scope = "type", Justification = "This allows us to exactly match names used by P2000 APIs", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ISessionManagementService")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HeartBeat", Scope = "type", Justification = "This allows us to exactly match names used by P2000 APIs", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ISessionManagementService")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ITypedSessionManagement.#P2000Logout(System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ITypedAlarmService.#AlarmGetListEx(System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Int32,JohnsonControls.BuildingSecurity.XmlRpc3.Services.SortOrder)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ITypedAlarmService.#AlarmDetails(System.String,System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.IAlarmService.#AlarmGetListEx(System.String,System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Cv", Scope = "type", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.MultiCvAlarmGuidFilter")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.MultipleCVAlarmGuidFilter.#CurrentValues")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.AlarmActionReply.#AlarmActionResponses")]
[assembly: SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.SortOrder.#.ctor(JohnsonControls.BuildingSecurity.XmlRpc3.Services.SortKey[],System.String)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.Caching")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ITypedSystemInformationService.#P2000GetVersionEx()")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.ISystemInformationService.#P2000GetVersionEx()")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rtl", Scope = "member", Target = "JohnsonControls.BuildingSecurity.XmlRpc3.Services.P2000GetSystemInfoReply.#XmlRtlPort")]
