# F2F.ReactiveNavigation

You already tried [PRISM](https://compositewpf.codeplex.com/) and you are not convinced about it's navigation capabilities? We too! So we took the concept of navigation and created a new framework for UI navigation which is portable, asynchronous and reactive.

The basic idea is that an UI consists of regions, identified by `INavigableRegion`. A region can contain one or more views which are related to view models, the `ReactiveViewModel`. Creating, showing and closing views in a region is done using the same navigation mechanism in `INavigate`. You can use an `IRegionAdapter` for monitoring regions and perform actions on UI elements.

The navigation target is the view model, not the view. Furthermore the navigation is done explicit by using the concrete type of a view model. Each navigation request can transport additional information, the `INavigationParameters`.

Because **F2F.ReactiveNavigation** is a portable library there is no direct dependency to WPF or other UI elements. We depend on [ReactiveUI](https://github.com/reactiveui/ReactiveUI) which provides a portable MVVM framework. Furthermore we use [Reactive Extensions](https://rx.codeplex.com/).

There is a second library **F2F.ReactiveNavigation.WPF** which provides several extensions for WPF. For example there is a `TabRegionAdapter` which allows you to use a WPF TabControl as region.

Builds are available via NuGet:
- [F2F.ReactiveNavigation](http://www.nuget.org/packages/F2F.ReactiveNavigation/)
- [F2F.ReactiveNavigation.WPF](http://www.nuget.org/packages/F2F.ReactiveNavigation.WPF/)
- [F2F.ReactiveNavigation.Autofac](http://www.nuget.org/packages/F2F.ReactiveNavigation.Autofac/)

## Basic interfaces ##

```csharp
public interface IObserveRegion
{
  IObservable<ReactiveViewModel> Added { get; }
  
  IObservable<ReactiveViewModel> Removed { get; }
  
  IObservable<ReactiveViewModel> Activated { get; }
}
```

```csharp
public interface INavigate
{
  Task RequestNavigate<TViewModel>(INavigationParameters parameters)
    where TViewModel : ReactiveViewModel;
  
  Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters);
  
  Task RequestClose<TViewModel>(INavigationParameters parameters)
    where TViewModel : ReactiveViewModel;
  
  Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters);
}
```

```csharp
public interface IRegionAdapter
{
	void Adapt(INavigableRegion region);
}
```

## Sample ##

As you see in `F2F.ReactiveNavigation.WPF.Sample` this framework works very well with IoC containers as [Autofac](https://github.com/autofac/Autofac) for controlling the lifetime of view, view model and it's dependencies.

The following code shows a sample bootstrapper. We use a **MenuBuilder** for adding an entry to a **Menu**. Furthermore we use a **TabRegionAdapter** for adapting the view models of a **MultiItemsRegion** to a **TabControl**.

```csharp
internal class Bootstrapper : AutofacBootstrapper
{
	public override void Run()
	{
		base.Run();

		RegisterViewModels(GetType().Assembly);

		var regionContainer = Container.Resolve<IRegionContainer>();
		var tabRegion = regionContainer.CreateMultiItemsRegion();

		var shell = new MainWindow();
		var menuBuilder = new MenuBuilder(shell.MainMenu);
		menuBuilder.AddMenuItem("Add",
			() => tabRegion.RequestNavigate<SampleViewModel>(NavigationParameters.Empty);

		var viewFactory = Container.Resolve<ICreateView>();
		var tabRegionAdapter = Scope.From(new TabRegionAdapter(viewFactory, shell.TabRegion));
		tabRegion.Adapt(tabRegionAdapter);

		Application.Current.MainWindow = shell;
		shell.Show();
	}
}
```
