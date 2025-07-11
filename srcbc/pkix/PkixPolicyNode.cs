using System;
using System.Collections;
using System.Text;

using iTextSharp.Org.BouncyCastle.Utilities;
using iTextSharp.Org.BouncyCastle.Utilities.Collections;

namespace iTextSharp.Org.BouncyCastle.Pkix
{
	/// <summary>
	/// Summary description for PkixPolicyNode.
	/// </summary>
	public class PkixPolicyNode
//		: IPolicyNode
	{
		protected IList				mChildren;
		protected int				mDepth;
		protected ISet				mExpectedPolicies;
		protected PkixPolicyNode	mParent;
		protected ISet				mPolicyQualifiers;
		protected string			mValidPolicy;
		protected bool				mCritical;

		public virtual int Depth
		{
			get { return this.mDepth; }
		}

		public virtual IEnumerable Children
		{
			get { return ArrayList.ReadOnly(mChildren); }
		}

		public virtual bool IsCritical
		{
			get { return this.mCritical; }
			set { this.mCritical = value; }
		}

		public virtual ISet PolicyQualifiers
		{
			get { return new HashSet(this.mPolicyQualifiers); }
		}

		public virtual string ValidPolicy
		{
			get { return this.mValidPolicy; }
		}

		public virtual bool HasChildren
		{
			get { return mChildren.Count != 0; }
		}

		public virtual ISet ExpectedPolicies
		{
			get { return new HashSet(this.mExpectedPolicies); }
			set { this.mExpectedPolicies = new HashSet(value); }
		}

		public virtual PkixPolicyNode Parent
		{
			get { return this.mParent; }
			set { this.mParent = value; }
		}

		/// Constructors
		public PkixPolicyNode(
			IList			children,
			int				depth,
			ISet			expectedPolicies,
			PkixPolicyNode	parent,
			ISet			policyQualifiers,
			string			validPolicy,
			bool			critical)
		{
			ArrayList newChildren = new ArrayList();
			if (children != null)
			{
				newChildren.AddRange(children);
			}

			this.mChildren = newChildren;
			this.mDepth = depth;
			this.mExpectedPolicies = expectedPolicies;
			this.mParent = parent;
			this.mPolicyQualifiers = policyQualifiers;
			this.mValidPolicy = validPolicy;
			this.mCritical = critical;
		}

		public virtual void AddChild(
			PkixPolicyNode child)
		{
			child.Parent = this;
			mChildren.Add(child);
		}

		public virtual void RemoveChild(
			PkixPolicyNode child)
		{
			mChildren.Remove(child);
		}

		public override string ToString()
		{
			return ToString("");
		}

		public virtual string ToString(
			string indent)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(indent);
			buf.Append(mValidPolicy);
			buf.Append(" {");
			buf.Append(Platform.NewLine);

			foreach (PkixPolicyNode child in mChildren)
			{
				buf.Append(child.ToString(indent + "    "));
			}

			buf.Append(indent);
			buf.Append("}");
			buf.Append(Platform.NewLine);
			return buf.ToString();
		}

		public virtual object Clone()
		{
			return Copy();
		}

		public virtual PkixPolicyNode Copy()
		{
			PkixPolicyNode node = new PkixPolicyNode(
				new ArrayList(),
				mDepth,
				new HashSet(mExpectedPolicies),
				null,
				new HashSet(mPolicyQualifiers),
				mValidPolicy,
				mCritical);

			foreach (PkixPolicyNode child in mChildren)
			{
				PkixPolicyNode copy = child.Copy();
				copy.Parent = node;
				node.AddChild(copy);
			}

			return node;
		}
	}
}
