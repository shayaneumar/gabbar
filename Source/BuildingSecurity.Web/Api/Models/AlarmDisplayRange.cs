/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace BuildingSecurity.Web.Api.Models
{
    /// <summary>
    /// The display options for a range of alarm priorities.
    /// </summary>
    [DataContract]
    public class AlarmDisplayRange
    {
        /// <summary>
        /// The id of the priority range.
        /// </summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// The lower limit of the priority range.
        /// </summary>
        [DataMember(Name = "lowerLimit")]
        [Range(0, 255)]
        public int LowerLimit { get; set; }

        /// <summary>
        /// The upper limit of the priority range.
        /// </summary>
        [DataMember(Name = "upperLimit")]
        [Range(0,255)]
        public int UpperLimit { get; set; }

        /// <summary>
        /// The color to display for the priority range. This can be any color string format that can be used in css.
        /// </summary>
        [DataMember(Name = "color")]
        public string Color { get; set; }

        /// <summary>
        /// The id of the audio alert to play for the priority range.
        /// </summary>
        [DataMember(Name = "audioAlertId")]
        public Guid AudioAlertId { get; set; }


        private static readonly Regex HexColorRegex = new Regex(@"\A#([0-9]|[a-f]){1,8}\Z", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant|RegexOptions.Compiled);
        public static bool Validate(IEnumerable<AlarmDisplayRange> alarmDisplayOptions)
        {//This should be refactored to make use of System.ComponentModel.DataAnnotations
            int lastUpperLimit = -1;

            foreach (var x in alarmDisplayOptions.OrderBy(x=>x.LowerLimit))
            {
                //Validate Limits
                if(x.LowerLimit != lastUpperLimit+1)//Validate LowerLimit
                {
                    return false;
                }
                if(x.UpperLimit< x.LowerLimit)
                {
                    return false;
                }
                lastUpperLimit = x.UpperLimit;
                //Validate color (This one is most important because it could be an arbitrary string)
                if(x.Color == null)
                {
                    return false;
                }
                if (x.Color.Length > 0 && !HexColorRegex.IsMatch(x.Color))//if a string is present by not a color
                {
                    return false;
                }
            }
            return true;
        }
    }
}
