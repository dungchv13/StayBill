using System.Text.Json;
using System.Text.Json.Serialization;
using StayBill.Api.Contracts;
using StackExchange.Redis;

namespace StayBill.Api.Cache;

public sealed class RedisVacantRoomCache : IVacantRoomCache
{
    public const string Key = "rooms:vacant";
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisVacantRoomCache> _logger;

    public RedisVacantRoomCache(IConnectionMultiplexer redis, ILogger<RedisVacantRoomCache> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RoomResponse>?> GetAsync(CancellationToken ct)
    {
        try
        {
            var value = await _redis.GetDatabase().StringGetAsync(Key);
            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<RoomResponse>>(value!, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read vacant-room cache");
            return null;
        }
    }

    public async Task SetAsync(IReadOnlyList<RoomResponse> rooms, CancellationToken ct)
    {
        try
        {
            var json = JsonSerializer.Serialize(rooms, JsonOptions);
            await _redis.GetDatabase().StringSetAsync(Key, json, Ttl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write vacant-room cache");
        }
    }

    public async Task InvalidateAsync(CancellationToken ct)
    {
        try
        {
            await _redis.GetDatabase().KeyDeleteAsync(Key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to invalidate vacant-room cache");
        }
    }
}
