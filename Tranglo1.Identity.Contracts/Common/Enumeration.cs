using CSharpFunctionalExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tranglo1.CustomerIdentity.Domain.Common
{
    public abstract class Enumeration : Entity, IComparable
    {
        protected static ConcurrentDictionary<Type, Enumeration[]> _CachedEnumerations = 
            new ConcurrentDictionary<Type, Enumeration[]>();

        public string Name { get; private set; }

        protected Enumeration()
        {

        }

        protected Enumeration(int id, string name)
            : base(id)
        {
            Name = name;
        }

        public override string ToString() => Name;

        public static T FindById<T>(long id) where T : Enumeration
		{
            return GetAll<T>().Where(e => e.Id == id).FirstOrDefault();
		}

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            return _CachedEnumerations.GetOrAdd(typeof(T), t =>
            {
                var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

                return fields
                    .Where(f => f.DeclaringType == t)
                    .Select(f => f.GetValue(null))
                    .Cast<T>()
                    .OrderBy(e => e.Name)
                    .ToArray();

            }).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);

        public static T FindByName<T>(string name) where T : Enumeration
        {
            return GetAll<T>().Where(e => e.Name == name).FirstOrDefault();
        }
    }
}
