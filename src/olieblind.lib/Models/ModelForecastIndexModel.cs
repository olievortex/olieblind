using olieblind.data.Entities;

namespace olieblind.lib.Models;

public class ModelForecastIndexModel
{
    public HeaderModel Header { get; set; } = new();
    public List<ItemModel> Items { get; set; } = [];

    public ItemModel? GetItemByParameterId(int parameterId)
    {
        return Items.FirstOrDefault(i => i.ParameterId == parameterId);
    }

    public class HeaderModel
    {
        public int Id { get; set; }
        public DateTime Effective { get; set; }
        public int ForecastHour { get; set; }

        public static HeaderModel Map(ProductMapEntity entity)
        {
            return new HeaderModel
            {
                Id = entity.Id,
                Effective = entity.Effective,
                ForecastHour = entity.ForecastHour
            };
        }
    }

    public class ItemModel
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int ParameterId { get; set; }

        public static ItemModel Map(ProductMapItemEntity entity)
        {
            return new ItemModel
            {
                Url = entity.Url,
                Title = entity.Title,
                ParameterId = entity.ParameterId
            };
        }
    }
}
