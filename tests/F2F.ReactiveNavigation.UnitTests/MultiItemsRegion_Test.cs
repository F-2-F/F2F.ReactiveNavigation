using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.Internal;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using F2F.Testing.Xunit.FakeItEasy;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class MultiItemsRegion_Test : AutoMockFeature
	{
		[Fact]
		public void AssertProperNullGuards()
		{
			var assertion = new GuardClauseAssertion(Fixture);
			assertion.Verify(typeof(MultiItemsRegion));
		}
	}
}