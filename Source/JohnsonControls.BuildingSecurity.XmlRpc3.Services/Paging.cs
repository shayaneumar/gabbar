/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Xml.Serialization;

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    /// <summary>
    /// Paging used by the P2000.  This class is used for communication
    /// to and from the server.
    /// </summary>
    [XmlRoot("Paging")]
    public class Paging
    {
        /// <summary>
        ///  The value of RecordCount to use to indicate that all records are requested.
        /// </summary>
        public static int AllRecordsRecordCount { get { return -1; } }

        /// <summary>
        /// The maximum respected value of RecordsPerPage 
        /// </summary>
        public static int MaxRecordsPerPage { get { return 50; } }

        /// <summary>
        /// Returns the value used to identify the first page (which is 0)
        /// </summary>
        public static int FirstPageNumber { get { return 0; } }

        /// <summary>
        /// Creates a default instance of <see cref="Paging"/>
        /// </summary>
        public Paging()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="Paging"/>
        /// </summary>
        /// <param name="pageNumber">The page number. This Page Number is 0-based.</param>
        /// <param name="recordsPerPage">The records per page.</param>
        /// <param name="recordCount">Total number of records associated with this particular call.</param>
        public Paging(int pageNumber, int recordsPerPage, int recordCount)
        {
            PageNumber = pageNumber;
            RecordsPerPage = recordsPerPage;
            RecordCount = recordCount;
        }

        /// <summary>
        /// Gets or sets the page number.  This Page Number is 0-based.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        [XmlElement("Page")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the records per page.
        /// </summary>
        /// <value>
        /// The records per page.
        /// </value>
        [XmlElement("RecordsPerPage")]
        public int RecordsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of records associated with
        /// this particular call or the total number of records requested.
        /// </summary>
        /// <value>
        /// The total record count.
        /// </value>
        public int RecordCount { get; set; }

        /// <summary>
        /// Creates and returns a <see cref="Paging"/> object containing
        /// the value of AllRecordsRecordCount for RecordCount. Setting this value 
        /// essentially tells the P2000 that we want all records returned.
        /// </summary>
        /// <returns></returns>
        public static Paging TotalRecordsRequest()
        {
            return new Paging(pageNumber: FirstPageNumber, recordsPerPage: MaxRecordsPerPage,
                              recordCount: AllRecordsRecordCount);
        }
    }
}
