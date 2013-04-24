/*----------------------------------------------------------------------------

  (C) Copyright 2012-2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace JohnsonControls.Collections
{
    /// <summary>
    /// A serializable IDataChunk
    /// </summary>
    [DataContract]
    public class DataChunk<T> : IDataChunk<T>
    {
        public DataChunk([NotNull]IEnumerable<T> data, bool isEnd)
        {
            if(data == null) throw new ArgumentNullException("data");
            Data = data.ToList();
            FirstElement = Data.FirstOrDefault();
            LastElement = Data.LastOrDefault();
            IsEnd = isEnd;
            Count = Data.Count();
        }

        [DataMember]
        public IEnumerable<T> Data { get; private set; }

        [DataMember]
        public T FirstElement { get; private set; }

        [DataMember]
        public T LastElement { get; private set; }

        [DataMember]
        public bool IsEnd { get; private set; }

        [DataMember]
        public int Count { get; private set; }
    }
}
