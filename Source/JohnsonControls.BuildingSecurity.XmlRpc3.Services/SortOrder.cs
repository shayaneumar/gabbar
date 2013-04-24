/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

namespace JohnsonControls.BuildingSecurity.XmlRpc3.Services
{
    public class SortOrder
    {
        public SortOrder(): this(new SortKey[0])
        {}

        /// <summary>
        /// Creates an instance of <see cref="SortOrder"/> with
        /// the specified array of SortKeys and Order
        /// </summary>
        /// <param name="sortKeys">Array of SortKeys to be
        /// applied to the results that are returned</param>
        /// <param name="order">Order of sorting
        /// (ASC = Ascending and DESC = Descending)</param>
        public SortOrder(SortKey[] sortKeys, string order= "DESC")
        {
            Order = order;
            SortKeys = sortKeys;
        }

        /// <summary>
        /// Array of SortKeys applied to the results that are returned
        /// </summary>
        public SortKey[] SortKeys { get; set; }

        /// <summary>
        /// Order of sorting (ASC = Ascending and DESC = Descending)
        /// </summary>
        public string Order { get; set; }
    }
}