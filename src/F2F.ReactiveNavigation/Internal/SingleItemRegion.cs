using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation.Internal
{
    /// <summary>
    /// A single item region does not cache the view models that are added to it. When a view model is added to the region
    /// and the region already contains a view model, the existing view model is removed before the new one is added.
    /// </summary>
    /// <remarks>
    /// Probably in 99% of the cases you want to use a MultiItemsRegion. A SingleItemRegion is really only useful when
    /// you want to completely get rid of the view model and view everytime you navigate to it.
    /// </remarks>
    internal class SingleItemRegion : Region
    {
        public SingleItemRegion(ICreateViewModel viewModelFactory)
            : base(viewModelFactory)
        {
        }

        protected internal override void Adding<TViewModel>(TViewModel itemToBeAdded)
        {
            if (ViewModels.Any())
            {
                Remove(ViewModels.Single());
            }
        }
    }
}