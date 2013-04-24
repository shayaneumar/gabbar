/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;

namespace JohnsonControls.TestExtensions
{
    public sealed class InterestingValueFactory
    {
        private enum ValueId
        {
            A, B
        }
        private readonly Dictionary<Type, Func<ValueId, object>> _interestingValueMap = new Dictionary<Type, Func<ValueId, object>>();
        public InterestingValueFactory()
        {
            RegisterValues('a', 'b');
            RegisterValues("a", "b");

            RegisterValues(new object(), new object());
            RegisterValues(Guid.NewGuid(), Guid.NewGuid());

            RegisterValues(0, 1);
            RegisterValues<uint>(0, 1);

            RegisterValues(0L, 1L);

            RegisterValues<short>(0, 1);
            RegisterValues<ushort>(0, 1);

            RegisterValues<byte>(0, 1);
            RegisterValues<sbyte>(0, 1);

            RegisterValues(decimal.Zero, decimal.One);

            RegisterValues(1.0D, 2.0D);
            RegisterValues(1.0F, 2.0F);

            RegisterValues(true, false);
        }
        public object A(Type t)
        {
            Func<ValueId, object> retVal;
            if(_interestingValueMap.TryGetValue(t, out retVal))
            {
                return retVal(ValueId.A);
            }
            throw new ArgumentOutOfRangeException("No interesting values were registered for "+t.Name);
        }
        public object B(Type t)
        {
            Func<ValueId, object> retVal;
            if (_interestingValueMap.TryGetValue(t, out retVal))
            {
                return retVal(ValueId.B);
            }
            throw new ArgumentOutOfRangeException("No interesting values were registered for " + t.Name);
        }

        public void RegisterValues<T>(T a, T b)
        {
            _interestingValueMap[typeof(T)] = x => x == ValueId.A ? a : b;
        }
    }
}