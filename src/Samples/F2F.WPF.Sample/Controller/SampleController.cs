using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.WPF.Sample.Controller
{
	public interface ISampleController
	{
		string LoadTitle(int value);
	}

	public class SampleController : ISampleController
	{
		private readonly DummyDisposable _dummy;
		
		public SampleController(DummyDisposable dummy)
		{
			_dummy = dummy;
		}

		public string LoadTitle(int value)
		{
			return value.ToString();
		}
	}
}
