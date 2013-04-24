/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;

namespace JohnsonControls.BuildingSecurity.Pseudo.Client
{
    public class PseudoCookie : IBuildingSecurityClientCookie
    {
        public string UserName { get; set; }

        public PseudoCookie(string userName)
        {
            UserName = userName;
            Id = Guid.NewGuid().ToString();
        }


        public string Id { get; set; }
    }
}