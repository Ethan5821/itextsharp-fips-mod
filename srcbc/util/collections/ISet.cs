using System;
using System.Collections;

namespace iTextSharp.Org.BouncyCastle.Utilities.Collections
{
	public interface ISet
		: ICollection
	{
		void Add(object o);
		void AddAll(IEnumerable e);
		void Clear();
		bool Contains(object o);
		bool IsEmpty { get; }
		void Remove(object o);
		void RemoveAll(IEnumerable e);
	}
}
