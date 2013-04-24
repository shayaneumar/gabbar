/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.TestExtensions
{
    public static class EqualityTestHelpers
    {
        public static MethodInfo GetConstructingFunction(this Type t, string functionName="ConstructingFunction")
        {
            return t.GetMethod(functionName, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        }

        public static void TestEquals(MethodInfo constructor)
        {
            TestEquals(constructor, new InterestingValueFactory());
        }

        public static void TestEquals(MethodInfo constructor, InterestingValueFactory valueFactory)
        {
            TestEquals(constructor, valueFactory, TestEqualsForSingleCase);
        }

        public static void TestGetHashCode(MethodInfo constructor)
        {
            TestGetHashCode(constructor, new InterestingValueFactory());
        }

        public static void TestGetHashCode(MethodInfo constructor, InterestingValueFactory valueFactory)
        {
            TestEquals(constructor, valueFactory, TestHashCodeForSingleCase);
        }


        private static void TestEquals(MethodInfo constructor, InterestingValueFactory valueFactory, Action<object, object, bool, string> verify)
        {
            var parameters = constructor.GetParameters();
            var aparams = new object[parameters.Length];
            var bparams = new object[parameters.Length];
            var usedInEquals = new bool[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                if (p.ParameterType.IsSubclassOf(typeof(UsedInEquals)))
                {
                    usedInEquals[i] = true;
                }
                else if (p.ParameterType.IsSubclassOf(typeof(NotUsedInEquals)))
                {
                    usedInEquals[i] = false;
                }
                else
                {
                    throw new InvalidCastException("Ambiguous typing, could not determine if type is used in equals check or not.");
                }

                var caster = p.ParameterType.GetMethod("op_Implicit");
                var boxedType = p.ParameterType.GetGenericArguments()[0];

                aparams[i] = caster.Invoke(null, new[] { valueFactory.A(boxedType) });
                bparams[i] = caster.Invoke(null, new[] { valueFactory.B(boxedType) });
            }
            //Interesting values generated
            object allAs = constructor.Invoke(null, aparams); //use the all a's for equality checks

            var current = new object[parameters.Length];
            aparams.CopyTo(current, 0);

            var isA = new bool[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                isA[i] = true;
            }

            //Test all a's scenario

            var objUnderTest = constructor.Invoke(null, current);
            verify(objUnderTest, allAs, true, string.Join("",isA.Select(x=>x?"A":"B")));

            for (int j = 0; j < parameters.Length; j++)
            {
                current[j] = aparams[j];
                isA[j] = true;

                for (int i = j; i < parameters.Length; i++) // toggle ever parameter remaining
                {
                    current[i] = bparams[i];
                    isA[i] = false;
                    objUnderTest = constructor.Invoke(null, current);
                    bool shouldBeEqual = ShouldBeEqual(isA, usedInEquals); //if no critical params have changed
                    verify(objUnderTest, allAs, shouldBeEqual, string.Join("", isA.Select(x => x ? "A" : "B")));

                    current[i] = aparams[i];
                    isA[i] = true;
                }

                current[j] = bparams[j];
                isA[j] = false;
            }

        }

        private static bool ShouldBeEqual(IEnumerable<bool> isA, IEnumerable<bool> usedInEquals)
        {
            return isA.Zip(usedInEquals, (x, inEqual) => (inEqual && x) || !inEqual).All(x => x);
        }

        private static void TestEqualsForSingleCase(object obj1, object obj2, bool shouldBeEqual, string state)
        {
            string failureMessage = "Expected equals to return: " + shouldBeEqual + " but was " + !shouldBeEqual + "Object was constructed with: " + state + "(these are the tokens for the input to the constructing function).";
            if(shouldBeEqual != obj1.Equals(obj2))
                throw new AssertFailedException(failureMessage);

            if (shouldBeEqual != obj2.Equals(obj1))
                throw new AssertFailedException("For state==" + state + "obj1.Equals(obj2) != obj2.Equals(obj1)");
        }

        private static void TestHashCodeForSingleCase(object obj1, object obj2, bool shouldBeEqual, string state)
        {
            var failureMessage = "Expected HashCodesMatch==" + shouldBeEqual + " but was " + !shouldBeEqual + "Object was constructed with: " + state + "(these are the tokens for the input to the constructing function).";
            if((obj1.GetHashCode() == obj2.GetHashCode()) == shouldBeEqual)
            {
                throw new AssertFailedException(failureMessage);
            }
        }
    }

    public class UsedInEquals { }
    public class NotUsedInEquals { }
    public sealed class UsedInEquals<T> : UsedInEquals
    {
        public T Value { get; set; }
        public static implicit operator UsedInEquals<T>(T t)
        {
            return new UsedInEquals<T>
            {
                Value = t
            };
        }
    }
    public sealed class NotUsedInEquals<T> : NotUsedInEquals
    {
        public T Value { get; set; }
        public static implicit operator NotUsedInEquals<T>(T t)
        {
            return new NotUsedInEquals<T>
            {
                Value = t
            };
        }
    }

}

