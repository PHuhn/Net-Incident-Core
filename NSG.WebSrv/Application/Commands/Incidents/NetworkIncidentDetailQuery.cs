//
using System;
using System.Text;
using System.Collections.Generic;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
    //
    /// <summary>
    /// Network-Incident detail unit-of-work query
    /// </summary>
    public class NetworkIncidentDetailQuery
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long IncidentId { get; set; }
        public int ServerId { get; set; }
        public string IPAddress { get; set; }
        public string NIC_Id { get; set; }
        public string NetworkName { get; set; }
        public string AbuseEmailAddress { get; set; }
        public string ISPTicketNumber { get; set; }
        public bool Mailed { get; set; }
        public bool Closed { get; set; }
        public bool Special { get; set; }
        public string Notes { get; set; }
        //
        public List<NetworkLogData> NetworkLogs;
        public List<NetworkLogData> DeletedLogs;
        //
        public List<IncidentNoteData> IncidentNotes;
        public List<IncidentNoteData> DeletedNotes;
        //
        public List<IncidentTypeData> TypeEmailTemplates;
        //
        public List<SelectItem> NICs;
        //
        public List<SelectItem> IncidentTypes;
        //
        public List<SelectItem> NoteTypes;
        //
        public string Message;
        //
    }
    //
    // ---------------------------------------------------------------------------
    //
    /// <summary>
    /// NetworkLog table from the database.
    /// </summary>
    public class NetworkLogData
    {
        #region "NetworkLogData Class Properties"
        //
        /// <summary>
        /// For column NetworkLogId
        /// </summary>
        public long NetworkLogId { get; set; }
        //
        /// <summary>
        /// For column CompanyId
        /// </summary>
        public int ServerId { get; set; }
        //
        /// <summary>
        /// For column IncidentId
        /// </summary>
        public long? IncidentId { get; set; }
        //
        /// <summary>
        /// For column IPAddress
        /// </summary>
        public string IPAddress { get; set; }
        //
        /// <summary>
        /// For column NetworkLogDate
        /// </summary>
        public DateTime NetworkLogDate { get; set; }
        //
        /// <summary>
        /// For column Logs
        /// </summary>
        public string Log { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeId
        /// </summary>
        public int IncidentTypeId { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeShortDesc in IncidentType
        /// </summary>
        public string IncidentTypeShortDesc { get; set; }
        //
        /// <summary>
        /// For pseudo column, for grid selection
        /// </summary>
        public bool Selected { get; set; }
        //
        /// <summary>
        /// For pseudo column, for change tracking
        /// </summary>
        public bool IsChanged { get; set; }
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public override string ToString()
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("NetworkLogId: {0}, ", NetworkLogId.ToString());
            _return.AppendFormat("ServerId: {0}, ", ServerId);
            if (IncidentId.HasValue)
                _return.AppendFormat("IncidentId: {0}, ", IncidentId.Value);
            else
                _return.AppendFormat("IncidentId: //, ");
            _return.AppendFormat("IncidentId: {0}, ", IncidentId);
            _return.AppendFormat("IPAddress: {0}, ", IPAddress);
            _return.AppendFormat("NetworkLogDate: {0}, ", NetworkLogDate.ToString());
            _return.AppendFormat("IncidentTypeId: {0}, ", IncidentTypeId.ToString());
            _return.AppendFormat("Selected: {0}, ", Selected.ToString());
            _return.AppendFormat("IsChanged: {0}, ", IsChanged.ToString());
            _return.AppendFormat("Log: {0}]", Log);
            return _return.ToString();
        }
        //
        #endregion
    }
    //
    /// <summary>
    /// IncidentNotes table from the database.
    /// </summary>
    public class IncidentNoteData
    {
        #region "IncidentNoteData Class Properties"
        //
        /// <summary>
        /// For column IncidentNotesId
        /// </summary>
        public long IncidentNoteId { get; set; }
        //
        /// <summary>
        /// For column NotesTypeId
        /// </summary>
        public int NoteTypeId { get; set; }
        //
        /// <summary>
        /// For column NoteTypeShortDesc from NoteType
        /// </summary>
        public string NoteTypeShortDesc { get; set; }
        //
        /// <summary>
        /// For column Notes
        /// </summary>
        public string Note { get; set; }
        //
        /// <summary>
        /// For column CreatedDate
        /// </summary>
        public DateTime CreatedDate { get; set; }
        //
        /// <summary>
        /// For pseudo column, for change tracking
        /// </summary>
        public bool IsChanged { get; set; }
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public override string ToString()
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("IncidentNotesId: {0}, ", IncidentNoteId.ToString());
            _return.AppendFormat("NoteTypeId: {0}, ", NoteTypeId.ToString());
            _return.AppendFormat("NoteTypeShortDesc: {0}, ", NoteTypeShortDesc);
            _return.AppendFormat("Note: {0}, ", Note);
            _return.AppendFormat("CreatedDate: {0}]", CreatedDate.ToString());
            return _return.ToString();
        }
        //
        #endregion
    }
    //
    /// <summary>
    /// IncidentType table from the database.
    /// </summary>
    public class IncidentTypeData
    {
        #region "IncidentTypeData Class Properties"
        //
        /// <summary>
        /// For column IncidentTypeId
        /// </summary>
        public int IncidentTypeId { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeShortDesc
        /// </summary>
        public string IncidentTypeShortDesc { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeDesc
        /// </summary>
        public string IncidentTypeDesc { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeFromServer
        /// </summary>
        public bool IncidentTypeFromServer { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeSubjectLine
        /// </summary>
        public string IncidentTypeSubjectLine { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeEmailTemplate
        /// </summary>
        public string IncidentTypeEmailTemplate { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeTimeTemplate
        /// </summary>
        public string IncidentTypeTimeTemplate { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeThanksTemplate
        /// </summary>
        public string IncidentTypeThanksTemplate { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeLogTemplate
        /// </summary>
        public string IncidentTypeLogTemplate { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeTemplate
        /// </summary>
        public string IncidentTypeTemplate { get; set; }
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public override string ToString()
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("IncidentTypeId: {0}, ", IncidentTypeId.ToString());
            _return.AppendFormat("IncidentTypeShortDesc: {0}, ", IncidentTypeShortDesc);
            _return.AppendFormat("IncidentTypeDesc: {0}, ", IncidentTypeDesc);
            _return.AppendFormat("IncidentTypeFromServer: {0}, ", IncidentTypeFromServer);
            _return.AppendFormat("IncidentTypeSubjectLine: {0}, ", IncidentTypeSubjectLine);
            _return.AppendFormat("IncidentTypeEmailTemplate: {0}, ", IncidentTypeEmailTemplate);
            _return.AppendFormat("IncidentTypeTimeTemplate: {0}, ", IncidentTypeTimeTemplate);
            _return.AppendFormat("IncidentTypeThanksTemplate: {0}, ", IncidentTypeThanksTemplate);
            _return.AppendFormat("IncidentTypeLogTemplate: {0}, ", IncidentTypeLogTemplate);
            _return.AppendFormat("IncidentTypeTemplate: {0}]", IncidentTypeTemplate);
            return _return.ToString();
        }
        //
        #endregion
    }
    //
    /// <summary>
    /// POCO of a SelectItem.
    /// </summary>
    /// <remarks>Multiple constructors</remarks>
    public class SelectItem
    {
        #region "SelectItem Class Properties"
        public object value { get; set; }
        public string label { get; set; }
        public string styleClass { get; set; }
        //
        /// <summary>
        /// Create a SelectItem using parameter-less (default) constructor.
        /// </summary>
        /// <remarks></remarks>
        public SelectItem()
        {
            this.value = "";
            this.label = "";
            this.styleClass = "";
        }
        //
        /// <summary>
        /// Create a SelectItem object using 2 parameters constructor, default selected to false.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <remarks></remarks>
        public SelectItem(string value, string label)
        {
            this.value = value;
            this.label = label;
            this.styleClass = "";
        }
        //
        /// <summary>
        /// Create a new object using all columns constructor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <param name="selected"></param>
        /// <remarks></remarks>
        public SelectItem(string value, string label, string styleClass)
        {
            this.value = value;
            this.label = label;
            this.styleClass = styleClass;
        }
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public override string ToString()
        {
            return String.Format("record:[value: {0}, label: {1}, styleClass: {2}]",
                this.value, this.label, this.styleClass);
        }
        //
        #endregion // SelectItem Class Properties
    }
    //
}
