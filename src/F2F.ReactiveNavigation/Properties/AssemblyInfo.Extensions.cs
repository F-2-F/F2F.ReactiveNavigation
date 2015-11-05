// These parts have to stay in a separate assembly info, since fake re-creates AssemblyInfo.cs on each build
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("F2F.ReactiveNavigation.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]  // necessary for FakeItEasy to create fakes of internal interfaces