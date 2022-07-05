using System.Collections.Generic;
using System.Linq;
using CriticalCommonLib;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Filters.Abstract;

namespace InventoryTools.Logic.Filters
{
    public class PurchasedWithCurrencyFilter : UintMultipleChoiceFilter
    {
        public override string Key { get; set; } = "PurchaseWithCurrency";
        public override string Name { get; set; } = "Purchased with Currency";

        public override string HelpText { get; set; } =
            "Filter items based on the currency they can be purchased with.";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Acquisition;

        public override FilterType AvailableIn { get; set; } =
            FilterType.SearchFilter | FilterType.SortingFilter | FilterType.GameItemFilter;
        
        public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            var currentValue = CurrentValue(configuration);
            if (currentValue.Count == 0)
            {
                return null;
            }
            return currentValue.Any(currencyItem => Service.ExcelCache.BoughtWithCurrency(currencyItem, item.ItemId));
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemEx item)
        {
            var currentValue = CurrentValue(configuration);
            if (currentValue.Count == 0)
            {
                return null;
            }
            return currentValue.Any(currencyItem => Service.ExcelCache.BoughtWithCurrency(currencyItem, item.RowId));
        }

        public override Dictionary<uint, string> GetChoices(FilterConfiguration configuration)
        {
            var currencies = Service.ExcelCache.GetCurrencies(3);
            return currencies.ToDictionary(c => c, c => Service.ExcelCache.GetSheet<ItemEx>().GetRow(c)?.Name.ToString() ?? "Unknown").OrderBy(c => c.Value).ToDictionary(c => c.Key, c => c.Value);
        }

        public override bool HideAlreadyPicked { get; set; } = true;
    }
}