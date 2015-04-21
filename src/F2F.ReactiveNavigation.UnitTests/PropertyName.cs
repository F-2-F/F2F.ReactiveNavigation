using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace F2F.ReactiveNavigation.UnitTests
{
	/// <summary>
	/// Supporting functions for property projection lambdas.
	/// </summary>
	public static class PropertyName
	{
		/// <summary>
		/// Extracts the property name from a property projection lambda.
		/// </summary>
		/// <typeparam name="TProperty">Property type</typeparam>
		/// <param name="projection">Property projection function</param>
		/// <returns>Property name</returns>
		/// /// <example>
		/// string propName = PropertyName.From(() => someObj.SomeProperty);
		/// Assert.AreEqual(propName, "SomeProperty");
		/// </example>
		public static string Of<TProperty>(Expression<Func<TProperty>> projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection", "projection is null.");
			if (!(projection.Body is MemberExpression))
				throw new ArgumentNullException("projection must be type of MemberExpression.");

			// can't test for property expression in portable lib (at least I don't know how)
			// MemberType is not defined
			//dbc.Contract.Requires<ArgumentException>((projection.Body as MemberExpression).Member.MemberType == System.Reflection.MemberTypes.Property,
			//				  "Projection must select a property.");

			var body = projection.Body as MemberExpression;

			return body.Member.Name;
		}

		/// <summary>
		/// Extracts the property name from a property projection lambda.
		/// </summary>
		/// <typeparam name="TProperty">Property type</typeparam>
		/// <param name="projection">Property projection function</param>
		/// <returns>Property name</returns>
		/// /// <example>
		/// string propName = PropertyName.From(() => someObj.SomeProperty);
		/// Assert.AreEqual(propName, "SomeProperty");
		/// </example>
		public static string Of<T, TProperty>(Expression<Func<T, TProperty>> projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection", "projection is null.");
			if (!(projection.Body is MemberExpression))
				throw new ArgumentNullException("projection must be type of MemberExpression.");

			// can't test for property expression in portable lib (at least I don't know how)
			// MemberType is not defined
			//dbc.Contract.Requires<ArgumentException>((projection.Body as MemberExpression).Member.MemberType == System.Reflection.MemberTypes.Property,
			//				  "Projection must select a property.");

			var body = projection.Body as MemberExpression;

			return body.Member.Name;
		}

		public static string GetPropertyName<T, TProperty>(this T @object, Expression<Func<T, TProperty>> projection)
		{
			if (@object == null)
				throw new ArgumentNullException("@object", "@object is null.");
			if (projection == null)
				throw new ArgumentNullException("projection", "projection is null.");
			if (!(projection.Body is MemberExpression))
				throw new ArgumentNullException("projection must be type of MemberExpression.");

			// can't test for property expression in portable lib (at least I don't know how)
			// MemberType is not defined
			//dbc.Contract.Requires<ArgumentException>((projection.Body as MemberExpression).Member.MemberType == System.Reflection.MemberTypes.Property,
			//				  "Projection must select a property.");

			var body = projection.Body as MemberExpression;

			return body.Member.Name;
		}
	}
}